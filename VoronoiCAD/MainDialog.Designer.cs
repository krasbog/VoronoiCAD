namespace VoronoiCAD
{
    partial class MainDialog
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
            this.textBoxGeoFile = new System.Windows.Forms.TextBox();
            this.buttonSelectGeoFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSelectLiraNewPileFile = new System.Windows.Forms.Button();
            this.textBoxLiraPileFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSelectRzFile = new System.Windows.Forms.Button();
            this.textBoxRzFile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonSelectOrigTxtFile = new System.Windows.Forms.Button();
            this.textBoxOrigTxtFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonSelectResultTxtFile = new System.Windows.Forms.Button();
            this.textBoxResultTxtFile = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxGeoFile
            // 
            this.textBoxGeoFile.Location = new System.Drawing.Point(12, 42);
            this.textBoxGeoFile.Name = "textBoxGeoFile";
            this.textBoxGeoFile.Size = new System.Drawing.Size(621, 20);
            this.textBoxGeoFile.TabIndex = 0;
            // 
            // buttonSelectGeoFile
            // 
            this.buttonSelectGeoFile.Location = new System.Drawing.Point(639, 42);
            this.buttonSelectGeoFile.Name = "buttonSelectGeoFile";
            this.buttonSelectGeoFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectGeoFile.TabIndex = 1;
            this.buttonSelectGeoFile.Text = "...";
            this.buttonSelectGeoFile.UseVisualStyleBackColor = true;
            this.buttonSelectGeoFile.Click += new System.EventHandler(this.buttonSelectGeoFile_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Файл .xls с геологией";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(490, 261);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(63, 24);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(570, 261);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(63, 24);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Файл .txt генерация свай";
            // 
            // buttonSelectLiraNewPileFile
            // 
            this.buttonSelectLiraNewPileFile.Location = new System.Drawing.Point(640, 82);
            this.buttonSelectLiraNewPileFile.Name = "buttonSelectLiraNewPileFile";
            this.buttonSelectLiraNewPileFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectLiraNewPileFile.TabIndex = 5;
            this.buttonSelectLiraNewPileFile.Text = "...";
            this.buttonSelectLiraNewPileFile.UseVisualStyleBackColor = true;
            this.buttonSelectLiraNewPileFile.Click += new System.EventHandler(this.buttonSelectLiraNewPileFile_Click);
            // 
            // textBoxLiraPileFile
            // 
            this.textBoxLiraPileFile.Location = new System.Drawing.Point(13, 82);
            this.textBoxLiraPileFile.Name = "textBoxLiraPileFile";
            this.textBoxLiraPileFile.Size = new System.Drawing.Size(621, 20);
            this.textBoxLiraPileFile.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Файл .xls реакции Rz";
            // 
            // buttonSelectRzFile
            // 
            this.buttonSelectRzFile.Location = new System.Drawing.Point(640, 122);
            this.buttonSelectRzFile.Name = "buttonSelectRzFile";
            this.buttonSelectRzFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectRzFile.TabIndex = 8;
            this.buttonSelectRzFile.Text = "...";
            this.buttonSelectRzFile.UseVisualStyleBackColor = true;
            this.buttonSelectRzFile.Click += new System.EventHandler(this.buttonSelectRzFile_Click);
            // 
            // textBoxRzFile
            // 
            this.textBoxRzFile.Location = new System.Drawing.Point(13, 122);
            this.textBoxRzFile.Name = "textBoxRzFile";
            this.textBoxRzFile.Size = new System.Drawing.Size(621, 20);
            this.textBoxRzFile.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Файл .txt исходный Лира";
            // 
            // buttonSelectOrigTxtFile
            // 
            this.buttonSelectOrigTxtFile.Location = new System.Drawing.Point(640, 162);
            this.buttonSelectOrigTxtFile.Name = "buttonSelectOrigTxtFile";
            this.buttonSelectOrigTxtFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectOrigTxtFile.TabIndex = 11;
            this.buttonSelectOrigTxtFile.Text = "...";
            this.buttonSelectOrigTxtFile.UseVisualStyleBackColor = true;
            this.buttonSelectOrigTxtFile.Click += new System.EventHandler(this.buttonSelectOrigTxtFile_Click);
            // 
            // textBoxOrigTxtFile
            // 
            this.textBoxOrigTxtFile.Location = new System.Drawing.Point(13, 162);
            this.textBoxOrigTxtFile.Name = "textBoxOrigTxtFile";
            this.textBoxOrigTxtFile.Size = new System.Drawing.Size(621, 20);
            this.textBoxOrigTxtFile.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 183);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Файл .txt результат Лира";
            // 
            // buttonSelectResultTxtFile
            // 
            this.buttonSelectResultTxtFile.Location = new System.Drawing.Point(640, 202);
            this.buttonSelectResultTxtFile.Name = "buttonSelectResultTxtFile";
            this.buttonSelectResultTxtFile.Size = new System.Drawing.Size(28, 20);
            this.buttonSelectResultTxtFile.TabIndex = 14;
            this.buttonSelectResultTxtFile.Text = "...";
            this.buttonSelectResultTxtFile.UseVisualStyleBackColor = true;
            this.buttonSelectResultTxtFile.Click += new System.EventHandler(this.buttonSelectResultTxtFile_Click);
            // 
            // textBoxResultTxtFile
            // 
            this.textBoxResultTxtFile.Location = new System.Drawing.Point(13, 202);
            this.textBoxResultTxtFile.Name = "textBoxResultTxtFile";
            this.textBoxResultTxtFile.Size = new System.Drawing.Size(621, 20);
            this.textBoxResultTxtFile.TabIndex = 13;
            // 
            // MainDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(680, 301);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonSelectResultTxtFile);
            this.Controls.Add(this.textBoxResultTxtFile);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonSelectOrigTxtFile);
            this.Controls.Add(this.textBoxOrigTxtFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonSelectRzFile);
            this.Controls.Add(this.textBoxRzFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonSelectLiraNewPileFile);
            this.Controls.Add(this.textBoxLiraPileFile);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSelectGeoFile);
            this.Controls.Add(this.textBoxGeoFile);
            this.Name = "MainDialog";
            this.Text = "VarC1# Свайное поле 2018";
            this.Load += new System.EventHandler(this.MainDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxGeoFile;
        private System.Windows.Forms.Button buttonSelectGeoFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSelectLiraNewPileFile;
        private System.Windows.Forms.TextBox textBoxLiraPileFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSelectRzFile;
        private System.Windows.Forms.TextBox textBoxRzFile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonSelectOrigTxtFile;
        private System.Windows.Forms.TextBox textBoxOrigTxtFile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonSelectResultTxtFile;
        private System.Windows.Forms.TextBox textBoxResultTxtFile;
    }
}