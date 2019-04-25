using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelSettings
    {
        public PiczelBasic Basic { get; set; }
        public PiczelRecording Recording { get; set; }
        public PiczelPrivate Private { get; set; }
        public PiczelEmail Emails { get; set; }
    }
}
