using JHK2247A5.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JHK2247A5.Models
{
    public class ActorAddViewModel
    {
        public ActorAddViewModel()
        {
           BirthDate = DateTime.Now.AddYears(-25);
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        [Display(Name = "Alternate Name")]
        public string AlternateName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Height (m)")]
        public double Height { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }
    }

    public class ActorBaseViewModel : ActorAddViewModel
    {
        [Required]
        [StringLength(250)]
        public string Executive { get; set; }
    }

    public class ActorWithShowInfoViewModel : ActorBaseViewModel
    {
        public ActorWithShowInfoViewModel()
        {
            Shows = new List<ShowBaseViewModel>();
            ActorMediaItems = new List<ActorMediaItemBaseViewModel>();
            Photos = new List<ActorMediaItemBaseViewModel>();
            Documents = new List<ActorMediaItemBaseViewModel>();
            AudioClips = new List<ActorMediaItemBaseViewModel>();
            VideoClips = new List<ActorMediaItemBaseViewModel>();
            ShowsCount = Shows.Count();
        }

        public IEnumerable<ShowBaseViewModel> Shows { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> ActorMediaItems { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> Photos { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> Documents { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> AudioClips { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> VideoClips { get; set; }

        [Display(Name = "Appeared In")]
        public int ShowsCount { get; set; }
    }
        
}