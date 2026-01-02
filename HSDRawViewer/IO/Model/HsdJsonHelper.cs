using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.Model
{
    public static class HsdJsonHelper
    {
        private static JsonSerializerOptions _settings = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public static void Export<T>(string filepath, T o)
        {
            File.WriteAllText(filepath, JsonSerializer.Serialize(o, _settings));
        }

        public static T Import<T>(string filepath)
        {
            string jsonString = File.ReadAllText(filepath);
            return JsonSerializer.Deserialize<T>(jsonString, _settings);
        }
    }

    public sealed class JsonInlineListConverter<T> : JsonConverter<List<T>>
    {
        private static readonly JsonSerializerOptions _inlineOptions = new()
        {
            WriteIndented = false
        };

        public override List<T> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<List<T>>(ref reader, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            List<T> value,
            JsonSerializerOptions options)
        {
            writer.WriteRawValue(JsonSerializer.Serialize(value, _inlineOptions));
        }
    }

    public sealed class JsonInlineDictionaryListConverter<TKey, TValue>
        : JsonConverter<Dictionary<TKey, List<TValue>>>
    {
        private static readonly JsonSerializerOptions _inlineOptions = new()
        {
            WriteIndented = false
        };

        public override Dictionary<TKey, List<TValue>> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Dictionary<TKey, List<TValue>>>(ref reader, options)!;
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dictionary<TKey, List<TValue>> value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kv in value)
            {
                writer.WritePropertyName(kv.Key!.ToString());

                // 🔑 INLINE JSON
                string inlineJson = JsonSerializer.Serialize(kv.Value, _inlineOptions);
                writer.WriteRawValue(inlineJson);
            }

            writer.WriteEndObject();
        }
    }
}
