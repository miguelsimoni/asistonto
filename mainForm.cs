﻿using System;
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
        private static string dataFile = Application.StartupPath + @"\" + Properties.Settings.Default.dataFile;

        const string PATTERN_URL = @"^(((https?|ftp)\://)?((\[?(\d{1,3}\.){3}\d{1,3}\]?)|(([-a-zA-Z0-9]+\.)+[a-zA-Z]{2,4}))(\:\d+)?(/[-a-zA-Z0-9._?,'+&amp;%$#=~\\]+)*/?)$";
        const string PATTERN_MAILTO = @"^mailto\:\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        const string PATTERN_NETWORK_ADDRESS = @"^\\\\\w+$";
        const string PATTERN_LOCAL_PATH = @"^[a-zA-Z]:[\\\w*(\.\w+)+]*$";

        public mainForm()
        {
            InitializeComponent();

            this.notifyIcon.Text = Application.ProductName + " v" + Application.ProductVersion;

            this.fileSystemWatcher.Path = Application.StartupPath;
            this.fileSystemWatcher.Filter = Properties.Settings.Default.dataFile;

            if (!File.Exists(dataFile))
                File.CreateText(dataFile).Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Hide();
            loadData();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            loadData();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(Properties.Resources.externalEditor, dataFile);
        }

        private void siteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Properties.Resources.productWebsite);
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

        private void loadData()
        {
            ToolStripItem menuItem = null;
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
                        foreach (ToolStripItem mit in contextMenuStrip.Items)
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
                        ((ToolStripMenuItem)menuItem).DropDownItems.Add(new ToolStripMenuItem(item, null, new EventHandler(genericToolStripMenuItem_Click)));
                    }
                    else if (item == "-")
                    {
                        menuItem = new ToolStripSeparator();
                    }
                    else
                    {
                        if(Regex.Match(item, PATTERN_URL).Success)
                        {
                            menuItem = new ToolStripMenuItem(item, Properties.Resources.iconWeb, new EventHandler(genericToolStripMenuItem_Click));
                        }
                        else
                        {
                            menuItem = new ToolStripMenuItem(item, null, new EventHandler(genericToolStripMenuItem_Click));
                        }
                    }
                    if (!contextMenuStrip.Items.Contains(menuItem))
                    {
                        contextMenuStrip.Items.Add(menuItem);
                    }
                }
                input.Close();
            }
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            menuItem = new ToolStripMenuItem(Properties.Resources.menuEdit, null, new EventHandler(NotifyIcon_DoubleClick));
            contextMenuStrip.Items.Add(menuItem);
            menuItem = new ToolStripMenuItem(Properties.Resources.menuWeb, null, new EventHandler(siteToolStripMenuItem_Click));
            contextMenuStrip.Items.Add(menuItem);
            menuItem = new ToolStripMenuItem(Properties.Resources.menuExit, Properties.Resources.iconExit, new EventHandler(exitToolStripMenuItem_Click));
            contextMenuStrip.Items.Add(menuItem);
        }

        private static void doAction(string value)
        {
            if (Regex.Match(value, PATTERN_URL).Success)
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
            if (Regex.Match(value, PATTERN_MAILTO).Success)
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
            if (Regex.Match(value, PATTERN_NETWORK_ADDRESS).Success)
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
            if (Regex.Match(value, PATTERN_LOCAL_PATH).Success)
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

    }
}
