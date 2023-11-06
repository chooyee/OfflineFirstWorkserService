namespace Factory.Crypto
{
    public class CipherString:Cipher
    {
        private readonly string secretMsg;

        public CipherString(string msg) {
            secretMsg = EncryptString(msg);
        }

        
        public string GetEncryptedMsg() { return secretMsg; } 
        public string GetDecryptedMsg() { return DecryptString(secretMsg); }
    }
}
