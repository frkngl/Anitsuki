using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnitsukiTV.Models
{
    public class VideoViewModel
    {
        public List<TBLEPISODE> Episode { get; set; }
        public List<TBLNOTIFICATIONS> Notification { get; set; }
        public List<TBLUSER> Users { get; set; }
        public string AnimeTitle { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public List<TBLEPISODE> OncekiBolum { get; set; }
        public List<TBLEPISODE> SonrakiBolum { get; set; }
        public List<TBLANIME> Anime { get; set; }
    }
}