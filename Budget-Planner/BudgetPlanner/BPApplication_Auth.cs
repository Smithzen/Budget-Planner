﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;

namespace Budget_Planner.BudgetPlanner
{
    public partial class BPApplication
    {
        private string UserGUID { get; set; } = string.Empty;

        public string tempUserGUID { get; set; } = "9d2afa96-d020-44e9-aa53-ff5753d5f08e";

        //how to hash a password and store as b64 in database
        //https://stackoverflow.com/questions/4181198/how-to-hash-a-password

        private bool CheckForExistingUserGUID()
        {

            //check for local json file

            //check json for encrypted UserGUID

            return false;
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


        public void GetUserGUID()
        {
            //encrypting data and decrypting data to be used for emails passwords and user ids
            //user ids to be stored in local json file to be collected when app opens or login/account creation will be requested
            //https://learn.microsoft.com/en-us/dotnet/standard/security/encrypting-data



            //retrieve encrypted hash string from local json file;
            


            //decrypt id string


            //return decrypted string



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
                        byte[] key =
                        {
                            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
                            0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
                        };
                        aes.Key = key;

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
