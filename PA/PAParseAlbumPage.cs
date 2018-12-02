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
    public class PAParseAlbumPage : ParseAlbumPage
    {
        string year_;

        // parse objects
        HtmlAgilityPack.HtmlDocument htmlDoc_;

        public override String Year
        {
            get { return year_; }
        }

        public PAParseAlbumPage(string sourceHTML)
        {
            // default
            year_ = "";

            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute subgenres
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            year_ = computeAlbumYear();
        }


        private string computeAlbumYear()
        {
            HtmlNode node1 = htmlDoc_.DocumentNode.Descendants("body").FirstOrDefault();
            HtmlNode node2 = Tools.NodeWithAttributeAndValue(node1, "div", "align", "center");
            HtmlNode node3 = Tools.NodeWithAttributeAndValue(node2, "div", "id", "main");

            // get album node
            HtmlNode node4 = null;
            foreach (HtmlNode node in node3.Descendants("div"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("background-color"))
                    {
                        node4 = node; // ok, found
                        break;
                    }
                }
            }
            if (node4 == null) // incoherent
                return "";

            HtmlNode node5 = null;
            foreach (HtmlNode node in node4.Descendants("div"))
            {
                if (node.Attributes.Contains("style"))
                {
                    if (node.Attributes["style"].Value.StartsWith("background-color"))
                    {
                        node5 = node; // ok, found
                        break;
                    }
                }
            }
            if (node5 == null) // incoherent
                return "";

            HtmlNode node6 = Tools.NodeWithAttributeAndValue(node5, "table", "width", "100%");
            HtmlNode node7 = node6.Descendants("tr").FirstOrDefault();
            List<HtmlNode> nodeList = Tools.NodeListWithAttributeAndValue(node7, "td", "valign", "top");

            foreach (HtmlNode node in nodeList)
            {
                HtmlNode nodeYear = node6.Descendants("strong").FirstOrDefault();
                if (nodeYear == null) continue;

                string yearText = nodeYear.InnerHtml;
                string releasedText = "released in ";

                if (yearText.Contains(releasedText))
                {
                    int indexReleased = yearText.IndexOf(releasedText);
                    yearText = yearText.Substring(indexReleased + releasedText.Length);

                    if (Tools.isStringNumerical(yearText))
                        return yearText; // ok, found

                    continue;
                }
            }

            return "";
        }
    }
}
