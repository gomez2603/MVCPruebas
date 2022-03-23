using CursoMVC.AccesoDatos.Data;
using CursoMVC.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CursoMVC.Areas.Admin.Controllers
{
    [Area( "Admin")]
    [Authorize(Roles = DS.ROLE_ADMIN)]
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _Db;

        public UsuariosController(ApplicationDbContext db)
        {
            _Db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Api
        [HttpGet]
        public IActionResult ObtenerTodos()
        {
            var userList = _Db.UsuarioAplicacion.ToList();
            var userRole = _Db.UserRoles.ToList();
            var roles = _Db.Roles.ToList();

            foreach(var usuario in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == usuario.Id).RoleId;
                usuario.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return Json(new { data= userList});
        }



        [HttpPost]
        public IActionResult BloquearDesbloquear([FromBody] String id)
        {
            var usuario = _Db.UsuarioAplicacion.FirstOrDefault(u => u.Id == id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Error de Usuario" });
            }
            if (usuario.LockoutEnd != null && usuario.LockoutEnd >DateTime.Now)
            {
                //usuario bloqueado
                usuario.LockoutEnd = DateTime.Now;
            }
            else
            {
                usuario.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _Db.SaveChanges();

            return Json(new { success = true, message = "Opercion Exitosa" });
        }
        #endregion
    }
}
