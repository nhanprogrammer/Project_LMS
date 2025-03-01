using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class RegistrationRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "NationalityId is required")]
        public int? NationalityId { get; set; }
        [Required(ErrorMessage = "SchoolId is required")]
        public int? SchoolId { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Course is required")]
        [MaxLength(100, ErrorMessage = "Course cannot exceed 100 characters")]
        public string Course { get; set; } = null!;
        [Required(ErrorMessage = "Fullname is required")]
        [MaxLength(150, ErrorMessage = "Fullname cannot exceed 150 characters")]
        public string Fullname { get; set; } = null!;
        [Required(ErrorMessage = "Birthday is required")]
        public DateTime Birthday { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public bool? Gender { get; set; }
        [Required(ErrorMessage = "Education is required")]
        [MaxLength(100, ErrorMessage = "Education cannot exceed 100 characters")]
        public string Education { get; set; } = null!;
        [Required(ErrorMessage = "CurrentSchool is required")]
        [MaxLength(150, ErrorMessage = "CurrentSchool cannot exceed 150 characters")]
        public string CurrentSchool { get; set; } = null!;
        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = null!;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "PhoneNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(15, ErrorMessage = "PhoneNumber cannot exceed 15 characters")]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "Image is required")]
        [MaxLength(200, ErrorMessage = "Image URL cannot exceed 200 characters")]
        public string Image { get; set; } = null!;
    }
}