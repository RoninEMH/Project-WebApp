namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class required_subscriber : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Subscribers", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Subscribers", "Nickname", c => c.String(nullable: false));
            AlterColumn("dbo.Subscribers", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Subscribers", "Password", c => c.String());
            AlterColumn("dbo.Subscribers", "Nickname", c => c.String());
            AlterColumn("dbo.Subscribers", "Email", c => c.String());
        }
    }
}
