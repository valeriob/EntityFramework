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
            var id = Guid.NewGuid();

            using (var ctx = new MyContext())
            {
                ctx.Database.Delete();
                ctx.Database.CreateTables();
                var test = ctx.Holdings.ToList();

                ctx.Holdings.Add(new Holding
                {
                    H_CODICE = "ciao3",
                    H_INTEGER = 1010,
                    H_DATETIME = DateTime.Now,
                    H_GUID = id,
                    H_BOOL = true,
                    H_BYTE = 10,
                    H_CHAR = 'c',
                    //H_DATETIMEOFFSET = DateTimeOffset.Now,
                    H_DOUBLE = 12312.131231,
                    H_DECIMAL = 13132.1m,
                    //H_SBYTE = 123,
                    //H_UINT = 123,
                    //H_ULONG = 123123,
                    //H_USHORT = 2133,

                });
                ctx.SaveChanges();
            }

            using (var ctx = new MyContext())
            {
                var test = ctx.Holdings.ToList();
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
                .Key(c => c.H_CODICE)
                .StorageName("T_HOLDING");
        }
    }


    public class Holding
    {
        public string H_CODICE { get; set; }
        public int H_INTEGER { get; set; }
        public DateTime H_DATETIME { get; set; }
        public Guid H_GUID { get; set; }
        public bool H_BOOL { get; set; }
        public byte H_BYTE { get; set; }
        public double H_DOUBLE { get; set; }
        public decimal H_DECIMAL { get; set; }
        //public DateTimeOffset H_DATETIMEOFFSET { get; set; }
        public char H_CHAR { get; set; }
        //public sbyte H_SBYTE { get; set; }
        //public ushort H_USHORT { get; set; }
        //public uint H_UINT { get; set; }
        //public ulong H_ULONG { get; set; }
    }

}
