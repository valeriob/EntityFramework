using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read
{
    class Program
    {
        //public static string cs = "USER ID=onwmstcc2;PASSWORD=onwmstcc2;DATA SOURCE=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=vborioni-onit.cloudapp.net)(PORT=11521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=orcl)))";
        public static string cs = "USER ID=testef7;PASSWORD=testef7;DATA SOURCE=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=vborioni-onit.cloudapp.net)(PORT=11521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=orcl)))";

        static void Main(string[] args)
        {
            using (var ctx = new MyContext())
            {
               // ctx.Database.Create();
                //ctx.Database.CreateTables();
                var test = ctx.Holdings.ToList();
                ctx.Holdings.Add(new Holding { Codice = "ciao" });
                ctx.SaveChanges();
            }
        }
    }

    public class MyContext : DbContext
    {
        public MyContext()
        {
        }

        public MyContext(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public DbSet<Holding> Holdings { get; set; }

        protected override void OnConfiguring(DbContextOptions builder)
        {
            builder.UseOracle(Program.cs);
            //builder.UseSqlServer(Program.cs);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Holding>()
                .Key(c => c.Codice)
                .Properties(h => h.Property(r => r.Codice).StorageName("HOL_CODICE"))
                .StorageName("T_HOLDING");
        }
    }


    public class Holding
    {
        public string Codice { get; set; }
        //public string CompanyName { get; set; }
        //public string Fax { get; set; }
    }

}
