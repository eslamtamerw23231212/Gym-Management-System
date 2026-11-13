using Gym_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Models
{
    public class MemberShip:BaseEntity
    {
        public Member Memeber { get; set; }
        public int MemberId { get; set; }
        public Plan Plan { get; set; }
        public int PlanId { get; set; }
        public DateTime EndDate { get; set; }
    }
}
