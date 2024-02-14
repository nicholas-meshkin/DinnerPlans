using Microsoft.EntityFrameworkCore;
using DinnerPlans.Server.Persistence.Entities;
using DinnerPlans.Server.Persistence.Entities.BaseEntities;

namespace DinnerPlans.Server.Persistence
{
    public partial class DinnerPlansContext : DbContext
    {
        public DinnerPlansContext(DbContextOptions options) : base(options) { }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<ImportErrorLog> ImportErrorLog { get; set; }
        public DbSet<KrogerErrorLog> KrogerErrorLog { get; set; }
        public DbSet<SeleniumErrorLog> SeleniumErrorLog { get; set; }
        public DbSet<AmountType> AmountType { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }
        public DbSet<RecipeIngredientAmountType> RecipeIngredientAmountType { get; set; }
        public DbSet<Measure> Measure { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Instruction> Instruction { get; set; }
        public DbSet<UserRecipeComment> UserRecipeComment { get; set; }
        public DbSet<UserRecipeRating> UserRecipeRating { get; set; }
        public DbSet<UserIngredientProductFavorite> UserIngredientProductFavorite { get; set; }
        public DbSet<IngredientCategory> IngredientCategory { get; set; }
        public DbSet<IngredientIngredientCategory> IngredientIngredientCategory { get; set; }
        public DbSet<RecipeCopyHistory> RecipeCopyHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
            modelBuilder.Entity<UserRecipeRating>().HasKey(a => new { a.UserId,a.RecipeId});
            modelBuilder.Entity<UserIngredientProductFavorite>().HasKey(a => new { a.UserId,a.IngredientId});
            modelBuilder.Entity<IngredientIngredientCategory>().HasKey(a => new { a.IngredientId, a.IngredientCategoryId });
            modelBuilder.Entity<RecipeCopyHistory>().HasKey(a => new { a.OriginalRecipeId, a.CopyRecipeId, a.UserId });
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
