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
        public string UserGUID { get; set; } = string.Empty;
        public string UserLoginToken { get; set; } = string.Empty;
        private string UserEncryptionKey { get; set; } = string.Empty; //base64 of aes.generatekey() used when creating account
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        private byte[] EmailEncryptionKey { get; set; } = 
        {
            0x04, 0x02, 0x01, 0x04, 0x05, 0x07, 0x07, 0x08,
            0x09, 0x10, 0x14, 0x12, 0x13, 0x12, 0x15, 0x16
        };


        public string tempUserGUID { get; set; } = "9d2afa96-d020-44e9-aa53-ff5753d5f08e";

        //how to hash a password and store as b64 in database
        //https://stackoverflow.com/questions/4181198/how-to-hash-a-password

        private bool CheckForExistingUserGUID()
        {

            //check for local json file

            //check json for encrypted UserGUID

            return false;
        }

        public bool AuthIsEmailNew(string userEmailEncrypted)
        {

            var builder = new MySqlConnectionStringBuilder
            {
                Server = "192.168.1.127",
                UserID = "myuser",
                Password = "mypass",
                Database = "budget_planner",
            };

            List<BPApplication> listBPUsers = new List<BPApplication>();
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
                    listBPUsers.Add(new BPApplication()
                    {
                        UserGUID = reader["UserGUID"].ToString()
                    });
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
            if (listBPUsers.Count > 0)
            {
                Debug.WriteLine("AuthIsEmailNew listBPUsers.Count: " + listBPUsers.Count);
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
                bpApplication.UserGUID = Guid.NewGuid().ToString();

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
                string passwordSaltHash = saltB64 + hash;
                Debug.WriteLine("AuthCreateAccount passwordSaltHash: " + passwordSaltHash);

                //generate userLoginToken and EncryptionKey
                Aes aes = Aes.Create();
                bpApplication.UserLoginToken = Guid.NewGuid().ToString();
                Debug.WriteLine("AuthCreateAccount userLoginToken: " + bpApplication.UserLoginToken);
                string UserEncryptionKey = Convert.ToBase64String(aes.Key);



                //insert all data to sql
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "192.168.1.127",
                    UserID = "myuser",
                    Password = "mypass",
                    Database = "budget_planner",
                };

                MySqlConnection DBConM = new MySqlConnection(builder.ConnectionString);
                try
                {
                    DBConM.Open();

                    MySqlCommand cmd;
                    int intResult = 0;

                    cmd = new MySqlCommand("INSERT INTO bpauth (UserGUID, UserEmail, UserPasswordHash, UserEncryptionKey, UserLoginToken) VALUE (@UserGUID, @UserEmail, @UserPasswordHash, @UserEncryptionKey, @UserLoginToken)", DBConM);
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserGUID", MySqlDbType = MySqlDbType.VarChar, Value = bpApplication.UserGUID });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserEmail", MySqlDbType = MySqlDbType.VarChar, Value = bpApplication.UserEmail });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserPasswordHash", MySqlDbType = MySqlDbType.VarChar, Value = passwordSaltHash });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserEncryptionKey", MySqlDbType = MySqlDbType.VarChar, Value = UserEncryptionKey });
                    cmd.Parameters.Add(new MySqlParameter() { ParameterName = "@UserLoginToken", MySqlDbType = MySqlDbType.VarChar, Value = bpApplication.UserLoginToken });
                    intResult = cmd.ExecuteNonQuery();

                    if (intResult > 0)
                    {
                        //if insert is successfult write LoginToken and encrypted email to local file and encrypt login token
                        string userLoginTokenEncrypted = AuthEncryptString(aes.Key, bpApplication.UserLoginToken);
                        Debug.WriteLine("AuthCreateAccount userLoginTokenEncrypted: " + userLoginTokenEncrypted);

                        bool bTokenCreated = AuthWriteUserLoginTokenToLocalDevice(userLoginTokenEncrypted, bpApplication.UserGUID).Result;
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

        public void Backgroundlogin()
        {
            //if there is an existing UserGUID GetUserGUID()

            //check if it matches an known user in database
            //true = set UserGUID equal to retrieved UserGUID
            //false = request manual login

            //hashing passwords
            //https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-7.0
        }


        public void GetUserLoginToken()
        {
            //encrypting data and decrypting data to be used for emails passwords and user ids
            //user ids to be stored in local json file to be collected when app opens or login/account creation will be requested
            //https://learn.microsoft.com/en-us/dotnet/standard/security/encrypting-data



            //retrieve encrypted hash string from local json file;
            


            //decrypt id string


            //return decrypted string



        }

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



        public async Task<bool> AuthWriteUserLoginTokenToLocalDevice(string userLoginTokenEncrypted, string userGUID)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, "BPData.json");
            Console.WriteLine(Path.Exists(filePath));

            try
            {
                string BPAuthJson = JsonConvert.SerializeObject(userGUID + userLoginTokenEncrypted);
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



            using (var readStream = File.OpenRead(filePath))
            {
                using (StreamReader reader = new StreamReader(readStream))
                {
                    string json = reader.ReadToEnd();
                    Console.WriteLine(json);
                }
            }

            return true;

        }


    }
}
