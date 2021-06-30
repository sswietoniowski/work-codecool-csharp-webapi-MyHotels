using Microsoft.EntityFrameworkCore;
using MyHotels.WebApi.Data;
using MyHotels.WebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyHotelsDbContext _context;

        public UnitOfWork(MyHotelsDbContext context)
        {
            this._context = context;
        }

        public IGenericRepository<Country> Countries => new GenericRepository<Country>(_context);

        public IGenericRepository<Hotel> Hotels => new GenericRepository<Hotel>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(true);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
