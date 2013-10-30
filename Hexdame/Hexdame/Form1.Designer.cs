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
            this.hexdameButton2 = new Hexdame.HexdameButton();
            this.buttonConfirmMove = new System.Windows.Forms.Button();
            this.hexdameButton1 = new Hexdame.HexdameButton();
            this.SuspendLayout();
            // 
            // hexdameButton2
            // 
            this.hexdameButton2.Location = new System.Drawing.Point(96, 205);
            this.hexdameButton2.Name = "hexdameButton2";
            this.hexdameButton2.Size = new System.Drawing.Size(75, 23);
            this.hexdameButton2.TabIndex = 0;
            this.hexdameButton2.Text = "button1";
            this.hexdameButton2.UseVisualStyleBackColor = true;
            // 
            // buttonConfirmMove
            // 
            this.buttonConfirmMove.Location = new System.Drawing.Point(327, 259);
            this.buttonConfirmMove.Name = "buttonConfirmMove";
            this.buttonConfirmMove.Size = new System.Drawing.Size(75, 23);
            this.buttonConfirmMove.TabIndex = 1;
            this.buttonConfirmMove.Text = "Move";
            this.buttonConfirmMove.UseVisualStyleBackColor = true;
            // 
            // hexdameButton1
            // 
            this.hexdameButton1.Location = new System.Drawing.Point(96, 152);
            this.hexdameButton1.Name = "hexdameButton1";
            this.hexdameButton1.Size = new System.Drawing.Size(75, 23);
            this.hexdameButton1.TabIndex = 2;
            this.hexdameButton1.Text = "hexdameButton1";
            this.hexdameButton1.UseVisualStyleBackColor = true;
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 315);
            this.Controls.Add(this.hexdameButton1);
            this.Controls.Add(this.buttonConfirmMove);
            this.Controls.Add(this.hexdameButton2);
            this.Name = "Gui";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private HexdameButton hexdameButton2;
        private System.Windows.Forms.Button buttonConfirmMove;
        private HexdameButton hexdameButton1;
    }
}

