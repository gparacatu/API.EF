using API.EF.Models;
using API.EF.Repository.Base;

namespace API.EF.Repository.ProdutoR
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<List<Produto>> GetByDescription(string description);
    }
}
