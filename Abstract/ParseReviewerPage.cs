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
    public abstract class ParseReviewerPage
    {
        public abstract string NameReviewer { get; }
        public abstract int NbReviewsRatings { get; }
        public abstract String AvatarURL { get; }
        public abstract List<string> FavoriteBands { get; }

        public abstract List<string> ReviewBands { get; }
        public abstract List<string> ReviewAlbums { get; }
        public abstract List<string> AlbumsURLs { get; }
        public abstract List<string> ReviewURLs { get; }
        public abstract List<string> Ratings { get; }
    }
}
