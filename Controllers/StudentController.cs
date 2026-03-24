using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 学生管理 API 控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;
        private readonly ILogger<StudentController> _logger;

        public StudentController(IStudentService service, ILogger<StudentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有学生列表
        /// </summary>
        /// <returns>学生列表</returns>
        /// <response code="200">返回学生列表</response>
        [HttpGet("getList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            _logger.LogInformation("获取所有学生列表");
            var students = await _service.GetAllAsync();
            return Ok(students);
        }

        /// <summary>
        /// 根据 ID 获取学生详情
        /// </summary>
        /// <param name="id">学生 ID</param>
        /// <returns>学生详情</returns>
        /// <response code="200">返回学生详情</response>
        /// <response code="404">未找到学生</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDto>> GetById(int id)
        {
            _logger.LogInformation("获取学生详情，ID: {Id}", id);
            var student = await _service.GetByIdAsync(id);

            if (student == null)
            {
                _logger.LogWarning("未找到学生，ID: {Id}", id);
                return NotFound(new { message = $"未找到 ID 为 {id} 的学生" });
            }

            return Ok(student);
        }

        /// <summary>
        /// 创建新学生
        /// </summary>
        /// <param name="dto">学生信息</param>
        /// <returns>创建的学生信息</returns>
        /// <response code="201">创建成功</response>
        /// <response code="400">请求参数无效</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto)
        {
            _logger.LogInformation("创建新学生，姓名：{Name}", dto.Name);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("创建学生请求参数验证失败");
                return BadRequest(ModelState);
            }

            var student = await _service.CreateAsync(dto);
            _logger.LogInformation("学生创建成功，ID: {Id}", student.Id);

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
        }

        /// <summary>
        /// 更新学生信息
        /// </summary>
        /// <param name="id">学生 ID</param>
        /// <param name="dto">更新的学生信息</param>
        /// <returns>更新后的学生信息</returns>
        /// <response code="200">更新成功</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="404">未找到学生</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDto>> Update(int id, [FromBody] UpdateStudentDto dto)
        {
            _logger.LogInformation("更新学生信息，ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("更新学生请求参数验证失败");
                return BadRequest(ModelState);
            }

            var student = await _service.UpdateAsync(id, dto);

            if (student == null)
            {
                _logger.LogWarning("更新失败，未找到学生，ID: {Id}", id);
                return NotFound(new { message = $"未找到 ID 为 {id} 的学生" });
            }

            _logger.LogInformation("学生更新成功，ID: {Id}", id);
            return Ok(student);
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="id">学生 ID</param>
        /// <returns>删除结果</returns>
        /// <response code="200">删除成功</response>
        /// <response code="404">未找到学生</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("删除学生，ID: {Id}", id);

            var success = await _service.DeleteAsync(id);

            if (!success)
            {
                _logger.LogWarning("删除失败，未找到学生，ID: {Id}", id);
                return NotFound(new { message = $"未找到 ID 为 {id} 的学生" });
            }

            _logger.LogInformation("学生删除成功，ID: {Id}", id);
            return Ok(new { message = $"学生 {id} 已删除" });
        }
    }
}
