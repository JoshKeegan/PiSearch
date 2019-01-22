using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StringSearch.Api.Mvc.NewtonsoftJson.Converters
{
    public class DictionaryKeysCamelCaseConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Check value is of type IDictionary<,>
            Type t = value.GetType();
            if (typeof(IDictionary).IsAssignableFrom(t) && 
                t.IsGenericType && 
                t.GetGenericTypeDefinition().IsAssignableFrom(typeof(IDictionary<,>)))
            {
                throw new ArgumentException("Must be of type Dictionary<string, object>", nameof(value));
            }

            // Serialise camel case, also acting on dictionary names (which should be disabled by default)
            string json = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                    {
                        ProcessDictionaryKeys = true
                    }
                }
            });
            writer.WriteRawValue(json);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, object>);
        }
    }
}
