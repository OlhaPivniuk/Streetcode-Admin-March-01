using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Account;

public class LogoutUserResponseDto
{
    public string Email { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
