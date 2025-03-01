using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class ModulesService : IModulesService
    {
        private readonly IModuleRepository _moduleRepository;
        private readonly ApplicationDbContext _context;

        public ModulesService(IModuleRepository moduleRepository, ApplicationDbContext context)
        {
            _moduleRepository = moduleRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<ModuleResponse>>> GetAllModuleAsync()
        {
            var modules = await _moduleRepository.GetAllAsync();
            var data = modules.Select(c => new ModuleResponse
            {
               Name = c.Name,
               Description = c.Description,
            }).ToList();

    
            return new ApiResponse<List<ModuleResponse>>(0, "Fill dữ liệu thành công ", data);
        }

        public async Task<ApiResponse<ModuleResponse>> CreateModuleAsync(CreateModuleRequest createModuleRequest)
        {
            var module = new Module
            {
                Name = createModuleRequest.Name,
                Description = createModuleRequest.Description,
                CreateAt = DateTime.Now,
            };
            await _moduleRepository.AddAsync(module);
            
            var response = new ModuleResponse
            {
                Name = module.Name,
                Description = module.Description,
                
         
            };
            return new ApiResponse<ModuleResponse>(0, "Module đã thêm thành công", response);
            
            
            
        }

        public async Task<ApiResponse<ModuleResponse>> UpdateModuleAsync(string id, UpdateModuleRequest updateModuleRequest)
        {
            if (!int.TryParse(id, out int moduleId))
            {
                return new ApiResponse<ModuleResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var module = await _moduleRepository.GetByIdAsync(moduleId);
            if (module == null)
            {
                return new ApiResponse<ModuleResponse>(1, "Không tìm thấy module.", null);
            }
            
             module.Name = updateModuleRequest.Name;
             module.Description = updateModuleRequest.Description;
             module.UpdateAt = DateTime.Now;
             await _moduleRepository.UpdateAsync(module);
             var response = new ModuleResponse
             {
                 Name = module.Name,
                 Description = module.Description,
                
         
             };
             return new ApiResponse<ModuleResponse>(0, "Module đã cập nhật thành công", response);
            
        }

        public async Task<ApiResponse<ModuleResponse>> DeleteModuleAsync(string id)
        {
            if (!int.TryParse(id, out int moduleId))
            {
                return new ApiResponse<ModuleResponse>(1, "ID không hợp lệ. Vui lòng kiểm tra lại.", null);
            }

            var module = await _moduleRepository.GetByIdAsync(moduleId);
            if (module == null)
            {
                return new ApiResponse<ModuleResponse>(1, "Không tìm thấy module.", null);
            }
            
            module.IsDelete = true;
            await _moduleRepository.UpdateAsync(module);

            return new ApiResponse<ModuleResponse>(0, "Module đã xóa thành công ");
        }
    }
}