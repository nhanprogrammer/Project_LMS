using System.Text.Json.Serialization;
using System.Text.Json;

namespace Project_LMS.Configurations
{
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            return DateOnly.Parse(dateString);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd"));  // Định dạng ngày phù hợp với JSON
        }
    }

}
