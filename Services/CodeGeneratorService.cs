
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;

        public CodeGeneratorService(ITeacherRepository teacherRepository, IStudentRepository studentRepository)
        {
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
        }

        public async Task<string> GenerateCodeAsync(string prefix, Func<string, Task<bool>> isCodeExists)
        {
            int number = 1; // Bắt đầu từ 1
            int digitLength = 5; // Độ dài số là 5 chữ số (MGV00001 → MGV99999)
            string code;

            do
            {
                code = $"{prefix}{number.ToString($"D{digitLength}")}"; // Ví dụ: MGV00001
                number++;

                // Nếu đã vượt quá số lớn nhất có thể (99999), tăng độ dài số
                if (number > Math.Pow(10, digitLength) - 1)
                {
                    digitLength++;
                }
            }
            while (await isCodeExists(code)); // Kiểm tra mã đã tồn tại chưa

            return code;
        }
    }
}