-- 创建 Student 表
-- 在 SQL Server 中执行此脚本以创建学生表

USE luda;
GO

-- 如果表已存在则删除
IF OBJECT_ID('dbo.Student', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Student;
    PRINT '已删除现有的 Student 表';
END
GO

-- 创建 Student 表
CREATE TABLE dbo.Student
(
    Id INT IDENTITY(1,1) PRIMARY KEY,           -- 主键，自增
    Name NVARCHAR(50) NOT NULL,                 -- 姓名，必填
    Age INT NOT NULL,                           -- 年龄，必填
    Email NVARCHAR(100) NULL,                   -- 邮箱，可选
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),  -- 创建时间
    UpdatedAt DATETIME NULL                     -- 更新时间
);
GO

-- 添加索引以提高查询性能
CREATE NONCLUSTERED INDEX IX_Student_Name ON dbo.Student(Name);
CREATE NONCLUSTERED INDEX IX_Student_Email ON dbo.Student(Email);
GO

-- 插入测试数据
INSERT INTO dbo.Student (Name, Age, Email, CreatedAt) VALUES
(N'张三', 20, N'zhangsan@example.com', GETDATE()),
(N'李四', 22, N'lisi@example.com', GETDATE()),
(N'王五', 21, N'wangwu@example.com', GETDATE()),
(N'赵六', 23, N'zhaoliu@example.com', GETDATE()),
(N'孙七', 19, N'sunqi@example.com', GETDATE());
GO

-- 验证数据
SELECT * FROM dbo.Student;
GO

PRINT 'Student 表创建成功，已插入 5 条测试数据';
