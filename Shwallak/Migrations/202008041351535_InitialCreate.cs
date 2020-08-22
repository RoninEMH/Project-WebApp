namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ArticleID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        SubscribersOnly = c.Boolean(nullable: false),
                        Section = c.Int(nullable: false),
                        WriterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleID)
                .ForeignKey("dbo.Writers", t => t.WriterID, cascadeDelete: true)
                .Index(t => t.WriterID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentID = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        Hour = c.Int(nullable: false),
                        Minute = c.Int(nullable: false),
                        Content = c.String(),
                        ArticleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.Articles", t => t.ArticleID, cascadeDelete: true)
                .Index(t => t.ArticleID);
            
            CreateTable(
                "dbo.Writers",
                c => new
                    {
                        WriterID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Gender = c.Int(nullable: false),
                        Email = c.String(),
                        Year = c.Int(nullable: false),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.WriterID);
            
            CreateTable(
                "dbo.Subscribers",
                c => new
                    {
                        SubscriberID = c.Int(nullable: false, identity: true),
                        Age = c.Int(nullable: false),
                        Gender = c.Int(nullable: false),
                        Email = c.String(),
                        Nickname = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.SubscriberID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Articles", "WriterID", "dbo.Writers");
            DropForeignKey("dbo.Comments", "ArticleID", "dbo.Articles");
            DropIndex("dbo.Comments", new[] { "ArticleID" });
            DropIndex("dbo.Articles", new[] { "WriterID" });
            DropTable("dbo.Subscribers");
            DropTable("dbo.Writers");
            DropTable("dbo.Comments");
            DropTable("dbo.Articles");
        }
    }
}
