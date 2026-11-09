using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;

[assembly: AssemblyTitle("PDF PRO 14.0.4 Setup Wizard (Mehedi Hasan Shihab @sshihabb007)")]
[assembly: AssemblyCompany("Mehedi Hasan Shihab (sshihabb007)")]
[assembly: AssemblyProduct("PDF PRO 14.0.4 Setup Wizard")]
[assembly: AssemblyVersion("14.0.4.0")]
[assembly: AssemblyFileVersion("14.0.4.0")]

namespace ShihabMehediPdfProInstaller
{
    public sealed class ShihabPdfProSetupWizard : Form
    {
        private TextBox shihabTargetDirBox;
        private CheckBox mehediLaunchAfterCheck;
        private Label sshihabb007StatusLabel;

        [STAThread]
        public static int Main(string[] shihabArgs)
        {
            string mehediDefaultDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "PDF PRO");

            if (shihabArgs != null && shihabArgs.Length > 0)
            {
                bool sshihabb007Silent = false;
                string shihabCustomDir = mehediDefaultDir;
                foreach (string mehediArg in shihabArgs)
                {
                    if (string.IsNullOrWhiteSpace(mehediArg)) continue;
                    if (string.Equals(mehediArg, "--silent", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(mehediArg, "/S", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(mehediArg, "/silent", StringComparison.OrdinalIgnoreCase))
                    {
                        sshihabb007Silent = true;
                    }
                    else if (mehediArg.StartsWith("--dir=", StringComparison.OrdinalIgnoreCase))
                    {
                        shihabCustomDir = mehediArg.Substring("--dir=".Length).Trim('"');
                    }
                }

                if (sshihabb007Silent)
                {
                    try
                    {
                        ExtractShihabPayloadToDirectory(shihabCustomDir);
                        return 0;
                    }
                    catch (Exception shihabEx)
                    {
                        Console.Error.WriteLine("PDF PRO Silent Setup Error: " + shihabEx.Message);
                        return 1;
                    }
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShihabPdfProSetupWizard());
            return 0;
        }

        public static void ExtractShihabPayloadToDirectory(string sshihabb007TargetDir)
        {
            Directory.CreateDirectory(sshihabb007TargetDir);
            Assembly shihabAsm = Assembly.GetExecutingAssembly();
            using (Stream mehediPayloadStream = shihabAsm.GetManifestResourceStream("ShihabMehediPdfProInstaller.payload.zip"))
            {
                if (mehediPayloadStream == null)
                {
                    throw new InvalidOperationException("Embedded payload.zip resource not found.");
                }
                using (ZipArchive sshihabb007Zip = new ZipArchive(mehediPayloadStream, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry shihabEntry in sshihabb007Zip.Entries)
                    {
                        string mehediDestPath = Path.Combine(sshihabb007TargetDir, shihabEntry.FullName.Replace('/', Path.DirectorySeparatorChar));
                        string sshihabb007Parent = Path.GetDirectoryName(mehediDestPath);
                        if (!string.IsNullOrEmpty(sshihabb007Parent))
                        {
                            Directory.CreateDirectory(sshihabb007Parent);
                        }
                        if (!string.IsNullOrEmpty(shihabEntry.Name))
                        {
                            shihabEntry.ExtractToFile(mehediDestPath, true);
                        }
                    }
                }
            }
        }

        public ShihabPdfProSetupWizard()
        {
            this.Text = "PDF PRO v14.0.4 Setup Wizard (x64) — Engineered by Mehedi Hasan Shihab (@sshihabb007)";
            this.Size = new Size(640, 380);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(15, 17, 26);
            this.ForeColor = Color.FromArgb(241, 245, 249);

            Label shihabHeader = new Label
            {
                Text = "⚡ Install PDF PRO v14.0.4 (Starship Architecture Edition)",
                Location = new Point(24, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(124, 92, 255)
            };

            Label mehediSub = new Label
            {
                Text = "Open-Source Enterprise PDF Editor, Neural OCR, Solid Office Converter & AI Suite\r\nDeveloped by Mehedi Hasan Shihab (@sshihabb007)",
                Location = new Point(24, 56),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(148, 163, 184)
            };

            Label sshihabb007DirLbl = new Label
            {
                Text = "Destination Installation Folder:",
                Location = new Point(24, 118),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold)
            };

            string shihabDefaultPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Programs",
                "PDF PRO");

            shihabTargetDirBox = new TextBox
            {
                Location = new Point(24, 144),
                Size = new Size(572, 26),
                BackColor = Color.FromArgb(24, 27, 42),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9.5f),
                Text = shihabDefaultPath
            };

            mehediLaunchAfterCheck = new CheckBox
            {
                Text = "Launch PDF PRO 14.0.4 immediately after installation",
                Location = new Point(24, 186),
                AutoSize = true,
                Checked = true,
                Font = new Font("Segoe UI", 9.5f)
            };

            sshihabb007StatusLabel = new Label
            {
                Text = "Ready to install pdfpro.exe, 32 Starship plugin descriptors, and Chrome/Edge extension.",
                Location = new Point(24, 226),
                AutoSize = true,
                ForeColor = Color.Aquamarine
            };

            Button shihabInstallBtn = new Button
            {
                Text = "Install PDF PRO Now",
                Location = new Point(396, 276),
                Size = new Size(200, 40),
                BackColor = Color.FromArgb(124, 92, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold)
            };
            shihabInstallBtn.FlatAppearance.BorderSize = 0;
            shihabInstallBtn.Click += OnShihabInstallClicked;

            this.Controls.AddRange(new Control[]
            {
                shihabHeader,
                mehediSub,
                sshihabb007DirLbl,
                shihabTargetDirBox,
                mehediLaunchAfterCheck,
                sshihabb007StatusLabel,
                shihabInstallBtn
            });
        }

        private void OnShihabInstallClicked(object shihabSender, EventArgs mehediArgs)
        {
            try
            {
                string sshihabb007TargetDir = shihabTargetDirBox.Text.Trim();
                ExtractShihabPayloadToDirectory(sshihabb007TargetDir);

                sshihabb007StatusLabel.Text = "✔ Installed PDF PRO v14.0.4 to: " + sshihabb007TargetDir;
                string shihabInstalledExe = Path.Combine(sshihabb007TargetDir, "pdfpro.exe");
                if (mehediLaunchAfterCheck.Checked && File.Exists(shihabInstalledExe))
                {
                    Process.Start(new ProcessStartInfo(shihabInstalledExe) { WorkingDirectory = sshihabb007TargetDir });
                }
                this.Close();
            }
            catch (Exception shihabEx)
            {
                MessageBox.Show(this, "Installation error: " + shihabEx.Message, "PDF PRO Setup (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

