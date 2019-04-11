using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace asistonto
{
    public partial class mainForm : Form
    {
        private static string dataFile = Application.StartupPath + @"\"  + Properties.Settings.Default.dataFile;

        public mainForm()
        {
            InitializeComponent();

            if (!File.Exists(dataFile))
                File.CreateText(dataFile).Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Process.Start("notepad", dataFile);
        }

        private void siteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://miguelsimoni.xyz");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void genericToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string value = ((ToolStripMenuItem)sender).Text;
            //if (value.IndexOf("\t") > -1)
            //    value = value.Split('\t')[1];
            doAction(value);
        }

        private static void doAction(string value)
        {
            string patternUrl = @"^(((https?|ftp)\://)?((\[?(\d{1,3}\.){3}\d{1,3}\]?)|(([-a-zA-Z0-9]+\.)+[a-zA-Z]{2,4}))(\:\d+)?(/[-a-zA-Z0-9._?,'+&amp;%$#=~\\]+)*/?)$";
            string patternMailto = @"^mailto\:\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            string patternLocalUrl = @"^\\\\\w+$";
            string patternPath = @"^[a-zA-Z]:[\\\w*(\.\w+)+]*$";
            if (Regex.Match(value, patternUrl).Success)
            {
                try
                {
                    Process.Start(value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid URL." + Environment.NewLine + ex.Message, "oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (Regex.Match(value, patternMailto).Success)
            {
                try
                {
                    Process.Start(value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid e-mail." + Environment.NewLine + ex.Message, "oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (Regex.Match(value, patternLocalUrl).Success)
            {
                try
                {
                    Process.Start(value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid network address." + Environment.NewLine + ex.Message, "oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (Regex.Match(value, patternPath).Success)
            {
                try
                {
                    Process.Start(value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid local path." + Environment.NewLine + ex.Message, "oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Clipboard.SetDataObject(value, true);
            }
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            ToolStripMenuItem menuItem = null;
            contextMenuStrip.Items.Clear();
            if (File.Exists(dataFile))
            {
                StreamReader input = new StreamReader(dataFile, Encoding.Default);
                string item;
                while ((item = input.ReadLine()) != null)
                {
                    menuItem = null;
                    if (item.IndexOf("|") > 0)
                    {
                        string submenu = item.Split('|')[0];
                        foreach (ToolStripMenuItem mit in contextMenuStrip.Items)
                        {
                            if (mit.Text == submenu)
                            {
                                menuItem = mit;
                            }
                        }
                        if (menuItem == null)
                        {
                            menuItem = new ToolStripMenuItem(submenu);
                        }
                        item = item.Substring(item.IndexOf("|") + 1);
                        menuItem.DropDownItems.Add(new ToolStripMenuItem(item, null, new EventHandler(genericToolStripMenuItem_Click)));
                    }
                    else
                    {
                        menuItem = new ToolStripMenuItem(item, null, new EventHandler(genericToolStripMenuItem_Click));
                    }
                    if (!contextMenuStrip.Items.Contains(menuItem))
                        contextMenuStrip.Items.Add(menuItem);
                }
                input.Close();
            }
            contextMenuStrip.Items.Add("-");
            menuItem = new ToolStripMenuItem("Edit...", null, new EventHandler(NotifyIcon_DoubleClick));
            contextMenuStrip.Items.Add(menuItem);
            menuItem = new ToolStripMenuItem("Web...", null, new EventHandler(siteToolStripMenuItem_Click));
            contextMenuStrip.Items.Add(menuItem);
            menuItem = new ToolStripMenuItem("Exit", Properties.Resources.Exit, new EventHandler(exitToolStripMenuItem_Click));
            contextMenuStrip.Items.Add(menuItem);
        }
    }
}
