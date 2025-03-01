﻿using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Registration
    {
        public Registration()
        {
            RegistrationContacts = new HashSet<RegistrationContact>();
        }

        public int Id { get; set; }
        public int NationalityId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public string Course { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public string Education { get; set; } = null!;
        public string CurrentSchool { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Image { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Nationality Nationality { get; set; } = null!;
        public virtual School School { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<RegistrationContact> RegistrationContacts { get; set; }
    }
}
