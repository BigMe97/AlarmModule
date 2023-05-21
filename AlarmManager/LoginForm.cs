using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace AlarmManager
{
    public partial class LoginForm : Form
    {
        private bool close = true;
        private bool guest = true;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            User.Name = "Guest";
            User.Permission = 1;
            this.close = false;
            this.Close();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            try
            {
                int user = Convert.ToInt32(txtUserID.Text);
                string passw = txtPassword.Text;
                if (Database.ValidateUser(user, passw))
                {
                    Database.SetUser(user);
                    this.close = false;
                    this.guest = false;
                    this.Close();
                }
                else
                {
                    throw new Exception("Wrong password");
                }
            }
            catch (Exception err)
            {
                MessageBox.Show($"Failed to log in: {err.Message}");
            }
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.close)
            {
                Application.Exit();
            }
            else if (this.guest)
            {
                User.Name = "Guest";
                User.Permission = 1;
            }
        }
    }
}
