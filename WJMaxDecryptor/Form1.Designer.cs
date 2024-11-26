namespace WJMaxDecryptor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            keyInput = new TextBox();
            label1 = new Label();
            encryptBtn = new Button();
            decryptBtn = new Button();
            progressBar = new ProgressBar();
            statusLabel = new Label();
            SuspendLayout();
            // 
            // keyInput
            // 
            keyInput.Location = new Point(12, 27);
            keyInput.Name = "keyInput";
            keyInput.Size = new Size(360, 23);
            keyInput.TabIndex = 0;
            keyInput.Text = "99FLKWJFL;l99r7@!()f09sodkjfs;a;o9fU#@";
            keyInput.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(114, 15);
            label1.TabIndex = 1;
            label1.Text = "WJMax 암/복호화기";
            // 
            // encryptBtn
            // 
            encryptBtn.Location = new Point(216, 56);
            encryptBtn.Name = "encryptBtn";
            encryptBtn.Size = new Size(75, 23);
            encryptBtn.TabIndex = 2;
            encryptBtn.Text = "암호화";
            encryptBtn.UseVisualStyleBackColor = true;
            encryptBtn.Click += encryptBtn_Click;
            // 
            // decryptBtn
            // 
            decryptBtn.Location = new Point(297, 56);
            decryptBtn.Name = "decryptBtn";
            decryptBtn.Size = new Size(75, 23);
            decryptBtn.TabIndex = 3;
            decryptBtn.Text = "복호화";
            decryptBtn.UseVisualStyleBackColor = true;
            decryptBtn.Click += decryptBtn_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 100);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(360, 23);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 4;
            // 
            // statusLabel
            // 
            statusLabel.Location = new Point(12, 130);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(360, 40);
            statusLabel.TabIndex = 5;
            statusLabel.AutoSize = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(384, 180);
            Controls.Add(decryptBtn);
            Controls.Add(encryptBtn);
            Controls.Add(label1);
            Controls.Add(keyInput);
            Controls.Add(progressBar);
            Controls.Add(statusLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Form1";
            Text = "WJMax 암/복호화기";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox keyInput;
        private Label label1;
        private Button encryptBtn;
        private Button decryptBtn;
        private ProgressBar progressBar;
        private Label statusLabel;
    }
}
