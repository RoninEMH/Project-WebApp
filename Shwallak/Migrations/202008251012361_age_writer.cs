namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class age_writer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Writers", "Age", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Writers", "Age");
        }
    }
}
