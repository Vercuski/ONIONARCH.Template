using Investor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ONIONARCH.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONIONARCH.Persistence.Contexts;
public abstract class BaseDbContext<T>(DbContextOptions<T> options) : DbContext(options) where T : DbContext
{
    public DbSet<SampleEntity1> SampleEntity1 { get; set; }
    public DbSet<SampleEntity2> SampleEntity2 { get; set; }
}
