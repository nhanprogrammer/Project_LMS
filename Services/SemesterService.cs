using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;
using Project_LMS.Interfaces.Responsitories;

namespace Project_LMS.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IAcademicYearRepository _academicYearRepository;
        private readonly IMapper _mapper;

        public SemesterService(ISemesterRepository semesterRepository, IAcademicYearRepository academicYearRepository, IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _academicYearRepository = academicYearRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SemesterResponse>> GetAllAsync()
        {
            var semesters = await _semesterRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SemesterResponse>>(semesters);
        }

        public async Task<SemesterResponse> GetByIdAsync(int id)
        {
            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new NotFoundException("Không tìm thấy học kỳ.");
            }

            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<ApiResponse<SemesterResponse>> CreateSemesters(List<CreateSemesterRequest> request, int academicYearId, int userId)
        {
            var academicYear = await _academicYearRepository.GetByIdAsync(academicYearId);
            if (academicYear == null)
            {
                return new ApiResponse<SemesterResponse>(1, "Niên khóa không tồn tại.");
            }
            
            if (request == null || !request.Any())
                {
                    return new ApiResponse<SemesterResponse>(1, "Danh sách học kỳ không hợp lệ.");
                }

                var sortedSemesters = request.OrderBy(s => s.DateStart).ToList();

                for (int i = 0; i < sortedSemesters.Count - 1; i++)
                {
                    var currentSemester = sortedSemesters[i];
                    var nextSemester = sortedSemesters[i + 1];

                    if (currentSemester.Name == nextSemester.Name)
                    {
                        return new ApiResponse<SemesterResponse>(1, "Tên học kỳ không thể trùng nhau.");
                    }

                    if (currentSemester.DateEnd > nextSemester.DateStart)
                    {
                        return new ApiResponse<SemesterResponse>(1,
                            $"Thời gian của {currentSemester.Name} không hợp lệ. " +
                            $"Ngày kết thúc ({currentSemester.DateEnd:yyyy-MM-dd}) " +
                            $"không thể lớn hơn Ngày bắt đầu của {nextSemester.Name} ({nextSemester.DateStart:yyyy-MM-dd}).");
                    }
                }

                academicYear.Semesters = _mapper.Map<List<Semester>>(request);

                foreach (var semester in academicYear.Semesters)
                {
                    semester.UserCreate = userId;
                    semester.CreateAt = TimeHelper.Now;
                }

                await _semesterRepository.AddRangeAsync(academicYear.Semesters);
            
            return new ApiResponse<SemesterResponse>(0, "Thêm Học Kỳ thành công.");
        }

        public async Task<ApiResponse<SemesterResponse>> UpdateSemesters(List<UpdateSemesterRequest> semesters, int academicYearId, int userId)
        {
            var academicYearRequest = await _academicYearRepository.GetByIdAsync(academicYearId);
            var existingSemesters = (await _semesterRepository.GetByAcademicYearIdAsync(academicYearId)).ToList();
            var updatedSemesters = new List<Semester>();
            var newSemesters = new List<Semester>();
            var sortedSemesters = semesters.OrderBy(s => s.DateStart).ToList();
            for (int i = 0; i < sortedSemesters.Count - 1; i++)
            {
                var currentSemester = sortedSemesters[i];
                var nextSemester = sortedSemesters[i + 1];

                if (currentSemester.Name == nextSemester.Name)
                {
                    return new ApiResponse<SemesterResponse>(1, "Tên học kỳ không thể trùng nhau.");
                }

                if (currentSemester.DateEnd > nextSemester.DateStart)
                {
                    return new ApiResponse<SemesterResponse>(1,
                        $"Thời gian của {currentSemester.Name} không hợp lệ. " +
                        $"Ngày kết thúc ({currentSemester.DateEnd:yyyy-MM-dd}) " +
                        $"không thể lớn hơn Ngày bắt đầu của {nextSemester.Name} ({nextSemester.DateStart:yyyy-MM-dd}).");
                }
            }
            
            foreach (var semester in semesters)
            {
                // Kiểm tra trong context trước để tránh lỗi theo dõi trùng lặp
                var existingSemester = existingSemesters.FirstOrDefault(s => s.Id == semester.Id);

                if (semester.DateStart > semester.DateEnd)
                {
                    return new ApiResponse<SemesterResponse>(1, $"Ngày kết thúc của {semester.Name} không thể thấp hơn ngày bắt đầu.");
                }
               
                if (semester.DateStart.ToDateTime(TimeOnly.MinValue) < academicYearRequest.StartDate || semester.DateEnd.ToDateTime(TimeOnly.MinValue) > academicYearRequest.EndDate)
                {
                    return new ApiResponse<SemesterResponse>(1, $"Thời gian của {semester.Name} không hợp lệ với Niên Khóa.");
                }

                if (existingSemester != null)
                {
                    _mapper.Map(semester, existingSemester);
                    existingSemester.AcademicYearId = academicYearId;
                    existingSemester.UserUpdate = userId;
                    existingSemester.UpdateAt = TimeHelper.Now;
                    updatedSemesters.Add(existingSemester);                 
                }
                else
                {
                    var newSemester = _mapper.Map<Semester>(semester);
                    newSemester.Id = 0;
                    newSemester.AcademicYearId = academicYearId;
                    newSemester.UserCreate = userId;
                    newSemester.CreateAt = TimeHelper.Now;
                    newSemesters.Add(newSemester);
                }
            }

            var semesterIdsFromRequest = semesters.Where(s => s.Id > 0).Select(s => s.Id).ToList();
            var semestersToDelete = existingSemesters.Where(s => !semesterIdsFromRequest.Contains(s.Id)).ToList();
            if (semestersToDelete.Any())
                await _semesterRepository.DeleteRangeAsync(semestersToDelete);

            if (updatedSemesters.Any())
            {
                await _semesterRepository.UpdateRangeAsync(updatedSemesters);
            }   

            if (newSemesters.Any())
            {
                await _semesterRepository.AddRangeAsync(newSemesters);
            }
            return new ApiResponse<SemesterResponse>(0, "Cập nhật Học Kỳ thành công.");
        }

        public async Task<SemesterResponse> DeleteAsync(int id)
        {
            var semester = await _semesterRepository.GetByIdAsync(id);
            if (semester == null)
            {
                throw new NotFoundException("Không tìm thấy học kỳ.");
            }

            semester.IsDelete = true;
            semester.UserUpdate = 1;

            await _semesterRepository.UpdateAsync(semester);

            return _mapper.Map<SemesterResponse>(semester);
        }
    }
}