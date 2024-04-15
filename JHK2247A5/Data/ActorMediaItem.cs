using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JHK2247A5.Data
{
    public class ActorMediaItem
    {
        #region Columns
        public int Id { get; set; }

        public byte[] Content { get; set; }

        [StringLength(200)]
        public string ContentType { get; set; }

        [Required, StringLength(100)]
        public string Caption { get; set; }

        #endregion

        #region Navigation Properties

        [Required]
        public Actor Actor { get; set; }

        #endregion
    }
}