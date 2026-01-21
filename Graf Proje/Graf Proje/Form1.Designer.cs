namespace Graf_Proje
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.button3 = new System.Windows.Forms.Button();
            this.pnlCiz = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.aranacakID = new System.Windows.Forms.TextBox();
            this.btnCiz = new System.Windows.Forms.Button();
            this.Analiz = new System.Windows.Forms.Button();
            this.KCore = new System.Windows.Forms.Button();
            this.KDegeri = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Reset = new System.Windows.Forms.Button();
            this.pnlCiz.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(17, 623);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(128, 86);
            this.button3.TabIndex = 0;
            this.button3.Text = "jsonOku";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.jsonOku);
            // 
            // pnlCiz
            // 
            this.pnlCiz.Controls.Add(this.label4);
            this.pnlCiz.Controls.Add(this.label2);
            this.pnlCiz.Location = new System.Drawing.Point(12, 12);
            this.pnlCiz.Name = "pnlCiz";
            this.pnlCiz.Size = new System.Drawing.Size(1324, 605);
            this.pnlCiz.TabIndex = 1;
            this.pnlCiz.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pnlCiz_MouseClick);
            this.pnlCiz.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlCiz_MouseDown);
            this.pnlCiz.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlCiz_MouseMove);
            this.pnlCiz.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlCiz_MouseUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.label4.Location = new System.Drawing.Point(3, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 15);
            this.label4.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 16);
            this.label2.TabIndex = 0;
            // 
            // aranacakID
            // 
            this.aranacakID.Location = new System.Drawing.Point(728, 623);
            this.aranacakID.Name = "aranacakID";
            this.aranacakID.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.aranacakID.Size = new System.Drawing.Size(239, 22);
            this.aranacakID.TabIndex = 2;
            this.aranacakID.Text = "Makale ID";
            // 
            // btnCiz
            // 
            this.btnCiz.Location = new System.Drawing.Point(274, 623);
            this.btnCiz.Name = "btnCiz";
            this.btnCiz.Size = new System.Drawing.Size(133, 86);
            this.btnCiz.TabIndex = 3;
            this.btnCiz.Text = "Çiz";
            this.btnCiz.UseVisualStyleBackColor = true;
            this.btnCiz.Click += new System.EventHandler(this.btnCiz_Click);
            // 
            // Analiz
            // 
            this.Analiz.Location = new System.Drawing.Point(151, 623);
            this.Analiz.Name = "Analiz";
            this.Analiz.Size = new System.Drawing.Size(117, 86);
            this.Analiz.TabIndex = 0;
            this.Analiz.Text = "Analiz";
            this.Analiz.UseVisualStyleBackColor = true;
            this.Analiz.Click += new System.EventHandler(this.Analiz_Click);
            // 
            // KCore
            // 
            this.KCore.Location = new System.Drawing.Point(413, 623);
            this.KCore.Name = "KCore";
            this.KCore.Size = new System.Drawing.Size(115, 86);
            this.KCore.TabIndex = 0;
            this.KCore.Text = "KCore";
            this.KCore.UseVisualStyleBackColor = true;
            this.KCore.Click += new System.EventHandler(this.KCore_Click);
            // 
            // KDegeri
            // 
            this.KDegeri.Location = new System.Drawing.Point(728, 687);
            this.KDegeri.Name = "KDegeri";
            this.KDegeri.Size = new System.Drawing.Size(239, 22);
            this.KDegeri.TabIndex = 0;
            this.KDegeri.Text = "K Core Degeri";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1128, 620);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 16);
            this.label3.TabIndex = 1;
            // 
            // Reset
            // 
            this.Reset.Location = new System.Drawing.Point(534, 623);
            this.Reset.Name = "Reset";
            this.Reset.Size = new System.Drawing.Size(124, 86);
            this.Reset.TabIndex = 2;
            this.Reset.Text = "Reset";
            this.Reset.UseVisualStyleBackColor = true;
            this.Reset.Click += new System.EventHandler(this.Reset_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1348, 721);
            this.Controls.Add(this.Reset);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.KDegeri);
            this.Controls.Add(this.KCore);
            this.Controls.Add(this.aranacakID);
            this.Controls.Add(this.Analiz);
            this.Controls.Add(this.btnCiz);
            this.Controls.Add(this.pnlCiz);
            this.Controls.Add(this.button3);
            this.Name = "Form1";
            this.pnlCiz.ResumeLayout(false);
            this.pnlCiz.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnJsonOku;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel pnlCiz;
        private System.Windows.Forms.TextBox aranacakID;
        private System.Windows.Forms.Button btnCiz;
        private System.Windows.Forms.Button Analiz;
        private System.Windows.Forms.Button KCore;
        private System.Windows.Forms.TextBox KDegeri;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Reset;
    }
}
