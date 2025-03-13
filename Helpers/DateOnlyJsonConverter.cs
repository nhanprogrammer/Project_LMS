﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Project_LMS.Helpers;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateOnly.TryParseExact(value, Format, out var dateOnly))
        {
            return dateOnly;
        }

        throw new JsonException($"Invalid date format. Expected {Format}");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}