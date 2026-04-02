using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WebApplication1.Infrastructure;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);


// ✅ 注册 DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加控制器
builder.Services.AddControllers();

// 添加 Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student API",
        Version = "v1",
        Description = "学生管理 API - 提供学生的增删改查功能"
    });
});

// 添加 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置数据库连接字符串
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=localhost;Initial Catalog=luda;User ID=luda;Password=395353;TrustServerCertificate=True;";

// 注册 Repository
builder.Services.AddSingleton<IStudentRepository>(provider => 
    new StudentRepository(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 注册 Service
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserService, UserService>();

// 注册AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// 注册Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Swagger UI（开发环境）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
    });
}

app.UseCors("AllowAll");
app.MapControllers();

app.Run();
