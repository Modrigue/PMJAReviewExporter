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
    public abstract class ParseAlbumPage
    {
        public abstract string Year { get; }

    }
}
