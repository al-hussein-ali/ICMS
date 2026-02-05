using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Settings;

public class JwtOptions
{
    public string Issuer { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public int ExpiresIn { get; init; }

    public string SigningKey { get; init; } = null!;

}
