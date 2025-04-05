namespace Project_LMS.Interfaces
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateCodeAsync(string prefix, Func<string, Task<bool>> isCodeExists);
    }
}