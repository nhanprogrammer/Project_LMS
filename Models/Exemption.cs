using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Project_LMS.Models
{

    [Table("exemptions")]
    public class Exemption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }  // ID tự tăng

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }  // Khóa ngoại tham chiếu Users
        [Column("exempted_subject")]
        public string? ExemptedSubject { get; set; }  // Môn học được miễn
        [Column("expression")]
        public string? Expression { get; set; }  // Lý do miễn
        [Column("create_at")]
        public DateTime? CreateAt { get; set; }
        [Column("update_at")]
        public DateTime? UpdateAt { get; set; }
        [Column("user_create")]
        public int? UserCreate { get; set; }
        [Column("user_update")]
        public int? UserUpdate { get; set; }
        [Column("is_delete")]
        public bool? IsDelete { get; set; }

        // Khóa ngoại - Thiết lập quan hệ với bảng Users
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }

}
