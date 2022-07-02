using System;
using System.Windows.Forms;

namespace SpeechToSignLanguage.Client.Forms
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.ActiveControl = txtLogin;
        }

        private void LogIn(object sender, EventArgs e)
        {
            if (txtLogin.Text == "")
            {
                MessageBox.Show("Please Enter Your Name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtLogin.Text.Length < 3)
            {
                MessageBox.Show("Please Enter Minimum 3 Character", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.Hide();
            Main main = new Main(txtLogin.Text);
            main.ShowDialog();
            this.Close();
        }

        private void GetUserName(object sender, EventArgs e)
        {
            this.AcceptButton = LogInBtn;
        }
    }
}
