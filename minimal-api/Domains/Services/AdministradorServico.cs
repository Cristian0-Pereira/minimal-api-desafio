using minimal_api.Domains.Entities;
using MinimalApi.DTOs;
using MinimalAPI.Infrastructure.Db;
using MinimalAPI.Domains.Interfaces;

namespace MinimalAPI.Domains.Services;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;

    public AdministradorServico(DbContexto db)
    {
        _contexto = contexto;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}
