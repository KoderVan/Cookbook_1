using Cookbook_1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cookbook_1.Database.Configurations
{
    public class RecipeEntityConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.HasKey(recipe => recipe.Id);
            builder.Property(recipe => recipe.Name).IsRequired().HasMaxLength(128);
            builder.Property(recipe => recipe.CookingDescription).HasMaxLength(256);
        }
    }
}
