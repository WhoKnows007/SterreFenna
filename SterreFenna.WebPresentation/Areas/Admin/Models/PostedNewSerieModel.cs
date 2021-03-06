﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SterreFenna.WebPresentation.Areas.Admin.Models
{
    public class PostedNewSerieModel
    {
        public int SerieId { get; set; }
        public string name { get; set; }
        public string publicationDate { get; set; }
        public string filenameOrder { get; set; }
        public IEnumerable<HttpPostedFileBase> files { get; set; }
        public string newProjectName { get; set; }
        public int? projectId { get; set; }
    }
}