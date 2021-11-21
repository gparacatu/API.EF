using API.EF.Infra;
using API.EF.Models;
using API.EF.Repository.Base;

namespace API.EF.Repository.CategoriaR
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {

        }

        public List<Categoria> GetByDescription(string description)
        {
            return Get().Where(c => c.Descricao.Contains(description)).ToList();
        }
    }
}
