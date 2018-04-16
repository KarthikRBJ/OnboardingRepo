using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Custom_Pages.Models
{
    public class Theme
    {
        public int artifactId { get; set; }
        public string Name { get; set; }
        public bool value { get; set; }
        public string  textValue { get; set; }
        public Theme()
        {
            Name = "Theme UI (light/dark)";
        }
    }
}