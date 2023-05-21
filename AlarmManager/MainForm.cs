using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace AlarmManager
{
    public partial class MainForm : Form
    {
        readonly Thread BackgroundWorker = new Thread(WorkerMethod);
        public static List<Device> devices;
        public static List<Alarm> alarms;
        public int selectedIndex = -1;
        public MainForm()
        {
            InitializeComponent();
            Database.LoadFromXML("..\\..\\SQLSetup.xml");
            User.Name = "Guest";
            User.PermissionChanged += OnPermissionChange;
            LoginForm form = new LoginForm();
            form.ShowDialog();
            this.Text = $"SCADA Alarm Manager - Logged in as {User.Name}";
            timer.Enabled = true;
            BackgroundWorker.Start();
        }

        private void BtnAcknowledge_Click(object sender, EventArgs e)
        {
            if (LvAlarms.SelectedIndices.Count > 0)
            {
                for (int i = LvAlarms.SelectedIndices[0]; i < alarms.Count; i++)
                {
                    if (alarms[i].ToString() == LvAlarms.Items[LvAlarms.SelectedIndices[0]].Text)
                    {
                        alarms[i].Suppressed = !alarms[i].Suppressed;
                    }

                }
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MessageBox.Show("This application reads device information and measurement data from an SQL database " +
                "and activates and deactivates alarms, based on the alarm limits for the device." +
                "The user can suppress/acknowledge alarms if the logged in user has high enough permission level", "Help");
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundWorker.Suspend();
            timer.Stop();
            LoginForm form = new LoginForm();
            form.ShowDialog();
            this.Text = $"SCADA Alarm Manager - Logged in as {User.Name}";
            BackgroundWorker.Resume();
            timer.Start();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BackgroundWorker.Suspend();
            timer.Stop();
            SQLSettingForm form = new SQLSettingForm();
            form.ShowDialog();
            BackgroundWorker.Resume();
            timer.Start();
        }
        private void hideAcknowledgedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAllToolStripMenuItem.Checked = false;
            hideAcknowledgedToolStripMenuItem.Checked = true;
        }

        private void showAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hideAcknowledgedToolStripMenuItem.Checked = false;
            showAllToolStripMenuItem.Checked = true;
        }


        private void OnPermissionChange(object sender, EventArgs e)
        {
            if (User.Permission < 50)
            {
                BtnAcknowledge.Enabled = false;
            }
            else
                BtnAcknowledge.Enabled = true;

        }

        /// <summary>
        /// Update device list and alarms if necessary
        /// </summary>
        private static void WorkerMethod()
        {
            while (true)
            {
                // Is there a new measurement?
                if (Database.connected && !Database.ChecksumMeasurements())
                {
                    // Refresh devices if they have changed
                    if (!Database.ChecksumDevices())
                    {
                        devices = Database.ReadDevices();
                    }
                    List<Alarm> allAlarms = new List<Alarm>();
                    // Find current alarms
                    foreach (Device item in devices)
                    {
                        Alarm A;
                        Device newMeasurement = Database.ReadMeasurement(item);
                        Thread.Sleep(300);
                        item.time = newMeasurement.time;
                        item.value = newMeasurement.value;
                        if (item.value < item.lowLowLimit)
                        {
                            A = new Alarm($"{item.value.ToString("#.###")} < {item.lowLowLimit}", "LowLow", item.time, item.ID);
                            allAlarms.Add(A);
                        }
                        else if (item.value < item.lowLimit)
                        {
                            A = new Alarm($"{item.value.ToString("#.###")} < {item.lowLimit}", "Low", item.time, item.ID);
                            allAlarms.Add(A);
                        }
                        else if (item.value > item.hiHiLimit)
                        {
                            A = new Alarm($"{item.value.ToString("#.###")} > {item.hiHiLimit}", "HighHigh", item.time, item.ID);
                            allAlarms.Add(A);
                        }
                        else if (item.value > item.hiLimit)
                        {
                            A = new Alarm($"{item.value.ToString("#.###")} > {item.hiLimit}", "High", item.time, item.ID);
                            allAlarms.Add(A);
                        }
                    }
                    if (allAlarms.Count() > 0 )
                    {


                        // Are there new alarms? Or are any old ones not valid?
                        if (alarms != null)
                        {
                            List<Alarm> newAlarms = new List<Alarm>();
                            foreach (Alarm item in allAlarms)
                            {
                                int index = IsAlarmPartOf(alarms, item);
                                if (index >= 0)
                                {
                                    newAlarms.Add(alarms[index]);
                                }
                                else newAlarms.Add(item);
                            }
                            alarms = newAlarms;
                        }
                        else alarms = allAlarms;
                    }
                    else alarms = null;
                }
                Thread.Sleep(500);
            }
            
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Get the currently selected item
                if (LvAlarms.SelectedItems.Count > 0)
                {
                    selectedIndex = LvAlarms.SelectedIndices[0];
                }
                else selectedIndex = -1;

                int lvNumber = 0;
                for (int AlarmNumber = 0; AlarmNumber < alarms.Count; AlarmNumber++)
                {
                    Alarm item = alarms[AlarmNumber];
                    if (hideAcknowledgedToolStripMenuItem.Checked && !item.Suppressed)
                    {
                        // Add new line
                        if (AlarmNumber < LvAlarms.Items.Count)
                        {
                            LvAlarms.Items[lvNumber] = new ListViewItem(item.ToString());
                        }
                        // Alter line
                        else
                        {
                            LvAlarms.Items.Add(item.ToString());
                        }
                        if (item.AlarmType == "Low" || item.AlarmType == "High")
                            LvAlarms.Items[lvNumber].BackColor = Color.Yellow;
                        else if (item.AlarmType == "LowLow" || item.AlarmType == "HighHigh")
                            LvAlarms.Items[lvNumber].BackColor = Color.Red;
                        lvNumber++;
                    }
                    else if (showAllToolStripMenuItem.Checked)
                    {
                        // Add new line
                        if (AlarmNumber < LvAlarms.Items.Count)
                        {
                            LvAlarms.Items[lvNumber] = new ListViewItem(item.ToString());
                        }
                        // Alter line
                        else
                        {
                            LvAlarms.Items.Add(item.ToString());
                        }
                        if (item.Suppressed)
                            LvAlarms.Items[lvNumber].BackColor = Color.LightGray;
                        else if (item.AlarmType == "Low" || item.AlarmType == "High")
                            LvAlarms.Items[lvNumber].BackColor = Color.Yellow;
                        else if (item.AlarmType == "LowLow" || item.AlarmType == "HighHigh")
                            LvAlarms.Items[lvNumber].BackColor = Color.Red;
                        lvNumber++;
                    }
                    

                }
                // Remove lines
                while (LvAlarms.Items.Count > lvNumber)
                {
                    LvAlarms.Items[lvNumber].Remove();
                }

                // Set selected back
                if (selectedIndex >= 0 && selectedIndex < LvAlarms.Items.Count)
                {
                    LvAlarms.Items[selectedIndex].Selected = true;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Failed to update list");
            }
            
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                BackgroundWorker.Abort();
            }
            catch (Exception)
            {
            }
        }

        public static int IsAlarmPartOf(List<Alarm> als, Alarm item)
        {
            for (int i = 0; i < als.Count; i++)
            {
                if (als[i] == item)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
