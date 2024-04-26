using AutoMapper;
using Streetcode.BLL.Dto.Sources;
using Streetcode.BLL.DTO.Sources;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.BLL.Mapping.Sources
{
    public class StreetcodeCategoryContentProfile : Profile
    {
        public StreetcodeCategoryContentProfile()
        {
            CreateMap<StreetcodeCategoryContent, StreetcodeCategoryContentDto>()
                .ReverseMap();
            CreateMap<CategoryContentUpdateDto, StreetcodeCategoryContent>();
        }
    }
}
