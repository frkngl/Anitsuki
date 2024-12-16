using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml;

namespace AnitsukiTV.Models
{
    public class SiteMapGenerator
    {
        public static void GenerateSiteMap()
        {
            try
            {
                var xmlDoc = new XmlDocument();
                var urlSet = xmlDoc.CreateElement("urlset");
                urlSet.SetAttribute("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                xmlDoc.AppendChild(urlSet);

                AddNode(xmlDoc, urlSet, "Anasayfa", "https://www.anitsuki.com/", "daily", "1.0");
                AddNode(xmlDoc, urlSet, "Animeler", "https://www.anitsuki.com/animeler", "daily", "0.8");
                AddNode(xmlDoc, urlSet, "Bağış", "https://www.anitsuki.com/bagis", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Giriş", "https://www.anitsuki.com/giris", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Kayıt", "https://www.anitsuki.com/kayit", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Hakkımızda", "https://www.anitsuki.com/hakkimizda", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Watch Together", "https://www.anitsuki.com/watch-together", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Gizlilik Politikası", "https://www.anitsuki.com/gizlilik-politikasi", "daily", "0.5");

                using (var db = new AnitsukiTVEntities())
                {
                    var animes = db.TBLANIME.Where(x=>x.STATUS == true).ToList();

                    foreach (var anime in animes)
                    {
                        var animeNameFormatted = anime.ANIMENAME.ToLower()
                            .Replace("ı", "i").Replace("ç", "c").Replace("ö", "o")
                            .Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s")
                            .Replace(" ", "-").Replace("?", "").Replace("!", "")
                            .Replace(">", "").Replace("<", "").Replace("&", "")
                            .Replace("%", "").Replace("$", "").Replace("#", "")
                            .Replace("@", "").Replace(":", "").Replace(";", "")
                            .Replace("/", "").Replace("\\", "").Replace(".", "")
                            .Replace(",", "");

                        var seasons = db.TBLSEASON.Where(s => s.ANIMEID == anime.ID && s.STATUS == true).ToList();
                        foreach (var season in seasons)
                        {
                            AddNode(xmlDoc, urlSet, anime.ANIMENAME,
                                $"https://www.anitsuki.com/anime/{anime.ID}/{animeNameFormatted}-{season.SEASONNUMBER}-sezon-izle", "weekly", "0.8");

                            var episodes = db.TBLEPISODE.Where(e => e.TBLSEASON.SEASONNUMBER == season.SEASONNUMBER && e.TBLSEASON.ANIMEID == anime.ID).ToList();
                            foreach (var episode in episodes)
                            {
                                var episodeUrl = $"https://www.anitsuki.com/{episode.ID}/{animeNameFormatted}-{season.SEASONNUMBER}-sezon-{episode.EPINUMBER}-bolum-izle";
                                AddNode(xmlDoc, urlSet, $"{anime.ANIMENAME} - Bölüm {episode.EPINUMBER}",
                                    episodeUrl, "weekly", "0.9");
                            }
                        }
                    }


                    var categories = db.TBLCATEGORY.Where(x=>x.STATUS == true).ToList();
                    foreach (var category in categories)
                    {
                        var categoryNameFormatted = category.CATEGORYNAME.ToLower()
                            .Replace("ı", "i").Replace("ç", "c").Replace("ö", "o")
                            .Replace("ü", "u").Replace("ğ", "g").Replace("ş", "s")
                            .Replace(" ", "-").Replace("?", "").Replace("!", "")
                            .Replace(">", "").Replace("<", "").Replace("&", "")
                            .Replace("%", "").Replace("$", "").Replace("#", "")
                            .Replace("@", "").Replace(":", "").Replace(";", "")
                            .Replace("/", "").Replace("\\", "").Replace(".", "")
                            .Replace(",", "");

                        var categoryUrl = $"https://www.anitsuki.com/animeler/{category.ID}/{categoryNameFormatted}-izle";
                        AddNode(xmlDoc, urlSet, category.CATEGORYNAME, categoryUrl, "weekly", "0.7");
                    }
                }

                xmlDoc.Save(HttpContext.Current.Server.MapPath("~/sitemap.xml"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        private static void AddNode(XmlDocument xmlDoc, XmlElement urlSet, string nodeName, string url, string changeFreq = "daily", string priority = "0.5")
        {
            var node = xmlDoc.CreateElement("url");
            urlSet.AppendChild(node);

            var loc = xmlDoc.CreateElement("loc");
            loc.InnerText = url;
            node.AppendChild(loc);

            var changefreq = xmlDoc.CreateElement("changefreq");
            changefreq.InnerText = changeFreq;
            node.AppendChild(changefreq);

            var priorityNode = xmlDoc.CreateElement("priority");
            priorityNode.InnerText = priority;
            node.AppendChild(priorityNode);

            var lastmod = xmlDoc.CreateElement("lastmod");
            lastmod.InnerText = DateTime.UtcNow.ToString("yyyy-MM-dd");
            node.AppendChild(lastmod);

            // Debugging: Priority değerini konsola yazdır
            System.Diagnostics.Debug.WriteLine($"Added URL: {url}, Priority: {priority}");
        }
    }
}