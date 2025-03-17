namespace ArtTicket.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lab4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ImageUrl = c.String(),
                        IsFeatured = c.Boolean(nullable: false),
                        VenueId = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Venues", t => t.VenueId)
                .ForeignKey("dbo.EventCategories", t => t.CategoryId)
                .Index(t => t.VenueId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        Rating = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        EventId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        PasswordHash = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PhoneNumber = c.String(),
                        RegistrationDate = c.DateTime(nullable: false),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsAvailable = c.Boolean(nullable: false),
                        SeatNumber = c.String(),
                        EventId = c.Int(nullable: false),
                        TicketTypeId = c.Int(),
                        Order_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TicketTypes", t => t.TicketTypeId)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.EventId)
                .Index(t => t.TicketTypeId)
                .Index(t => t.Order_Id);
            
            CreateTable(
                "dbo.TicketTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        PriceMultiplier = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Venues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        Description = c.String(),
                        Capacity = c.Int(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "CategoryId", "dbo.EventCategories");
            DropForeignKey("dbo.Events", "VenueId", "dbo.Venues");
            DropForeignKey("dbo.Tickets", "EventId", "dbo.Events");
            DropForeignKey("dbo.Reviews", "EventId", "dbo.Events");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tickets", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.Tickets", "TicketTypeId", "dbo.TicketTypes");
            DropIndex("dbo.Tickets", new[] { "Order_Id" });
            DropIndex("dbo.Tickets", new[] { "TicketTypeId" });
            DropIndex("dbo.Tickets", new[] { "EventId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.Reviews", new[] { "EventId" });
            DropIndex("dbo.Events", new[] { "CategoryId" });
            DropIndex("dbo.Events", new[] { "VenueId" });
            DropTable("dbo.Venues");
            DropTable("dbo.TicketTypes");
            DropTable("dbo.Tickets");
            DropTable("dbo.Orders");
            DropTable("dbo.Users");
            DropTable("dbo.Reviews");
            DropTable("dbo.Events");
            DropTable("dbo.EventCategories");
        }
    }
}
