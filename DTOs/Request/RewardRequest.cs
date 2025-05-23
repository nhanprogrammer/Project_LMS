﻿ using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;

namespace Project_LMS.DTOs.Request
{
    public class RewardRequest
    {

        public string? UserCode { get; set; }
        public int? SemesterId { get; set; }

        //public int? RewardCode { get; set; }
        public string? FileName { get; set; } = null!;

        public string RewardContent { get; set; } = null!;
        [JsonIgnore]
        public DateTime? CreateAt { get; set; }
        [JsonIgnore]
        public DateTime? UpdateAt { get; set; }
        [JsonIgnore]
        public int? UserCreate { get; set; }
        [JsonIgnore]
        public int? UserUpdate { get; set; }
    }
    public class UpdateRewardRequest : RewardRequest
    {
        public int id { get; set; }
    }
    public class RewardRequestValidator : AbstractValidator<RewardRequest>
    {
        private readonly ApplicationDbContext _context;

        public RewardRequestValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.UserCode)
                .NotNull().WithMessage("Mã học viên không được để trống.")
                .Must(UserExists).WithMessage("Mã học viên không tồn tại trong hệ thống.")
                .Must(StatusExists).WithMessage("Học viên không thuộc trạng thái đang đi học không được khen thưởng.");

            RuleFor(x => x.SemesterId)
                .NotNull().WithMessage("Mã học kỳ không được để trống.")
                .Must(SemesterExists).WithMessage("Mã học kỳ không tồn tại trong hệ thống.");


            //RuleFor(x => x.RewardName)
            //    .NotEmpty().WithMessage("Tên phần thưởng không được để trống.");

            RuleFor(x => x.RewardContent)
                .NotEmpty().WithMessage("Nội dung phần thưởng không được để trống.");
           
        }

        private bool UserExists(string? userCode)
        {
            if (userCode == null) return false; // Kiểm tra ID hợp lệ

            var user =  _context.Users
                .FirstOrDefault(u => u.UserCode == userCode && u.IsDelete ==false);

            return user != null; 
        }     
        private bool StatusExists(string? userCode)
        {
            if (userCode == null) return false; // Kiểm tra ID hợp lệ

            var user =  _context.Users
                .FirstOrDefault(u => u.UserCode == userCode && u.IsDelete ==false && u.StudentStatusId ==1);

            return user != null; 
        } 
        private bool SemesterExists(int? semesterId)
        {
            if (semesterId <= 0) return false;

            var user =  _context.Semesters
                .FirstOrDefault(u => u.Id == semesterId && u.IsDelete == false);

            return user != null; 
        }
    }

}