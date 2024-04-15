using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JHK2247A5.Data
{
    public class Actor
    {
        #region Constructor
        public Actor() 
        {
            BirthDate = DateTime.Now.AddYears(-25);
            Shows = new HashSet<Show>();
            ActorMediaItems = new HashSet<ActorMediaItem>();
        }

        #endregion

        #region Columns
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string AlternateName { get; set; }

        public DateTime? BirthDate { get; set; }

        public double? Height { get; set; }

        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(250)]
        public string Executive { get; set; }

        public string Biography { get; set; }   

        #endregion

        #region Entity Collections

        public ICollection<Show> Shows { get; set; }

        public ICollection<ActorMediaItem> ActorMediaItems { get; set; }

        #endregion
    }
}