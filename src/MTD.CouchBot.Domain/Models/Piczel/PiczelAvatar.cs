using System;
using System.Collections.Generic;
using System.Text;

namespace MTD.CouchBot.Domain.Models.Piczel
{
    public class PiczelAvatar
    {
        public Avatar Avatar { get; set; }
    }

    public class Avatar
    {
        public string Url { get; set; }
    }
}
