using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Models;
using Project_LMS.Exceptions;

namespace Project_LMS.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRepository _registrationRepository;

        public RegistrationService(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task<IEnumerable<RegistrationResponse>> GetAllAsync()
        {
            var registrations = await _registrationRepository.GetAllAsync();
            return registrations.Select(r => new RegistrationResponse
            {
                Id = r.Id,
                NationalityId = r.NationalityId,
                SchoolId = r.SchoolId,
                UserId = r.UserId,
                Course = r.Course,
                Fullname = r.Fullname,
                Birthday = r.Birthday,
                Gender = r.Gender,
                Education = r.Education,
                CurrentSchool = r.CurrentSchool,
                Address = r.Address,
                Email = r.Email,
                PhoneNumber = r.PhoneNumber,
                Image = r.Image,
                CreateAt = r.CreateAt,
                UpdateAt = r.UpdateAt,
                UserCreate = r.UserCreate,
                UserUpdate = r.UserUpdate,
                IsDelete = r.IsDelete
            });
        }

        public async Task<RegistrationResponse> GetByIdAsync(int id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
            {
                return null;
            }
            return new RegistrationResponse
            {
                Id = registration.Id,
                NationalityId = registration.NationalityId,
                SchoolId = registration.SchoolId,
                UserId = registration.UserId,
                Course = registration.Course,
                Fullname = registration.Fullname,
                Birthday = registration.Birthday,
                Gender = registration.Gender,
                Education = registration.Education,
                CurrentSchool = registration.CurrentSchool,
                Address = registration.Address,
                Email = registration.Email,
                PhoneNumber = registration.PhoneNumber,
                Image = registration.Image,
                CreateAt = registration.CreateAt,
                UpdateAt = registration.UpdateAt,
                UserCreate = registration.UserCreate,
                UserUpdate = registration.UserUpdate,
                IsDelete = registration.IsDelete
            };
        }

        public async Task<RegistrationResponse> CreateAsync(RegistrationRequest request)
        {
            if (request.NationalityId == null || request.SchoolId == null || request.UserId == null || request.Gender == null)
            {
                throw new ArgumentNullException("Data cannot be null.");
            }
            var registration = new Registration
            {
                NationalityId = request.NationalityId.Value,
                SchoolId = request.SchoolId.Value,
                UserId = request.UserId.Value,
                Course = request.Course,
                Fullname = request.Fullname,
                Birthday = request.Birthday,
                Gender = request.Gender.Value,
                Education = request.Education,
                CurrentSchool = request.CurrentSchool,
                Address = request.Address,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Image = request.Image,
                UserCreate = 1,
                IsDelete = false,
            };
            await _registrationRepository.AddAsync(registration);
            return new RegistrationResponse
            {
                Id = registration.Id,
                NationalityId = registration.NationalityId,
                SchoolId = registration.SchoolId,
                UserId = registration.UserId,
                Course = registration.Course,
                Fullname = registration.Fullname,
                Birthday = registration.Birthday,
                Gender = registration.Gender,
                Education = registration.Education,
                CurrentSchool = registration.CurrentSchool,
                Address = registration.Address,
                Email = registration.Email,
                PhoneNumber = registration.PhoneNumber,
                Image = registration.Image,
                CreateAt = registration.CreateAt,
                UpdateAt = registration.UpdateAt,
                UserCreate = registration.UserCreate,
                UserUpdate = registration.UserUpdate,
                IsDelete = registration.IsDelete
            };
        }

        public async Task<RegistrationResponse> UpdateAsync(int id, RegistrationRequest request)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
            {
                throw new NotFoundException("Bản ghi không tồn tại.");
            }
            if (request.NationalityId == null || request.SchoolId == null || request.UserId == null || request.Gender == null)
            {
                throw new ArgumentNullException("Data cannot be null.");
            }
            registration.NationalityId = request.NationalityId.Value;
            registration.SchoolId = request.SchoolId.Value;
            registration.UserId = request.UserId.Value;
            registration.Course = request.Course;
            registration.Fullname = request.Fullname;
            registration.Birthday = request.Birthday;
            registration.Gender = request.Gender.Value;
            registration.Education = request.Education;
            registration.CurrentSchool = request.CurrentSchool;
            registration.Address = request.Address;
            registration.Email = request.Email;
            registration.PhoneNumber = request.PhoneNumber;
            registration.Image = request.Image;
            registration.UserUpdate = 1;

            await _registrationRepository.UpdateAsync(registration);
            return new RegistrationResponse
            {
                Id = registration.Id,
                NationalityId = registration.NationalityId,
                SchoolId = registration.SchoolId,
                UserId = registration.UserId,
                Course = registration.Course,
                Fullname = registration.Fullname,
                Birthday = registration.Birthday,
                Gender = registration.Gender,
                Education = registration.Education,
                CurrentSchool = registration.CurrentSchool,
                Address = registration.Address,
                Email = registration.Email,
                PhoneNumber = registration.PhoneNumber,
                Image = registration.Image,
                CreateAt = registration.CreateAt,
                UpdateAt = registration.UpdateAt,
                UserCreate = registration.UserCreate,
                UserUpdate = registration.UserUpdate,
                IsDelete = registration.IsDelete
            };
        }

        public async Task<RegistrationResponse> DeleteAsync(int id)
        {
            var registration = await _registrationRepository.GetByIdAsync(id);
            if (registration == null)
            {
                return null;
            }
            registration.IsDelete = true;
            registration.UserUpdate = 1;

            await _registrationRepository.UpdateAsync(registration);
            return new RegistrationResponse
            {
                Id = registration.Id,
                NationalityId = registration.NationalityId,
                SchoolId = registration.SchoolId,
                UserId = registration.UserId,
                Course = registration.Course,
                Fullname = registration.Fullname,
                Birthday = registration.Birthday,
                Gender = registration.Gender,
                Education = registration.Education,
                CurrentSchool = registration.CurrentSchool,
                Address = registration.Address,
                Email = registration.Email,
                PhoneNumber = registration.PhoneNumber,
                Image = registration.Image,
                CreateAt = registration.CreateAt,
                UpdateAt = registration.UpdateAt,
                UserCreate = registration.UserCreate,
                UserUpdate = registration.UserUpdate,
                IsDelete = registration.IsDelete
            };
        }
    }
}