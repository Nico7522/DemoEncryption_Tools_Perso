using System.Security.Cryptography;
using System.Text;

namespace Tools.Cryptography
{
    public class CryptoRSA : ICryptoRSA
    {
        private readonly RSA _cryptor;

        public CryptoRSA(KeySizes keySize = KeySizes.S2048)
        {
            _cryptor = RSA.Create((int)keySize);
        }

        public CryptoRSA(string pem)
        {
            _cryptor = RSA.Create();
            _cryptor.ImportFromPem(pem);
        }

        public string PrivateKey
        {
            get { return _cryptor.ExportRSAPrivateKeyPem(); }
        }

        public string PublicKey
        {
            get { return _cryptor.ExportRSAPublicKeyPem(); }
        }

        public byte[] Encrypt(string data)
        {
            byte[] bytes = Encoding.Default.GetBytes(data);
            return _cryptor.Encrypt(bytes, RSAEncryptionPadding.OaepSHA512);
        }

        public string Decrypt(byte[] cypher)
        {
            try
            {
                byte[] bytes = _cryptor.Decrypt(cypher, RSAEncryptionPadding.OaepSHA512);
                return Encoding.Default.GetString(bytes);
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException("Please check the keys!", ex);
            }
        }
    }
}
