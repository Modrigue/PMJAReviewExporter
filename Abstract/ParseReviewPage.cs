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

namespace PMJAReviewExporter
{
    public abstract class ParseReviewPage
    {
        public abstract string ReviewText { get; }
    }
}
