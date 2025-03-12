using Microsoft.EntityFrameworkCore;
using Project_LMS.Data;

using Project_LMS.Interfaces.Services;
using Project_LMS.Interfaces.Repositories;
using Project_LMS.Services;
using Project_LMS.Repositories;
using Project_LMS.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using System.Text.Json;
using Project_LMS.Mappers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Project_LMS.Configurations;

var builder = WebApplication.CreateBuilder(args);
// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Tắt tự động kiểm tra ModelState trong API behavior để sử dụng ValidationFilter
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Không sử dụng filter ModelState mặc định
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project_LMS", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        RoleClaimType = "Role"
                    };
                });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ValidationFilter>();

// Services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<ISchoolBranchService, SchoolBranchService>();
builder.Services.AddScoped<ISchoolTransferService, SchoolTransferService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();
builder.Services.AddScoped<IDistrictsService, DistrictsService>();
builder.Services.AddScoped<IProvincesService, ProvincesService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassService, ClassService>();
// builder.Services.AddScoped<ILessonsService, LessonService>();
builder.Services.AddScoped<IFavouritesService, FavouritesService>();
// builder.Services.AddScoped<IDepartmentsService, DepartmentsService>();
builder.Services.AddScoped<IDisciplinesService, DisciplinesService>();
builder.Services.AddScoped<IModulesService, ModulesService>();
// builder.Services.AddScoped<IClassStudentsOnlineService, ClassStudentOnlineService>();
builder.Services.AddScoped<IClassTypeService, ClassTypeService>();
builder.Services.AddScoped<IClassOnlineService, ClassOnlineService>();
// builder.Services.AddScoped<IQuestionsService, QuestionService>();
builder.Services.AddScoped<IQuestionsAnswerTopicViewService, QuestionsAnswerTopicViewService>();
// builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IAcademicHoldsService, AcademicHoldsService>();
builder.Services.AddScoped<IAcademicYearsService, AcademicYearsService>();
builder.Services.AddScoped<IAnswersService, AnswersService>();
builder.Services.AddScoped<IAssignmentsService, AssignmentsService>();
builder.Services.AddScoped<IAssignmentDetailsService, AssignmentDetailsService>();
builder.Services.AddScoped<IChatMessagesService, ChatMessagesService>();
builder.Services.AddScoped<ITestExamTypeService, TestExamTypeService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectTypeService, SubjectTypeService>();
// builder.Services.AddScoped<ISubjectGroupService, SubjectGroupService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentStatusService, StudentStatusService>();
// Repositories
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<ISchoolBranchRepository, SchoolBranchRepository>();
builder.Services.AddScoped<ISchoolTransferRepository, SchoolTransferRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<IClassStudentOnlineRepository, ClassStudentOnlineRepository>();
// builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDisciplineRepository, DisciplineRepository>();
builder.Services.AddScoped<IFavouriteRepository, FavouriteRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassTypeRepository, ClassTypeRepository>();
builder.Services.AddScoped<IClassOnlineRepository, ClassOnlineRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionsAnswerTopicViewRepository, QuestionsAnswerTopicViewRepository>();
builder.Services.AddScoped<IRewardRepository, RewardRepository>();
builder.Services.AddScoped<IAcademicHoldRepository, AcademicHoldRepository>();
builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddScoped<IAnswerRepository, AnswerRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IAssignmentDetailRepository, AssignmentDetailRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<ITestExamTypeRepository, TestExamTypeRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectTypeRepository, SubjectTypeRepository>();
// builder.Services.AddScoped<ISubjectGroupRepository, SubjectGroupRepository>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingRepository>();
builder.Services.AddScoped<ITeachingAssignmentService, TeachingAssignmentService>();
// builder.Services.AddScoped<ISubjectGroupRepository, SubjectGroupRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentStatusRepository, StudenStatusRepository>();
//mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(typeof(UserMapper));
builder.Services.AddAutoMapper(typeof(StudentStatusMapper));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
//loging
builder.Services.AddLogging(); // Đăng ký logging
var app = builder.Build();
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        var error = new { Status = 1, Message = "Lỗi hệ thống không mong muốn.", Details = "Xem log để biết thêm chi tiết." };
        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    });
});

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
