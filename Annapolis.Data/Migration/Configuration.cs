namespace Annapolis.Data.Migration
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class CircleConfiguration : DbMigrationsConfiguration<AnnapolisDbContext>
    {
        public CircleConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(AnnapolisDbContext context)
        {
           
            base.Seed(context);

            if (context.Settings.Count() <= 0)
            {
               
                ConfigurationData configurationData = new ConfigurationData();

           
                configurationData.AddSettings(context);

             
              
            }
        }

       
    }
}
