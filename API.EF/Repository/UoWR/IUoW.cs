using API.EF.Repository.CategoriaR;
using API.EF.Repository.ProdutoR;

namespace API.EF.Repository.UOWR
{
    public interface IUOW
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        Task Commit();
    }
}
