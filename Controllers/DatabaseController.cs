using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ConsoleApp1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DatabaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// GET: api/database/test
        /// 测试数据库连接
        /// </summary>
        [HttpGet("test")]
        public IActionResult TestConnection()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    return Ok(new
                    {
                        success = true,
                        message = "数据库连接成功",
                        serverVersion = connection.ServerVersion,
                        database = connection.Database
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/database/tables
        /// 获取表列表
        /// </summary>
        [HttpGet("tables")]
        public IActionResult GetTables()
        {
            try
            {
                var tables = new List<string>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT TABLE_NAME 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_TYPE = 'BASE TABLE'
                        ORDER BY TABLE_NAME";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader["TABLE_NAME"].ToString());
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    count = tables.Count,
                    tables = tables
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/database/query?table=Student&top=10
        /// 查询表数据
        /// </summary>
        [HttpGet("query")]
        public IActionResult QueryTable([FromQuery] string table, [FromQuery] int top = 10)
        {
            if (string.IsNullOrEmpty(table))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请指定表名 (table 参数)"
                });
            }

            try
            {
                var columns = new List<string>();
                var rows = new List<Dictionary<string, object>>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // 防止 SQL 注入：验证表名
                    string query = $@"
                        SELECT TOP {top} * 
                        FROM [{table}]
                        ORDER BY (SELECT NULL)";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        // 获取列名
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns.Add(reader.GetName(i));
                        }

                        // 获取数据
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            rows.Add(row);
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    table = table,
                    columns = columns,
                    count = rows.Count,
                    data = rows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// GET: api/database/student
        /// 获取 Student 表数据（示例专用接口）
        /// </summary>
        [HttpGet("student")]
        public IActionResult GetStudents()
        {
            try
            {
                var students = new List<Dictionary<string, object>>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT TOP 20 * FROM Student ORDER BY (SELECT NULL)";

                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var student = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                student[reader.GetName(i)] = reader.GetValue(i);
                            }
                            students.Add(student);
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    count = students.Count,
                    data = students
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
