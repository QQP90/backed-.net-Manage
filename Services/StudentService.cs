using WebApplication1.Models;
using WebApplication1.Models.DTOs;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    /// <summary>
    /// 学生业务逻辑实现类
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students = await _repository.GetAllAsync();
            return students.Select(MapToDto);
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            return student == null ? null : MapToDto(student);
        }

        public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
        {
            var student = new Student
            {
                Name = dto.Name,
                Age = dto.Age,
                Phone = dto.Phone
            };

            var id = await _repository.CreateAsync(student);
            student.Id = id;

            return MapToDto(student);
        }

        public async Task<StudentDto?> UpdateAsync(int id, UpdateStudentDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return null;
            }

            // 只更新提供的字段
            if (dto.Name != null)
            {
                existing.Name = dto.Name;
            }
            if (dto.Age.HasValue)
            {
                existing.Age = dto.Age.Value;
            }
            if (dto.Email != null)
            {
                existing.Phone = dto.Email;
            }

            var success = await _repository.UpdateAsync(existing);
            return success ? MapToDto(existing) : null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// 将实体映射为 DTO
        /// </summary>
        private static StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Name = student.Name,
                Age = student.Age,
                Phone = student.Phone,
                CreatedAt = student.CreatedAt,
                UpdatedAt = student.UpdatedAt
            };
        }
    }
}
