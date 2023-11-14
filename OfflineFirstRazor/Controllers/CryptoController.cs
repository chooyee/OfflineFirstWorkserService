using Factory;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    public class CryptoController : ControllerBase
    {
        [HttpPost, Route("/crypto/encrypt")]
        public string Encrypt(string msg)
        {
            return Cryptolib2.Crypto.EncryptText(msg);
        }
        [HttpPost, Route("/crypto/decrypt")]
        public string Decrypt(string msg)
        {
            return Cryptolib2.Crypto.DecryptText(msg);
        }
    }
}
