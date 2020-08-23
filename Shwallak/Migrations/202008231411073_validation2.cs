namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class validation2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Comments", "Author", c => c.String(nullable: false));
            AlterColumn("dbo.Comments", "Content", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Comments", "Content", c => c.String());
            AlterColumn("dbo.Comments", "Author", c => c.String());
        }
    }
}
