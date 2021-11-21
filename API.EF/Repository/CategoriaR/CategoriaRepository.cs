using API.EF.Infra;
using API.EF.Models;
using API.EF.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace API.EF.Repository.CategoriaR
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<Categoria>> GetByDescription(string description)
        {
            return await Get().Where(c => c.Descricao.Contains(description)).ToListAsync();
        }
    }
}
