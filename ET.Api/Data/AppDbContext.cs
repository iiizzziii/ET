using ET.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ET.Api.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Position> Positions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(p => p.PositionId);
            entity.Property(p => p.PositionName).IsRequired();
            entity.HasData(
                new Position { PositionId = 1, PositionName = "Manager" },
                new Position { PositionId = 2, PositionName = "Developer" },
                new Position { PositionId = 3, PositionName = "Analyst" }
            );
        });
        
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Surname).IsRequired();

            entity.HasOne(e => e.Position);
                  // .WithMany(p => p.Employees)
                  // .HasForeignKey(e => e.PositionId)
                  // .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasData(
                new Employee
                {
                    EmployeeId = 1,
                    Name = "John",
                    Surname = "Doe",
                    BirthDate = "1999/02/02",
                    PositionId = 1,
                    IpAddress = "192.168.1.1",
                    IpCountryCode = "US"
                },
                new Employee
                {
                    EmployeeId = 2,
                    Name = "Jane",
                    Surname = "Smith",
                    BirthDate = "1999/02/02",
                    PositionId = 2,
                    IpAddress = "192.168.1.2",
                    IpCountryCode = "CA"
                },
                new Employee
                {
                    EmployeeId = 3,
                    Name = "Alice",
                    Surname = "Brown",
                    BirthDate = "1999/02/02",
                    PositionId = 3,
                    IpAddress = "192.168.1.3",
                    IpCountryCode = "UK"
                }
            );
        });
    }
}