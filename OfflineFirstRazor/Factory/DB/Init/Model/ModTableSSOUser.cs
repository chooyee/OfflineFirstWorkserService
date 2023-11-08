using Factory.DB;

namespace Model
{
    [SqlTable("tbl_sso")]
    internal class ModTableSSOUser
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        internal int Id { get; set; }

        [SqlProperty("username", DataType.TEXT)]
        internal string UserName { get; set; }

        [SqlProperty("token", DataType.TEXT)]
        internal string Token { get; set; }

        [SqlProperty("expiryDate", DataType.DATETIME)]
        internal string ExpiryDate { get; set; }

        [SqlProperty("logDate", DataType.DATETIME)]
        internal string LogDate { get; set; }

        internal ModTableSSOUser(string username, string token, DateTime expiryDate)
        {
            UserName = username;
            Token = token;
            ExpiryDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", expiryDate);
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
        }

    }


    [SqlTable("tbl_audit_log")]
    internal class ModTableAuditLog
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        internal int Id { get; set; }

        [SqlProperty("username", DataType.TEXT)]
        internal string UserName { get; set; }

        [SqlProperty("action", DataType.TEXT)]
        internal string Action { get; set; }

        [SqlProperty("action_desc", DataType.TEXT)]
        internal string ActionDesc { get; set; }

        [SqlProperty("logDate", DataType.DATETIME)]
        internal string LogDate { get; set; }

        public ModTableAuditLog() { }

        public ModTableAuditLog(string username, string action, string actionDesc) { 
            UserName = username;
            Action = action;
            ActionDesc = actionDesc;
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

        }


    }

    [SqlTable("tbl_machine_log")]
    internal class ModTableMachineLog
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        internal int Id { get; set; }

        [SqlProperty("machineId", DataType.TEXT)]
        internal string MachineId { get; set; }

        [SqlProperty("fingerprint", DataType.TEXT)]
        internal string Fingerprint { get; set; }
       

        [SqlProperty("logDate", DataType.DATETIME)]
        internal string LogDate { get; set; }

      

        public ModTableMachineLog(string machineId, string fingerprint)
        {
            MachineId = machineId;
            Fingerprint = fingerprint;
            LogDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

        }


    }
}
