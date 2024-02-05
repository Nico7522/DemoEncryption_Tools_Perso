using DemoEncryption.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Tools.Cryptography;

namespace DemoEncryption
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ////Generate a public/private key pair.  
            //RSA rsa = RSA.Create(2048);
            //string privateKey = rsa.ExportRSAPrivateKeyPem();
            //Console.WriteLine(privateKey);

            //string publicKey = rsa.ExportRSAPublicKeyPem();
            //Console.WriteLine(publicKey);

            ////Pour encrypter
            //rsa = RSA.Create();
            //rsa.ImportFromPem(publicKey);
            //byte[] data = Encoding.Default.GetBytes("Hello World!");
            //byte[] cypher = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA512);


            ////pour décrypter
            //rsa = RSA.Create();
            //rsa.ImportFromPem(privateKey);
            //data = rsa.Decrypt(cypher, RSAEncryptionPadding.OaepSHA512);
            //Console.WriteLine(Encoding.Default.GetString(data));

            //ICryptoRSA cryptoRSA = new CryptoRSA(KeySizes.S4096);
            //byte[] cypher = Encrypt(cryptoRSA.PublicKey, "Hello Salle 2");
            //Console.WriteLine(Decrypt(cryptoRSA.PrivateKey, cypher));


            // Récupération d'une clée
            string? publicKey = null;
            using(HttpClient _client = new HttpClient())
            {
                _client.BaseAddress = new Uri("https://localhost:7273/api/");
                using (HttpResponseMessage response = _client.GetAsync("Security/Security").Result) 
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();   
                        string json = response.Content.ReadAsStringAsync().Result;
                        PublicKeyInfo key = JsonSerializer.Deserialize<PublicKeyInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

                        if (key is null)
                            throw new Exception();
                       
                      publicKey = Encoding.Default.GetString(key.PublicKey);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }


            // Register
            //using (HttpClient _client = new HttpClient())
            //{
            //    _client.BaseAddress = new Uri("https://localhost:7273/api/");
            //    ICryptoRSA rsa = new CryptoRSA(publicKey);
            //    string passwd = "Test12345=";
            //    HttpContent httpContent = JsonContent.Create(new { Nom = "Nico", Prenom = "D", Email = "nn.d@gmail.com", Passwd = rsa.Encrypt(passwd) });
            //    using (HttpResponseMessage response = _client.PostAsync("Auth/Register", httpContent).Result)
            //    {
            //        response.EnsureSuccessStatusCode();
            //    }
            //}

            using (HttpClient _client = new HttpClient())
            {
                _client.BaseAddress = new Uri("https://localhost:7273/api/");
                ICryptoRSA rsa = new CryptoRSA(publicKey);
                string passwd = "Test12345=";
                HttpContent httpContent = JsonContent.Create(new { Email = "nn.d@gmail.com", Passwd = rsa.Encrypt(passwd) });
                using (HttpResponseMessage response = _client.PostAsync("Auth/Login", httpContent).Result)
                {
                    try
                    {
                    response.EnsureSuccessStatusCode();
                        Console.WriteLine(response.Content);
                    }
                    catch (HttpRequestException ex)
                    {

                        Console.WriteLine(ex.Message);
                    }
                }
            }

        }

        static byte[] Encrypt(string publicKey, string text)
        {
            ICryptoRSA cryptoRSA = new CryptoRSA(publicKey);
            return cryptoRSA.Encrypt(text);
        }

        static string Decrypt(string privateKey, byte[] cypher)
        {
            ICryptoRSA cryptoRSA = new CryptoRSA(privateKey);
            return cryptoRSA.Decrypt(cypher);
        }
    }
}
