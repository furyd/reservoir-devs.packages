using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservoirDevs.Idempotence.DataTransferObjects;
using ReservoirDevs.Idempotence.Repositories.Interfaces;

namespace ReservoirDevs.Idempotence.Tests.Integration.Setup.Repositories
{
    public sealed class InMemoryIdempotenceTokenRepository : DbContext, IIdempotenceTokenRepository
    {
        public InMemoryIdempotenceTokenRepository(DbContextOptions<InMemoryIdempotenceTokenRepository> options) : base(options)
        {
        }
        
        public Task Create(string token)
        {
            Tokens.Add(new IdempotenceTokenDTO {Token = token, Created = DateTime.Now});
            return SaveChangesAsync();
        }

        public Task<IdempotenceTokenDTO> Retrieve(string token)
        {
            return Tokens.FirstOrDefaultAsync(item => item.Token.Equals(token, StringComparison.InvariantCultureIgnoreCase));
        }

        public DbSet<IdempotenceTokenDTO> Tokens { get; set; }
    }
}