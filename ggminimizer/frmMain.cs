using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ggminimizer
{
    public partial class frmMain : Form
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

        private const int MOD_ALT = 0x1;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        private const int MOD_NOREPEAT = 0x4000;

        private const int WM_HOTKEY = 0x312;

        private Settings settings = new Settings();

        private bool bAllowClose = false;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ShowWindowAsync(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("User32.dll")]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bAllowClose == false)
            {
                e.Cancel = true;
                this.Visible = false;
                return;
            }

            settings.Save();
            UnregisterHotKey(this.Handle, 100);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            notifyIcon.ContextMenuStrip = this.contextMenuStrip;

            cmbKeys.DataSource = Enum.GetValues(typeof(System.Windows.Forms.Keys));

            ShowNotification();
            LoadSettings();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (settings.HotKey == 0)
            {
                MessageBox.Show("Select a HotKey");
                return;
            }

            settings.Save();
            UnregisterHotKey(this.Handle, 69);
            RegisterHotKey(this.Handle, 69, settings.Modifiers, settings.HotKey);

            ShowNotification();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
        }

        private void cmbKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            settings.HotKey = (int)cmbKeys.SelectedItem;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bAllowClose = true;
            this.Close();
        }

        private void chkModifiers_CheckedChanged(object sender, EventArgs e)
        {
            ApplyModifiers();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtProcess.Text.Trim() != string.Empty && !settings.Processes.Contains(txtProcess.Text.Trim()))
            {
                settings.Processes.Add(txtProcess.Text.Trim());
                lstProcess.Items.Add(txtProcess.Text.Trim());
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstProcess.SelectedIndex != -1)
            {
                settings.Processes.RemoveAt(lstProcess.SelectedIndex);
                lstProcess.Items.RemoveAt(lstProcess.SelectedIndex);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                IntPtr id = m.WParam;
                switch ((id.ToString()))
                {
                    case "69":
                        {
                            Process[] p = null;

                            for (var i = 0; i <= settings.Processes.Count - 1; i++)
                            {
                                p = Process.GetProcessesByName(settings.Processes[i]);
                                if (p != null && p.Length > 0) break;
                            }

                            if (p == null || p.Length == 0)
                                return;

                            var mwh = p[0].MainWindowHandle;

                            IntPtr fw = GetForegroundWindow();

                            if (fw.ToInt32() != mwh.ToInt32())
                            {
                                ShowWindowAsync(mwh, SW_SHOWMINIMIZED);
                                ShowWindowAsync(mwh, SW_SHOWMAXIMIZED);
                            }
                            else
                            {
                                ShowWindowAsync(mwh, SW_SHOWMINNOACTIVE);
                                IntPtr dw = GetDesktopWindow();
                                SetForegroundWindow(dw);
                            }

                            break;
                        }
                }
            }
            base.WndProc(ref m);
        }

        private void ShowNotification()
        {
            notifyIcon.BalloonTipText = "GG Minimizer";
            notifyIcon.ShowBalloonTip(2);
        }

        private void LoadSettings()
        {
            settings = settings.Load();

            for (var i = 0; i <= cmbKeys.Items.Count - 1; i++)
            {
                if ((int)cmbKeys.Items[i] == settings.HotKey)
                {
                    cmbKeys.SelectedIndex = i;
                }
            }

            for (var i = 0; i <= settings.Processes.Count - 1; i++)
            {
                lstProcess.Items.Add(settings.Processes[i]);
            }

            int modifiers = settings.Modifiers;

            chkCtrl.Checked = (modifiers & MOD_CONTROL) == MOD_CONTROL;
            chkAlt.Checked = (modifiers & MOD_ALT) == MOD_ALT;
            chkShift.Checked = (modifiers & MOD_SHIFT) == MOD_SHIFT;
            chkWin.Checked = (modifiers & MOD_WIN) == MOD_WIN;
        }

        private void ApplyModifiers()
        {
            settings.Modifiers = 0;
            settings.Modifiers = chkCtrl.Checked ? settings.Modifiers | MOD_CONTROL : settings.Modifiers;
            settings.Modifiers = chkAlt.Checked ? settings.Modifiers | MOD_ALT : settings.Modifiers;
            settings.Modifiers = chkShift.Checked ? settings.Modifiers | MOD_SHIFT : settings.Modifiers;
            settings.Modifiers = chkWin.Checked ? settings.Modifiers | MOD_WIN : settings.Modifiers;
            settings.Modifiers = MOD_NOREPEAT | settings.Modifiers;
        }
    }
}
