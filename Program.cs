using Microsoft.OpenApi;
using WebApplication1.Repositories;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

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

// 注册 Service
builder.Services.AddScoped<IStudentService, StudentService>();

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
