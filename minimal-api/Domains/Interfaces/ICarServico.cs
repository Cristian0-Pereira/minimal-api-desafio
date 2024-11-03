using minimal_api.Domains.Entities;
using MinimalApi.DTOs;

namespace MinimalAPI.Domains.Interfaces;

public interface ICarServico
{
    List<Car> All(int page = 1, string? name = null, string? model = null);

    Car? SearchFoId(int id);

    void Include(Car car);

    void Update(Car car);

    void Delete(Car car);
}