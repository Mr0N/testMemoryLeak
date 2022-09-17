using System;
using System.Diagnostics;
using System.Diagnostics.PerformanceData;
using Microsoft.EntityFrameworkCore;

var builder = new DbContextOptionsBuilder();
builder.UseSqlite("Filename=MyDatabase.db");
var sql = new SqlLiteDbContext(builder.Options);
sql.ChangeTracker.AutoDetectChangesEnabled = true;
sql.Database.EnsureDeleted();
sql.Database.EnsureCreated();
var process = Process.GetCurrentProcess();
PerformanceCounter ramCounter = new PerformanceCounter("Process", "Working Set", process.ProcessName);
var random = new Random();
var ls = new List<string>();
for (int i = 0; i < 100; i++)
{
    sql.information.Add(new Info()
    {
        BigOrMiniText = string.Concat(Enumerable.Repeat('1', random.Next(10000, 100000)))
    });
    sql.SaveChanges();
    string text = string.Concat(Enumerable.Repeat('1', random.Next(10000, 100000)));
    //ls.Add(text);
    GC.Collect();
    //await Task.Delay(5000);
    Console.WriteLine(ramCounter.NextValue());
}
///ls.Clear();
//sql.ChangeTracker.Clear();
//sql.Dispose();
GC.Collect();
await Task.Delay(1000*120);
Console.WriteLine("End "+ramCounter.NextValue());
sql.Database.EnsureDeleted();
Console.ReadKey();









class SqlLiteDbContext :DbContext
{
    public DbSet<Info> information { set; get; }
    public SqlLiteDbContext(DbContextOptions options):base(options)
    {

    }
}
class Info
{
    public int Id { set; get; }
    public string BigOrMiniText { set; get; }
}
