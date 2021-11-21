using API.EF.Infra;
using API.EF.Models;
using API.EF.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace API.EF.Repository.ProdutoR
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<Produto>> GetByDescription(string description)
        {
            return await Get().Where(p => p.Descricao.Contains(description)).ToListAsync();
        }
    }
}
