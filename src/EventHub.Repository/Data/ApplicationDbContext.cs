using EventHub.Domain.DomainModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Repository.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly bool _isSqlServer;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        // Detect if using SQL Server
        _isSqlServer = Database.IsSqlServer();
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Event configuration
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(300);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Organizer).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            
            // Handle decimal precision based on database type
            if (_isSqlServer)
            {
                entity.Property(e => e.TicketPrice).HasColumnType("decimal(18,2)");
            }
            else
            {
                // SQLite doesn't enforce decimal precision, just use default
                entity.Property(e => e.TicketPrice);
            }
            
            entity.HasMany(e => e.Tickets)
                .WithOne(t => t.Event)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Schedules)
                .WithOne(s => s.Event)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Attendee configuration
        modelBuilder.Entity<Attendee>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Email).IsRequired().HasMaxLength(200);
            entity.Property(a => a.PhoneNumber).HasMaxLength(20);
            entity.Property(a => a.Address).HasMaxLength(500);
            
            entity.HasIndex(a => a.Email).IsUnique();

            entity.HasMany(a => a.Tickets)
                .WithOne(t => t.Attendee)
                .HasForeignKey(t => t.AttendeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Ticket configuration
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.TicketNumber).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Price).HasColumnType("decimal(18,2)");
            entity.Property(t => t.SeatNumber).HasMaxLength(20);
            entity.Property(t => t.QRCode).HasMaxLength(100);
            
            entity.HasIndex(t => t.TicketNumber).IsUnique();
        });

        // Schedule configuration
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Title).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Description).HasMaxLength(1000);
            entity.Property(s => s.Location).HasMaxLength(300);
            entity.Property(s => s.Speaker).HasMaxLength(200);
        });

        // ApplicationUser configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne(u => u.Attendee)
                .WithMany()
                .HasForeignKey(u => u.AttendeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
