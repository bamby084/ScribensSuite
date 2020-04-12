﻿using PluginScribens_Word.WPF.Views;

namespace PluginScribens_Word
{
    partial class TaskPaneHost
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wpfContentHost = new System.Windows.Forms.Integration.ElementHost();
            this.taskPane = new TaskPaneView();
            this.SuspendLayout();
            // 
            // wpfContentHost
            // 
            this.wpfContentHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfContentHost.Location = new System.Drawing.Point(0, 0);
            this.wpfContentHost.Name = "wpfContentHost";
            this.wpfContentHost.Size = new System.Drawing.Size(379, 341);
            this.wpfContentHost.TabIndex = 0;
            this.wpfContentHost.Text = "elementHost1";
            this.wpfContentHost.Child = this.taskPane;
            // 
            // TaskPaneHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wpfContentHost);
            this.Name = "TaskPaneHost";
            this.Size = new System.Drawing.Size(379, 341);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost wpfContentHost;
        private TaskPaneView taskPane;
    }
}