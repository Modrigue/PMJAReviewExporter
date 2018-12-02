using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Net;
using System.Globalization;

namespace PMJAReviewExporter
{
    public class PAParseReviewerPage : ParseReviewerPage
    {
        string nameReviewer_;
        int nbReviewsRatings_;
        string avatarURL_;
        List<string> reviewBands_;
        List<string> reviewAlbums_;
        List<string> reviewURLs_;
        List<string> albumsURLs_;
        List<string> ratings_;

        // parse objects
        HtmlAgilityPack.HtmlDocument htmlDoc_;

        public override String NameReviewer
        {
            get { return nameReviewer_; }
        }

        public override int NbReviewsRatings
        {
            get { return nbReviewsRatings_; }
        }

        public override String AvatarURL
        {
            get { return avatarURL_; }
        }

        // not existant
        public override List<string> FavoriteBands
        {
            get { return new List<string>(); }
        }

        public override List<string> ReviewBands
        {
            get { return reviewBands_; }
        }

        public override List<string> ReviewAlbums
        {
            get { return reviewAlbums_; }
        }

        public override List<string> AlbumsURLs
        {
            get { return albumsURLs_; }
        }

        public override List<string> ReviewURLs
        {
            get { return reviewURLs_; }
        }

        public override List<string> Ratings
        {
            get { return ratings_; }
        }

        public PAParseReviewerPage(string sourceHTML)
        {
            // default
            nbReviewsRatings_ = 0;
            nameReviewer_ = "";
            reviewBands_ = null;
            reviewAlbums_ = null;
            reviewURLs_ = null;
            albumsURLs_ = null;
            ratings_ = null;
            avatarURL_ = "";

            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and get reviews data
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            computeReviewsData();

            // get avatar URL
            avatarURL_ = getAvatarURL();
        }

        private void computeReviewsData()
        {
            int nbReviewsRatings = 0;
            string nameReviewer = "";
            List<string> reviewBands = new List<string>();
            List<string> reviewAlbums = new List<string>();
            List<string> reviewURLs = new List<string>();
            List<string> albumsURLs = new List<string>();
            List<string> ratings = new List<string>();


            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "align", "center");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "main");

            // get reviewer node
            HtmlNode nodeReviewer = null;
            foreach (HtmlNode node in node3.Descendants("div"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if(node.Attributes["style"].Value.StartsWith("background-color"))
                    {
                        nodeReviewer = node; // ok, found
                        break;
                    }
                }
            }
            if (nodeReviewer == null) // incoherent
                return;

            // get reviewer name
            foreach (HtmlNode node in nodeReviewer.Descendants("h1"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("line-height"))
                    {
                        nameReviewer = node.InnerText;
                        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                        nameReviewer = textInfo.ToTitleCase(nameReviewer.ToLower());
                        break; // ok, found
                    }
                }
            }

            // get number of reviews + ratings
            foreach (HtmlNode node in nodeReviewer.Descendants("strong"))
            {
                string text = node.InnerText;
                string ratingsText = " ratings/reviews total";

                if (text.EndsWith(ratingsText))
                {
                    int indexRatings = text.IndexOf(ratingsText);
                    string nbReviewsCandidate = text.Substring(0, indexRatings);
                    if (Tools.isStringNumerical(nbReviewsCandidate))
                    {
                        // convert to integer
                        int.TryParse(nbReviewsCandidate, out nbReviewsRatings);
                        break; // ok, found
                    }
                }
            }

            // get reviews data
            HtmlNode nodeReviews = nodeReviewer.Descendants("ul").FirstOrDefault();
            foreach (HtmlNode node in nodeReviews.Descendants())
            {
                // get rating
                if (node.Name == "img" && node.Attributes.Contains("src"))
                {
                    string ratingText = node.Attributes["src"].Value;
                    ratingText = ratingText.Replace("static-images/", "");
                    ratingText = ratingText.Replace("stars.gif", "");
                    if (Tools.isStringNumerical(ratingText))
                        ratings.Add(ratingText);
                }

                // get band and album
                if (node.Name == "strong")
                {
                    foreach (HtmlNode nodeArtistAlbum in node.Descendants("a"))
                    {
                        // get band
                        if (nodeArtistAlbum.Attributes.Contains("href") && nodeArtistAlbum.Attributes["href"].Value.StartsWith("artist"))
                        {
                            string band = nodeArtistAlbum.InnerText;
                            band = Tools.CleanString(band);
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                            band = textInfo.ToTitleCase(band.ToLower());
                            reviewBands.Add(band);
                        }

                        // get album
                        if (nodeArtistAlbum.Attributes.Contains("href") && nodeArtistAlbum.Attributes["href"].Value.StartsWith("album"))
                        {
                            string album = nodeArtistAlbum.InnerText;
                            album = Tools.CleanString(album);
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                            album = textInfo.ToTitleCase(album.ToLower());
                            reviewAlbums.Add(album);

                            // get album URL and year
                            string albumURL = nodeArtistAlbum.Attributes["href"].Value;
                            albumsURLs.Add(albumURL);
                            //string year = getAlbumYear(albumURL);
                        }
                    }
                }

                // get review (if existing)
                if (node.Name == "a" && node.Attributes.Contains("href") && node.InnerText == "review")
                {
                    string url = node.Attributes["href"].Value;
                    reviewURLs.Add(url);
                }
                else if (node.Name == "span" && node.Attributes.Contains("style") && node.InnerText == "(rating only)")
                {
                    // get rating only
                    string url = "";
                    reviewURLs.Add(url);
                }
            }

            nbReviewsRatings_ = nbReviewsRatings;
            nameReviewer_ = nameReviewer;
            reviewBands_ = reviewBands;
            reviewAlbums_ = reviewAlbums;
            reviewURLs_ = reviewURLs;
            albumsURLs_ = albumsURLs;
            ratings_ = ratings;
        }

        // get avatar url
        private string getAvatarURL()
        {
            string avatarURL = ""; // default

            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "align", "center");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "main");

            HtmlNode nodeA = null;
            foreach (HtmlNode node in node3.Descendants("div"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("background-color"))
                    {
                        nodeA = node; // ok, found
                        break;
                    }
                }
            }
            if (nodeA == null) // incoherent
                return "";

            HtmlNode nodeB = null;
            foreach (HtmlNode node in nodeA.Descendants("div"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("margin"))
                    {
                        nodeB = node; // ok, found
                        break;
                    }
                }
            }
            if (nodeB == null) // incoherent
                return "";

            foreach (HtmlNode node in nodeB.Descendants("img"))
            {
                if (node.Attributes.Contains("src"))
                {
                    avatarURL = node.Attributes["src"].Value;
                    break; // ok, found
                }
            }


            return avatarURL;
        }
    }
}
