using minimal_api.Domains.Entities;
using MinimalApi.DTOs;

namespace MinimalAPI.Domains.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}