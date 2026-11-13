using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Models
{
    public class Member:GymUser
    {
        public string? Photo { get; set; }
        //
        public HealthRecord healthRecord { get; set; }
        //
        public ICollection<MemberShip> MemberShips { get; set; }
        //
        public ICollection<Booking> MemberSessions { get; set; }
    }
}
