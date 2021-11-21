namespace API.EF.Models.DTOs
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public int CategoriaId { get; set; }
    }
}
