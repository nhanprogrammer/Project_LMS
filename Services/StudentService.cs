using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Mappers;
using Project_LMS.Models;

namespace Project_LMS.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IClassStudentRepository _classStudentRepository;
    private readonly IClassRepository _classRepository;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IMapper _mapper;
    private readonly ILogger<StudentService> _logger;

    public StudentService(IStudentRepository studentRepository, IClassStudentRepository classStudentRepository, IClassRepository classRepository, ICloudinaryService cloudinaryService, IMapper mapper, ILogger<StudentService> logger)
    {
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _classStudentRepository = classStudentRepository ?? throw new ArgumentNullException(nameof(classStudentRepository));
        _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
        _cloudinaryService = cloudinaryService ?? throw new ArgumentNullException(nameof(cloudinaryService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApiResponse<object>> AddAsync(StudentRequest request)
    {
        var student = _mapper.Map<User>(request);
        student.IsDelete = false;
        student.CreateAt = DateTime.Now;
        try
        {
            if (request.Image != null)
            {
                string imageBase64 = await ConvertFormFileToBase64(request.Image);
                string url = await _cloudinaryService.UploadImageAsync(imageBase64);
                student.Image = url;
                Console.WriteLine("Url : " + url);
            }
            var user = await _studentRepository.AddAsync(student);
            await _classStudentRepository.AddAsync(new ClassStudentRequest()
            {
                UserId = user.Id,
                ClassId = request.ClassId,
                UserCreate = user.UserCreate
            });

            _logger.LogInformation("Student added successfully.");
            return new ApiResponse<object>(0, "Add Student success.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding student.");
            return new ApiResponse<object>(4, "Database error occurred while adding student.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding student. Data: {@Student}", student);
            return new ApiResponse<object>(5, "An unexpected error occurred while adding student.");
        }
    }


    public async Task<ApiResponse<PaginatedResponse<object>>> GetAllStudentOfRewardOrDisciplines(bool isReward, int academicId, int departmentId, PaginationRequest request, string columnm, bool orderBy, string searchItem)
    {
        var classes = await _classRepository.GetAllClassByAcademicDepartment(academicId, departmentId);
        var classesId = classes.Select(c => c.Id).ToList();
        var classStudents = await _classStudentRepository.getAllStudentByClasses(classesId);
        var studentsId = classStudents.Select(c => (int)c.UserId).ToList();
        var students = await _studentRepository.GetAllOfRewardByIds(isReward, studentsId, request, columnm, orderBy, searchItem);
        var studentResponse = students.Select(st => (object)new
        {
            st.UserCode,
            st.FullName,
            st.BirthDate,
            gender = (st.Gender != null && st.Gender.Length > 0) ? st.Gender[0] : false,
            st.Rewards.Count

        }).ToList();
        var totalItems = await _studentRepository.CountStudentOfRewardByIds(isReward, studentsId, searchItem);
        var paginatedResponse = new PaginatedResponse<object>
        {
            Items = studentResponse,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize),
            HasPreviousPage = request.PageNumber > 1,
            HasNextPage = request.PageNumber < (int)Math.Ceiling(totalItems / (double)request.PageSize)
        };
        return new ApiResponse<PaginatedResponse<object>>(0, "GetAll User success.") { Data = paginatedResponse };
    }

    public async Task<ApiResponse<object>> LearningOutcomesOfStudent(int studentId,int classId)
    {
        var student = await _studentRepository.FindStudentById(studentId);
        if (student == null || student.Assignments == null)
        {
            return new ApiResponse<object>(1, "Học viên không tồn tại.");
        }

        double totalScore1 = student.Assignments
            .Where(asm => asm.TestExam.Semesters.Name.ToLower().Contains("học kỳ 1") && asm.TestExam.ClassId == classId)
            .Sum(asm => (asm.TotalScore.Value * asm.TestExam.TestExamType.Coefficient.Value)/ asm.TestExam.TestExamType.Coefficient.Value);

        double totalScore2 = student.Assignments
            .Where(asm => asm.TestExam.Semesters.Name.ToLower().Contains("học kỳ 2") && asm.TestExam.ClassId == classId)
            .Sum(asm => (asm.TotalScore.Value * asm.TestExam.TestExamType.Coefficient.Value) / asm.TestExam.TestExamType.Coefficient.Value);

        string academicPerformance1;
        string conduct1;

        if (totalScore1 == null || totalScore1 ==0)
        {
            academicPerformance1 = "Không có dữ liệu";
            conduct1 = "không có dữ liệu";
            totalScore1 = 0;
        }
        else
        {
            double score = totalScore1;
            academicPerformance1 = score switch
            {
                >= 8.0 => "Giỏi",
                >= 6.5 => "Khá",
                >= 5.0 => "Trung bình",
                _ => "Yếu"
            };
            conduct1 = score > 5 ? "Tốt" : "Khá";
        }
        string academicPerformance2;
        string conduct2;
        if (totalScore2 == null || totalScore2==0)
        {
            academicPerformance2 = "Không có dữ liệu";
            conduct2 = "không có dữ liệu";
            totalScore2 = 0;
        }
        else
        {
            double score = totalScore1;
            academicPerformance2 = score switch
            {
                >= 8.0 => "Giỏi",
                >= 6.5 => "Khá",
                >= 5.0 => "Trung bình",
                _ => "Yếu"
            };
            conduct2 = score > 5 ? "Tốt" : "Khá";
        }
        //Cả năm 
        string academicPerformanceOld;
        string conDuct;
        double totalScore=0;  
        if (totalScore1 == null || totalScore2== null|| totalScore1 == 0|| totalScore2 ==0)
        {
            academicPerformanceOld = "Không có dữ liệu";
            conDuct = "không có dữ liệu";
        }
        else
        {
            double score = ((totalScore2 * 2)+ totalScore1) /3 ;
            academicPerformanceOld = score switch
            {
                >= 8.0 => "Giỏi",
                >= 6.5 => "Khá",
                >= 5.0 => "Trung bình",
                _ => "Yếu"
            };
            conDuct = score > 5 ? "Tốt" : "Khá";
        }


        var studentResponse = new
        {
            semesterTranscript = new
            {
                semsester1 = (object)new
                {
                    academicPerformance = academicPerformance1,
                    conduct = conduct1,
                    averagescore = totalScore1
                },
                semsester2 = (object)new
                {
                    academicPerformance = academicPerformance2,
                    conduct = conduct2,
                    averagescore = totalScore2
                },
                semsester = (object)new
                {
                    academicPerformance = academicPerformanceOld,
                    conduct = conDuct,
                    averagescore = totalScore
                }

            }
        };
        return new ApiResponse<object>(0, "Lấy bảng điểm thành công.")
        {
            Data = studentResponse
        };

    }

    public async Task<ApiResponse<object>> UpdateAsync(UpdateStudentRequest request)
    {
        // Tìm student theo Id
        var student = await _studentRepository.FindById(request.Id);
        if (student == null)
        {
            return new ApiResponse<object>(1, "Student not found");
        }
        try
        {
            // Ánh xạ dữ liệu từ request sang student hiện có
            _mapper.Map(request, student); // Cập nhật student thay vì tạo mới


            await _studentRepository.UpdateAsync(student);


            return new ApiResponse<object>(0, "Student updated successfully", student.Id);
        }
        catch (DbUpdateException ex)
        {
            // Trả về lỗi nếu có vấn đề khi cập nhật cơ sở dữ liệu
            return new ApiResponse<object>(1, $"Error updating student: {ex.Message}");
        }
    }
    private async Task<string> ConvertFormFileToBase64(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream); // Chuyển IFormFile vào MemoryStream
            byte[] imageBytes = memoryStream.ToArray(); // Chuyển thành mảng byte
            string base64String = Convert.ToBase64String(imageBytes); // Mã hóa Base64
            string mimeType = GetMimeType(file.FileName); // Lấy MIME type
            return $"data:{mimeType};base64,{base64String}"; // Trả về chuỗi Base64
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Error reading uploaded file.");
            return null;
        }
    }

    private string GetMimeType(string fileName)
    {
        string extension = Path.GetExtension(fileName).ToLower();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            _ => "application/octet-stream"
        };
    }
}