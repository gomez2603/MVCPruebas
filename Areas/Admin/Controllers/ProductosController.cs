using CursoMVC.AccesoDatos.Repositorio.IRepositorio;
using CursoMVC.Modelos;
using CursoMVC.Modelos.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;

namespace CursoMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductosController : Controller
    {

        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _hostenvironment;
        public ProductosController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment hostenvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _hostenvironment = hostenvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ProductoViewModel productoView = new ProductoViewModel
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.categoria.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                }),
                MarcaLista = _unidadTrabajo.marca.ObtenerTodos().Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.Id.ToString()
                }),
                PadreLista = _unidadTrabajo.producto.ObtenerTodos().Select(p => new SelectListItem
                {
                    Text = p.Descripcion,
                    Value = p.Id.ToString()
                })

            };
            if (id == null)
            {
                return View(productoView);
            }

            productoView.Producto= _unidadTrabajo.producto.Obtener(id.GetValueOrDefault());
            if (productoView.Producto == null) {
                return NotFound();
            }
            return View(productoView);

        }
    

        #region API
        [HttpGet]
        public IActionResult obtenerTodos() {
            var todos = _unidadTrabajo.producto.ObtenerTodos(incluirPropiedades:"Categoria,Marca");
            return Json(new { data = todos });
        }
        #endregion

        [HttpDelete]
        public IActionResult delete(int id)
        {
            var productoDb = _unidadTrabajo.producto.Obtener(id);
             if(productoDb == null)
            {
                return Json(new { success = false, message = "Error al Borrar" });
            }
             //Eliminar Imagen
            string webRootPath = _hostenvironment.WebRootPath;
            var imagenPath = Path.Combine(webRootPath, productoDb.ImagenURL.TrimStart('\\'));
            if (System.IO.File.Exists(imagenPath))
            {
                System.IO.File.Delete(imagenPath);
            }
            _unidadTrabajo.producto.Remover(productoDb);
            _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Borrado Exitosamente" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductoViewModel productoVM)
        {
            if (ModelState.IsValid)
            {
                //Cargar Imagenes
                string webRootPath = _hostenvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"img\productos");
                    var extension = Path.GetExtension(files[0].FileName);
                    if (productoVM.Producto.ImagenURL != null) {

                        //esto es para editar la imagen anterior}
                        var imagenPath = Path.Combine(webRootPath, productoVM.Producto.ImagenURL.TrimStart('\\'));
                        if (System.IO.File.Exists(imagenPath)) {
                            System.IO.File.Delete(imagenPath);
                        }
                    }

                    using (var filesStreams = new FileStream(Path.Combine(uploads, filename + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productoVM.Producto.ImagenURL = @"\img\productos\" + filename + extension;
                }
                else
                {
                    //si no cambia la imagen 
                    if (productoVM.Producto.Id != 0) {
                        Producto productoDb = _unidadTrabajo.producto.Obtener(productoVM.Producto.Id);
                        productoVM.Producto.ImagenURL = productoDb.ImagenURL;
                    } 
                }





                if (productoVM.Producto.Id == 0)
                {
                    _unidadTrabajo.producto.Agregar(productoVM.Producto);

                }
                else
                {
                    _unidadTrabajo.producto.Actualizar(productoVM.Producto);
                }
                _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productoVM.CategoriaLista = _unidadTrabajo.categoria.ObtenerTodos().Select(c => new SelectListItem
                {
                    Text = c.Nombre,
                    Value = c.Id.ToString()
                });
                productoVM.MarcaLista = _unidadTrabajo.marca.ObtenerTodos().Select(m => new SelectListItem
                {
                    Text = m.Nombre,
                    Value = m.Id.ToString()
                });
                productoVM.PadreLista = _unidadTrabajo.producto.ObtenerTodos().Select(p => new SelectListItem
                {
                    Text = p.Descripcion,
                    Value = p.Id.ToString()
                });
                if (productoVM.Producto.Id != 0) {
                    productoVM.Producto= _unidadTrabajo.producto.Obtener(productoVM.Producto.Id);
                }

            }
            return View(productoVM);
        }

    }
}
