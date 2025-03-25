using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Project_LMS.Helpers;
using Project_LMS.Models;

namespace Project_LMS.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AcademicHold> AcademicHolds { get; set; } = null!;
        public virtual DbSet<AcademicYear> AcademicYears { get; set; } = null!;
        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Assignment> Assignments { get; set; } = null!;
        public virtual DbSet<AssignmentDetail> AssignmentDetails { get; set; } = null!;
        public virtual DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<ClassOnline> ClassOnlines { get; set; } = null!;
        public virtual DbSet<ClassStudent> ClassStudents { get; set; } = null!;
        public virtual DbSet<ClassStudentOnline> ClassStudentOnlines { get; set; } = null!;
        public virtual DbSet<ClassSubject> ClassSubjects { get; set; } = null!;
        public virtual DbSet<ClassTestExam> ClassTestExams { get; set; } = null!;
        public virtual DbSet<ClassType> ClassTypes { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Dependent> Dependents { get; set; } = null!;
        public virtual DbSet<Discipline> Disciplines { get; set; } = null!;
        public virtual DbSet<Examiner> Examiners { get; set; } = null!;
        public virtual DbSet<Favourite> Favourites { get; set; } = null!;
        public virtual DbSet<FileFormat> FileFormats { get; set; } = null!;
        public virtual DbSet<GroupModulePermisson> GroupModulePermissons { get; set; } = null!;
        public virtual DbSet<Lesson> Lessons { get; set; } = null!;
        public virtual DbSet<Module> Modules { get; set; } = null!;
        public virtual DbSet<ModulePermission> ModulePermissions { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionAnswer> QuestionAnswers { get; set; } = null!;
        public virtual DbSet<QuestionAnswerTopicView> QuestionAnswerTopicViews { get; set; } = null!;
        public virtual DbSet<Reward> Rewards { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<SchoolBranch> SchoolBranches { get; set; } = null!;
        public virtual DbSet<SchoolTransfer> SchoolTransfers { get; set; } = null!;
        public virtual DbSet<Semester> Semesters { get; set; } = null!;
        public virtual DbSet<StatusAssignment> StatusAssignments { get; set; } = null!;
        public virtual DbSet<StudentStatus> StudentStatuses { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectGroup> SubjectGroups { get; set; } = null!;
        public virtual DbSet<SubjectGroupSubject> SubjectGroupSubjects { get; set; } = null!;
        public virtual DbSet<SubjectType> SubjectTypes { get; set; } = null!;
        public virtual DbSet<SubmissionFile> SubmissionFiles { get; set; } = null!;
        public virtual DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public virtual DbSet<TeacherClassSubject> TeacherClassSubjects { get; set; } = null!;
        public virtual DbSet<TeacherStatus> TeacherStatuses { get; set; } = null!;
        public virtual DbSet<TeachingAssignment> TeachingAssignments { get; set; } = null!;
        public virtual DbSet<TestExam> TestExams { get; set; } = null!;
        public virtual DbSet<TestExamType> TestExamTypes { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<TrainingRank> TrainingRanks { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserTrainingRank> UserTrainingRanks { get; set; } = null!;
        public virtual DbSet<ExamScheduleStatus> ExamScheduleStatusEnumerable { get; set; } = null!;
        public virtual DbSet<Exemption> Exemptions { get; set; } = null!;
        public virtual DbSet<TeacherStatusHistory> TeacherStatusHistories { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(
                    "Host=dpg-cv6l940gph6c73dnr7hg-a.oregon-postgres.render.com;Port=5432;Database=lms_rvdc;Username=lms_rvdc_user;Password=GJKc4tITIEh9s1MXQ97tH94RTR8xia8G");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Exemption>()
                .HasOne(e => e.User)
                .WithMany(u => u.Exemptions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.SubjectGroup)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SubjectGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherStatusHistory>()
                .HasOne(t => t.User)
                .WithMany(u => u.TeacherStatusHistories)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AcademicHold>(entity =>
            {
                entity.ToTable("academic_holds");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FileName).HasColumnName("file_name");

                entity.Property(e => e.HoldDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("hold_date");

                entity.Property(e => e.HoldDuration).HasColumnName("hold_duration");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AcademicHolds)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_academic_holds_user");
            });

            modelBuilder.Entity<AcademicYear>(entity =>
            {
                entity.ToTable("academic_years");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcademicParent).HasColumnName("academic_parent");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsInherit)
                    .HasColumnName("is_inherit")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.ToTable("answers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Answer1)
                    .HasColumnType("character varying")
                    .HasColumnName("answer");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsCorrect)
                    .HasColumnName("is_correct")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.QuestionId).HasColumnName("question_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_answers_question");
            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.ToTable("assignments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Comment).HasColumnName("comment");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsSubmit).HasColumnName("is_submit");

                entity.Property(e => e.StatusAssignmentId).HasColumnName("status_assignment_id");

                entity.Property(e => e.SubmissionDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("submission_date");

                entity.Property(e => e.SubmissionFile)
                    .HasColumnType("character varying")
                    .HasColumnName("submission_file");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.TotalScore).HasColumnName("total_score");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.StatusAssignment)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.StatusAssignmentId)
                    .HasConstraintName("fk_assignments_status_assignment");

                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.TestExamId)
                    .HasConstraintName("fk_assignments_test_exam");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_assignments_user");
            });

            modelBuilder.Entity<AssignmentDetail>(entity =>
            {
                entity.ToTable("assignment_details");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AnswerId).HasColumnName("answer_id");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsCorrect)
                    .HasColumnName("is_correct")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.AssignmentDetails)
                    .HasForeignKey(d => d.AnswerId)
                    .HasConstraintName("fk_assignment_details_answer");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.AssignmentDetails)
                    .HasForeignKey(d => d.AssignmentId)
                    .HasConstraintName("fk_assignment_details_assignment");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("chat_messages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassOnlineId).HasColumnName("class_online_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.FileType)
                    .HasMaxLength(50)
                    .HasColumnName("file_type");

                entity.Property(e => e.FileUrl)
                    .HasMaxLength(255)
                    .HasColumnName("file_url");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsPinned)
                    .HasColumnName("is_pinned")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsQuestion)
                    .HasColumnName("is_question")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.MessageContent).HasColumnName("message_content");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.ClassOnline)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.ClassOnlineId)
                    .HasConstraintName("fk_chat_messages_class_online");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_chat_messages_user");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("classes");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcademicYearId).HasColumnName("academic_year_id");

                entity.Property(e => e.ClassCode)
                    .HasMaxLength(255)
                    .HasColumnName("class_code");


                entity.Property(e => e.ClassTypeId).HasColumnName("class_type_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordClass)
                    .HasMaxLength(50)
                    .HasColumnName("password_class");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.StatusClass).HasColumnName("status_class");

                entity.Property(e => e.StudentCount).HasColumnName("student_count");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.AcademicYearId)
                    .HasConstraintName("fk_classes_academic_year");

                entity.HasOne(d => d.ClassType)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ClassTypeId)
                    .HasConstraintName("fk_classes_class_type");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("fk_classes_department");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_classes_user");
            });

            modelBuilder.Entity<ClassOnline>(entity =>
            {
                entity.ToTable("class_onlines");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ChatCode).HasColumnName("chat_code");

                entity.Property(e => e.ClassOnlineCode)
                    .HasMaxLength(50)
                    .HasColumnName("class_online_code");

                entity.Property(e => e.ClassDescription).HasColumnName("class_description");

                entity.Property(e => e.ClassLink)
                    .HasMaxLength(255)
                    .HasColumnName("class_link");

                entity.Property(e => e.ClassPassword)
                    .HasMaxLength(100)
                    .HasColumnName("class_password");

                entity.Property(e => e.ClassStatus)
                    .HasColumnName("class_status")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.ClassTitle)
                    .HasMaxLength(100)
                    .HasColumnName("class_title");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CurrentStudents).HasColumnName("current_students");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.MaxStudents).HasColumnName("max_students");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.User)
                 .WithMany(p => p.ClassOnlines)
                 .HasForeignKey(d => d.UserId)
                 .HasConstraintName("fk_class_online_user");

                entity.Property(e => e.LessonId).HasColumnName("lesson_id");

                entity.HasOne(d => d.Lesson)
                .WithMany(p => p.ClassOnlines)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("fk_class_online_lesson");


            });

            modelBuilder.Entity<ClassStudent>(entity =>
            {
                entity.ToTable("class_students");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassStudents)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_class_students_class");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ClassStudents)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_class_students_student");
            });

            modelBuilder.Entity<ClassStudentOnline>(entity =>
            {
                entity.ToTable("class_student_onlines");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddedAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("added_at");

                entity.Property(e => e.ClassOnlineId).HasColumnName("class_online_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsAdmin)
                    .HasColumnName("is_admin")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsCamera)
                    .HasColumnName("is_camera")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsMuted)
                    .HasColumnName("is_muted")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.JoinTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("join_time");

                entity.Property(e => e.LeaveTime)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("leave_time");

                entity.Property(e => e.RemoveAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("remove_at");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.ClassOnline)
                    .WithMany(p => p.ClassStudentOnlines)
                    .HasForeignKey(d => d.ClassOnlineId)
                    .HasConstraintName("fk_class_students_online_class");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ClassStudentOnlines)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_class_students_online_user");
            });

            modelBuilder.Entity<ClassSubject>(entity =>
            {
                entity.ToTable("class_subjects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.SubjectId).HasColumnName("subject_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassSubjects)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_class_subjects_class");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.ClassSubjects)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_class_subjects_student");
            });

            modelBuilder.Entity<ClassTestExam>(entity =>
            {
                entity.ToTable("class_test_exams");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassTestExams)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_class_test_exams_class");

                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.ClassTestExams)
                    .HasForeignKey(d => d.TestExamId)
                    .HasConstraintName("fk_class_test_exams_exam");
            });

            modelBuilder.Entity<ClassType>(entity =>
            {
                entity.ToTable("class_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("departments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentCode)
                    .HasMaxLength(255)
                    .HasColumnName("department_code");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_departments_user");
            });

            modelBuilder.Entity<Dependent>(entity =>
            {
                entity.ToTable("dependents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DependentCode).HasColumnName("dependent_code");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Discipline>(entity =>
            {
                entity.ToTable("disciplines");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DisciplineContent)
                    .HasMaxLength(1000)
                    .HasColumnName("discipline_content");

                entity.Property(e => e.DisciplineDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("discipline_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.SemesterId).HasColumnName("semester_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Disciplines)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("fk_disciplines_semester");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Disciplines)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_disciplines_user");
            });

            modelBuilder.Entity<Examiner>(entity =>
            {
                entity.ToTable("examiners");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
                entity.Property(e => e.ClassId).HasColumnName("class_id");
                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.Examiners)
                    .HasForeignKey(d => d.TestExamId)
                    .HasConstraintName("fk_examiners_exam");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Examiners)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_examiners_user");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Examiners)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_examiners_class");
            });

            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.ToTable("favourites");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.QuestionsAnswerId).HasColumnName("questions_answer_id");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.QuestionsAnswer)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.QuestionsAnswerId)
                    .HasConstraintName("fk_favourites_questions_answer");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("fk_favourites_topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_favourites_user");
            });

            modelBuilder.Entity<FileFormat>(entity =>
            {
                entity.ToTable("file_formats");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Is10).HasColumnName("is_10");

                entity.Property(e => e.Is20).HasColumnName("is_20");

                entity.Property(e => e.Is30).HasColumnName("is_30");

                entity.Property(e => e.Is40).HasColumnName("is_40");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsDoc).HasColumnName("is_doc");

                entity.Property(e => e.IsJpeg).HasColumnName("is_jpeg");

                entity.Property(e => e.IsPowerpoint).HasColumnName("is_powerpoint");

                entity.Property(e => e.IsXxls).HasColumnName("is_xxls");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<GroupModulePermisson>(entity =>
            {
                entity.ToTable("group_module_permissons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("lessons");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TeachingAssignmentId).HasColumnName("teaching_assignment_id");
                entity.Property(e => e.ClassLessonCode)
                    .HasMaxLength(255)
                    .HasColumnName("class_lesson_code");
                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Duration)
                    .HasMaxLength(50)
                    .HasColumnName("duration");
                entity.Property(e => e.LessonLink)
                    .HasColumnType("character varying")
                    .HasColumnName("lesson_link");
                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");
                entity.Property(e => e.IsAutoStart)
                    .HasColumnName("is_auto_start")
                    .HasDefaultValueSql("false");
                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");
                entity.Property(e => e.IsResearchable)
                    .HasColumnName("is_researchable")
                    .HasDefaultValueSql("false");
                entity.Property(e => e.IsSave)
                    .HasColumnName("is_save")
                    .HasDefaultValueSql("false");
                entity.Property(e => e.PaswordLeassons).HasColumnName("pasword_leassons");
                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");
                entity.Property(e => e.Topic)
                    .HasMaxLength(255)
                    .HasColumnName("topic");
                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UserCreate).HasColumnName("user_create");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.TeachingAssignment)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.TeachingAssignmentId)
                    .HasConstraintName("lessons_teaching_assignments_fk");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_lessons_user");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("modules");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
                entity.Property(e => e.DisplayName).HasColumnName("display_name");
            });

            modelBuilder.Entity<ModulePermission>(entity =>
            {
                entity.ToTable("module_permissions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EnterScore).HasColumnName("enter_score");

                entity.Property(e => e.GroupRoleId).HasColumnName("group_role_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsInsert).HasColumnName("is_insert");

                entity.Property(e => e.IsUpdate).HasColumnName("is_update");

                entity.Property(e => e.IsView).HasColumnName("is_view");

                entity.Property(e => e.ModuleId).HasColumnName("module_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");


                entity.HasOne(d => d.GroupModulePermisson)
                    .WithMany(p => p.ModulePermissions)
                    .HasForeignKey(d => d.GroupRoleId)
                    .HasConstraintName("fk_module_permissions_group_module_permissons");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.ModulePermissions)
                    .HasForeignKey(d => d.ModuleId)
                    .HasConstraintName("fk_module_permissions_modules");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .HasMaxLength(1024)
                    .HasColumnName("content");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsRead).HasColumnName("is_read");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.Property(e => e.Subject)
                    .HasMaxLength(255)
                    .HasColumnName("subject");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.NotificationSenders)
                    .HasForeignKey(d => d.SenderId)
                    .HasConstraintName("fk_notifications_sender");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NotificationUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_notifications_user");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Mark).HasColumnName("mark");

                entity.Property(e => e.QuestionText).HasColumnName("question");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TestExamId)
                    .HasConstraintName("fk_questions_test_exam");
            });

            modelBuilder.Entity<QuestionAnswer>(entity =>
            {
                entity.ToTable("question_answers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Message).HasColumnName("message");

                entity.Property(e => e.QuestionsAnswerId).HasColumnName("questions_answer_id");

                entity.Property(e => e.TeachingAssignmentId).HasColumnName("teaching_assignment_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.QuestionsAnswer)
                    .WithMany(p => p.InverseQuestionsAnswer)
                    .HasForeignKey(d => d.QuestionsAnswerId)
                    .HasConstraintName("fk_questions_answers_questions_answers");

                entity.HasOne(d => d.TeachingAssignment)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.TeachingAssignmentId)
                    .HasConstraintName("fk_questions_answers_teaching_assignment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuestionAnswers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_questions_answers_user");
            });

            modelBuilder.Entity<QuestionAnswerTopicView>(entity =>
            {
                entity.ToTable("question_answer_topic_views");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.QuestionsAnswerId).HasColumnName("questions_answer_id");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Topic)
                    .WithMany(t => t.QuestionAnswerTopicViews)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("fk_topic_question_answer_topic_views")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(u => u.QuestionAnswerTopicViews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_User_question_answer_topic_views")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.QuestionAnswer)
                    .WithMany(q => q.QuestionAnswerTopicViews)
                    .HasForeignKey(d => d.QuestionsAnswerId)
                    .HasConstraintName("fk_question_answer_topic_views_questions_answers")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Reward>(entity =>
            {
                entity.ToTable("rewards");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                //entity.Property(e => e.RewardCode).HasColumnName("reward_code");

                entity.Property(e => e.RewardContent)
                    .HasMaxLength(500)
                    .HasColumnName("reward_content");

                entity.Property(e => e.RewardDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("reward_date");

                entity.Property(e => e.RewardName)
                    .HasMaxLength(255)
                    .HasColumnName("reward_name");

                entity.Property(e => e.SemesterId).HasColumnName("semester_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Rewards)
                    .HasForeignKey(d => d.SemesterId)
                    .HasConstraintName("fk_rewards_semester");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Rewards)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_rewards_user");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<School>(entity =>
            {
                entity.ToTable("schools");

                entity.HasIndex(e => e.SchoolCode, "schools_school_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EducationModel)
                    .HasMaxLength(255)
                    .HasColumnName("education_model");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.EstablishmentDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("establishment_date");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .HasColumnName("fax");

                entity.Property(e => e.HeadOffice)
                    .HasMaxLength(225)
                    .HasColumnName("head_office");

                entity.Property(e => e.Image)
                    .HasColumnName("image");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasColumnName("phone");

                entity.Property(e => e.Principal)
                    .HasMaxLength(255)
                    .HasColumnName("principal");

                entity.Property(e => e.PrincipalPhone)
                    .HasMaxLength(255)
                    .HasColumnName("principal_phone");

                entity.Property(e => e.Province)
                    .HasMaxLength(50)
                    .HasColumnName("province");

                entity.Property(e => e.District)
                    .HasMaxLength(50)
                    .HasColumnName("district");

                entity.Property(e => e.Ward)
                    .HasMaxLength(50)
                    .HasColumnName("ward");

                entity.Property(e => e.SchoolCode)
                    .HasMaxLength(50)
                    .HasColumnName("school_code");

                entity.Property(e => e.IsJuniorHigh)
                    .HasColumnName("is_junior_high")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsHighSchool)
                    .HasColumnName("is_high_school")
                    .HasDefaultValueSql("false");
                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate)
                    .HasColumnName("user_create");

                entity.Property(e => e.UserUpdate)
                    .HasColumnName("user_update");

                entity.Property(e => e.Website)
                    .HasMaxLength(255)
                    .HasColumnName("website");
            });

            modelBuilder.Entity<SchoolBranch>(entity =>
            {
                entity.ToTable("school_branches");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");
                entity.Property(e => e.SchoolPhone)
              .HasMaxLength(10)
              .HasColumnName("school_phone");

                entity.Property(e => e.BranchName)
                    .HasMaxLength(255)
                    .HasColumnName("branch_name");
                entity.Property(e => e.Image)
                                    .HasColumnName("image");
                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Manager)
                    .HasMaxLength(255)
                    .HasColumnName("manager");

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .HasColumnName("phone");


                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.SchoolBranches)
                    .HasForeignKey(d => d.SchoolId)
                    .HasConstraintName("fk_school_branches_school");
            });

            modelBuilder.Entity<SchoolTransfer>(entity =>
            {
                entity.ToTable("school_transfers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.SchoolBranchesId).HasColumnName("school_branches_id");

                entity.Property(e => e.Semester)
                    .HasMaxLength(50)
                    .HasColumnName("semester");

                entity.Property(e => e.Status)
                    .HasColumnType("bit(1)")
                    .HasColumnName("status");

                entity.Property(e => e.TransferDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("transfer_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SchoolTransfers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_school_transfers_user");
            });

            modelBuilder.Entity<Semester>(entity =>
            {
                entity.ToTable("semesters");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcademicYearId).HasColumnName("academic_year_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.Semesters)
                    .HasForeignKey(d => d.AcademicYearId)
                    .HasConstraintName("fk_semesters_academic_year");
            });

            modelBuilder.Entity<StatusAssignment>(entity =>
            {
                entity.ToTable("status_assignments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(100)
                    .HasColumnName("status_name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<StudentStatus>(entity =>
            {
                entity.ToTable("student_statuses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(255)
                    .HasColumnName("status_name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("subjects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsStatus)
                    .HasColumnName("is_status")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Semester1PeriodCount).HasColumnName("semester_1_period_count");

                entity.Property(e => e.Semester2PeriodCount).HasColumnName("semester_2_period_count");

                entity.Property(e => e.SubjectCode)
                    .HasMaxLength(50)
                    .HasColumnName("subject_code");

                entity.Property(e => e.SubjectName)
                    .HasMaxLength(100)
                    .HasColumnName("subject_name");

                entity.Property(e => e.SubjectTypeId).HasColumnName("subject_type_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.SubjectType)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.SubjectTypeId)
                    .HasConstraintName("fk_subjects_subject_type");
            });

            modelBuilder.Entity<SubjectGroup>(entity =>
            {
                entity.ToTable("subject_groups");


                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd()
                    .IsRequired();


                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SubjectGroups)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_subjects_group_user");
            });

            modelBuilder.Entity<SubjectGroupSubject>(entity =>
            {
                entity.ToTable("subject_group_subjects");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd()
                    .IsRequired();

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.SubjectGroupId).HasColumnName("subject_group_id");

                entity.Property(e => e.SubjectId).HasColumnName("subject_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.SubjectGroup)
                    .WithMany(p => p.SubjectGroupSubjects)
                    .HasForeignKey(d => d.SubjectGroupId)
                    .HasConstraintName("fk_subject_group_subjects_subject_group_subject");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.SubjectGroupSubjects)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_subject_group_subjects_subject");
            });

            modelBuilder.Entity<SubjectType>(entity =>
            {
                entity.ToTable("subject_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Note).HasColumnName("note");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasDefaultValueSql("true");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<SubmissionFile>(entity =>
            {
                entity.ToTable("submission_files");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AssignmentId).HasColumnName("assignment_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<SystemSetting>(entity =>
            {
                entity.ToTable("system_settings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CaptchaEnabled).HasColumnName("captcha_enabled");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CurrentTheme)
                    .HasMaxLength(50)
                    .HasColumnName("current_theme");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Language)
                    .HasMaxLength(30)
                    .HasColumnName("language");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SystemSettings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_system_settings_user");
            });

            modelBuilder.Entity<TeacherClassSubject>(entity =>
            {
                entity.ToTable("teacher_class_subjects");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsPrimary)
                    .HasColumnName("is_primary")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.SubjectsId).HasColumnName("subjects_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Subjects)
                    .WithMany(p => p.TeacherClassSubjects)
                    .HasForeignKey(d => d.SubjectsId)
                    .HasConstraintName("fk_teacher_class_subject_subject");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TeacherClassSubjects)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_teacher_class_subject_user");
            });

            modelBuilder.Entity<TeacherStatus>(entity =>
            {
                entity.ToTable("teacher_statuses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(255)
                    .HasColumnName("status_name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<TeachingAssignment>(entity =>
            {
                entity.ToTable("teaching_assignments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("end_date");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.SubjectId).HasColumnName("subject_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.TeachingAssignments)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_teaching_assignments_class");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.TeachingAssignments)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_teaching_assignments_subject");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TeachingAssignments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_teaching_assignments_user");
            });

            modelBuilder.Entity<TestExam>(entity =>
 {
     entity.ToTable("test_exams");

     entity.Property(e => e.Id).HasColumnName("id");

     entity.Property(e => e.Attachment).HasColumnName("attachment");

     entity.Property(e => e.ClassId).HasColumnName("class_id");

     entity.Property(e => e.CreateAt)
         .HasColumnType("timestamp with time zone")
         .HasColumnName("create_at")
         .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

     entity.Property(e => e.Description).HasColumnName("description");

     entity.Property(e => e.Duration).HasColumnName("duration");

     entity.Property(e => e.EndDate)
         .HasColumnType("timestamp with time zone")
         .HasColumnName("end_date");

     entity.Property(e => e.Form)
         .HasMaxLength(30)
         .HasColumnName("form");

     entity.Property(e => e.IsExam)
         .HasColumnName("is_exam")
         .HasDefaultValueSql("false");

     entity.Property(e => e.IsDelete)
         .HasColumnName("is_delete")
         .HasDefaultValueSql("false");

     entity.Property(e => e.SemestersId).HasColumnName("semesters_id");

     entity.Property(e => e.StartDate)
         .HasColumnType("timestamp with time zone")
         .HasColumnName("start_date");

     entity.Property(e => e.TestExamTypeId).HasColumnName("test_exam_type_id");
     entity.Property(e => e.SubjectId).HasColumnName("subject_id");
     entity.Property(e => e.DepartmentId).HasColumnName("department_id");
     entity.Property(e => e.ScheduleStatusId).HasColumnName("schedule_status_id");
     entity.Property(e => e.Topic)
         .HasMaxLength(50)
         .HasColumnName("topic");

     entity.Property(e => e.UpdateAt)
         .HasColumnType("timestamp with time zone")
         .HasColumnName("update_at")
         .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

     entity.Property(e => e.UserId).HasColumnName("user_id");

     entity.HasOne(d => d.Class)
         .WithMany(p => p.TestExams)
         .HasForeignKey(d => d.ClassId)
         .HasConstraintName("fk_test_exams_class");

     entity.HasOne(d => d.Semesters)
         .WithMany(p => p.TestExams)
         .HasForeignKey(d => d.SemestersId)
         .HasConstraintName("fk_test_exams_semester");

     entity.HasOne(d => d.TestExamType)
         .WithMany(p => p.TestExams)
         .HasForeignKey(d => d.TestExamTypeId)
         .HasConstraintName("fk_test_exams_test_exam_type");

     entity.HasOne(d => d.User)
         .WithMany(p => p.TestExams)
         .HasForeignKey(d => d.UserId)
         .HasConstraintName("fk_test_exams_user");

     entity.HasOne(d => d.Subject)
         .WithMany(p => p.TestExams)
         .HasForeignKey(d => d.SubjectId)
         .HasConstraintName("fk_test_exams_subjects");

     entity.HasOne(e => e.ExamScheduleStatus)
         .WithMany(e => e.TestExams)
         .HasForeignKey(e => e.ScheduleStatusId)
         .HasConstraintName("fk_test_exams_exam_schedule_status");

     entity.HasOne(d => d.Department)
         .WithMany(p => p.TestExams)
         .HasForeignKey(d => d.DepartmentId)
         .HasConstraintName("fk_test_exam_department");
 });

            modelBuilder.Entity<TestExam>(entity =>
            {
                entity.ToTable("test_exams");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachment).HasColumnName("attachment");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.EndDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("end_date");

                entity.Property(e => e.Form)
                    .HasMaxLength(30)
                    .HasColumnName("form");

                entity.Property(e => e.IsExam)
                    .HasColumnName("is_exam")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.SemestersId).HasColumnName("semesters_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.TestExamTypeId).HasColumnName("test_exam_type_id");
                entity.Property(e => e.SubjectId).HasColumnName("subject_id");
                entity.Property(e => e.DepartmentId).HasColumnName("department_id");
                entity.Property(e => e.ScheduleStatusId).HasColumnName("schedule_status_id");
                entity.Property(e => e.Topic)
                    .HasMaxLength(50)
                    .HasColumnName("topic");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.IsAttachmentRequired).HasColumnName("is_attachment_required");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_test_exams_class");

                entity.HasOne(d => d.Semesters)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.SemestersId)
                    .HasConstraintName("fk_test_exams_semester");

                entity.HasOne(d => d.TestExamType)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.TestExamTypeId)
                    .HasConstraintName("fk_test_exams_test_exam_type");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_test_exams_user");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("fk_test_exams_subjects");

                entity.HasOne(e => e.ExamScheduleStatus)
                    .WithMany(e => e.TestExams)
                    .HasForeignKey(e => e.ScheduleStatusId)
                    .HasConstraintName("fk_test_exams_exam_schedule_status");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("fk_test_exam_department");
            });


            modelBuilder.Entity<TestExamType>(entity =>
            {
                entity.ToTable("test_exam_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Coefficient).HasColumnName("coefficient");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.MinimunEntriesSem1).HasColumnName("minimun_entries_sem1");

                entity.Property(e => e.MinimunEntriesSem2).HasColumnName("minimun_entries_sem2");

                entity.Property(e => e.PointTypeName)
                    .HasMaxLength(255)
                    .HasColumnName("point_type_name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("topics");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloseAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("close_at");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasColumnName("file_name");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.TeachingAssignmentId).HasColumnName("teaching_assignment_id");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.TeachingAssignment)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.TeachingAssignmentId)
                    .HasConstraintName("fk_topics_teaching_assignment");

                entity.HasOne(d => d.TopicNavigation)
                    .WithMany(p => p.InverseTopicNavigation)
                    .HasForeignKey(d => d.TopicId)
                    .HasConstraintName("fk_topics_topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_topics_user");
            });

            modelBuilder.Entity<TrainingRank>(entity =>
            {
                entity.ToTable("training_ranks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.EducationLevel)
                    .HasMaxLength(512)
                    .HasColumnName("education_level");

                entity.Property(e => e.ElectiveModule).HasColumnName("elective_module");

                entity.Property(e => e.FormTraining)
                    .HasMaxLength(512)
                    .HasColumnName("form_training");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsModule).HasColumnName("is_module");

                entity.Property(e => e.IsYear).HasColumnName("is_year");

                entity.Property(e => e.RequiredModule).HasColumnName("required_module");

                entity.Property(e => e.SemesterYear).HasColumnName("semester_year");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Disable).HasColumnName("disable");
                entity.Property(e => e.ResetCode).HasColumnName("reset_code");
                entity.Property(e => e.ResetCodeExpiry).HasColumnName("reset_code_expiry");
                entity.Property(e => e.PermissionChanged).HasColumnName("permission_changed");
                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.AdmissionType)
                    .HasMaxLength(100)
                    .HasColumnName("admission_type");

                entity.Property(e => e.Alias)
                    .HasMaxLength(50)
                    .HasColumnName("alias");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("birth_date");

                entity.Property(e => e.BirthFather)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("birth_father");

                entity.Property(e => e.BirthGuardianship)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("birth_guardianship");

                entity.Property(e => e.BirthMother)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("birth_mother");

                entity.Property(e => e.Card)
                    .HasMaxLength(50)
                    .HasColumnName("card");

                entity.Property(e => e.CardIssueDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("card_issue_date");

                entity.Property(e => e.CardIssuePlace)
                    .HasMaxLength(255)
                    .HasColumnName("card_issue_place");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DistrictId)
                    .HasMaxLength(50)
                    .HasColumnName("district_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(255)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullnameFather)
                    .HasMaxLength(50)
                    .HasColumnName("fullname_father");

                entity.Property(e => e.FullnameGuardianship)
                    .HasMaxLength(50)
                    .HasColumnName("fullname_guardianship");

                entity.Property(e => e.FullnameMother)
                    .HasMaxLength(50)
                    .HasColumnName("fullname_mother");

                entity.Property(e => e.Gender)
                    .HasColumnType("bit(1)")
                    .HasColumnName("gender");

                entity.Property(e => e.GroupModulePermissonId).HasColumnName("group_module_permisson_id");
                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.National)
                    .HasMaxLength(100)
                    .HasColumnName("national");

                entity.Property(e => e.PartyJoinDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("party_join_date");

                entity.Property(e => e.PartyMember).HasColumnName("party_member");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .HasColumnName("phone");

                entity.Property(e => e.PhoneFather)
                    .HasMaxLength(20)
                    .HasColumnName("phone_father");

                entity.Property(e => e.PhoneGuardianship)
                    .HasMaxLength(50)
                    .HasColumnName("phone_guardianship");

                entity.Property(e => e.PhoneMother)
                    .HasMaxLength(20)
                    .HasColumnName("phone_mother");

                entity.Property(e => e.PlaceOfBirth)
                    .HasMaxLength(255)
                    .HasColumnName("place_of_birth");

                entity.Property(e => e.ProvinceId)
                    .HasMaxLength(50)
                    .HasColumnName("province_id");

                entity.Property(e => e.Religion)
                    .HasMaxLength(255)
                    .HasColumnName("religion");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.StudentStatusId).HasColumnName("student_status_id");

                entity.Property(e => e.StudyMode)
                    .HasMaxLength(255)
                    .HasColumnName("study_mode");

                entity.Property(e => e.TeacherStatusId).HasColumnName("teacher_status_id");

                entity.Property(e => e.UnionJoinDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("union_join_date");

                entity.Property(e => e.UnionMember).HasColumnName("union_member");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCode)
                    .HasMaxLength(255)
                    .HasColumnName("user_code");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.Property(e => e.WardId)
                    .HasMaxLength(50)
                    .HasColumnName("ward_id");

                entity.Property(e => e.WorkFather)
                    .HasMaxLength(100)
                    .HasColumnName("work_father");

                entity.Property(e => e.WorkGuardianship)
                    .HasMaxLength(50)
                    .HasColumnName("work_guardianship");

                entity.Property(e => e.WorkMother)
                    .HasMaxLength(100)
                    .HasColumnName("work_mother");

                entity.HasOne(d => d.GroupModulePermisson)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.GroupModulePermissonId)
                    .HasConstraintName("fk_users_group_module_permisson");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("fk_users_role");

                entity.HasOne(d => d.StudentStatus)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.StudentStatusId)
                    .HasConstraintName("fk_users_student_status");

                entity.HasOne(d => d.TeacherStatus)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.TeacherStatusId)
                    .HasConstraintName("fk_users_teacher_status");
            });

            modelBuilder.Entity<UserTrainingRank>(entity =>
            {
                entity.ToTable("user_training_ranks");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.TrainingRankId).HasColumnName("training_rank_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.TrainingRank)
                    .WithMany(p => p.UserTrainingRanks)
                    .HasForeignKey(d => d.TrainingRankId)
                    .HasConstraintName("fk_user_training_ranks_training_rank");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTrainingRanks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_user_training_ranks_user");
            });
            modelBuilder.Entity<ExamScheduleStatus>(entity =>
            {
                entity.ToTable("exam_schedule_statuses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Names).HasColumnName("names");

                entity.HasMany(e => e.TestExams)
                    .WithOne(e => e.ExamScheduleStatus)
                    .HasForeignKey(e => e.ScheduleStatusId)
                    .HasConstraintName("fk_test_exams_schedule_status");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}