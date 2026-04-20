using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Exceptions
{
    public class InvalidDoseTakeDateException : DomainException
    {
        public InvalidDoseTakeDateException(string messageKey, params object[] args) : base(messageKey, args)
        {
        }
    }
}
