using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;

namespace PMJAReviewExporter
{
    public class PAParseReviewPage : ParseReviewPage
    {
        // review info
        string reviewText_;

        public override string ReviewText
        {
            get { return reviewText_; }
        }

        // parse objects
        HtmlAgilityPack.HtmlDocument htmlDoc_;

        public PAParseReviewPage(string sourceHTML)
        {
            // default
            reviewText_ = "";

            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute album nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);

            // get review data
            reviewText_ = getReviewText();

            // write album infos (data + cover image)
            //writeAlbumInfos();
        }

        private string getReviewText()
        {
            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "align", "center");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "main");

            // get review node
            HtmlNode nodeReview = null;
            foreach (HtmlNode node in node3.Descendants("div"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("background-color"))
                    {
                        nodeReview = node; // ok, found
                        break;
                    }
                }
            }
            if (nodeReview == null) // incoherent
                return "";

            HtmlNode nodeReview2 = null;
            foreach (HtmlNode node in nodeReview.Descendants("table"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("margin"))
                    {
                        nodeReview2 = node;
                    }
                }
            }
            if (nodeReview2 == null) // incoherent
                return "";

            foreach (HtmlNode nodeReview3 in nodeReview2.Descendants("tr"))
            {
                foreach (HtmlNode nodeReview4 in nodeReview3.Descendants("td"))
                {
                    if (nodeReview4.Attributes.Count == 1 && nodeReview4.Attributes.Contains("valign") && nodeReview4.Attributes["valign"].Value == "top")
                    {
                        foreach (HtmlNode nodeReview5 in nodeReview4.Descendants("div"))
                        {
                            // get review text from html to keep styles
                            string text = nodeReview5.InnerHtml;

                            string textStart = "border=\"0\">";
                            int indexStart = text.IndexOf(textStart);
                            text = text.Substring(indexStart + textStart.Length);

                            string textEnd = "<div itemscope";
                            int indexEnd = text.IndexOf(textEnd);
                            text = text.Substring(0, indexEnd);

                            text = text.Trim();
                            text = convertReviewHTML(text);

                            return text;
                        }
                    }
                }
            }

            return "";
        }

        private string convertReviewHTML(string text)
        {
            // decode special characters
            text = WebUtility.HtmlDecode(text);

            // new lines
            text = text.Replace("\n", " "); // wraps
            text = text.Replace("<p>", "\r\n\r\n");

            // styles
            text = text.Replace("<strong>", "<b>");
            text = text.Replace("</strong>", "</b>");
            text = text.Replace("<em>", "<i>");
            text = text.Replace("</em>", "</i>");
            //...

            return text;
        }
    }
}
