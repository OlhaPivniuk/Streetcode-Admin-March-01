using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Media.Video
{
    public sealed record UpdateVideoResponseDto(int Id, int StreetcodeId, string? Title, string? Description, string Url);
}
