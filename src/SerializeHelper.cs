using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using System.Windows;

namespace accounts
{
    public static class SerializeHelper
    {
        static byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8 };  
                                                         
        static byte[] iv = { 1, 2, 3, 4, 5, 6, 7, 8 };
        static DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        public static void SerialilZe<T>(T data, string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var cryptoStream = new CryptoStream(fs, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // This is where you serialize the class
                formatter.Serialize(cryptoStream, data);

            }
        }
        public static T DeserialilZe<T>(string path)
        {
            T deserialized=default(T);
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var cryptoStream = new CryptoStream(fs, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();


                      deserialized = (T)formatter.Deserialize(cryptoStream);
                   
                }
            }
            catch(Exception er)
            {
                MessageBox.Show(er.Message.ToString());
                //MessageBox.Show("Wrong file format");
            } 
            return deserialized;
        }
       
    }
} 
