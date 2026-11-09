using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShihabMehediPdfPro
{
    public static class ShihabDialogsAndToolsWindows
    {
        // =====================================================================
        // 1. IN-PLACE TEXT BLOCK EDITOR DIALOG (PageEditor.fpi)
        // =====================================================================
        public static bool ShowShihabTextBlockEditorDialog(IWin32Window shihabOwner, ShihabPdfTextBlockItem mehediBlock)
        {
            if (mehediBlock == null) return false;

            using (Form sshihabb007Dlg = new Form())
            {
                sshihabb007Dlg.Text = "Edit PDF Content Stream Text Block — PDF PRO (Mehedi Hasan Shihab @sshihabb007)";
                sshihabb007Dlg.Size = new Size(580, 340);
                sshihabb007Dlg.StartPosition = FormStartPosition.CenterParent;
                sshihabb007Dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                sshihabb007Dlg.MaximizeBox = false;
                sshihabb007Dlg.MinimizeBox = false;
                sshihabb007Dlg.BackColor = ShihabThemePalette.MehediWindowBg;
                sshihabb007Dlg.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

                Label shihabLbl = new Label
                {
                    Text = "Modify Content-Stream Text (Page " + (mehediBlock.MehediPageIndex + 1) + "):",
                    Location = new Point(18, 16),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
                };

                TextBox mehediTxt = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Location = new Point(18, 42),
                    Size = new Size(526, 130),
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 10.5f),
                    Text = mehediBlock.ShihabText
                };

                Label sshihabb007SizeLbl = new Label
                {
                    Text = "Font Size (pt):",
                    Location = new Point(18, 190),
                    AutoSize = true
                };

                NumericUpDown shihabFontSizeNum = new NumericUpDown
                {
                    Location = new Point(115, 187),
                    Width = 75,
                    Minimum = 6,
                    Maximum = 72,
                    DecimalPlaces = 1,
                    Value = (decimal)Math.Max(6f, Math.Min(72f, mehediBlock.ShihabFontSizePt)),
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary
                };

                CheckBox mehediBoldChk = new CheckBox
                {
                    Text = "Bold (/F2 Helvetica-Bold)",
                    Location = new Point(215, 188),
                    AutoSize = true,
                    Checked = mehediBlock.Sshihabb007IsBold
                };

                Button sshihabb007OkBtn = new Button
                {
                    Text = "Apply Text Edit",
                    Location = new Point(318, 242),
                    Size = new Size(120, 36),
                    BackColor = ShihabThemePalette.ShihabPrimaryAccent,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };
                sshihabb007OkBtn.FlatAppearance.BorderSize = 0;

                Button shihabCancelBtn = new Button
                {
                    Text = "Cancel",
                    Location = new Point(448, 242),
                    Size = new Size(96, 36),
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.Cancel
                };

                sshihabb007Dlg.Controls.AddRange(new Control[] { shihabLbl, mehediTxt, sshihabb007SizeLbl, shihabFontSizeNum, mehediBoldChk, sshihabb007OkBtn, shihabCancelBtn });
                ShihabThemePalette.ApplyMehediDarkWindowFrame(sshihabb007Dlg.Handle, ShihabThemePalette.IsShihabDarkTheme);

                if (sshihabb007Dlg.ShowDialog(shihabOwner) == DialogResult.OK)
                {
                    mehediBlock.ShihabText = mehediTxt.Text;
                    mehediBlock.ShihabFontSizePt = (float)shihabFontSizeNum.Value;
                    mehediBlock.Sshihabb007IsBold = mehediBoldChk.Checked;
                    mehediBlock.MehediIsModified = true;
                    return true;
                }
                return false;
            }
        }

        // =====================================================================
        // 2. DYNAMIC PLUGIN LOADER & SENSOR TELEMETRY STUDIO (PlgDynLoader.fpi)
        // =====================================================================
        public static void ShowMehediPluginAndTelemetryStudio(
            IWin32Window shihabOwner,
            ShihabPlgDynLoader mehediLoader,
            MehediSensorOpenTracingEngine sshihabb007Sensor)
        {
            using (Form shihabDlg = new Form())
            {
                shihabDlg.Text = "Starship Dynamic Plugin Loader (PlgDynLoader.fpi) & Sensor.db OpenTracing Studio — PDF PRO (@sshihabb007)";
                shihabDlg.Size = new Size(980, 620);
                shihabDlg.StartPosition = FormStartPosition.CenterParent;
                shihabDlg.BackColor = ShihabThemePalette.MehediWindowBg;
                shihabDlg.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

                Label mehediHeader = new Label
                {
                    Text = "PlgDynLoader.fpi ABI Registry (LoadBehavior 0=Bootstrap, 3=Eager Core, 9=Lazy Demand) — Engineered by Mehedi Hasan Shihab",
                    Location = new Point(16, 14),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                    ForeColor = ShihabThemePalette.ShihabPrimaryAccent
                };

                ListView sshihabb007List = new ListView
                {
                    Location = new Point(16, 44),
                    Size = new Size(932, 310),
                    View = View.Details,
                    FullRowSelect = true,
                    GridLines = true,
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                    BorderStyle = BorderStyle.FixedSingle
                };
                sshihabb007List.Columns.Add("Internal Name", 165);
                sshihabb007List.Columns.Add("Plugin Binary (.fpi)", 155);
                sshihabb007List.Columns.Add("LoadBehavior", 95);
                sshihabb007List.Columns.Add("Memory State", 115);
                sshihabb007List.Columns.Add("Calls", 60);
                sshihabb007List.Columns.Add("Ribbon Tab", 95);
                sshihabb007List.Columns.Add("Architectural Role", 230);

                TextBox shihabSensorLog = new TextBox
                {
                    Location = new Point(16, 392),
                    Size = new Size(932, 128),
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = Color.Aquamarine,
                    Font = new Font("Consolas", 8.8f),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Action mehediRefreshUi = () =>
                {
                    sshihabb007List.BeginUpdate();
                    sshihabb007List.Items.Clear();
                    foreach (MehediPluginDescriptor shihabPlg in mehediLoader.ShihabAllPlugins)
                    {
                        string mehediBehLabel = shihabPlg.ShihabLoadBehavior == 0
                            ? "0 (Bootstrap)"
                            : (shihabPlg.ShihabLoadBehavior == 3 ? "3 (Eager)" : "9 (Lazy)");
                        string sshihabb007State = shihabPlg.MehediIsLoadedInMemory ? "LOADED (In-Proc)" : "UNLOADED (Lazy)";

                        ListViewItem shihabItem = new ListViewItem(new[]
                        {
                            shihabPlg.ShihabInternalName,
                            shihabPlg.Sshihabb007RealFilePath,
                            mehediBehLabel,
                            sshihabb007State,
                            shihabPlg.ShihabInvocationCount.ToString(CultureInfo.InvariantCulture),
                            shihabPlg.ShihabRibbonTab,
                            shihabPlg.MehediDisplayName
                        });
                        shihabItem.Tag = shihabPlg;
                        sshihabb007List.Items.Add(shihabItem);
                    }
                    sshihabb007List.EndUpdate();

                    StringBuilder mehediSb = new StringBuilder();
                    foreach (ShihabSensorTelemetrySpan sshihabb007Span in sshihabb007Sensor.ShihabRecordedSpans)
                    {
                        mehediSb.AppendFormat("[Span #{0}] {1} | {2}\r\n",
                            sshihabb007Span.ShihabRecordId,
                            sshihabb007Span.ShihabOperationName,
                            sshihabb007Span.MehediSerializedJsonTags);
                    }
                    shihabSensorLog.Text = mehediSb.ToString();
                };

                Label sshihabb007SensorLbl = new Label
                {
                    Text = "OpenTracing Telemetry Span Ledger (Sensor.dll -> Sensor_Data_Table):",
                    Location = new Point(16, 368),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9.2f, FontStyle.Bold)
                };

                Button shihabLoadSelectedBtn = new Button
                {
                    Text = "Map & Invoke Selected Plugin (PlugInMain)",
                    Location = new Point(16, 532),
                    Size = new Size(255, 36),
                    BackColor = ShihabThemePalette.ShihabPrimaryAccent,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                shihabLoadSelectedBtn.FlatAppearance.BorderSize = 0;
                shihabLoadSelectedBtn.Click += (shihabS, mehediE) =>
                {
                    if (sshihabb007List.SelectedItems.Count == 0) return;
                    MehediPluginDescriptor sshihabb007Sel = sshihabb007List.SelectedItems[0].Tag as MehediPluginDescriptor;
                    if (sshihabb007Sel != null)
                    {
                        mehediLoader.TriggerShihabPluginOnDemand(sshihabb007Sel.ShihabInternalName);
                        mehediRefreshUi();
                    }
                };

                Button mehediLoadAllBtn = new Button
                {
                    Text = "Load All 32 Plugins Into Memory",
                    Location = new Point(282, 532),
                    Size = new Size(215, 36),
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                    FlatStyle = FlatStyle.Flat
                };
                mehediLoadAllBtn.Click += (shihabS, mehediE) =>
                {
                    foreach (MehediPluginDescriptor sshihabb007Plg in mehediLoader.ShihabAllPlugins)
                    {
                        mehediLoader.TriggerShihabPluginOnDemand(sshihabb007Plg.ShihabInternalName);
                    }
                    mehediRefreshUi();
                };

                Button sshihabb007CloseBtn = new Button
                {
                    Text = "Close Studio",
                    Location = new Point(840, 532),
                    Size = new Size(108, 36),
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };

                shihabDlg.Controls.AddRange(new Control[] { mehediHeader, sshihabb007List, sshihabb007SensorLbl, shihabSensorLog, shihabLoadSelectedBtn, mehediLoadAllBtn, sshihabb007CloseBtn });
                mehediRefreshUi();
                ShihabThemePalette.ApplyMehediDarkWindowFrame(shihabDlg.Handle, ShihabThemePalette.IsShihabDarkTheme);
                ShihabThemePalette.ApplySshihabb007DarkScrollbarsRecursive(shihabDlg);
                shihabDlg.ShowDialog(shihabOwner);
            }
        }

        // =====================================================================
        // 3. CALLAS PDF/A PREFLIGHT & ACCESSIBILITY STUDIO (PDFA.fpi)
        // =====================================================================
        public static void ShowShihabPreflightAndOptimizerStudio(IWin32Window shihabOwner, ShihabPdfDocumentModel mehediDoc, Action sshihabb007OnUpdated)
        {
            using (Form shihabDlg = new Form())
            {
                shihabDlg.Text = "Callas PDF/A-2b Preflight, Section 508 Accessibility & Stream Optimizer — PDF PRO (@sshihabb007)";
                shihabDlg.Size = new Size(860, 520);
                shihabDlg.StartPosition = FormStartPosition.CenterParent;
                shihabDlg.BackColor = ShihabThemePalette.MehediWindowBg;
                shihabDlg.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

                ListView mehediListView = new ListView
                {
                    Location = new Point(16, 18),
                    Size = new Size(812, 370),
                    View = View.Details,
                    FullRowSelect = true,
                    GridLines = true,
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary
                };
                mehediListView.Columns.Add("Rule ID", 145);
                mehediListView.Columns.Add("Status", 85);
                mehediListView.Columns.Add("Standard Category", 170);
                mehediListView.Columns.Add("Diagnostic Finding", 390);

                Action sshihabb007Populate = () =>
                {
                    mehediListView.Items.Clear();
                    List<MehediPreflightIssueItem> shihabChecks = MehediOemEnginesAndConverters.RunShihabCallasPreflightAudit(mehediDoc);
                    foreach (MehediPreflightIssueItem mehediIssue in shihabChecks)
                    {
                        mehediListView.Items.Add(new ListViewItem(new[]
                        {
                            mehediIssue.ShihabRuleCode,
                            mehediIssue.MehediSeverity,
                            mehediIssue.Sshihabb007Category,
                            mehediIssue.ShihabMessage
                        }));
                    }
                };

                Button shihabFixPdfABtn = new Button
                {
                    Text = "✔ Auto-Fix & Convert to ISO PDF/A-2b + PDF/UA",
                    Location = new Point(16, 412),
                    Size = new Size(310, 38),
                    BackColor = ShihabThemePalette.ShihabPrimaryAccent,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                shihabFixPdfABtn.FlatAppearance.BorderSize = 0;
                shihabFixPdfABtn.Click += (shihabS, mehediE) =>
                {
                    MehediOemEnginesAndConverters.ApplyMehediPdfAConversionAndFixups(mehediDoc);
                    ShihabSecuritySignAndAiEngine.BurnAllMehediPendingRedactions(mehediDoc);
                    sshihabb007Populate();
                    if (sshihabb007OnUpdated != null) sshihabb007OnUpdated();
                    MessageBox.Show(shihabDlg, "All ISO 19005-2 (PDF/A-2b) and Section 508 (PDF/UA) preflight fixups applied!", "PDF PRO Preflight Studio (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                Button mehediCloseBtn = new Button
                {
                    Text = "Close",
                    Location = new Point(720, 412),
                    Size = new Size(108, 38),
                    BackColor = ShihabThemePalette.ShihabPanelBg,
                    ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                    FlatStyle = FlatStyle.Flat,
                    DialogResult = DialogResult.OK
                };

                shihabDlg.Controls.AddRange(new Control[] { mehediListView, shihabFixPdfABtn, mehediCloseBtn });
                sshihabb007Populate();
                ShihabThemePalette.ApplyMehediDarkWindowFrame(shihabDlg.Handle, ShihabThemePalette.IsShihabDarkTheme);
                ShihabThemePalette.ApplySshihabb007DarkScrollbarsRecursive(shihabDlg);
                shihabDlg.ShowDialog(shihabOwner);
            }
        }

        // =====================================================================
        // 4.ODA 3D CAD / U3D INTERACTIVE VIEWPORT STUDIO (U3DBrowser.fpi)
        // =====================================================================
        public static void ShowMehediOda3dCadViewportStudio(IWin32Window shihabOwner, ShihabPdfDocumentModel mehediDoc, int sshihabb007ActivePageIdx, Action shihabOnStamped)
        {
            using (Form mehediDlg = new Form())
            {
                mehediDlg.Text = "ODA 3D (U3D / PRC / CAD) Interactive Model Viewport — PDF PRO (ShihabU3dBrowser.fpi)";
                mehediDlg.Size = new Size(780, 560);
                mehediDlg.StartPosition = FormStartPosition.CenterParent;
                mehediDlg.BackColor = ShihabThemePalette.MehediWindowBg;
                mehediDlg.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

                float sshihabb007Yaw = 35f;
                float shihabPitch = 22f;
                float mehediZoom = 1.15f;

                PictureBox sshihabb007Canvas = new PictureBox
                {
                    Location = new Point(16, 16),
                    Size = new Size(732, 390),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BorderStyle = BorderStyle.FixedSingle
                };

                Action shihabRedraw3D = () =>
                {
                    Image mehediOld = sshihabb007Canvas.Image;
                    sshihabb007Canvas.Image = MehediOemEnginesAndConverters.RenderShihabOda3dCadViewport(732, 390, sshihabb007Yaw, shihabPitch, mehediZoom);
                    if (mehediOld != null) mehediOld.Dispose();
                };

                TrackBar mehediYawSlider = new TrackBar
                {
                    Location = new Point(80, 420),
                    Size = new Size(210, 42),
                    Minimum = 0,
                    Maximum = 360,
                    Value = 35,
                    TickFrequency = 30
                };
                mehediYawSlider.Scroll += (s, e) => { sshihabb007Yaw = mehediYawSlider.Value; shihabRedraw3D(); };

                TrackBar sshihabb007PitchSlider = new TrackBar
                {
                    Location = new Point(350, 420),
                    Size = new Size(190, 42),
                    Minimum = -80,
                    Maximum = 80,
                    Value = 22,
                    TickFrequency = 20
                };
                sshihabb007PitchSlider.Scroll += (s, e) => { shihabPitch = sshihabb007PitchSlider.Value; shihabRedraw3D(); };

                Button shihabEmbed3dBtn = new Button
                {
                    Text = "Embed 3D CAD Snapshot on Active Page",
                    Location = new Point(16, 468),
                    Size = new Size(265, 36),
                    BackColor = ShihabThemePalette.ShihabPrimaryAccent,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                shihabEmbed3dBtn.FlatAppearance.BorderSize = 0;
                shihabEmbed3dBtn.Click += (s, e) =>
                {
                    if (mehediDoc != null && sshihabb007ActivePageIdx >= 0 && sshihabb007ActivePageIdx < mehediDoc.Sshihabb007Pages.Count)
                    {
                        using (Bitmap mehediSnap = MehediOemEnginesAndConverters.RenderShihabOda3dCadViewport(480, 300, sshihabb007Yaw, shihabPitch, mehediZoom))
                        using (MemoryStream sshihabb007Ms = new MemoryStream())
                        {
                            mehediSnap.Save(sshihabb007Ms, System.Drawing.Imaging.ImageFormat.Png);
                            mehediDoc.Sshihabb007Pages[sshihabb007ActivePageIdx].MehediAnnotations.Add(new ShihabPdfAnnotationItem
                            {
                                MehediPageIndex = sshihabb007ActivePageIdx,
                                Sshihabb007Kind = MehediAnnotationKind.MehediImageOverlay,
                                ShihabBoundsPt = new RectangleF(72f, 240f, 340f, 210f),
                                Sshihabb007EmbeddedImageBytes = sshihabb007Ms.ToArray(),
                                Sshihabb007TextContent = "ODA 3D U3D/CAD Render"
                            });
                            mehediDoc.ShihabIsModifiedDirty = true;
                        }
                        if (shihabOnStamped != null) shihabOnStamped();
                        mehediDlg.Close();
                    }
                };

                Label mehediYawLbl = new Label { Text = "Yaw (°):", Location = new Point(16, 426), AutoSize = true };
                Label sshihabb007PitchLbl = new Label { Text = "Pitch (°):", Location = new Point(295, 426), AutoSize = true };

                mehediDlg.Controls.AddRange(new Control[] { sshihabb007Canvas, mehediYawLbl, mehediYawSlider, sshihabb007PitchLbl, sshihabb007PitchSlider, shihabEmbed3dBtn });
                shihabRedraw3D();
                ShihabThemePalette.ApplyMehediDarkWindowFrame(mehediDlg.Handle, ShihabThemePalette.IsShihabDarkTheme);
                mehediDlg.ShowDialog(shihabOwner);
            }
        }

        // =====================================================================
        // 5. SIDE-BY-SIDE COMPARE PDF DOCUMENTS STUDIO (ComparePDF.fpi)
        // =====================================================================
        public static void ShowShihabComparePdfStudio(IWin32Window shihabOwner, ShihabPdfDocumentModel mehediLeftDoc)
        {
            using (OpenFileDialog sshihabb007Ofd = new OpenFileDialog())
            {
                sshihabb007Ofd.Title = "Select Second PDF Document to Compare Against '" + mehediLeftDoc.Sshihabb007DisplayTitle + "' (ComparePDF.fpi)";
                sshihabb007Ofd.Filter = "PDF Documents (*.pdf)|*.pdf|All Files (*.*)|*.*";
                if (sshihabb007Ofd.ShowDialog(shihabOwner) != DialogResult.OK) return;

                ShihabPdfDocumentModel shihabRightDoc = MehediPdfCoreEngine.LoadShihabPdfFromFile(sshihabb007Ofd.FileName);

                using (Form mehediCmpDlg = new Form())
                {
                    mehediCmpDlg.Text = "Side-by-Side PDF Document Comparison (ComparePDF.fpi) — PDF PRO (@sshihabb007)";
                    mehediCmpDlg.Size = new Size(920, 560);
                    mehediCmpDlg.StartPosition = FormStartPosition.CenterParent;
                    mehediCmpDlg.BackColor = ShihabThemePalette.MehediWindowBg;
                    mehediCmpDlg.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

                    StringBuilder sshihabb007DiffSb = new StringBuilder();
                    sshihabb007DiffSb.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
                    sshihabb007DiffSb.AppendLine("  SHIHAB COMPARE-PDF DIFFERENTIAL REPORT (ComparePDF.fpi — @sshihabb007)");
                    sshihabb007DiffSb.AppendLine("═══════════════════════════════════════════════════════════════════════════════");
                    sshihabb007DiffSb.AppendLine("• Left Document  (A): " + mehediLeftDoc.Sshihabb007DisplayTitle + " (" + mehediLeftDoc.Sshihabb007Pages.Count + " pages, SHA256=" + mehediLeftDoc.MehediSha256Digest + ")");
                    sshihabb007DiffSb.AppendLine("• Right Document (B): " + shihabRightDoc.Sshihabb007DisplayTitle + " (" + shihabRightDoc.Sshihabb007Pages.Count + " pages, SHA256=" + shihabRightDoc.MehediSha256Digest + ")");
                    sshihabb007DiffSb.AppendLine("• Identical Digest? : " + (string.Equals(mehediLeftDoc.MehediSha256Digest, shihabRightDoc.MehediSha256Digest, StringComparison.OrdinalIgnoreCase) ? "YES (100% Byte-Identical)" : "NO (Content/Structure Deltas Found)"));
                    sshihabb007DiffSb.AppendLine();

                    int shihabMaxPages = Math.Max(mehediLeftDoc.Sshihabb007Pages.Count, shihabRightDoc.Sshihabb007Pages.Count);
                    for (int mehediP = 0; mehediP < shihabMaxPages; mehediP++)
                    {
                        string sshihabb007LeftText = mehediP < mehediLeftDoc.Sshihabb007Pages.Count ? (mehediLeftDoc.Sshihabb007Pages[mehediP].ShihabExtractedText ?? "") : "[Page Absent]";
                        string shihabRightText = mehediP < shihabRightDoc.Sshihabb007Pages.Count ? (shihabRightDoc.Sshihabb007Pages[mehediP].ShihabExtractedText ?? "") : "[Page Absent]";
                        bool mehediSame = string.Equals(sshihabb007LeftText.Trim(), shihabRightText.Trim(), StringComparison.Ordinal);

                        sshihabb007DiffSb.AppendFormat("── Page {0}: {1}\r\n", mehediP + 1, mehediSame ? "✔ TEXT IDENTICAL" : "⚠ TEXT MODIFIED");
                        if (!mehediSame)
                        {
                            sshihabb007DiffSb.AppendLine("   [-] Left  (A): " + (sshihabb007LeftText.Length > 140 ? sshihabb007LeftText.Substring(0, 140).Replace("\r", " ").Replace("\n", " ") + "..." : sshihabb007LeftText.Replace("\r", " ").Replace("\n", " ")));
                            sshihabb007DiffSb.AppendLine("   [+] Right (B): " + (shihabRightText.Length > 140 ? shihabRightText.Substring(0, 140).Replace("\r", " ").Replace("\n", " ") + "..." : shihabRightText.Replace("\r", " ").Replace("\n", " ")));
                        }
                    }

                    TextBox sshihabb007ReportBox = new TextBox
                    {
                        Dock = DockStyle.Fill,
                        Multiline = true,
                        ReadOnly = true,
                        ScrollBars = ScrollBars.Both,
                        BackColor = ShihabThemePalette.ShihabPanelBg,
                        ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                        Font = new Font("Consolas", 9.5f),
                        Text = sshihabb007DiffSb.ToString()
                    };

                    mehediCmpDlg.Controls.Add(sshihabb007ReportBox);
                    ShihabThemePalette.ApplyMehediDarkWindowFrame(mehediCmpDlg.Handle, ShihabThemePalette.IsShihabDarkTheme);
                    mehediCmpDlg.ShowDialog(shihabOwner);
                }
            }
        }
    }
}

