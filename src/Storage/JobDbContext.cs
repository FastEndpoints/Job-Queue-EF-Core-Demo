using Microsoft.EntityFrameworkCore;

namespace JobQueuesEfCoreDemo;

public class JobDbContext(DbContextOptions o) : DbContext(o)
{
    public DbSet<JobRecord> Jobs { get; set; }
}