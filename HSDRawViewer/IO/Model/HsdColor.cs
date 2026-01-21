using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HSDRawViewer.IO.Model
{
    [JsonConverter(typeof(HsdColorJsonConverter))]
    public class HsdColor
    {
        public float R { get; set; }

        public float G { get; set; }

        public float B { get; set; }

        public float A { get; set; }

        public HsdColor()
        {
        }

        public HsdColor(byte r, byte g, byte b, byte a)
        {
            R = r / 255f;
            G = g / 255f;
            B = b / 255f;
            A = a / 255f;
        }

        public HsdColor(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public HsdColor(Color color)
        {
            R = color.R / 255f;
            G = color.G / 255f;
            B = color.B / 255f;
            A = color.A / 255f;
        }

        public Color ToColor()
        {
            return Color.FromArgb((byte)(A * 255), (byte)(R * 255), (byte)(G * 255), (byte)(B * 255));
        }
    }

    public sealed class HsdColorJsonConverter : JsonConverter<HsdColor>
    {
        public override HsdColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of array for HsdColor");

            reader.Read();
            float r = reader.GetSingle();

            reader.Read();
            float g = reader.GetSingle();

            reader.Read();
            float b = reader.GetSingle();

            reader.Read();
            float a = reader.GetSingle();

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException("Expected end of array for HsdColor");

            return new HsdColor(r, g, b, a);
        }

        public override void Write(Utf8JsonWriter writer, HsdColor value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.R);
            writer.WriteNumberValue(value.G);
            writer.WriteNumberValue(value.B);
            writer.WriteNumberValue(value.A);
            writer.WriteEndArray();
        }
    }
}
