using FastColoredTextBoxNS;
namespace Tester
{
    partial class PowerfulSample
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PowerfulSample));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
      this.goBackwardCtrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.goForwardCtrlShiftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
      this.goLeftBracketToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.goRightBracketToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
      this.miExport = new System.Windows.Forms.ToolStripMenuItem();
      this.hTMLToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.rTFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.miChangeColors = new System.Windows.Forms.ToolStripMenuItem();
      this.changeHotkeysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.fctb = new FastColoredTextBoxNS.FastColoredTextBox();
      this.regExHighlighter1 = new Qhta.RegularExpressions.FCTB.Tools.RegExHighlighter(this.components);
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.fctb)).BeginInit();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.miExport,
            this.miChangeColors,
            this.changeHotkeysToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(605, 24);
      this.menuStrip1.TabIndex = 4;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // editToolStripMenuItem
      // 
      this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.replaceToolStripMenuItem,
            this.toolStripMenuItem4,
            this.goBackwardCtrlToolStripMenuItem,
            this.goForwardCtrlShiftToolStripMenuItem,
            this.toolStripMenuItem5,
            this.goLeftBracketToolStripMenuItem,
            this.goRightBracketToolStripMenuItem,
            this.toolStripMenuItem7});
      this.editToolStripMenuItem.Name = "editToolStripMenuItem";
      this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
      this.editToolStripMenuItem.Text = "&Edit";
      // 
      // findToolStripMenuItem
      // 
      this.findToolStripMenuItem.Name = "findToolStripMenuItem";
      this.findToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.findToolStripMenuItem.Text = "&Find [Ctrl+F]";
      this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
      // 
      // replaceToolStripMenuItem
      // 
      this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
      this.replaceToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.replaceToolStripMenuItem.Text = "&Replace [Ctrl+H]";
      this.replaceToolStripMenuItem.Click += new System.EventHandler(this.replaceToolStripMenuItem_Click);
      // 
      // toolStripMenuItem4
      // 
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(210, 6);
      // 
      // goBackwardCtrlToolStripMenuItem
      // 
      this.goBackwardCtrlToolStripMenuItem.Name = "goBackwardCtrlToolStripMenuItem";
      this.goBackwardCtrlToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.goBackwardCtrlToolStripMenuItem.Text = "Go Backward [Ctrl+ -]";
      this.goBackwardCtrlToolStripMenuItem.Click += new System.EventHandler(this.goBackwardCtrlToolStripMenuItem_Click);
      // 
      // goForwardCtrlShiftToolStripMenuItem
      // 
      this.goForwardCtrlShiftToolStripMenuItem.Name = "goForwardCtrlShiftToolStripMenuItem";
      this.goForwardCtrlShiftToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.goForwardCtrlShiftToolStripMenuItem.Text = "Go Forward [Ctrl+Shift+ -]";
      this.goForwardCtrlShiftToolStripMenuItem.Click += new System.EventHandler(this.goForwardCtrlShiftToolStripMenuItem_Click);
      // 
      // toolStripMenuItem5
      // 
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new System.Drawing.Size(210, 6);
      // 
      // goLeftBracketToolStripMenuItem
      // 
      this.goLeftBracketToolStripMenuItem.Name = "goLeftBracketToolStripMenuItem";
      this.goLeftBracketToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.goLeftBracketToolStripMenuItem.Text = "Go Left Bracket";
      this.goLeftBracketToolStripMenuItem.Click += new System.EventHandler(this.goLeftBracketToolStripMenuItem_Click);
      // 
      // goRightBracketToolStripMenuItem
      // 
      this.goRightBracketToolStripMenuItem.Name = "goRightBracketToolStripMenuItem";
      this.goRightBracketToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
      this.goRightBracketToolStripMenuItem.Text = "Go Right Bracket";
      this.goRightBracketToolStripMenuItem.Click += new System.EventHandler(this.goRightBracketToolStripMenuItem_Click);
      // 
      // toolStripMenuItem7
      // 
      this.toolStripMenuItem7.Name = "toolStripMenuItem7";
      this.toolStripMenuItem7.Size = new System.Drawing.Size(210, 6);
      // 
      // miExport
      // 
      this.miExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hTMLToolStripMenuItem1,
            this.rTFToolStripMenuItem});
      this.miExport.Name = "miExport";
      this.miExport.Size = new System.Drawing.Size(53, 20);
      this.miExport.Text = "Export";
      // 
      // hTMLToolStripMenuItem1
      // 
      this.hTMLToolStripMenuItem1.Name = "hTMLToolStripMenuItem1";
      this.hTMLToolStripMenuItem1.Size = new System.Drawing.Size(106, 22);
      this.hTMLToolStripMenuItem1.Text = "HTML";
      this.hTMLToolStripMenuItem1.Click += new System.EventHandler(this.hTMLToolStripMenuItem1_Click);
      // 
      // rTFToolStripMenuItem
      // 
      this.rTFToolStripMenuItem.Name = "rTFToolStripMenuItem";
      this.rTFToolStripMenuItem.Size = new System.Drawing.Size(106, 22);
      this.rTFToolStripMenuItem.Text = "RTF";
      this.rTFToolStripMenuItem.Click += new System.EventHandler(this.rTFToolStripMenuItem_Click);
      // 
      // miChangeColors
      // 
      this.miChangeColors.Enabled = false;
      this.miChangeColors.Name = "miChangeColors";
      this.miChangeColors.Size = new System.Drawing.Size(95, 20);
      this.miChangeColors.Text = "Change colors";
      this.miChangeColors.Click += new System.EventHandler(this.miChangeColors_Click);
      // 
      // changeHotkeysToolStripMenuItem
      // 
      this.changeHotkeysToolStripMenuItem.Name = "changeHotkeysToolStripMenuItem";
      this.changeHotkeysToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
      this.changeHotkeysToolStripMenuItem.Text = "Change hotkeys";
      this.changeHotkeysToolStripMenuItem.Click += new System.EventHandler(this.changeHotkeysToolStripMenuItem_Click);
      // 
      // fctb
      // 
      this.fctb.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
      this.fctb.AutoIndentCharsPatterns = "^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;=]+);\r\n^\\s*(case|default)\\s*[^:]*" +
    "(?<range>:)\\s*(?<range>[^;]+);";
      this.fctb.AutoIndentExistingLines = false;
      this.fctb.AutoScrollMinSize = new System.Drawing.Size(380, 15);
      this.fctb.BackBrush = null;
      this.fctb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.fctb.CharHeight = 15;
      this.fctb.CharWidth = 7;
      this.fctb.Cursor = System.Windows.Forms.Cursors.IBeam;
      this.fctb.DelayedEventsInterval = 200;
      this.fctb.DelayedTextChangedInterval = 500;
      this.fctb.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
      this.fctb.Dock = System.Windows.Forms.DockStyle.Fill;
      this.fctb.Font = new System.Drawing.Font("Consolas", 9.75F);
      this.fctb.ImeMode = System.Windows.Forms.ImeMode.Off;
      this.fctb.IsReplaceMode = false;
      this.fctb.Location = new System.Drawing.Point(0, 24);
      this.fctb.Name = "fctb";
      this.fctb.Paddings = new System.Windows.Forms.Padding(0);
      this.fctb.PreferredLineWidth = 80;
      this.fctb.ReservedCountOfLineNumberChars = 2;
      this.fctb.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
      this.fctb.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fctb.ServiceColors")));
      this.fctb.ShowLineNumbers = false;
      this.fctb.Size = new System.Drawing.Size(605, 149);
      this.fctb.TabIndex = 3;
      this.fctb.Text = "\\b(?<month>\\d{1,2})/(?<day>\\d{1,2})/(?<year>\\d{2,4})\\b";
      this.fctb.Zoom = 100;
      this.fctb.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fctb_TextChanged);
      this.fctb.SelectionChangedDelayed += new System.EventHandler(this.fctb_SelectionChangedDelayed);
      this.fctb.AutoIndentNeeded += new System.EventHandler<FastColoredTextBoxNS.AutoIndentEventArgs>(this.fctb_AutoIndentNeeded);
      this.fctb.CustomAction += new System.EventHandler<FastColoredTextBoxNS.CustomActionEventArgs>(this.fctb_CustomAction);
      // 
      // regExHighlighter1
      // 
      this.regExHighlighter1.FCTB = null;
      // 
      // PowerfulSample
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(605, 173);
      this.Controls.Add(this.fctb);
      this.Controls.Add(this.menuStrip1);
      this.ImeMode = System.Windows.Forms.ImeMode.Hiragana;
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "PowerfulSample";
      this.Text = "Qhta Regular Expressions sample";
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.fctb)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem replaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miExport;
        private System.Windows.Forms.ToolStripMenuItem hTMLToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem goBackwardCtrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goForwardCtrlShiftToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem goLeftBracketToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goRightBracketToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem miChangeColors;
        private System.Windows.Forms.ToolStripMenuItem changeHotkeysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rTFToolStripMenuItem;
    public Qhta.RegularExpressions.FCTB.Tools.RegExHighlighter regExHighlighter1;
    public FastColoredTextBox fctb;
  }
}

