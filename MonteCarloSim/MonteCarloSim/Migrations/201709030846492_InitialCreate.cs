namespace MonteCarloSim.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OptionPrice",
                c => new
                    {
                        OptionPriceID = c.Int(nullable: false, identity: true),
                        OptionID = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Day = c.DateTime(nullable: false),
                        Varation = c.String(),
                    })
                .PrimaryKey(t => t.OptionPriceID)
                .ForeignKey("dbo.Option", t => t.OptionID, cascadeDelete: true)
                .Index(t => t.OptionID);
            
            CreateTable(
                "dbo.Option",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ContractName = c.String(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        CurrentPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StrikePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImpliedVolatility = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RiskFreeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OptionType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OptionPrice", "OptionID", "dbo.Option");
            DropIndex("dbo.OptionPrice", new[] { "OptionID" });
            DropTable("dbo.Option");
            DropTable("dbo.OptionPrice");
        }
    }
}
