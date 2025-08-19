using System;
using System.Windows.Forms;
using PromptCad.AdminPanel.Models;

namespace PromptCad.AdminPanel.Forms
{
    public partial class ExtendAPIKeyForm : Form
    {
        public UpdateAPIKeyRequest Result { get; private set; }
        private readonly ApiKeyInfo _apiKeyInfo;

        public ExtendAPIKeyForm(ApiKeyInfo apiKeyInfo)
        {
            _apiKeyInfo = apiKeyInfo;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            lblApiKey = new Label();
            txtApiKey = new TextBox();
            lblCurrentExpiry = new Label();
            txtCurrentExpiry = new TextBox();
            lblDuration = new Label();
            cboDuration = new ComboBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblApiKey
            // 
            lblApiKey.AutoSize = true;
            lblApiKey.Location = new Point(12, 15);
            lblApiKey.Name = "lblApiKey";
            lblApiKey.Size = new Size(62, 20);
            lblApiKey.TabIndex = 0;
            lblApiKey.Text = "API Key:";
            // 
            // txtApiKey
            // 
            txtApiKey.Location = new Point(120, 12);
            txtApiKey.Name = "txtApiKey";
            txtApiKey.ReadOnly = true;
            txtApiKey.Size = new Size(350, 27);
            txtApiKey.TabIndex = 0;
            // 
            // lblCurrentExpiry
            // 
            lblCurrentExpiry.AutoSize = true;
            lblCurrentExpiry.Location = new Point(12, 48);
            lblCurrentExpiry.Name = "lblCurrentExpiry";
            lblCurrentExpiry.Size = new Size(104, 20);
            lblCurrentExpiry.TabIndex = 1;
            lblCurrentExpiry.Text = "Current Expiry:";
            // 
            // txtCurrentExpiry
            // 
            txtCurrentExpiry.Location = new Point(120, 45);
            txtCurrentExpiry.Name = "txtCurrentExpiry";
            txtCurrentExpiry.ReadOnly = true;
            txtCurrentExpiry.Size = new Size(350, 27);
            txtCurrentExpiry.TabIndex = 1;
            // 
            // lblDuration
            // 
            lblDuration.AutoSize = true;
            lblDuration.Location = new Point(12, 81);
            lblDuration.Name = "lblDuration";
            lblDuration.Size = new Size(140, 20);
            lblDuration.TabIndex = 2;
            lblDuration.Text = "Extend by (months):";
            // 
            // cboDuration
            // 
            cboDuration.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDuration.Items.AddRange(new object[] { 3, 6, 12 });
            cboDuration.Location = new Point(158, 78);
            cboDuration.Name = "cboDuration";
            cboDuration.Size = new Size(312, 28);
            cboDuration.TabIndex = 2;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(220, 120);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(100, 35);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(330, 120);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // ExtendAPIKeyForm
            // 
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            ClientSize = new Size(484, 171);
            Controls.Add(lblApiKey);
            Controls.Add(txtApiKey);
            Controls.Add(lblCurrentExpiry);
            Controls.Add(txtCurrentExpiry);
            Controls.Add(lblDuration);
            Controls.Add(cboDuration);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ExtendAPIKeyForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Extend API Key";
            ResumeLayout(false);
            PerformLayout();
        }

        private void LoadData()
        {
            txtApiKey.Text = _apiKeyInfo.ApiKey;
            txtCurrentExpiry.Text = _apiKeyInfo.ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboDuration.SelectedItem == null)
            {
                MessageBox.Show("Please select a duration.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboDuration.Focus();
                return;
            }

            Result = new UpdateAPIKeyRequest
            {
                ApiKey = _apiKeyInfo.ApiKey,
                DurationMonths = (int)cboDuration.SelectedItem
            };
        }

        private Label lblApiKey;
        private TextBox txtApiKey;
        private Label lblCurrentExpiry;
        private TextBox txtCurrentExpiry;
        private Label lblDuration;
        private ComboBox cboDuration;
        private Button btnOK;
        private Button btnCancel;
    }
}
