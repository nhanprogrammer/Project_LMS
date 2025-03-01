using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class DisciplinesService : IDisciplinesService
    {
        private readonly IDisciplineRepository _disciplineRepository;
        private readonly ApplicationDbContext _context;

        public DisciplinesService(IDisciplineRepository disciplineRepository, ApplicationDbContext context)
        {
            _disciplineRepository = disciplineRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<DisciplineResponse>>> GetAllDisciplineAsync()
        {
            var disciplines = await _disciplineRepository.GetAllAsync();
    
            var data = disciplines.Select(c => new DisciplineResponse
            {
                Id = c.Id,
                DisciplineContent = c.DisciplineContent,
                CreateAt = c.CreateAt,
            }).ToList();
    
            return new ApiResponse<List<DisciplineResponse>>(0, "Fill dữ liệu thành công ", data);
        }

        public async Task<ApiResponse<DisciplineResponse>> CreateDisciplineAsync(CreateDisciplineRequest createDisciplineRequest)
        {
            var discipline = new Discipline
            {
                StudentId = createDisciplineRequest.StudentId,
                SemesterId = createDisciplineRequest.SemesterId,
                DisciplineCode = createDisciplineRequest.DisciplineCode,
                DisciplineContent = createDisciplineRequest.DisciplineContent,
                Name = createDisciplineRequest.Name,
                // userCreate vì chưa phân quyền nên ko có token 
            };
            await _disciplineRepository.AddAsync(discipline);
            var response = new DisciplineResponse
            {  
                Id = discipline.Id,
                DisciplineContent = discipline.DisciplineContent,
                CreateAt = discipline.CreateAt,
             
            };
            return new ApiResponse<DisciplineResponse>(0, "Department đã thêm thành công", response);
        }

        public async Task<ApiResponse<DisciplineResponse>> UpdateDisciplineAsync(string id, UpdateDisciplineRequest updateDisciplineRequest)
        {
            if (!int.TryParse(id, out int disciplineId))
            {
                return new ApiResponse<DisciplineResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var discipline = await _disciplineRepository.GetByIdAsync(disciplineId);
            if (discipline == null)
            {
                return new ApiResponse<DisciplineResponse>(1, "Không tìm thấy discipline.", null);
            }
            discipline.DisciplineContent = updateDisciplineRequest.DisciplineContent;
            discipline.Name = updateDisciplineRequest.Name;
            discipline.StudentId = updateDisciplineRequest.StudentId;
            discipline.SemesterId = updateDisciplineRequest.SemesterId;
            discipline.UpdateAt = DateTime.Now;
            discipline.UserUpdate = null;
            discipline.DisciplineCode = updateDisciplineRequest.DisciplineCode;
           
            await _disciplineRepository.UpdateAsync(discipline);
            var response = new DisciplineResponse
            {  
                Id = discipline.Id,
                DisciplineContent = discipline.DisciplineContent,
                CreateAt = discipline.CreateAt,
             
            };

            return new ApiResponse<DisciplineResponse>(0, "Department đã cập nhật thành công", response);
        }

        public async Task<ApiResponse<DisciplineResponse>> DeleteDisciplineAsync(string id)
        {
            if (!int.TryParse(id, out int departmentId))
            {
                return new ApiResponse<DisciplineResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }
            var discipline = await _disciplineRepository.GetByIdAsync(departmentId);
            if (discipline == null)
            {
                return new ApiResponse<DisciplineResponse>(1, "Department không tìm thấy");
            }

            discipline.IsDelete = true;
            await _disciplineRepository.UpdateAsync(discipline);

            return new ApiResponse<DisciplineResponse>(0, "Department đã xóa thành công ");
        }
    }
}