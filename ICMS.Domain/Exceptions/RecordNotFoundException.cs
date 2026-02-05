using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Domain.Exceptions
{
    public class RecordNotFoundException : DomainException
    {
        public RecordNotFoundException(string message) : base(message)
        {   
        }
    }
}
