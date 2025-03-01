using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public virtual DbSet<ClassStudentsOnline> ClassStudentsOnlines { get; set; } = null!;
        public virtual DbSet<ClassType> ClassTypes { get; set; } = null!;
        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Discipline> Disciplines { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Favourite> Favourites { get; set; } = null!;
        public virtual DbSet<Lesson> Lessons { get; set; } = null!;
        public virtual DbSet<Module> Modules { get; set; } = null!;
        public virtual DbSet<Nationality> Nationalities { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<NotificationsReceiver> NotificationsReceivers { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<Province> Provinces { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionsAnswer> QuestionsAnswers { get; set; } = null!;
        public virtual DbSet<QuestionsAnswerTopicView> QuestionsAnswerTopicViews { get; set; } = null!;
        public virtual DbSet<Registration> Registrations { get; set; } = null!;
        public virtual DbSet<RegistrationContact> RegistrationContacts { get; set; } = null!;
        public virtual DbSet<Reward> Rewards { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<RolePermission> RolePermissions { get; set; } = null!;
        public virtual DbSet<School> Schools { get; set; } = null!;
        public virtual DbSet<SchoolBranch> SchoolBranches { get; set; } = null!;
        public virtual DbSet<SchoolTransfer> SchoolTransfers { get; set; } = null!;
        public virtual DbSet<Semester> Semesters { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<StudentGuardian> StudentGuardians { get; set; } = null!;
        public virtual DbSet<StudentParent> StudentParents { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectType> SubjectTypes { get; set; } = null!;
        public virtual DbSet<SubjectsGroup> SubjectsGroups { get; set; } = null!;
        public virtual DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public virtual DbSet<Teacher> Teachers { get; set; } = null!;
        public virtual DbSet<TeacherClassSubject> TeacherClassSubjects { get; set; } = null!;
        public virtual DbSet<TeacherContact> TeacherContacts { get; set; } = null!;
        public virtual DbSet<TeachingAssignment> TeachingAssignments { get; set; } = null!;
        public virtual DbSet<TestExam> TestExams { get; set; } = null!;
        public virtual DbSet<TestExamType> TestExamTypes { get; set; } = null!;
        public virtual DbSet<Topic> Topics { get; set; } = null!;
        public virtual DbSet<TrainingRank> TrainingRanks { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserTrainingRank> UserTrainingRanks { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CreateTable;Username=postgres;Password=123456");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AcademicHolds)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_academic_holds_student");
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
                    .HasConstraintName("fk_questions");
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

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.Submission)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("submission");

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

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("fk_students");

                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.TestExamId)
                    .HasConstraintName("fk_test_exams");
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

                entity.Property(e => e.QuestionId).HasColumnName("question_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.AssignmentDetails)
                    .HasForeignKey(d => d.AnswerId)
                    .HasConstraintName("fk_answers");

                entity.HasOne(d => d.Assignment)
                    .WithMany(p => p.AssignmentDetails)
                    .HasForeignKey(d => d.AssignmentId)
                    .HasConstraintName("fk_asignments");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.AssignmentDetails)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("fk_questions");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("chat_messages");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

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

                entity.Property(e => e.IsPinned)
                    .HasColumnName("is_pinned")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.IsQuestion)
                    .HasColumnName("is_question")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.MessageContent).HasColumnName("message_content");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_class");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_users");
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

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.ClassType)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.ClassTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_classes_class_type");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_classes_department");
            });

            modelBuilder.Entity<ClassOnline>(entity =>
            {
                entity.ToTable("class_online");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassCode)
                    .HasMaxLength(50)
                    .HasColumnName("class_code");

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

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.ClassOnlines)
                    .HasForeignKey(d => d.TeacherId)
                    .HasConstraintName("fk_teachers");
            });

            modelBuilder.Entity<ClassStudent>(entity =>
            {
                entity.ToTable("class_students");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassStudents)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_class");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.ClassStudents)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("fk_students");
            });

            modelBuilder.Entity<ClassStudentsOnline>(entity =>
            {
                entity.ToTable("class_students_online");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

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

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.ClassStudentsOnlines)
                    .HasForeignKey(d => d.ClassId)
                    .HasConstraintName("fk_class");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.ClassStudentsOnlines)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("fk_students");
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

            modelBuilder.Entity<Discipline>(entity =>
            {
                entity.ToTable("disciplines");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DisciplineCode).HasColumnName("discipline_code");

                entity.Property(e => e.DisciplineContent)
                    .HasMaxLength(1000)
                    .HasColumnName("discipline_content");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.SemesterId).HasColumnName("semester_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Disciplines)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_discipline_semeter");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("districts");

                entity.HasIndex(e => e.Code, "districts_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(50)
                    .HasColumnName("code_name");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEnd)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_end");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.ProvinceCode).HasColumnName("province_code");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.ProvinceCodeNavigation)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_districts_province");
            });

            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.ToTable("favourites");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.QuestionsAnswerId).HasColumnName("questions_answer_id");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.QuestionsAnswer)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.QuestionsAnswerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_favourites_questions_answer");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_favourites_topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_favourites_user");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.ToTable("lessons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

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

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.StartDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("start_date");

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.Property(e => e.Topic)
                    .HasMaxLength(255)
                    .HasColumnName("topic");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_lesson_class");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.ToTable("modules");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Description).HasColumnName("description");

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

            modelBuilder.Entity<Nationality>(entity =>
            {
                entity.ToTable("nationalities");

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

                entity.Property(e => e.PhoneCode)
                    .HasMaxLength(50)
                    .HasColumnName("phone_code");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(50)
                    .HasColumnName("short_name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

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

                entity.Property(e => e.IsType).HasColumnName("is_type");

                entity.Property(e => e.SenderId).HasColumnName("sender_id");

                entity.Property(e => e.Subject)
                    .HasMaxLength(255)
                    .HasColumnName("subject");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_notifications_class");

                entity.HasOne(d => d.Sender)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_notifications_sender");

                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.TestExamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_notifications_test_exam");
            });

            modelBuilder.Entity<NotificationsReceiver>(entity =>
            {
                entity.ToTable("notifications_receivers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.NotificationId).HasColumnName("notification_id");

                entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Notification)
                    .WithMany(p => p.NotificationsReceivers)
                    .HasForeignKey(d => d.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_notifications_receivers_notification");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.NotificationsReceivers)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_notifications_receivers_receiver");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("permissions");

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
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("provinces");

                entity.HasIndex(e => e.Code, "provinces_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(50)
                    .HasColumnName("code_name");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.FullNameEnd)
                    .HasMaxLength(255)
                    .HasColumnName("full_name_end");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
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

                entity.Property(e => e.Question1).HasColumnName("question");

                entity.Property(e => e.TestExamId).HasColumnName("test_exam_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.TestExam)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TestExamId)
                    .HasConstraintName("fk_test_exams");
            });

            modelBuilder.Entity<QuestionsAnswer>(entity =>
            {
                entity.ToTable("questions_answers");

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

                entity.HasOne(d => d.QuestionsAnswerNavigation)
                    .WithMany(p => p.InverseQuestionsAnswerNavigation)
                    .HasForeignKey(d => d.QuestionsAnswerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_questions_answers_parent");

                entity.HasOne(d => d.TeachingAssignment)
                    .WithMany(p => p.QuestionsAnswers)
                    .HasForeignKey(d => d.TeachingAssignmentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_questions_answers_teaching_assignment");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuestionsAnswers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_questions_answers_user");
            });

            modelBuilder.Entity<QuestionsAnswerTopicView>(entity =>
            {
                entity.ToTable("questions_answer_topic_views");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.QuestionsAnswerId).HasColumnName("questions_answer_id");

                entity.Property(e => e.TopicId).HasColumnName("topic_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.QuestionsAnswer)
                    .WithMany(p => p.QuestionsAnswerTopicViews)
                    .HasForeignKey(d => d.QuestionsAnswerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_views_questions_answer");

                entity.HasOne(d => d.Topic)
                    .WithMany(p => p.QuestionsAnswerTopicViews)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_views_topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.QuestionsAnswerTopicViews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_views_user");
            });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.ToTable("registrations");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.Birthday)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("birthday");

                entity.Property(e => e.Course)
                    .HasMaxLength(255)
                    .HasColumnName("course");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CurrentSchool)
                    .HasMaxLength(255)
                    .HasColumnName("current_school");

                entity.Property(e => e.Education)
                    .HasMaxLength(255)
                    .HasColumnName("education");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(255)
                    .HasColumnName("fullname");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .HasColumnName("image");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.NationalityId).HasColumnName("nationality_id");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .HasColumnName("phone_number");

                entity.Property(e => e.SchoolId).HasColumnName("school_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Nationality)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.NationalityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_registrations_nationality");

                entity.HasOne(d => d.School)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.SchoolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_registrations_school");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Registrations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_registrations_user");
            });

            modelBuilder.Entity<RegistrationContact>(entity =>
            {
                entity.ToTable("registration_contacts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FamilyAddress)
                    .HasMaxLength(255)
                    .HasColumnName("family_address");

                entity.Property(e => e.FamilyName)
                    .HasMaxLength(255)
                    .HasColumnName("family_name");

                entity.Property(e => e.FamilyNumber)
                    .HasMaxLength(10)
                    .HasColumnName("family_number");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.RegistrationId).HasColumnName("registration_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Registration)
                    .WithMany(p => p.RegistrationContacts)
                    .HasForeignKey(d => d.RegistrationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_registration_contacts_registration");
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

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.RewardCode).HasColumnName("reward_code");

                entity.Property(e => e.RewardContent)
                    .HasMaxLength(500)
                    .HasColumnName("reward_content");

                entity.Property(e => e.SemesterId).HasColumnName("semester_id");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.Rewards)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_rewards_semeter");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

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
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("role_permissions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.ModuleId).HasColumnName("module_id");

                entity.Property(e => e.PermissionId).HasColumnName("permission_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_role_permissions_module");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_role_permissions_permission");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_role_permissions_role");
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
                    .HasMaxLength(255)
                    .HasColumnName("province");

                entity.Property(e => e.SchoolCode)
                    .HasMaxLength(50)
                    .HasColumnName("school_code");

                entity.Property(e => e.SchoolType)
                    .HasMaxLength(255)
                    .HasColumnName("school_type");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

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

                entity.Property(e => e.BranchName)
                    .HasMaxLength(255)
                    .HasColumnName("branch_name");

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
                    .OnDelete(DeleteBehavior.ClientSetNull)
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

                entity.Property(e => e.DistrictId).HasColumnName("district_id");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.ProvinceId).HasColumnName("province_id");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.SchoolBranchesId).HasColumnName("school_branches_id");

                entity.Property(e => e.Semester)
                    .HasMaxLength(50)
                    .HasColumnName("semester");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.TransferDate)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("transfer_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.Property(e => e.WardId).HasColumnName("ward_id");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.SchoolTransfers)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_district_id");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.SchoolTransfers)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_province_id");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.SchoolTransfers)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_student_id");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.SchoolTransfers)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_ward_id");
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

                entity.Property(e => e.DateEnd)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_end");

                entity.Property(e => e.DateStart)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("date_start");

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

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.Semesters)
                    .HasForeignKey(d => d.AcademicYearId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_semester_academic_year");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("students");

                entity.HasIndex(e => e.StudentCode, "students_student_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcademicYearId).HasColumnName("academic_year_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(200)
                    .HasColumnName("address");

                entity.Property(e => e.AdmissionDate).HasColumnName("admission_date");

                entity.Property(e => e.BirthDate).HasColumnName("birth_date");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(50)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .HasColumnName("gender");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.Property(e => e.PlaceOfBirth)
                    .HasMaxLength(100)
                    .HasColumnName("place_of_birth");

                entity.Property(e => e.Religion)
                    .HasMaxLength(50)
                    .HasColumnName("religion");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.StudentCode)
                    .HasMaxLength(20)
                    .HasColumnName("student_code");

                entity.Property(e => e.StudyMode)
                    .HasMaxLength(50)
                    .HasColumnName("study_mode");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.AcademicYear)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.AcademicYearId)
                    .HasConstraintName("fk_academic_year");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_users");
            });

            modelBuilder.Entity<StudentGuardian>(entity =>
            {
                entity.ToTable("student_guardians");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.GuardianBirthYear).HasColumnName("guardian_birth_year");

                entity.Property(e => e.GuardianName)
                    .HasMaxLength(100)
                    .HasColumnName("guardian_name");

                entity.Property(e => e.GuardianOccupation)
                    .HasMaxLength(100)
                    .HasColumnName("guardian_occupation");

                entity.Property(e => e.GuardianPhone)
                    .HasMaxLength(15)
                    .HasColumnName("guardian_phone");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentGuardians)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("fk_students");
            });

            modelBuilder.Entity<StudentParent>(entity =>
            {
                entity.ToTable("student_parents");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FatherBirthYear).HasColumnName("father_birth_year");

                entity.Property(e => e.FatherName)
                    .HasMaxLength(100)
                    .HasColumnName("father_name");

                entity.Property(e => e.FatherOccupation)
                    .HasMaxLength(100)
                    .HasColumnName("father_occupation");

                entity.Property(e => e.FatherPhone)
                    .HasMaxLength(15)
                    .HasColumnName("father_phone");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.MotherBirthYear).HasColumnName("mother_birth_year");

                entity.Property(e => e.MotherName)
                    .HasMaxLength(100)
                    .HasColumnName("mother_name");

                entity.Property(e => e.MotherOccupation)
                    .HasMaxLength(100)
                    .HasColumnName("mother_occupation");

                entity.Property(e => e.MotherPhone)
                    .HasMaxLength(15)
                    .HasColumnName("mother_phone");

                entity.Property(e => e.StudentId).HasColumnName("student_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.StudentParents)
                    .HasForeignKey(d => d.StudentId)
                    .HasConstraintName("fk_students");
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

                entity.Property(e => e.SubjectGroupId).HasColumnName("subject_group_id");

                entity.Property(e => e.TypeSubjectId).HasColumnName("type_subject_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.SubjectGroup)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.SubjectGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subjects_subject_group");

                entity.HasOne(d => d.TypeSubject)
                    .WithMany(p => p.Subjects)
                    .HasForeignKey(d => d.TypeSubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subjects_subject_type");
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
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<SubjectsGroup>(entity =>
            {
                entity.ToTable("subjects_group");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

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

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.SubjectsGroups)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_subjects_group_department");
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

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("teachers");

                entity.HasIndex(e => e.TeacherCode, "teachers_teacher_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(200)
                    .HasColumnName("address");

                entity.Property(e => e.BirthDate).HasColumnName("birth_date");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentsId).HasColumnName("departments_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Ethnicity)
                    .HasMaxLength(50)
                    .HasColumnName("ethnicity");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .HasColumnName("gender");

                entity.Property(e => e.IdCard)
                    .HasMaxLength(20)
                    .HasColumnName("id_card");

                entity.Property(e => e.IdCardIssueDate).HasColumnName("id_card_issue_date");

                entity.Property(e => e.IdCardIssuePlace)
                    .HasMaxLength(100)
                    .HasColumnName("id_card_issue_place");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Nationality)
                    .HasMaxLength(50)
                    .HasColumnName("nationality");

                entity.Property(e => e.PartyJoinDate).HasColumnName("party_join_date");

                entity.Property(e => e.PartyJoinPlace)
                    .HasMaxLength(100)
                    .HasColumnName("party_join_place");

                entity.Property(e => e.PartyMember)
                    .HasColumnName("party_member")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.Property(e => e.Position)
                    .HasMaxLength(50)
                    .HasColumnName("position");

                entity.Property(e => e.Religion)
                    .HasMaxLength(50)
                    .HasColumnName("religion");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TeacherCode)
                    .HasMaxLength(20)
                    .HasColumnName("teacher_code");

                entity.Property(e => e.TeachingSubjectId).HasColumnName("teaching_subject_id");

                entity.Property(e => e.UnionJoinDate).HasColumnName("union_join_date");

                entity.Property(e => e.UnionMember)
                    .HasColumnName("union_member")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Teachers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_users");
            });

            modelBuilder.Entity<TeacherClassSubject>(entity =>
            {
                entity.ToTable("teacher_class_subject");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClassId).HasColumnName("class_id");

                entity.Property(e => e.IsPrimary)
                    .HasColumnName("is_primary")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.SubjectsId).HasColumnName("subjects_id");

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.TeacherClassSubjects)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teacher_class_subject_class");

                entity.HasOne(d => d.Subjects)
                    .WithMany(p => p.TeacherClassSubjects)
                    .HasForeignKey(d => d.SubjectsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teacher_class_subject_subject");
            });

            modelBuilder.Entity<TeacherContact>(entity =>
            {
                entity.ToTable("teacher_contacts");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");

                entity.Property(e => e.ContactName)
                    .HasMaxLength(100)
                    .HasColumnName("contact_name");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.Relation)
                    .HasMaxLength(50)
                    .HasColumnName("relation");

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.TeacherContacts)
                    .HasForeignKey(d => d.TeacherId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_teacher_contacts_teacher");
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

                entity.Property(e => e.TeacherId).HasColumnName("teacher_id");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.TeachingAssignments)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teaching_assignments_class");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.TeachingAssignments)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_teaching_assignments_subjects");
            });

            modelBuilder.Entity<TestExam>(entity =>
            {
                entity.ToTable("test_exams");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Attachment).HasColumnName("attachment");

                entity.Property(e => e.Classify).HasColumnName("classify");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DepartmentId).HasColumnName("department_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.Form)
                    .HasMaxLength(30)
                    .HasColumnName("form");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.SubmissionFormat).HasColumnName("submission_format");

                entity.Property(e => e.TestExamTypeId).HasColumnName("test_exam_type_id");

                entity.Property(e => e.Topic)
                    .HasMaxLength(50)
                    .HasColumnName("topic");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("fk_departments");

                entity.HasOne(d => d.TestExamType)
                    .WithMany(p => p.TestExams)
                    .HasForeignKey(d => d.TestExamTypeId)
                    .HasConstraintName("fk_test_exam_types");
            });

            modelBuilder.Entity<TestExamType>(entity =>
            {
                entity.ToTable("test_exam_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("topics");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CloseAt)
                    .HasColumnType("timestamp without time zone")
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
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_topics_teaching_assignment");

                entity.HasOne(d => d.TopicNavigation)
                    .WithMany(p => p.InverseTopicNavigation)
                    .HasForeignKey(d => d.TopicId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_topics_parent_topic");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
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

                entity.Property(e => e.ConfigurationId).HasColumnName("configuration_id");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .HasColumnName("full_name");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .HasColumnName("username");

                entity.HasOne(d => d.Configuration)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.ConfigurationId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_users_configuration");
            });

            modelBuilder.Entity<UserTrainingRank>(entity =>
            {
                entity.ToTable("user_training_rank");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TrainingRankId).HasColumnName("training_rank_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.TrainingRank)
                    .WithMany(p => p.UserTrainingRanks)
                    .HasForeignKey(d => d.TrainingRankId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user_training_rank_training_rank");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTrainingRanks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_user_training_rank_user");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.ToTable("wards");

                entity.HasIndex(e => e.Code, "wards_code_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .HasColumnName("code");

                entity.Property(e => e.CodeName)
                    .HasMaxLength(50)
                    .HasColumnName("code_name");

                entity.Property(e => e.CreateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("create_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DistrictId).HasColumnName("district_id");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("full_name");

                entity.Property(e => e.IsDelete)
                    .HasColumnName("is_delete")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.NameEn)
                    .HasMaxLength(255)
                    .HasColumnName("name_en");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("update_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UserCreate).HasColumnName("user_create");

                entity.Property(e => e.UserUpdate).HasColumnName("user_update");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_wards_district");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
       
    }
}
