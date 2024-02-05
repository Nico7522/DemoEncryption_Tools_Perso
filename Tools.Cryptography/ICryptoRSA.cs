namespace Tools.Cryptography
{
    public interface ICryptoRSA
    {
        string PrivateKey { get; }
        string PublicKey { get; }

        string Decrypt(byte[] cypher);
        byte[] Encrypt(string data);
    }
}