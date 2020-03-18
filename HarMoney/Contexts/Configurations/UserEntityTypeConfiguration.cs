using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarMoney.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HarMoney.Contexts.Configurations
{
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user_data");
            builder.HasKey(prop => prop.Id);
            builder.Property(prop => prop.Id).HasColumnName("id").ValueGeneratedOnAdd();
            builder.Property(prop => prop.FirstName).HasColumnName("first_name").HasMaxLength(50).IsRequired();
            builder.Property(prop => prop.LastName).HasColumnName("last_name").HasMaxLength(20).IsRequired();
            builder.Property(prop => prop.Email).HasColumnName("email").HasMaxLength(320).IsRequired();
            //builder.Property(prop => prop.Password).HasColumnName("password").HasMaxLength(20).IsRequired();
            //builder.Property(prop => prop.Token).HasColumnName("token");
        }
    }
}
