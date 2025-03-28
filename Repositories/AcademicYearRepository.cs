﻿using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Models;

namespace Project_LMS.Repositories;

public class AcademicYearRepository : IAcademicYearRepository
{
    private readonly ApplicationDbContext _context;

    public AcademicYearRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AcademicYear>> GetAllAsync()
    {
        return await _context.AcademicYears
            .Include(a => a.Semesters)
            .Where(ah => (bool)!ah.IsDelete)
            .ToListAsync();
    }

    public async Task<AcademicYear> GetByIdAsync(int id)
    {
        return await _context.AcademicYears.FindAsync(id);
    }

    public async Task AddAsync(AcademicYear academicYear)
    {
        _context.AcademicYears.Add(academicYear);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(AcademicYear academicYear)
    {
        _context.AcademicYears.Update(academicYear);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var academicYear = await _context.AcademicYears.FindAsync(id);
        if (academicYear != null)
        {
            _context.AcademicYears.Remove(academicYear);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<AcademicYear>> GetByIdsAsync(List<int> ids)
    {
        return await _context.AcademicYears.Where(l => ids.Contains(l.Id)).ToListAsync();
    }

    public async Task UpdateRangeAsync(List<AcademicYear> academicYears)
    {
        _context.AcademicYears.UpdateRange(academicYears);
        await _context.SaveChangesAsync();
    }

    public IQueryable<AcademicYear> GetQueryable()
    {
        return _context.AcademicYears
            .Include(a => a.Semesters)
            .Where(ah => (bool)!ah.IsDelete)
            .AsQueryable();
    }

    public async Task<List<AcademicYear>> SearchAcademicYear(int year)
    {
        return await _context.AcademicYears
            .Include(a => a.Semesters)
            .Where(a => (a.StartDate.HasValue && a.StartDate.Value.Year == year) ||
                        (a.EndDate.HasValue && a.EndDate.Value.Year == year))
            .AsQueryable()
            .ToListAsync();
    }

    public async Task<bool> IsAcademicYearExist(int academicYearId)
    {
        return await _context.AcademicYears.AnyAsync(a => a.Id == academicYearId);
    }


}