using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class RegistrationContactRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "RegistrationId is required")]
        public int? RegistrationId { get; set; }
        [Required(ErrorMessage = "FamilyName is required")]
        [MaxLength(100, ErrorMessage = "FamilyName cannot exceed 100 characters")]
        public string FamilyName { get; set; } = null!;
        [Required(ErrorMessage = "FamilyAddress is required")]
        [MaxLength(200, ErrorMessage = "FamilyAddress cannot exceed 200 characters")]
        public string FamilyAddress { get; set; } = null!;
        [Required(ErrorMessage = "FamilyNumber is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(15, ErrorMessage = "FamilyNumber cannot exceed 15 characters")]
        public string FamilyNumber { get; set; } = null!;
    }
}