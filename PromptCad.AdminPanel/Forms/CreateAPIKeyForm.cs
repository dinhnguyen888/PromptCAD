using System;
using System.Windows.Forms;
using PromptCad.AdminPanel.Models;

namespace PromptCad.AdminPanel.Forms
{
    public partial class CreateAPIKeyForm : Form
    {
        public CreateAPIKeyRequest Result { get; private set; }

        public CreateAPIKeyForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblUserName = new Label();
            this.txtUserName = new TextBox();
            this.lblPhoneNumber = new Label();
            this.txtPhoneNumber = new TextBox();
            this.lblDuration = new Label();
            this.cboDuration = new ComboBox();
            this.btnOK = new Button();
            this.btnCancel = new Button();
            this.SuspendLayout();

            // lblUserName
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(12, 15);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(75, 20);
            this.lblUserName.Text = "User Name:";

            // txtUserName
            this.txtUserName.Location = new System.Drawing.Point(120, 12);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(250, 27);
            this.txtUserName.TabIndex = 0;

            // lblPhoneNumber
            this.lblPhoneNumber.AutoSize = true;
            this.lblPhoneNumber.Location = new System.Drawing.Point(12, 48);
            this.lblPhoneNumber.Name = "lblPhoneNumber";
            this.lblPhoneNumber.Size = new System.Drawing.Size(100, 20);
            this.lblPhoneNumber.Text = "Phone Number:";

            // txtPhoneNumber
            this.txtPhoneNumber.Location = new System.Drawing.Point(120, 45);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(250, 27);
            this.txtPhoneNumber.TabIndex = 1;

            // lblDuration
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(12, 81);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(100, 20);
            this.lblDuration.Text = "Duration (months):";

            // cboDuration
            this.cboDuration.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboDuration.Location = new System.Drawing.Point(120, 78);
            this.cboDuration.Name = "cboDuration";
            this.cboDuration.Size = new System.Drawing.Size(250, 28);
            this.cboDuration.TabIndex = 2;
            this.cboDuration.Items.AddRange(new object[] { 3, 6, 12 });

            // btnOK
            this.btnOK.DialogResult = DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(120, 120);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 35);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new EventHandler(this.btnOK_Click);

            // btnCancel
            this.btnCancel.DialogResult = DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(230, 120);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            // CreateAPIKeyForm
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 171);
            this.Controls.AddRange(new Control[] {
                this.lblUserName, this.txtUserName,
                this.lblPhoneNumber, this.txtPhoneNumber,
                this.lblDuration, this.cboDuration,
                this.btnOK, this.btnCancel
            });
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateAPIKeyForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Create API Key";
            this.ResumeLayout(false);
            this.PerformLayout();

            // Set default values
            this.cboDuration.SelectedIndex = 0;
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

            if (cboDuration.SelectedItem == null)
            {
                MessageBox.Show("Please select a duration.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboDuration.Focus();
                return;
            }

            Result = new CreateAPIKeyRequest
            {
                user_name = txtUserName.Text.Trim(),
                phone_number = txtPhoneNumber.Text.Trim(),
                duration_months = (int)cboDuration.SelectedItem
            };
        }

        private Label lblUserName;
        private TextBox txtUserName;
        private Label lblPhoneNumber;
        private TextBox txtPhoneNumber;
        private Label lblDuration;
        private ComboBox cboDuration;
        private Button btnOK;
        private Button btnCancel;
    }
}
