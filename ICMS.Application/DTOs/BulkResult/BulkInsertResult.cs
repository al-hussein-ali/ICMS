using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.DTOs.BulkResult
{
    public class BulkInsertResult
    {
        public int InsertedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
