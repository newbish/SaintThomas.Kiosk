using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SaintThomas.Kiosk.Models
{
    public class Image
    {
        public string Id { get; set; }
        [Display(Name="Body Text")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        [Display(Name="Video")]
        [DataType(DataType.Url)]
        public string Video { get; set; }
        [Display(Name="Active")]
        public bool Active { get; set; }
        [Display(Name="Position")]
        public int Position { get; set; }
        public int PrimaryKey
        {
            get
            {
                if (Id != null)
                    return int.Parse(Id.Substring(Id.LastIndexOf("/") + 1));
                return 0;
            }
        }
        public string VideoExt
        {
            get
            {
                if (Video.IndexOf("youtube", StringComparison.InvariantCultureIgnoreCase) > 0)
                    return string.Format("{0}&autoplay=1", Video);
                return Video;
            }
        }
    }
    public class ImageCreateEditModel : Image
    {
        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public HttpPostedFileBase ImageContent { get; set; }
    }
}