using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaConsultasUVV.Data;
using SistemaConsultasUVV.Models;

namespace SistemaConsultasUVV.Controllers
{
    public class ContaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public ContaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Conta/Registrar
        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        // POST: /Conta/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verifica se já existe usuário com este e-mail
            bool emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == model.Email);
            if (emailJaExiste)
            {
                ModelState.AddModelError(nameof(model.Email), "Este e-mail já está cadastrado.");
                return View(model);
            }

            var usuario = new Usuario
            {
                Nome = model.Nome,
                Email = model.Email,
                DataCadastro = DateTime.Now
            };

            // Gera o hash seguro da senha (nunca salvamos texto puro)
            usuario.SenhaHash = _passwordHasher.HashPassword(usuario, model.Senha);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Cadastro realizado com sucesso! Faça login para continuar.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Conta/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Conta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (usuario is null)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, model.Senha);

            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            // Monta as claims (identidade) do usuário autenticado
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Consultas");
        }

        // POST: /Conta/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
