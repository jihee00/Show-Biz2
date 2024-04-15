using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JHK2247A5.Data
{
    public class Episode
    {
        #region Constructor
        public Episode()
        {
            AirDate = DateTime.Now;
        }

        #endregion

        #region Columns

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        public int SeasonNumber { get; set; }

        public int EpisodeNumber { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public DateTime AirDate { get; set; }

        [Required]
        [StringLength(250)]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(250)]
        public string Clerk { get; set; }

        public string Premise { get; set; }

        [StringLength(200)]
        public string VideoContentType { get; set; }
        public byte[] Video { get; set; }

        #endregion

        #region Navigation Properties

        [Required]
        public Show Show { get; set; }

        #endregion
    }
}