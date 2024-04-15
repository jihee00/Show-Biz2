using AutoMapper;
using JHK2247A5.Data;
using JHK2247A5.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

// ************************************************************************************
// WEB524 Project Template V2 == 2241-5f12b092-4dee-4e72-ad57-96b8a2549cc3
//
// By submitting this assignment you agree to the following statement.
// I declare that this assignment is my own work in accordance with the Seneca Academic
// Policy. No part of this assignment has been copied manually or electronically from
// any other source (including web sites) or distributed to other students.
// ************************************************************************************

namespace JHK2247A5.Controllers
{
    public class Manager
    {
        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper instance
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            var config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Product, ProductBaseViewModel>();

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();
                cfg.CreateMap<Genre, GenreBaseViewModel>();

                cfg.CreateMap<Actor, ActorBaseViewModel>();
                cfg.CreateMap<Actor, ActorWithShowInfoViewModel>();
                cfg.CreateMap<ActorAddViewModel, Actor>();

                cfg.CreateMap<ActorMediaItem, ActorMediaItemBaseViewModel>();
                cfg.CreateMap<ActorMediaItem, ActorMediaItemWithContentViewModel>();

                cfg.CreateMap<Show, ShowBaseViewModel>();
                cfg.CreateMap<Show, ShowWithInfoViewModel>();
                cfg.CreateMap<ShowAddViewModel, Show>();

                cfg.CreateMap<Episode, EpisodeBaseViewModel>();
                cfg.CreateMap<Episode, EpisodeWithShowNameViewModel>();
                cfg.CreateMap<Episode, EpisodeVideoViewModel>();
                cfg.CreateMap<EpisodeAddViewModel, Episode>();
            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }


        // Add your methods below and call them from controllers. Ensure that your methods accept
        // and deliver ONLY view model objects and collections. When working with collections, the
        // return type is almost always IEnumerable<T>.
        //
        // Remember to use the suggested naming convention, for example:
        // ProductGetAll(), ProductGetById(), ProductAdd(), ProductEdit(), and ProductDelete().

        public IEnumerable<GenreBaseViewModel> GenreGetAll()
        {
            return mapper.Map<IEnumerable<Genre>, IEnumerable<GenreBaseViewModel>>(ds.Genres.OrderBy(g => g.Name));
        }

        public IEnumerable<ActorBaseViewModel> ActorGetAll() 
        {
            return mapper.Map<IEnumerable<Actor>, IEnumerable<ActorBaseViewModel>>(ds.Actors.OrderBy(a => a.Name));
        }

        public ActorWithShowInfoViewModel ActorGetByIdWithDetail(int id)
        { 
            var actor = ds.Actors.Include("Shows").Include("ActorMediaItems").SingleOrDefault(a => a.Id == id);
            return actor == null ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(actor);
        }

        public ActorMediaItemWithContentViewModel ActorMediaItemGetById(int id)
        { 
            var media = ds.ActorMediaItems.SingleOrDefault(m => m.Id == id);
            return (media == null) ? null : mapper.Map<ActorMediaItem, ActorMediaItemWithContentViewModel>(media);
        }

        public ActorWithShowInfoViewModel ActorAdd(ActorAddViewModel newActor)
        {
            var actor = ds.Actors.Add(mapper.Map<ActorAddViewModel, Actor>(newActor));
            actor.Executive = HttpContext.Current.User.Identity.Name;
            ds.SaveChanges();
            return (actor == null) ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(actor);
        }

        public ActorWithShowInfoViewModel ActorMediaItemAdd(ActorMediaItemAddViewModel newItem)
        {
             var actor = ds.Actors.Find(newItem.ActorId);

            if (actor == null)
            {
                return null;
            }
            else
            {
                var addedItem = new ActorMediaItem();
                ds.ActorMediaItems.Add(addedItem);
                addedItem.Caption = newItem.Caption;
                addedItem.Actor = actor;

                byte[] contentBytes = new byte[newItem.ContentUpload.ContentLength];
                newItem.ContentUpload.InputStream.Read(contentBytes, 0, newItem.ContentUpload.ContentLength);

                addedItem.Content = contentBytes;
                addedItem.ContentType = newItem.ContentUpload.ContentType;

                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(actor);
            }

        }

        public IEnumerable<ShowBaseViewModel> ShowGetAll()
        { 
            return mapper.Map<IEnumerable<Show>, IEnumerable<ShowBaseViewModel>>(ds.Shows.OrderBy(s => s.Name));
        }

        public ShowWithInfoViewModel ShowGetByIdWithDetail(int id)
        { 
            var show = ds.Shows.Include("Actors").Include("Episodes").SingleOrDefault(s => s.Id == id);
            return show == null ? null : mapper.Map<Show, ShowWithInfoViewModel>(show);
        }

        public ShowWithInfoViewModel ShowAdd(ShowAddViewModel newItem) 
        {
            var addedItem = ds.Shows.Add(mapper.Map<ShowAddViewModel, Show>(newItem));

            addedItem.Coordinator = HttpContext.Current.User.Identity.Name;

            var actors = new List<Actor>();

            foreach (var actorId in newItem.ActorIds)
            {
                var actor = ds.Actors.Find(actorId);
                if (actor == null)
                {
                    return null;
                }
                else 
                {
                    actors.Add(actor);
                }
            }

            addedItem.Actors = actors;

            ds.SaveChanges();

            return (addedItem == null) ? null : mapper.Map<Show, ShowWithInfoViewModel>(addedItem);
        }

        public IEnumerable<EpisodeWithShowNameViewModel> EpisodeGetAll()
        { 
            return mapper.Map<IEnumerable<Episode>, IEnumerable<EpisodeWithShowNameViewModel>>(ds.Episodes.Include("Show").OrderBy(e => e.Show.Name).ThenBy(e => e.SeasonNumber).ThenBy(e => e.EpisodeNumber));   
        }

        public EpisodeWithShowNameViewModel EpisodeGetByIdWithDetail(int id)
        {
            var episode = ds.Episodes.Include("Show").SingleOrDefault(e => e.Id == id);
            return episode == null ? null : mapper.Map<Episode, EpisodeWithShowNameViewModel>(episode);
        }

        public EpisodeWithShowNameViewModel EpisodeAdd(EpisodeAddViewModel newItem)
        {
            var show = ds.Shows.Find(newItem.ShowId);

            if (show == null)
            {
                return null;
            }
            else 
            {
                var addedItem = ds.Episodes.Add(mapper.Map<EpisodeAddViewModel, Episode>(newItem));
                byte[] videoBytes = new byte[newItem.VideoUpload.ContentLength];
                newItem.VideoUpload.InputStream.Read(videoBytes, 0, newItem.VideoUpload.ContentLength);

                addedItem.Clerk = HttpContext.Current.User.Identity.Name;
                addedItem.Show = show;
                addedItem.Video = videoBytes;
                addedItem.VideoContentType = newItem.VideoUpload.ContentType;

                ds.SaveChanges();
                return (addedItem == null) ? null : mapper.Map<Episode, EpisodeWithShowNameViewModel>(addedItem);
            }
        }

        public EpisodeVideoViewModel EpisodeVideoGetById(int id)
        {
            var video = ds.Episodes.Find(id);
            return video == null ? null : mapper.Map<Episode, EpisodeVideoViewModel>(video);
        }

        // *** Add your methods ABOVE this line **

        #region Role Claims

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
        }

        #endregion

        #region Load Data Methods

        // Add some programmatically-generated objects to the data store
        // Write a method for each entity and remember to check for existing
        // data first.  You will call this/these method(s) from a controller action.

        public bool LoadGenres()
        {
            if (ds.Genres.Count() > 0) { return false; }

            ds.Genres.Add(new Genre { Name = "Comedy" });
            ds.Genres.Add(new Genre { Name = "Science Fiction" });
            ds.Genres.Add(new Genre { Name = "Fantasy" });
            ds.Genres.Add(new Genre { Name = "Horror" });
            ds.Genres.Add(new Genre { Name = "Crime" });
            ds.Genres.Add(new Genre { Name = "Drama" });
            ds.Genres.Add(new Genre { Name = "Kids" });
            ds.Genres.Add(new Genre { Name = "Mystery" });
            ds.Genres.Add(new Genre { Name = "Thriller" });
            ds.Genres.Add(new Genre { Name = "Biography" });

            ds.SaveChanges();
            return true;
        }

        public bool LoadActors()
        {
            if (ds.Actors.Count() > 0) { return false; }
            var admin = User.Name;

            ds.Actors.Add(new Actor 
            {
                Name = "Millie Bobby Brown",
                BirthDate = new DateTime(2004, 2, 19),
                Height = 1.61,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/04/Millie_Bobby_Brown_by_Gage_Skidmore.jpg/800px-Millie_Bobby_Brown_by_Gage_Skidmore.jpg",
                Executive = admin
            });

            ds.Actors.Add(new Actor 
            {
                Name = "Finn Wolfhard", 
                BirthDate = new DateTime(2002, 12, 23),
                Height = 1.8, 
                ImageUrl = "https://ew.com/thmb/4kxaF_YU1JFCeSXut7qPY7gx2m4=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/Finn-Wolfhard-in-the-film-Hell-of-a-Summer-060523-2-59a74555f3c14ba1b31c96d112fa4bd8.jpg",
                Executive = admin
            });

            ds.Actors.Add(new Actor
            {
                Name = "Adam DeVine",
                BirthDate = new DateTime(1983, 11, 7),
                Height = 1.73,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/db/Adam_DeVine_Macy%27s_Thanksgiving_Parade_2022_%28cropped%29.jpg/330px-Adam_DeVine_Macy%27s_Thanksgiving_Parade_2022_%28cropped%29.jpg",
                Executive = admin
            });

            ds.SaveChanges();
            return true;
        }

        public bool LoadShows()
        {
            if (ds.Shows.Count() > 0) { return false; }
            var admin = User.Name;

            var millie = ds.Actors.SingleOrDefault(a => a.Name == "Millie Bobby Brown");
            if (millie == null) { return false; }

            var finn = ds.Actors.SingleOrDefault(a => a.Name == "Finn Wolfhard");
            if (finn == null) { return false; }

            ds.Shows.Add(new Show
            {
                Actors = new Actor[] { millie, finn },
                Name = "Stranger Things",
                Genre = "Science Fiction",
                ReleaseDate = new DateTime(2016, 7, 15),
                ImageUrl = "https://i.ebayimg.com/images/g/zXsAAOSwnBpdMufI/s-l1600.jpg",
                Coordinator = admin
            });
            ds.SaveChanges();

            var adam = ds.Actors.SingleOrDefault(a => a.Name == "Adam DeVine");
            if (adam == null) { return false; }

            ds.Shows.Add(new Show
            {
                Actors = new Actor[] { millie, adam },
                Name = "Modern Family",
                Genre = "Comedy",
                ReleaseDate = new DateTime(2009, 9, 23),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b6/Modern_Family_season_10_poster.jpg",
                Coordinator = admin
            });
            ds.SaveChanges();

            return true;
        }

        public bool LoadEpisodes()
        {
            if (ds.Episodes.Count() > 0) { return false; }
            var admin = User.Name;

            var modernFamily = ds.Shows.SingleOrDefault(s => s.Name == "Modern Family");
            if (modernFamily == null) { return false; }
            ds.Episodes.Add(new Episode
            {
                Show = modernFamily,
                Name = "Closet? You'll Love It!",
                SeasonNumber = 6,
                EpisodeNumber = 17,
                Genre = "Comedy",
                AirDate = new DateTime(2015, 3, 4),
                ImageUrl = "https://resizing.flixster.com/2cASiuoSzT7qrsUoUcWzCdqpzeY=/fit-in/352x330/v2/https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/assets/p10778822_b_v9_ac.jpg",
                Clerk = admin
            });
            ds.Episodes.Add(new Episode
            {
                Show = modernFamily,
                Name = "Spring Break",
                SeasonNumber = 6,
                EpisodeNumber = 18,
                Genre = "Comedy",
                AirDate = new DateTime(2015, 3, 25),
                ImageUrl = "https://resizing.flixster.com/2cASiuoSzT7qrsUoUcWzCdqpzeY=/fit-in/352x330/v2/https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/assets/p10778822_b_v9_ac.jpg",
                Clerk = admin
            });
            ds.Episodes.Add(new Episode
            {
                Show = modernFamily,
                Name = "Grill, Interrupted",
                SeasonNumber = 6,
                EpisodeNumber = 19,
                Genre = "Comedy",
                AirDate = new DateTime(2015, 4, 1),
                ImageUrl = "https://resizing.flixster.com/2cASiuoSzT7qrsUoUcWzCdqpzeY=/fit-in/352x330/v2/https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/assets/p10778822_b_v9_ac.jpg",
                Clerk = admin
            });
            ds.SaveChanges();

            var strangerThings = ds.Shows.SingleOrDefault(s => s.Name == "Stranger Things");
            if (strangerThings == null) { return false; }
            ds.Episodes.Add(new Episode
            {
                Show = strangerThings,
                Name = "Chapter One: The Vanishing of Will Byers",
                SeasonNumber = 1,
                EpisodeNumber = 1,
                Genre = "Science Fiction",
                AirDate = new DateTime(2016, 7, 15),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b1/Stranger_Things_season_1.jpg",
                Clerk = admin
            });
            ds.Episodes.Add(new Episode
            {
                Show = strangerThings,
                Name = "Chapter Two: The Weirdo on Maple Street",
                SeasonNumber = 1,
                EpisodeNumber = 2,
                Genre = "Science Fiction",
                AirDate = new DateTime(2016, 7, 15),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b1/Stranger_Things_season_1.jpg",
                Clerk = admin
            });
            ds.Episodes.Add(new Episode
            {
                Show = strangerThings,   
                Name = "Chapter Three: Holly, Jolly",
                SeasonNumber = 1,
                EpisodeNumber = 3,
                Genre = "Science Fiction",
                AirDate = new DateTime(2016, 7, 15),
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/b1/Stranger_Things_season_1.jpg",
                Clerk = admin
            });
            ds.SaveChanges();

            return true;
        }

        public bool LoadRoles()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Monitor the progress
            bool done = false;

            // *** Role claims ***
            if (ds.RoleClaims.Count() == 0)
            {
                ds.RoleClaims.Add(new RoleClaim() { Name = "Administrator" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Executive" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Coordinator" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Clerk" });

                ds.SaveChanges();
                done = true;
            }

            return done;
        }

        public bool RemoveData()
        {
            try
            {
                foreach (var e in ds.RoleClaims)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Genres)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Actors)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Shows)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var e in ds.Episodes)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    #endregion

    #region RequestUser Class

    // This "RequestUser" class includes many convenient members that make it
    // easier work with the authenticated user and render user account info.
    // Study the properties and methods, and think about how you could use this class.

    // How to use...
    // In the Manager class, declare a new property named User:
    //    public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value:
    //    User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }

        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }

        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }

        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

    #endregion

}