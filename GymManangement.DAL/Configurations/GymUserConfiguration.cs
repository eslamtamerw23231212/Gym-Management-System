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
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(u => u.Name)
                .HasColumnType("varchar")
               .HasMaxLength(50);
            builder.Property(u => u.Email)
                .HasColumnType("varchar")
               .HasMaxLength(100);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "EmailCheck",
                    "Email LIKE '_%@_%._%'");

                tb.HasCheckConstraint(
                    "PhoneCheck",
                    "Phone LIKE '010%' OR Phone LIKE '011%' OR Phone LIKE '012%' OR Phone LIKE '015%'");
            });

            builder.OwnsOne(x => x.address, a =>
            {
                a.Property(x => x.City).HasMaxLength(30).HasColumnName("City").HasColumnType("varchar");
                a.Property(x => x.Street).HasMaxLength(30).HasColumnName("Street").HasColumnType("varchar");
            });
        }
    }
}
