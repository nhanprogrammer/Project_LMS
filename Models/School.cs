using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class School
    {
        public School()
        {
            SchoolBranches = new HashSet<SchoolBranch>();
        }

        public int Id { get; set; }
        public string? SchoolCode { get; set; } // Mã trường
        public string? Name { get; set; } // Tên trường
        public string? Principal { get; set; } // Hiệu trưởng
        public string? PrincipalPhone { get; set; } // SĐT hiệu trưởng
        public string? Image { get; set; } // Đường dẫn ảnh
        public string? Email { get; set; } // Email
        public string? Website { get; set; } // Website
        public string? Province { get; set; } // Tỉnh/Thành phố
        public string? District { get; set; } // Quận/Huyện
        public string? Ward { get; set; } // Xã/Phường
        public string? Fax { get; set; } // Số fax
        public bool? Thcs { get; set; } // Là THCS
        public bool? Thpt { get; set; } // Là THPT
        public string? EducationModel { get; set; } // Mô hình giáo dục
        public string? Phone { get; set; } // SĐT chung
        public DateTime? EstablishmentDate { get; set; } // Ngày thành lập
        public DateTime? CreateAt { get; set; } // Thời gian tạo
        public DateTime? UpdateAt { get; set; } // Thời gian cập nhật
        public int? UserCreate { get; set; } // Người tạo
        public int? UserUpdate { get; set; } // Người cập nhật
        public bool? IsDelete { get; set; } // Cờ xóa mềm
        public bool? HeadOffice { get; set; } // Trụ sở chính

        public virtual ICollection<SchoolBranch> SchoolBranches { get; set; }
    }
}