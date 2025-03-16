﻿namespace Project_LMS.DTOs.Request;

public class CreateAcademicHoldRequest
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public DateTime HoldDate { get; set; }
    public int? HoldDuration { get; set; }
    public string? Reason { get; set; }
    public string? FileName { get; set; }
    public DateTime CreateAt { get; set; }
    public int? UserCreate { get; set; }
}