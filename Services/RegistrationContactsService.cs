using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class RegistrationContactsService : IRegistrationContactsService
    {
        private readonly IRegistrationContactRepository _registrationContactRepository;
        private readonly ApplicationDbContext _context;

        public RegistrationContactsService(IRegistrationContactRepository registrationContactRepository, ApplicationDbContext context)
        {
            _registrationContactRepository = registrationContactRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<RegistrationContactResponse>>> GetAllRegistrationContactsAsync()
        {
            var registrationContacts = await _registrationContactRepository.GetAllAsync();
            var data = registrationContacts.Select(c => new RegistrationContactResponse
            {
                Id = c.Id,
                FamilyName = c.FamilyName,
                FamilyNumber = c.FamilyNumber,
                FamilyAddress = c.FamilyAddress,
                CreateAt = c.CreateAt,
                UpdateAt = c.UpdateAt,
            }).ToList();

            return new ApiResponse<List<RegistrationContactResponse>>(0, "Fill dữ liệu thành công ", data);
        }

        public async Task<ApiResponse<RegistrationContactResponse>> CreateRegistrationContactAsync(CreateRegistrationContactRequest createRegistrationContactRequest)
        {
            var registrationContact = new RegistrationContact
            {
                FamilyName = createRegistrationContactRequest.FamilyName,
                FamilyNumber = createRegistrationContactRequest.FamilyNumber,
                FamilyAddress = createRegistrationContactRequest.FamilyAddress,
                CreateAt = DateTime.Now,
            };

            await _registrationContactRepository.AddAsync(registrationContact);
            var response = new RegistrationContactResponse
            {
                Id = registrationContact.Id,
                FamilyName = registrationContact.FamilyName,
                FamilyNumber = registrationContact.FamilyNumber,
                FamilyAddress = registrationContact.FamilyAddress,
                CreateAt = registrationContact.CreateAt,
            };

            return new ApiResponse<RegistrationContactResponse>(0, "RegistrationContact đã thêm thành công", response);
        }

        public async Task<ApiResponse<RegistrationContactResponse>> UpdateRegistrationContactAsync(string id, UpdateRegistrationContactRequest updateRegistrationContactRequest)
        {
            if (!int.TryParse(id, out int registrationContactId))
            {
                return new ApiResponse<RegistrationContactResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var registrationContact = await _registrationContactRepository.GetByIdAsync(registrationContactId);
            if (registrationContact == null)
            {
                return new ApiResponse<RegistrationContactResponse>(1, "Không tìm thấy registrationContact.", null);
            }

            registrationContact.FamilyName = updateRegistrationContactRequest.FamilyName;
            registrationContact.FamilyNumber = updateRegistrationContactRequest.FamilyNumber;
            registrationContact.FamilyAddress = updateRegistrationContactRequest.FamilyAddress;
            registrationContact.UpdateAt = DateTime.Now;

            await _registrationContactRepository.UpdateAsync(registrationContact);
            var response = new RegistrationContactResponse
            {
                Id = registrationContact.Id,
                FamilyName = registrationContact.FamilyName,
                FamilyNumber = registrationContact.FamilyNumber,
                FamilyAddress = registrationContact.FamilyAddress,
                CreateAt = registrationContact.CreateAt,
            };

            return new ApiResponse<RegistrationContactResponse>(0, "RegistrationContact đã cập nhật thành công", response);
        }

        public async Task<ApiResponse<RegistrationContactResponse>> DeleteRegistrationContactAsync(string id)
        {
            if (!int.TryParse(id, out int registrationContactId))
            {
                return new ApiResponse<RegistrationContactResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var registrationContact = await _registrationContactRepository.GetByIdAsync(registrationContactId);
            if (registrationContact == null)
            {
                return new ApiResponse<RegistrationContactResponse>(1, "RegistrationContact không tìm thấy");
            }

            registrationContact.IsDelete = true;
            await _registrationContactRepository.UpdateAsync(registrationContact);

            return new ApiResponse<RegistrationContactResponse>(0, "RegistrationContact đã xóa thành công ");
        }

        public Task<IEnumerable<RegistrationContactResponse>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationContactResponse> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationContactResponse> CreateAsync(RegistrationContactRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationContactResponse> UpdateAsync(int id, RegistrationContactRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationContactResponse> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}