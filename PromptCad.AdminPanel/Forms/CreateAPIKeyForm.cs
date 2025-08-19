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
            lblUserName = new Label();
            txtUserName = new TextBox();
            lblPhoneNumber = new Label();
            txtPhoneNumber = new TextBox();
            lblDuration = new Label();
            cboDuration = new ComboBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Location = new Point(12, 15);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(85, 20);
            lblUserName.TabIndex = 0;
            lblUserName.Text = "User Name:";
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(120, 12);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(250, 27);
            txtUserName.TabIndex = 0;
            // 
            // lblPhoneNumber
            // 
            lblPhoneNumber.AutoSize = true;
            lblPhoneNumber.Location = new Point(12, 48);
            lblPhoneNumber.Name = "lblPhoneNumber";
            lblPhoneNumber.Size = new Size(111, 20);
            lblPhoneNumber.TabIndex = 1;
            lblPhoneNumber.Text = "Phone Number:";
            // 
            // txtPhoneNumber
            // 
            txtPhoneNumber.Location = new Point(120, 45);
            txtPhoneNumber.Name = "txtPhoneNumber";
            txtPhoneNumber.Size = new Size(250, 27);
            txtPhoneNumber.TabIndex = 1;
            // 
            // lblDuration
            // 
            lblDuration.AutoSize = true;
            lblDuration.Location = new Point(12, 81);
            lblDuration.Name = "lblDuration";
            lblDuration.Size = new Size(133, 20);
            lblDuration.TabIndex = 2;
            lblDuration.Text = "Duration (months):";
            // 
            // cboDuration
            // 
            cboDuration.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDuration.Items.AddRange(new object[] { 3, 6, 12 });
            cboDuration.Location = new Point(142, 78);
            cboDuration.Name = "cboDuration";
            cboDuration.Size = new Size(228, 28);
            cboDuration.TabIndex = 2;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(120, 120);
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
            btnCancel.Location = new Point(230, 120);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // CreateAPIKeyForm
            // 
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            ClientSize = new Size(384, 171);
            Controls.Add(lblUserName);
            Controls.Add(txtUserName);
            Controls.Add(lblPhoneNumber);
            Controls.Add(txtPhoneNumber);
            Controls.Add(lblDuration);
            Controls.Add(cboDuration);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CreateAPIKeyForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create API Key";
            ResumeLayout(false);
            PerformLayout();
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
