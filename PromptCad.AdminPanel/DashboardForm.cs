using Newtonsoft.Json;
using PromptCad.AdminPanel.Models;
using PromptCad.AdminPanel.Services;
using PromptCad.AdminPanel.Forms;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PromptCad.AdminPanel
{
    public partial class DashboardForm : Form
    {
        private readonly HttpClient _httpClient;
        private readonly ProjectServices _projectServices;

        public DashboardForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _projectServices = new ProjectServices();

            // Gọi load API key khi form load
            this.Load += DashboardForm_Load;

            // Đánh số thứ tự mỗi khi vẽ lại hàng
            this.ApiKeyGridView.RowPostPaint += ApiKeyGridView_RowPostPaint;
        }

        private async void DashboardForm_Load(object sender, EventArgs e)
        {
            // Setup cho DataGridView
            ApiKeyGridView.AutoGenerateColumns = true;
            ApiKeyGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ApiKeyGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ApiKeyGridView.AllowUserToAddRows = false;

            await LoadApiKeysAsync();
        }
        private List<ApiKeyInfo> _allApiKeys = new List<ApiKeyInfo>();


        private async Task LoadApiKeysAsync()
        {
            try
            {
                var response = await _projectServices.GetAllAPIKey();
                _allApiKeys = response.ApiKeys;
                ApiKeyGridView.DataSource = response.ApiKeys;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải API keys: " + ex.Message);
            }
        }

        private void ApiKeyGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = ApiKeyGridView.Rows[e.RowIndex].DataBoundItem as ApiKeyInfo;
                if (row != null)
                {
                    MessageBox.Show($"API Key: {row.ApiKey}\nCreated: {row.CreatedAt}");
                }
            }
        }

        private void ApiKeyGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Vẽ số thứ tự vào cột đầu tiên (header)
            var grid = sender as DataGridView;
            var rowNumber = (e.RowIndex + 1).ToString();

            var centerFormat = new System.Drawing.StringFormat()
            {
                Alignment = System.Drawing.StringAlignment.Center,
                LineAlignment = System.Drawing.StringAlignment.Center
            };

            // Vẽ vào phần header của row
            var headerBounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowNumber, this.Font, System.Drawing.SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private async void SearchApiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = SearchAPITextbox.Text.Trim();

                if (string.IsNullOrEmpty(keyword))
                {
                    // Nếu ô search trống thì load lại tất cả
                    ApiKeyGridView.DataSource = _allApiKeys;
                    await LoadApiKeysAsync();
                }
                else
                {
                    var filtered = _allApiKeys.FindAll(k =>
                    k.ApiKey.ToLower().Contains(keyword.ToLower()) ||
                    k.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss").Contains(keyword) ||
                    k.ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss").Contains(keyword) ||
                    (k.UserName?.ToLower().Contains(keyword.ToLower()) ?? false) ||
                    (k.PhoneNumber?.Contains(keyword) ?? false)
                );

                    ApiKeyGridView.DataSource = filtered;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm API keys: " + ex.Message);
            }
        }

        private async void CreateApiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new CreateAPIKeyForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var newKey = await _projectServices.CreateAPIKey(form.Result);

                        if (newKey != null)
                        {
                            MessageBox.Show($"Tạo API Key thành công!\nAPI Key: {newKey.ApiKey}\nExpires: {newKey.ExpiresAt:yyyy-MM-dd HH:mm:ss}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadApiKeysAsync();
                        }
                        else
                        {
                            MessageBox.Show("Tạo API Key thất bại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo API key: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ExtendAPIBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ApiKeyGridView.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một API key để cập nhật.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedKey = ApiKeyGridView.CurrentRow.DataBoundItem as ApiKeyInfo;
                if (selectedKey == null) return;

                using (var form = new ExtendAPIKeyForm(selectedKey))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var updated = await _projectServices.UpdateAPIKey(form.Result);

                        if (updated)
                        {
                            MessageBox.Show("Cập nhật API Key thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadApiKeysAsync();
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thất bại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật API key: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void DeleteApiBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ApiKeyGridView.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một API key để xóa.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedKey = ApiKeyGridView.CurrentRow.DataBoundItem as ApiKeyInfo;
                if (selectedKey == null) return;

                var confirm = MessageBox.Show($"Bạn có chắc muốn xóa API Key:\n{selectedKey.ApiKey} ?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    var request = new DeleteAPIKeyRequest
                    {
                        ApiKey = selectedKey.ApiKey,
                        CleanupExpired = false
                    };

                    bool deleted = await _projectServices.DeleteAPIKey(request);

                    if (deleted)
                    {
                        MessageBox.Show("Xóa thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await LoadApiKeysAsync();
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa API key: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogoutBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Xóa token file
                if (File.Exists(globalAPI.TokenFilePath))
                {
                    File.Delete(globalAPI.TokenFilePath);
                }

                MessageBox.Show("Đã đăng xuất thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đăng xuất: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ChangeAPIGeminiKeyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var newApiKey = Microsoft.VisualBasic.Interaction.InputBox(
                    "Nhập Gemini API Key mới:", 
                    "Change Gemini API Key", 
                    "");

                if (!string.IsNullOrWhiteSpace(newApiKey))
                {
                    bool updated = await _projectServices.UpdateAIModelAPIKey("gemini", newApiKey);
                    if (updated)
                    {
                        MessageBox.Show("Cập nhật Gemini API Key thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thay đổi Gemini API Key: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UpdateAPIKeyBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (ApiKeyGridView.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một API key để cập nhật thông tin.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedKey = ApiKeyGridView.CurrentRow.DataBoundItem as ApiKeyInfo;
                if (selectedKey == null) return;

                using (var form = new UpdateAPIKeyForm(selectedKey))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        var updated = await _projectServices.UpdateAPIKeyInfo(form.Result);

                        if (updated)
                        {
                            MessageBox.Show("Cập nhật thông tin API Key thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await LoadApiKeysAsync();
                        }
                        else
                        {
                            MessageBox.Show("Cập nhật thất bại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật thông tin API key: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ExportExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var filePath = await _projectServices.ExportPromptsToExcel();
                if (!string.IsNullOrEmpty(filePath))
                {
                    MessageBox.Show($"Xuất Excel thành công!\nFile: {filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ImportExcelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    openFileDialog.Title = "Chọn file Excel để import";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        var confirm = MessageBox.Show("Bạn có chắc muốn import prompts từ file này? Dữ liệu cũ sẽ bị ghi đè.", 
                            "Xác nhận import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (confirm == DialogResult.Yes)
                        {
                            bool imported = await _projectServices.ImportPromptsFromExcel(openFileDialog.FileName);
                            if (imported)
                            {
                                MessageBox.Show("Import Excel thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Import thất bại!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi import Excel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void GetShapeFileBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var content = await _projectServices.GetShapesContent();
                
                using (var form = new Form())
                {
                    form.Text = "Shapes.txt Content";
                    form.Size = new System.Drawing.Size(800, 600);
                    form.StartPosition = FormStartPosition.CenterParent;

                    var textBox = new TextBox
                    {
                        Multiline = true,
                        ScrollBars = ScrollBars.Both,
                        Dock = DockStyle.Fill,
                        ReadOnly = true,
                        Text = content.Content
                    };

                    form.Controls.Add(textBox);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lấy nội dung shapes.txt: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ViewAIModelBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var config = await _projectServices.GetAIModelConfig();
                
                var message = "AI Model Configuration:\n\n";
                foreach (var model in config.Models)
                {
                    message += $"{model.Key.ToUpper()}:\n";
                    message += $"  Has API Key: {(model.Value.HasApiKey ? "Yes" : "No")}\n";
                    message += $"  Key Length: {model.Value.KeyLength}\n\n";
                }

                MessageBox.Show(message, "AI Model Config", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xem cấu hình AI model: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RefreshBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Refresh admin session token
                var newToken = await _projectServices.RefreshAdminSessionToken();
                
                // Lưu token mới vào file
                File.WriteAllText(globalAPI.TokenFilePath, newToken);
                
                MessageBox.Show("Đã refresh admin token thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi refresh admin token: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RefreshDataBtn_Click(object sender, EventArgs e)
        {
            try
            {
                await LoadApiKeysAsync();
                MessageBox.Show("Đã refresh dữ liệu thành công!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi refresh dữ liệu: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
                                                                                                                                                                                                        }
}
