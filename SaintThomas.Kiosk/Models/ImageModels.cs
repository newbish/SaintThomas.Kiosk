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
        public string Video { get; set; }
        [Display(Name="Active")]
        public bool Active { get; set; }
        [Display(Name="Position")]
        public int Position { get; set; }
        public int PrimaryKey
        {
            get { return int.Parse(Id.Substring(Id.LastIndexOf("/") + 1)); }
        }
    }
    public class ImageCreateModel
    {
        [Display(Name = "Body Text")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        [DataType(DataType.Upload)]
        [Display(Name = "Image")]
        public HttpPostedFileBase ImageContent { get; set; }
        [DataType(DataType.Url)]
        [Display(Name = "Video")]
        public string Video { get; set; }
        [Display(Name = "Active")]
        public bool Active { get; set; }
        [Display(Name = "Position")]
        public int Position { get; set; }
    }
}