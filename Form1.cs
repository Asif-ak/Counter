using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Threading;

namespace Counter
{
    public partial class Form1 : MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }
        datacontainer dcc = new datacontainer();
        Task ProcessImport(Dictionary<int, string> dt, IProgress<ProgressReport> progress)
        {
            int index = 1;
            int totalprogress = dt.Count;
            var progressreport = new ProgressReport();
            return Task.Run(() =>
            {
                for (int i = 0; i < totalprogress; i++)
                {
                    progressreport.PercentComplete = index++ * 100 / totalprogress;
                    progress.Report(progressreport);
                    Thread.Sleep(5);
                }
            });
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select New Excel File";
            ofd.InitialDirectory = @"c:\";
            ofd.Filter = "Excel Sheet(*.xlsx)|*.xlsx|Excel Sheet(*.xls)|*.xls";
            ofd.FilterIndex = 2;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;
            ofd.ShowDialog();
            if (!string.IsNullOrEmpty(ofd.FileName)) // no need of this check -> fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0

            {
                try
                {

                    string query = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ofd.FileName + ";Extended Properties = 'Excel 12.0 XML;HDR=YES;IMEX=1'";
                    metroComboBox1.DataSource = dcc.getsheetname(query);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }

            else
            {

                MessageBox.Show("Please select excel file ", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // code shifted to metrobutton5 click event

        }
        async void progress()
        {
            metroLabel1.Text = "Getting Distinct Values from Data ..... ";
            var progressreport = new Progress<ProgressReport>();
            progressreport.ProgressChanged += (o, report) =>
              {
                  metroLabel1.Text = string.Format("Processing ...... {0} ", report.PercentComplete);
                  metroProgressBar1.Value = report.PercentComplete;
                  metroProgressBar1.Update();
              };
            try
            {
                await ProcessImport(dcc.distinctvalues(), progressreport);
                metroLabel1.Text = "Done !!!!";
                timer1.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            metroLabel3.Text = string.Empty;
            metroProgressSpinner1.Visible = true;
            metroProgressSpinner1.Spinning = true;
            metroProgressSpinner1.Style = MetroFramework.MetroColorStyle.Blue;
            metroProgressSpinner1.Speed = 1;
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            metroProgressSpinner1.Spinning = false;
            try
            {
                int a = dcc.getrandomid();
                if (a == 0)
                    MessageBox.Show("Please try again");

                else
                {
                    string key = dcc.individualwinner(a).Key.ToString();
                    string value = dcc.individualwinner(a).Value;
                    MessageBox.Show(string.Format("Winner ID: {0}" + "\n" + "Winner Name: {1}", key, value));
                    //metroLabel3.Text = a.ToString(); // present but not visisble.
                    //dataGridView2.DataSource = null;
                    //dataGridView2.DataSource = dcc.updatedistinctvalues(); // this datagridview is invisible.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No Unique Membership IDs' left for balloting. Please load another file!" + "\n" + ex.Message.ToString());
            }


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            metroPanel1.Visible = false;
            metroPanel2.Visible = false;
            metroPanel3.Visible = true;
        }

        private void Form1_Leave(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dcc.updatedistinctvalues().Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<int, string> entry in dcc.updatedistinctvalues())
                {
                    sb.AppendFormat("\n Membership ID: {0} Member Name: {1}", entry.Key.ToString(), entry.Value);
                }
                MessageBox.Show("Winning Members: " + "\n" + sb.ToString());
            }
            else
            {
                e.Cancel = false; // to ensure form closes if empty without exception or hangs.
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select New Excel File";
            ofd.InitialDirectory = @"c:\";
            ofd.Filter = "Excel Sheet(*.xlsx)|*.xlsx|Excel Sheet(*.xls)|*.xls|CSV File(*.csv)|*.csv";
            ofd.FilterIndex = 3;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;
            ofd.ShowDialog();
            if (!string.IsNullOrEmpty(ofd.FileName)) // no need of this check -> fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0

            {
                //string query = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ofd.FileName + ";Extended Properties = 'Excel 12.0 XML;HDR=YES;IMEX=1'";
                try
                {
                    if (dcc.writetofile(ofd.FileName))
                        MessageBox.Show("Records Saved on " + ofd.FileName);
                    else
                        MessageBox.Show("No record to save");

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(metroTextBox1.Text))
            { MessageBox.Show("Please Enter Draw Name ", "Wait", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); }
            else
            {
                try
                {
                    dcc.drawname = metroTextBox1.Text;
                    string query = "Select * from [" + metroComboBox1.Text + "$]";
                    //dataGridView1.DataSource = dcc.loadsheet(query);
                    if (dcc.loadsheet(query))
                    {
                        metroPanel2.Visible = true;
                        progress();

                    }

                    else
                    {
                        MessageBox.Show("Empty data loaded");
                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
