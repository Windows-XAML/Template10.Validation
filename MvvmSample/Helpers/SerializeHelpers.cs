using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MvvmSample.Helpers
{
    public static class SerializeHelpers
    {
        public static string Serialize(object value)
        {
            if (value == null)
                return string.Empty;

            return JsonConvert.SerializeObject(value);
        }

        public static T Deserialize<T>(string json, T defautlValue = default(T))
        {
            if (string.IsNullOrEmpty(json))
                return defautlValue;

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
