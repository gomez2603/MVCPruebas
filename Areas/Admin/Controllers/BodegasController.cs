using CursoMVC.AccesoDatos.Repositorio.IRepositorio;
using CursoMVC.Modelos;
using CursoMVC.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =DS.ROLE_ADMIN)]
    public class BodegasController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public BodegasController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Bodega bodega = new Bodega();
            if (id == null)
            {
                return View(bodega);
            }
            
                bodega = _unidadTrabajo.bodega.Obtener(id.GetValueOrDefault());
                if (bodega == null) {
                    return NotFound();
                }
                return View(bodega);
            
            
        }


        #region API
        [HttpGet]
        public IActionResult obtenerTodos() {
            var todos = _unidadTrabajo.bodega.ObtenerTodos();
            return Json(new { data = todos });
        }
        #endregion

        [HttpDelete]
        public IActionResult delete(int id)
        {
            var bodegaDb = _unidadTrabajo.bodega.Obtener(id);
             if(bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al Borrar" });
            }
            _unidadTrabajo.bodega.Remover(bodegaDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado Exitosamente" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)
                {
                    _unidadTrabajo.bodega.Agregar(bodega);

                }
                else
                {
                    _unidadTrabajo.bodega.Actualizar(bodega);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(bodega);
        }

    }
}
