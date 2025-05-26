using System.Data.Entity;
using ArtTicket.Domain.Models;

namespace ArtTicket.Infrastructure.Data
{
    public class ArtTicketDbContext : DbContext
    {
        public ArtTicketDbContext() : base("name=ArtTicketDb")
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Настройка отношений между моделями
            
            // User - Order (один ко многим)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithRequired(o => o.User)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);
                
            // User - Review (один ко многим)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithRequired(r => r.User)
                .HasForeignKey(r => r.UserId)
                .WillCascadeOnDelete(false);
                
            // User - CartItem (один ко многим)
            modelBuilder.Entity<User>()
                .HasMany(u => u.CartItems)
                .WithRequired(c => c.User)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);
                
            // Venue - Event (один ко многим)
            modelBuilder.Entity<Venue>()
                .HasMany(v => v.Events)
                .WithRequired(e => e.Venue)
                .HasForeignKey(e => e.VenueId)
                .WillCascadeOnDelete(false);
                
            // EventCategory - Event (один ко многим)
            modelBuilder.Entity<EventCategory>()
                .HasMany(c => c.Events)
                .WithRequired(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .WillCascadeOnDelete(false);
                
            // Event - Ticket (один ко многим)
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tickets)
                .WithRequired(t => t.Event)
                .HasForeignKey(t => t.EventId)
                .WillCascadeOnDelete(false);
                
            // Event - Review (один ко многим)
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Reviews)
                .WithRequired(r => r.Event)
                .HasForeignKey(r => r.EventId)
                .WillCascadeOnDelete(false);
                
            // TicketType - Ticket (один ко многим)
            modelBuilder.Entity<TicketType>()
                .HasMany(tt => tt.Tickets)
                .WithOptional(t => t.TicketType)
                .HasForeignKey(t => t.TicketTypeId)
                .WillCascadeOnDelete(false);
                
            // Order - OrderItems (один ко многим)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithRequired(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .WillCascadeOnDelete(false);
                
            // Ticket - CartItems (один ко многим)
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.CartItems)
                .WithRequired(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .WillCascadeOnDelete(false);
                
            // Ticket - OrderItems (один ко многим)
            modelBuilder.Entity<Ticket>()
                .HasMany(t => t.OrderItems)
                .WithRequired(i => i.Ticket)
                .HasForeignKey(i => i.TicketId)
                .WillCascadeOnDelete(false);
                
            base.OnModelCreating(modelBuilder);
        }
    }
} 