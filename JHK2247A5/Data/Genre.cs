using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JHK2247A5.Data
{
    public class Genre
    {
        #region Columns

        public int Id { get; set; } 

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        #endregion
    }
}