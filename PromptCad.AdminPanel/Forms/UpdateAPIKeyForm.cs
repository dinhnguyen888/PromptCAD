using System;
using System.Windows.Forms;
using PromptCad.AdminPanel.Models;

namespace PromptCad.AdminPanel.Forms
{
    public partial class UpdateAPIKeyForm : Form
    {
        public UpdateAPIKeyInfoRequest Result { get; private set; }
        private readonly ApiKeyInfo _apiKeyInfo;

        public UpdateAPIKeyForm(ApiKeyInfo apiKeyInfo)
        {
            _apiKeyInfo = apiKeyInfo;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.lblApiKey = new Label();
            this.txtApiKey = new TextBox();
            this.lblUserName = new Label();
            this.txtUserName = new TextBox();
            this.lblPhoneNumber = new Label();
            this.txtPhoneNumber = new TextBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // lblApiKey
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Location = new System.Drawing.Point(12, 15);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(75, 20);
            this.lblApiKey.Text = "API Key:";

            // txtApiKey
            this.txtApiKey.Location = new System.Drawing.Point(120, 12);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.Size = new System.Drawing.Size(350, 27);
            this.txtApiKey.TabIndex = 0;
            this.txtApiKey.ReadOnly = true;

            // lblUserName
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(12, 48);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(75, 20);
            this.lblUserName.Text = "User Name:";

            // txtUserName
            this.txtUserName.Location = new System.Drawing.Point(120, 45);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(350, 27);
            this.txtUserName.TabIndex = 1;

            // lblPhoneNumber
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(12, 81);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.lblPhoneNumber.Text = "Phone Number:";

            // txtPhoneNumber
            this.txtPhoneNumber.Location = new System.Drawing.Point(120, 78);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(350, 27);
            this.txtPhoneNumber.TabIndex = 2;

            // btnOK
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(220, 120);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 35);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);

            // btnCancel
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(330, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            // UpdateAPIKeyForm
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 171);
            this.Controls.AddRange(new Control[] {
                this.lblApiKey, this.txtApiKey,
                this.lblUserName, this.txtUserName,
                this.lblPhoneNumber, this.txtPhoneNumber,
                this.btnOK, this.btnCancel
            });
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateAPIKeyForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Update API Key Info";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadData()
        {
            txtApiKey.Text = _apiKeyInfo.ApiKey;
            txtUserName.Text = _apiKeyInfo.UserName ?? "";
            txtPhoneNumber.Text = _apiKeyInfo.PhoneNumber ?? "";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Please enter a user name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Please enter a phone number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhoneNumber.Focus();
                return;
            }

            Result = new UpdateAPIKeyInfoRequest
            {
                ApiKey = _apiKeyInfo.ApiKey,
                UserName = txtUserName.Text.Trim(),
                PhoneNumber = txtPhoneNumber.Text.Trim()
            };
        }

        private Label lblApiKey;
        private TextBox txtApiKey;
        private Label lblUserName;
        private TextBox txtUserName;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Button btnOK;
        private Button btnCancel;
    }
}
