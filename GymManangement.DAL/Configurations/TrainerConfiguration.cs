using GymManangement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManangement.DAL.Configurations
{

    public class TrainerConfiguration:GymUserConfiguration<Trainer>,IEntityTypeConfiguration<Trainer>
    {
        public new void Configure(EntityTypeBuilder<Trainer> builder)
        {
            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasColumnName("HireDate");
            base.Configure(builder);
        }
    }
}
