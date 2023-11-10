
using System.Security;

namespace Factory.DB.Model
{
    [SqlTable("tbl_sso")]
    public class ModTableSSOUser : IDBObjectBase
    {

        private string _refreshToken;

        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }

        [SqlPrimaryKey]
        [SqlProperty("username", DataType.TEXT)]
        public string UserName { get; set; }

        [SqlProperty("token", DataType.TEXT)]
        public string EncryptedRefreshToken {
            get { 
                return _refreshToken;
            } 
            set {
                _refreshToken = Crypto.Cipher.Instance.EncryptString(value);
            } 
        }

        [SqlProperty("expiryDate", DataType.DATETIME)]
        public string RefreshTokenExpireDate { get; set; }

        [SqlProperty("logDate", DataType.DATETIME)]
        public string LogDate { get; set; }

        public ModTableSSOUser() { }

        public ModTableSSOUser(string username)
        {
            UserName = username;
        }

        public ModTableSSOUser(string username, string refreshToken, DateTime refreshTokenExpireDate)
        {
            UserName = username;
            EncryptedRefreshToken = refreshToken;
            RefreshTokenExpireDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", refreshTokenExpireDate);
        }

        public string getDecryptedToken()
        {
            return Crypto.Cipher.Instance.DecryptString(_refreshToken);
        }

  

    }


    [SqlTable("tbl_audit_log")]
    public class ModTableAuditLog: IDBObjectBase
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }

        [SqlProperty("username", DataType.TEXT)]
        public string UserName { get; set; }

        [SqlProperty("action", DataType.TEXT)]
        public string Action { get; set; }

        [SqlProperty("action_desc", DataType.TEXT)]
        public string ActionDesc { get; set; }

        [SqlProperty("logDate", DataType.DATETIME)]
        public string LogDate { get; set; }

        public ModTableAuditLog() { }

        public ModTableAuditLog(string username, string action, string actionDesc)
        {
            UserName = username;
            Action = action;
            ActionDesc = actionDesc;
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

        }


    }

    [SqlTable("tbl_machine_log")]
    public class ModTableMachineLog: IDBObjectBase
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }


        [SqlProperty("fingerprint", DataType.TEXT)]
        public string Fingerprint { get; set; }


        [SqlProperty("logDate", DataType.DATETIME)]
        public string LogDate { get; set; }

        public ModTableMachineLog() { }

        public ModTableMachineLog(string fingerprint)
        {
            Fingerprint = fingerprint;
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

        }


    }
}
