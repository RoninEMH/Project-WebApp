namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_second_to_comment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Second", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "Second");
        }
    }
}
