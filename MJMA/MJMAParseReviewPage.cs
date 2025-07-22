using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;

namespace PMJAReviewExporter
{
    public class MJMAParseReviewPage : ParseReviewPage
    {
        // review info
        readonly string reviewText_;
        readonly string year_;

        public override string ReviewText
        {
            get { return reviewText_; }
        }

        public string Year
        {
            get { return year_; }
        }

        // parse objects
        readonly HtmlAgilityPack.HtmlDocument htmlDoc_;
        HtmlNode nodeMid_;

        public MJMAParseReviewPage(string sourceHTML)
        {
            // default
            reviewText_ = "";
            year_ = "";

            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            computeNodes();

            // get review text
            reviewText_ = getReviewText();

            // get year
            year_ = getAlbumYear();
        }

        private void computeNodes()
        {
            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "id", "mainSite");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "class", "colmask holygrail");
            HtmlNode node4 = Tools.NodeWithAttributeAndValue(node3, "div", "class", "colmid");
            HtmlNode node5 = Tools.NodeWithAttributeAndValue(node4, "div", "class", "colleft");
            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "div", "class", "col1wrap");
            nodeMid_ = Tools.NodeWithAttributeAndValue(node6, "div", "class", "col1");
        }

        private string getReviewText()
        {
            HtmlNode node1 = Tools.NodeWithAttributeAndValue(nodeMid_, "div", "class", "latestReviewText");
            HtmlNode nodeReview = Tools.NodeWithAttributeAndValue(node1, "span", "itemprop", "description");
            if (nodeReview == null) return"";

            // get review text from html to keep styles
            string text = nodeReview.InnerHtml;
            text = text.Trim();
            text = convertReviewHTML(text);

            return text;
        }

        private string getAlbumYear()
        {
            string year = "";
            foreach (HtmlNode node in nodeMid_.Descendants("span"))
            {
                if (node.Attributes.Contains("style") && node.Attributes["style"].Value.StartsWith("color"))
                {
                    string desc = Tools.CleanString(node.InnerText);
                    string[] descList = desc.Split('·'); // album, year, genre
                    if (descList.Count() >= 2)
                    {
                        string yearText = Tools.CleanString(descList[1]);
                        if (Tools.isStringNumerical(yearText))
                        {
                            // ok, found
                            year = yearText;
                            break;
                        }
                    }
                }
            }

            return year;
        }

        private string convertReviewHTML(string text)
        {
            // decode special characters
            text = WebUtility.HtmlDecode(text);

            // new lines
            text = text.Replace("\n", " "); // wraps
            text = text.Replace("<br>", "\r\n");
            text = text.Replace("<br/>", "\r\n");
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
