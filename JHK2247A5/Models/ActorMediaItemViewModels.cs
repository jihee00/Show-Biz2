using JHK2247A5.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JHK2247A5.Models
{
    public class ActorMediaItemAddFormViewModel
    {
        public int ActorId { get; set; }

        public string ActorName { get; set; }

        [Required, StringLength(100)]
        public string Caption { get; set; }

        [Required]
        [Display(Name = "Attachment")]
        [DataType(DataType.Upload)]
        public string ContentUpload { get; set; }
    }

    public class ActorMediaItemAddViewModel
    {
        public int ActorId { get; set; }

        [Required, StringLength(100)]
        public string Caption { get; set; }

        [Required]
        public HttpPostedFileBase ContentUpload { get; set; }
    }

    public class ActorMediaItemBaseViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Caption { get; set; }

        [StringLength(200)]
        public string ContentType { get; set; }
    }

    public class ActorMediaItemWithContentViewModel : ActorMediaItemBaseViewModel
    {
        public byte[] Content { get; set; }
    }
}