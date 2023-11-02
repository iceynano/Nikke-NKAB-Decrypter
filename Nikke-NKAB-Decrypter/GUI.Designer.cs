namespace Nikke_NKAB_Decrypter
{
    partial class GUI
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
            this.tbDirPath = new System.Windows.Forms.TextBox();
            this.btnDirSelect = new System.Windows.Forms.Button();
            this.btnExportSelect = new System.Windows.Forms.Button();
            this.tbExportPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnDecrypt = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnEncryptSelect = new System.Windows.Forms.Button();
            this.tbEncryptPath = new System.Windows.Forms.TextBox();
            this.btnEncrypt = new System.Windows.Forms.Button();
            this.linkGithub = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // tbDirPath
            // 
            this.tbDirPath.Location = new System.Drawing.Point(93, 37);
            this.tbDirPath.Name = "tbDirPath";
            this.tbDirPath.ReadOnly = true;
            this.tbDirPath.Size = new System.Drawing.Size(279, 20);
            this.tbDirPath.TabIndex = 0;
            // 
            // btnDirSelect
            // 
            this.btnDirSelect.Location = new System.Drawing.Point(12, 35);
            this.btnDirSelect.Name = "btnDirSelect";
            this.btnDirSelect.Size = new System.Drawing.Size(75, 23);
            this.btnDirSelect.TabIndex = 1;
            this.btnDirSelect.Text = "Select";
            this.btnDirSelect.UseVisualStyleBackColor = true;
            this.btnDirSelect.Click += new System.EventHandler(this.btnDirSelect_Click);
            // 
            // btnExportSelect
            // 
            this.btnExportSelect.Location = new System.Drawing.Point(12, 92);
            this.btnExportSelect.Name = "btnExportSelect";
            this.btnExportSelect.Size = new System.Drawing.Size(75, 23);
            this.btnExportSelect.TabIndex = 3;
            this.btnExportSelect.Text = "Select";
            this.btnExportSelect.UseVisualStyleBackColor = true;
            this.btnExportSelect.Click += new System.EventHandler(this.btnExportSelect_Click);
            // 
            // tbExportPath
            // 
            this.tbExportPath.Location = new System.Drawing.Point(93, 94);
            this.tbExportPath.Name = "tbExportPath";
            this.tbExportPath.ReadOnly = true;
            this.tbExportPath.Size = new System.Drawing.Size(279, 20);
            this.tbExportPath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "EB Path (Game Location)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Decrypt Path";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 250);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(360, 23);
            this.progressBar.TabIndex = 6;
            // 
            // btnDecrypt
            // 
            this.btnDecrypt.Location = new System.Drawing.Point(12, 193);
            this.btnDecrypt.Name = "btnDecrypt";
            this.btnDecrypt.Size = new System.Drawing.Size(360, 23);
            this.btnDecrypt.TabIndex = 7;
            this.btnDecrypt.Text = "Decrypt";
            this.btnDecrypt.UseVisualStyleBackColor = true;
            this.btnDecrypt.Click += new System.EventHandler(this.btnDecrypt_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Encrypt Path";
            // 
            // btnEncryptSelect
            // 
            this.btnEncryptSelect.Location = new System.Drawing.Point(12, 144);
            this.btnEncryptSelect.Name = "btnEncryptSelect";
            this.btnEncryptSelect.Size = new System.Drawing.Size(75, 23);
            this.btnEncryptSelect.TabIndex = 9;
            this.btnEncryptSelect.Text = "Select";
            this.btnEncryptSelect.UseVisualStyleBackColor = true;
            this.btnEncryptSelect.Click += new System.EventHandler(this.btnEncryptSelect_Click);
            // 
            // tbEncryptPath
            // 
            this.tbEncryptPath.Location = new System.Drawing.Point(93, 146);
            this.tbEncryptPath.Name = "tbEncryptPath";
            this.tbEncryptPath.ReadOnly = true;
            this.tbEncryptPath.Size = new System.Drawing.Size(279, 20);
            this.tbEncryptPath.TabIndex = 8;
            // 
            // btnEncrypt
            // 
            this.btnEncrypt.Location = new System.Drawing.Point(12, 220);
            this.btnEncrypt.Name = "btnEncrypt";
            this.btnEncrypt.Size = new System.Drawing.Size(360, 23);
            this.btnEncrypt.TabIndex = 11;
            this.btnEncrypt.Text = "Encrypt";
            this.btnEncrypt.UseVisualStyleBackColor = true;
            this.btnEncrypt.Click += new System.EventHandler(this.btnEncrypt_Click);
            // 
            // linkGithub
            // 
            this.linkGithub.AutoSize = true;
            this.linkGithub.Location = new System.Drawing.Point(12, 285);
            this.linkGithub.Name = "linkGithub";
            this.linkGithub.Size = new System.Drawing.Size(91, 13);
            this.linkGithub.TabIndex = 12;
            this.linkGithub.TabStop = true;
            this.linkGithub.Text = "Github Repository";
            this.linkGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkGithub_LinkClicked);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 311);
            this.Controls.Add(this.linkGithub);
            this.Controls.Add(this.btnEncrypt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnEncryptSelect);
            this.Controls.Add(this.tbEncryptPath);
            this.Controls.Add(this.btnDecrypt);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExportSelect);
            this.Controls.Add(this.tbExportPath);
            this.Controls.Add(this.btnDirSelect);
            this.Controls.Add(this.tbDirPath);
            this.Name = "GUI";
            this.Text = "Nikke NKAB Decrypter";
            this.Load += new System.EventHandler(this.GUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDirPath;
        private System.Windows.Forms.Button btnDirSelect;
        private System.Windows.Forms.Button btnExportSelect;
        private System.Windows.Forms.TextBox tbExportPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnDecrypt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnEncryptSelect;
        private System.Windows.Forms.TextBox tbEncryptPath;
        private System.Windows.Forms.Button btnEncrypt;
        private System.Windows.Forms.LinkLabel linkGithub;
    }
}