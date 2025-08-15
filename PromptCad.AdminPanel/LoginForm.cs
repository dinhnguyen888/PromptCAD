using PromptCad.AdminPanel.Models;
using PromptCad.AdminPanel.Services;

namespace PromptCad.AdminPanel
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.FormClosing += LoginForm_FormClosing;

        }
        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); 
        }
        // handle click event for Login button
        private async void LoginBtn_Click(object sender, EventArgs e)
        {
            // Get email and password from textboxes
            string email = EmailTb.Text;
            string password = PasswordTb.Text;
            // Validate email and password
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập email và mật khẩu.");
                return;
            }
            // Call the login service to authenticate the user
            var baseService = new ProjectServices();
            var loginRequest = new LoginRequest
            {
                email = email,
                password = password
            };
            var response = await baseService.LoginAsync(loginRequest);
            if (response != null && !string.IsNullOrEmpty(response.ToString()))
            {
                MessageBox.Show("Đăng nhập thành công!");
                baseService.StoreToken(response.access_token);
                this.Hide();
                // Show the main dashboard form after successful login
                var dashboardForm = new DashboardForm();
                dashboardForm.Show();
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin đăng nhập.");
            }
        }

    }
}
