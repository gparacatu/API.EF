using API.EF.Infra;
using API.EF.Repository.CategoriaR;
using API.EF.Repository.ProdutoR;

namespace API.EF.Repository.UOWR
{
    public class UOW : IUOW
    {
        private ProdutoRepository _produtoRepository;
        private CategoriaRepository _categoriaRepository;
        public IUoW _context;

        public UOW(IUoW context)
        {
            _context = context;
        }

        public IProdutoRepository ProdutoRepository
        {
            get { return _produtoRepository ??= new ProdutoRepository(_context); }
        }

        public ICategoriaRepository CategoriaRepository
        { 
            get { return _categoriaRepository ??= new CategoriaRepository(_context); }
        }
                
        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
