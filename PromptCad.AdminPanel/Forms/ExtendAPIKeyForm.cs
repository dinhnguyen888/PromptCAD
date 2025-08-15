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
            this.lblApiKey = new Label();
            this.txtApiKey = new TextBox();
            this.lblCurrentExpiry = new Label();
            this.txtCurrentExpiry = new TextBox();
            this.lblDuration = new Label();
            this.cboDuration = new ComboBox();
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

            // lblCurrentExpiry
            this.lblCurrentExpiry.AutoSize = true;
            this.lblCurrentExpiry.Location = new System.Drawing.Point(12, 48);
            this.lblCurrentExpiry.Name = "lblCurrentExpiry";
            this.lblCurrentExpiry.Size = new System.Drawing.Size(100, 20);
            this.lblCurrentExpiry.Text = "Current Expiry:";

            // txtCurrentExpiry
            this.txtCurrentExpiry.Location = new System.Drawing.Point(120, 45);
            this.txtCurrentExpiry.Name = "txtCurrentExpiry";
            this.txtCurrentExpiry.Size = new System.Drawing.Size(350, 27);
            this.txtCurrentExpiry.TabIndex = 1;
            this.txtCurrentExpiry.ReadOnly = true;

            // lblDuration
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(12, 81);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(100, 20);
            this.lblDuration.Text = "Extend by (months):";

            // cboDuration
            this.cboDuration.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboDuration.Location = new System.Drawing.Point(120, 78);
            this.cboDuration.Name = "cboDuration";
            this.cboDuration.Size = new System.Drawing.Size(350, 28);
            this.cboDuration.TabIndex = 2;
            this.cboDuration.Items.AddRange(new object[] { 3, 6, 12 });

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

            // ExtendAPIKeyForm
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(484, 171);
            this.Controls.AddRange(new Control[] {
                this.lblApiKey, this.txtApiKey,
                this.lblCurrentExpiry, this.txtCurrentExpiry,
                this.lblDuration, this.cboDuration,
                this.btnOK, this.btnCancel
            });
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExtendAPIKeyForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Extend API Key";
            this.ResumeLayout(false);
            this.PerformLayout();

            // Set default values
            this.cboDuration.SelectedIndex = 0;
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
