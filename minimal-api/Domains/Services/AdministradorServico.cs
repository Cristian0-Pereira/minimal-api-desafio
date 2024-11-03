using MinimalAPI.Domains.Entities;
using MinimalAPI.Domains.Interfaces;
using MinimalAPI.DTOs;
using MinimalAPI.Infrastructure.Db;

namespace MinimalAPI.Domains.Services;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto db)
    {
        _contexto = contexto;
    }
    public Administador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}
