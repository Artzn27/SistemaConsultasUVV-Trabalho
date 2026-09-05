using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Controllers
{
    // Protege TODAS as ações deste controller: só usuários autenticados acessam
    [Authorize]
    public class ConsultasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConsultasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Recupera o Id do usuário logado a partir do cookie de autenticação
        private int UsuarioLogadoId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Consultas
        public async Task<IActionResult> Index()
        {
            var minhasConsultas = await _context.Consultas
                .Where(c => c.UsuarioId == UsuarioLogadoId)
                .OrderBy(c => c.DataHora)
                .ToListAsync();

            return View(minhasConsultas);
        }

        // GET: /Consultas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Consultas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Especialidade,DataHora,Descricao")] Consulta consulta)
        {
            // O UsuarioId nunca vem do formulário (evita que alguém marque consulta em nome de outro usuário)
            consulta.UsuarioId = UsuarioLogadoId;
            ModelState.Remove(nameof(Consulta.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(consulta);
            }

            _context.Add(consulta);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Consulta registrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Consultas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioLogadoId);

            if (consulta is null) return NotFound();

            return View(consulta);
        }

        // POST: /Consultas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Especialidade,DataHora,Descricao")] Consulta consultaEditada)
        {
            if (id != consultaEditada.Id) return NotFound();

            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioLogadoId);

            if (consulta is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(consultaEditada);
            }

            consulta.Especialidade = consultaEditada.Especialidade;
            consulta.DataHora = consultaEditada.DataHora;
            consulta.Descricao = consultaEditada.Descricao;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Consulta atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Consultas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioLogadoId);

            if (consulta is null) return NotFound();

            return View(consulta);
        }

        // POST: /Consultas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consulta = await _context.Consultas
                .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == UsuarioLogadoId);

            if (consulta is not null)
            {
                _context.Consultas.Remove(consulta);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Consulta excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
