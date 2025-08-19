namespace PromptCad.AdminPanel
{
    partial class DashboardForm
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
            Panel panel1;
            UpdateAPIKeyBtn = new Button();
            SearchAPITextbox = new TextBox();
            SearchApiBtn = new Button();
            DeleteApiBtn = new Button();
            ExtendAPIBtn = new Button();
            CreateApiBtn = new Button();
            PromptPage = new TabPage();
            panel4 = new Panel();
            PromptGridView = new DataGridView();
            panel3 = new Panel();
            label1 = new Label();
            CountPromptLabel = new Label();
            DeletePromptBtn = new Button();
            GetShapeFileBtn = new Button();
            ImportExcelBtn = new Button();
            ExportExcelBtn = new Button();
            APIKeyPage = new TabPage();
            panel2 = new Panel();
            ApiKeyGridView = new DataGridView();
            tabControl1 = new TabControl();
            AdminPage = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            LogoutBtn = new Button();
            ChangeAPIGeminiKeyBtn = new Button();
            ViewAIModelBtn = new Button();
            RefreshBtn = new Button();
            panel1 = new Panel();
            panel1.SuspendLayout();
            PromptPage.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PromptGridView).BeginInit();
            panel3.SuspendLayout();
            APIKeyPage.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ApiKeyGridView).BeginInit();
            tabControl1.SuspendLayout();
            AdminPage.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(UpdateAPIKeyBtn);
            panel1.Controls.Add(SearchAPITextbox);
            panel1.Controls.Add(SearchApiBtn);
            panel1.Controls.Add(DeleteApiBtn);
            panel1.Controls.Add(ExtendAPIBtn);
            panel1.Controls.Add(CreateApiBtn);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(912, 75);
            panel1.TabIndex = 1;
            // 
            // UpdateAPIKeyBtn
            // 
            UpdateAPIKeyBtn.Font = new Font("Arial", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            UpdateAPIKeyBtn.Location = new Point(555, 38);
            UpdateAPIKeyBtn.Name = "UpdateAPIKeyBtn";
            UpdateAPIKeyBtn.Size = new Size(184, 29);
            UpdateAPIKeyBtn.TabIndex = 5;
            UpdateAPIKeyBtn.Text = "Sửa Info APIKey";
            UpdateAPIKeyBtn.UseVisualStyleBackColor = true;
            UpdateAPIKeyBtn.Click += UpdateAPIKeyBtn_Click;
            // 
            // SearchAPITextbox
            // 
            SearchAPITextbox.Location = new Point(5, 5);
            SearchAPITextbox.Name = "SearchAPITextbox";
            SearchAPITextbox.Size = new Size(734, 27);
            SearchAPITextbox.TabIndex = 4;
            // 
            // SearchApiBtn
            // 
            SearchApiBtn.Font = new Font("Arial", 10.2F, FontStyle.Bold);
            SearchApiBtn.ForeColor = Color.Purple;
            SearchApiBtn.Location = new Point(745, 5);
            SearchApiBtn.Name = "SearchApiBtn";
            SearchApiBtn.Size = new Size(153, 29);
            SearchApiBtn.TabIndex = 3;
            SearchApiBtn.Text = "Search";
            SearchApiBtn.UseVisualStyleBackColor = true;
            SearchApiBtn.Click += SearchApiBtn_Click;
            // 
            // DeleteApiBtn
            // 
            DeleteApiBtn.Font = new Font("Arial", 10.2F, FontStyle.Bold);
            DeleteApiBtn.ForeColor = Color.Red;
            DeleteApiBtn.Location = new Point(396, 38);
            DeleteApiBtn.Name = "DeleteApiBtn";
            DeleteApiBtn.Size = new Size(153, 29);
            DeleteApiBtn.TabIndex = 2;
            DeleteApiBtn.Text = "Xoá APIKey";
            DeleteApiBtn.UseVisualStyleBackColor = true;
            DeleteApiBtn.Click += DeleteApiBtn_Click;
            // 
            // ExtendAPIBtn
            // 
            ExtendAPIBtn.Font = new Font("Arial", 10.2F, FontStyle.Bold);
            ExtendAPIBtn.ForeColor = Color.Lime;
            ExtendAPIBtn.Location = new Point(5, 38);
            ExtendAPIBtn.Name = "ExtendAPIBtn";
            ExtendAPIBtn.Size = new Size(182, 29);
            ExtendAPIBtn.TabIndex = 1;
            ExtendAPIBtn.Text = "Gia hạn APIKey";
            ExtendAPIBtn.UseVisualStyleBackColor = true;
            ExtendAPIBtn.Click += ExtendAPIBtn_Click;
            // 
            // CreateApiBtn
            // 
            CreateApiBtn.Font = new Font("Arial", 10.2F, FontStyle.Bold);
            CreateApiBtn.ForeColor = SystemColors.ActiveCaption;
            CreateApiBtn.Location = new Point(193, 38);
            CreateApiBtn.Name = "CreateApiBtn";
            CreateApiBtn.Size = new Size(197, 29);
            CreateApiBtn.TabIndex = 0;
            CreateApiBtn.Text = "Thêm APIKey";
            CreateApiBtn.UseVisualStyleBackColor = true;
            CreateApiBtn.Click += CreateApiBtn_Click;
            // 
            // PromptPage
            // 
            PromptPage.Controls.Add(panel4);
            PromptPage.Controls.Add(panel3);
            PromptPage.Location = new Point(4, 28);
            PromptPage.Name = "PromptPage";
            PromptPage.Padding = new Padding(3);
            PromptPage.Size = new Size(918, 601);
            PromptPage.TabIndex = 3;
            PromptPage.Text = "Prompt";
            PromptPage.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Controls.Add(PromptGridView);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(3, 111);
            panel4.Name = "panel4";
            panel4.Size = new Size(912, 487);
            panel4.TabIndex = 1;
            // 
            // PromptGridView
            // 
            PromptGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            PromptGridView.Dock = DockStyle.Fill;
            PromptGridView.Location = new Point(0, 0);
            PromptGridView.Name = "PromptGridView";
            PromptGridView.RowHeadersWidth = 51;
            PromptGridView.Size = new Size(912, 487);
            PromptGridView.TabIndex = 0;
            // 
            // panel3
            // 
            panel3.Controls.Add(label1);
            panel3.Controls.Add(CountPromptLabel);
            panel3.Controls.Add(DeletePromptBtn);
            panel3.Controls.Add(GetShapeFileBtn);
            panel3.Controls.Add(ImportExcelBtn);
            panel3.Controls.Add(ExportExcelBtn);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(3, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(912, 108);
            panel3.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(49, 63);
            label1.Name = "label1";
            label1.Size = new Size(314, 19);
            label1.TabIndex = 5;
            label1.Text = "Prompts của user đang ở trong database.";
            // 
            // CountPromptLabel
            // 
            CountPromptLabel.AutoSize = true;
            CountPromptLabel.Location = new Point(14, 63);
            CountPromptLabel.Name = "CountPromptLabel";
            CountPromptLabel.Size = new Size(29, 19);
            CountPromptLabel.TabIndex = 4;
            CountPromptLabel.Text = "....";
            // 
            // DeletePromptBtn
            // 
            DeletePromptBtn.ForeColor = Color.Red;
            DeletePromptBtn.Location = new Point(364, 58);
            DeletePromptBtn.Name = "DeletePromptBtn";
            DeletePromptBtn.Size = new Size(94, 29);
            DeletePromptBtn.TabIndex = 3;
            DeletePromptBtn.Text = "Xoá ?";
            DeletePromptBtn.UseVisualStyleBackColor = true;
            DeletePromptBtn.Click += DeletePromptBtn_Click;
            // 
            // GetShapeFileBtn
            // 
            GetShapeFileBtn.Location = new Point(617, 3);
            GetShapeFileBtn.Name = "GetShapeFileBtn";
            GetShapeFileBtn.Size = new Size(292, 29);
            GetShapeFileBtn.TabIndex = 2;
            GetShapeFileBtn.Text = "lấy nội dung file RAG (shapes.txt)";
            GetShapeFileBtn.UseVisualStyleBackColor = true;
            GetShapeFileBtn.Click += GetShapeFileBtn_Click;
            // 
            // ImportExcelBtn
            // 
            ImportExcelBtn.Location = new Point(617, 38);
            ImportExcelBtn.Name = "ImportExcelBtn";
            ImportExcelBtn.Size = new Size(290, 29);
            ImportExcelBtn.TabIndex = 1;
            ImportExcelBtn.Text = "Import excel prompt vào để RAG";
            ImportExcelBtn.UseVisualStyleBackColor = true;
            ImportExcelBtn.Click += ImportExcelBtn_Click;
            // 
            // ExportExcelBtn
            // 
            ExportExcelBtn.Location = new Point(5, 3);
            ExportExcelBtn.Name = "ExportExcelBtn";
            ExportExcelBtn.Size = new Size(203, 29);
            ExportExcelBtn.TabIndex = 0;
            ExportExcelBtn.Text = "Xuất ra file Excel";
            ExportExcelBtn.UseVisualStyleBackColor = true;
            ExportExcelBtn.Click += ExportExcelBtn_Click;
            // 
            // APIKeyPage
            // 
            APIKeyPage.Controls.Add(panel2);
            APIKeyPage.Controls.Add(panel1);
            APIKeyPage.Font = new Font("Arial", 10.2F);
            APIKeyPage.Location = new Point(4, 28);
            APIKeyPage.Name = "APIKeyPage";
            APIKeyPage.Padding = new Padding(3);
            APIKeyPage.Size = new Size(918, 601);
            APIKeyPage.TabIndex = 1;
            APIKeyPage.Text = "List API Key";
            APIKeyPage.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            panel2.Controls.Add(ApiKeyGridView);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(3, 78);
            panel2.Name = "panel2";
            panel2.Size = new Size(912, 520);
            panel2.TabIndex = 2;
            // 
            // ApiKeyGridView
            // 
            ApiKeyGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            ApiKeyGridView.Dock = DockStyle.Fill;
            ApiKeyGridView.Location = new Point(0, 0);
            ApiKeyGridView.Name = "ApiKeyGridView";
            ApiKeyGridView.RowHeadersWidth = 51;
            ApiKeyGridView.Size = new Size(912, 520);
            ApiKeyGridView.TabIndex = 2;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(APIKeyPage);
            tabControl1.Controls.Add(PromptPage);
            tabControl1.Controls.Add(AdminPage);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Font = new Font("Arial", 10.2F);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(926, 633);
            tabControl1.TabIndex = 0;
            // 
            // AdminPage
            // 
            AdminPage.Controls.Add(tableLayoutPanel1);
            AdminPage.Font = new Font("Arial", 10.2F);
            AdminPage.Location = new Point(4, 28);
            AdminPage.Name = "AdminPage";
            AdminPage.Padding = new Padding(3);
            AdminPage.Size = new Size(918, 601);
            AdminPage.TabIndex = 2;
            AdminPage.Text = "Admin Feature";
            AdminPage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(LogoutBtn, 0, 0);
            tableLayoutPanel1.Controls.Add(ChangeAPIGeminiKeyBtn, 1, 0);
            tableLayoutPanel1.Controls.Add(ViewAIModelBtn, 0, 1);
            tableLayoutPanel1.Controls.Add(RefreshBtn, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(912, 595);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // LogoutBtn
            // 
            LogoutBtn.Dock = DockStyle.Fill;
            LogoutBtn.Location = new Point(3, 3);
            LogoutBtn.Name = "LogoutBtn";
            LogoutBtn.Size = new Size(450, 291);
            LogoutBtn.TabIndex = 0;
            LogoutBtn.Text = "Đăng Xuất";
            LogoutBtn.UseVisualStyleBackColor = true;
            LogoutBtn.Click += LogoutBtn_Click;
            // 
            // ChangeAPIGeminiKeyBtn
            // 
            ChangeAPIGeminiKeyBtn.Dock = DockStyle.Fill;
            ChangeAPIGeminiKeyBtn.Location = new Point(459, 3);
            ChangeAPIGeminiKeyBtn.Name = "ChangeAPIGeminiKeyBtn";
            ChangeAPIGeminiKeyBtn.Size = new Size(450, 291);
            ChangeAPIGeminiKeyBtn.TabIndex = 1;
            ChangeAPIGeminiKeyBtn.Text = "Đổi Gemini ApiKey";
            ChangeAPIGeminiKeyBtn.UseVisualStyleBackColor = true;
            ChangeAPIGeminiKeyBtn.Click += ChangeAPIGeminiKeyBtn_Click;
            // 
            // ViewAIModelBtn
            // 
            ViewAIModelBtn.Dock = DockStyle.Fill;
            ViewAIModelBtn.Location = new Point(3, 300);
            ViewAIModelBtn.Name = "ViewAIModelBtn";
            ViewAIModelBtn.Size = new Size(450, 292);
            ViewAIModelBtn.TabIndex = 2;
            ViewAIModelBtn.Text = "Xem AI Model";
            ViewAIModelBtn.UseVisualStyleBackColor = true;
            ViewAIModelBtn.Click += ViewAIModelBtn_Click;
            // 
            // RefreshBtn
            // 
            RefreshBtn.Dock = DockStyle.Fill;
            RefreshBtn.Location = new Point(459, 300);
            RefreshBtn.Name = "RefreshBtn";
            RefreshBtn.Size = new Size(450, 292);
            RefreshBtn.TabIndex = 3;
            RefreshBtn.Text = "Làm mới phiên";
            RefreshBtn.UseVisualStyleBackColor = true;
            RefreshBtn.Click += RefreshBtn_Click;
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(926, 633);
            Controls.Add(tabControl1);
            Font = new Font("Arial", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "DashboardForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            PromptPage.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PromptGridView).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            APIKeyPage.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ApiKeyGridView).EndInit();
            tabControl1.ResumeLayout(false);
            AdminPage.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabPage PromptPage;
        private Panel panel4;
        private DataGridView PromptGridView;
        private Panel panel3;
        private Button ImportExcelBtn;
        private Button ExportExcelBtn;
        private TabPage APIKeyPage;
        private Panel panel2;
        private DataGridView ApiKeyGridView;
        private TextBox SearchAPITextbox;
        private Button SearchApiBtn;
        private Button DeleteApiBtn;
        private Button ExtendAPIBtn;
        private Button CreateApiBtn;
        private TabControl tabControl1;
        private Button GetShapeFileBtn;
        private TabPage AdminPage;
        private TableLayoutPanel tableLayoutPanel1;
        private Button LogoutBtn;
        private Button ChangeAPIGeminiKeyBtn;
        private Button ViewAIModelBtn;
        private Button RefreshBtn;
        private Button UpdateAPIKeyBtn;
        private Button DeletePromptBtn;
        private Label label1;
        private Label CountPromptLabel;
    }
}