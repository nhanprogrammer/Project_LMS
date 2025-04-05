using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using FluentValidation;
using AutoMapper;
using Project_LMS.Interfaces.Responsitories;
namespace Project_LMS.Services
{
    public class RewardService : IRewardService
    {
        private readonly IRewardRepository _rewardRepository;
        private readonly IValidator<RewardRequest> _validator;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassStudentRepository _classStudentRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public RewardService(IRewardRepository rewardRepository, IValidator<RewardRequest> validator, IMapper mapper, IStudentRepository studentRepository, IClassStudentRepository classStudentRepository, ICloudinaryService cloudinaryService)
        {
            _rewardRepository = rewardRepository;
            _validator = validator;
            _mapper = mapper;
            _studentRepository = studentRepository;
            _classStudentRepository = classStudentRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ApiResponse<object>> AddAsync(RewardRequest request)
        {
            var errors = new List<string>();
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return new ApiResponse<object>(1, "Thêm khen thưởng thất bại.")
                {
                    Data = errors
                };
            }
            var reward = _mapper.Map<Reward>(request);
            try
            {
                if (request.FileName != null)
                {
                    reward.FileName = await _cloudinaryService.UploadDocAsync(request.FileName);
                    Console.WriteLine("url : "+reward.FileName);
                }

              var student = await _studentRepository.FindStudentByUserCode(request.UserCode);
                reward.UserId = student.Id;
                reward.RewardDate = DateTime.Now;
                reward.CreateAt = DateTime.Now;
                await _rewardRepository.AddAsync(reward);
                return new ApiResponse<object>(0, "Thêm khen thưởng thành công.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, "Thêm khen thưởng thất bại.")
                {
                    Data = "error : " + ex.Message
                };
            }


        }



        public async Task<ApiResponse<object>> UpdateAsync(UpdateRewardRequest request)
        {
            var errors = new List<string>();
            var reward = await _rewardRepository.GetByIdAsync(request.id);
            if (reward == null) return new ApiResponse<object>(1, "Khen thưởng không tồn tại.");
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                errors = (validationResult.Errors.Select(e => e.ErrorMessage)).ToList();
                return new ApiResponse<object>(1, "Cập nhật khen thưởng thất bại.")
                {
                    Data = errors
                };
            }

            string rewardName = reward?.FileName;
            try
            {
                reward = _mapper.Map(request, reward);
                if (request.FileName != null)
                {
                    rewardName = await _cloudinaryService.UploadDocAsync(request.FileName);
                }

                    reward.FileName = rewardName;
                
                var student = await _studentRepository.FindStudentByUserCode(request.UserCode);
                reward.UserId = student.Id;
                reward.UpdateAt = DateTime.Now;
                await _rewardRepository.UpdateAsync(reward);
                return new ApiResponse<object>(0, "Cập nhật khen thưởng thành công.");
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(1, "Cập nhật khen thưởng thất bại.")
                {
                    Data = "error : " + ex.Message
                };
            }

        }

        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            var reward = await _rewardRepository.GetByIdAsync(id);
            if (reward == null)
            {
                return new ApiResponse<object>(1, "Khen thưởng không tồn tại.");
            }
            reward.IsDelete = true;
            await _rewardRepository.UpdateAsync(reward);
            return new ApiResponse<object>(0, "Xóa khen thưởng thành công.");
        }

        public async Task<ApiResponse<object>> GetByIdAsync(int id)
        {
            var reward = await _rewardRepository.GetByIdAsync(id);
            if (reward == null) return new ApiResponse<object>(1, "Khen thưởng không tồn tại.");
            //var classStudent = await _classStudentRepository.FindStudentByIdIsActive(reward.UserId ?? 0);
            //string className = classStudent?.Class.Name?.ToString()??"Chưa có dữ liệu";
            var rewardResponse = new
            {
                reward.Id,
                reward.RewardContent,
                reward.FileName,
                reward.RewardDate,
                reward?.User?.FullName,
                //className,
                reward?.Semester?.Name
            };
            return  new ApiResponse<object>(0, "Đã tìm thấy khen thưởng.")
            {
                Data = rewardResponse
            };
        }
    }
}