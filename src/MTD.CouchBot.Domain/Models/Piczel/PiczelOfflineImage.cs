using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelOfflineImage
    {
        public OfflineImage offline_image { get; set; }
    }

    public class OfflineImage
    {
        public string Url { get; set; }
    }
}
