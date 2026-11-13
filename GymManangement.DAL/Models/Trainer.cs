using GymManangement.DAL.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Models
{
    public class Trainer:GymUser
    {
        public Speciality speciality { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}
