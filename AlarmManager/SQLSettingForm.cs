using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlarmManager
{
    public partial class SQLSettingForm : Form
    {
        public SQLSettingForm()
        {
            InitializeComponent();
            txtDBHost.Text = Database.GetDBHost();
            txtDatabase.Text = Database.GetDBName();
            txtUser.Text = Database.GetDBUserName();
            txtPassword.Text = Database.GetDBPassword();
            
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
