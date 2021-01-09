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
    public class Tools
    {
        public static string DownloadPath = "";

        public static void Initialize()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3
                | SecurityProtocolType.Tls
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;
        }

        public static string GetWebPageSourceHTML(string url)
        {
            string sourceHTML = "";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        //readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    }

                    //string data = readStream.ReadToEnd();
                    sourceHTML = readStream.ReadToEnd();

                    response.Close();
                    readStream.Close();
                }
            }
            catch { }

            return sourceHTML;
        }

        // search for first of default node with format "<type ...attribute = value..."
        public static HtmlNode NodeWithAttributeAndValue(HtmlNode node, string type, string attribute, string value)
        {
            if (node == null)
                return null;

            HtmlNode resNode = node.Descendants(type)
                .Where(n => n.Attributes.Contains(attribute) && n.Attributes[attribute].Value == value)
                .FirstOrDefault();

            return resNode;
        }

        // return list of nodes with format "<type ...attribute = value..."
        public static List<HtmlNode> NodeListWithAttributeAndValue(HtmlNode node, string type, string attribute, string value)
        {
            List<HtmlNode> nodes = new List<HtmlNode>();

            if (node == null)
                return nodes;

            IEnumerable<HtmlNode> resNodes = node.Descendants(type)
                .Where(n => n.Attributes.Contains(attribute) && n.Attributes[attribute].Value == value);

            nodes = resNodes.ToList();
            return nodes;
        }

        // returns true iff node has format "<type ...attribute = value..."
        public static bool NodeHasAttributeAndValue(HtmlNode node, string type, string attribute, string value)
        {
            bool res = false;

            if (node == null)
                return false;

            if (node.Name == type && node.Attributes.Contains(attribute) && node.Attributes[attribute].Value == value)
                res = true;

            return res;
        }

        #region String functions

        public static string ToTitleCase(string text)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(text.ToLower());
        }

        public static string CleanString(string text)
        {
            string res = text.Trim();
            res = res.Replace("\t", "");
            res = HtmlEntity.DeEntitize(res);

            return res;
        }

        public static bool isStringNumerical(string text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            long myInt;
            bool isNumerical = long.TryParse(text, out myInt);

            return isNumerical;
        }

        public static string TrimTime(string time)
        {
            if (time.StartsWith("0"))
                if (time.Length == 5 || time.Length == 8)
                {
                    time = time.Substring(1);
                }

            return time;
        }

        public static string RemoveWindowsForbiddenCharacters(string text)
        {
            string newText = text;

            newText = newText.Replace("\\", "");
            newText = newText.Replace("/", "");
            newText = newText.Replace(":", "");
            newText = newText.Replace("*", "");
            newText = newText.Replace("?", "");
            newText = newText.Replace("\"", "");
            newText = newText.Replace("<", "");
            newText = newText.Replace(">", "");
            newText = newText.Replace("|", "");

            return newText;
        }


        #endregion

        public static string DownloadDirectory()
        {
            string dirUser = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string dirDownload = Path.Combine(dirUser, "Downloads", "PMJA_REVIEWS");

            return dirDownload;
        }

        public static string DownloadReviewerDirectory(string reviewer)
        {
            string reviewerDir = RemoveWindowsForbiddenCharacters(reviewer);
            return Path.Combine(Tools.DownloadDirectory(), reviewerDir, DownloadPath);
        }

        public static string ReviewerAllReviewsPath(string reviewer)
        {
            string dirReviewer = Tools.DownloadReviewerDirectory(reviewer);

            string allReviewsFilename = "_ALL_REVIEWS.txt";
            string allReviewsPath = Path.Combine(dirReviewer, allReviewsFilename);

            return allReviewsPath;
        }

        public static void DownloadAvatarImage(string reviewer, string url)
        {
            if (String.IsNullOrEmpty(url))
                return;

            // create reviewer directory if not existing
            string dirReviewer = Tools.DownloadReviewerDirectory(reviewer);
            if (!Directory.Exists(dirReviewer))
                Directory.CreateDirectory(dirReviewer);
            
            string extension = Path.GetExtension(url);
            string avatarFilename = "_AVATAR" + extension;
            string avatarPath = Path.Combine(dirReviewer, avatarFilename);

            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(url, avatarPath);
            }
            catch (Exception /*e*/)
            {
                //
            }
        }

        public static void DownloadFavoriteBands(string reviewer, List<string> favoriteBands)
        {
            if (favoriteBands == null || favoriteBands.Count == 0)
                return;

            // create reviewer directory if not existing
            string dirReviewer = Tools.DownloadReviewerDirectory(reviewer);
            if (!Directory.Exists(dirReviewer))
                Directory.CreateDirectory(dirReviewer);
            string favBandsFilename = "_FAVORITE_BANDS.txt";
            string favBandsPath = Path.Combine(dirReviewer, favBandsFilename);

            // build text
            string text = "";
            foreach (string band in favoriteBands)
                text += band + "\r\n";

            // write text into file
            File.WriteAllText(favBandsPath, text);
        }
    }
}
