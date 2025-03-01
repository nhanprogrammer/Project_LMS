using AutoMapper;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Models;
using Project_LMS.Exceptions;

namespace Project_LMS.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleResponse>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleResponse>>(roles);
        }

        public async Task<RoleResponse> GetByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new NotFoundException("Không tìm thấy vai trò");
            }

            return _mapper.Map<RoleResponse>(role);
        }

        public async Task<RoleResponse> CreateAsync(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentNullException("Tên vai trò không được để trống.");
            }

            var role = _mapper.Map<Role>(request);
            role.UserCreate = 1;
            role.IsDelete = false;

            await _roleRepository.AddAsync(role);

            return _mapper.Map<RoleResponse>(role);
        }

        public async Task<RoleResponse> UpdateAsync(int id, RoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new NotFoundException("Không tìm thấy vai trò");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentNullException("Tên vai trò không được để trống.");
            }

            _mapper.Map(request, role);
            role.UserUpdate = 1;

            await _roleRepository.UpdateAsync(role);

            return _mapper.Map<RoleResponse>(role);
        }

        public async Task<RoleResponse> DeleteAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                throw new NotFoundException("Không tìm thấy vai trò");
            }

            role.IsDelete = true;
            role.UserUpdate = 1;

            await _roleRepository.UpdateAsync(role);

            return _mapper.Map<RoleResponse>(role);
        }
    }
}