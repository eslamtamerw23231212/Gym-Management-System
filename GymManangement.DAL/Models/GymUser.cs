using GymManangement.DAL.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Models
{
    public abstract class GymUser:BaseEntity
    {
        public string Name { get; set; } = default;
        public string Email { get; set; } = default;
        public string Phone { get; set; } = default;
        public DateOnly DateofBirth { get; set; } = default;
        public Gender Gender { get; set; }
        public Address address { get; set; }
        [Owned] 
        public class Address { 
            public string City { get; set; }
            public string Street { get; set; }
            public int BuildingNumber { get; set; }
        }



    }
}
