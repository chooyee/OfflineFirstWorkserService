using System.Text;

namespace Factory.CouchbaseLiteFactory
{
    public class CouchbaseObjectId
    {
        private readonly static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        public static string CouchbaseObjectIdColumnName => "ID";

        protected string _couchObjectId { get; set; }

        public string ObjectId =>_couchObjectId;
        public long UnixTimestamp => ToUnixTimestamp();
        public string ToString()
        {
            return _couchObjectId;
        }

        public long ToUnixTimestamp()
        {
            return GetTimestampFromId(_couchObjectId);
        }

        public CouchbaseObjectId() {
            _couchObjectId = GenerateId();
        }

        public CouchbaseObjectId(long unixTimestamp) {
            _couchObjectId = GenerateId(unixTimestamp);
        }

        public CouchbaseObjectId(string couchIDStr)
        {
            try
            {
                var dt = GetTimestampFromId(couchIDStr);               
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid CouchbaseObjectId string!");
            }
            _couchObjectId = couchIDStr;
        }

        protected static string GenerateId(long unixTimestamp = 0, long sequence = 0)
        {
            DateTime timestamp;
            if (unixTimestamp > 1)
            {
                timestamp = GetDateTimeFromUnixTimestamp(unixTimestamp);
            }
            else {
                timestamp = DateTime.Now;
            }
          
            // Generate a random value.
           

            // Create a 12-byte buffer.
            byte[] buffer = new byte[12];

            // Write the timestamp to the buffer.
            buffer[0] = (byte)(timestamp.Ticks >> 24);
            buffer[1] = (byte)(timestamp.Ticks >> 16);
            buffer[2] = (byte)(timestamp.Ticks >> 8);
            buffer[3] = (byte)(timestamp.Ticks);

            if (sequence<1)
            {
                sequence = new Random().NextInt64(1000000, 9999999);
            }
            
            buffer[4] = (byte)(sequence >> 24);
            buffer[5] = (byte)(sequence >> 16);
            buffer[6] = (byte)(sequence >> 8);
            buffer[7] = (byte)(sequence);

            long randomValue = new Random().NextInt64(1000000, 9999999);
            // Write the incrementing counter to the buffer.
            buffer[8] = (byte)(randomValue);
            buffer[9] = (byte)(randomValue & 0xFF);
            buffer[10] = (byte)((randomValue >> 8) & 0xFF);

            // Convert the buffer to a string.
            string id = BitConverter.ToString(buffer).Replace("-", "");

            return id;
        }

        protected static long GetTimestampFromId(string id)
        {
            // Convert the ID to a byte array.
            byte[] buffer = Encoding.UTF8.GetBytes(id);

            // Get the timestamp value.
            int timestamp = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];

            // Convert the timestamp value to a DateTime.
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(timestamp);

            return GetUnixTimestamp(dateTime);
        }

        public static DateTime GetDateTimeFromUnixTimestamp(long unixTimestamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = _epoch;
            dateTime = dateTime.AddSeconds(unixTimestamp).ToLocalTime();
            return dateTime;
        }

        public static long GetUnixTimestamp(DateTime dt)
        {
            long epochTicks = _epoch.Ticks;
            long unixTime = ((dt.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
            return unixTime;
        }

        public static long GetUnixTimestamp()
        {           
            return GetUnixTimestamp(DateTime.Now);
        }
    }
}
