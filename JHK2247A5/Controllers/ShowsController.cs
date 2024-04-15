using JHK2247A5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JHK2247A5.Controllers
{
    [Authorize]
    public class ShowsController : Controller
    {
        private Manager manager = new Manager();

        // GET: Shows
        public ActionResult Index()
        {
            return View(manager.ShowGetAll());
        }

        // GET: Shows/Details/5
        public ActionResult Details(int? id)
        {
            var show = manager.ShowGetByIdWithDetail(id.GetValueOrDefault());

            if (show == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(show);
            }
        }

        // GET: Shows/5/AddEpisode
        [Authorize(Roles = "Clerk")]
        [Route("Shows/{id}/AddEpisode")]
        public ActionResult AddEpisode(int? id)
        {
            var show = manager.ShowGetByIdWithDetail(id.GetValueOrDefault());
            var preSelect = manager.GenreGetAll().ElementAt(0).Id;

            if (show == null)
            {
                return HttpNotFound();
            }
            else
            {
                var formModel = new EpisodeAddFormViewModel();
                formModel.ShowName = show.Name;
                formModel.ShowId = show.Id;
                formModel.GenreList = new SelectList(manager.GenreGetAll(), "Name", "Name", selectedValue: preSelect);
                return View(formModel);
            }
        }

        // POST: Shows/5/AddEpisode
        [Authorize(Roles = "Clerk")]
        [Route("Shows/{id}/AddEpisode")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddEpisode(EpisodeAddViewModel newItem)
        {
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }

            var addedItem = manager.EpisodeAdd(newItem);
            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", "Episodes", new { id = addedItem.Id });
            }
        }
    }
}
