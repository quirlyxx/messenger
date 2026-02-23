namespace Client.Forms
{
    partial class MainForm
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
            label1 = new Label();
            txtTo = new TextBox();
            lstMessage = new ListBox();
            txtMessage = new TextBox();
            btnSend = new Button();
            lblCurrentUser = new Label();
            lstContacts = new ListBox();
            lstRequests = new ListBox();
            txtAddContact = new TextBox();
            btnSendRequest = new Button();
            btnAccept = new Button();
            btnDecline = new Button();
            btnRemoveContact = new Button();
            btnLogout = new Button();
            label2 = new Label();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            btnSendFile = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 10.8F);
            label1.ForeColor = Color.FromArgb(233, 254, 254);
            label1.Location = new Point(6, 26);
            label1.Name = "label1";
            label1.Size = new Size(73, 20);
            label1.TabIndex = 0;
            label1.Text = "Send to: ";
            // 
            // txtTo
            // 
            txtTo.BackColor = SystemColors.ActiveCaption;
            txtTo.Font = new Font("Times New Roman", 10.8F);
            txtTo.ForeColor = SystemColors.ActiveCaptionText;
            txtTo.Location = new Point(79, 23);
            txtTo.Name = "txtTo";
            txtTo.Size = new Size(125, 28);
            txtTo.TabIndex = 1;
            // 
            // lstMessage
            // 
            lstMessage.BackColor = Color.FromArgb(47, 84, 145);
            lstMessage.Font = new Font("Times New Roman", 12F);
            lstMessage.ForeColor = Color.FromArgb(233, 254, 254);
            lstMessage.FormattingEnabled = true;
            lstMessage.Location = new Point(6, 56);
            lstMessage.MultiColumn = true;
            lstMessage.Name = "lstMessage";
            lstMessage.Size = new Size(646, 334);
            lstMessage.TabIndex = 2;
            // 
            // txtMessage
            // 
            txtMessage.BackColor = SystemColors.ActiveCaption;
            txtMessage.Font = new Font("Times New Roman", 10.8F);
            txtMessage.ForeColor = SystemColors.ActiveCaptionText;
            txtMessage.Location = new Point(6, 412);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(125, 28);
            txtMessage.TabIndex = 3;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.SteelBlue;
            btnSend.Font = new Font("Times New Roman", 12F);
            btnSend.ForeColor = SystemColors.GradientInactiveCaption;
            btnSend.Location = new Point(137, 404);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(94, 42);
            btnSend.TabIndex = 4;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // lblCurrentUser
            // 
            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Font = new Font("Times New Roman", 10.8F);
            lblCurrentUser.ForeColor = Color.FromArgb(233, 254, 254);
            lblCurrentUser.Location = new Point(1066, 38);
            lblCurrentUser.Name = "lblCurrentUser";
            lblCurrentUser.Size = new Size(42, 20);
            lblCurrentUser.TabIndex = 5;
            lblCurrentUser.Text = "You:";
            // 
            // lstContacts
            // 
            lstContacts.BackColor = Color.FromArgb(47, 84, 145);
            lstContacts.Font = new Font("Times New Roman", 12F);
            lstContacts.ForeColor = Color.FromArgb(233, 254, 254);
            lstContacts.FormattingEnabled = true;
            lstContacts.Location = new Point(6, 26);
            lstContacts.MultiColumn = true;
            lstContacts.Name = "lstContacts";
            lstContacts.Size = new Size(219, 268);
            lstContacts.TabIndex = 6;
            lstContacts.SelectedIndexChanged += lstContacts_SelectedIndexChanged;
            // 
            // lstRequests
            // 
            lstRequests.BackColor = Color.FromArgb(47, 84, 145);
            lstRequests.Font = new Font("Times New Roman", 12F);
            lstRequests.ForeColor = Color.FromArgb(233, 254, 254);
            lstRequests.FormattingEnabled = true;
            lstRequests.Location = new Point(6, 341);
            lstRequests.MultiColumn = true;
            lstRequests.Name = "lstRequests";
            lstRequests.Size = new Size(219, 92);
            lstRequests.TabIndex = 7;
            // 
            // txtAddContact
            // 
            txtAddContact.BackColor = SystemColors.ActiveCaption;
            txtAddContact.Font = new Font("Times New Roman", 10.8F);
            txtAddContact.ForeColor = SystemColors.ActiveCaptionText;
            txtAddContact.Location = new Point(231, 26);
            txtAddContact.Name = "txtAddContact";
            txtAddContact.Size = new Size(135, 28);
            txtAddContact.TabIndex = 8;
            // 
            // btnSendRequest
            // 
            btnSendRequest.BackColor = Color.SteelBlue;
            btnSendRequest.Font = new Font("Times New Roman", 12F);
            btnSendRequest.ForeColor = SystemColors.GradientInactiveCaption;
            btnSendRequest.Location = new Point(231, 73);
            btnSendRequest.Name = "btnSendRequest";
            btnSendRequest.Size = new Size(135, 35);
            btnSendRequest.TabIndex = 9;
            btnSendRequest.Text = "Send Request";
            btnSendRequest.UseVisualStyleBackColor = false;
            btnSendRequest.Click += btnSendRequest_Click;
            // 
            // btnAccept
            // 
            btnAccept.BackColor = Color.SteelBlue;
            btnAccept.Font = new Font("Times New Roman", 12F);
            btnAccept.ForeColor = SystemColors.GradientInactiveCaption;
            btnAccept.Location = new Point(231, 337);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(135, 35);
            btnAccept.TabIndex = 10;
            btnAccept.Text = "Accept";
            btnAccept.UseVisualStyleBackColor = false;
            btnAccept.Click += btnAccept_Click;
            // 
            // btnDecline
            // 
            btnDecline.BackColor = Color.SteelBlue;
            btnDecline.Font = new Font("Times New Roman", 12F);
            btnDecline.ForeColor = SystemColors.GradientInactiveCaption;
            btnDecline.Location = new Point(231, 378);
            btnDecline.Name = "btnDecline";
            btnDecline.Size = new Size(135, 35);
            btnDecline.TabIndex = 11;
            btnDecline.Text = "Decline";
            btnDecline.UseVisualStyleBackColor = false;
            btnDecline.Click += btnDecline_Click;
            // 
            // btnRemoveContact
            // 
            btnRemoveContact.BackColor = Color.SteelBlue;
            btnRemoveContact.Font = new Font("Times New Roman", 12F);
            btnRemoveContact.ForeColor = SystemColors.GradientInactiveCaption;
            btnRemoveContact.Location = new Point(231, 114);
            btnRemoveContact.Name = "btnRemoveContact";
            btnRemoveContact.Size = new Size(135, 35);
            btnRemoveContact.TabIndex = 12;
            btnRemoveContact.Text = "Remove Contact";
            btnRemoveContact.UseVisualStyleBackColor = false;
            btnRemoveContact.Click += btnRemoveContact_Click;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.SteelBlue;
            btnLogout.Font = new Font("Times New Roman", 12F);
            btnLogout.ForeColor = SystemColors.GradientInactiveCaption;
            btnLogout.Location = new Point(1066, 430);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(126, 40);
            btnLogout.TabIndex = 13;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Times New Roman", 10.8F);
            label2.ForeColor = Color.FromArgb(233, 254, 254);
            label2.Location = new Point(6, 316);
            label2.Name = "label2";
            label2.Size = new Size(135, 20);
            label2.TabIndex = 14;
            label2.Text = "Contact Requests";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lstContacts);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnRemoveContact);
            groupBox1.Controls.Add(lstRequests);
            groupBox1.Controls.Add(btnSendRequest);
            groupBox1.Controls.Add(btnDecline);
            groupBox1.Controls.Add(txtAddContact);
            groupBox1.Controls.Add(btnAccept);
            groupBox1.ForeColor = SystemColors.ActiveCaption;
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(381, 458);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "Contacts";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnSendFile);
            groupBox2.Controls.Add(lstMessage);
            groupBox2.Controls.Add(txtMessage);
            groupBox2.Controls.Add(btnSend);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(txtTo);
            groupBox2.ForeColor = SystemColors.ActiveCaption;
            groupBox2.Location = new Point(399, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(661, 458);
            groupBox2.TabIndex = 16;
            groupBox2.TabStop = false;
            groupBox2.Text = "Chat";
            // 
            // btnSendFile
            // 
            btnSendFile.BackColor = Color.SteelBlue;
            btnSendFile.Font = new Font("Times New Roman", 12F);
            btnSendFile.ForeColor = SystemColors.GradientInactiveCaption;
            btnSendFile.Location = new Point(237, 404);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(94, 42);
            btnSendFile.TabIndex = 5;
            btnSendFile.Text = "File";
            btnSendFile.UseVisualStyleBackColor = false;
            btnSendFile.Click += btnSendFile_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(1204, 492);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnLogout);
            Controls.Add(lblCurrentUser);
            Name = "MainForm";
            Text = "MainForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtTo;
        private ListBox lstMessage;
        private TextBox txtMessage;
        private Button btnSend;
        private Label lblCurrentUser;
        private ListBox lstContacts;
        private ListBox lstRequests;
        private TextBox txtAddContact;
        private Button btnSendRequest;
        private Button btnAccept;
        private Button btnDecline;
        private Button btnRemoveContact;
        private Button btnLogout;
        private Label label2;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btnSendFile;
    }
}