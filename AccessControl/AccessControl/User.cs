﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;

namespace AccessControl
{
    public class User
    {
        public string UserName { get; set; }
        public string Password_PlainText { get; set; }
        public string Password_Hash { get; set; }
        public string Password_SaltedHash { get; set; }
        public string Password_SaltedHashSlow { get; set; }
        public string Salt { get; set; }


        public User (string _UserName, string _Password)
        {
            UserName = _UserName;
            Password_PlainText = _Password;
        }

        public User ()
        {}

        public void AddUser()
        {
            //Apliquem hash
            this.Password_Hash = sha512Conversion(this.Password_PlainText);

            //Apliquem hash+salt
            this.Salt = saltGeneration();
            this.Password_SaltedHash = sha512Conversion(this.Password_PlainText) + this.Salt;

            //Apliquem hash+salt amb algorisme de hash lent.
            this.Salt = BytesToStringHex(hashByteConversion());
            this.Password_SaltedHashSlow = slowHashConvertion(this.Password_PlainText, Encoding.UTF8.GetBytes(this.Salt));

            ((App)Application.Current).Database.Add(this);            
        }

        public bool Validate (string _UserName, string _Password)
        {
            User MyUser = ((App)Application.Current).Database.Find(User => User.UserName == _UserName);

            //Validate amb Text pla

            /*if (!ReferenceEquals(MyUser, null))
            {
                if (MyUser.Password_PlainText.Equals(_Password))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }*/

            //Validate amb Hash (comenta l'anterior validació)

            // VALIDATION OK

            /*if (!ReferenceEquals(MyUser, null))
            {
                String passwordCheck = sha512Conversion(_Password);

                if (MyUser.Password_Hash.Equals(passwordCheck))
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }*/


            //Validate amb Hash i salt (comenta l'anterior validació)

            // VALIDATION OK

            /*if (!ReferenceEquals(MyUser, null))
            {
                String fullPasswordCheck = sha512Conversion(_Password) + MyUser.Salt;

                if (MyUser.Password_SaltedHash.Equals(fullPasswordCheck))
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }*/

            //Validate amb Hash slow i salt. Pots utilitzar la classe Rfc2898DeriveBytes

            // VALIDATION OK

            if (!ReferenceEquals(MyUser, null))
            {
                String fullPasswordCheck = slowHashConvertion(_Password, Encoding.UTF8.GetBytes(MyUser.Salt));

                if (MyUser.Password_SaltedHashSlow.Equals(fullPasswordCheck))
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                return false;
            }
        }

        string BytesToStringHex (byte[] result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in result)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }

        string sha512Conversion (string plainText)
        {
            var data = Encoding.UTF8.GetBytes(plainText);
            byte[] hash;

            using (SHA512 sha512Instance = new SHA512Managed())
            {
                hash = sha512Instance.ComputeHash(data);
            }

            return BytesToStringHex(hash);
        }

        string saltGeneration ()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[1024];

            rng.GetBytes(buffer);
            string salt = BytesToStringHex(buffer);

            return salt;
        }

        string slowHashConvertion(string plainText, byte[] saltBytes)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(plainText, saltBytes, 1500);
            return BytesToStringHex(deriveBytes.GetBytes(128));
        }

        byte[] hashByteConversion()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[1024];
            rng.GetBytes(buffer);

            return buffer;
        }
    }

}
