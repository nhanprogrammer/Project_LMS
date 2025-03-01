using System.Text.Json.Serialization;
using System.Text.Json;

namespace Project_LMS.Configurations
{
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timeString = reader.GetString();
            return TimeOnly.Parse(timeString);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("HH:mm"));  // Định dạng thời gian phù hợp với JSON
        }
    }

}
