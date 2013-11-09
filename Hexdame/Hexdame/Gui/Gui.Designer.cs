namespace Hexdame
{
    partial class Gui
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonConfirmMove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonConfirmMove
            // 
            this.buttonConfirmMove.Location = new System.Drawing.Point(12, 12);
            this.buttonConfirmMove.Name = "buttonConfirmMove";
            this.buttonConfirmMove.Size = new System.Drawing.Size(75, 23);
            this.buttonConfirmMove.TabIndex = 1;
            this.buttonConfirmMove.Text = "Move";
            this.buttonConfirmMove.UseVisualStyleBackColor = true;
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 501);
            this.Controls.Add(this.buttonConfirmMove);
            this.Name = "Gui";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonConfirmMove;
    }
}

