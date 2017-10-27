using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Face2Face.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        // GET: Administration
        public ActionResult Index()
        {
            return View();
        }
        //Eliminar perfis
        public ActionResult DeleteProfiles()
        {
            return View();
        }

        // Gestao Homepage (inclui reporte de abusos)
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        // Pesquisar Eventos()
        // Criar/editar/apagar eventos
        // Aderir eventos
        //Editar/eliminar perfil
        //Reportar abuso/classificar

    }
}