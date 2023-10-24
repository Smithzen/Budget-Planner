using System.Security.Cryptography;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Mail;
using MySqlConnector;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Budget_Planner.BudgetPlanner.Data;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    {
        //how to hash a password and store as b64 in database
        //https://stackoverflow.com/questions/4181198/how-to-hash-a-password

        private static string UserGUID { get; set; } = string.Empty;
        public string UserLoginToken { get; set; } = string.Empty;
        public string UserLoginTokenGUID { get; set; } = string.Empty;
        private string UserEncryptionKey { get; set; } = string.Empty; //base64 of aes.generatekey() used when creating account
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        private byte[] EmailEncryptionKey { get; set; } = 
        {
            0x04, 0x02, 0x01, 0x04, 0x05, 0x07, 0x07, 0x08,
            0x09, 0x10, 0x14, 0x12, 0x13, 0x12, 0x15, 0x16
        };
        private MySqlConnectionStringBuilder builder { get; } = new MySqlConnectionStringBuilder
        {
            Server = "192.168.1.127",
            UserID = "myuser",
            Password = "mypass",
            Database = "budget_planner",
        };

        public string tempUserGUID { get; set; } = "9d2afa96-d020-44e9-aa53-ff5753d5f08e";




        //Account Creation
        public bool AuthIsEmailNew(string userEmailEncrypted)
        {
            int userCount = 0;
            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;

                //get all users where that email already exists
                cmd = new MySqlCommand("SELECT UserGUID FROM bpauth WHERE UserEmail=@UserEmail", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserEmail", MySqlDbType = MySqlDbType.VarChar, Value = userEmailEncrypted });
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //add to list to check how many
                    userCount++;
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AddNewExpense Ex: " + ex.Message);
                return false;

            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }

            //check if any users were returned
            if (userCount > 0)
            {
                Debug.WriteLine("AuthIsEmailNew userCount: " + userCount);
                return false;
            }
            else
            {
                return true;
            }
        }
        public BPServerResult AuthCreateAccount(string userEmail, string userPassword)
        {
            BPServerResult bpServerResult = new BPServerResult();
            BPApplication bpApplication = new BPApplication();

            //check email and password are ok and email has not been used before
            bool bIsEmailAvailable = false;
            string userEmailEncrypted = string.Empty;
            if (AuthIsValidEmail(userEmail))
            {

                //create userGUID
                BPApplication.UserGUID = Guid.NewGuid().ToString();

                //encrypt email
                userEmailEncrypted = AuthEncryptString(EmailEncryptionKey, userEmail.ToLower());

                //check if email has been used before
                bIsEmailAvailable = AuthIsEmailNew(userEmailEncrypted);

            }
            else { bpServerResult.ServerResult = false; bpServerResult.ServerResultMessage = "Email is not valid"; }

            if (bIsEmailAvailable)
            {
                if(!string.IsNullOrEmpty(userEmailEncrypted))
                {
                    bpApplication.UserEmail = userEmailEncrypted;
                }

                //hash password
                var salt = AuthGenerateSalt();
                string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(userPassword, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8));
                Debug.WriteLine("AuthCreateAccount hash: " + hash);
                string saltB64 = Convert.ToBase64String(salt);
                string passwordSaltHash = saltB64 + "," + hash;
                Debug.WriteLine("AuthCreateAccount passwordSaltHash: " + passwordSaltHash);

                //generate userLoginToken and EncryptionKey
                Aes aes = Aes.Create();
                bpApplication.UserLoginToken = Guid.NewGuid().ToString();
                bpApplication.UserLoginTokenGUID = Guid.NewGuid().ToString();
                Debug.WriteLine("AuthCreateAccount userLoginToken: " + bpApplication.UserLoginToken);
                string UserEncryptionKey = Convert.ToBase64String(aes.Key);



                //insert all data to sql
                MySqlConnection DBConM = new MySqlConnection(builder.ConnectionString);
                try
                {
                    DBConM.Open();

                    MySqlCommand cmd;
                    int intResult = 0;

                    cmd = new MySqlCommand("INSERT INTO bpauth (UserGUID, UserEmail, UserPasswordHash, UserEncryptionKey, UserLoginToken, UserLoginTokenGUID) VALUE (@UserGUID, @UserEmail, @UserPasswordHash, @UserEncryptionKey, @UserLoginToken, @UserLoginTokenGUID)", DBConM);
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = BPApplication.UserGUID });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserEmail", MySqlDbType = MySqlDbType.VarChar, Value = bpApplication.UserEmail });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserPasswordHash", MySqlDbType = MySqlDbType.VarChar, Value = passwordSaltHash });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserEncryptionKey", MySqlDbType = MySqlDbType.VarChar, Value = UserEncryptionKey });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserLoginToken", MySqlDbType = MySqlDbType.VarChar, Value = bpApplication.UserLoginToken });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserLoginTokenGUID", MySqlDbType = MySqlDbType.VarChar, Value = bpApplication.UserLoginTokenGUID });
                    intResult = cmd.ExecuteNonQuery();

                    if (intResult > 0)
                    {
                        //if insert is successfult write LoginToken and encrypted email to local file and encrypt login token
                        string userLoginTokenEncrypted = AuthEncryptString(aes.Key, bpApplication.UserLoginToken);
                        Debug.WriteLine("AuthCreateAccount userLoginTokenEncrypted: " + userLoginTokenEncrypted);

                        bool bTokenCreated = AuthWriteUserLoginTokenToLocalDevice(userLoginTokenEncrypted, bpApplication.UserLoginTokenGUID).Result;
                        if (bTokenCreated)
                        {
                            bpServerResult.ServerResult = true;
                            bpServerResult.ServerResultMessage = "Account successfully created";
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("AuthCreateAccount Ex: " + ex.Message);
                    bpServerResult.ServerResult = false;
                    bpServerResult.ServerResultMessage = ex.Message;
                }
                finally
                {
                    DBConM.Close();
                    DBConM.Dispose();
                }



            }
            else { bpServerResult.ServerResult = false; bpServerResult.ServerResultMessage = "Email is not available"; }

            return bpServerResult;


        }
        private static byte[] AuthGenerateSalt(int maxSaltLength = 32)
        {
            var salt = new byte[maxSaltLength];
            var random = RandomNumberGenerator.Create();
            random.GetNonZeroBytes(salt);

            return salt;
        }
        public async Task<bool> AuthWriteUserLoginTokenToLocalDevice(string userLoginTokenEncrypted, string userLoginTokenGUID)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, "BPData.json");
            Console.WriteLine(Path.Exists(filePath));

            try
            {
                string BPAuthJson = JsonConvert.SerializeObject(userLoginTokenGUID + userLoginTokenEncrypted);
                Debug.WriteLine("AuthWriteUserLoginTokenToDevice userLoginTokenEncrypted: " + userLoginTokenEncrypted);

                using (var fileStream = File.Create(filePath))
                {
                    Console.WriteLine(filePath);
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        await writer.WriteAsync(BPAuthJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthWriteUserLogintokenToDevice Ex: " + ex.Message);
                return false;
            }

            return true;

        }


        //Account Login
        public BPServerResult AuthBackgroundlogin()
        {
            BPServerResult serverResult = new BPServerResult();

            //if there is an existing UserGUID GetUserGUID()
            string userLoginToken = null;

            if (AuthCheckForExistingLocalBPDataFile())
            {
                userLoginToken = AuthGetUserLoginToken();
                userLoginToken = userLoginToken.Remove(36);
            } 
            else
            {
                serverResult.ServerResult = false;
                serverResult.ServerResultMessage = "No local login token found";
                return serverResult;
            }

            //check if it matches an known user in database
            //true = set UserGUID equal to retrieved UserGUID
            //false = request manual login

            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;
                int userCount = 0;

                cmd = new MySqlCommand("SELECT UserGUID FROM bpauth WHERE UserLoginToken=@UserLoginToken", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserLoginToken", MySqlDbType = MySqlDbType.VarChar, Value = userLoginToken });
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    UserGUID = reader["UserGUID"].ToString();
                    //increase userCount so as to check only 1 user was returned
                    userCount++;
                }
                reader.Close();
                reader.Dispose();

                if (userCount != 1)
                {
                    serverResult.ServerResultMessage = "Invalid login token";
                    serverResult.ServerResult = false;
                    return serverResult;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthBackgroundLogin Ex: " + ex.Message);
            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }

            serverResult.ServerResult = true;
            serverResult.ServerResultMessage = "Successfully logged in";
            return serverResult;


            //hashing passwords
            //https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-7.0
        }
        private bool AuthCheckForExistingLocalBPDataFile(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(FileSystem.CacheDirectory, "BPData.json");
            }
            else
            {
                path = Path.Combine(path, "BPData.json");
            }

            var result = Path.Exists(path) ? true : false;
            return result;
        }
        public string AuthGetUserLoginToken(string path = null)
        {
            //encrypting data and decrypting data to be used for emails passwords and user ids
            //user ids to be stored in local json file to be collected when app opens or login/account creation will be requested
            //https://learn.microsoft.com/en-us/dotnet/standard/security/encrypting-data

            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(FileSystem.CacheDirectory, "BPData.json");
            }
            else
            {
                path = Path.Combine(path, "BPData.json");
            }


            //retrieve encrypted hash string from local json file
            string jsonData = null;
            string userData = null;
            string userLoginTokenGUID = null;
            string userLoginTokenEncrypted = null;
            try
            {
                using (var readStream = File.OpenRead(path))
                {
                    using (StreamReader reader = new StreamReader(readStream))
                    {
                        jsonData = reader.ReadToEnd();
                        Console.WriteLine(jsonData);
                    }
                }

                userData = JsonConvert.DeserializeObject<string>(jsonData);
                userLoginTokenGUID = userData.Remove(36);
                userLoginTokenEncrypted = userData.Remove(0, 36);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthGetUserLoginToken" + ex.Message);
            }

            //decrypt id string
            string userLoginTokenDecrypted = null;

            if (string.IsNullOrEmpty(userLoginTokenGUID))
                throw new ArgumentNullException("userLoginTokenGUID cannot be null");



            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                //get decryption key from DB
                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;
                string userEncryptionKey = null;

                cmd = new MySqlCommand("SELECT UserEncryptionKey FROM bpauth WHERE UserLoginTokenGUID=@UserLoginTokenGUID", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName="@UserLoginTokenGUID", MySqlDbType=MySqlDbType.VarChar, Value=userLoginTokenGUID });
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    userEncryptionKey = reader["UserEncryptionKey"].ToString();
                }
                reader.Close();
                reader.Dispose();

                //decrypt string using encryption key retrieved
                userLoginTokenDecrypted = AuthDecryptString(Convert.FromBase64String(userEncryptionKey), userLoginTokenEncrypted);



            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthGetUserLoginToken Ex: " + ex.Message);
            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }


            //return decrypted string
            return userLoginTokenDecrypted;

        }
        public BPServerResult AuthLogin(string userEmail, string userPassword)
        {
            BPServerResult result = new BPServerResult();

            //check user email is valid email
            if(AuthIsValidEmail(userEmail))
            {
                //get list of emails from db
                //if email matches an email in the database check password
                string userLoginTokenGUID = AuthEmailExistsInDB(userEmail.ToLower());

                if(userLoginTokenGUID == null)
                {
                    result.ServerResult = false;
                    result.ServerResultMessage = "Incorrect email";
                }
                else
                {
                    string passwordHash = AuthGetPasswordHashFromDB(userLoginTokenGUID);
                    string[] passwordSaltHash = passwordHash.Split(",");
                    byte[] passwordHashBytes = Convert.FromBase64String(passwordSaltHash[1]);
                    byte[] salt = Convert.FromBase64String(passwordSaltHash[0]);
                    if (salt.Length != 32)
                    {
                        result.ServerResult = false;
                        result.ServerResultMessage = "incorrect salt";
                    }

                    //hash password and check if it matches those in database
                    byte[] hashEnteredPassword = KeyDerivation.Pbkdf2(userPassword, salt, KeyDerivationPrf.HMACSHA256, 10000, 256 / 8);
                    if (AuthByteArraysAreEqual(passwordHashBytes, hashEnteredPassword))
                    {
                        MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
                        try
                        {
                            DBConRO.Open();

                            MySqlCommand cmd;
                            MySqlDataReader reader;
                            int resultCount = 0;
                            string userLoginToken = null;
                            string userEncryptionKey = null;

                            cmd = new MySqlCommand("SELECT UserGUID, UserLoginToken, UserEncryptionKey FROM bpauth WHERE UserLoginTokenGUID=@UserLoginTokenGUID", DBConRO);
                            cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserLoginTokenGUID", MySqlDbType = MySqlDbType.VarChar, Value = userLoginTokenGUID });
                            reader = cmd.ExecuteReader();

                            while(reader.Read())
                            {
                                //if email and password match then set userGUID to be correct user guid and set server result to true
                                UserGUID = reader["UserGUID"].ToString();
                                userEncryptionKey = reader["UserEncryptionKey"].ToString();
                                userLoginToken = reader["UserLoginToken"].ToString();
                                resultCount++;
                            }
                            reader.Close();
                            reader.Dispose();

                            if (resultCount != 1)
                            {
                                result.ServerResult = false;
                                result.ServerResultMessage = "Login Token did not match a unique user";
                            }


                            //if successful then write logintoken to device using same function as on account creation
                            if(!AuthCheckForExistingLocalBPDataFile())
                            {
                                string userLoginTokenEncrypted = AuthEncryptString(Convert.FromBase64String(userEncryptionKey), userLoginToken);
                                if(AuthWriteUserLoginTokenToLocalDevice(userLoginTokenEncrypted, userLoginTokenGUID)    .Result)
                                {
                                    result.ServerResult = true;
                                    result.ServerResultMessage = "Login Successful! Token Created";
                                }

                            }
                            else
                            {
                                result.ServerResult = true;
                                result.ServerResultMessage = "Login Successful!";
                            }

                        }
                        catch(Exception ex)
                        {
                            Debug.WriteLine("AuthLogin Ex: " + ex.Message);
                            result.ServerResult = false;
                            result.ServerResultMessage = ex.Message;
                        }
                        finally
                        {
                            DBConRO.Close();
                            DBConRO.Dispose();
                        }
                    }
                    else
                    {
                        //set incorrect password serverResult
                        result.ServerResult = false;
                        result.ServerResultMessage = "Email or password is incorrect";
                    }

                }

            }
            else
            {
                result.ServerResult = false;
                result.ServerResultMessage = "Invalid email address";
            }

            return result;
        }
        private string AuthEmailExistsInDB(string userEmail)
        {
            string acceptedEmail = null;
            string userLoginTokenGUID = null;
            MySqlConnection DBconRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBconRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;

                cmd = new MySqlCommand("SELECT UserEmail, UserLoginTokenGUID FROM bpauth", DBconRO);
                reader = cmd.ExecuteReader();

                string emailEncrypted;

                while (reader.Read())
                {
                    //foreach email decrypt and check if identical to userEmail
                    emailEncrypted = reader["UserEmail"].ToString();
                    string emailDecrypted = AuthDecryptString(EmailEncryptionKey, emailEncrypted);
                    emailDecrypted = emailDecrypted.Remove(emailDecrypted.Length-1);
                    if (userEmail == emailDecrypted)
                    {
                        acceptedEmail = userEmail;
                        userLoginTokenGUID = reader["UserLoginTokenGUID"].ToString();
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                reader.Close();
                reader.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthLogin Ex: " + ex.Message);
                return userLoginTokenGUID = null;
            }
            finally
            {
                DBconRO.Close();
                DBconRO.Dispose();
            }

            if (acceptedEmail != null)
            {
                return userLoginTokenGUID;
            }
            else
            {
                userLoginTokenGUID = null;
                return userLoginTokenGUID;
            }

        }
        private string AuthGetPasswordHashFromDB(string userLoginTokenGUID) 
        {
            string passwordHash = null;
            MySqlConnection DBConRO = new MySqlConnection(builder.ConnectionString);
            try
            {
                DBConRO.Open();

                MySqlCommand cmd;
                MySqlDataReader reader;
                int resultCount = 0;

                cmd = new MySqlCommand("SELECT UserPasswordHash FROM bpauth WHERE UserLoginTokenGUID=@UserLoginTokenGUID", DBConRO);
                cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserLoginTokenGUID", MySqlDbType = MySqlDbType.VarChar, Value = userLoginTokenGUID });
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    passwordHash = reader["UserPasswordHash"].ToString();
                    resultCount++;
                }
                reader.Close();
                reader.Dispose();

                if (resultCount != 1)
                {
                    passwordHash = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthGetPasswordHashFromDB Ex: " + ex.Message);
                passwordHash = null;
            }
            finally
            {
                DBConRO.Close();
                DBConRO.Dispose();
            }

            return passwordHash;
        }


        //Useful functions
        private string AuthEncryptString(byte[] UserEncryptionKey, string text)
        {
            string textEncrypted = string.Empty;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = UserEncryptionKey;
                        byte[] iv = aes.IV;
                        memoryStream.Write(iv, 0, iv.Length);

                        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new(cryptoStream))
                            {
                                writer.WriteLine(text);
                            }
                        }

                        textEncrypted = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                Debug.WriteLine("AuthEncryptString textEncrypted: " + textEncrypted);

                return textEncrypted;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthEncryptString Ex: " + ex.Message);
                return null;
            }
        }
        private string AuthDecryptString(byte[] encryptionKey, string textEncrypted)
        {
            string textDecrypted = null;

            try
            {
                using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(textEncrypted)))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = memoryStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(encryptionKey, iv), CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new(cryptoStream))
                            {
                                textDecrypted = reader.ReadToEnd();
                            }
                        }

                        Debug.WriteLine("AuthDecryptString textDecrypted: " + textDecrypted);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthDecryptString Ex: " + ex.Message);
            }

            return textDecrypted;

        }
        private static bool AuthIsValidEmail(string userEmail)
        {
            bool bValidEmail = true;
            try
            {
                MailAddress mail = new MailAddress(userEmail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AuthCreateAccount Ex: " + ex.Message);
                bValidEmail = false;
            }

            return bValidEmail;
        }
        private bool AuthByteArraysAreEqual(byte[] a, byte[] b)
        {
            bool result = false;

            if (a.Length == b.Length)
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] == b[i])
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }

            }

            return result;

        }





        //template for encrypting and decrypting string
        public void EncryptUserGUID()
        {
            string UserGUIDEncrypted = string.Empty;

            try
            {

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (Aes aes = Aes.Create())
                    {

                        byte[] iv = aes.IV;
                        memoryStream.Write(iv, 0, iv.Length);

                        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            using (StreamWriter encryptWriter = new(cryptoStream))
                            {
                                encryptWriter.WriteLine(tempUserGUID);
                            }
                        }

                        UserGUIDEncrypted = Convert.ToBase64String(memoryStream.ToArray());

                    }
                }

                Debug.WriteLine(UserGUIDEncrypted);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GenerateEncryptedUserGUID Ex: " + ex.Message);
            }


            try
            {
                string UserGUIDDecrypted = string.Empty;

                using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(UserGUIDEncrypted)))
                {
                    using (Aes aes = Aes.Create())
                    {
                        byte[] iv = new byte[aes.IV.Length];
                        int numBytesToRead = aes.IV.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = memoryStream.Read(iv, numBytesRead, numBytesToRead);
                            if (n == 0) break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }

                        byte[] key =
                        {
                            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                        };


                        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new(cryptoStream))
                            {
                                UserGUIDDecrypted = reader.ReadToEnd();
                            }
                        }

                        Debug.WriteLine(UserGUIDDecrypted);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("EncryptUserGUID Ex: " + ex.Message);
            }
        }




    }
}
