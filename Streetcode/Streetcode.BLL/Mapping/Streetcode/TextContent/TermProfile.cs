using AutoMapper;
using Streetcode.BLL.Dto.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Mapping.Streetcode.TextContent;

public class TermProfile : Profile
{
    public TermProfile()
    {
        CreateMap<Term, TermDto>().ReverseMap();
        CreateMap<CreateTermRequestDto, Term>();
        CreateMap<Term, CreateTermResponseDto>();
        CreateMap<UpdateTermRequestDto, Term>();
        CreateMap<Term, UpdateTermResponseDto>();
        CreateMap<DeleteTermRequestDto, Term>();
        CreateMap<Term, DeleteTermResponseDto>();
    }
}