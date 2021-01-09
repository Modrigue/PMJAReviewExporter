using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;


namespace PMJAReviewExporter
{
    public class MJMAParseReviewerPage : ParseReviewerPage
    {
        string nameReviewer_;
        string avatarURL_;
        List<string> favoriteBands_;

        int nbReviewsRatings_;
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

        public override String AvatarURL
        {
            get { return avatarURL_; }
        }

        public override List<string> FavoriteBands
        {
            get { return favoriteBands_; }
        }

        public override int NbReviewsRatings
        {
            get { return nbReviewsRatings_; }
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

        public MJMAParseReviewerPage(string sourceHTML)
        {
            // default
            
            nameReviewer_ = "";
            avatarURL_ = "";
            favoriteBands_ = null;

            nbReviewsRatings_ = 0;
            reviewBands_ = null;
            reviewAlbums_ = null;
            reviewURLs_ = null;
            albumsURLs_ = null;
            ratings_ = null;
            
            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and get reviewer data
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            computeReviewerData();
        }

        private void computeReviewerData()
        {
            string nameReviewer = "";
            string avatarURL = "";
            List<string> favoriteBands = new List<string>();

            int nbReviewsRatings = 0;
            List<string> reviewBands = new List<string>();
            List<string> reviewAlbums = new List<string>();
            List<string> reviewURLs = new List<string>();
            List<string> albumsURLs = new List<string>();
            List<string> ratings = new List<string>();

            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "mainSite");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "class", "colmask holygrail");
            HtmlNode node4 = Tools.NodeWithAttributeAndValue(node3, "div", "class", "colmid");
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "div", "class", "colleft");
            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "div", "class", "col1wrap");
            HtmlNode nodeMid = Tools.NodeWithAttributeAndValue(node6, "div", "class", "col1");

            HtmlNode nodeProfilePage = Tools.NodeWithAttributeAndValue(node6, "div", "id", "profilePage");
            HtmlNode nodeProfileContainer = Tools.NodeWithAttributeAndValue(node6, "div", "id", "profileContainer");

            // get reviewer name node
            HtmlNode nodeReviewerName = nodeProfilePage.Descendants("h1").FirstOrDefault();
            nameReviewer = Tools.CleanString(nodeReviewerName.InnerText);

            // get reviewer avatar URL
            HtmlNode nodeProfileAvatar = Tools.NodeWithAttributeAndValue(nodeProfileContainer, "div", "id", "profileAvatar");
            foreach (HtmlNode node in nodeProfileAvatar.Descendants("img"))
            {
                if (node.Attributes.Contains("src"))
                {
                    avatarURL = node.Attributes["src"].Value;
                    avatarURL = WebUtility.HtmlDecode(avatarURL);
                    break; // ok, found
                }
            }

            // get favorite bands
            HtmlNode nodeFavoriteArtists = Tools.NodeWithAttributeAndValue(nodeProfilePage, "div", "id", "profileFavArtists");
            foreach (HtmlNode node in nodeFavoriteArtists.Descendants("a"))
            {
                if (node.Attributes.Count == 1 && node.Attributes.Contains("href") && node.Attributes["href"].Value.StartsWith("/artist/"))
                {
                    string band = node.InnerText;
                    band = Tools.CleanString(band);
                    band = Tools.ToTitleCase(band);
                    favoriteBands.Add(band);
                }
            }

            // get number of reviews + ratings
            HtmlNode nodeReviews = Tools.NodeWithAttributeAndValue(nodeProfilePage, "div", "id", "profilePublishedReviewsContainer");
            HtmlNode nodeNbReviewsRatings = Tools.NodeWithAttributeAndValue(nodeProfilePage, "span", "id", "ctl00_MainContentPlaceHolder_NbReviewsLabel");
            string nbReviewsCandidate = nodeNbReviewsRatings.InnerText;
            nbReviewsCandidate = nbReviewsCandidate.Replace(" reviews/ratings", "");
            if (Tools.isStringNumerical(nbReviewsCandidate))
                int.TryParse(nbReviewsCandidate, out nbReviewsRatings);

            // get reviews data
            List<HtmlNode> nodeProfileReviews = Tools.NodeListWithAttributeAndValue(nodeReviews, "div", "class", "profileReview");
            foreach (HtmlNode node in nodeProfileReviews)
            {
                // get rating
                HtmlNode nodeGenRating = Tools.NodeWithAttributeAndValue(node, "script", "language", "javascript");
                string genRatingText = Tools.CleanString(nodeGenRating.InnerText);
                string ratingText = genRatingText.Split(',').LastOrDefault();
                ratingText = ratingText.Replace(");", "");
                ratingText = Tools.CleanString(ratingText);
                //if (Tools.isStringNumerical(ratingText))
                ratings.Add(ratingText);

                // get band
                HtmlNode nodeBand = Tools.NodeWithAttributeAndValue(node, "a", "class", "profileReviewArtistLink");
                string band = nodeBand.InnerText;
                band = Tools.CleanString(band);
                band = Tools.ToTitleCase(band);
                reviewBands.Add(band);

                // // get album name + URL and review URL if existing
                bool hasReview = false;
                foreach (HtmlNode nodeA in node.Descendants("a"))
                {
                    // get album name + URL
                    if (nodeA.Attributes.Contains("href") && nodeA.Attributes["href"].Value.StartsWith("/album/"))
                    {
                        // get album name
                        string album = nodeA.InnerText;
                        album = Tools.CleanString(album);
                        album = Tools.ToTitleCase(album);
                        reviewAlbums.Add(album);

                        // get album URL and year (not used)
                        string albumURL = nodeA.Attributes["href"].Value;
                        albumsURLs.Add(albumURL);
                        //string year = getAlbumYear(albumURL);
                    }

                    // get review URL (if existing)
                    if (nodeA.Name == "a" && nodeA.Attributes.Contains("href") && nodeA.InnerText == "review permalink")
                    {
                        string url = nodeA.Attributes["href"].Value;
                        reviewURLs.Add(url);
                        hasReview = true;
                    }
                }

                // if no review URL found, rating only
                if (!hasReview)
                    reviewURLs.Add("");
            }

            nameReviewer_ = nameReviewer;
            avatarURL_ = avatarURL;
            favoriteBands_ = favoriteBands;

            nbReviewsRatings_ = nbReviewsRatings;
            reviewBands_ = reviewBands;
            reviewAlbums_ = reviewAlbums;
            reviewURLs_ = reviewURLs;
            albumsURLs_ = albumsURLs;
            ratings_ = ratings;
        }
    }
}
