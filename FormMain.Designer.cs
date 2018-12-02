namespace PMJAReviewExporter
{
    partial class FormMain
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.buttonQuit = new System.Windows.Forms.Button();
            this.labelReviewer = new System.Windows.Forms.Label();
            this.buttonExport = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textboxReviewer = new System.Windows.Forms.TextBox();
            this.pictureboxHeader = new System.Windows.Forms.PictureBox();
            this.labelReviewerName = new System.Windows.Forms.Label();
            this.labelSite = new System.Windows.Forms.Label();
            this.comboboxSite = new System.Windows.Forms.ComboBox();
            this.tooltipTextboxReviewer = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureboxHeader)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonQuit
            // 
            this.buttonQuit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonQuit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonQuit.Location = new System.Drawing.Point(253, 368);
            this.buttonQuit.Name = "buttonQuit";
            this.buttonQuit.Size = new System.Drawing.Size(144, 28);
            this.buttonQuit.TabIndex = 0;
            this.buttonQuit.Text = "Quit";
            this.buttonQuit.UseVisualStyleBackColor = true;
            this.buttonQuit.Click += new System.EventHandler(this.buttonQuit_Click);
            // 
            // labelReviewer
            // 
            this.labelReviewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReviewer.AutoSize = true;
            this.labelReviewer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReviewer.Location = new System.Drawing.Point(33, 301);
            this.labelReviewer.Name = "labelReviewer";
            this.labelReviewer.Size = new System.Drawing.Size(65, 16);
            this.labelReviewer.TabIndex = 3;
            this.labelReviewer.Text = "Reviewer";
            // 
            // buttonExport
            // 
            this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonExport.Enabled = false;
            this.buttonExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExport.Location = new System.Drawing.Point(253, 269);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(144, 73);
            this.buttonExport.TabIndex = 4;
            this.buttonExport.Text = "EXPORT";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.ForeColor = System.Drawing.Color.Gray;
            this.labelVersion.Location = new System.Drawing.Point(12, 213);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(3);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(62, 13);
            this.labelVersion.TabIndex = 5;
            this.labelVersion.Text = "Version X.Y";
            // 
            // labelStatus
            // 
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.ForeColor = System.Drawing.Color.Green;
            this.labelStatus.Location = new System.Drawing.Point(26, 371);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(190, 23);
            this.labelStatus.TabIndex = 8;
            this.labelStatus.Text = "Exporting review XXXX/YYYY (ZZ%)";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelStatus.Visible = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(-3, 0);
            this.progressBar1.MarqueeAnimationSpeed = 30;
            this.progressBar1.Maximum = 100000;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(441, 4);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // textboxReviewer
            // 
            this.textboxReviewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxReviewer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxReviewer.Location = new System.Drawing.Point(36, 320);
            this.textboxReviewer.MaxLength = 19;
            this.textboxReviewer.Name = "textboxReviewer";
            this.textboxReviewer.Size = new System.Drawing.Size(170, 22);
            this.textboxReviewer.TabIndex = 10;
            this.textboxReviewer.Click += new System.EventHandler(this.textboxReviewer_Click);
            this.textboxReviewer.TextChanged += new System.EventHandler(this.textboxReviewer_TextChanged);
            this.textboxReviewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textboxReviewer_KeyDown);
            // 
            // pictureboxHeader
            // 
            this.pictureboxHeader.Location = new System.Drawing.Point(-3, 0);
            this.pictureboxHeader.Name = "pictureboxHeader";
            this.pictureboxHeader.Size = new System.Drawing.Size(438, 200);
            this.pictureboxHeader.TabIndex = 1;
            this.pictureboxHeader.TabStop = false;
            // 
            // labelReviewerName
            // 
            this.labelReviewerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReviewerName.AutoSize = true;
            this.labelReviewerName.BackColor = System.Drawing.Color.Transparent;
            this.labelReviewerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelReviewerName.ForeColor = System.Drawing.Color.Gray;
            this.labelReviewerName.Location = new System.Drawing.Point(36, 348);
            this.labelReviewerName.Margin = new System.Windows.Forms.Padding(3);
            this.labelReviewerName.Name = "labelReviewerName";
            this.labelReviewerName.Size = new System.Drawing.Size(81, 13);
            this.labelReviewerName.TabIndex = 11;
            this.labelReviewerName.Text = "Reviewer name";
            this.labelReviewerName.Visible = false;
            // 
            // labelSite
            // 
            this.labelSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSite.AutoSize = true;
            this.labelSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSite.Location = new System.Drawing.Point(33, 250);
            this.labelSite.Name = "labelSite";
            this.labelSite.Size = new System.Drawing.Size(31, 16);
            this.labelSite.TabIndex = 12;
            this.labelSite.Text = "Site";
            // 
            // comboboxSite
            // 
            this.comboboxSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboboxSite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboboxSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboboxSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboboxSite.FormattingEnabled = true;
            this.comboboxSite.Items.AddRange(new object[] {
            "Prog Archives",
            "Metal Music Archives",
            "Jazz Music Archives"});
            this.comboboxSite.Location = new System.Drawing.Point(36, 269);
            this.comboboxSite.Name = "comboboxSite";
            this.comboboxSite.Size = new System.Drawing.Size(170, 24);
            this.comboboxSite.TabIndex = 13;
            this.comboboxSite.SelectedIndexChanged += new System.EventHandler(this.comboboxSite_SelectedIndexChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 411);
            this.Controls.Add(this.comboboxSite);
            this.Controls.Add(this.labelSite);
            this.Controls.Add(this.labelReviewerName);
            this.Controls.Add(this.textboxReviewer);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.buttonExport);
            this.Controls.Add(this.labelReviewer);
            this.Controls.Add(this.pictureboxHeader);
            this.Controls.Add(this.buttonQuit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PMJA Review Exporter";
            this.Load += new System.EventHandler(this.FormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureboxHeader)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonQuit;
        private System.Windows.Forms.PictureBox pictureboxHeader;
        private System.Windows.Forms.Label labelReviewer;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelStatus;
        public System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textboxReviewer;
        private System.Windows.Forms.Label labelReviewerName;
        private System.Windows.Forms.Label labelSite;
        private System.Windows.Forms.ComboBox comboboxSite;
        private System.Windows.Forms.ToolTip tooltipTextboxReviewer;
    }
}

