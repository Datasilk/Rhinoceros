namespace Rhinoceros
{
    partial class MenuButton
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
            this.SuspendLayout();
            // 
            // MenuButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Name = "MenuButton";
            this.Size = new System.Drawing.Size(59, 48);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MenuButton_MouseDown);
            this.MouseEnter += new System.EventHandler(this.MenuButton_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.MenuButton_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MenuButton_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
