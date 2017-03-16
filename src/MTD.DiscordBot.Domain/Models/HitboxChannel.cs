using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTD.DiscordBot.Domain.Models
{
    public class HitboxChannel
    {
        public Request request { get; set; }
        public string media_type { get; set; }
        public List<Livestream> livestream { get; set; }
        public bool success { get; set; }
        public bool error { get; set; }
        public string error_msg { get; set; }

        public class Request
        {
            public string @this { get; set; }
        }

        public class Channel
        {
            public string followers { get; set; }
            public string videos { get; set; }
            public string recordings { get; set; }
            public string teams { get; set; }
            public string user_id { get; set; }
            public string user_name { get; set; }
            public string user_status { get; set; }
            public string user_logo { get; set; }
            public string user_cover { get; set; }
            public string user_logo_small { get; set; }
            public string user_partner { get; set; }
            public string partner_type { get; set; }
            public string user_beta_profile { get; set; }
            public string media_is_live { get; set; }
            public string media_live_since { get; set; }
            public string user_media_id { get; set; }
            public string twitter_account { get; set; }
            public string twitter_enabled { get; set; }
            public string livestream_count { get; set; }
            public string channel_link { get; set; }
        }

        public class Livestream
        {
            public string media_user_name { get; set; }
            public string media_id { get; set; }
            public string media_file { get; set; }
            public string media_user_id { get; set; }
            public string media_profiles { get; set; }
            public string media_type_id { get; set; }
            public string media_is_live { get; set; }
            public string media_live_delay { get; set; }
            public string media_date_added { get; set; }
            public string media_live_since { get; set; }
            public string media_transcoding { get; set; }
            public string media_chat_enabled { get; set; }
            public List<string> media_countries { get; set; }
            public object media_hosted_id { get; set; }
            public object media_mature { get; set; }
            public object media_hidden { get; set; }
            public object media_offline_id { get; set; }
            public object user_banned { get; set; }
            public string media_name { get; set; }
            public string media_display_name { get; set; }
            public string media_status { get; set; }
            public string media_title { get; set; }
            public string media_description { get; set; }
            public string media_description_md { get; set; }
            public string media_tags { get; set; }
            public string media_duration { get; set; }
            public string media_bg_image { get; set; }
            public string media_views { get; set; }
            public string media_views_daily { get; set; }
            public string media_views_weekly { get; set; }
            public string media_views_monthly { get; set; }
            public object media_chat_channel { get; set; }
            public string category_id { get; set; }
            public string category_name { get; set; }
            public object category_name_short { get; set; }
            public string category_seo_key { get; set; }
            public string category_viewers { get; set; }
            public string category_media_count { get; set; }
            public object category_channels { get; set; }
            public string category_logo_small { get; set; }
            public string category_logo_large { get; set; }
            public string category_updated { get; set; }
            public string team_name { get; set; }
            public string media_start_in_sec { get; set; }
            public bool media_is_spherical { get; set; }
            public bool following { get; set; }
            public bool subscribed { get; set; }
            public string media_duration_format { get; set; }
            public string media_thumbnail { get; set; }
            public string media_thumbnail_large { get; set; }
            public Channel channel { get; set; }
        }        
    }
}
