using AutoMapper;
using Streetcode.BLL.Dto.Streetcode;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.Mapping.Streetcode;

public class StreetcodeProfile : Profile
{
    public StreetcodeProfile()
    {
        CreateMap<StreetcodeContent, StreetcodeDto>()
            .ForMember(x => x.StreetcodeType, conf => conf.MapFrom(s => GetStreetcodeType(s)))
            .ReverseMap();

        CreateMap<int, StreetcodeContent>()
           .ForMember(x => x.Id, conf => conf.MapFrom(i => i));

        CreateMap<StreetcodeContent, StreetcodeShortDto>().ReverseMap();

        CreateMap<StreetcodeContent, StreetcodeMainPageDto>()
             .ForPath(dto => dto.Text, conf => conf
                .MapFrom(e => e.Text != null ? e.Text.Title : ""))
            .ForPath(dto => dto.ImageId, conf => conf
                .MapFrom(e => e.Images.Select(i => i.Id).LastOrDefault()));

        CreateMap<CreateStreetcodeRequestDto, StreetcodeContent>()
            .ConstructUsing((dto, sc) => dto.StreetcodeType switch
            {
                StreetcodeType.Event => new EventStreetcode(),
                StreetcodeType.Person => new PersonStreetcode
                {
                    FirstName = dto.FirstName!,
                    Rank = dto.Rank,
                    LastName = dto.LastName!,
                },
                _ => new StreetcodeContent(),
            });
    }

    private static StreetcodeType GetStreetcodeType(StreetcodeContent streetcode)
    {
        if (streetcode is EventStreetcode)
        {
            return StreetcodeType.Event;
        }

        return StreetcodeType.Person;
    }
}
