using Microsoft.EntityFrameworkCore;

namespace CivicRegistry.API.Models
{
    /// <summary>
    /// Entity Framework Core DbContext cho ứng dụng
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình soft delete filter cho tất cả entities kế thừa BaseModel
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseModel).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseModel.IsDeleted));
                    var constant = System.Linq.Expressions.Expression.Constant(false);
                    var body = System.Linq.Expressions.Expression.Equal(property, constant);
                    var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);
                    
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }
    }
}
