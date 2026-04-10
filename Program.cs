using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WebApplication1.Infrastructure;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.Repositories.Order;
var builder = WebApplication.CreateBuilder(args);


// 第 1 步：读取连接字符串（来源：appsettings.json 的 ConnectionStrings:DefaultConnection）
// 目的：让整个应用（EF Core + ADO.NET）都用同一份数据库配置
// 如果不做：可能连到错误的库/错误的服务器，或者在运行时才报连接错误（更难定位）
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing connection string: ConnectionStrings:DefaultConnection");
}

// 第 2 步：注册 EF Core 的 DbContext（这里只是“注册”，不会立刻打开数据库连接）
// 目的：让 Repository 可以通过依赖注入拿到 AppDbContext
// 如果不做：凡是依赖 AppDbContext 的地方（User/Order）都会启动失败或运行时报错
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// 第 3 步：注册 MVC 控制器（让 /api/... 路由能工作）
// 如果不做：Swagger 能打开但接口 404，或者路由找不到
builder.Services.AddControllers();

// 第 4 步：注册 Order 模块（Repository + Service）
// 目的：Controller -> Service -> Repository -> DbContext 的调用链条能完整组装起来
// 如果不做：调用 /api/order 会报 500（DI 无法解析 IOrderService / IOrderRepository）
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
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

// 第 5 步：配置 CORS（让浏览器前端可以跨域调用）
// 如果不做：浏览器会被 CORS 拦截（Postman/curl 一般不受影响）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 注册 Repository
builder.Services.AddSingleton<IStudentRepository>(provider => 
    new StudentRepository(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 注册 Service
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserService, UserService>();

// 注册AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

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

// 添加根路径处理
app.MapGet("/", async context =>
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { status = "ok", message = "Server is running" });
});

// 第 6 步：把 CORS / Controller 路由挂到 HTTP 管道
// 如果中间件顺序不对：会出现 CORS 头没加上、路由不生效等问题
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
