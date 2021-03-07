using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text;

namespace LiveStreamStore.Lib.Utilities
{
    public static class CommonExtensions
    {
        public static string EncryptMd5(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            var md5 = new MD5CryptoServiceProvider();
            byte[] valueArray = Encoding.ASCII.GetBytes(value);
            valueArray = md5.ComputeHash(valueArray);
            var sb = new StringBuilder();
            for (int i = 0; i < valueArray.Length; i++)
                sb.Append(valueArray[i].ToString("x2").ToLower());
            return sb.ToString();
        }

        public static string ToJson(this object Obj)
        {
            return JsonConvert.SerializeObject(Obj);
        }

        public static T JsonToObject<T>(this string Data)
        {
            return JsonConvert.DeserializeObject<T>(Data);
        }

    }
}
