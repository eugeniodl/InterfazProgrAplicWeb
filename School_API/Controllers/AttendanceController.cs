using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SharedModels.Dto;
using School_API.Repository.IRepository;
using SharedModels;

namespace School_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IAttendanceRepository attendanceRepo,
            IStudentRepository studentRepo,
            IMapper mapper,
            ILogger<AttendanceController> logger)
        {
            _attendanceRepo = attendanceRepo;
            _studentRepo = studentRepo;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendances()
        {
            try
            {
                var list = await _attendanceRepo.GetAllAsync();
                return Ok(_mapper.Map<IEnumerable<AttendanceDto>>(list));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener asistencias: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AttendanceDto>> GetAttendance(int id)
        {
            if (id <= 0) return BadRequest("ID no válido.");

            try
            {
                var attendance = await _attendanceRepo.GetByIdAsync(id);
                if (attendance == null)
                    return NotFound("Asistencia no encontrada.");

                return Ok(_mapper.Map<AttendanceDto>(attendance));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener asistencia con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AttendanceDto>> PostAttendance([FromBody] AttendanceCreateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validar existencia del estudiante
                bool studentExists = await _studentRepo.ExistsAsync(s => s.StudentId == createDto.StudentId);
                if (!studentExists)
                {
                    ModelState.AddModelError("StudentId", "El estudiante no existe.");
                    return BadRequest(ModelState);
                }

                // Validar que no exista ya una asistencia ese mismo día para ese estudiante
                bool duplicate = await _attendanceRepo.ExistsAsync(a =>
                    a.StudentId == createDto.StudentId && a.Date == createDto.Date);

                if (duplicate)
                {
                    ModelState.AddModelError("Duplicate", 
                        "Ya se ha registrado asistencia para este estudiante en esa fecha.");
                    return BadRequest(ModelState);
                }

                var attendance = _mapper.Map<Attendance>(createDto);
                await _attendanceRepo.CreateAsync(attendance);

                return CreatedAtAction(nameof(GetAttendance),
                    new { id = attendance.AttendanceId },
                    _mapper.Map<AttendanceDto>(attendance));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear asistencia: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al crear la asistencia.");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAttendance(int id, [FromBody] AttendanceUpdateDto updateDto)
        {
            if (id != updateDto.AttendanceId)
                return BadRequest("El ID del cuerpo no coincide con el de la URL.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existing = await _attendanceRepo.GetByIdAsync(id);
                if (existing == null)
                    return NotFound("Asistencia no encontrada.");

                // Validar que el estudiante exista
                bool studentExists = await _studentRepo.ExistsAsync(s => s.StudentId == updateDto.StudentId);
                if (!studentExists)
                {
                    ModelState.AddModelError("StudentId", "El estudiante no existe.");
                    return BadRequest(ModelState);
                }

                // Validar duplicado si cambia la fecha o el estudiante
                if (existing.StudentId != updateDto.StudentId || existing.Date != updateDto.Date)
                {
                    bool duplicate = await _attendanceRepo.ExistsAsync(a =>
                        a.StudentId == updateDto.StudentId &&
                        a.Date == updateDto.Date &&
                        a.AttendanceId != id);

                    if (duplicate)
                    {
                        ModelState.AddModelError("Duplicate", 
                            "Ya existe una asistencia para este estudiante en esa fecha.");
                        return BadRequest(ModelState);
                    }
                }

                _mapper.Map(updateDto, existing);
                await _attendanceRepo.UpdateAsync(existing);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar asistencia con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al actualizar la asistencia.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            if (id <= 0) return BadRequest("ID no válido.");

            try
            {
                var attendance = await _attendanceRepo.GetByIdAsync(id);
                if (attendance == null)
                    return NotFound("Asistencia no encontrada.");

                await _attendanceRepo.DeleteAsync(attendance);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar asistencia con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor al eliminar la asistencia.");
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PatchAttendance(int id, JsonPatchDocument<AttendanceUpdateDto> patchDto)
        {
            if (id <= 0)
                return BadRequest("ID no válido.");

            try
            {
                var attendance = await _attendanceRepo.GetByIdAsync(id);
                if (attendance == null)
                    return NotFound("Asistencia no encontrada.");

                var attendanceDto = _mapper.Map<AttendanceUpdateDto>(attendance);
                patchDto.ApplyTo(attendanceDto, ModelState);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validar existencia del estudiante
                bool studentExists = await _studentRepo.ExistsAsync(s => s.StudentId == attendanceDto.StudentId);
                if (!studentExists)
                {
                    ModelState.AddModelError("StudentId", "El estudiante no existe.");
                    return BadRequest(ModelState);
                }

                // Validar duplicado si cambia la fecha o el estudiante
                if (attendance.StudentId != attendanceDto.StudentId || attendance.Date != attendanceDto.Date)
                {
                    bool duplicate = await _attendanceRepo.ExistsAsync(a =>
                        a.StudentId == attendanceDto.StudentId &&
                        a.Date == attendanceDto.Date &&
                        a.AttendanceId != id);

                    if (duplicate)
                    {
                        ModelState.AddModelError("Duplicate", 
                            "Ya existe una asistencia para este estudiante en esa fecha.");
                        return BadRequest(ModelState);
                    }
                }

                _mapper.Map(attendanceDto, attendance);

                using var transaction = await _attendanceRepo.BeginTransactionAsync();
                try
                {
                    await _attendanceRepo.SaveChangesAsync();
                    transaction.Commit();
                    return NoContent();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"Error al aplicar el parche a la asistencia con ID {id}: {ex.Message}");
                    return StatusCode(500, "Error interno del servidor al aplicar el parche.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error general en PATCH para asistencia con ID {id}: {ex.Message}");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}

