using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class RewardRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "StudentId is required")]
        public int? StudentId { get; set; }
        [Required(ErrorMessage = "SemesterId is required")]
        public int? SemesterId { get; set; }
        [Required(ErrorMessage = "RewardCode is required")]
        public int? RewardCode { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "RewardContent is required")]
        public string RewardContent { get; set; } = null!;
    }
}