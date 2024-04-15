using JHK2247A5.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JHK2247A5.Controllers
{
    [Authorize]
    public class ActorsController : Controller
    {
        private Manager manager = new Manager();

        // GET: Actors
        public ActionResult Index()
        {
            return View(manager.ActorGetAll());
        }

        // GET: Actors/Details/5
        public ActionResult Details(int? id)
        {
            var actor = manager.ActorGetByIdWithDetail(id.GetValueOrDefault());

            if (actor == null)
            {
                return HttpNotFound();
            }
            else 
            {
                actor.Photos = actor.ActorMediaItems
                                    .Where(m => m.ContentType.StartsWith("image"))
                                    .OrderBy(m => m.Caption)
                                    .ToList();

                actor.Documents = actor.ActorMediaItems
                                        .Where(m => m.ContentType.StartsWith("application"))
                                        .OrderBy(m => m.Caption)
                                        .ToList();

                actor.AudioClips = actor.ActorMediaItems
                                          .Where(m => m.ContentType.StartsWith("audio"))
                                          .OrderBy(m => m.Caption)
                                          .ToList();

                actor.VideoClips = actor.ActorMediaItems
                                        .Where(m => m.ContentType.StartsWith("video"))
                                        .OrderBy(m => m.Caption)    
                                        .ToList();

                return View(actor);
            }
        }

        // GET: Actors/MediaItem/5
        [Route("Actors/MediaItem/{id}")]
        public ActionResult MediaItemDownload(int? id)
        {
            var media = manager.ActorMediaItemGetById(id.GetValueOrDefault());

            if (media == null)
            {
                return HttpNotFound();
            }
            else
            {
                string contentDisposition;
                string fileName = media.Caption;

                if (media.ContentType == "application/pdf")
                {
                    contentDisposition = "attachment";
                    if (!fileName.ToLower().EndsWith(".pdf"))
                    {
                        fileName += ".pdf"; 
                    }
                }
                else
                {
                    contentDisposition = "inline"; 
                }

                Response.AppendHeader("Content-Disposition", $"{contentDisposition}; filename=\"{fileName}\"");
                return File(media.Content, media.ContentType);
            }

        }

        // GET: Actors/Create
        [Authorize(Roles = "Executive")]
        public ActionResult Create()
        {
            return View(new ActorAddViewModel());
        }

        // POST: Actors/Create
        [Authorize(Roles = "Executive")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ActorAddViewModel newActor)
        {
            if (!ModelState.IsValid)
            {
                return View(newActor);
            }

            var addedActor = manager.ActorAdd(newActor);

            if (addedActor == null)
            {
                return View(newActor);
            }
            else
            {
                return RedirectToAction("Details", new { id = addedActor.Id });
            }   
        }

        // GET: Actors/5/AddContent
        [Authorize(Roles = "Executive")]
        [Route("Actors/{id}/AddContent")]
        public ActionResult AddContent(int? id)
        {
            var actor = manager.ActorGetByIdWithDetail(id.GetValueOrDefault());

            if (actor == null)
            {
                return HttpNotFound();
            }
            else
            {
                var form = new ActorMediaItemAddFormViewModel();
                form.ActorId = actor.Id;
                form.ActorName = actor.Name;
                return View(form);
            }
        }

        // POST: Actors/5/AddContent
        [Authorize(Roles = "Executive")]
        [HttpPost]
        [Route("Actors/{id}/AddContent")]
        public ActionResult AddContent(int? id, ActorMediaItemAddViewModel newItem)
        {
            if (!ModelState.IsValid && id.GetValueOrDefault() == newItem.ActorId)
            {
                return View(newItem);
            }

            var addedItem = manager.ActorMediaItemAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", new { id = addedItem.Id });
            }
        }

        // GET: Actors/5/AddShow
        [Authorize(Roles = "Coordinator")]
        [Route("Actors/{id}/AddShow")]
        public ActionResult AddShow(int? id)
        {
            var actor = manager.ActorGetByIdWithDetail(id.GetValueOrDefault());
            var preSelect = manager.GenreGetAll().ElementAt(0).Id;

            if (actor == null)
            {
                return HttpNotFound();
            }
            else 
            {
                var formModel = new ShowAddFormViewModel();
                formModel.ActorId = actor.Id;
                formModel.AcotrName = actor.Name;

                formModel.GenreList = new SelectList(manager.GenreGetAll(), "Name", "Name", selectedValue: preSelect);
           
                var currentActors = new List<int> { actor.Id };

                formModel.ActorsList = new MultiSelectList
                    (items: manager.ActorGetAll(),
                    dataValueField: "Id",
                    dataTextField: "Name",
                    selectedValues: currentActors);

                return View(formModel);
            }
        }

        // POST: Actors/5/AddShow
        [Authorize(Roles = "Coordinator")]
        [Route("Actors/{id}/AddShow")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddShow(ShowAddViewModel newItem)
        {
            if (!ModelState.IsValid)
            { 
                return View(newItem);
            }

            try
            {
                var addedItem = manager.ShowAdd(newItem);

                if (addedItem == null)
                {
                    return View(newItem);
                }
                else
                {
                    return RedirectToAction("Details", "Shows", new { id = addedItem.Id });
                }
            }
            catch
            {
                return View(newItem);
            } 
        }
    }
}
