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
    [Authorize(Roles = DS.ROLE_ADMIN)]
    public class MarcasController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        public MarcasController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Marca marca = new Marca();
            if (id == null)
            {
                return View(marca);
            }
          
                marca = _unidadTrabajo.marca.Obtener(id.GetValueOrDefault());
                if (marca == null) {
                    return NotFound();
                }
                return View(marca);
          
        }


        #region API
        [HttpGet]
        public IActionResult obtenerTodos() {
            var todos = _unidadTrabajo.marca.ObtenerTodos();
            return Json(new { data = todos });
        }
        #endregion

        [HttpDelete]
        public IActionResult delete(int id)
        {
            var macarDb = _unidadTrabajo.marca.Obtener(id);
             if(macarDb == null)
            {
                return Json(new { success = false, message = "Error al Borrar" });
            }
            _unidadTrabajo.marca.Remover(macarDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado Exitosamente" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Marca marca)
        {
            if (ModelState.IsValid)
            {
                if (marca.Id == 0)
                {
                    _unidadTrabajo.marca.Agregar(marca);

                }
                else
                {
                    _unidadTrabajo.marca.Actualizar(marca);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            return View(marca);
        }

    }
}
