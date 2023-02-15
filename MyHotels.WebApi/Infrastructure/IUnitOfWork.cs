using MyHotels.WebApi.Domain;
using System;
using System.Threading.Tasks;

namespace MyHotels.WebApi.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Country> Countries { get; }
    IGenericRepository<Hotel> Hotels { get; }
    Task Save();
}