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
    public class CategoriasController : ControllerBase
    {
        private readonly IUOW _uow;
        private readonly IMapper _mapper;

        public CategoriasController(IUOW context, IMapper mapper)
        {
            _uow = context;
            _mapper = mapper;
        }

        // GET: api/Categorias
        [HttpGet]
        public ActionResult<List<CategoriaDTO>> GetCategorias()
        {
            var categoria = _uow.CategoriaRepository.Get();

            var categoriaDTO = _mapper.Map<List<CategoriaDTO>>(categoria);

            return categoriaDTO;
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public ActionResult<CategoriaDTO> GetCategoria(int id)
        {
            var categoria = _uow.CategoriaRepository.GetById(p => p.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return categoriaDTO;
        }

        [HttpGet("nome/{nome}")]
        public ActionResult<List<CategoriaDTO>> GetByDescription(string nome)
        {
            var categoria = _uow.CategoriaRepository.GetByDescription(nome);

            return _mapper.Map<List<CategoriaDTO>>(categoria);

        }

        // PUT: api/Categorias/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDTO categoriaRequestDTO)
        {
            if (id != categoriaRequestDTO.CategoriaId)
            {
                return BadRequest();
            }

            var categoria = _mapper.Map<Categoria>(categoriaRequestDTO);

            _uow.CategoriaRepository.Update(categoria);

            try
            {
                await _uow.Commit();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDTO);
        }

        // POST: api/Categorias
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> PostCategoria(CategoriaDTO categoriaRquestDTO)
        {
            var categoria = _mapper.Map<Categoria>(categoriaRquestDTO);

            _uow.CategoriaRepository.Add(categoria);
            await _uow.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return CreatedAtAction("GetCategoria", new { id = categoria.CategoriaId }, categoriaDTO);
        }

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoria(int id)
        {
            var categoria = _uow.CategoriaRepository.GetById(c => c.CategoriaId == id);
            if (categoria == null)
            {
                return NotFound();
            }

            _uow.CategoriaRepository.Delete(categoria);
            await _uow.Commit();

            var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDTO);
        }

        private bool CategoriaExists(int id)
        {
            return _uow.CategoriaRepository.Get().Any(e => e.CategoriaId == id);
        }
    }
}
