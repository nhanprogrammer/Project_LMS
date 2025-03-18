//using Project_LMS.Interfaces.Services;
//using Project_LMS.Interfaces.Repositories;
//using Project_LMS.DTOs.Request;
//using Project_LMS.DTOs.Response;
//using Project_LMS.Models;
//using Project_LMS.Exceptions;

//namespace Project_LMS.Services
//{
//    public class RewardService : IRewardService
//    {
//        private readonly IRewardRepository _rewardRepository;

//        public RewardService(IRewardRepository rewardRepository)
//        {
//            _rewardRepository = rewardRepository;
//        }

//        public async Task<IEnumerable<RewardResponse>> GetAllAsync()
//        {
//            var rewards = await _rewardRepository.GetAllAsync();
//            return rewards.Select(r => new RewardResponse
//            {
//                Id = r.Id,
//                StudentId = r.UserId,
//                SemesterId = r.SemesterId,
//                RewardCode = r.RewardCode,
//                Name = r.RewardName,
//                RewardContent = r.RewardContent,
//                IsDelete = r.IsDelete,
//                CreateAt = r.CreateAt,
//                UpdateAt = r.UpdateAt,
//                UserCreate = r.UserCreate,
//                UserUpdate = r.UserUpdate
//            });
//        }

//        public async Task<RewardResponse> GetByIdAsync(int id)
//        {
//            var reward = await _rewardRepository.GetByIdAsync(id);
//            if (reward == null)
//            {
//                return null;
//            }
//            return new RewardResponse
//            {
//                Id = reward.Id,
//                StudentId = reward.UserId,
//                SemesterId = reward.SemesterId,
//                RewardCode = reward.RewardCode,
//                Name = reward.RewardName,
//                RewardContent = reward.RewardContent,
//                IsDelete = reward.IsDelete,
//                CreateAt = reward.CreateAt,
//                UpdateAt = reward.UpdateAt,
//                UserCreate = reward.UserCreate,
//                UserUpdate = reward.UserUpdate
//            };
//        }

//        public async Task<RewardResponse> CreateAsync(RewardRequest request)
//        {
//            if (request.StudentId == null || request.SemesterId == null || request.RewardCode == null)
//            {
//                throw new ArgumentNullException("Data cannot be null.");
//            }
//            var reward = new Reward
//            {
//                UserId = request.StudentId.Value,
//                SemesterId = request.SemesterId.Value,
//                RewardCode = request.RewardCode.Value,
//                RewardName = request.Name,
//                RewardContent = request.RewardContent,
//                UserCreate = 1,
//                IsDelete = false,
//            };
//            await _rewardRepository.AddAsync(reward);
//            return new RewardResponse
//            {
//                Id = reward.Id,
//                StudentId = reward.UserId,
//                SemesterId = reward.SemesterId,
//                RewardCode = reward.RewardCode,
//                Name = reward.RewardName,
//                RewardContent = reward.RewardContent,
//                IsDelete = reward.IsDelete,
//                CreateAt = reward.CreateAt,
//                UpdateAt = reward.UpdateAt,
//                UserCreate = reward.UserCreate,
//                UserUpdate = reward.UserUpdate
//            };
//        }

//        public async Task<RewardResponse> UpdateAsync(int id, RewardRequest request)
//        {
//            var reward = await _rewardRepository.GetByIdAsync(id);
//            if (reward == null)
//            {
//                throw new NotFoundException("Bản ghi không tồn tại.");
//            }
//            if (request.StudentId == null || request.SemesterId == null || request.RewardCode == null)
//            {
//                throw new ArgumentNullException("Data cannot be null.");
//            }
//            reward.UserId = request.StudentId.Value;
//            reward.SemesterId = request.SemesterId.Value;
//            reward.RewardCode = request.RewardCode.Value;
//            reward.RewardName = request.Name;
//            reward.RewardContent = request.RewardContent;
//            reward.UserUpdate = 1;

//            await _rewardRepository.UpdateAsync(reward);
//            return new RewardResponse
//            {
//                Id = reward.Id,
//                StudentId = reward.UserId,
//                SemesterId = reward.SemesterId,
//                RewardCode = reward.RewardCode,
//                Name = reward.RewardName,
//                RewardContent = reward.RewardContent,
//                IsDelete = reward.IsDelete,
//                CreateAt = reward.CreateAt,
//                UpdateAt = reward.UpdateAt,
//                UserCreate = reward.UserCreate,
//                UserUpdate = reward.UserUpdate
//            };
//        }

//        public async Task<RewardResponse> DeleteAsync(int id)
//        {
//            var reward = await _rewardRepository.GetByIdAsync(id);
//            if (reward == null)
//            {
//                return null;
//            }
//            reward.IsDelete = true;
//            reward.UserUpdate = 1;

//            await _rewardRepository.UpdateAsync(reward);
//            return new RewardResponse
//            {
//                Id = reward.Id,
//                StudentId = reward.UserId,
//                SemesterId = reward.SemesterId,
//                RewardCode = reward.RewardCode,
//                Name = reward.RewardName,
//                RewardContent = reward.RewardContent,
//                IsDelete = reward.IsDelete,
//                CreateAt = reward.CreateAt,
//                UpdateAt = reward.UpdateAt,
//                UserCreate = reward.UserCreate,
//                UserUpdate = reward.UserUpdate
//            };
//        }
//    }
//}