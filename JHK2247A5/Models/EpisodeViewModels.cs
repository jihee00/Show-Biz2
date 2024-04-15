using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JHK2247A5.Models
{
    public class EpisodeAddViewModel
    {
        public EpisodeAddViewModel()
        {
            AirDate = DateTime.Now;
            SeasonNumber = 1;
            EpisodeNumber = 1;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Range(1, Int32.MaxValue)]
        public int ShowId { get; set; }

        public ShowBaseViewModel Show { get; set; }

        [Range(1, Int32.MaxValue)]
        [Display(Name = "Season")]
        public int SeasonNumber { get; set; }

        [Range(1, Int32.MaxValue)]
        [Display(Name = "Episode")]
        public int EpisodeNumber { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Aired")]
        public DateTime AirDate { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        [Required]
        public HttpPostedFileBase VideoUpload { get; set; }
    }

    public class EpisodeAddFormViewModel
    {
        public EpisodeAddFormViewModel()
        {
            AirDate = DateTime.Now;
            SeasonNumber = 1;
            EpisodeNumber = 1;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [Range(1, Int32.MaxValue)]
        public int ShowId { get; set; }

        public ShowBaseViewModel Show { get; set; }

        [Range(1, Int32.MaxValue)]
        [Display(Name = "Season")]
        public int SeasonNumber { get; set; }

        [Range(1, Int32.MaxValue)]
        [Display(Name = "Episode")]
        public int EpisodeNumber { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Aired")]
        public DateTime AirDate { get; set; }

        [Required]
        [StringLength(250)]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        [Display(Name = "Show Name")]
        public string ShowName { get; set; }

        [Display(Name = "Genre")]
        public SelectList GenreList { get; set; }

        [Required]
        [Display(Name = "Video Attachment")]
        [DataType(DataType.Upload)]
        public string VideoUpload { get; set; }
    }

    public class EpisodeBaseViewModel : EpisodeAddViewModel
    {
        [Required]
        [StringLength(250)]
        public string Clerk { get; set; }
    }

    public class EpisodeWithShowNameViewModel : EpisodeVideoViewModel
    {
    }

    public class EpisodeVideoViewModel : EpisodeBaseViewModel
    { 
        public string VideoContentType { get; set; }

        public byte[] Video { get; set; }
    }
}