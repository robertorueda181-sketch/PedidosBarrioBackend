namespace PedidosBarrio.Application.Services
{
    public interface IPiiEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
