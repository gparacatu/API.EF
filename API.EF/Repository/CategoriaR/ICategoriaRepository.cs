using API.EF.Repository.Base;
using API.EF.Models;
namespace API.EF.Repository.CategoriaR
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<List<Categoria>> GetByDescription(string description);
    }
}
