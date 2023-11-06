using System.Runtime.InteropServices;
using System.Security;

namespace Extension
{
    public static class SecureStringExtension
    {
        public static string ToCString(this SecureString value)
        {

            IntPtr bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }

        }
    }

    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string GetValue(this String whole, string fieldName, string encapsulate = "[]")
        {
            try
            {
                char[] enChar = encapsulate.ToCharArray(0, 2);
                int fieldPos = whole.LastIndexOf(fieldName + "=");
                string fieldString = whole.Substring(fieldPos);
                int en1Pos = fieldString.IndexOf(enChar[0]);
                int en2Pos = fieldString.IndexOf(enChar[1]) - 1;
                string result = fieldString.Substring(en1Pos + 1, en2Pos - en1Pos);
                return result;
            }
            catch (Exception)
            {
                var funcName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                return "";
            }
        }

        public static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            else
            {
                SecureString result = new SecureString();
                foreach (char c in source.ToCharArray())
                    result.AppendChar(c);
                return result;
            }
        }
    }

    public static class DateTimeExtension
    {
        public static bool IsWithinPeriod(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            if (dateToCheck >= startDate && dateToCheck <= endDate)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
