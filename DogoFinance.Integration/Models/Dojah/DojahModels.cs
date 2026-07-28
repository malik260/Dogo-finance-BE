using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DogoFinance.Integration.Models.Dojah
{
    public class DojahResponse<T>
    {
        [JsonPropertyName("entity")]
        public T? Entity { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    [JsonConverter(typeof(DojahFieldStatusConverter))]
    public class DojahFieldStatus
    {
        [JsonPropertyName("confidence_value")]
        public double ConfidenceValue { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; } = true;

        public string? StringValue { get; set; }
    }

    [JsonConverter(typeof(DojahValueFieldConverter))]
    public class DojahValueField
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; } = true;
    }

    public class DojahFieldStatusConverter : JsonConverter<DojahFieldStatus>
    {
        public override DojahFieldStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new DojahFieldStatus { StringValue = reader.GetString(), Status = true, ConfidenceValue = 100 };
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;
                var result = new DojahFieldStatus { Status = true };

                if (root.TryGetProperty("confidence_value", out var confProp) && confProp.TryGetDouble(out var conf))
                    result.ConfidenceValue = conf;
                if (root.TryGetProperty("status", out var statusProp))
                    result.Status = statusProp.ValueKind == JsonValueKind.True;
                if (root.TryGetProperty("value", out var valProp))
                    result.StringValue = valProp.GetString();

                return result;
            }

            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return new DojahFieldStatus();
        }

        public override void Write(Utf8JsonWriter writer, DojahFieldStatus value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("confidence_value", value.ConfidenceValue);
            writer.WriteBoolean("status", value.Status);
            if (value.StringValue != null)
                writer.WriteString("value", value.StringValue);
            writer.WriteEndObject();
        }
    }

    public class DojahValueFieldConverter : JsonConverter<DojahValueField>
    {
        public override DojahValueField? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new DojahValueField { Value = reader.GetString(), Status = true };
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;
                var result = new DojahValueField { Status = true };

                if (root.TryGetProperty("value", out var valProp))
                    result.Value = valProp.GetString();
                if (root.TryGetProperty("status", out var statusProp))
                    result.Status = statusProp.ValueKind == JsonValueKind.True;

                return result;
            }

            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return new DojahValueField();
        }

        public override void Write(Utf8JsonWriter writer, DojahValueField value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.Value != null)
                writer.WriteString("value", value.Value);
            writer.WriteBoolean("status", value.Status);
            writer.WriteEndObject();
        }
    }

    public class DojahBvnData
    {
        [JsonPropertyName("bvn")]
        public DojahValueField? Bvn { get; set; }

        [JsonPropertyName("first_name")]
        public DojahFieldStatus? FirstName { get; set; }

        [JsonPropertyName("last_name")]     
        public DojahFieldStatus? LastName { get; set; }

        [JsonPropertyName("middle_name")]
        public DojahFieldStatus? MiddleName { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DojahFieldStatus? DateOfBirth { get; set; }
    }

    public class DojahNinData
    {
        [JsonPropertyName("nin")]
        public DojahValueField? Nin { get; set; }

        [JsonPropertyName("firstname")]
        public DojahFieldStatus? FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public DojahFieldStatus? LastName { get; set; }

        [JsonPropertyName("middlename")]
        public DojahFieldStatus? MiddleName { get; set; }

        [JsonPropertyName("birthdate")]
        public DojahFieldStatus? BirthDate { get; set; }

        [JsonPropertyName("first_name")]
        public DojahFieldStatus? RawFirstName { get; set; }

        [JsonPropertyName("last_name")]
        public DojahFieldStatus? RawLastName { get; set; }
    }

    public class BvnVerificationRequest
    {
        public string Bvn { get; set; } = string.Empty;
    }

    public class NinVerificationRequest
    {
        public string Nin { get; set; } = string.Empty;
    }
}
