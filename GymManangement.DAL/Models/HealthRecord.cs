using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Models
{
    public class HealthRecord:BaseEntity
    {
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public string? Note { get; set; }
        public string BloodType { get; set; }
        //
        public Member Member { get; set; }
        public int MemberId { get; set; }
    }
}
