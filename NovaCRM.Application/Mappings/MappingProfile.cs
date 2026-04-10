using AutoMapper;
using NovaCRM.Application.DTOs;
using NovaCRM.Domain.Entities;

namespace NovaCRM.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Customer, CustomerDto>();
        CreateMap<Customer, CustomerDetailDto>()
            .IncludeBase<Customer, CustomerDto>();

        CreateMap<Contact, ContactDto>();

        CreateMap<Deal, DealDto>()
            .ForMember(d => d.CustomerName,
                opt => opt.MapFrom(s => s.Customer != null ? s.Customer.FullName : string.Empty));

        CreateMap<Note, NoteDto>();

        CreateMap<Activity, ActivityDto>();

        CreateMap<Attachment, AttachmentDto>()
            .ForMember(d => d.SizeFormatted,
                opt => opt.MapFrom(s => FormatSize(s.SizeBytes)));
    }

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1024               => $"{bytes} B",
        < 1024 * 1024        => $"{bytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
        _                    => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
    };
}
