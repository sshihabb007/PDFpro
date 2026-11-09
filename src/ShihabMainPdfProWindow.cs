using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ShihabMehediPdfPro
{
    public sealed class ShihabMainPdfProWindow : Form
    {
        private readonly MehediAppConfiguration shihabConfig;
        private readonly MehediSensorOpenTracingEngine mehediSensor;
        private readonly ShihabPlgDynLoader sshihabb007PluginLoader;
        private readonly List<ShihabPdfDocumentModel> shihabOpenDocuments = new List<ShihabPdfDocumentModel>();
        private readonly List<string> mehediDiscoveredLocalPdfFiles = new List<string>();

        private int mehediActiveDocIndex = 0;
        private int sshihabb007ActivePageIndex = 0;
        private float shihabZoomScale = 1.25f;
        private ShihabPdfToolMode mehediCurrentTool = ShihabPdfToolMode.ShihabHandPan;
        private string sshihabb007ActiveRibbonTab = "Edit";
        private bool shihabIsBackstageVisible = false;
        private string mehediActiveBackstageMenu = "Open";
        private bool sshihabb007IsRibbonCollapsed = false;
        private bool shihabIsLeftDrawerExpanded = false;
        private string mehediActiveLeftRailMode = "Thumbnails";
        private bool sshihabb007IsRightAiDrawerOpen = false;

        private int mehediSelectedAnnotationIdx = -1;
        private int sshihabb007SelectedTextBlockIdx = -1;

        // Interactive Mouse Drawing & Hand Panning State on Page Canvas
        private bool shihabIsMouseDragging = false;
        private Point mehediPanMouseStartScreen = Point.Empty;
        private Point sshihabb007PanScrollStart = Point.Empty;
        private PointF mehediMouseDownPt = PointF.Empty;
        private PointF sshihabb007MouseCurrentPt = PointF.Empty;
        private List<PointF> shihabCurrentInkStrokePts = new List<PointF>();

        // Top-Level UI Layout Panels
        private Panel mehediTopPurpleTitleBar;
        private Label sshihabb007TopDocTitleLabel;
        private Panel shihabTopSearchContainer;
        private TextBox mehediTopSearchInputBox;
        private Button sshihabb007ThemeSwitchPillBtn;
        private Button shihabUpdatePillBtn;
        private Button mehediCollapseRibbonChevronBtn;

        private Panel sshihabb007RibbonTabStripPanel;
        private Panel shihabRibbonCommandsBarPanel;
        private Panel mehediDocTabStripPanel;

        // Backstage ("File" Tab) Overlay Panel
        private Panel sshihabb007BackstageHostPanel;
        private Panel shihabBackstageLeftNavPanel;
        private Panel mehediBackstageMiddleSubPanel;
        private Panel sshihabb007BackstageRightListPanel;

        // Main Document Workspace Controls
        private Panel shihabMainWorkspaceHostPanel;
        private Panel mehediLeftIconRailPanel;
        private Panel sshihabb007LeftSlideOutDrawerPanel;
        private Label shihabLeftDrawerTitleLabel;
        private ListBox mehediLeftNavigationList;

        private Panel sshihabb007CenterCanvasScrollHost;
        private PictureBox shihabPagePictureBox;
        private Button mehediFloatingRobotBadgeBtn;

        private Panel sshihabb007RightAiDrawerPanel;
        private TextBox shihabAiOutputConsoleBox;
        private TextBox mehediAiQuestionInputBox;
        private TextBox sshihabb007FxjseOutputBox;
        private TextBox shihabFxjseCommandInputBox;

        // Bottom Status Bar Controls (Exact Foxit Layout)
        private Panel mehediBottomStatusBar;
        private TextBox sshihabb007PageNumberInputBox;
        private Label shihabTotalPagesStatusLabel;
        private Label mehediToolHintStatusLabel;
        private TrackBar sshihabb007ZoomSliderBar;
        private Button shihabZoomPercentButton;

        public ShihabPdfDocumentModel ShihabActiveDoc
        {
            get
            {
                if (shihabOpenDocuments.Count == 0) return null;
                if (mehediActiveDocIndex < 0 || mehediActiveDocIndex >= shihabOpenDocuments.Count)
                    mehediActiveDocIndex = 0;
                return shihabOpenDocuments[mehediActiveDocIndex];
            }
        }

        public ShihabMainPdfProWindow(
            MehediAppConfiguration shihabAppConfig,
            MehediSensorOpenTracingEngine mehediTelemetry,
            ShihabPlgDynLoader sshihabb007Loader,
            string shihabInitialPdfFilePath)
        {
            this.shihabConfig = shihabAppConfig ?? new MehediAppConfiguration();
            this.mehediSensor = mehediTelemetry;
            this.sshihabb007PluginLoader = sshihabb007Loader;
            this.shihabZoomScale = 1.25f;

            DiscoverShihabLocalRecentPdfFiles();
            InitializeShihabFoxitProUiStructure();

            if (!string.IsNullOrWhiteSpace(shihabInitialPdfFilePath) && File.Exists(shihabInitialPdfFilePath))
            {
                OpenShihabPdfFileIntoNewTab(shihabInitialPdfFilePath);
            }
            else if (mehediDiscoveredLocalPdfFiles.Count > 0)
            {
                // Automatically open the first discovered real PDF or our showcase PDF so the user immediately sees a real document!
                string sshihabb007FirstPdf = mehediDiscoveredLocalPdfFiles[0];
                OpenShihabPdfFileIntoNewTab(sshihabb007FirstPdf);
            }
            else
            {
                ShihabPdfDocumentModel mehediShowcase = MehediPdfCoreEngine.CreateShihabShowcaseDocument("Starship");
                shihabOpenDocuments.Add(mehediShowcase);
                mehediActiveDocIndex = 0;
            }

            RefreshAllShihabWorkstationPanels();
        }

        private void DiscoverShihabLocalRecentPdfFiles()
        {
            mehediDiscoveredLocalPdfFiles.Clear();
            List<string> shihabCandidateDirs = new List<string>
            {
                @"C:\Users\MEHEDI\Downloads\Chrome",
                @"D:\xampp\htdocs\REA\Google_Gemini_Level2",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "Chrome"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "samples")
            };

            HashSet<string> mehediSeen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<FileInfo> shihabCollectedInfos = new List<FileInfo>();
            foreach (string sshihabb007Dir in shihabCandidateDirs)
            {
                if (string.IsNullOrWhiteSpace(sshihabb007Dir) || !Directory.Exists(sshihabb007Dir)) continue;
                try
                {
                    string[] shihabPdfs = Directory.GetFiles(sshihabb007Dir, "*.pdf", SearchOption.TopDirectoryOnly);
                    foreach (string mehediPdfPath in shihabPdfs)
                    {
                        if (mehediSeen.Add(mehediPdfPath))
                        {
                            try { shihabCollectedInfos.Add(new FileInfo(mehediPdfPath)); } catch { }
                        }
                    }
                }
                catch { }
            }

            shihabCollectedInfos.Sort((shihabA, mehediB) =>
            {
                bool sshihabb007AGoogle = shihabA.Name.IndexOf("Google for Education Level 2", StringComparison.OrdinalIgnoreCase) >= 0;
                bool shihabBGoogle = mehediB.Name.IndexOf("Google for Education Level 2", StringComparison.OrdinalIgnoreCase) >= 0;
                if (sshihabb007AGoogle != shihabBGoogle) return sshihabb007AGoogle ? -1 : 1;
                return mehediB.LastWriteTimeUtc.CompareTo(shihabA.LastWriteTimeUtc);
            });

            for (int mehediIdx = 0; mehediIdx < shihabCollectedInfos.Count && mehediDiscoveredLocalPdfFiles.Count < 18; mehediIdx++)
            {
                mehediDiscoveredLocalPdfFiles.Add(shihabCollectedInfos[mehediIdx].FullName);
            }
        }

        private void InitializeShihabFoxitProUiStructure()
        {
            this.Text = "PDF PRO 14.0.4 — Foxit Starship Edition (Mehedi Hasan Shihab @sshihabb007)";
            this.Size = new Size(1480, 920);
            this.MinimumSize = new Size(1024, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.AllowDrop = true;
            this.BackColor = ShihabThemePalette.MehediWindowBg;
            this.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

            try
            {
                string shihabIcoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "pdfpro.ico");
                if (File.Exists(shihabIcoPath))
                {
                    this.Icon = new Icon(shihabIcoPath);
                }
            }
            catch { }

            this.DragEnter += (shihabS, mehediE) =>
            {
                if (mehediE.Data != null && mehediE.Data.GetDataPresent(DataFormats.FileDrop))
                    mehediE.Effect = DragDropEffects.Copy;
            };
            this.DragDrop += (shihabS, mehediE) =>
            {
                if (mehediE.Data != null && mehediE.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] sshihabb007Files = mehediE.Data.GetData(DataFormats.FileDrop) as string[];
                    if (sshihabb007Files != null)
                    {
                        foreach (string shihabFile in sshihabb007Files)
                        {
                            if (File.Exists(shihabFile) && shihabFile.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                OpenShihabPdfFileIntoNewTab(shihabFile);
                            }
                        }
                    }
                }
            };

            // =================================================================
            // 1. TOP PURPLE HEADER BAR (Exact Foxit Quick-Access + Centered Search + Update/Theme Pills)
            // =================================================================
            mehediTopPurpleTitleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = ShihabThemePalette.ShihabTopPurpleHeaderBg
            };

            int shihabQuickX = 8;
            Button mehediLogoQuickBtn = CreateShihabTopHeaderIconBtn("⚡", "PDF PRO 14.0.4 (@sshihabb007)", ref shihabQuickX, () => ShowShihabAboutBox());
            mehediLogoQuickBtn.BackColor = Color.FromArgb(40, 255, 255, 255);
            Button sshihabb007OpenQuickBtn = CreateShihabTopHeaderIconBtn("📂", "Open PDF File (Ctrl+O)", ref shihabQuickX, PromptAndOpenShihabPdfFile);
            Button shihabSaveQuickBtn = CreateShihabTopHeaderIconBtn("💾", "Save PDF (Ctrl+S)", ref shihabQuickX, SaveCurrentShihabDocument);
            Button mehediPrintQuickBtn = CreateShihabTopHeaderIconBtn("🖨", "Print / Export PDF", ref shihabQuickX, () => ExportActiveShihabDoc("docx"));
            Button sshihabb007NewQuickBtn = CreateShihabTopHeaderIconBtn("📄+", "Create New Blank PDF", ref shihabQuickX, CreateNewBlankShihabPdfTab);
            Button shihabUndoQuickBtn = CreateShihabTopHeaderIconBtn("↩", "Undo Last Markup", ref shihabQuickX, UndoLastShihabAnnotationOnPage);
            Button mehediRedoQuickBtn = CreateShihabTopHeaderIconBtn("↪", "Rotate Page 90° CW", ref shihabQuickX, () =>
            {
                if (ShihabActiveDoc != null)
                {
                    MehediPdfCoreEngine.RotateShihabPage(ShihabActiveDoc, sshihabb007ActivePageIndex, 90);
                    ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
                    RefreshAllShihabWorkstationPanels();
                }
            });

            sshihabb007TopDocTitleLabel = new Label
            {
                Text = "|   PDF PRO — Google for Education Level 2 — Easy Guide",
                Location = new Point(shihabQuickX + 6, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Regular),
                ForeColor = Color.White
            };

            // Centered Rounded Search Bar on Top Purple Bar
            shihabTopSearchContainer = new Panel
            {
                Size = new Size(320, 26),
                Location = new Point(560, 6),
                BackColor = ShihabThemePalette.MehediTopSearchBoxBg
            };
            Label mehediSearchMagLbl = new Label
            {
                Text = "🔍",
                Location = new Point(6, 4),
                AutoSize = true,
                ForeColor = Color.FromArgb(230, 255, 255, 255),
                Font = new Font("Segoe UI", 8.8f)
            };
            mehediTopSearchInputBox = new TextBox
            {
                Location = new Point(28, 4),
                Size = new Size(284, 20),
                BorderStyle = BorderStyle.None,
                BackColor = ShihabThemePalette.MehediTopSearchBoxBg,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.2f),
                Text = "Search"
            };
            mehediTopSearchInputBox.GotFocus += (s, e) =>
            {
                if (mehediTopSearchInputBox.Text == "Search") mehediTopSearchInputBox.Text = string.Empty;
            };
            mehediTopSearchInputBox.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(mehediTopSearchInputBox.Text)) mehediTopSearchInputBox.Text = "Search";
            };
            mehediTopSearchInputBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ExecuteShihabLuceneQuickSearch(mehediTopSearchInputBox.Text);
                }
            };
            shihabTopSearchContainer.Controls.Add(mehediSearchMagLbl);
            shihabTopSearchContainer.Controls.Add(mehediTopSearchInputBox);

            // Right Side Pills on Top Purple Bar (Update Pill + Bright/Dark Theme Pill + Collapse Ribbon)
            shihabUpdatePillBtn = new Button
            {
                Text = "⬆ Update",
                Size = new Size(76, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(62, 148, 36),
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            shihabUpdatePillBtn.FlatAppearance.BorderSize = 0;
            shihabUpdatePillBtn.Click += (s, e) => ShihabDialogsAndToolsWindows.ShowMehediPluginAndTelemetryStudio(this, sshihabb007PluginLoader, mehediSensor);

            sshihabb007ThemeSwitchPillBtn = new Button
            {
                Text = "🌙 Dark Mode",
                Size = new Size(98, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(48, 255, 255, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            sshihabb007ThemeSwitchPillBtn.FlatAppearance.BorderSize = 1;
            sshihabb007ThemeSwitchPillBtn.FlatAppearance.BorderColor = Color.FromArgb(110, 255, 255, 255);
            sshihabb007ThemeSwitchPillBtn.Click += (s, e) => ToggleShihabBrightAndDarkTheme();

            mehediCollapseRibbonChevronBtn = new Button
            {
                Text = "⌃",
                Size = new Size(28, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            mehediCollapseRibbonChevronBtn.FlatAppearance.BorderSize = 0;
            mehediCollapseRibbonChevronBtn.Click += (s, e) =>
            {
                sshihabb007IsRibbonCollapsed = !sshihabb007IsRibbonCollapsed;
                shihabRibbonCommandsBarPanel.Visible = !sshihabb007IsRibbonCollapsed && !shihabIsBackstageVisible;
                mehediCollapseRibbonChevronBtn.Text = sshihabb007IsRibbonCollapsed ? "⌄" : "⌃";
            };

            mehediTopPurpleTitleBar.Controls.AddRange(new Control[]
            {
                mehediLogoQuickBtn,
                sshihabb007OpenQuickBtn,
                shihabSaveQuickBtn,
                mehediPrintQuickBtn,
                sshihabb007NewQuickBtn,
                shihabUndoQuickBtn,
                mehediRedoQuickBtn,
                sshihabb007TopDocTitleLabel,
                shihabTopSearchContainer,
                shihabUpdatePillBtn,
                sshihabb007ThemeSwitchPillBtn,
                mehediCollapseRibbonChevronBtn
            });

            mehediTopPurpleTitleBar.Resize += (s, e) =>
            {
                int sshihabb007W = mehediTopPurpleTitleBar.ClientSize.Width;
                shihabTopSearchContainer.Location = new Point(Math.Max(420, (sshihabb007W - shihabTopSearchContainer.Width) / 2), 6);
                mehediCollapseRibbonChevronBtn.Location = new Point(sshihabb007W - 36, 7);
                sshihabb007ThemeSwitchPillBtn.Location = new Point(sshihabb007W - 140, 7);
                shihabUpdatePillBtn.Location = new Point(sshihabb007W - 222, 7);
            };

            // =================================================================
            // 2. RIBBON MENU TAB STRIP (File, Home, Convert, Edit, Organize, Comment, View, Form, Protect, Share, Accessibility, Help)
            // =================================================================
            sshihabb007RibbonTabStripPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = ShihabThemePalette.MehediWindowBg
            };

            // =================================================================
            // 3. TALL FOXIT ICON-ABOVE-LABEL RIBBON COMMAND BAR (72px)
            // =================================================================
            shihabRibbonCommandsBarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = ShihabThemePalette.Sshihabb007RibbonBg,
                AutoScroll = true
            };
            shihabRibbonCommandsBarPanel.Paint += (shihabS, mehediE) =>
            {
                using (Pen sshihabb007BottomPen = new Pen(ShihabThemePalette.MehediBorderColor, 1f))
                {
                    mehediE.Graphics.DrawLine(sshihabb007BottomPen, 0, shihabRibbonCommandsBarPanel.Height - 1, shihabRibbonCommandsBarPanel.Width, shihabRibbonCommandsBarPanel.Height - 1);
                }
            };

            // =================================================================
            // 4. DOCUMENT TAB STRIP (Start | Document.pdf × | +)
            // =================================================================
            mehediDocTabStripPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 29,
                BackColor = ShihabThemePalette.ShihabDocTabStripBg
            };

            // =================================================================
            // 5. BOTTOM STATUS BAR (Exact Foxit Page Nav + View Modes + Zoom Slider)
            // =================================================================
            mehediBottomStatusBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 28,
                BackColor = ShihabThemePalette.Sshihabb007RibbonBg
            };
            BuildShihabBottomStatusBarControls();

            // =================================================================
            // 6. MAIN WORKSPACE HOST (Left Narrow Icon Rail + Collapsible Drawer + Center PDF Canvas + Right AI Drawer)
            // =================================================================
            shihabMainWorkspaceHostPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ShihabThemePalette.MehediCanvasWorkspaceBg
            };

            // 6A. Far-Left Narrow Vertical Icon Rail (38px wide — Exact Foxit Left Sidebar!)
            mehediLeftIconRailPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 38,
                BackColor = ShihabThemePalette.MehediWindowBg
            };
            BuildShihabLeftIconRailControls();

            // 6B. Collapsible Left Slide-Out Drawer (245px wide, collapsed by default like Screenshots 1, 3, 4!)
            sshihabb007LeftSlideOutDrawerPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 245,
                Visible = false,
                BackColor = ShihabThemePalette.ShihabPanelBg,
                Padding = new Padding(6)
            };

            Panel shihabDrawerHeaderRow = new Panel { Dock = DockStyle.Top, Height = 28 };
            shihabLeftDrawerTitleLabel = new Label
            {
                Text = "Page Thumbnails",
                Location = new Point(4, 5),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Bold),
                ForeColor = ShihabThemePalette.ShihabPrimaryAccent
            };
            Button mehediCloseDrawerBtn = new Button
            {
                Text = "◂",
                Dock = DockStyle.Right,
                Width = 28,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = ShihabThemePalette.ShihabTextSecondary,
                Cursor = Cursors.Hand
            };
            mehediCloseDrawerBtn.FlatAppearance.BorderSize = 0;
            mehediCloseDrawerBtn.Click += (s, e) =>
            {
                shihabIsLeftDrawerExpanded = false;
                sshihabb007LeftSlideOutDrawerPanel.Visible = false;
                CenterShihabPagePictureBoxInViewport();
            };
            shihabDrawerHeaderRow.Controls.Add(shihabLeftDrawerTitleLabel);
            shihabDrawerHeaderRow.Controls.Add(mehediCloseDrawerBtn);

            mehediLeftNavigationList = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = ShihabThemePalette.ShihabPanelBg,
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.0f),
                IntegralHeight = false,
                ItemHeight = 24
            };
            mehediLeftNavigationList.SelectedIndexChanged += OnShihabLeftNavigationSelectedIndexChanged;

            sshihabb007LeftSlideOutDrawerPanel.Controls.Add(mehediLeftNavigationList);
            sshihabb007LeftSlideOutDrawerPanel.Controls.Add(shihabDrawerHeaderRow);

            // 6C. Collapsible Right AI & FXJSE Console Drawer (330px wide, toggled via Floating Robot Badge or AI Tab)
            sshihabb007RightAiDrawerPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 330,
                Visible = false,
                BackColor = ShihabThemePalette.ShihabPanelBg,
                Padding = new Padding(8)
            };
            BuildShihabRightAiAndFxjseDrawer();

            // 6D. Center Scrollable PDF Viewport Canvas
            sshihabb007CenterCanvasScrollHost = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ShihabThemePalette.MehediCanvasWorkspaceBg,
                Padding = new Padding(20)
            };

            shihabPagePictureBox = new PictureBox
            {
                Location = new Point(40, 18),
                SizeMode = PictureBoxSizeMode.AutoSize,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            // Subtle Page Border & Drop Shadow on the PDF Page Card
            shihabPagePictureBox.Paint += OnMehediCanvasLiveRubberbandPaint;
            shihabPagePictureBox.MouseDown += OnShihabCanvasMouseDown;
            shihabPagePictureBox.MouseMove += OnMehediCanvasMouseMove;
            shihabPagePictureBox.MouseUp += OnSshihabb007CanvasMouseUp;
            shihabPagePictureBox.DoubleClick += OnShihabCanvasDoubleClick;

            // Floating Purple AI Assistant Badge in Top-Right of Viewport (Exact Match to Screenshot 1!)
            mehediFloatingRobotBadgeBtn = new Button
            {
                Text = "🤖",
                Size = new Size(38, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = ShihabThemePalette.ShihabPrimaryAccent,
                Font = new Font("Segoe UI Emoji", 13.0f),
                Cursor = Cursors.Hand
            };
            mehediFloatingRobotBadgeBtn.FlatAppearance.BorderColor = ShihabThemePalette.MehediBorderColor;
            mehediFloatingRobotBadgeBtn.FlatAppearance.BorderSize = 1;
            mehediFloatingRobotBadgeBtn.Click += (s, e) =>
            {
                sshihabb007IsRightAiDrawerOpen = !sshihabb007IsRightAiDrawerOpen;
                sshihabb007RightAiDrawerPanel.Visible = sshihabb007IsRightAiDrawerOpen;
                CenterShihabPagePictureBoxInViewport();
            };

            sshihabb007CenterCanvasScrollHost.Controls.Add(mehediFloatingRobotBadgeBtn);
            sshihabb007CenterCanvasScrollHost.Controls.Add(shihabPagePictureBox);
            sshihabb007CenterCanvasScrollHost.Resize += (s, e) =>
            {
                CenterShihabPagePictureBoxInViewport();
                mehediFloatingRobotBadgeBtn.Location = new Point(Math.Max(16, sshihabb007CenterCanvasScrollHost.ClientSize.Width - 54), 14);
            };
            sshihabb007CenterCanvasScrollHost.MouseWheel += OnShihabViewportMouseWheel;

            shihabMainWorkspaceHostPanel.Controls.Add(sshihabb007CenterCanvasScrollHost);
            shihabMainWorkspaceHostPanel.Controls.Add(sshihabb007LeftSlideOutDrawerPanel);
            shihabMainWorkspaceHostPanel.Controls.Add(mehediLeftIconRailPanel);
            shihabMainWorkspaceHostPanel.Controls.Add(sshihabb007RightAiDrawerPanel);

            // =================================================================
            // 7. FULL-WINDOW "FILE" BACKSTAGE VIEW (Exact Match to Screenshot 2!)
            // =================================================================
            sshihabb007BackstageHostPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                BackColor = ShihabThemePalette.MehediWindowBg
            };
            BuildShihabFileBackstagePanel();

            // Add Top-Level Panels in WinForms Dock Order
            this.Controls.Add(sshihabb007BackstageHostPanel);
            this.Controls.Add(shihabMainWorkspaceHostPanel);
            this.Controls.Add(mehediBottomStatusBar);
            this.Controls.Add(mehediDocTabStripPanel);
            this.Controls.Add(shihabRibbonCommandsBarPanel);
            this.Controls.Add(sshihabb007RibbonTabStripPanel);
            this.Controls.Add(mehediTopPurpleTitleBar);

            this.KeyDown += OnShihabGlobalShortcutKeyDown;

            RebuildShihabRibbonTabStrip();
            RebuildShihabRibbonButtonsForActiveTab();
            ApplyShihabThemeToAllControls();
        }

        private Button CreateShihabTopHeaderIconBtn(string shihabGlyph, string mehediTooltipText, ref int sshihabb007CursorX, Action shihabClickAction)
        {
            Button mehediBtn = new Button
            {
                Text = shihabGlyph,
                Location = new Point(sshihabb007CursorX, 6),
                Size = new Size(30, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Symbol", 9.5f),
                Cursor = Cursors.Hand
            };
            mehediBtn.FlatAppearance.BorderSize = 0;
            mehediBtn.FlatAppearance.MouseOverBackColor = Color.FromArgb(55, 255, 255, 255);
            ToolTip sshihabb007Tip = new ToolTip();
            sshihabb007Tip.SetToolTip(mehediBtn, mehediTooltipText);
            mehediBtn.Click += (s, e) => shihabClickAction();
            sshihabb007CursorX += 32;
            return mehediBtn;
        }

        // =====================================================================
        // RIBBON TAB STRIP & TALL VECTOR-ICON RIBBON BUTTONS (Screenshots 3 & 4)
        // =====================================================================
        private void RebuildShihabRibbonTabStrip()
        {
            sshihabb007RibbonTabStripPanel.SuspendLayout();
            sshihabb007RibbonTabStripPanel.Controls.Clear();

            string[] shihabTabs = new[]
            {
                "File", "Home", "Convert", "Edit", "Organize", "Comment",
                "View", "Form", "Protect", "Share", "Accessibility", "Help"
            };

            int mehediX = 6;
            foreach (string sshihabb007Tab in shihabTabs)
            {
                string shihabCap = sshihabb007Tab;
                bool mehediIsSelected = (shihabIsBackstageVisible && shihabCap == "File") ||
                                        (!shihabIsBackstageVisible && string.Equals(shihabCap, sshihabb007ActiveRibbonTab, StringComparison.OrdinalIgnoreCase));

                Button sshihabb007TabBtn = new Button
                {
                    Text = shihabCap,
                    Location = new Point(mehediX, 1),
                    AutoSize = true,
                    Height = 28,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = ShihabThemePalette.MehediWindowBg,
                    ForeColor = mehediIsSelected ? ShihabThemePalette.ShihabPrimaryAccent : ShihabThemePalette.Sshihabb007TextPrimary,
                    Font = new Font("Segoe UI", 8.8f, mehediIsSelected ? FontStyle.Bold : FontStyle.Regular),
                    Cursor = Cursors.Hand
                };
                sshihabb007TabBtn.FlatAppearance.BorderSize = 0;

                if (mehediIsSelected)
                {
                    sshihabb007TabBtn.Paint += (shihabS, mehediE) =>
                    {
                        Control sshihabb007C = shihabS as Control;
                        using (Pen shihabUnderlinePen = new Pen(ShihabThemePalette.ShihabPrimaryAccent, 2.5f))
                        {
                            mehediE.Graphics.DrawLine(shihabUnderlinePen, 6, sshihabb007C.Height - 2, sshihabb007C.Width - 6, sshihabb007C.Height - 2);
                        }
                    };
                }

                sshihabb007TabBtn.Click += (s, e) =>
                {
                    if (shihabCap == "File")
                    {
                        shihabIsBackstageVisible = !shihabIsBackstageVisible;
                        SetShihabBackstageVisibility(shihabIsBackstageVisible);
                    }
                    else
                    {
                        shihabIsBackstageVisible = false;
                        sshihabb007ActiveRibbonTab = shihabCap;
                        SetShihabBackstageVisibility(false);
                        RebuildShihabRibbonButtonsForActiveTab();
                    }
                    RebuildShihabRibbonTabStrip();
                };

                sshihabb007RibbonTabStripPanel.Controls.Add(sshihabb007TabBtn);
                mehediX += sshihabb007TabBtn.PreferredSize.Width + 4;
            }

            sshihabb007RibbonTabStripPanel.ResumeLayout();
        }

        private void SetShihabBackstageVisibility(bool mehediShowBackstage)
        {
            shihabIsBackstageVisible = mehediShowBackstage;
            sshihabb007BackstageHostPanel.Visible = mehediShowBackstage;
            shihabMainWorkspaceHostPanel.Visible = !mehediShowBackstage;
            shihabRibbonCommandsBarPanel.Visible = !mehediShowBackstage && !sshihabb007IsRibbonCollapsed;
            mehediDocTabStripPanel.Visible = !mehediShowBackstage;
            mehediBottomStatusBar.Visible = !mehediShowBackstage;

            if (mehediShowBackstage)
            {
                sshihabb007BackstageHostPanel.BringToFront();
                RefreshShihabBackstageRecentDocumentsList();
            }
            else
            {
                shihabMainWorkspaceHostPanel.BringToFront();
            }
        }

        private void AddShihabTallRibbonIconBtn(ref int shihabCursorX, string mehediIconCode, string sshihabb007TitleLine1, string shihabTitleLine2, bool mehediIsSelected, Action sshihabb007OnClick)
        {
            int shihabBtnW = Math.Max(54, Math.Max(sshihabb007TitleLine1.Length, (shihabTitleLine2 ?? "").Length) * 6 + 18);
            Button mehediBtn = new Button
            {
                Location = new Point(shihabCursorX, 4),
                Size = new Size(shihabBtnW, 62),
                FlatStyle = FlatStyle.Flat,
                BackColor = mehediIsSelected ? ShihabThemePalette.Sshihabb007CardHoverBg : Color.Transparent,
                Cursor = Cursors.Hand
            };
            mehediBtn.FlatAppearance.BorderSize = mehediIsSelected ? 1 : 0;
            mehediBtn.FlatAppearance.BorderColor = ShihabThemePalette.ShihabPrimaryAccent;
            mehediBtn.FlatAppearance.MouseOverBackColor = ShihabThemePalette.Sshihabb007CardHoverBg;

            mehediBtn.Paint += (shihabS, mehediE) =>
            {
                Graphics sshihabb007G = mehediE.Graphics;
                sshihabb007G.SmoothingMode = SmoothingMode.AntiAlias;
                sshihabb007G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Draw Vector Icon at top-center (24x24 box)
                Rectangle shihabIconBox = new Rectangle((mehediBtn.Width - 24) / 2, 6, 24, 22);
                DrawShihabVectorRibbonGlyph(sshihabb007G, shihabIconBox, mehediIconCode, mehediIsSelected);

                // Draw 1- or 2-line Label underneath
                using (Font mehediLblFont = new Font("Segoe UI", 7.8f, mehediIsSelected ? FontStyle.Bold : FontStyle.Regular))
                using (SolidBrush sshihabb007TextBrush = new SolidBrush(mehediIsSelected ? ShihabThemePalette.ShihabPrimaryAccent : ShihabThemePalette.Sshihabb007TextPrimary))
                using (StringFormat shihabSf = new StringFormat { Alignment = StringAlignment.Center })
                {
                    sshihabb007G.DrawString(sshihabb007TitleLine1, mehediLblFont, sshihabb007TextBrush, new RectangleF(0, 32, mehediBtn.Width, 14), shihabSf);
                    if (!string.IsNullOrEmpty(shihabTitleLine2))
                    {
                        sshihabb007G.DrawString(shihabTitleLine2, mehediLblFont, sshihabb007TextBrush, new RectangleF(0, 45, mehediBtn.Width, 14), shihabSf);
                    }
                }
            };

            mehediBtn.Click += (s, e) => sshihabb007OnClick();
            shihabRibbonCommandsBarPanel.Controls.Add(mehediBtn);
            shihabCursorX += shihabBtnW + 2;
        }

        private void AddMehediRibbonGroupDivider(ref int shihabCursorX)
        {
            Panel mehediSep = new Panel
            {
                Location = new Point(shihabCursorX + 3, 10),
                Size = new Size(1, 50),
                BackColor = ShihabThemePalette.MehediBorderColor
            };
            shihabRibbonCommandsBarPanel.Controls.Add(mehediSep);
            shihabCursorX += 9;
        }

        private void DrawShihabVectorRibbonGlyph(Graphics shihabG, Rectangle mehediBox, string sshihabb007Code, bool shihabActive)
        {
            Color mehediStrokeColor = shihabActive
                ? ShihabThemePalette.ShihabPrimaryAccent
                : (ShihabThemePalette.IsShihabDarkTheme ? Color.FromArgb(208, 214, 226) : Color.FromArgb(68, 74, 88));
            Color sshihabb007AccentColor = ShihabThemePalette.ShihabPrimaryAccent;

            using (Pen shihabPen = new Pen(mehediStrokeColor, 1.5f))
            using (Pen mehediAccentPen = new Pen(sshihabb007AccentColor, 1.6f))
            using (Font sshihabb007MiniFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            using (SolidBrush shihabStrokeBrush = new SolidBrush(mehediStrokeColor))
            using (SolidBrush mehediAccentBrush = new SolidBrush(sshihabb007AccentColor))
            {
                // Draw clean vector page/tool outline + badge symbol
                shihabG.DrawRectangle(shihabPen, mehediBox.X + 2, mehediBox.Y + 1, mehediBox.Width - 4, mehediBox.Height - 2);
                StringFormat sshihabb007Sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                shihabG.DrawString(sshihabb007Code, sshihabb007MiniFont, mehediAccentBrush, mehediBox, sshihabb007Sf);
            }
        }

        private void RebuildShihabRibbonButtonsForActiveTab()
        {
            shihabRibbonCommandsBarPanel.SuspendLayout();
            shihabRibbonCommandsBarPanel.Controls.Clear();
            int shihabX = 8;

            // Every Foxit tab starts with Hand & Select on the far left (Screenshots 3 & 4!)
            AddShihabTallRibbonIconBtn(ref shihabX, "✋", "Hand", "", mehediCurrentTool == ShihabPdfToolMode.ShihabHandPan, () => SetShihabToolMode(ShihabPdfToolMode.ShihabHandPan));
            AddShihabTallRibbonIconBtn(ref shihabX, "I cursor", "Select", "▾", mehediCurrentTool == ShihabPdfToolMode.MehediSelectText, () => SetShihabToolMode(ShihabPdfToolMode.MehediSelectText));
            AddMehediRibbonGroupDivider(ref shihabX);

            switch (sshihabb007ActiveRibbonTab)
            {
                case "Home":
                    AddShihabTallRibbonIconBtn(ref shihabX, "📂", "Open", "PDF", false, PromptAndOpenShihabPdfFile);
                    AddShihabTallRibbonIconBtn(ref shihabX, "💾", "Save", "PDF", false, SaveCurrentShihabDocument);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "[T]", "Edit", "Text", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007EditContent, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007EditContent));
                    AddShihabTallRibbonIconBtn(ref shihabX, "T+", "Add", "Text", mehediCurrentTool == ShihabPdfToolMode.ShihabAddTextBox, () => SetShihabToolMode(ShihabPdfToolMode.ShihabAddTextBox));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🖍", "Highlight", "", mehediCurrentTool == ShihabPdfToolMode.MehediHighlight, () => SetShihabToolMode(ShihabPdfToolMode.MehediHighlight));
                    AddShihabTallRibbonIconBtn(ref shihabX, "📌", "Sticky", "Note", mehediCurrentTool == ShihabPdfToolMode.MehediStickyNote, () => SetShihabToolMode(ShihabPdfToolMode.MehediStickyNote));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔍+", "Zoom", "In", false, () => ChangeShihabZoom(0.15f));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔍-", "Zoom", "Out", false, () => ChangeShihabZoom(-0.15f));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔄", "Rotate", "Right", false, () => RotateActiveShihabPage(90));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "W/X", "To MS", "Office ▾", false, () => ExportActiveShihabDoc("docx"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "OCR", "Quick", "OCR", false, RunShihabOcrOnActivePage);
                    AddShihabTallRibbonIconBtn(ref shihabX, "✒", "Protect &", "Sign", false, SignActiveShihabPageQuick);
                    break;

                case "Convert": // Exact Foxit Convert Tab (Screenshot 4!)
                    AddShihabTallRibbonIconBtn(ref shihabX, "📄+", "From", "Files ▾", false, ImportExternalFilesIntoShihabPdf);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🖨", "From", "Scanner", false, ImportExternalFilesIntoShihabPdf);
                    AddShihabTallRibbonIconBtn(ref shihabX, "📋", "From", "Clipboard", false, CreatePdfFromShihabClipboard);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🌐", "From", "Web Page", false, CreateNewBlankShihabPdfTab);
                    AddShihabTallRibbonIconBtn(ref shihabX, "📑", "PDF", "Portfolio ▾", false, MergeAllOpenShihabDocuments);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔗", "Combine", "Files", false, MergeAllOpenShihabDocuments);
                    AddShihabTallRibbonIconBtn(ref shihabX, "➕", "Blank", "", false, CreateNewBlankShihabPdfTab);
                    AddShihabTallRibbonIconBtn(ref shihabX, "📐", "From", "Template", false, CreateNewShowcaseShihabTab);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🖼", "Export All", "Images", false, () => ExportActiveShihabDoc("png"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "W", "To MS", "Word ▾", false, () => ExportActiveShihabDoc("docx"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "X", "To MS", "Excel", false, () => ExportActiveShihabDoc("xlsx"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "PNG", "To", "Image ▾", false, () => ExportActiveShihabDoc("png"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "HTML", "To", "HTML", false, () => ExportActiveShihabDoc("html"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "CSV", "To", "Other ▾", false, () => ExportActiveShihabDoc("csv"));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "OCR", "Recognize", "Text ▾", false, RunShihabOcrOnActivePage);
                    AddShihabTallRibbonIconBtn(ref shihabX, "OCR⚡", "Quick", "Recognition", false, RunShihabOcrOnActivePage);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "PDF/A", "Preflight", "", false, () => ShihabDialogsAndToolsWindows.ShowShihabPreflightAndOptimizerStudio(this, ShihabActiveDoc, RefreshAllShihabWorkstationPanels));
                    break;

                case "Edit": // Exact Foxit Edit Tab (Screenshot 3!)
                    AddShihabTallRibbonIconBtn(ref shihabX, "[T]", "Edit", "Text", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007EditContent, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007EditContent));
                    AddShihabTallRibbonIconBtn(ref shihabX, "❐", "Edit", "Object ▾", false, EditSelectedShihabTextBlockModal);
                    AddShihabTallRibbonIconBtn(ref shihabX, "AB", "Link &", "Join Text", false, EditSelectedShihabTextBlockModal);
                    AddShihabTallRibbonIconBtn(ref shihabX, "ABC✓", "Check", "Spelling", false, () => ExecuteShihabLuceneQuickSearch(mehediTopSearchInputBox.Text));
                    AddShihabTallRibbonIconBtn(ref shihabX, "A/B", "Search &", "Replace", false, () => ExecuteShihabLuceneQuickSearch(mehediTopSearchInputBox.Text));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "T+", "Add", "Text", mehediCurrentTool == ShihabPdfToolMode.ShihabAddTextBox, () => SetShihabToolMode(ShihabPdfToolMode.ShihabAddTextBox));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🖼+", "Add", "Images ▾", false, InsertShihabImageOverlayOnActivePage);
                    AddShihabTallRibbonIconBtn(ref shihabX, "▭", "Add", "Shapes ▾", mehediCurrentTool == ShihabPdfToolMode.ShihabRectangleShape, () => SetShihabToolMode(ShihabPdfToolMode.ShihabRectangleShape));
                    AddShihabTallRibbonIconBtn(ref shihabX, "¶", "Reflow", "Editing", false, EditSelectedShihabTextBlockModal);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔗", "Link", "", mehediCurrentTool == ShihabPdfToolMode.ShihabRectangleShape, () => SetShihabToolMode(ShihabPdfToolMode.ShihabRectangleShape));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔖", "Bookmark", "", false, AddBookmarkForCurrentShihabPage);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔖+", "Auto-Create", "Bookmarks", false, () =>
                    {
                        MehediOemEnginesAndConverters.ApplyMehediPdfAConversionAndFixups(ShihabActiveDoc);
                        RefreshAllShihabWorkstationPanels();
                    });
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "📎", "File", "Attachment", false, ImportExternalFilesIntoShihabPdf);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🖼", "Image", "Annotation", false, InsertShihabImageOverlayOnActivePage);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🧊3D", "Add", "3D", false, () => ShihabDialogsAndToolsWindows.ShowMehediOda3dCadViewportStudio(this, ShihabActiveDoc, sshihabb007ActivePageIndex, RefreshAllShihabWorkstationPanels));
                    break;

                case "Organize":
                    AddShihabTallRibbonIconBtn(ref shihabX, "📄+", "Insert", "Blank Page", false, () =>
                    {
                        MehediPdfCoreEngine.InsertShihabBlankPage(ShihabActiveDoc, sshihabb007ActivePageIndex);
                        sshihabb007ActivePageIndex++;
                        RefreshAllShihabWorkstationPanels();
                    });
                    AddShihabTallRibbonIconBtn(ref shihabX, "📋", "Duplicate", "Page", false, () =>
                    {
                        MehediPdfCoreEngine.DuplicateMehediPage(ShihabActiveDoc, sshihabb007ActivePageIndex);
                        sshihabb007ActivePageIndex++;
                        RefreshAllShihabWorkstationPanels();
                    });
                    AddShihabTallRibbonIconBtn(ref shihabX, "🗑", "Delete", "Page", false, DeleteActiveShihabPage);
                    AddShihabTallRibbonIconBtn(ref shihabX, "✂", "Split", "PDF", false, SplitActiveShihabDocumentToFolder);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "↻", "Rotate", "Right", false, () => RotateActiveShihabPage(90));
                    AddShihabTallRibbonIconBtn(ref shihabX, "↺", "Rotate", "Left", false, () => RotateActiveShihabPage(-90));
                    AddShihabTallRibbonIconBtn(ref shihabX, "⬆", "Move", "Page Up", false, () => MoveActiveShihabPage(-1));
                    AddShihabTallRibbonIconBtn(ref shihabX, "⬇", "Move", "Page Down", false, () => MoveActiveShihabPage(1));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "💧", "Watermark", "▾", false, PromptShihabWatermarkText);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🏷", "Header &", "Footer", false, PromptShihabHeaderFooter);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔢", "Bates", "Numbering", false, () =>
                    {
                        MehediPdfCoreEngine.ApplyShihabBatesNumberingAllPages(ShihabActiveDoc, "SHIHAB-", 1001);
                        RefreshAllShihabWorkstationPanels();
                    });
                    break;

                case "Comment":
                    AddShihabTallRibbonIconBtn(ref shihabX, "🖍", "Highlight", "", mehediCurrentTool == ShihabPdfToolMode.MehediHighlight, () => SetShihabToolMode(ShihabPdfToolMode.MehediHighlight));
                    AddShihabTallRibbonIconBtn(ref shihabX, "U_", "Underline", "", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007Underline, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007Underline));
                    AddShihabTallRibbonIconBtn(ref shihabX, "S-", "Strikeout", "", mehediCurrentTool == ShihabPdfToolMode.ShihabStrikeout, () => SetShihabToolMode(ShihabPdfToolMode.ShihabStrikeout));
                    AddShihabTallRibbonIconBtn(ref shihabX, "📌", "Sticky", "Note", mehediCurrentTool == ShihabPdfToolMode.MehediStickyNote, () => SetShihabToolMode(ShihabPdfToolMode.MehediStickyNote));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "✍", "Freehand", "Pencil", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007FreehandInk, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007FreehandInk));
                    AddShihabTallRibbonIconBtn(ref shihabX, "▭", "Rectangle", "", mehediCurrentTool == ShihabPdfToolMode.ShihabRectangleShape, () => SetShihabToolMode(ShihabPdfToolMode.ShihabRectangleShape));
                    AddShihabTallRibbonIconBtn(ref shihabX, "◯", "Oval", "", mehediCurrentTool == ShihabPdfToolMode.MehediEllipseShape, () => SetShihabToolMode(ShihabPdfToolMode.MehediEllipseShape));
                    AddShihabTallRibbonIconBtn(ref shihabX, "↗", "Arrow", "", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007ArrowLine, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007ArrowLine));
                    AddShihabTallRibbonIconBtn(ref shihabX, "💬", "Callout", "", mehediCurrentTool == ShihabPdfToolMode.ShihabCalloutBox, () => SetShihabToolMode(ShihabPdfToolMode.ShihabCalloutBox));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "✅", "Stamps", "▾", mehediCurrentTool == ShihabPdfToolMode.ShihabStampApproved, () => SetShihabToolMode(ShihabPdfToolMode.ShihabStampApproved));
                    AddShihabTallRibbonIconBtn(ref shihabX, "📏", "Measure", "Distance", mehediCurrentTool == ShihabPdfToolMode.MehediMeasureRuler, () => SetShihabToolMode(ShihabPdfToolMode.MehediMeasureRuler));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🗑", "Delete", "Markup", false, DeleteSelectedShihabAnnotation);
                    break;

                case "View":
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔍", "Zoom", "100%", false, () => SetExactShihabZoom(1.0f));
                    AddShihabTallRibbonIconBtn(ref shihabX, "⤢", "Fit", "Page", false, FitShihabPageToViewportHeight);
                    AddShihabTallRibbonIconBtn(ref shihabX, "↔", "Fit", "Width", false, FitShihabPageToViewportWidth);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "⚖", "Compare", "Files", false, () => ShihabDialogsAndToolsWindows.ShowShihabComparePdfStudio(this, ShihabActiveDoc));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🤖", "AI & JS", "Assistant", sshihabb007IsRightAiDrawerOpen, () =>
                    {
                        sshihabb007IsRightAiDrawerOpen = !sshihabb007IsRightAiDrawerOpen;
                        sshihabb007RightAiDrawerPanel.Visible = sshihabb007IsRightAiDrawerOpen;
                        CenterShihabPagePictureBoxInViewport();
                    });
                    AddShihabTallRibbonIconBtn(ref shihabX, "🎨", "Switch", "Theme", false, ToggleShihabBrightAndDarkTheme);
                    break;

                case "Form":
                    AddShihabTallRibbonIconBtn(ref shihabX, "[Ab]", "Text", "Field", mehediCurrentTool == ShihabPdfToolMode.ShihabFormTextField, () => SetShihabToolMode(ShihabPdfToolMode.ShihabFormTextField));
                    AddShihabTallRibbonIconBtn(ref shihabX, "☑", "Check", "Box", mehediCurrentTool == ShihabPdfToolMode.MehediFormCheckBox, () => SetShihabToolMode(ShihabPdfToolMode.MehediFormCheckBox));
                    AddShihabTallRibbonIconBtn(ref shihabX, "▾", "Combo", "Box", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007FormComboBox, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007FormComboBox));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔘", "Push", "Button", mehediCurrentTool == ShihabPdfToolMode.ShihabFormPushButton, () => SetShihabToolMode(ShihabPdfToolMode.ShihabFormPushButton));
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "📤", "Export", "Form JSON", false, ExportShihabAcroFormDataJson);
                    AddShihabTallRibbonIconBtn(ref shihabX, "JS", "JavaScript", "Console", false, () =>
                    {
                        sshihabb007IsRightAiDrawerOpen = true;
                        sshihabb007RightAiDrawerPanel.Visible = true;
                        CenterShihabPagePictureBoxInViewport();
                    });
                    break;

                case "Protect":
                    AddShihabTallRibbonIconBtn(ref shihabX, "⬛", "Mark for", "Redaction", mehediCurrentTool == ShihabPdfToolMode.MehediRedactionMark, () => SetShihabToolMode(ShihabPdfToolMode.MehediRedactionMark));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🕵", "Search &", "Redact PII", false, AutoScanAndMarkShihabPii);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔥", "Apply", "Redactions", false, BurnAllPendingShihabRedactions);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔒", "Protect", "Password", false, ApplyShihabRmsProtectionQuick);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🛡", "AIP", "Labels", false, ApplyShihabRmsProtectionQuick);
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "✒", "Sign", "Document", false, SignActiveShihabPageQuick);
                    AddShihabTallRibbonIconBtn(ref shihabX, "✍", "Place", "Signature", mehediCurrentTool == ShihabPdfToolMode.Sshihabb007SignatureStamp, () => SetShihabToolMode(ShihabPdfToolMode.Sshihabb007SignatureStamp));
                    break;

                case "Share":
                    AddShihabTallRibbonIconBtn(ref shihabX, "☁", "cPDF", "Lineage", false, () =>
                    {
                        sshihabb007IsRightAiDrawerOpen = true;
                        sshihabb007RightAiDrawerPanel.Visible = true;
                        CenterShihabPagePictureBoxInViewport();
                    });
                    AddShihabTallRibbonIconBtn(ref shihabX, "📘", "Share as", "Word (.docx)", false, () => ExportActiveShihabDoc("docx"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "📊", "Share as", "Excel (.xlsx)", false, () => ExportActiveShihabDoc("xlsx"));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🌐", "Share as", "HTML5", false, () => ExportActiveShihabDoc("html"));
                    break;

                case "Accessibility":
                    AddShihabTallRibbonIconBtn(ref shihabX, "✔508", "Full", "Check", false, () => ShihabDialogsAndToolsWindows.ShowShihabPreflightAndOptimizerStudio(this, ShihabActiveDoc, RefreshAllShihabWorkstationPanels));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🏷+", "Autotag", "Document", false, () =>
                    {
                        MehediOemEnginesAndConverters.ApplyMehediPdfAConversionAndFixups(ShihabActiveDoc);
                        RefreshAllShihabWorkstationPanels();
                    });
                    AddMehediRibbonGroupDivider(ref shihabX);
                    AddShihabTallRibbonIconBtn(ref shihabX, "🔊", "Read", "Out Loud", false, () =>
                    {
                        if (ShihabActiveDoc != null)
                            ShihabSecuritySignAndAiEngine.SpeakShihabPageTextAloud(ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex].ShihabExtractedText);
                    });
                    AddShihabTallRibbonIconBtn(ref shihabX, "⏹", "Stop", "Reading", false, ShihabSecuritySignAndAiEngine.StopMehediSpeechReadOutLoud);
                    break;

                default: // Help
                    AddShihabTallRibbonIconBtn(ref shihabX, "🧩", "Plug-Ins", "Manager", false, () => ShihabDialogsAndToolsWindows.ShowMehediPluginAndTelemetryStudio(this, sshihabb007PluginLoader, mehediSensor));
                    AddShihabTallRibbonIconBtn(ref shihabX, "🌐", "Browser", "Extension", false, OpenShihabExtensionFolderInExplorer);
                    AddShihabTallRibbonIconBtn(ref shihabX, "ℹ", "About", "PDF PRO", false, ShowShihabAboutBox);
                    break;
            }

            shihabRibbonCommandsBarPanel.ResumeLayout();
        }

        // =====================================================================
        // FULL-WINDOW "FILE" BACKSTAGE VIEW (Exact Match to Screenshot 2!)
        // =====================================================================
        private void BuildShihabFileBackstagePanel()
        {
            shihabBackstageLeftNavPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 162,
                BackColor = ShihabThemePalette.ShihabPanelBg,
                Padding = new Padding(8, 12, 8, 12)
            };

            mehediBackstageMiddleSubPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 225,
                BackColor = ShihabThemePalette.MehediWindowBg,
                Padding = new Padding(16, 18, 16, 18)
            };

            sshihabb007BackstageRightListPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ShihabThemePalette.MehediWindowBg,
                Padding = new Padding(24, 18, 24, 18),
                AutoScroll = true
            };

            sshihabb007BackstageHostPanel.Controls.Add(sshihabb007BackstageRightListPanel);
            sshihabb007BackstageHostPanel.Controls.Add(mehediBackstageMiddleSubPanel);
            sshihabb007BackstageHostPanel.Controls.Add(shihabBackstageLeftNavPanel);
        }

        private void RefreshShihabBackstageRecentDocumentsList()
        {
            // 1. Rebuild Left Backstage Menu Column (Open, Create, Properties, Save, Save As, Print, Export, PDF Optimizer, Skins, Preferences...)
            shihabBackstageLeftNavPanel.SuspendLayout();
            shihabBackstageLeftNavPanel.Controls.Clear();

            string[] shihabBackstageItems = new[]
            {
                "Open", "Create", "Properties", "Save", "Save As", "Print",
                "Batch Print", "Share", "Revert", "Export", "Close",
                "PDF Optimizer", "Index", "Action Wizard", "Skins", "Preferences"
            };

            int mehediNavY = 8;
            foreach (string sshihabb007Item in shihabBackstageItems)
            {
                string shihabCap = sshihabb007Item;
                bool mehediSelected = string.Equals(shihabCap, mehediActiveBackstageMenu, StringComparison.OrdinalIgnoreCase);

                Button sshihabb007NavBtn = new Button
                {
                    Text = "  " + shihabCap,
                    Location = new Point(6, mehediNavY),
                    Size = new Size(148, 30),
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = mehediSelected ? ShihabThemePalette.ShihabPrimaryAccent : Color.Transparent,
                    ForeColor = mehediSelected ? Color.White : ShihabThemePalette.Sshihabb007TextPrimary,
                    Font = new Font("Segoe UI", 8.8f, mehediSelected ? FontStyle.Bold : FontStyle.Regular),
                    Cursor = Cursors.Hand
                };
                sshihabb007NavBtn.FlatAppearance.BorderSize = 0;
                sshihabb007NavBtn.Click += (s, e) =>
                {
                    mehediActiveBackstageMenu = shihabCap;
                    if (shihabCap == "Save") { SetShihabBackstageVisibility(false); SaveCurrentShihabDocument(); return; }
                    if (shihabCap == "Save As") { SetShihabBackstageVisibility(false); SaveAsCurrentMehediDocument(); return; }
                    if (shihabCap == "Create") { SetShihabBackstageVisibility(false); CreateNewBlankShihabPdfTab(); return; }
                    if (shihabCap == "Export") { SetShihabBackstageVisibility(false); ExportActiveShihabDoc("docx"); return; }
                    if (shihabCap == "PDF Optimizer") { SetShihabBackstageVisibility(false); ShihabDialogsAndToolsWindows.ShowShihabPreflightAndOptimizerStudio(this, ShihabActiveDoc, RefreshAllShihabWorkstationPanels); return; }
                    if (shihabCap == "Skins") { ToggleShihabBrightAndDarkTheme(); return; }
                    if (shihabCap == "Close") { CloseActiveShihabDocumentTab(mehediActiveDocIndex); SetShihabBackstageVisibility(false); return; }
                    RefreshShihabBackstageRecentDocumentsList();
                };
                shihabBackstageLeftNavPanel.Controls.Add(sshihabb007NavBtn);
                mehediNavY += 33;
            }
            shihabBackstageLeftNavPanel.ResumeLayout();

            // 2. Rebuild Middle Sub-Navigation Column ("Open" | Recent Documents | Computer | Add a place)
            mehediBackstageMiddleSubPanel.SuspendLayout();
            mehediBackstageMiddleSubPanel.Controls.Clear();

            Label shihabOpenTitle = new Label
            {
                Text = "Open",
                Location = new Point(16, 12),
                AutoSize = true,
                Font = new Font("Segoe UI Light", 15.0f),
                ForeColor = ShihabThemePalette.ShihabTextSecondary
            };

            Button mehediRecentPlaceBtn = new Button
            {
                Text = "  🕒  Recent Documents",
                Location = new Point(12, 56),
                Size = new Size(198, 34),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = ShihabThemePalette.Sshihabb007CardHoverBg,
                ForeColor = ShihabThemePalette.ShihabPrimaryAccent,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            mehediRecentPlaceBtn.FlatAppearance.BorderSize = 0;

            Button sshihabb007ComputerBrowseBtn = new Button
            {
                Text = "  💻  Computer (Browse...)",
                Location = new Point(12, 96),
                Size = new Size(198, 34),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                Font = new Font("Segoe UI", 9.0f),
                Cursor = Cursors.Hand
            };
            sshihabb007ComputerBrowseBtn.FlatAppearance.BorderSize = 0;
            sshihabb007ComputerBrowseBtn.Click += (s, e) => PromptAndOpenShihabPdfFile();

            Button shihabAddPlaceBtn = new Button
            {
                Text = "  ➕  Add a place",
                Location = new Point(12, 136),
                Size = new Size(198, 34),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent,
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                Font = new Font("Segoe UI", 9.0f),
                Cursor = Cursors.Hand
            };
            shihabAddPlaceBtn.FlatAppearance.BorderSize = 0;
            shihabAddPlaceBtn.Click += (s, e) => PromptAndOpenShihabPdfFile();

            mehediBackstageMiddleSubPanel.Controls.AddRange(new Control[]
            {
                shihabOpenTitle,
                mehediRecentPlaceBtn,
                sshihabb007ComputerBrowseBtn,
                shihabAddPlaceBtn
            });
            mehediBackstageMiddleSubPanel.ResumeLayout();

            // 3. Rebuild Right Recent Documents List (Exact Foxit 2-line PDF rows!)
            sshihabb007BackstageRightListPanel.SuspendLayout();
            sshihabb007BackstageRightListPanel.Controls.Clear();

            Label mehediRecentHeader = new Label
            {
                Text = "Recent Documents",
                Location = new Point(18, 12),
                AutoSize = true,
                Font = new Font("Segoe UI Light", 15.0f),
                ForeColor = ShihabThemePalette.ShihabTextSecondary
            };

            Label sshihabb007LocalSub = new Label
            {
                Text = "Local",
                Location = new Point(20, 50),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.0f, FontStyle.Bold),
                ForeColor = ShihabThemePalette.ShihabTextSecondary
            };

            sshihabb007BackstageRightListPanel.Controls.Add(mehediRecentHeader);
            sshihabb007BackstageRightListPanel.Controls.Add(sshihabb007LocalSub);

            DiscoverShihabLocalRecentPdfFiles();
            int shihabRowY = 78;

            foreach (string mehediPdfPath in mehediDiscoveredLocalPdfFiles)
            {
                string sshihabb007CapPath = mehediPdfPath;
                string shihabFileName = Path.GetFileName(sshihabb007CapPath);

                Panel mehediRowCard = new Panel
                {
                    Location = new Point(18, shihabRowY),
                    Size = new Size(680, 48),
                    BackColor = Color.Transparent,
                    Cursor = Cursors.Hand
                };

                mehediRowCard.Paint += (shihabS, mehediE) =>
                {
                    Graphics sshihabb007G = mehediE.Graphics;
                    sshihabb007G.SmoothingMode = SmoothingMode.AntiAlias;
                    sshihabb007G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Draw Foxit-style PDF Document Icon on left (24x30)
                    using (Pen shihabIconPen = new Pen(ShihabThemePalette.Sshihabb007TextPrimary, 1.4f))
                    using (Font mehediPdfBadgeFont = new Font("Segoe UI", 6.0f, FontStyle.Bold))
                    using (Font sshihabb007NameFont = new Font("Segoe UI", 9.5f, FontStyle.Regular))
                    using (Font shihabPathFont = new Font("Segoe UI", 8.0f, FontStyle.Regular))
                    using (SolidBrush mehediTitleBrush = new SolidBrush(ShihabThemePalette.Sshihabb007TextPrimary))
                    using (SolidBrush sshihabb007SubBrush = new SolidBrush(ShihabThemePalette.ShihabTextSecondary))
                    using (Pen shihabDividerPen = new Pen(ShihabThemePalette.MehediBorderColor, 1f))
                    {
                        sshihabb007G.DrawRectangle(shihabIconPen, 6, 8, 22, 28);
                        sshihabb007G.DrawString("PDF", mehediPdfBadgeFont, mehediTitleBrush, 8, 22);
                        sshihabb007G.DrawString(shihabFileName, sshihabb007NameFont, mehediTitleBrush, 40, 5);
                        sshihabb007G.DrawString(sshihabb007CapPath, shihabPathFont, sshihabb007SubBrush, 40, 24);
                        sshihabb007G.DrawLine(shihabDividerPen, 40, 46, mehediRowCard.Width - 10, 46);
                    }
                };

                mehediRowCard.MouseEnter += (s, e) => mehediRowCard.BackColor = ShihabThemePalette.Sshihabb007CardHoverBg;
                mehediRowCard.MouseLeave += (s, e) => mehediRowCard.BackColor = Color.Transparent;
                mehediRowCard.Click += (s, e) =>
                {
                    SetShihabBackstageVisibility(false);
                    OpenShihabPdfFileIntoNewTab(sshihabb007CapPath);
                    RebuildShihabRibbonTabStrip();
                };

                sshihabb007BackstageRightListPanel.Controls.Add(mehediRowCard);
                shihabRowY += 50;
            }

            sshihabb007BackstageRightListPanel.ResumeLayout();
        }

        // =====================================================================
        // FAR-LEFT NARROW ICON RAIL (Exact Match to Screenshots 1, 3, 4!)
        // =====================================================================
        private void BuildShihabLeftIconRailControls()
        {
            mehediLeftIconRailPanel.Controls.Clear();
            string[,] shihabRailModes = new string[,]
            {
                {"🔖", "Bookmarks"},
                {"📄", "Thumbnails"},
                {"💬", "Comments"},
                {"📎", "Objects"},
                {"🔍", "Search"}
            };

            int mehediY = 10;
            for (int sshihabb007Idx = 0; sshihabb007Idx < shihabRailModes.GetLength(0); sshihabb007Idx++)
            {
                string shihabGlyph = shihabRailModes[sshihabb007Idx, 0];
                string mehediMode = shihabRailModes[sshihabb007Idx, 1];

                Button sshihabb007RailBtn = new Button
                {
                    Text = shihabGlyph,
                    Location = new Point(3, mehediY),
                    Size = new Size(32, 34),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = ShihabThemePalette.ShihabTextSecondary,
                    Font = new Font("Segoe UI Symbol", 11.0f),
                    Cursor = Cursors.Hand
                };
                sshihabb007RailBtn.FlatAppearance.BorderSize = 0;
                ToolTip shihabTip = new ToolTip();
                shihabTip.SetToolTip(sshihabb007RailBtn, mehediMode);

                sshihabb007RailBtn.Click += (s, e) =>
                {
                    if (shihabIsLeftDrawerExpanded && mehediActiveLeftRailMode == mehediMode)
                    {
                        shihabIsLeftDrawerExpanded = false;
                        sshihabb007LeftSlideOutDrawerPanel.Visible = false;
                    }
                    else
                    {
                        mehediActiveLeftRailMode = mehediMode;
                        shihabIsLeftDrawerExpanded = true;
                        sshihabb007LeftSlideOutDrawerPanel.Visible = true;
                        RefreshMehediLeftNavigationDock();
                    }
                    CenterShihabPagePictureBoxInViewport();
                };

                mehediLeftIconRailPanel.Controls.Add(sshihabb007RailBtn);
                mehediY += 40;
            }

            mehediLeftIconRailPanel.Paint += (shihabS, mehediE) =>
            {
                using (Pen sshihabb007RightBorder = new Pen(ShihabThemePalette.MehediBorderColor, 1f))
                {
                    mehediE.Graphics.DrawLine(sshihabb007RightBorder, mehediLeftIconRailPanel.Width - 1, 0, mehediLeftIconRailPanel.Width - 1, mehediLeftIconRailPanel.Height);
                }
            };
        }

        // =====================================================================
        // BOTTOM STATUS BAR (Exact Match to Screenshot 3: 1/20 Page Nav + View Modes + Zoom Slider)
        // =====================================================================
        private void BuildShihabBottomStatusBarControls()
        {
            mehediBottomStatusBar.Controls.Clear();

            int shihabX = 8;
            Button mehediFirstPgBtn = CreateShihabStatusBtn("|◀", ref shihabX, 26, () => JumpToShihabPage(0));
            Button sshihabb007PrevPgBtn = CreateShihabStatusBtn("◀", ref shihabX, 26, () => NavigateShihabPage(-1));

            sshihabb007PageNumberInputBox = new TextBox
            {
                Location = new Point(shihabX + 2, 4),
                Size = new Size(38, 20),
                TextAlign = HorizontalAlignment.Center,
                BackColor = ShihabThemePalette.MehediWindowBg,
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 8.5f),
                Text = "1"
            };
            sshihabb007PageNumberInputBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    int shihabParsedPg;
                    if (int.TryParse(sshihabb007PageNumberInputBox.Text.Trim(), out shihabParsedPg))
                    {
                        JumpToShihabPage(shihabParsedPg - 1);
                    }
                }
            };
            shihabX += 44;

            shihabTotalPagesStatusLabel = new Label
            {
                Text = "/ 1",
                Location = new Point(shihabX, 6),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = ShihabThemePalette.ShihabTextSecondary
            };
            shihabX += 44;

            Button mehediNextPgBtn = CreateShihabStatusBtn("▶", ref shihabX, 26, () => NavigateShihabPage(1));
            Button sshihabb007LastPgBtn = CreateShihabStatusBtn("▶|", ref shihabX, 26, () =>
            {
                if (ShihabActiveDoc != null) JumpToShihabPage(ShihabActiveDoc.Sshihabb007Pages.Count - 1);
            });

            mehediToolHintStatusLabel = new Label
            {
                Text = "Hand Mode  |  PlgDynLoader: 10/32 .fpi",
                Location = new Point(shihabX + 14, 6),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.2f),
                ForeColor = ShihabThemePalette.ShihabTextSecondary
            };

            // Right-Docked Zoom & Fit Controls on Status Bar
            Panel shihabRightStatusGroup = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340
            };

            int mehediRx = 4;
            CreateShihabStatusChildBtn(shihabRightStatusGroup, "📄", "Single Page", ref mehediRx, 26, () => FitShihabPageToViewportHeight());
            CreateShihabStatusChildBtn(shihabRightStatusGroup, "⤢", "Fit Page", ref mehediRx, 26, () => FitShihabPageToViewportHeight());
            CreateShihabStatusChildBtn(shihabRightStatusGroup, "↔", "Fit Width", ref mehediRx, 26, () => FitShihabPageToViewportWidth());
            CreateShihabStatusChildBtn(shihabRightStatusGroup, "—", "Zoom Out", ref mehediRx, 26, () => ChangeShihabZoom(-0.15f));

            sshihabb007ZoomSliderBar = new TrackBar
            {
                Location = new Point(mehediRx, 2),
                Size = new Size(110, 24),
                Minimum = 40,
                Maximum = 280,
                Value = 125,
                TickStyle = TickStyle.None,
                AutoSize = false
            };
            sshihabb007ZoomSliderBar.Scroll += (s, e) =>
            {
                SetExactShihabZoom(sshihabb007ZoomSliderBar.Value / 100f);
            };
            shihabRightStatusGroup.Controls.Add(sshihabb007ZoomSliderBar);
            mehediRx += 114;

            CreateShihabStatusChildBtn(shihabRightStatusGroup, "+", "Zoom In", ref mehediRx, 26, () => ChangeShihabZoom(0.15f));

            shihabZoomPercentButton = new Button
            {
                Text = "125% ▾",
                Location = new Point(mehediRx + 2, 2),
                Size = new Size(64, 24),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.2f),
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                Cursor = Cursors.Hand
            };
            shihabZoomPercentButton.FlatAppearance.BorderSize = 0;
            shihabZoomPercentButton.Click += (s, e) => SetExactShihabZoom(1.0f);
            shihabRightStatusGroup.Controls.Add(shihabZoomPercentButton);

            mehediBottomStatusBar.Controls.Add(shihabRightStatusGroup);
            mehediBottomStatusBar.Controls.AddRange(new Control[]
            {
                mehediFirstPgBtn,
                sshihabb007PrevPgBtn,
                sshihabb007PageNumberInputBox,
                shihabTotalPagesStatusLabel,
                mehediNextPgBtn,
                sshihabb007LastPgBtn,
                mehediToolHintStatusLabel
            });
        }

        private Button CreateShihabStatusBtn(string shihabText, ref int mehediX, int sshihabb007W, Action shihabClick)
        {
            Button mehediBtn = new Button
            {
                Text = shihabText,
                Location = new Point(mehediX, 2),
                Size = new Size(sshihabb007W, 24),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Symbol", 7.8f),
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                Cursor = Cursors.Hand
            };
            mehediBtn.FlatAppearance.BorderSize = 0;
            mehediBtn.Click += (s, e) => shihabClick();
            mehediX += sshihabb007W + 2;
            return mehediBtn;
        }

        private void CreateShihabStatusChildBtn(Panel shihabHost, string mehediText, string sshihabb007Tooltip, ref int shihabX, int mehediW, Action sshihabb007Click)
        {
            Button shihabBtn = new Button
            {
                Text = mehediText,
                Location = new Point(shihabX, 2),
                Size = new Size(mehediW, 24),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Symbol", 8.2f),
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                Cursor = Cursors.Hand
            };
            shihabBtn.FlatAppearance.BorderSize = 0;
            ToolTip mehediTip = new ToolTip();
            mehediTip.SetToolTip(shihabBtn, sshihabb007Tooltip);
            shihabBtn.Click += (s, e) => sshihabb007Click();
            shihabHost.Controls.Add(shihabBtn);
            shihabX += mehediW + 2;
        }

        // =====================================================================
        // RIGHT AI & FXJSE CONSOLE DRAWER
        // =====================================================================
        private void BuildShihabRightAiAndFxjseDrawer()
        {
            sshihabb007RightAiDrawerPanel.Controls.Clear();

            Label shihabAiHeader = new Label
            {
                Text = "🤖 SHIHAB AI ASSISTANT (@sshihabb007)",
                Dock = DockStyle.Top,
                Height = 24,
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = ShihabThemePalette.ShihabPrimaryAccent
            };

            Panel mehediAiRow = new Panel { Dock = DockStyle.Top, Height = 30 };
            mehediAiQuestionInputBox = new TextBox
            {
                Location = new Point(0, 3),
                Size = new Size(225, 23),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Summarize this document"
            };
            Button sshihabb007AskBtn = new Button
            {
                Text = "Ask AI",
                Location = new Point(230, 2),
                Size = new Size(80, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = ShihabThemePalette.ShihabPrimaryAccent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold)
            };
            sshihabb007AskBtn.FlatAppearance.BorderSize = 0;
            sshihabb007AskBtn.Click += (s, e) =>
            {
                sshihabb007PluginLoader.TriggerShihabPluginOnDemand("ShihabAiAssistant");
                shihabAiOutputConsoleBox.Text = ShihabSecuritySignAndAiEngine.AnswerMehediDocumentQuestion(ShihabActiveDoc, mehediAiQuestionInputBox.Text);
                UpdateShihabStatusBar();
            };
            mehediAiRow.Controls.Add(mehediAiQuestionInputBox);
            mehediAiRow.Controls.Add(sshihabb007AskBtn);

            shihabAiOutputConsoleBox = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 240,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8.5f)
            };

            Label mehediFxjseHeader = new Label
            {
                Text = "⚡ FXJSE V8 JAVASCRIPT CONSOLE",
                Dock = DockStyle.Top,
                Height = 26,
                Padding = new Padding(0, 6, 0, 0),
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = ShihabThemePalette.ShihabPrimaryAccent
            };

            Panel sshihabb007JsRow = new Panel { Dock = DockStyle.Top, Height = 30 };
            shihabFxjseCommandInputBox = new TextBox
            {
                Location = new Point(0, 3),
                Size = new Size(225, 23),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8.8f),
                Text = "this.info"
            };
            Button shihabRunJsBtn = new Button
            {
                Text = "Run JS",
                Location = new Point(230, 2),
                Size = new Size(80, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = ShihabThemePalette.ShihabPrimaryAccent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Bold)
            };
            shihabRunJsBtn.FlatAppearance.BorderSize = 0;
            shihabRunJsBtn.Click += (s, e) => ExecuteMehediFxjseCommandFromBox();
            sshihabb007JsRow.Controls.Add(shihabFxjseCommandInputBox);
            sshihabb007JsRow.Controls.Add(shihabRunJsBtn);

            sshihabb007FxjseOutputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8.5f),
                Text = "// FXJSE V8-Bridge Ready (@sshihabb007)\r\n// Try: this.numPages, this.info, this.addWatermark('CONFIDENTIAL')\r\n"
            };

            sshihabb007RightAiDrawerPanel.Controls.Add(sshihabb007FxjseOutputBox);
            sshihabb007RightAiDrawerPanel.Controls.Add(sshihabb007JsRow);
            sshihabb007RightAiDrawerPanel.Controls.Add(mehediFxjseHeader);
            sshihabb007RightAiDrawerPanel.Controls.Add(shihabAiOutputConsoleBox);
            sshihabb007RightAiDrawerPanel.Controls.Add(mehediAiRow);
            sshihabb007RightAiDrawerPanel.Controls.Add(shihabAiHeader);
        }

        // =====================================================================
        // DOCUMENT TAB STRIP (Start | Document.pdf × | +)
        // =====================================================================
        private void RebuildMehediDocumentTabs()
        {
            mehediDocTabStripPanel.SuspendLayout();
            mehediDocTabStripPanel.Controls.Clear();

            int shihabX = 4;

            // 1. "Start" Tab (Matches Screenshots 1, 3, 4!)
            Button mehediStartTabBtn = new Button
            {
                Text = "Start",
                Location = new Point(shihabX, 2),
                Size = new Size(58, 27),
                FlatStyle = FlatStyle.Flat,
                BackColor = ShihabThemePalette.ShihabDocTabStripBg,
                ForeColor = ShihabThemePalette.ShihabTextSecondary,
                Font = new Font("Segoe UI", 8.2f),
                Cursor = Cursors.Hand
            };
            mehediStartTabBtn.FlatAppearance.BorderSize = 0;
            mehediStartTabBtn.Click += (s, e) =>
            {
                SetShihabBackstageVisibility(true);
                RebuildShihabRibbonTabStrip();
            };
            mehediDocTabStripPanel.Controls.Add(mehediStartTabBtn);
            shihabX += 62;

            // 2. Open PDF Document Tabs with Close (×) Button
            for (int sshihabb007Idx = 0; sshihabb007Idx < shihabOpenDocuments.Count; sshihabb007Idx++)
            {
                int shihabCapIdx = sshihabb007Idx;
                ShihabPdfDocumentModel mehediDoc = shihabOpenDocuments[sshihabb007Idx];
                bool sshihabb007IsActive = (sshihabb007Idx == mehediActiveDocIndex);

                string shihabShortTitle = mehediDoc.Sshihabb007DisplayTitle ?? "Untitled.pdf";
                if (shihabShortTitle.Length > 26) shihabShortTitle = shihabShortTitle.Substring(0, 23) + "...";
                if (mehediDoc.ShihabIsModifiedDirty) shihabShortTitle += " *";

                Panel mehediTabHost = new Panel
                {
                    Location = new Point(shihabX, 2),
                    Size = new Size(190, 27),
                    BackColor = sshihabb007IsActive ? ShihabThemePalette.MehediWindowBg : ShihabThemePalette.ShihabDocTabStripBg,
                    Cursor = Cursors.Hand
                };

                Label sshihabb007TitleLbl = new Label
                {
                    Text = shihabShortTitle,
                    Location = new Point(8, 6),
                    Size = new Size(154, 16),
                    Font = new Font("Segoe UI", 8.2f, sshihabb007IsActive ? FontStyle.Regular : FontStyle.Regular),
                    ForeColor = sshihabb007IsActive ? ShihabThemePalette.ShihabPrimaryAccent : ShihabThemePalette.ShihabTextSecondary,
                    Cursor = Cursors.Hand
                };
                sshihabb007TitleLbl.Click += (s, e) =>
                {
                    mehediActiveDocIndex = shihabCapIdx;
                    sshihabb007ActivePageIndex = 0;
                    RefreshAllShihabWorkstationPanels();
                };

                Button shihabCloseTabBtn = new Button
                {
                    Text = "×",
                    Location = new Point(164, 3),
                    Size = new Size(22, 20),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = ShihabThemePalette.ShihabTextSecondary,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                shihabCloseTabBtn.FlatAppearance.BorderSize = 0;
                shihabCloseTabBtn.Click += (s, e) => CloseActiveShihabDocumentTab(shihabCapIdx);

                mehediTabHost.Controls.Add(sshihabb007TitleLbl);
                mehediTabHost.Controls.Add(shihabCloseTabBtn);
                mehediDocTabStripPanel.Controls.Add(mehediTabHost);
                shihabX += 194;
            }

            // 3. "+" Button to Open Another PDF File Into a New Tab
            Button sshihabb007PlusTabBtn = new Button
            {
                Text = "+",
                Location = new Point(shihabX + 2, 2),
                Size = new Size(28, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = ShihabThemePalette.Sshihabb007TextPrimary,
                Font = new Font("Segoe UI", 10.0f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            sshihabb007PlusTabBtn.FlatAppearance.BorderSize = 0;
            sshihabb007PlusTabBtn.Click += (s, e) => PromptAndOpenShihabPdfFile();
            mehediDocTabStripPanel.Controls.Add(sshihabb007PlusTabBtn);

            mehediDocTabStripPanel.ResumeLayout();
        }

        private void CloseActiveShihabDocumentTab(int mehediTabIndex)
        {
            if (mehediTabIndex < 0 || mehediTabIndex >= shihabOpenDocuments.Count) return;
            if (shihabOpenDocuments.Count <= 1)
            {
                SetShihabBackstageVisibility(true);
                RebuildShihabRibbonTabStrip();
                return;
            }
            shihabOpenDocuments.RemoveAt(mehediTabIndex);
            if (mehediActiveDocIndex >= shihabOpenDocuments.Count)
                mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
            sshihabb007ActivePageIndex = 0;
            RefreshAllShihabWorkstationPanels();
        }

        // =====================================================================
        // REFRESH & RENDER PAGE PIPELINE
        // =====================================================================
        public void RefreshAllShihabWorkstationPanels()
        {
            if (ShihabActiveDoc == null) return;
            if (sshihabb007ActivePageIndex < 0 || sshihabb007ActivePageIndex >= ShihabActiveDoc.Sshihabb007Pages.Count)
            {
                sshihabb007ActivePageIndex = 0;
            }

            string shihabTitleNoExt = Path.GetFileNameWithoutExtension(ShihabActiveDoc.Sshihabb007DisplayTitle);
            sshihabb007TopDocTitleLabel.Text = "|   " + shihabTitleNoExt;
            this.Text = shihabTitleNoExt + " - PDF PRO 14.0.4 (Mehedi Hasan Shihab @sshihabb007)";

            RebuildMehediDocumentTabs();
            if (shihabIsLeftDrawerExpanded)
            {
                RefreshMehediLeftNavigationDock();
            }
            RenderActiveShihabPageToCanvas();
            if (sshihabb007IsRightAiDrawerOpen)
            {
                shihabAiOutputConsoleBox.Text = ShihabSecuritySignAndAiEngine.GenerateShihabAiExecutiveSummary(ShihabActiveDoc);
            }
            UpdateShihabStatusBar();
        }

        private void RefreshMehediLeftNavigationDock()
        {
            ShihabPdfDocumentModel shihabDoc = ShihabActiveDoc;
            if (shihabDoc == null) return;

            shihabLeftDrawerTitleLabel.Text = mehediActiveLeftRailMode;
            mehediLeftNavigationList.BeginUpdate();
            mehediLeftNavigationList.Items.Clear();

            switch (mehediActiveLeftRailMode)
            {
                case "Thumbnails":
                    for (int mehediP = 0; mehediP < shihabDoc.Sshihabb007Pages.Count; mehediP++)
                    {
                        MehediPdfPageModel sshihabb007Pg = shihabDoc.Sshihabb007Pages[mehediP];
                        mehediLeftNavigationList.Items.Add(string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}📄 Page {1} of {2} ({3:F0}×{4:F0} pt)",
                            mehediP == sshihabb007ActivePageIndex ? "▶ " : "   ",
                            mehediP + 1,
                            shihabDoc.Sshihabb007Pages.Count,
                            sshihabb007Pg.MehediWidthPt,
                            sshihabb007Pg.Sshihabb007HeightPt));
                    }
                    break;

                case "Bookmarks":
                    foreach (MehediPdfBookmarkItem shihabBm in shihabDoc.ShihabBookmarks)
                    {
                        mehediLeftNavigationList.Items.Add("🔖 [P." + (shihabBm.Sshihabb007TargetPageIndex + 1) + "] " + shihabBm.MehediTitle);
                    }
                    break;

                case "Comments":
                    for (int mehediP = 0; mehediP < shihabDoc.Sshihabb007Pages.Count; mehediP++)
                    {
                        foreach (ShihabPdfAnnotationItem sshihabb007An in shihabDoc.Sshihabb007Pages[mehediP].MehediAnnotations)
                        {
                            mehediLeftNavigationList.Items.Add("[P." + (mehediP + 1) + "] " + sshihabb007An.Sshihabb007Kind + ": " + sshihabb007An.Sshihabb007TextContent);
                        }
                    }
                    if (mehediLeftNavigationList.Items.Count == 0)
                        mehediLeftNavigationList.Items.Add("(No markups yet)");
                    break;

                case "Objects":
                    foreach (MehediPdfObjectNode shihabObj in shihabDoc.MehediParsedObjects)
                    {
                        mehediLeftNavigationList.Items.Add(string.Format(
                            CultureInfo.InvariantCulture,
                            "{0} {1} obj [{2}]",
                            shihabObj.ShihabObjectNumber,
                            shihabObj.MehediGenerationNumber,
                            shihabObj.ShihabObjectTypeSummary));
                    }
                    break;

                case "Search":
                    string mehediQuery = mehediTopSearchInputBox.Text == "Search" ? "Shihab" : mehediTopSearchInputBox.Text;
                    List<ShihabSearchMatchItem> sshihabb007Hits = MehediOemEnginesAndConverters.SearchShihabLuceneIndex(shihabDoc, mehediQuery, false);
                    foreach (ShihabSearchMatchItem shihabHit in sshihabb007Hits)
                    {
                        mehediLeftNavigationList.Items.Add("[P." + (shihabHit.ShihabPageIndex + 1) + "] " + shihabHit.ShihabContextSnippet);
                    }
                    break;
            }

            mehediLeftNavigationList.EndUpdate();
        }

        private void OnShihabLeftNavigationSelectedIndexChanged(object shihabSender, EventArgs mehediArgs)
        {
            int sshihabb007Sel = mehediLeftNavigationList.SelectedIndex;
            if (sshihabb007Sel < 0 || ShihabActiveDoc == null) return;

            if (mehediActiveLeftRailMode == "Thumbnails" && sshihabb007Sel < ShihabActiveDoc.Sshihabb007Pages.Count)
            {
                JumpToShihabPage(sshihabb007Sel);
            }
            else if (mehediActiveLeftRailMode == "Bookmarks" && sshihabb007Sel < ShihabActiveDoc.ShihabBookmarks.Count)
            {
                JumpToShihabPage(ShihabActiveDoc.ShihabBookmarks[sshihabb007Sel].Sshihabb007TargetPageIndex);
            }
        }

        private void RenderActiveShihabPageToCanvas()
        {
            if (ShihabActiveDoc == null) return;
            Bitmap shihabNewBmp = ShihabWinRtRenderAndOcrEngine.RenderShihabCompositePage(
                ShihabActiveDoc,
                sshihabb007ActivePageIndex,
                shihabZoomScale,
                mehediSelectedAnnotationIdx,
                sshihabb007SelectedTextBlockIdx);

            Image mehediOld = shihabPagePictureBox.Image;
            shihabPagePictureBox.Image = shihabNewBmp;
            if (mehediOld != null) mehediOld.Dispose();

            CenterShihabPagePictureBoxInViewport();
        }

        private void CenterShihabPagePictureBoxInViewport()
        {
            if (shihabPagePictureBox == null || sshihabb007CenterCanvasScrollHost == null) return;
            int shihabHostW = sshihabb007CenterCanvasScrollHost.ClientSize.Width;
            int mehediBmpW = shihabPagePictureBox.Width;
            int sshihabb007Left = Math.Max(18, (shihabHostW - mehediBmpW) / 2);
            shihabPagePictureBox.Location = new Point(sshihabb007Left, 14);
        }

        private void OnShihabViewportMouseWheel(object shihabSender, MouseEventArgs mehediE)
        {
            if (ShihabActiveDoc == null) return;
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                ChangeShihabZoom(mehediE.Delta > 0 ? 0.12f : -0.12f);
                return;
            }

            // If user scrolls past the bottom or top of the current page, smoothly transition to the next/previous page!
            int sshihabb007VertScroll = sshihabb007CenterCanvasScrollHost.VerticalScroll.Value;
            int shihabMaxScroll = sshihabb007CenterCanvasScrollHost.VerticalScroll.Maximum - sshihabb007CenterCanvasScrollHost.ClientSize.Height;

            if (mehediE.Delta < 0 && sshihabb007VertScroll >= Math.Max(0, shihabMaxScroll - 15) && sshihabb007ActivePageIndex < ShihabActiveDoc.Sshihabb007Pages.Count - 1)
            {
                sshihabb007ActivePageIndex++;
                sshihabb007CenterCanvasScrollHost.AutoScrollPosition = new Point(0, 0);
                RenderActiveShihabPageToCanvas();
                UpdateShihabStatusBar();
            }
            else if (mehediE.Delta > 0 && sshihabb007VertScroll <= 5 && sshihabb007ActivePageIndex > 0)
            {
                sshihabb007ActivePageIndex--;
                RenderActiveShihabPageToCanvas();
                UpdateShihabStatusBar();
            }
        }

        // =====================================================================
        // MOUSE INTERACTIONS ON PDF CANVAS (HAND PAN, EDIT TEXT, ANNOTATIONS)
        // =====================================================================
        private void OnShihabCanvasMouseDown(object shihabSender, MouseEventArgs mehediE)
        {
            if (mehediE.Button != MouseButtons.Left || ShihabActiveDoc == null) return;
            MehediPdfPageModel sshihabb007Page = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            PointF shihabPt = new PointF(mehediE.X / shihabZoomScale, mehediE.Y / shihabZoomScale);

            if (mehediCurrentTool == ShihabPdfToolMode.ShihabHandPan)
            {
                shihabIsMouseDragging = true;
                mehediPanMouseStartScreen = shihabPagePictureBox.PointToScreen(mehediE.Location);
                sshihabb007PanScrollStart = new Point(
                    -sshihabb007CenterCanvasScrollHost.AutoScrollPosition.X,
                    -sshihabb007CenterCanvasScrollHost.AutoScrollPosition.Y);
                return;
            }

            if (mehediCurrentTool == ShihabPdfToolMode.Sshihabb007EditContent || mehediCurrentTool == ShihabPdfToolMode.MehediSelectText)
            {
                mehediSelectedAnnotationIdx = -1;
                for (int mehediIdx = sshihabb007Page.MehediAnnotations.Count - 1; mehediIdx >= 0; mehediIdx--)
                {
                    if (sshihabb007Page.MehediAnnotations[mehediIdx].ShihabBoundsPt.Contains(shihabPt))
                    {
                        mehediSelectedAnnotationIdx = mehediIdx;
                        break;
                    }
                }

                sshihabb007SelectedTextBlockIdx = -1;
                for (int shihabIdx = 0; shihabIdx < sshihabb007Page.ShihabTextBlocks.Count; shihabIdx++)
                {
                    if (sshihabb007Page.ShihabTextBlocks[shihabIdx].Sshihabb007BoundsPt.Contains(shihabPt))
                    {
                        sshihabb007SelectedTextBlockIdx = shihabIdx;
                        break;
                    }
                }

                RenderActiveShihabPageToCanvas();
                return;
            }

            shihabIsMouseDragging = true;
            mehediMouseDownPt = shihabPt;
            sshihabb007MouseCurrentPt = shihabPt;
            shihabCurrentInkStrokePts.Clear();
            shihabCurrentInkStrokePts.Add(shihabPt);
        }

        private void OnMehediCanvasMouseMove(object shihabSender, MouseEventArgs mehediE)
        {
            if (!shihabIsMouseDragging) return;

            if (mehediCurrentTool == ShihabPdfToolMode.ShihabHandPan)
            {
                Point shihabNowScreen = shihabPagePictureBox.PointToScreen(mehediE.Location);
                int mehediDx = mehediPanMouseStartScreen.X - shihabNowScreen.X;
                int sshihabb007Dy = mehediPanMouseStartScreen.Y - shihabNowScreen.Y;
                sshihabb007CenterCanvasScrollHost.AutoScrollPosition = new Point(
                    Math.Max(0, sshihabb007PanScrollStart.X + mehediDx),
                    Math.Max(0, sshihabb007PanScrollStart.Y + sshihabb007Dy));
                return;
            }

            PointF shihabPt = new PointF(mehediE.X / shihabZoomScale, mehediE.Y / shihabZoomScale);
            sshihabb007MouseCurrentPt = shihabPt;
            if (mehediCurrentTool == ShihabPdfToolMode.Sshihabb007FreehandInk)
            {
                shihabCurrentInkStrokePts.Add(shihabPt);
            }
            shihabPagePictureBox.Invalidate();
        }

        private void OnSshihabb007CanvasMouseUp(object shihabSender, MouseEventArgs mehediE)
        {
            if (!shihabIsMouseDragging || ShihabActiveDoc == null) return;
            shihabIsMouseDragging = false;

            if (mehediCurrentTool == ShihabPdfToolMode.ShihabHandPan) return;

            MehediPdfPageModel sshihabb007Page = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            PointF shihabUpPt = new PointF(mehediE.X / shihabZoomScale, mehediE.Y / shihabZoomScale);
            RectangleF mehediDragRect = new RectangleF(
                Math.Min(mehediMouseDownPt.X, shihabUpPt.X),
                Math.Min(mehediMouseDownPt.Y, shihabUpPt.Y),
                Math.Max(64f, Math.Abs(shihabUpPt.X - mehediMouseDownPt.X)),
                Math.Max(22f, Math.Abs(shihabUpPt.Y - mehediMouseDownPt.Y)));

            switch (mehediCurrentTool)
            {
                case ShihabPdfToolMode.ShihabAddTextBox:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.ShihabTextBox,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007TextContent = "Added Text Block (@sshihabb007)",
                        Sshihabb007StrokeColor = Color.FromArgb(255, 132, 34, 166),
                        ShihabFillColor = Color.FromArgb(25, 132, 34, 166)
                    });
                    break;

                case ShihabPdfToolMode.MehediHighlight:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.MehediHighlight,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007TextContent = "Highlight"
                    });
                    break;

                case ShihabPdfToolMode.Sshihabb007Underline:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.Sshihabb007Underline,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007StrokeColor = Color.FromArgb(255, 16, 185, 129)
                    });
                    break;

                case ShihabPdfToolMode.ShihabStrikeout:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.ShihabStrikeout,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007StrokeColor = Color.FromArgb(255, 239, 68, 68)
                    });
                    break;

                case ShihabPdfToolMode.MehediStickyNote:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.MehediStickyNote,
                        ShihabBoundsPt = new RectangleF(mehediMouseDownPt.X, mehediMouseDownPt.Y, 175f, 44f),
                        Sshihabb007TextContent = "📌 Note — Mehedi Hasan Shihab",
                        Sshihabb007StrokeColor = Color.FromArgb(255, 217, 119, 6),
                        ShihabFillColor = Color.FromArgb(225, 254, 243, 199)
                    });
                    break;

                case ShihabPdfToolMode.Sshihabb007FreehandInk:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.Sshihabb007FreehandInk,
                        ShihabBoundsPt = mehediDragRect,
                        MehediInkPointsPt = new List<PointF>(shihabCurrentInkStrokePts),
                        Sshihabb007StrokeColor = Color.FromArgb(255, 132, 34, 166),
                        MehediStrokeWidth = 2.4f
                    });
                    break;

                case ShihabPdfToolMode.ShihabRectangleShape:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.ShihabRectangle,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007StrokeColor = Color.FromArgb(255, 132, 34, 166),
                        ShihabFillColor = Color.FromArgb(28, 132, 34, 166)
                    });
                    break;

                case ShihabPdfToolMode.MehediEllipseShape:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.MehediEllipse,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007StrokeColor = Color.FromArgb(255, 132, 34, 166),
                        ShihabFillColor = Color.FromArgb(28, 132, 34, 166)
                    });
                    break;

                case ShihabPdfToolMode.Sshihabb007ArrowLine:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.Sshihabb007Arrow,
                        ShihabBoundsPt = new RectangleF(mehediMouseDownPt.X, mehediMouseDownPt.Y, shihabUpPt.X - mehediMouseDownPt.X, shihabUpPt.Y - mehediMouseDownPt.Y),
                        Sshihabb007StrokeColor = Color.FromArgb(255, 239, 68, 68),
                        MehediStrokeWidth = 2.2f
                    });
                    break;

                case ShihabPdfToolMode.ShihabCalloutBox:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.ShihabCallout,
                        ShihabBoundsPt = new RectangleF(mehediDragRect.X, mehediDragRect.Y, Math.Max(140f, mehediDragRect.Width), Math.Max(36f, mehediDragRect.Height)),
                        Sshihabb007TextContent = "💬 Callout (@sshihabb007)",
                        Sshihabb007StrokeColor = Color.FromArgb(255, 224, 86, 36),
                        ShihabFillColor = Color.FromArgb(230, 255, 247, 237)
                    });
                    break;

                case ShihabPdfToolMode.MehediRedactionMark:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.MehediRedaction,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007TextContent = "Redaction Target",
                        Sshihabb007IsRedactionBurned = false
                    });
                    break;

                case ShihabPdfToolMode.ShihabStampApproved:
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.ShihabStamp,
                        ShihabBoundsPt = new RectangleF(mehediMouseDownPt.X, mehediMouseDownPt.Y, 178f, 38f),
                        Sshihabb007TextContent = "APPROVED BY SHIHAB",
                        Sshihabb007StrokeColor = Color.FromArgb(255, 16, 185, 129),
                        ShihabFillColor = Color.FromArgb(38, 16, 185, 129)
                    });
                    break;

                case ShihabPdfToolMode.Sshihabb007SignatureStamp:
                    SignActiveShihabPageQuick();
                    break;

                case ShihabPdfToolMode.MehediMeasureRuler:
                    double shihabDistPt = Math.Sqrt(Math.Pow(shihabUpPt.X - mehediMouseDownPt.X, 2) + Math.Pow(shihabUpPt.Y - mehediMouseDownPt.Y, 2));
                    double mehediDistMm = shihabDistPt * 25.4 / 72.0;
                    sshihabb007Page.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.Sshihabb007RulerMeasurement,
                        ShihabBoundsPt = mehediDragRect,
                        Sshihabb007TextContent = string.Format(CultureInfo.InvariantCulture, "📏 {0:F1} pt ({1:F1} mm)", shihabDistPt, mehediDistMm),
                        Sshihabb007StrokeColor = Color.FromArgb(255, 14, 165, 233),
                        ShihabFillColor = Color.FromArgb(30, 14, 165, 233)
                    });
                    break;

                case ShihabPdfToolMode.ShihabFormTextField:
                case ShihabPdfToolMode.MehediFormCheckBox:
                case ShihabPdfToolMode.Sshihabb007FormComboBox:
                case ShihabPdfToolMode.ShihabFormPushButton:
                    string sshihabb007FldKind = mehediCurrentTool == ShihabPdfToolMode.MehediFormCheckBox ? "CheckBox"
                        : (mehediCurrentTool == ShihabPdfToolMode.Sshihabb007FormComboBox ? "ComboBox"
                        : (mehediCurrentTool == ShihabPdfToolMode.ShihabFormPushButton ? "PushButton" : "Text"));
                    sshihabb007Page.Sshihabb007FormFields.Add(new ShihabAcroFormFieldItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007FieldType = sshihabb007FldKind,
                        ShihabFieldName = "shihab_" + sshihabb007FldKind.ToLowerInvariant() + "_" + (sshihabb007Page.Sshihabb007FormFields.Count + 1),
                        Sshihabb007FieldValue = "Mehedi Hasan Shihab (@sshihabb007)",
                        ShihabIsChecked = true,
                        Sshihabb007BoundsPt = new RectangleF(mehediDragRect.X, mehediDragRect.Y, Math.Max(140f, mehediDragRect.Width), 26f)
                    });
                    break;
            }

            ShihabActiveDoc.ShihabIsModifiedDirty = true;
            RenderActiveShihabPageToCanvas();
            if (shihabIsLeftDrawerExpanded) RefreshMehediLeftNavigationDock();
        }

        private void OnMehediCanvasLiveRubberbandPaint(object shihabSender, PaintEventArgs mehediE)
        {
            using (Pen sshihabb007PageBorder = new Pen(ShihabThemePalette.MehediBorderColor, 1f))
            {
                mehediE.Graphics.DrawRectangle(sshihabb007PageBorder, 0, 0, shihabPagePictureBox.Width - 1, shihabPagePictureBox.Height - 1);
            }

            if (!shihabIsMouseDragging || mehediCurrentTool == ShihabPdfToolMode.ShihabHandPan) return;

            RectangleF shihabDragPx = new RectangleF(
                Math.Min(mehediMouseDownPt.X, sshihabb007MouseCurrentPt.X) * shihabZoomScale,
                Math.Min(mehediMouseDownPt.Y, sshihabb007MouseCurrentPt.Y) * shihabZoomScale,
                Math.Abs(sshihabb007MouseCurrentPt.X - mehediMouseDownPt.X) * shihabZoomScale,
                Math.Abs(sshihabb007MouseCurrentPt.Y - mehediMouseDownPt.Y) * shihabZoomScale);

            using (Pen mehediRubberPen = new Pen(ShihabThemePalette.ShihabPrimaryAccent, 1.8f) { DashStyle = DashStyle.Dash })
            {
                mehediE.Graphics.DrawRectangle(mehediRubberPen, shihabDragPx.X, shihabDragPx.Y, shihabDragPx.Width, shihabDragPx.Height);
            }
        }

        private void OnShihabCanvasDoubleClick(object shihabSender, EventArgs mehediE)
        {
            EditSelectedShihabTextBlockModal();
        }

        // =====================================================================
        // ACTION HELPERS (OPEN, SAVE, EXPORT, OCR, ZOOM, THEME SWITCHING)
        // =====================================================================
        public void OpenShihabPdfFileIntoNewTab(string mehediFilePath)
        {
            if (string.IsNullOrWhiteSpace(mehediFilePath) || !File.Exists(mehediFilePath)) return;
            try
            {
                ShihabPdfDocumentModel sshihabb007Loaded = MehediPdfCoreEngine.LoadShihabPdfFromFile(mehediFilePath);
                shihabOpenDocuments.Add(sshihabb007Loaded);
                mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
                sshihabb007ActivePageIndex = 0;

                if (!mehediDiscoveredLocalPdfFiles.Contains(mehediFilePath))
                {
                    mehediDiscoveredLocalPdfFiles.Insert(0, mehediFilePath);
                }

                mehediSensor.RecordShihabOpenTracingSpan("Document.Open", "file=" + Path.GetFileName(mehediFilePath) + ",pages=" + sshihabb007Loaded.Sshihabb007Pages.Count);
                SetShihabBackstageVisibility(false);
                RefreshAllShihabWorkstationPanels();
            }
            catch (Exception shihabEx)
            {
                MessageBox.Show(this, "Failed to open PDF file:\r\n" + shihabEx.Message, "PDF PRO Open Error (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PromptAndOpenShihabPdfFile()
        {
            using (OpenFileDialog shihabOfd = new OpenFileDialog())
            {
                shihabOfd.Title = "Open PDF Document — PDF PRO 14.0.4 (Mehedi Hasan Shihab @sshihabb007)";
                shihabOfd.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                if (Directory.Exists(@"C:\Users\MEHEDI\Downloads\Chrome"))
                {
                    shihabOfd.InitialDirectory = @"C:\Users\MEHEDI\Downloads\Chrome";
                }
                if (shihabOfd.ShowDialog(this) == DialogResult.OK)
                {
                    OpenShihabPdfFileIntoNewTab(shihabOfd.FileName);
                }
            }
        }

        private void CreateNewBlankShihabPdfTab()
        {
            ShihabPdfDocumentModel shihabBlank = new ShihabPdfDocumentModel { Sshihabb007DisplayTitle = "Blank-Document-Shihab.pdf" };
            MehediPdfCoreEngine.InsertShihabBlankPage(shihabBlank, -1);
            shihabOpenDocuments.Add(shihabBlank);
            mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
            sshihabb007ActivePageIndex = 0;
            SetShihabBackstageVisibility(false);
            RefreshAllShihabWorkstationPanels();
        }

        private void CreateNewShowcaseShihabTab()
        {
            shihabOpenDocuments.Add(MehediPdfCoreEngine.CreateShihabShowcaseDocument("Starship"));
            mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
            sshihabb007ActivePageIndex = 0;
            SetShihabBackstageVisibility(false);
            RefreshAllShihabWorkstationPanels();
        }

        private void CreatePdfFromShihabClipboard()
        {
            string shihabClipText = Clipboard.ContainsText() ? Clipboard.GetText() : "Clipboard PDF Created by Mehedi Hasan Shihab (@sshihabb007)";
            ShihabPdfDocumentModel mehediClipDoc = new ShihabPdfDocumentModel { Sshihabb007DisplayTitle = "Clipboard-Shihab.pdf" };
            MehediPdfPageModel sshihabb007Pg = new MehediPdfPageModel { ShihabPageIndex = 0, ShihabExtractedText = shihabClipText };
            MehediPdfCoreEngine.PopulateShihabEditableBlocksFromExtractedText(sshihabb007Pg);
            mehediClipDoc.Sshihabb007Pages.Add(sshihabb007Pg);
            shihabOpenDocuments.Add(mehediClipDoc);
            mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
            sshihabb007ActivePageIndex = 0;
            RefreshAllShihabWorkstationPanels();
        }

        private void SaveCurrentShihabDocument()
        {
            if (ShihabActiveDoc == null) return;
            if (!string.IsNullOrWhiteSpace(ShihabActiveDoc.MehediFilePath))
            {
                MehediPdfCoreEngine.SaveShihabPdfDocumentToFile(ShihabActiveDoc, ShihabActiveDoc.MehediFilePath);
                ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
                RefreshAllShihabWorkstationPanels();
                MessageBox.Show(this, "Saved PDF to:\r\n" + ShihabActiveDoc.MehediFilePath, "PDF PRO Saved (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                SaveAsCurrentMehediDocument();
            }
        }

        private void SaveAsCurrentMehediDocument()
        {
            if (ShihabActiveDoc == null) return;
            using (SaveFileDialog mehediSfd = new SaveFileDialog())
            {
                mehediSfd.Title = "Save PDF As — PDF PRO (@sshihabb007)";
                mehediSfd.Filter = "PDF Document (*.pdf)|*.pdf";
                mehediSfd.FileName = ShihabActiveDoc.Sshihabb007DisplayTitle;
                if (mehediSfd.ShowDialog(this) == DialogResult.OK)
                {
                    MehediPdfCoreEngine.SaveShihabPdfDocumentToFile(ShihabActiveDoc, mehediSfd.FileName);
                    ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
                    RefreshAllShihabWorkstationPanels();
                }
            }
        }

        private void UndoLastShihabAnnotationOnPage()
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfPageModel shihabPg = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            if (shihabPg.MehediAnnotations.Count > 0)
            {
                shihabPg.MehediAnnotations.RemoveAt(shihabPg.MehediAnnotations.Count - 1);
                ShihabActiveDoc.ShihabIsModifiedDirty = true;
                RenderActiveShihabPageToCanvas();
            }
        }

        private void EditSelectedShihabTextBlockModal()
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfPageModel shihabPage = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            if (shihabPage.ShihabTextBlocks.Count == 0)
            {
                // Run quick OCR on page if it came from a raster/compressed stream with no text blocks yet
                int mehediWords;
                ShihabWinRtRenderAndOcrEngine.RunMehediWinRtOcrOnPage(ShihabActiveDoc, sshihabb007ActivePageIndex, out mehediWords);
            }
            if (sshihabb007SelectedTextBlockIdx < 0 && shihabPage.ShihabTextBlocks.Count > 0)
            {
                sshihabb007SelectedTextBlockIdx = 0;
            }
            if (sshihabb007SelectedTextBlockIdx >= 0 && sshihabb007SelectedTextBlockIdx < shihabPage.ShihabTextBlocks.Count)
            {
                sshihabb007PluginLoader.TriggerShihabPluginOnDemand("MehediPageEditor");
                ShihabPdfTextBlockItem mehediBlock = shihabPage.ShihabTextBlocks[sshihabb007SelectedTextBlockIdx];
                if (ShihabDialogsAndToolsWindows.ShowShihabTextBlockEditorDialog(this, mehediBlock))
                {
                    List<string> sshihabb007Lines = new List<string>();
                    foreach (ShihabPdfTextBlockItem shihabTb in shihabPage.ShihabTextBlocks)
                        sshihabb007Lines.Add(shihabTb.ShihabText);
                    shihabPage.ShihabExtractedText = string.Join(Environment.NewLine, sshihabb007Lines);
                    ShihabActiveDoc.ShihabIsModifiedDirty = true;
                    ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
                    RefreshAllShihabWorkstationPanels();
                }
            }
        }

        private void DeleteSelectedShihabAnnotation()
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfPageModel shihabPage = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            if (mehediSelectedAnnotationIdx >= 0 && mehediSelectedAnnotationIdx < shihabPage.MehediAnnotations.Count)
            {
                shihabPage.MehediAnnotations.RemoveAt(mehediSelectedAnnotationIdx);
                mehediSelectedAnnotationIdx = -1;
                ShihabActiveDoc.ShihabIsModifiedDirty = true;
                RefreshAllShihabWorkstationPanels();
            }
        }

        private void SetShihabToolMode(ShihabPdfToolMode mehediNewTool)
        {
            this.mehediCurrentTool = mehediNewTool;
            shihabPagePictureBox.Cursor = (mehediNewTool == ShihabPdfToolMode.ShihabHandPan) ? Cursors.Hand : Cursors.Cross;
            RebuildShihabRibbonButtonsForActiveTab();
            UpdateShihabStatusBar();
        }

        private void RotateActiveShihabPage(int mehediDeltaDegrees)
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfCoreEngine.RotateShihabPage(ShihabActiveDoc, sshihabb007ActivePageIndex, mehediDeltaDegrees);
            ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
            RefreshAllShihabWorkstationPanels();
        }

        private void DeleteActiveShihabPage()
        {
            if (ShihabActiveDoc == null) return;
            if (MehediPdfCoreEngine.DeleteSshihabb007Page(ShihabActiveDoc, sshihabb007ActivePageIndex))
            {
                if (sshihabb007ActivePageIndex >= ShihabActiveDoc.Sshihabb007Pages.Count)
                    sshihabb007ActivePageIndex = ShihabActiveDoc.Sshihabb007Pages.Count - 1;
                RefreshAllShihabWorkstationPanels();
            }
        }

        private void MoveActiveShihabPage(int mehediDirection)
        {
            if (ShihabActiveDoc == null) return;
            int sshihabb007Target = sshihabb007ActivePageIndex + mehediDirection;
            if (sshihabb007Target >= 0 && sshihabb007Target < ShihabActiveDoc.Sshihabb007Pages.Count)
            {
                MehediPdfCoreEngine.MoveMehediPageOrder(ShihabActiveDoc, sshihabb007ActivePageIndex, sshihabb007Target);
                sshihabb007ActivePageIndex = sshihabb007Target;
                RefreshAllShihabWorkstationPanels();
            }
        }

        private void AddBookmarkForCurrentShihabPage()
        {
            if (ShihabActiveDoc == null) return;
            ShihabActiveDoc.ShihabBookmarks.Add(new MehediPdfBookmarkItem
            {
                MehediTitle = "Bookmark — Page " + (sshihabb007ActivePageIndex + 1) + " (@sshihabb007)",
                Sshihabb007TargetPageIndex = sshihabb007ActivePageIndex
            });
            mehediActiveLeftRailMode = "Bookmarks";
            shihabIsLeftDrawerExpanded = true;
            sshihabb007LeftSlideOutDrawerPanel.Visible = true;
            RefreshMehediLeftNavigationDock();
        }

        private void AutoScanAndMarkShihabPii()
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("ShihabWhiteoutRedaction");
            int shihabHits = ShihabSecuritySignAndAiEngine.ScanAndMarkShihabSensitivePiiPatterns(ShihabActiveDoc, null);
            RefreshAllShihabWorkstationPanels();
            MessageBox.Show(this, "Marked " + shihabHits + " sensitive PII pattern(s). Click 'Apply Redactions' to permanently burn.", "Protect & Redact (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BurnAllPendingShihabRedactions()
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("ShihabWhiteoutRedaction");
            int mehediBurned = ShihabSecuritySignAndAiEngine.BurnAllMehediPendingRedactions(ShihabActiveDoc);
            RefreshAllShihabWorkstationPanels();
            MessageBox.Show(this, "Permanently burned " + mehediBurned + " redaction region(s).", "Protect & Redact (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplyShihabRmsProtectionQuick()
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("ShihabAipLabel");
            ShihabSecuritySignAndAiEngine.ApplySshihabb007PasswordAndAipProtection(ShihabActiveDoc, "shihab2026", "mehedi2026", "CONFIDENTIAL — SHIHAB RMS");
            RefreshAllShihabWorkstationPanels();
            MessageBox.Show(this, "Applied Microsoft RMS / AIP Classification ('CONFIDENTIAL — SHIHAB RMS') and SHA-256 Permission Guard.", "Protect (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SignActiveShihabPageQuick()
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("MehediDocSign");
            ShihabSecuritySignAndAiEngine.SignShihabDocumentWithX509(
                ShihabActiveDoc,
                sshihabb007ActivePageIndex,
                "Mehedi Hasan Shihab (@sshihabb007)",
                "Approved & Digitally Signed in PDF PRO 14.0.4",
                "Global Open Source Workstation",
                null,
                null);
            RefreshAllShihabWorkstationPanels();
        }

        private void ExportActiveShihabDoc(string mehediFormatExt)
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("ShihabFxExport");

            if (mehediFormatExt == "png")
            {
                using (FolderBrowserDialog sshihabb007Fbd = new FolderBrowserDialog())
                {
                    sshihabb007Fbd.Description = "Select folder to export high-DPI PNG page images:";
                    if (sshihabb007Fbd.ShowDialog(this) == DialogResult.OK)
                    {
                        List<string> shihabFiles = MehediOemEnginesAndConverters.ExportSshihabb007PdfPagesToImages(ShihabActiveDoc, sshihabb007Fbd.SelectedPath, ImageFormat.Png, ".png");
                        MessageBox.Show(this, "Exported " + shihabFiles.Count + " PNG page image(s) to:\r\n" + sshihabb007Fbd.SelectedPath, "PDF PRO Export (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                return;
            }

            using (SaveFileDialog shihabSfd = new SaveFileDialog())
            {
                string mehediStem = Path.GetFileNameWithoutExtension(ShihabActiveDoc.Sshihabb007DisplayTitle);
                shihabSfd.FileName = mehediStem + "-Shihab." + mehediFormatExt;
                shihabSfd.Filter = mehediFormatExt.ToUpperInvariant() + " File (*." + mehediFormatExt + ")|*." + mehediFormatExt;
                if (shihabSfd.ShowDialog(this) != DialogResult.OK) return;

                // Ensure text is extracted via OCR if pages had compressed streams
                if (string.IsNullOrWhiteSpace(ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex].ShihabExtractedText))
                {
                    int sshihabb007Words;
                    ShihabWinRtRenderAndOcrEngine.RunMehediWinRtOcrOnPage(ShihabActiveDoc, sshihabb007ActivePageIndex, out sshihabb007Words);
                }

                switch (mehediFormatExt)
                {
                    case "docx":
                        MehediOemEnginesAndConverters.ExportShihabPdfToWordDocx(ShihabActiveDoc, shihabSfd.FileName);
                        break;
                    case "xlsx":
                        MehediOemEnginesAndConverters.ExportMehediPdfToExcelXlsx(ShihabActiveDoc, shihabSfd.FileName);
                        break;
                    case "csv":
                        MehediOemEnginesAndConverters.ExportSshihabb007PdfToCsv(ShihabActiveDoc, shihabSfd.FileName);
                        break;
                    case "html":
                        MehediOemEnginesAndConverters.ExportShihabPdfToHtml5(ShihabActiveDoc, shihabSfd.FileName);
                        break;
                    default:
                        MehediOemEnginesAndConverters.ExportMehediPdfToMarkdownOrText(ShihabActiveDoc, shihabSfd.FileName, false);
                        break;
                }

                mehediSensor.RecordShihabOpenTracingSpan("FXExport.Convert:" + mehediFormatExt, "out=" + Path.GetFileName(shihabSfd.FileName));
                UpdateShihabStatusBar();
                MessageBox.Show(this, "Exported to " + mehediFormatExt.ToUpperInvariant() + ":\r\n" + shihabSfd.FileName, "Solid Office Export (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ImportExternalFilesIntoShihabPdf()
        {
            using (OpenFileDialog shihabOfd = new OpenFileDialog())
            {
                shihabOfd.Title = "Select Files (.pdf, .png, .jpg, .txt, .md, .html, .csv) to Open or Convert to PDF PRO";
                shihabOfd.Multiselect = true;
                shihabOfd.Filter = "All Supported (*.pdf;*.png;*.jpg;*.jpeg;*.bmp;*.txt;*.md;*.html;*.csv)|*.pdf;*.png;*.jpg;*.jpeg;*.bmp;*.txt;*.md;*.html;*.csv|All Files (*.*)|*.*";
                if (shihabOfd.ShowDialog(this) == DialogResult.OK)
                {
                    if (shihabOfd.FileNames.Length == 1 && shihabOfd.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        OpenShihabPdfFileIntoNewTab(shihabOfd.FileName);
                        return;
                    }
                    ShihabPdfDocumentModel mehediConverted = MehediOemEnginesAndConverters.ImportShihabFilesToNewPdfDocument(shihabOfd.FileNames);
                    shihabOpenDocuments.Add(mehediConverted);
                    mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
                    sshihabb007ActivePageIndex = 0;
                    RefreshAllShihabWorkstationPanels();
                }
            }
        }

        private void RunShihabOcrOnActivePage()
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("MehediOcrRecognition");
            int mehediWords;
            string sshihabb007Text = ShihabWinRtRenderAndOcrEngine.RunMehediWinRtOcrOnPage(ShihabActiveDoc, sshihabb007ActivePageIndex, out mehediWords);
            RefreshAllShihabWorkstationPanels();
            MessageBox.Show(this, "Neural OCR Engine recognized " + mehediWords + " words on Page " + (sshihabb007ActivePageIndex + 1) + ":\r\n\r\n" + (sshihabb007Text.Length > 340 ? sshihabb007Text.Substring(0, 340) + "..." : sshihabb007Text), "Neural OCR (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InsertShihabImageOverlayOnActivePage()
        {
            if (ShihabActiveDoc == null) return;
            using (OpenFileDialog shihabOfd = new OpenFileDialog())
            {
                shihabOfd.Title = "Select Image to Place on PDF Page";
                shihabOfd.Filter = "Images (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
                if (shihabOfd.ShowDialog(this) == DialogResult.OK)
                {
                    byte[] mehediImgBytes = File.ReadAllBytes(shihabOfd.FileName);
                    ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex].MehediAnnotations.Add(new ShihabPdfAnnotationItem
                    {
                        MehediPageIndex = sshihabb007ActivePageIndex,
                        Sshihabb007Kind = MehediAnnotationKind.MehediImageOverlay,
                        ShihabBoundsPt = new RectangleF(96f, 180f, 240f, 180f),
                        Sshihabb007EmbeddedImageBytes = mehediImgBytes,
                        Sshihabb007TextContent = Path.GetFileName(shihabOfd.FileName)
                    });
                    ShihabActiveDoc.ShihabIsModifiedDirty = true;
                    RefreshAllShihabWorkstationPanels();
                }
            }
        }

        private void PromptShihabWatermarkText()
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfCoreEngine.ApplySshihabb007WatermarkAllPages(ShihabActiveDoc, "CONFIDENTIAL — SHIHAB PDF PRO");
            ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
            RefreshAllShihabWorkstationPanels();
        }

        private void PromptShihabHeaderFooter()
        {
            if (ShihabActiveDoc == null) return;
            foreach (MehediPdfPageModel shihabPg in ShihabActiveDoc.Sshihabb007Pages)
            {
                shihabPg.MehediHeaderText = "PDF PRO Enterprise Edition  |  Author: Mehedi Hasan Shihab (@sshihabb007)";
            }
            ShihabActiveDoc.ShihabIsModifiedDirty = true;
            RefreshAllShihabWorkstationPanels();
        }

        private void MergeAllOpenShihabDocuments()
        {
            if (shihabOpenDocuments.Count < 2)
            {
                PromptAndOpenShihabPdfFile();
                return;
            }
            ShihabPdfDocumentModel mehediMerged = MehediPdfCoreEngine.MergeShihabPdfDocuments(shihabOpenDocuments, "Combined-Shihab-PDFPRO.pdf");
            shihabOpenDocuments.Add(mehediMerged);
            mehediActiveDocIndex = shihabOpenDocuments.Count - 1;
            sshihabb007ActivePageIndex = 0;
            RefreshAllShihabWorkstationPanels();
        }

        private void SplitActiveShihabDocumentToFolder()
        {
            if (ShihabActiveDoc == null) return;
            using (FolderBrowserDialog shihabFbd = new FolderBrowserDialog())
            {
                shihabFbd.Description = "Select output folder to split each page into an individual PDF file:";
                if (shihabFbd.ShowDialog(this) == DialogResult.OK)
                {
                    List<string> mehediCreated = MehediPdfCoreEngine.SplitMehediPdfDocumentToDirectory(ShihabActiveDoc, shihabFbd.SelectedPath);
                    MessageBox.Show(this, "Split document into " + mehediCreated.Count + " PDF file(s) in:\r\n" + shihabFbd.SelectedPath, "Split PDF (@sshihabb007)", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ExportShihabAcroFormDataJson()
        {
            if (ShihabActiveDoc == null) return;
            StringBuilder mehediJson = new StringBuilder();
            mehediJson.AppendLine("{");
            mehediJson.AppendLine("  \"generator\": \"PDF PRO v14.0.4 (Mehedi Hasan Shihab @sshihabb007)\",");
            mehediJson.AppendLine("  \"fields\": [");
            for (int sshihabb007P = 0; sshihabb007P < ShihabActiveDoc.Sshihabb007Pages.Count; sshihabb007P++)
            {
                foreach (ShihabAcroFormFieldItem shihabFld in ShihabActiveDoc.Sshihabb007Pages[sshihabb007P].Sshihabb007FormFields)
                {
                    mehediJson.AppendFormat("    {{ \"page\": {0}, \"name\": \"{1}\", \"type\": \"{2}\", \"value\": \"{3}\" }},\r\n",
                        sshihabb007P + 1, shihabFld.ShihabFieldName, shihabFld.Sshihabb007FieldType, shihabFld.Sshihabb007FieldValue);
                }
            }
            mehediJson.AppendLine("  ]");
            mehediJson.AppendLine("}");
            sshihabb007IsRightAiDrawerOpen = true;
            sshihabb007RightAiDrawerPanel.Visible = true;
            shihabAiOutputConsoleBox.Text = mehediJson.ToString();
        }

        private void ExecuteMehediFxjseCommandFromBox()
        {
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("ShihabJsDebugger");
            bool shihabMutated;
            string mehediResult = ShihabSecuritySignAndAiEngine.ExecuteShihabFxjseScript(ShihabActiveDoc, shihabFxjseCommandInputBox.Text, out shihabMutated);
            sshihabb007FxjseOutputBox.AppendText(mehediResult + Environment.NewLine);
            if (shihabMutated)
            {
                ShihabWinRtRenderAndOcrEngine.InvalidateShihabDocumentCache(ShihabActiveDoc.ShihabDocumentId);
                RefreshAllShihabWorkstationPanels();
            }
            else
            {
                UpdateShihabStatusBar();
            }
        }

        private void ExecuteShihabLuceneQuickSearch(string mehediQuery)
        {
            sshihabb007PluginLoader.TriggerShihabPluginOnDemand("MehediLuceneSearch");
            mehediActiveLeftRailMode = "Search";
            shihabIsLeftDrawerExpanded = true;
            sshihabb007LeftSlideOutDrawerPanel.Visible = true;
            RefreshMehediLeftNavigationDock();
            UpdateShihabStatusBar();
        }

        private void JumpToShihabPage(int mehediTargetPageIdx)
        {
            if (ShihabActiveDoc == null) return;
            sshihabb007ActivePageIndex = Math.Max(0, Math.Min(ShihabActiveDoc.Sshihabb007Pages.Count - 1, mehediTargetPageIdx));
            sshihabb007CenterCanvasScrollHost.AutoScrollPosition = new Point(0, 0);
            RenderActiveShihabPageToCanvas();
            if (shihabIsLeftDrawerExpanded && mehediActiveLeftRailMode == "Thumbnails")
                RefreshMehediLeftNavigationDock();
            UpdateShihabStatusBar();
        }

        private void NavigateShihabPage(int mehediDirection)
        {
            if (ShihabActiveDoc == null) return;
            JumpToShihabPage(sshihabb007ActivePageIndex + mehediDirection);
        }

        private void ChangeShihabZoom(float mehediDelta)
        {
            SetExactShihabZoom(shihabZoomScale + mehediDelta);
        }

        private void SetExactShihabZoom(float mehediNewScale)
        {
            shihabZoomScale = Math.Max(0.40f, Math.Min(2.80f, mehediNewScale));
            int sshihabb007Percent = (int)Math.Round(shihabZoomScale * 100f);
            if (sshihabb007ZoomSliderBar != null)
            {
                sshihabb007ZoomSliderBar.Value = Math.Max(sshihabb007ZoomSliderBar.Minimum, Math.Min(sshihabb007ZoomSliderBar.Maximum, sshihabb007Percent));
            }
            if (shihabZoomPercentButton != null)
            {
                shihabZoomPercentButton.Text = sshihabb007Percent + "% ▾";
            }
            RenderActiveShihabPageToCanvas();
            UpdateShihabStatusBar();
        }

        private void FitShihabPageToViewportHeight()
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfPageModel shihabPg = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            float mehediAvailH = Math.Max(200f, sshihabb007CenterCanvasScrollHost.ClientSize.Height - 36f);
            SetExactShihabZoom(mehediAvailH / Math.Max(200f, shihabPg.Sshihabb007HeightPt));
        }

        private void FitShihabPageToViewportWidth()
        {
            if (ShihabActiveDoc == null) return;
            MehediPdfPageModel shihabPg = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
            float mehediAvailW = Math.Max(240f, sshihabb007CenterCanvasScrollHost.ClientSize.Width - 64f);
            SetExactShihabZoom(mehediAvailW / Math.Max(200f, shihabPg.MehediWidthPt));
        }

        private void ToggleShihabBrightAndDarkTheme()
        {
            ShihabThemePalette.ShihabCurrentSkin = ShihabThemePalette.IsShihabDarkTheme
                ? Sshihabb007UiThemeSkin.MehediFoxitClassicBright
                : Sshihabb007UiThemeSkin.ShihabObsidianVioletDark;

            ApplyShihabThemeToAllControls();
        }

        private void ApplyShihabThemeToAllControls()
        {
            bool shihabDark = ShihabThemePalette.IsShihabDarkTheme;
            sshihabb007ThemeSwitchPillBtn.Text = shihabDark ? "☀ Bright Mode" : "🌙 Dark Mode";

            this.BackColor = ShihabThemePalette.MehediWindowBg;
            mehediTopPurpleTitleBar.BackColor = ShihabThemePalette.ShihabTopPurpleHeaderBg;
            shihabTopSearchContainer.BackColor = ShihabThemePalette.MehediTopSearchBoxBg;
            mehediTopSearchInputBox.BackColor = ShihabThemePalette.MehediTopSearchBoxBg;

            sshihabb007RibbonTabStripPanel.BackColor = ShihabThemePalette.MehediWindowBg;
            shihabRibbonCommandsBarPanel.BackColor = ShihabThemePalette.Sshihabb007RibbonBg;
            mehediDocTabStripPanel.BackColor = ShihabThemePalette.ShihabDocTabStripBg;

            mehediLeftIconRailPanel.BackColor = ShihabThemePalette.MehediWindowBg;
            sshihabb007LeftSlideOutDrawerPanel.BackColor = ShihabThemePalette.ShihabPanelBg;
            mehediLeftNavigationList.BackColor = ShihabThemePalette.ShihabPanelBg;
            mehediLeftNavigationList.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

            sshihabb007CenterCanvasScrollHost.BackColor = ShihabThemePalette.MehediCanvasWorkspaceBg;
            mehediFloatingRobotBadgeBtn.BackColor = ShihabThemePalette.MehediWindowBg;
            mehediFloatingRobotBadgeBtn.ForeColor = ShihabThemePalette.ShihabPrimaryAccent;

            sshihabb007RightAiDrawerPanel.BackColor = ShihabThemePalette.ShihabPanelBg;
            shihabAiOutputConsoleBox.BackColor = ShihabThemePalette.MehediWindowBg;
            shihabAiOutputConsoleBox.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;
            mehediAiQuestionInputBox.BackColor = ShihabThemePalette.MehediWindowBg;
            mehediAiQuestionInputBox.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;
            sshihabb007FxjseOutputBox.BackColor = ShihabThemePalette.MehediWindowBg;
            sshihabb007FxjseOutputBox.ForeColor = shihabDark ? Color.Aquamarine : Color.DarkSlateBlue;
            shihabFxjseCommandInputBox.BackColor = ShihabThemePalette.MehediWindowBg;
            shihabFxjseCommandInputBox.ForeColor = ShihabThemePalette.Sshihabb007TextPrimary;

            sshihabb007BackstageHostPanel.BackColor = ShihabThemePalette.MehediWindowBg;
            shihabBackstageLeftNavPanel.BackColor = ShihabThemePalette.ShihabPanelBg;
            mehediBackstageMiddleSubPanel.BackColor = ShihabThemePalette.MehediWindowBg;
            sshihabb007BackstageRightListPanel.BackColor = ShihabThemePalette.MehediWindowBg;

            mehediBottomStatusBar.BackColor = ShihabThemePalette.Sshihabb007RibbonBg;
            BuildShihabBottomStatusBarControls();

            ShihabThemePalette.ApplyMehediDarkWindowFrame(this.Handle, shihabDark);
            ShihabThemePalette.ApplySshihabb007DarkScrollbarsRecursive(this);

            RebuildShihabRibbonTabStrip();
            RebuildShihabRibbonButtonsForActiveTab();
            if (shihabIsBackstageVisible) RefreshShihabBackstageRecentDocumentsList();
            RefreshAllShihabWorkstationPanels();
        }

        private void UpdateShihabStatusBar()
        {
            if (ShihabActiveDoc == null) return;
            if (sshihabb007PageNumberInputBox != null)
            {
                sshihabb007PageNumberInputBox.Text = (sshihabb007ActivePageIndex + 1).ToString(CultureInfo.InvariantCulture);
            }
            if (shihabTotalPagesStatusLabel != null)
            {
                shihabTotalPagesStatusLabel.Text = "/ " + ShihabActiveDoc.Sshihabb007Pages.Count;
            }
            if (mehediToolHintStatusLabel != null)
            {
                MehediPdfPageModel sshihabb007Pg = ShihabActiveDoc.Sshihabb007Pages[sshihabb007ActivePageIndex];
                mehediToolHintStatusLabel.Text = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}  |  {1:F0}×{2:F0} pt  |  PlgDynLoader: {3}/{4} .fpi",
                    mehediCurrentTool,
                    sshihabb007Pg.MehediWidthPt,
                    sshihabb007Pg.Sshihabb007HeightPt,
                    sshihabb007PluginLoader.MehediLoadedPluginCount,
                    sshihabb007PluginLoader.ShihabAllPlugins.Count);
            }
            int shihabPct = (int)Math.Round(shihabZoomScale * 100f);
            if (shihabZoomPercentButton != null)
            {
                shihabZoomPercentButton.Text = shihabPct + "% ▾";
            }
        }

        private void OpenShihabExtensionFolderInExplorer()
        {
            string shihabExtDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extension");
            if (Directory.Exists(shihabExtDir))
            {
                Process.Start("explorer.exe", "\"" + shihabExtDir + "\"");
            }
        }

        private void ShowShihabAboutBox()
        {
            MessageBox.Show(
                this,
                "PDF PRO v14.0.4.33508 (Starship Architecture Edition)\r\n" +
                "Engineered by Mehedi Hasan Shihab (@sshihabb007)\r\n\r\n" +
                "• Native Direct2D High-DPI PDF Renderer (Windows.Data.Pdf)\r\n" +
                "• Neural Optical Character Recognition (Windows.Media.Ocr)\r\n" +
                "• Solid Office Exporter (.docx, .xlsx, .csv, .html, .md, .png)\r\n" +
                "• Callas PDF/A-2b & Section 508 Preflight Auditor\r\n" +
                "• PlgDynLoader Dynamic .fpi Plugin Architecture (32 Modules)\r\n" +
                "• X.509 PKCS#7 Digital Signatures, Whiteout Redaction & AI Assistant",
                "About PDF PRO (@sshihabb007)",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void OnShihabGlobalShortcutKeyDown(object shihabSender, KeyEventArgs mehediE)
        {
            if (mehediE.Control && mehediE.KeyCode == Keys.O)
            {
                mehediE.SuppressKeyPress = true;
                PromptAndOpenShihabPdfFile();
            }
            else if (mehediE.Control && mehediE.KeyCode == Keys.S)
            {
                mehediE.SuppressKeyPress = true;
                SaveCurrentShihabDocument();
            }
            else if (mehediE.KeyCode == Keys.PageDown || mehediE.KeyCode == Keys.Right)
            {
                if (!mehediTopSearchInputBox.Focused && !sshihabb007PageNumberInputBox.Focused)
                    NavigateShihabPage(1);
            }
            else if (mehediE.KeyCode == Keys.PageUp || mehediE.KeyCode == Keys.Left)
            {
                if (!mehediTopSearchInputBox.Focused && !sshihabb007PageNumberInputBox.Focused)
                    NavigateShihabPage(-1);
            }
            else if (mehediE.KeyCode == Keys.Delete)
            {
                DeleteSelectedShihabAnnotation();
            }
            else if (mehediE.KeyCode == Keys.Escape && shihabIsBackstageVisible)
            {
                SetShihabBackstageVisibility(false);
                RebuildShihabRibbonTabStrip();
            }
        }
    }
}
