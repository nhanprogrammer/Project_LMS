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
using Project_LMS.Authorization;
using Project_LMS.DTOs.Response;
using Microsoft.Extensions.Caching.Memory;
using Aspose.Cells.Charts;
using Project_LMS.DTOs.Request;
using FluentValidation.AspNetCore;
using FluentValidation;
using Project_LMS.Hubs;
using Project_LMS.Middleware;
using Hangfire;
using Hangfire.PostgreSql;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddControllers(options => { options.Filters.Add<ValidationFilter>(); });

// Tắt tự động kiểm tra ModelState trong API behavior để sử dụng ValidationFilter
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; // Không sử dụng filter ModelState mặc định
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project_LMS", Version = "v1" }); });
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ValidationFilter>();
builder.Services.AddScoped<AcademicHoldStatusCheckerJob>();
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
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IFavouritesService, FavouritesService>();
builder.Services.AddScoped<IDisciplinesService, DisciplinesService>();
builder.Services.AddScoped<IModulesService, ModulesService>();
builder.Services.AddScoped<IClassTypeService, ClassTypeService>();
// builder.Services.AddScoped<IClassOnlineService, ClassOnlineService>();
builder.Services.AddScoped<IQuestionsAnswerTopicViewService, QuestionsAnswerTopicViewService>();
builder.Services.AddScoped<IAcademicHoldsService, AcademicHoldsService>();
builder.Services.AddScoped<IAcademicYearsService, AcademicYearsService>();
builder.Services.AddScoped<IAnswersService, AnswersService>();
builder.Services.AddScoped<IAssignmentsService, AssignmentsService>();
builder.Services.AddScoped<IAssignmentDetailsService, AssignmentDetailsService>();
builder.Services.AddScoped<IChatMessagesService, ChatMessagesService>();
builder.Services.AddScoped<ITestExamTypeService, TestExamTypeService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ITestExamService, TestExamService>();
builder.Services.AddScoped<ISubjectTypeService, SubjectTypeService>();
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITestExamService, TestExamService>();
builder.Services.AddScoped<ISubjectTypeService, SubjectTypeService>();
builder.Services.AddScoped<ISubjectGroupService, SubjectGroupService>();
builder.Services.AddScoped<IDepartmentsService, DepartmentsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentStatusService, StudentStatusService>();
builder.Services.AddScoped<IClassStudentService, ClassStudentService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IClassStudentService, ClassStudentService>();
builder.Services.AddScoped<IExemptionService, ExemptionService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IDisciplinesService, DisciplinesService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ITeacherStatusHistoryService, TeacherStatusHistoryService>();
builder.Services.AddScoped<IQuestionsAnswersService, QuestionsAnswersService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<INotificationsService, NotificationsService>();
builder.Services.AddScoped<ITeacherTestExamService, TeacherTestExamService>();
builder.Services.AddScoped<IDependentService, DependentService>();
builder.Services.AddScoped<ITranscriptService, TranscriptService>();
//AddSingleton
builder.Services.AddSingleton<ISupportService, SupportService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IGradeEntryService, GradeEntryService>();


// Repositories
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<ISchoolBranchRepository, SchoolBranchRepository>();
builder.Services.AddScoped<ISchoolTransferRepository, SchoolTransferRepository>();
builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<IClassStudentOnlineRepository, ClassStudentOnlineRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ITestExamRepository, TestExamRepository>();
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
builder.Services.AddScoped<ISubjectTypeRepository, SubjectTypeRepository>();
builder.Services.AddScoped<IJwtReponsitory, JwtReponsitory>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IExemptionRepository, ExemptionRepository>();
builder.Services.AddScoped<IDisciplineRepository, DisciplineRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ITeacherClassSubjectRepository, TeacherClassSubjectRepository>();
builder.Services.AddScoped<ITeacherStatusHistoryRepository, TeacherStatusHistoryRepository>();
builder.Services.AddScoped<ITeachingAssignmentRepository, TeachingAssignmentRepository>();

builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();
builder.Services.AddScoped<ISystemSettingService, SystemSettingService>();
builder.Services.AddScoped<ITeachingAssignmentService, TeachingAssignmentService>();
builder.Services.AddScoped<IGradeEntryRepository, GradeEntryRepository>();

builder.Services.AddScoped<ISubjectGroupRepository, SubjectGroupRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentStatusRepository, StudenStatusRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IDependentRepository, DependentRepository>();


builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IDepartmentsService, Deparmen>();
builder.Services.AddScoped<IQuestionsAnswerRepository, QuestionsAnswerRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<IDepartmentsService, Deparmen>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassStudentRepository, ClassStudentRepository>();
builder.Services.AddScoped<IClassSubjectRepository, ClassSubjectRepository>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


//mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(typeof(UserMapper));
builder.Services.AddAutoMapper(typeof(StudentStatusMapper));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddAutoMapper(typeof(StudentMapper));
builder.Services.AddAutoMapper(typeof(ExempferMapper));
builder.Services.AddAutoMapper(typeof(RewardMapper));
builder.Services.AddAutoMapper(typeof(DisciplineMapper));
builder.Services.AddAutoMapper(typeof(TeacherMapper));
builder.Services.AddAutoMapper(typeof(TeacherStatusHistoryMapper));
builder.Services.AddAutoMapper(typeof(DependentMapper));

//loging

builder.Services.AddLogging(); // Đăng ký logging

builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMeetService, MeetService>(); // Đăng ký IMeetService với MeetService
builder.Services.AddScoped<IWorkProcessService, WorkProcessService>(); 
builder.Services.AddScoped<IEducationInformationService, EducationInformationService>();

// Đọc cấu hình JWT từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);


// Cấu hình Authentication với JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = async context =>
            {
                var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
                var token = context.Request.Cookies["AccessToken"];
                Console.WriteLine($"Cookie AccessToken: {token}");

                if (string.IsNullOrEmpty(token) && context.Request.Headers.ContainsKey("Authorization"))
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    Console.WriteLine($"Authorization Header: {authHeader}");
                    if (authHeader.StartsWith("Bearer "))
                    {
                        token = authHeader.Substring("Bearer ".Length).Trim();
                    }
                }

                Console.WriteLine($"Extracted Token: {token}");

                // Kiểm tra token trong blacklist
                if (!string.IsNullOrEmpty(token) && memoryCache.TryGetValue($"blacklist:{token}", out _))
                {
                    Console.WriteLine($"Token {token} is blacklisted");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var response = new ApiResponse<string>(1, "Token đã bị vô hiệu hóa. Vui lòng đăng nhập lại.", null);
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }

                // Lưu token vào context nếu hợp lệ
                if (!string.IsNullOrEmpty(token))
                {
                    context.HttpContext.Items["Token"] = token;
                    context.Token = token;
                }
            },
            OnChallenge = context =>
            {
                Console.WriteLine("OnChallenge");
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null);
                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var response = new ApiResponse<string>(1, "Bạn không có quyền truy cập!", null);
                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        };
    });

builder.Services.AddSignalR();

// Thêm Authorization
builder.Services.AddAuthorization();
builder.Services.AddPermissionAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();
//register validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RewardRequestValidator>(); // 
builder.Services.AddValidatorsFromAssemblyContaining<DisciplineRequestValidator>(); // 
builder.Services.AddValidatorsFromAssemblyContaining<TeacherRequestValidator>(); // 
builder.Services.AddValidatorsFromAssemblyContaining<TeacherStatusHistoryRequestValidator>(); // 
builder.Services.AddValidatorsFromAssemblyContaining<DependentRequestValidator>(); // 

builder.Services.AddLogging();

// Cấu hình logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // Xóa các provider mặc định (tùy chọn)
    logging.AddConsole(); // Ghi log ra console
    logging.AddDebug(); // Ghi log ra debug window
    logging.SetMinimumLevel(LogLevel.Information); // Cấu hình mức log tối thiểu
});

var app = builder.Build();
app.MapHub<MeetHubService>("/meetHub");
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
        var error = new
            { Status = 1, Message = "Lỗi hệ thống không mong muốn.", Details = "Xem log để biết thêm chi tiết." };
        await context.Response.WriteAsync(JsonSerializer.Serialize(error));
    });
});

//app.UseCors("AllowFrontend");
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.MapHub<RealtimeHub>("/realtimeHub");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();

// Đăng ký job chạy vào 12 giờ đêm mỗi ngày
RecurringJob.AddOrUpdate<AcademicHoldStatusCheckerJob>(
    "check-academic-hold-status",
    job => job.ExecuteAsync(CancellationToken.None),
    "0 0 * * *",
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
    });
app.Run();