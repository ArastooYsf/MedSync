using System.Linq;
using System.Threading.Tasks;
using MedSync.Data;

namespace MedSync.Services;

public class DbSeeder
{
    private readonly AppDbContext _context;

    public DbSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await Task.CompletedTask;
    }
}