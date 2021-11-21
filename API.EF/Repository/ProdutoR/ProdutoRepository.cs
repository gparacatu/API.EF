using API.EF.Infra;
using API.EF.Models;
using API.EF.Repository.Base;

namespace API.EF.Repository.ProdutoR
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(IUoW context) : base(context)
        {

        }

        public List<Produto> GetByDescription(string description)
        {
            return Get().Where(p => p.Descricao.Contains(description)).ToList();
        }
    }
}
