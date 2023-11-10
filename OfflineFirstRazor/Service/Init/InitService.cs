using Factory;
using Factory.DB.Model;
using Serilog;
using System.Diagnostics;

namespace Service.Init
{
    internal sealed class InitService
    {
        internal async Task Init()
        {
            try
            {
                //init Crypto
                new Factory.Crypto.Keyman().InitKey();
                //init table
                Factory.DB.Init.InitDB.Init();
                //init fingerprint
                await RegisterDeviceId();
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task RegisterDeviceId()
        {
            try
            {
                var tableName = ReflectionFactory.GetTableAttribute(typeof(ModTableMachineLog));
                using var dbContext = new Factory.DB.DBContext();
                var countResult = await dbContext.ExecuteScalarAsync("select count(*) from " + tableName);
                if (int.Parse(countResult.ToString()) < 1)
                {
                    var cipher = new Factory.Crypto.Cipher();
                    var deviceId = cipher.EncryptString(Fingerprint.GenFingerprint());
                    var deviceId3 = cipher.DecryptString(deviceId);
                    var machineLog = new ModTableMachineLog(deviceId);

                    var result = dbContext.QueryFactory.Insert(machineLog);
                    await dbContext.ExecuteNonQueryAsync(result.Item1, result.Item2);
                }
            }
            catch (Exception ex)
            {
                var funcName = string.Format("{0} : {1}", new StackFrame().GetMethod().DeclaringType.FullName, System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log.Error("{funcName}: {error}", funcName, ex.Message);
                throw new Exception(ex.Message);
            }
           
        }
    }

   
}
