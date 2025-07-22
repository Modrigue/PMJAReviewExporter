using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PMJAReviewExporter
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler OnProcessReviewerName;
        public static event EventHandler OnProcessReviewParams;
        public static event EventHandler OnProcessError;

        private int nbReviewsRatingsProcessed_;
        Thread threadExport_;
        private bool isProcessing_;

        // deprecated
        //private TaskbarManager taskbarManager_;
        //private float taskbarManagerValue_;

        private string site_;
        private string urlSite_;
        private string urlReviewer_;

        private delegate void changeReviewerNameLabel(string str);
        private delegate void changeProgressLabel(string str);
        private delegate void incrementProgressBar(float value);
        private delegate void incrementProgressTaskBar(float value);

        public MainWindow()
        {
            InitializeComponent();

            OnProcessReviewerName += FormMain_OnProcessReviewerName;
            OnProcessReviewParams += FormMain_OnProcessReviewParams;
            OnProcessError += FormMain_OnProcessError;

            nbReviewsRatingsProcessed_ = 0;
            isProcessing_ = false;

            //taskbarManager_ = TaskbarManager.Instance;
            //taskbarManagerValue_ = 0;

            labelVersion.Text = "Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

            Tools.Initialize();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // check if required dlls are present
            string execDir = Environment.CurrentDirectory;
            string dllName = "HtmlAgilityPack.dll";
            string dllPath = Path.Combine(execDir, dllName);
            if (!File.Exists(dllPath))
            {
                MessageBox.Show("Library " + dllName + " not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            textboxReviewer.Focus();
        }

        private void buttonExport_Click(object sender, RoutedEventArgs e)
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

            labelStatus.Content = "Processing reviewer page...";
            labelStatus.Visibility = Visibility.Visible;

            progressbar.Visibility = Visibility.Visible;
            //progressbar.Style = ProgressBarStyle.Continuous;
            progressbar.Maximum = 10000;
            progressbar.Value = 0;

            //taskbarManager_.SetProgressState(TaskbarProgressBarState.Normal);
            //taskbarManagerValue_ = 0;
            //taskbarManager_.SetProgressValue(0, 10000);

            nbReviewsRatingsProcessed_ = 0;
            isProcessing_ = true;

            threadExport_ = new Thread(() => exportReviews(id));
            threadExport_.Start();

            updateGUI();

            while (isProcessing_)
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));

            updateGUI();

            progressbar.Visibility = Visibility.Hidden;
            progressbar.Maximum = 0;
            progressbar.Value = 0;

            //taskbarManager_.SetProgressState(TaskbarProgressBarState.NoProgress);
            //taskbarManagerValue_ = 0;
            //taskbarManager_.SetProgressValue(0, 10000);

            // update status label
            labelStatus.Content = nbReviewsRatingsProcessed_.ToString() + " review(s) exported";
        }

        private void buttonQuit_Click(object sender, RoutedEventArgs e)
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

            string headerText = band.ToUpper() + " - " + album + " (" + year + ")";
            string ratingText = rating + ((rating == "1") ? " star" : " stars");
            string reviewText = headerText + "\r\n" + ratingText + "\r\n\r\n" + text;

            if (!String.IsNullOrEmpty(urlReview))
                reviewText += "\r\n\r\n" + "Source: " + urlReview;

            // write into general file

            using (StreamWriter file = new StreamWriter(allReviewsPath, true))
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
            string reviewPath = System.IO.Path.Combine(dirReviewer, reviewFilename);

            File.WriteAllText(reviewPath, reviewText);
        }

        #region Events

        private void sendProcessReviewerName(string reviewer)
        {
            OnProcessReviewerName?.Invoke(reviewer, null);
        }

        private void sendProcessReviewParams(int index, int nbTotal)
        {
            Tuple<int, int> values = Tuple.Create(index, nbTotal);
            OnProcessReviewParams?.Invoke(values, null);
        }

        private void sendProcessError(string text)
        {
            OnProcessError?.Invoke(text, null);
        }

        void FormMain_OnProcessReviewerName(object sender, EventArgs e)
        {
            string reviewer = sender as string;

            if (!String.IsNullOrEmpty(reviewer))
                Dispatcher.Invoke(new changeReviewerNameLabel(changeReviewerNameLabelText), "(" + reviewer + ")");
        }

        void FormMain_OnProcessReviewParams(object sender, EventArgs e)
        {
            Tuple<int, int> values = (Tuple<int, int>)sender;
            int index = values.Item1;
            int nbTotal = values.Item2;

            int percent = 100 * (index + 1) / nbTotal;
            float progress = (float)(1.0 / (nbTotal + 1));
            string text = "Exporting review " + (index + 1).ToString() + "/" + nbTotal + " (" + percent.ToString() + "%)...";

            Dispatcher.Invoke(new changeProgressLabel(changeProgressLabelText), text);
            Dispatcher.Invoke(new incrementProgressBar(incrementProgressBarValue), progress);
            Dispatcher.Invoke(new incrementProgressTaskBar(incrementProgressTaskBarValue), progress);
        }

        private void FormMain_OnProcessError(object sender, EventArgs e)
        {
            string error = sender as string;

            Dispatcher.Invoke(new changeProgressLabel(changeProgressLabelText), error);
        }

        private void changeReviewerNameLabelText(string text)
        {
            labelReviewerName.Text = text;
            labelReviewerName.Visibility = Visibility.Visible;
        }

        private void changeProgressLabelText(string text)
        {
            labelStatus.Content = text;
        }

        private void incrementProgressBarValue(float value)
        {
            int increment = (int)Math.Round(progressbar.Maximum * value);
            progressbar.Value += increment;
        }

        private void incrementProgressTaskBarValue(float value)
        {
            //taskbarManagerValue_ += 10000 * value;
            //taskbarManager_.SetProgressValue((int)(taskbarManagerValue_ + 0.5), 10000);
        }

        #endregion

        #region Interface functions

        private void textboxReviewer_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            labelReviewerName.Visibility = Visibility.Hidden;
            labelStatus.Visibility = Visibility.Hidden;

            updateGUI();
        }


        private void textboxReviewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            textboxReviewer.SelectAll();
        }

        private void textboxReviewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
                buttonExport_Click(sender, e);
        }

        private void comboboxSite_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string sitePrev = site_;
            bool siteWasProg = !String.IsNullOrEmpty(sitePrev) && sitePrev.ToLower().StartsWith("prog");

            site_ = ((ComboBoxItem)comboboxSite.SelectedItem).Content.ToString();
            switch (site_)
            {
                case "Prog Archives":

                    urlSite_ = "http://www.progarchives.com/";
                    urlReviewer_ = "http://www.progarchives.com/Collaborators.asp?id=" + "<REVIEWER>" + "&listreviews=alpha&showall=true#reviews"; ;
                    imageHeader.Source = (ImageSource)Application.Current.FindResource("PA_Logo");

                    labelReviewer.Text = "Reviewer ID";
                    string tooltipText = "Enter reviewer ID.\r\n\r\n";
                    tooltipText += "If reviewer page is http://www.progarchives.com/Collaborators.asp?id=11816\r\n";
                    tooltipText += "=> reviewer ID is 11816";
                    textboxReviewer.ToolTip = tooltipText;

                    if (!siteWasProg)
                    {
                        textboxReviewer.Text = String.Empty;
                        labelReviewerName.Text = String.Empty;
                    }

                    break;

                case "Metal Music Archives":

                    urlSite_ = "http://www.metalmusicarchives.com";
                    urlReviewer_ = "http://www.metalmusicarchives.com/member/" + "<REVIEWER>" + "?reviews=all";
                    imageHeader.Source = (ImageSource)Application.Current.FindResource("MMA_Logo");

                    labelReviewer.Text = "Reviewer";
                    textboxReviewer.ToolTip = "Enter reviewer pseudo";

                    if (siteWasProg)
                    {
                        textboxReviewer.Text = String.Empty;
                        labelReviewerName.Text = String.Empty;
                    }

                    break;

                case "Jazz Music Archives":
                    urlSite_ = "http://www.jazzmusicarchives.com";
                    urlReviewer_ = "http://www.jazzmusicarchives.com/member/" + "<REVIEWER>" + "?reviews=all";
                    imageHeader.Source = (ImageSource)Application.Current.FindResource("JMA_Logo");

                    labelReviewer.Text = "Reviewer";
                    textboxReviewer.ToolTip = "Enter reviewer pseudo";

                    if (siteWasProg)
                    {
                        textboxReviewer.Text = String.Empty;
                        labelReviewerName.Text = String.Empty;
                    }

                    break;
            }



            labelStatus.Visibility = Visibility.Hidden;

            Tools.DownloadPath = site_; // site_.Replace(" ", "");
            updateGUI();
        }

        private void updateGUI()
        {
            buttonExport.Content = isProcessing_ ? "Abort" : "EXPORT";

            buttonQuit.IsEnabled = !isProcessing_;
            comboboxSite.IsEnabled = !isProcessing_;
            textboxReviewer.IsEnabled = !isProcessing_;

            switch (site_)
            {
                case "Prog Archives":
                    buttonExport.IsEnabled = Tools.isStringNumerical(textboxReviewer.Text);
                    break;

                case "Metal Music Archives":
                case "Jazz Music Archives":
                    buttonExport.IsEnabled = !String.IsNullOrEmpty(textboxReviewer.Text);
                    break;
            }
        }

        #endregion
    }
}
