using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook_1.Database.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Login).IsRequired().HasMaxLength(128);
            builder.Property(user => user.Password).IsRequired().HasMaxLength(256);

            builder.HasMany(user => user.UserRecipes)
               .WithOne(recipe => recipe.user)
               .HasForeignKey(recipe => recipe.UserId);
        }
    }
}
