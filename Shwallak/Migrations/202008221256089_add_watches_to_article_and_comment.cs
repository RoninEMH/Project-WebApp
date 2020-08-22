namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_watches_to_article_and_comment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Watches", c => c.Int(nullable: false));
            AddColumn("dbo.Comments", "Watches", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Watches");
            DropColumn("dbo.Articles", "Watches");
        }
    }
}
