namespace TestApplication
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.tbPackageOrigin = new System.Windows.Forms.TextBox();
            this.btnInstallPackage = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "label1";
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(12, 25);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(100, 23);
            this.pbProgress.TabIndex = 1;
            // 
            // tbPackageOrigin
            // 
            this.tbPackageOrigin.Location = new System.Drawing.Point(48, 196);
            this.tbPackageOrigin.Name = "tbPackageOrigin";
            this.tbPackageOrigin.Size = new System.Drawing.Size(174, 20);
            this.tbPackageOrigin.TabIndex = 2;
            // 
            // btnInstallPackage
            // 
            this.btnInstallPackage.Location = new System.Drawing.Point(228, 194);
            this.btnInstallPackage.Name = "btnInstallPackage";
            this.btnInstallPackage.Size = new System.Drawing.Size(75, 23);
            this.btnInstallPackage.TabIndex = 3;
            this.btnInstallPackage.Text = "button1";
            this.btnInstallPackage.UseVisualStyleBackColor = true;
            this.btnInstallPackage.Click += new System.EventHandler(this.btnInstallPackage_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(379, 25);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnInstallPackage);
            this.Controls.Add(this.tbPackageOrigin);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.lblStatus);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.TextBox tbPackageOrigin;
        private System.Windows.Forms.Button btnInstallPackage;
        private System.Windows.Forms.Button btnStart;
    }
}

