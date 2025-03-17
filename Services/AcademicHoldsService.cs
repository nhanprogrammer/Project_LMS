using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;
using Project_LMS.Repositories;

namespace Project_LMS.Services
{
    public class AcademicHoldsService : IAcademicHoldsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAcademicHoldRepository _academicHoldRepository;
        private readonly IMapper _mapper;

        public AcademicHoldsService(ApplicationDbContext context,IAcademicHoldRepository academicHoldRepository ,IMapper mapper)
        {
            _context = context;
            _academicHoldRepository = academicHoldRepository;
            _mapper = mapper;

        }

        public async Task<PaginatedResponse<AcademicHoldResponse>> GetPagedAcademicHolds(PaginationRequest request)
        {
            var query = _academicHoldRepository.GetQueryable()
                         .Where(ah => !ah.IsDelete);
            int totalItems = await query.CountAsync();

            int pageSize = request.PageSize > 0 ? request.PageSize : 10;

            var academicYearsList = await query
                .Skip((request.PageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponse<AcademicHoldResponse>
            {
                Items = _mapper.Map<List<AcademicHoldResponse>>(academicYearsList),
                PageNumber = request.PageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                HasPreviousPage = request.PageNumber > 1,
                HasNextPage = request.PageNumber * pageSize < totalItems
            };
        }

        public async Task<AcademicHoldResponse> GetByIdAcademicHold(int id)
        {
            var academicHold = await _context.AcademicHolds
                .AsNoTracking()
                .FirstOrDefaultAsync(ah => ah.Id == id && !ah.IsDelete);

            if (academicHold == null)
                return null;

            return new AcademicHoldResponse
            {
                Id = academicHold.Id,
                UserId = academicHold.UserId,
                HoldDate = academicHold.HoldDate,
                HoldDuration = academicHold.HoldDuration,
                Reason = academicHold.Reason,
                FileName = academicHold.FileName,
                CreateAt = academicHold.CreateAt,
                UserCreate = academicHold.UserCreate
            };
        }

        public async Task AddAcademicHold(CreateAcademicHoldRequest academicHold)
        {
            var newAcademicHold = new AcademicHold
            {
                UserId = academicHold.UserId,
                HoldDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                HoldDuration = academicHold.HoldDuration,
                Reason = academicHold.Reason,
                FileName = academicHold.FileName,
                CreateAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UserCreate = academicHold.UserCreate,
                IsDelete = false
            };

            _context.AcademicHolds.Add(newAcademicHold);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAcademicHold(UpdateAcademicHoldRequest academicHold)
        {
            var existingAcademicHold = await _context.AcademicHolds.FindAsync(academicHold.Id);
            if (existingAcademicHold != null)
            {
                existingAcademicHold.UserId = academicHold.UserId;
                existingAcademicHold.HoldDate = DateTime.SpecifyKind(academicHold.HoldDate, DateTimeKind.Unspecified);
                existingAcademicHold.HoldDuration = academicHold.HoldDuration;
                existingAcademicHold.Reason = academicHold.Reason;
                existingAcademicHold.FileName = academicHold.FileName;
                existingAcademicHold.UpdateAt = DateTime.SpecifyKind(academicHold.UpdateAt, DateTimeKind.Unspecified);
                existingAcademicHold.UserUpdate = academicHold.UserUpdate;

                _context.AcademicHolds.Update(existingAcademicHold);
                await _context.SaveChangesAsync();
            }
        }

        //public async Task DeleteAcademicHold(int id)
        //{
        //    var academicHold = await _context.AcademicHolds.FindAsync(id);
        //    if (academicHold != null)
        //    {
        //        academicHold.IsDelete = true;
        //        await _context.SaveChangesAsync();
        //    }
        //}
        public async Task<bool> DeleteAcademicHold(int id)
        {
            var existingAcademicYear = await _academicHoldRepository.GetByIdAsync(id);
            if (existingAcademicYear == null)
            {
                return false;
            }

            existingAcademicYear.IsDelete = true;
            await _academicHoldRepository.UpdateAsync(existingAcademicYear);

            return true;
        }
    }
}