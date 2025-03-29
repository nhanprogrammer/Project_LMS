using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    [Table("teacher_status_histories")]
    public class TeacherStatusHistory
    {
        [Column("id")]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("TeacherStatus")]
        [Column("teacher_status_id")]
        public int? TeacherStatusId { get; set; }
        public virtual TeacherStatus? TeacherStatus { get; set; }

        [Column("note", TypeName = "text")]
        public string? Note { get; set; }

        [Column("leave_date")]
        public DateTime? LeaveDate { get; set; }

        [Column("is_active")]
        public bool? IsActive { get; set; } = false;

        [Column("is_delete")]
        public bool? IsDelete { get; set; } = false;

        [Column("file_name", TypeName = "text")]
        public string? FileName { get; set; }

        [Column("create_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("update_at")]
        public DateTime? UpdatedAt { get; set; }


        [Column("user_create")]
        public int? UserCreate { get; set; }

        [Column("user_update")]
        public int? UserUpdate { get; set; }
    }
}
