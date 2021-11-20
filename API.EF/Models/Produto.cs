using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace API.EF.Models
{
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }
        [Required]
        [MaxLength(255)]
        public string? Descricao { get; set; }
        [Required]
        public decimal Valor { get; set; }
        public Categoria? Categoria { get; set; }
        //[BindNever]
        public int CategoriaId { get; set; }
    }
}
