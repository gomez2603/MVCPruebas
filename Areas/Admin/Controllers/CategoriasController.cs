using CursoMVC.AccesoDatos.Repositorio.IRepositorio;
using CursoMVC.Modelos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriasController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriasController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Categoria categoria = new Categoria();
            if (id == null)
            {
                return View(categoria);
            }
          
                categoria = _unidadTrabajo.categoria.Obtener(id.GetValueOrDefault());
                if (categoria == null) {
                    return NotFound();
                }
                return View(categoria);
          
        }


        #region API
        [HttpGet]
        public IActionResult obtenerTodos() {
            var todos = _unidadTrabajo.categoria.ObtenerTodos();
            return Json(new { data = todos });
        }
        #endregion

        [HttpDelete]
        public IActionResult delete(int id)
        {
            var categoriaDb = _unidadTrabajo.categoria.Obtener(id);
             if(categoriaDb == null)
            {
                return Json(new { success = false, message = "Error al Borrar" });
            }
            _unidadTrabajo.categoria.Remover(categoriaDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado Exitosamente" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.Id == 0)
                {
                    _unidadTrabajo.categoria.Agregar(categoria);

                }
                else
                {
                    _unidadTrabajo.categoria.Actualizar(categoria);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

    }
}
