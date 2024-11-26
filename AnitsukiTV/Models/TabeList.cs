using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnitsukiTV.Models
{
    public class TabeList
    {
        public List<TBLCATEGORY> Category { get; set; }
        public List<TBLANIME> Anime { get; set; }
        public List<TBLSEASON> Season { get; set; }
        public List<TBLEPISODE> Episode { get; set; }
        public List<TBLEPISODE> OncekiBolum { get; set; }
        public List<TBLEPISODE> SonrakiBolum { get; set; }
        public List<TBLDONATE> Donate { get; set; }
        public List<TBLUSER> Users { get; set; } // For a list of users
        public TBLUSER User { get; set; } // For a single user
        public List<TBLEPISODELIKE> EpisodeLike { get; set; }
        public List<TBLFAVORITES> Favorites { get; set; }
        public List<TBLWATCHLATER> WatchLater { get; set; }
        public List<TBLANIMECOMMENTLIKE> AnimeCommentLike { get; set; }
        public List<TBLUSER> Followers { get; set; }
        public List<TBLUSER> Following { get; set; }
        public List<TBLNOTIFICATIONS> Notifications { get; set; }
        public List<TBLANIME> FriendsAnime { get; set; }
        public VideoViewModel VideoInfo { get; set; }
    }

}