using minimal_api.Domains.Entities;
using MinimalApi.DTOs;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domains.Interfaces;

namespace MinimalAPI.Domains.Services;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Administrador? SearchById(int id)
    {
        return _contexto.Administradores.FirstOrDefault(c => c.Id == id);
    }

    public Administrador Include(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _contexto.Administradores.FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }

    public List<Administrador> All(int? page)
    {
        var query = _contexto.Administradores.AsQueryable();

        int pageSize = 10;

        if(page.HasValue && page > 0)
            query = query.Skip(((int)page - 1) * pageSize).Take(pageSize);

        return query.ToList();
    }
}
