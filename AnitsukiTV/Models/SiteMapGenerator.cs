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

                // URL'leri ekleyin
                AddNode(xmlDoc, urlSet, "Anasayfa", "https://anitsuki.com/", "daily", "1.0");
                AddNode(xmlDoc, urlSet, "Animeler", "https://anitsuki.com/animeler", "daily", "0.9");
                AddNode(xmlDoc, urlSet, "Bağış", "https://anitsuki.com/bagis", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Giriş", "https://anitsuki.com/giris", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Kayıt", "https://anitsuki.com/kayit", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Hakkımızda", "https://anitsuki.com/hakkimizda", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Watch Together", "https://anitsuki.com/watch-together", "daily", "0.5");
                AddNode(xmlDoc, urlSet, "Gizlilik Politikası", "https://anitsuki.com/gizlilik-politikasi", "daily", "0.5");
                

                // Sitemap dosyasını kaydedin
                xmlDoc.Save(HttpContext.Current.Server.MapPath("~/sitemap.xml"));
            }
            catch (Exception ex)
            {
                // Hata mesajını loglama
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