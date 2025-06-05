using AutoMapper;
using School_API.Dto;
using SharedModels;

namespace School_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Student, StudentDto>().ReverseMap();
            CreateMap<Student, StudentCreateDto>().ReverseMap();
            CreateMap<Student, StudentUpdateDto>().ReverseMap();

            CreateMap<Attendance, AttendanceDto>().ReverseMap();
            CreateMap<Attendance, AttendanceCreateDto>().ReverseMap();
            CreateMap<Attendance, AttendanceUpdateDto>().ReverseMap();

            CreateMap<User, RegisterUserDto>().ReverseMap();
        }
    }
}
