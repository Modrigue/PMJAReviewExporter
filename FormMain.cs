using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Microsoft.WindowsAPICodePack.Taskbar;

namespace PMJAReviewExporter
{
    public partial class FormMain : Form
    {
        public static event EventHandler OnProcessReviewerName;
        public static event EventHandler OnProcessReviewParams;
        public static event EventHandler OnProcessError;

        private int nbReviewsRatingsProcessed_;
        Thread threadExport_;
        private bool isProcessing_;

        private TaskbarManager taskbarManager_;
        private float taskbarManagerValue_;

        private string site_;
        private string urlSite_;
        private string urlReviewer_;

        private delegate void changeReviewerNameLabel(string str);
        private delegate void changeProgressLabel(string str);
        private delegate void incrementProgressBar(float value);
        private delegate void incrementProgressTaskBar(float value);

        public FormMain()
        {
            InitializeComponent();

            labelVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            OnProcessReviewerName += FormMain_OnProcessReviewerName;
            OnProcessReviewParams += FormMain_OnProcessReviewParams;
            OnProcessError += FormMain_OnProcessError;

            nbReviewsRatingsProcessed_ = 0;
            isProcessing_ = false;

            taskbarManager_ = TaskbarManager.Instance;
            taskbarManagerValue_ = 0;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // check if required dlls are present
            string execDir = Environment.CurrentDirectory;
            string dllName = "HtmlAgilityPack.dll";
            string dllPath = Path.Combine(execDir, dllName);
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("Library " + dllName + " not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            // for debug purposes only

            // PA
            //textboxReviewer.Text = "28482"; // Sheavy
            //textboxReviewer.Text = "49074"; // Lakeglade12
            //textboxReviewer.Text = "11816"; // Modrigue
            //textboxReviewer.Text = "10439"; // Atavachron
            //textboxReviewer.Text = "9980";  // Mellotron Storm

            // MMA
            //textboxReviewer.Text = "modrigue";
            //textboxReviewer.Text = "silly puppy";
            //textboxReviewer.Text = "nightfly";
            //textboxReviewer.Text = "unitron";
            //textboxReviewer.Text = "666sharon666";

            comboboxSite.SelectedIndex = 0;

            // set focus to text box
            this.ActiveControl = textboxReviewer;
            textboxReviewer.Focus();       
        }

        private void buttonExport_Click(object sender, EventArgs e)
        {
            // stop process
            if (isProcessing_)
            {
                threadExport_.Abort();
                isProcessing_ = false;
                updateGUI();
                return;
            }

            // start process

            string id = textboxReviewer.Text;

            labelStatus.Visible = true;
            labelStatus.Text = "Processing reviewer page...";

            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Maximum = 10000;
            progressBar1.Value = 0;

            taskbarManager_.SetProgressState(TaskbarProgressBarState.Normal);
            taskbarManagerValue_ = 0;
            taskbarManager_.SetProgressValue(0, 10000);

            nbReviewsRatingsProcessed_ = 0;
            isProcessing_ = true;

            threadExport_ = new Thread(() => exportReviews(id));
            threadExport_.Start();

            updateGUI();

            while (isProcessing_)
                Application.DoEvents();

            updateGUI();

            progressBar1.Visible = false;
            //progressBar1.Maximum = 0;
            progressBar1.Value = 0;

            taskbarManager_.SetProgressState(TaskbarProgressBarState.NoProgress);
            taskbarManagerValue_ = 0;
            taskbarManager_.SetProgressValue(0, 10000);

            // update status label
            labelStatus.Text = nbReviewsRatingsProcessed_.ToString() + " review(s) exported";
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exportReviews(string reviewerText)
        {
            // get reviewer data
            string urlReviews = urlReviewer_.Replace("<REVIEWER>", reviewerText);
            string sourceHTMLReviewerPage = Tools.GetWebPageSourceHTML(urlReviews);

            ParseReviewerPage parsedReviewerPage;
            switch (site_)
            {
                case "Prog Archives":
                    parsedReviewerPage = new PAParseReviewerPage(sourceHTMLReviewerPage);
                    break;

                default: // MMA, JMA
                    parsedReviewerPage = new MJMAParseReviewerPage(sourceHTMLReviewerPage);
                    break;
            }

            if (sourceHTMLReviewerPage == null || String.IsNullOrEmpty(parsedReviewerPage.NameReviewer))
            {
                // send error event
                sendProcessError("Reviewer page does not exist");
                isProcessing_ = false;
                return;
            }
            string reviewer = parsedReviewerPage.NameReviewer;
            sendProcessReviewerName(reviewer);

            // download avatar image if existing
            string avatarURL = parsedReviewerPage.AvatarURL;
            if (!String.IsNullOrEmpty(avatarURL))
                Tools.DownloadAvatarImage(reviewer, avatarURL);

            // download favorite bands if existing
            List<string> favoriteBands = parsedReviewerPage.FavoriteBands;
            if (favoriteBands != null && favoriteBands.Count > 0)
                Tools.DownloadFavoriteBands(reviewer, favoriteBands);

            // delete all reviews file if already existing
            string allReviewsPath = Tools.ReviewerAllReviewsPath(reviewer);
            if (File.Exists(allReviewsPath))
                File.Delete(allReviewsPath);

            if (parsedReviewerPage.ReviewURLs == null) // secure
                return;

            // export reviews / ratings
            int indexReviewRating = 0;
            foreach (string url in parsedReviewerPage.ReviewURLs)
            {
                string band = parsedReviewerPage.ReviewBands[indexReviewRating];
                string album = parsedReviewerPage.ReviewAlbums[indexReviewRating];
                string albumURL = parsedReviewerPage.AlbumsURLs[indexReviewRating];
                string rating = parsedReviewerPage.Ratings[indexReviewRating];

                sendProcessReviewParams(indexReviewRating, parsedReviewerPage.NbReviewsRatings);

                // parse review page
                string reviewText = "(rating only)"; // default
                string urlReview = "";
                if (!String.IsNullOrEmpty(url)) // review exists
                {
                    urlReview = urlSite_ + url;
                    string sourceHTMLReviewPage = Tools.GetWebPageSourceHTML(urlReview);
                    if (String.IsNullOrEmpty(sourceHTMLReviewPage)) // incoherent
                    {
                        indexReviewRating++;
                        continue;
                    }
                    ParseReviewPage reviewPage;
                    switch (site_)
                    {
                        case "Prog Archives":
                            reviewPage = new PAParseReviewPage(sourceHTMLReviewPage);
                            break;

                        default: // MMA, JMA
                            reviewPage = new MJMAParseReviewPage(sourceHTMLReviewPage);
                            break;
                    }
                    reviewText = reviewPage.ReviewText;
                    //year = reviewPage.Year;
                }

                // parse album page
                string year = ""; // default
                if (!String.IsNullOrEmpty(albumURL)) // review exists
                {
                    string urlPage = urlSite_ + albumURL;
                    string sourceHTMLAbumPage = Tools.GetWebPageSourceHTML(urlPage);
                    if (String.IsNullOrEmpty(sourceHTMLAbumPage)) // incoherent
                    {
                        indexReviewRating++;
                        continue;
                    }
                    ParseAlbumPage albumPage;
                    switch (site_)
                    {
                        case "Prog Archives":
                            albumPage = new PAParseAlbumPage(sourceHTMLAbumPage);
                            break;

                        default: // MMA, JMA
                            albumPage = new MJMAParseAlbumPage(sourceHTMLAbumPage);
                            break;
                    }
                    
                    year = albumPage.Year;
                }

                writeReview(reviewText, band, album, year, rating, reviewer, urlReview);
                indexReviewRating++;
                nbReviewsRatingsProcessed_++;
            }

            isProcessing_ = false;
        }

        // write review in specific file + general file
        private void writeReview(string text, string band, string album, string year, string rating, string reviewer, string urlReview = "")
        {
            string dirReviewer = Tools.DownloadReviewerDirectory(reviewer);

            // create reviewer directory if not existing
            if (!Directory.Exists(dirReviewer))
                Directory.CreateDirectory(dirReviewer);

            string allReviewsPath = Tools.ReviewerAllReviewsPath(reviewer);

            // build review text

            string headerText = band.ToUpper() + " - " + album  + " (" + year + ")";
            string ratingText = rating + ((rating == "1") ? " star" : " stars");
            string reviewText = headerText + "\r\n" + ratingText + "\r\n\r\n" + text;

            if (!String.IsNullOrEmpty(urlReview))
                reviewText += "\r\n\r\n" + "Source: " + urlReview;

            // write into general file

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(allReviewsPath, true))
            {
                file.WriteLine(reviewText);
                file.WriteLine("");
                file.WriteLine("");
                file.WriteLine("");
            }

            // write into specific file

            string bandString = band.ToUpper();
            bandString = Tools.RemoveWindowsForbiddenCharacters(bandString);

            string albumString = album;
            albumString = Tools.RemoveWindowsForbiddenCharacters(albumString);

            string reviewFilename = bandString + "_" + albumString + "_" + year + ".txt"; // TODO: add year
            string reviewPath = Path.Combine(dirReviewer, reviewFilename);

            File.WriteAllText(reviewPath, reviewText);
        }

        #region Events

        private void sendProcessReviewerName(string reviewer)
        {
            if (OnProcessReviewerName != null)
                OnProcessReviewerName(reviewer, null);
        }

        private void sendProcessReviewParams(int index, int nbTotal)
        {
            Tuple<int, int> values = Tuple.Create(index, nbTotal);
            if (OnProcessReviewParams != null)
                OnProcessReviewParams(values, null);
        }

        private void sendProcessError(string text)
        {
            if (OnProcessError != null)
                OnProcessError(text, null);
        }

        void FormMain_OnProcessReviewerName(object sender, EventArgs e)
        {
            string reviewer = sender as string;

            if (!String.IsNullOrEmpty(reviewer))
                this.Invoke(new changeReviewerNameLabel(changeReviewerNameLabelText), "(" + reviewer + ")");
        }

        void FormMain_OnProcessReviewParams(object sender, EventArgs e)
        {
            Tuple<int, int> values = (Tuple<int, int>)sender;
            int index = values.Item1;
            int nbTotal = values.Item2;

            int percent = 100 * (index + 1) / nbTotal;
            float progress = (float)(1.0 / (nbTotal + 1));
            string text = "Exporting review " + (index + 1).ToString() + "/" + nbTotal + " (" + percent.ToString() + "%)...";
            
            this.Invoke(new changeProgressLabel(changeProgressLabelText), text);
            this.Invoke(new incrementProgressBar(incrementProgressBarValue), progress);
            this.Invoke(new incrementProgressTaskBar(incrementProgressTaskBarValue), progress);
        }

        private void FormMain_OnProcessError(object sender, EventArgs e)
        {
            string error = sender as string;

            this.Invoke(new changeProgressLabel(changeProgressLabelText), error);
        }

        private void changeReviewerNameLabelText(string text)
        {
            labelReviewerName.Text = text;
            labelReviewerName.Visible = true;
        }

        private void changeProgressLabelText(string text)
        {
            labelStatus.Text = text;
        }

        private void incrementProgressBarValue(float value)
        {
            int increment = (int)Math.Round(progressBar1.Maximum * value);
            progressBar1.Increment(increment);
        }

        private void incrementProgressTaskBarValue(float value)
        {
            taskbarManagerValue_ += 10000 * value;
            taskbarManager_.SetProgressValue((int)(taskbarManagerValue_ + 0.5), 10000);
        }

        #endregion

        #region Interface functions

        private void textboxReviewer_TextChanged(object sender, EventArgs e)
        {
            labelReviewerName.Visible = false;
            labelStatus.Visible = false;

            updateGUI();
        }

        private void textboxReviewer_Click(object sender, EventArgs e)
        {
            textboxReviewer.SelectAll();
        }

        private void textboxReviewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                buttonExport_Click(sender, e);
        }

        private void comboboxSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            site_ = comboboxSite.SelectedItem as string;
            switch (site_)
            {
                case "Prog Archives":

                    urlSite_ = "http://www.progarchives.com/";
                    urlReviewer_ = "http://www.progarchives.com/Collaborators.asp?id=" + "<REVIEWER>" + "&listreviews=alpha&showall=true#reviews";;
                    pictureboxHeader.Image = global::PMJAReviewExporter.Properties.Resources.PA_Logo;

                    labelReviewer.Text = "Reviewer ID";
                    string tooltipText = "Enter you reviewer ID.\r\n\r\n";
                    tooltipText += "If your reviewer page is http://www.progarchives.com/Collaborators.asp?id=11816\r\n";
                    tooltipText += "=> your reviewer ID is 11816";
                    tooltipTextboxReviewer.SetToolTip(textboxReviewer, tooltipText);

                    break;

                case "Metal Music Archives":
                    
                    urlSite_ = "http://www.metalmusicarchives.com";
                    urlReviewer_ = "http://www.metalmusicarchives.com/member/" + "<REVIEWER>" + "?reviews=all";
                    pictureboxHeader.Image = global::PMJAReviewExporter.Properties.Resources.MMA_Logo;

                    labelReviewer.Text = "Reviewer";
                    tooltipTextboxReviewer.SetToolTip(textboxReviewer, "Enter you reviewer pseudo");
                    
                    break;

                case "Jazz Music Archives":
                    urlSite_ = "http://www.jazzmusicarchives.com";
                    urlReviewer_ = "http://www.jazzmusicarchives.com/member/" + "<REVIEWER>" + "?reviews=all";
                    pictureboxHeader.Image = global::PMJAReviewExporter.Properties.Resources.JMA_Logo;

                    labelReviewer.Text = "Reviewer";
                    tooltipTextboxReviewer.SetToolTip(textboxReviewer, "Enter you reviewer pseudo");
                    
                    break;
            }

            Tools.DownloadPath = site_; // site_.Replace(" ", "");
            updateGUI();
        }

        private void updateGUI()
        {
            buttonExport.Text = isProcessing_ ? "Abort" : "EXPORT";

            buttonQuit.Enabled = !isProcessing_;
            comboboxSite.Enabled = !isProcessing_;
            textboxReviewer.Enabled = !isProcessing_;

            switch (site_)
            {
                case "Prog Archives":
                    buttonExport.Enabled = Tools.isStringNumerical(textboxReviewer.Text);
                    break;

                case "Metal Music Archives":
                case "Jazz Music Archives":
                    buttonExport.Enabled = !String.IsNullOrEmpty(textboxReviewer.Text);
                    break;
            }
        }

        #endregion
    }
}
