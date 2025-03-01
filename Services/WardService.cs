
﻿using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;


namespace Project_LMS.Services;

public class WardService : IWardService
{

    private readonly ApplicationDbContext _context;
    public WardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<WardResponse>> Create(WardRequest ward)
    {
        try
        {
            var _ward = ToWardRequest(ward);
            var district = await _context.Districts.FindAsync(ward.DistrictId);
            _ward.District = district;
            _ward.CreateAt = DateTime.Now;
            await _context.Wards.AddAsync(_ward);
            await _context.SaveChangesAsync();
            return new ApiResponse<WardResponse>(0, "Create Ward success.")
            {
                Data = ToWard(_ward)
            };

        }
        catch (Exception ex)
        {
            return new ApiResponse<WardResponse>(1, "Create Ward error : " + ex);
        }


    }

    public async Task<ApiResponse<WardResponse>> Delete(int id)
    {
        var _ward = await _context.Wards.FindAsync(id);
        if (_ward != null)
        {
            try
            {
                _context.Wards.Remove(_ward);
            }
            catch (Exception ex)

            {
                _ward.IsDelete = true;
            }
            await _context.SaveChangesAsync();
            return new ApiResponse<WardResponse>(0, "Delete Ward success.");
        }
        else
        {
            return new ApiResponse<WardResponse>(1, "Ward does not exist.");
        }
    }

    public async Task<ApiResponse<List<WardResponse>>> GetAll()
    {
        var wards = await _context.Wards.ToListAsync();
        if (wards != null)
        {
            var wardResponses = wards.Select(ward => ToWard(ward)).ToList();
            return new ApiResponse<List<WardResponse>>(0, "GetAll Ward success.")
            {
                Data = wardResponses
            };

        }
        else
        {
            return new ApiResponse<List<WardResponse>>(1, "No Ward found.");
        }
    }
    public async Task<ApiResponse<WardResponse>> Search(int id)
    {
        var ward = await _context.Wards.FindAsync(id);
        if (ward != null)
        {
            return new ApiResponse<WardResponse>(0, "Found success.")
            {
                Data = ToWard(ward)
            };
        }
        else
        {
            return new ApiResponse<WardResponse>(1, "Ward does not exist.");
        }
    }

    public WardResponse ToWard(Ward ward)
    {
        return new WardResponse
        {

            Id = ward.Id,
            Code = ward.Code,
            DistrictId = ward.DistrictId,
            Name = ward.Name,
            NameEn = ward.NameEn,
            CodeName = ward.CodeName,
            FullName = ward.FullName,
            CreateAt = ward.CreateAt,
            UpdateAt = ward.UpdateAt,
            UserCreate = ward.UserCreate,
            UserUpdate = ward.UserUpdate,
            IsDelete = ward.IsDelete


        };
    }

    public Ward ToWardRequest(WardRequest ward)
    {
        return new Ward
        {

            Code = ward.Code,
            DistrictId = ward.DistrictId,
            Name = ward.Name,
            NameEn = ward.NameEn,
            CodeName = ward.CodeName,
            FullName = ward.FullName,
            UserCreate = ward.UserCreate,
            UserUpdate = ward.UserUpdate,



        };
    }

    public async Task<ApiResponse<WardResponse>> Update(int id, WardRequest ward)
    {
        var _ward = await _context.Wards.FindAsync(id);
        if (_ward != null)
        {
            try
            {
                _ward.Code = ward.Code;
                _ward.DistrictId = ward.DistrictId;
                _ward.Name = ward.Name;
                _ward.NameEn = ward.NameEn;
                _ward.CodeName = ward.CodeName;
                _ward.FullName = ward.FullName;
                _ward.UserCreate = ward.UserCreate;
                _ward.UserUpdate = ward.UserUpdate;
                _ward.UpdateAt = DateTime.Now;

                var district = await _context.Districts.FindAsync(ward.DistrictId);
                _ward.District = district;
                await _context.SaveChangesAsync();
                return new ApiResponse<WardResponse>(1, "Updare Ward success")
                {
                    Data = ToWard(_ward)
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<WardResponse>(1, "Updare Ward error : " + ex);
            }
        }
        else
        {
            return new ApiResponse<WardResponse>(1, "Ward does not exist.");
        }
    }

}