using API.EF.Models;
using API.EF.Models.DTOs;
using API.EF.Repository.UOWR;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.EF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUOW _uow;
        private readonly IMapper _mapper;

        public ProdutosController(IUOW context, IMapper mapper)
        {
            _uow = context;
            _mapper = mapper;
        }

        [HttpGet("nome/{nome}")]
        public ActionResult<List<ProdutoDTO>> GetByDescription(string nome)
        {
            var produtos = _uow.ProdutoRepository.GetByDescription(nome);
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDTO; 
        }

        // GET: api/Produtos
        [HttpGet]
        public ActionResult<List<ProdutoDTO>> GetProdutos()
        {
            var produtos = _uow.ProdutoRepository.Get();
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDTO;
        }

        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public ActionResult<ProdutoDTO> GetProduto(int id)
        {
            var produto = _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
            {
                return NotFound();
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);
            return produtoDTO;
        }

        // PUT: api/Produtos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDTO);
            _uow.ProdutoRepository.Update(produto);

            try
            {
                await _uow.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Produtos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> PostProduto(ProdutoDTO produtoRequestDTO)
        {
            var produto = _mapper.Map<Produto>(produtoRequestDTO);

            _uow.ProdutoRepository.Add(produto);
            await _uow.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return CreatedAtAction("GetProduto", new { id = produto.ProdutoId }, produtoDTO);
        }

        // DELETE: api/Produtos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProdutoDTO>> DeleteProduto(int id)
        {
            var produto = _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if(produto == null)
            {
                return NotFound();
            }

            _uow.ProdutoRepository.Delete(produto);
            await _uow.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        private bool ProdutoExists(int id)
        {
            return _uow.ProdutoRepository.Get().Any(e => e.ProdutoId == id);
        }
    }
}
