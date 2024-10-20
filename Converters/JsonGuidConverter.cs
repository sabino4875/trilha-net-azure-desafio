namespace TrilhaNetAzureDesafio.Converters
{
    using System;
    using System.Diagnostics;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class JsonGuidConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(Guid));
            var result = Guid.Empty;

            if (ContainsValue(reader.GetString()))
            {
                if (Guid.TryParse(reader.GetString(), out Guid validated))
                {
                    result = validated;
                }
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            writer.WriteStringValue(value.ToString());
        }

        private static bool ContainsValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;
            if (string.IsNullOrWhiteSpace(value)) return false;
            if (value.Trim().Length < 1) return false;
            return true;
        }
    }
}
