using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Exceptions
{
    public class InvalidDoseTakeDateException : DomainException
    {
        public InvalidDoseTakeDateException(string message) : base(message)
        {
        }
    }
}
