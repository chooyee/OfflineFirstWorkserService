using System.Security;

namespace Model
{
    public struct EncryptResultModel
    {
        public string EncryptedKeyBase64Str { get; set; }
        public string EncryptedIVBase64Str { get; set; }
        public string EncryptedBase64Str { get; set; }


    }
}
