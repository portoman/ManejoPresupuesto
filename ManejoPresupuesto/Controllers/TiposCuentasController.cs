using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;

        public IServicioUsuarios servicioUsuarios { get; }

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            TipoCuenta tipoCuenta = new TipoCuenta();
            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            IEnumerable<TipoCuenta> tiposCuentas = await repositorioTiposCuentas.Obtener(tipoCuenta.UsuarioId);
            return View(tiposCuentas);

        }
        public IActionResult Crear()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            bool yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await repositorioTiposCuentas.Crear(tipoCuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            TipoCuenta tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);

            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            TipoCuenta tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Actualizar(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            TipoCuenta tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            TipoCuenta tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioTiposCuentas.Borrar(id);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            int usuarioId = servicioUsuarios.ObtenerUsuarioId();
            IEnumerable<TipoCuenta> tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
            new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);
            return Ok();
        }
    }
}
