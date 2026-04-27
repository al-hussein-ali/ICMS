using System;

namespace ICMS.Application.DTOs.Schedules
{
    public class MissedScheduleQueryDto
    {
        public DateOnly FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public int? SubNeighborhoodId { get; set; }
    }
}
