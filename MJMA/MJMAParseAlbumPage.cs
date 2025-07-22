using HtmlAgilityPack;
using System;
using System.Linq;


namespace PMJAReviewExporter
{
    public class MJMAParseAlbumPage : ParseAlbumPage
    {
        readonly string year_;

        // parse objects
        readonly HtmlAgilityPack.HtmlDocument htmlDoc_;
        HtmlNode nodeMid_;

        public override String Year
        {
            get { return year_; }
        }

        public MJMAParseAlbumPage(string sourceHTML)
        {
            // default
            year_ = "";

            if (String.IsNullOrEmpty(sourceHTML))
                return;

            // create html doc and compute nodes
            htmlDoc_ = new HtmlAgilityPack.HtmlDocument();
            htmlDoc_.LoadHtml(sourceHTML);
            computeNodes();

            // get album year
            year_ = computeAlbumYear();
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

        private string computeAlbumYear()
        {
            string year = "";

            HtmlNode nodeDesc = Tools.NodeWithAttributeAndValue(nodeMid_, "div", "id", "albumInfosType");
            if (nodeDesc == null) return "";

            string desc = Tools.CleanString(nodeDesc.InnerText);
            string[] descList = desc.Split('·'); // type, year
            if (descList.Count() >= 2)
            {
                string yearText = Tools.CleanString(descList[1]);
                if (Tools.isStringNumerical(yearText))
                {
                    // ok, found
                    year = yearText;
                }
            }

            return year;
        }
    }
}
