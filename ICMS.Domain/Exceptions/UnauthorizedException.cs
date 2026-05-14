using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string messageKey) : base(messageKey)
        {
        }

        public UnauthorizedException(string messageKey, params object[] args) : base(messageKey, args)
        {
        }
    }
}
