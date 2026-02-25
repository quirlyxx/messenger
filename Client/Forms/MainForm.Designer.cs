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
            btnRenameContact = new Button();
            txtAlias = new TextBox();
            label4 = new Label();
            groupBox2 = new GroupBox();
            flpChat = new FlowLayoutPanel();
            lblTyping = new Label();
            lblStatus = new Label();
            btnSendFile = new Button();
            txtMyNick = new TextBox();
            label3 = new Label();
            btnSaveMyNick = new Button();
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
            // txtMessage
            // 
            txtMessage.BackColor = SystemColors.ActiveCaption;
            txtMessage.Font = new Font("Times New Roman", 10.8F);
            txtMessage.ForeColor = SystemColors.ActiveCaptionText;
            txtMessage.Location = new Point(6, 527);
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(446, 28);
            txtMessage.TabIndex = 3;
            // 
            // btnSend
            // 
            btnSend.BackColor = Color.SteelBlue;
            btnSend.Font = new Font("Times New Roman", 12F);
            btnSend.ForeColor = SystemColors.GradientInactiveCaption;
            btnSend.Location = new Point(458, 519);
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
            lstRequests.Location = new Point(6, 482);
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
            btnAccept.Location = new Point(231, 482);
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
            btnDecline.Location = new Point(231, 523);
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
            btnRemoveContact.Location = new Point(231, 155);
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
            btnLogout.Location = new Point(1067, 535);
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
            label2.Location = new Point(6, 459);
            label2.Name = "label2";
            label2.Size = new Size(135, 20);
            label2.TabIndex = 14;
            label2.Text = "Contact Requests";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnRenameContact);
            groupBox1.Controls.Add(txtAlias);
            groupBox1.Controls.Add(label4);
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
            groupBox1.Size = new Size(381, 580);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            groupBox1.Text = "Contacts";
            // 
            // btnRenameContact
            // 
            btnRenameContact.BackColor = Color.SteelBlue;
            btnRenameContact.Font = new Font("Times New Roman", 12F);
            btnRenameContact.ForeColor = SystemColors.GradientInactiveCaption;
            btnRenameContact.Location = new Point(231, 114);
            btnRenameContact.Name = "btnRenameContact";
            btnRenameContact.Size = new Size(135, 35);
            btnRenameContact.TabIndex = 17;
            btnRenameContact.Text = "Rename Contact";
            btnRenameContact.UseVisualStyleBackColor = false;
            btnRenameContact.Click += btnRenameContact_Click;
            // 
            // txtAlias
            // 
            txtAlias.Location = new Point(6, 320);
            txtAlias.Name = "txtAlias";
            txtAlias.Size = new Size(125, 27);
            txtAlias.TabIndex = 16;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Times New Roman", 10.8F);
            label4.ForeColor = Color.FromArgb(233, 254, 254);
            label4.Location = new Point(6, 297);
            label4.Name = "label4";
            label4.Size = new Size(44, 20);
            label4.TabIndex = 15;
            label4.Text = "Alias";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(flpChat);
            groupBox2.Controls.Add(lblTyping);
            groupBox2.Controls.Add(lblStatus);
            groupBox2.Controls.Add(btnSendFile);
            groupBox2.Controls.Add(txtMessage);
            groupBox2.Controls.Add(btnSend);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(txtTo);
            groupBox2.ForeColor = SystemColors.ActiveCaption;
            groupBox2.Location = new Point(399, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(661, 580);
            groupBox2.TabIndex = 16;
            groupBox2.TabStop = false;
            groupBox2.Text = "Chat";
            // 
            // flpChat
            // 
            flpChat.AutoScroll = true;
            flpChat.FlowDirection = FlowDirection.TopDown;
            flpChat.Location = new Point(9, 57);
            flpChat.Name = "flpChat";
            flpChat.Size = new Size(646, 456);
            flpChat.TabIndex = 8;
            flpChat.WrapContents = false;
            // 
            // lblTyping
            // 
            lblTyping.AutoSize = true;
            lblTyping.Font = new Font("Times New Roman", 10.8F);
            lblTyping.ForeColor = Color.FromArgb(233, 254, 254);
            lblTyping.Location = new Point(379, 26);
            lblTyping.Name = "lblTyping";
            lblTyping.Size = new Size(68, 20);
            lblTyping.TabIndex = 7;
            lblTyping.Text = "typing...";
            lblTyping.Visible = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Times New Roman", 10.8F);
            lblStatus.ForeColor = Color.FromArgb(233, 254, 254);
            lblStatus.Location = new Point(210, 26);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 20);
            lblStatus.TabIndex = 6;
            // 
            // btnSendFile
            // 
            btnSendFile.BackColor = Color.SteelBlue;
            btnSendFile.Font = new Font("Times New Roman", 12F);
            btnSendFile.ForeColor = SystemColors.GradientInactiveCaption;
            btnSendFile.Location = new Point(558, 519);
            btnSendFile.Name = "btnSendFile";
            btnSendFile.Size = new Size(94, 42);
            btnSendFile.TabIndex = 5;
            btnSendFile.Text = "File";
            btnSendFile.UseVisualStyleBackColor = false;
            btnSendFile.Click += btnSendFile_Click;
            // 
            // txtMyNick
            // 
            txtMyNick.Location = new Point(1067, 109);
            txtMyNick.Name = "txtMyNick";
            txtMyNick.Size = new Size(125, 27);
            txtMyNick.TabIndex = 17;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Times New Roman", 10.8F);
            label3.ForeColor = Color.FromArgb(233, 254, 254);
            label3.Location = new Point(1067, 85);
            label3.Name = "label3";
            label3.Size = new Size(86, 20);
            label3.TabIndex = 18;
            label3.Text = "Nickname:";
            // 
            // btnSaveMyNick
            // 
            btnSaveMyNick.BackColor = Color.SteelBlue;
            btnSaveMyNick.Font = new Font("Times New Roman", 12F);
            btnSaveMyNick.ForeColor = SystemColors.GradientInactiveCaption;
            btnSaveMyNick.Location = new Point(1067, 142);
            btnSaveMyNick.Name = "btnSaveMyNick";
            btnSaveMyNick.Size = new Size(126, 40);
            btnSaveMyNick.TabIndex = 19;
            btnSaveMyNick.Text = "Save";
            btnSaveMyNick.UseVisualStyleBackColor = false;
            btnSaveMyNick.Click += btnSaveMyNick_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(1249, 615);
            Controls.Add(btnSaveMyNick);
            Controls.Add(label3);
            Controls.Add(txtMyNick);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnLogout);
            Controls.Add(lblCurrentUser);
            Name = "MainForm";
            Text = "MainForm";
            FormClosing += MainForm_FormClosing;
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
        private TextBox txtMyNick;
        private Label label3;
        private Button btnRenameContact;
        private TextBox txtAlias;
        private Label label4;
        private Button btnSaveMyNick;
        private Label lblTyping;
        private Label lblStatus;
        private FlowLayoutPanel flpChat;
    }
}