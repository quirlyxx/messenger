namespace Client.Forms
{
    partial class LoginForm
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
            btnLogin = new Button();
            btnRegister = new Button();
            Login = new Label();
            label1 = new Label();
            txtLogin = new TextBox();
            txtPassword = new TextBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // btnLogin
            // 
            btnLogin.BackColor = SystemColors.ActiveCaption;
            btnLogin.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            btnLogin.Location = new Point(65, 363);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(129, 48);
            btnLogin.TabIndex = 0;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click;
            // 
            // btnRegister
            // 
            btnRegister.BackColor = SystemColors.ActiveCaption;
            btnRegister.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            btnRegister.Location = new Point(225, 363);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(132, 48);
            btnRegister.TabIndex = 1;
            btnRegister.Text = "Register";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += btnRegister_Click;
            // 
            // Login
            // 
            Login.AutoSize = true;
            Login.BackColor = Color.FromArgb(15, 23, 42);
            Login.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            Login.ForeColor = Color.FromArgb(233, 254, 254);
            Login.Location = new Point(65, 118);
            Login.Name = "Login";
            Login.Size = new Size(85, 32);
            Login.TabIndex = 2;
            Login.Text = "Login";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.ForeColor = Color.FromArgb(233, 254, 254);
            label1.Location = new Point(65, 204);
            label1.Name = "label1";
            label1.Size = new Size(129, 32);
            label1.TabIndex = 3;
            label1.Text = "Password";
            // 
            // txtLogin
            // 
            txtLogin.BackColor = SystemColors.ActiveCaption;
            txtLogin.Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            txtLogin.ForeColor = SystemColors.WindowText;
            txtLogin.Location = new Point(232, 124);
            txtLogin.Name = "txtLogin";
            txtLogin.Size = new Size(125, 30);
            txtLogin.TabIndex = 4;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = SystemColors.ActiveCaption;
            txtPassword.Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            txtPassword.ForeColor = SystemColors.WindowText;
            txtPassword.Location = new Point(232, 209);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(125, 30);
            txtPassword.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(15, 23, 42);
            label2.Font = new Font("Times New Roman", 19.8000011F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label2.ForeColor = Color.PaleTurquoise;
            label2.Location = new Point(124, 27);
            label2.Name = "label2";
            label2.Size = new Size(160, 38);
            label2.TabIndex = 6;
            label2.Text = "Welcome!";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(441, 473);
            Controls.Add(label2);
            Controls.Add(txtPassword);
            Controls.Add(txtLogin);
            Controls.Add(label1);
            Controls.Add(Login);
            Controls.Add(btnRegister);
            Controls.Add(btnLogin);
            Name = "LoginForm";
            Text = "LoginForm";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnLogin;
        private Button btnRegister;
        private Label Login;
        private Label label1;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Label label2;
    }
}