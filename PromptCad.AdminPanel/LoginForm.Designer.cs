namespace PromptCad.AdminPanel
{
    partial class LoginForm
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
            EmailTb = new TextBox();
            PasswordTb = new TextBox();
            LoginBtn = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // EmailTb
            // 
            EmailTb.Font = new Font("Arial", 10.8F);
            EmailTb.Location = new Point(24, 111);
            EmailTb.Name = "EmailTb";
            EmailTb.Size = new Size(604, 28);
            EmailTb.TabIndex = 0;
            // 
            // PasswordTb
            // 
            PasswordTb.Font = new Font("Arial", 10.8F);
            PasswordTb.Location = new Point(26, 175);
            PasswordTb.Name = "PasswordTb";
            PasswordTb.Size = new Size(604, 28);
            PasswordTb.TabIndex = 1;
            // 
            // LoginBtn
            // 
            LoginBtn.Font = new Font("Arial", 10.8F);
            LoginBtn.Location = new Point(24, 231);
            LoginBtn.Name = "LoginBtn";
            LoginBtn.Size = new Size(176, 37);
            LoginBtn.TabIndex = 2;
            LoginBtn.Text = "Đăng nhập";
            LoginBtn.UseVisualStyleBackColor = true;
            LoginBtn.Click += LoginBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 88);
            label1.Name = "label1";
            label1.Size = new Size(71, 20);
            label1.TabIndex = 3;
            label1.Text = "Tài khoản";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(26, 152);
            label2.Name = "label2";
            label2.Size = new Size(74, 20);
            label2.TabIndex = 4;
            label2.Text = "Mật khẩu ";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(LoginBtn);
            Controls.Add(PasswordTb);
            Controls.Add(EmailTb);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox EmailTb;
        private TextBox PasswordTb;
        private Button LoginBtn;
        private Label label1;
        private Label label2;
    }
}
