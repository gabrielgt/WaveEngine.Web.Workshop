using System.IO;
using System.Text.Json;

namespace BetiJaiDemo
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string path)
        {
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var @object = JsonSerializer.Deserialize<T>(json, options);

            return @object;
        }
    }
}
