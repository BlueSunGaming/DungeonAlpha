using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCrypto
{
    public abstract class Crypter
    {
        protected string name;
        public string Name
        {
            get
            {
                return name;
            }
        }
        public abstract string Encrypt(string str, string args);
        public abstract string Decrypt(string str, string args);
    }
}

namespace nCrypto.Crypters
{
    public class Xor : nCrypto.Crypter
    {

        public Xor()
        {
            name = "XOR Encryption";
        }

        private string EncryptOrDecrypt(string text, string key)
        {
            var result = new StringBuilder();

            for (int c = 0; c < text.Length; c++)
                result.Append((char)((uint)text[c] ^ (uint)key[c % key.Length]));

            return result.ToString();
        }

        public override string Encrypt(string str, string args)
        {
            return EncryptOrDecrypt(str, args);
        }
        public override string Decrypt(string str, string args)
        {
            return EncryptOrDecrypt(str, args);
        }
    }
}