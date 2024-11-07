using minimal_api.Domains.Entities;
using MinimalApi.DTOs;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MinimalAPI.Domains.Services;

public class CarServico : ICarServico
{
    private readonly DbContexto _contexto;

    public CarServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public List<Car> All(int? page = 1, string? name = null, string? model = null)
    {
        var query = _contexto.Cars.AsQueryable();
        if(!string.IsNullOrEmpty(name))
        {
            query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{name}%"));
        }

        if(!string.IsNullOrEmpty(model))
        {
            query = query.Where(c => EF.Functions.Like(c.Model.ToLower(), $"{model}"));
        }

        int pageSize = 10;

        if(page.HasValue && page > 0)
            query = query.Skip(((int)page - 1) * pageSize).Take(pageSize);

        return query.ToList();
    }

    public void Delete(Car car)
    {
        _contexto.Cars.Remove(car);
        _contexto.SaveChanges();
    }

    public void Include(Car car)
    {
        _contexto.Cars.Add(car);
        _contexto.SaveChanges();
    }

    public Car? SearchById(int id)
    {
        return _contexto.Cars.Where(c => c.Id == id).FirstOrDefault();
    }

    public void Update(Car car)
    {
        _contexto.Cars.Update(car);
        _contexto.SaveChanges();
    }
}
