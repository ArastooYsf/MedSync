using System.Linq;
using System.Threading.Tasks;
using MedSync.Data;

namespace MedSync.Services;

public class DbSeeder(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task SeedAsync()
    {
        await Task.CompletedTask;
    }
}