namespace Shwallak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class required_and_address : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Writers", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Writers", "FullName", c => c.String(nullable: false));
            AlterColumn("dbo.Writers", "Email", c => c.String(nullable: false));
            AlterColumn("dbo.Writers", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Writers", "Password", c => c.String());
            AlterColumn("dbo.Writers", "Email", c => c.String());
            AlterColumn("dbo.Writers", "FullName", c => c.String());
            DropColumn("dbo.Writers", "Address");
        }
    }
}
