using Microsoft.EntityFrameworkCore;
using ValuteApi.Models;

namespace ValuteApi.Data;

public class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
        
    }

    public DbSet<Valute> Valutes { get; set; }
    public DbSet<Rate> Rates { get; set; }
}