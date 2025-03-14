using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    public partial class User
    {
        public User()
        {
            AcademicHolds = new HashSet<AcademicHold>();
            Assignments = new HashSet<Assignment>();
            ChatMessages = new HashSet<ChatMessage>();
            ClassStudentOnlines = new HashSet<ClassStudentOnline>();
            ClassStudents = new HashSet<ClassStudent>();
            Classes = new HashSet<Class>();
            Disciplines = new HashSet<Discipline>();
            Examiners = new HashSet<Examiner>();
            Favourites = new HashSet<Favourite>();
            Lessons = new HashSet<Lesson>();
            NotificationSenders = new HashSet<Notification>();
            NotificationUsers = new HashSet<Notification>();
            QuestionAnswers = new HashSet<QuestionAnswer>();
            Rewards = new HashSet<Reward>();
            SchoolTransfers = new HashSet<SchoolTransfer>();
            SubjectGroups = new HashSet<SubjectGroup>();
            SystemSettings = new HashSet<SystemSetting>();
            TeacherClassSubjects = new HashSet<TeacherClassSubject>();
            TeachingAssignments = new HashSet<TeachingAssignment>();
            TestExams = new HashSet<TestExam>();
            Topics = new HashSet<Topic>();
            UserTrainingRanks = new HashSet<UserTrainingRank>();
            Departments = new HashSet<Department>();
        }

        public int Id { get; set; }
        public int? GroupModulePermissonId { get; set; }
        public bool? Disable { get; set; }
        public int? RoleId { get; set; }
        public int? StudentStatusId { get; set; }
        public int? TeacherStatusId { get; set; }
        public string? UserCode { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? Active { get; set; }
        public string? Image { get; set; }
        public BitArray? Gender { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? StudyMode { get; set; }
        public string? Phone { get; set; }
        public string? Card { get; set; }
        public DateTime? CardIssueDate { get; set; }
        public string? CardIssuePlace { get; set; }
        public bool? UnionMember { get; set; }
        public DateTime? UnionJoinDate { get; set; }
        public bool? PartyMember { get; set; }
        public DateTime? PartyJoinDate { get; set; }
        public string? Address { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? Alias { get; set; }
        public string? AdmissionType { get; set; }
        public string? National { get; set; }
        public string? FullnameFather { get; set; }
        public DateTime? BirthFather { get; set; }
        public string? WorkFather { get; set; }
        public string? PhoneFather { get; set; }
        public string? FullnameMother { get; set; }
        public DateTime? BirthMother { get; set; }
        public string? WorkMother { get; set; }
        public string? PhoneMother { get; set; }
        public string? FullnameGuardianship { get; set; }
        public DateTime? BirthGuardianship { get; set; }
        public string? WorkGuardianship { get; set; }
        public string? PhoneGuardianship { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual GroupModulePermisson? GroupModulePermisson { get; set; }
        public virtual Role? Role { get; set; }
        public virtual StudentStatus? StudentStatus { get; set; }
        public virtual TeacherStatus? TeacherStatus { get; set; }
        public virtual ICollection<AcademicHold> AcademicHolds { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ClassStudentOnline> ClassStudentOnlines { get; set; }
        public virtual ICollection<ClassStudent> ClassStudents { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Discipline> Disciplines { get; set; }
        public virtual ICollection<Examiner> Examiners { get; set; }
        public virtual ICollection<Favourite> Favourites { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Notification> NotificationSenders { get; set; }
        public virtual ICollection<Notification> NotificationUsers { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }
        public virtual ICollection<Reward> Rewards { get; set; }
        public virtual ICollection<SchoolTransfer> SchoolTransfers { get; set; }
        public virtual ICollection<SubjectGroup> SubjectGroups { get; set; }
        public virtual ICollection<SystemSetting> SystemSettings { get; set; }
        public virtual ICollection<TeacherClassSubject> TeacherClassSubjects { get; set; }
        public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; }
        public virtual ICollection<TestExam> TestExams { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<UserTrainingRank> UserTrainingRanks { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        //public string Email { get; set; }
    }
}
