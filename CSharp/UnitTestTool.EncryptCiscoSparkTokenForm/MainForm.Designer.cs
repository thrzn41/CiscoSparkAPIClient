﻿namespace UnitTestTool.EncryptCiscoSparkTokenForm
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelToken = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.labelPath = new System.Windows.Forms.Label();
            this.maskedTextBoxToken = new System.Windows.Forms.MaskedTextBox();
            this.buttonExportToken = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelToken
            // 
            resources.ApplyResources(this.labelToken, "labelToken");
            this.labelToken.Name = "labelToken";
            // 
            // textBoxPath
            // 
            resources.ApplyResources(this.textBoxPath, "textBoxPath");
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.ReadOnly = true;
            // 
            // labelPath
            // 
            resources.ApplyResources(this.labelPath, "labelPath");
            this.labelPath.Name = "labelPath";
            // 
            // maskedTextBoxToken
            // 
            resources.ApplyResources(this.maskedTextBoxToken, "maskedTextBoxToken");
            this.maskedTextBoxToken.Name = "maskedTextBoxToken";
            this.maskedTextBoxToken.UseSystemPasswordChar = true;
            // 
            // buttonExportToken
            // 
            resources.ApplyResources(this.buttonExportToken, "buttonExportToken");
            this.buttonExportToken.Name = "buttonExportToken";
            this.buttonExportToken.UseVisualStyleBackColor = true;
            this.buttonExportToken.Click += new System.EventHandler(this.buttonExportToken_Click);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonExportToken);
            this.Controls.Add(this.maskedTextBoxToken);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.labelToken);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelToken;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.MaskedTextBox maskedTextBoxToken;
        private System.Windows.Forms.Button buttonExportToken;
    }
}

