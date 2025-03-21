using System.Text.Json.Serialization;

namespace Project_LMS.DTOs.Response;

public class ApiResponse<T>
{
    private int v;
    private Task<string> task;
    private object value;

    public int Status { get; set; }
    public string Message { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T Data { get; set; }

    public ApiResponse(int status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public ApiResponse(int status, string message)
    {
        Status = status;
        Message = message;
        Data = default;
    }

    public ApiResponse(int v, Task<string> task, object value)
    {
        this.v = v;
        this.task = task;
        this.value = value;
    }
}