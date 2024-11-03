using.MinimalAPI.Domains.Entities;
using.MinimalAPI.DTOs;

namespace MinimalAPI.Domains.Interfaces;

public insterface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
}