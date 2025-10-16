using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralizedOne.Data.Models.DTOs
{
    public class AppointmentSummaryDto
    {
        public int TotalAppointment {  get; set; }
        public int Pending {  get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Upcoming { get; set; } //Future scheduled appointments
    }
    
}
