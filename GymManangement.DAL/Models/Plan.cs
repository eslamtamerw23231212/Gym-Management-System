using GymManangement.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace Gym_Management_System.Models
{
    public class Plan:BaseEntity
    {
        
        
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int DurationDays { get; set; }
        //
        public ICollection<MemberShip> MemberShips { get; set; }


    }
}
