using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Services
{
    public class AcademicHoldsService : IAcademicHoldsService
    {
        private readonly ApplicationDbContext _context;

        public AcademicHoldsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicHoldResponse>> GetAllAcademicHold()
        {
            return await _context.AcademicHolds
                .Where(ah => !ah.IsDelete)
                .Select(ah => new AcademicHoldResponse
                {
                    Id = ah.Id,
                    UserId = ah.UserId,
                    HoldDate = ah.HoldDate,
                    HoldDuration = ah.HoldDuration,
                    Reason = ah.Reason,
                    FileName = ah.FileName,
                    CreateAt = ah.CreateAt,
                    UserCreate = ah.UserCreate
                })
                .ToListAsync();
        }

        public async Task<AcademicHoldResponse> GetByIdAcademicHold(int id)
        {
            var academicHold = await _context.AcademicHolds
                .AsNoTracking()
                .FirstOrDefaultAsync(ah => ah.Id == id);

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
                existingAcademicHold.CreateAt = DateTime.SpecifyKind(academicHold.CreateAt, DateTimeKind.Unspecified);
                existingAcademicHold.UserCreate = academicHold.UserCreate;

                _context.AcademicHolds.Update(existingAcademicHold);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAcademicHold(int id)
        {
            var academicHold = await _context.AcademicHolds.FindAsync(id);
            if (academicHold != null)
            {
                academicHold.IsDelete = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}