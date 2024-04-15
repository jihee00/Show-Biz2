using JHK2247A5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JHK2247A5.Controllers
{
    [Authorize]
    public class EpisodesController : Controller
    {
        private Manager manager = new Manager();

        // GET: Episodes
        public ActionResult Index()
        {
            return View(manager.EpisodeGetAll());
        }

        // GET: Episodes/Details/5
        public ActionResult Details(int? id)
        {
            var episode = manager.EpisodeGetByIdWithDetail(id.GetValueOrDefault());

            if (episode == null)
            {
                return HttpNotFound();
            }
            else 
            { 
                return View(episode);
            }
        }

        // GET: Episodes/Video/5
        [Route("Episodes/Video/{id}")]
        public ActionResult VideoDetails(int? id)
        {
            var video = manager.EpisodeVideoGetById(id.GetValueOrDefault());

            if (video == null)
            {
                return HttpNotFound();
            }
            else
            {
                return File(video.Video, video.VideoContentType);
            }

        }
    }
}
