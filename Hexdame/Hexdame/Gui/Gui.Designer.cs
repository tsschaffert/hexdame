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
            this.gamePanel = new System.Windows.Forms.Panel();
            this.messageLog = new System.Windows.Forms.TextBox();
            this.buttonNewGame = new System.Windows.Forms.Button();
            this.labelCurrentPlayer = new System.Windows.Forms.Label();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonConfirmMove
            // 
            this.buttonConfirmMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConfirmMove.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F);
            this.buttonConfirmMove.Location = new System.Drawing.Point(3, 447);
            this.buttonConfirmMove.Name = "buttonConfirmMove";
            this.buttonConfirmMove.Size = new System.Drawing.Size(247, 50);
            this.buttonConfirmMove.TabIndex = 1;
            this.buttonConfirmMove.Text = "Move";
            this.buttonConfirmMove.UseVisualStyleBackColor = true;
            // 
            // gamePanel
            // 
            this.gamePanel.Location = new System.Drawing.Point(12, 12);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(500, 500);
            this.gamePanel.TabIndex = 2;
            // 
            // messageLog
            // 
            this.messageLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageLog.Location = new System.Drawing.Point(12, 518);
            this.messageLog.Multiline = true;
            this.messageLog.Name = "messageLog";
            this.messageLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.messageLog.Size = new System.Drawing.Size(760, 131);
            this.messageLog.TabIndex = 3;
            // 
            // buttonNewGame
            // 
            this.buttonNewGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNewGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonNewGame.Location = new System.Drawing.Point(3, 3);
            this.buttonNewGame.Name = "buttonNewGame";
            this.buttonNewGame.Size = new System.Drawing.Size(247, 50);
            this.buttonNewGame.TabIndex = 4;
            this.buttonNewGame.Text = "New Game";
            this.buttonNewGame.UseVisualStyleBackColor = true;
            this.buttonNewGame.Click += new System.EventHandler(this.buttonNewGame_Click);
            // 
            // labelCurrentPlayer
            // 
            this.labelCurrentPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCurrentPlayer.AutoSize = true;
            this.labelCurrentPlayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentPlayer.Location = new System.Drawing.Point(3, 56);
            this.labelCurrentPlayer.MaximumSize = new System.Drawing.Size(247, 0);
            this.labelCurrentPlayer.Name = "labelCurrentPlayer";
            this.labelCurrentPlayer.Size = new System.Drawing.Size(0, 36);
            this.labelCurrentPlayer.TabIndex = 5;
            // 
            // panelMenu
            // 
            this.panelMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMenu.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelMenu.Controls.Add(this.buttonConfirmMove);
            this.panelMenu.Controls.Add(this.buttonNewGame);
            this.panelMenu.Controls.Add(this.labelCurrentPlayer);
            this.panelMenu.Location = new System.Drawing.Point(519, 12);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(253, 500);
            this.panelMenu.TabIndex = 6;
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(784, 661);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.messageLog);
            this.Controls.Add(this.gamePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximumSize = new System.Drawing.Size(800, 700);
            this.MinimumSize = new System.Drawing.Size(800, 700);
            this.Name = "Gui";
            this.Text = "Hexdame";
            this.panelMenu.ResumeLayout(false);
            this.panelMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonConfirmMove;
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.TextBox messageLog;
        private System.Windows.Forms.Button buttonNewGame;
        private System.Windows.Forms.Label labelCurrentPlayer;
        private System.Windows.Forms.Panel panelMenu;
    }
}

