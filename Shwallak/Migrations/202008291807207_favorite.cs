namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class favorite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subscribers", "Favorite", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscribers", "Favorite");
        }
    }
}
