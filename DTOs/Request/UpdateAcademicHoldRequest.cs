﻿using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class UpdateAcademicHoldRequest
    {
        [Required(ErrorMessage = "Id là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Id phải lớn hơn 0.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "HoldDate là bắt buộc.")]
        public DateTime HoldDate { get; set; }
        public string? HoldDuration { get; set; }
        public string? Reason { get; set; }
        public string? FileName { get; set; }
    }
}