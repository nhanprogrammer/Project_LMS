using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Project_LMS.Helpers;

namespace Project_LMS.DTOs.Response
{
    public class SemesterResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
       
    }
}