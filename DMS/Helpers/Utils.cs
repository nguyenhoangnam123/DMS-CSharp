using Newtonsoft.Json;

namespace DMS.Helpers
{
    public static class Utils
    {
        public static T Clone<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            T newObj = JsonConvert.DeserializeObject<T>(json);
            return newObj;
        }
    }
}
