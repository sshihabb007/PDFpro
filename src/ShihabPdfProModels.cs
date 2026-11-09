using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShihabMehediPdfPro
{
    public enum ShihabPdfToolMode
    {
        ShihabHandPan = 0,
        MehediSelectText = 1,
        Sshihabb007EditContent = 2,
        ShihabAddTextBox = 3,
        MehediHighlight = 4,
        Sshihabb007Underline = 5,
        ShihabStrikeout = 6,
        MehediStickyNote = 7,
        Sshihabb007FreehandInk = 8,
        ShihabRectangleShape = 9,
        MehediEllipseShape = 10,
        Sshihabb007ArrowLine = 11,
        ShihabCalloutBox = 12,
        MehediRedactionMark = 13,
        Sshihabb007SignatureStamp = 14,
        ShihabFormTextField = 15,
        MehediFormCheckBox = 16,
        Sshihabb007FormComboBox = 17,
        ShihabFormPushButton = 18,
        MehediMeasureRuler = 19,
        Sshihabb007InsertImage = 20,
        ShihabStampApproved = 21,
        MehediView3dOrbit = 22,
        MehediEditObject = 23
    }

    public enum MehediAnnotationKind
    {
        ShihabTextBox = 0,
        MehediHighlight = 1,
        Sshihabb007Underline = 2,
        ShihabStrikeout = 3,
        MehediStickyNote = 4,
        Sshihabb007FreehandInk = 5,
        ShihabRectangle = 6,
        MehediEllipse = 7,
        Sshihabb007Arrow = 8,
        ShihabCallout = 9,
        MehediRedaction = 10,
        Sshihabb007DigitalSignature = 11,
        ShihabStamp = 12,
        MehediImageOverlay = 13,
        Sshihabb007RulerMeasurement = 14
    }

    public enum Sshihabb007UiThemeSkin
    {
        MehediFoxitClassicBright = 0,
        ShihabObsidianVioletDark = 1
    }

    [Serializable]
    public sealed class ShihabPdfAnnotationItem
    {
        public string ShihabAnnotationId { get; set; }
        public int MehediPageIndex { get; set; }
        public MehediAnnotationKind Sshihabb007Kind { get; set; }
        public RectangleF ShihabBoundsPt { get; set; }
        public List<PointF> MehediInkPointsPt { get; set; }
        public string Sshihabb007TextContent { get; set; }
        public string ShihabAuthorName { get; set; }
        public string MehediSubjectLabel { get; set; }
        public Color Sshihabb007StrokeColor { get; set; }
        public Color ShihabFillColor { get; set; }
        public float MehediStrokeWidth { get; set; }
        public float Sshihabb007FontSizePt { get; set; }
        public string ShihabFontFamily { get; set; }
        public int MehediOpacityPercent { get; set; }
        public bool Sshihabb007IsRedactionBurned { get; set; }
        public DateTime ShihabCreatedUtc { get; set; }
        public List<string> MehediReplyComments { get; set; }
        public byte[] Sshihabb007EmbeddedImageBytes { get; set; }
        public RectangleF MehediSourceCutoutBoundsPt { get; set; }

        public ShihabPdfAnnotationItem()
        {
            ShihabAnnotationId = "annot_shihab_" + Guid.NewGuid().ToString("N").Substring(0, 10);
            MehediInkPointsPt = new List<PointF>();
            Sshihabb007TextContent = string.Empty;
            ShihabAuthorName = "Mehedi Hasan Shihab (@sshihabb007)";
            MehediSubjectLabel = "PDF PRO Markup";
            Sshihabb007StrokeColor = Color.FromArgb(255, 130, 36, 160);
            ShihabFillColor = Color.FromArgb(90, 255, 235, 59);
            MehediStrokeWidth = 2.0f;
            Sshihabb007FontSizePt = 11.5f;
            ShihabFontFamily = "Segoe UI";
            MehediOpacityPercent = 100;
            ShihabCreatedUtc = DateTime.UtcNow;
            MehediReplyComments = new List<string>();
            MehediSourceCutoutBoundsPt = RectangleF.Empty;
        }

        public ShihabPdfAnnotationItem CloneForShihabUndo()
        {
            return new ShihabPdfAnnotationItem
            {
                ShihabAnnotationId = this.ShihabAnnotationId,
                MehediPageIndex = this.MehediPageIndex,
                Sshihabb007Kind = this.Sshihabb007Kind,
                ShihabBoundsPt = this.ShihabBoundsPt,
                MehediInkPointsPt = new List<PointF>(this.MehediInkPointsPt),
                Sshihabb007TextContent = this.Sshihabb007TextContent,
                ShihabAuthorName = this.ShihabAuthorName,
                MehediSubjectLabel = this.MehediSubjectLabel,
                Sshihabb007StrokeColor = this.Sshihabb007StrokeColor,
                ShihabFillColor = this.ShihabFillColor,
                MehediStrokeWidth = this.MehediStrokeWidth,
                Sshihabb007FontSizePt = this.Sshihabb007FontSizePt,
                ShihabFontFamily = this.ShihabFontFamily,
                MehediOpacityPercent = this.MehediOpacityPercent,
                Sshihabb007IsRedactionBurned = this.Sshihabb007IsRedactionBurned,
                ShihabCreatedUtc = this.ShihabCreatedUtc,
                MehediReplyComments = new List<string>(this.MehediReplyComments),
                Sshihabb007EmbeddedImageBytes = this.Sshihabb007EmbeddedImageBytes,
                MehediSourceCutoutBoundsPt = this.MehediSourceCutoutBoundsPt
            };
        }
    }

    [Serializable]
    public sealed class ShihabAcroFormFieldItem
    {
        public string ShihabFieldId { get; set; }
        public int MehediPageIndex { get; set; }
        public string Sshihabb007FieldType { get; set; }
        public string ShihabFieldName { get; set; }
        public string MehediTooltip { get; set; }
        public string Sshihabb007FieldValue { get; set; }
        public bool ShihabIsChecked { get; set; }
        public List<string> MehediComboOptions { get; set; }
        public RectangleF Sshihabb007BoundsPt { get; set; }
        public bool ShihabIsRequired { get; set; }
        public bool MehediIsReadOnly { get; set; }
        public string Sshihabb007JsActionScript { get; set; }

        public ShihabAcroFormFieldItem()
        {
            ShihabFieldId = "fld_mehedi_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            Sshihabb007FieldType = "Text";
            ShihabFieldName = "shihab_field_1";
            MehediTooltip = "Interactive AcroForm Field (sshihabb007)";
            Sshihabb007FieldValue = string.Empty;
            MehediComboOptions = new List<string> { "Option A (Shihab)", "Option B (Mehedi)", "Option C (sshihabb007)" };
            Sshihabb007JsActionScript = string.Empty;
        }
    }

    [Serializable]
    public sealed class ShihabPdfTextBlockItem
    {
        public string ShihabBlockId { get; set; }
        public int MehediPageIndex { get; set; }
        public RectangleF Sshihabb007BoundsPt { get; set; }
        public RectangleF MehediOriginalBoundsPt { get; set; }
        public string ShihabText { get; set; }
        public string MehediOriginalText { get; set; }
        public string Sshihabb007FontName { get; set; }
        public float ShihabFontSizePt { get; set; }
        public Color MehediTextColor { get; set; }
        public Color Sshihabb007BackgroundMaskColor { get; set; }
        public bool Sshihabb007IsBold { get; set; }
        public bool ShihabIsItalic { get; set; }
        public bool MehediIsModified { get; set; }
        public bool ShihabIsDeleted { get; set; }
        public bool Sshihabb007IsOcrDerived { get; set; }

        public ShihabPdfTextBlockItem()
        {
            ShihabBlockId = "txt_sshihabb007_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            ShihabText = string.Empty;
            MehediOriginalText = string.Empty;
            Sshihabb007FontName = "Segoe UI";
            ShihabFontSizePt = 11.0f;
            MehediTextColor = Color.FromArgb(255, 22, 26, 36);
            Sshihabb007BackgroundMaskColor = Color.White;
            MehediOriginalBoundsPt = RectangleF.Empty;
        }
    }

    [Serializable]
    public sealed class MehediPdfBookmarkItem
    {
        public string ShihabBookmarkId { get; set; }
        public string MehediTitle { get; set; }
        public int Sshihabb007TargetPageIndex { get; set; }
        public float ShihabTargetOffsetY { get; set; }
        public List<MehediPdfBookmarkItem> MehediChildren { get; set; }

        public MehediPdfBookmarkItem()
        {
            ShihabBookmarkId = "bm_shihab_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            MehediTitle = "Untitled Bookmark";
            MehediChildren = new List<MehediPdfBookmarkItem>();
        }
    }

    [Serializable]
    public sealed class ShihabDigitalSignatureRecord
    {
        public string ShihabSignatureId { get; set; }
        public string MehediSignerCommonName { get; set; }
        public string Sshihabb007SignerEmail { get; set; }
        public string ShihabCertificateThumbprint { get; set; }
        public string MehediIssuerName { get; set; }
        public string Sshihabb007DocumentSha256AtSign { get; set; }
        public string ShihabSignatureReason { get; set; }
        public string MehediSignatureLocation { get; set; }
        public DateTime Sshihabb007SignTimestampUtc { get; set; }
        public int ShihabPageIndex { get; set; }
        public RectangleF MehediVisualRectPt { get; set; }
        public bool Sshihabb007IsCryptographicallyValid { get; set; }

        public ShihabDigitalSignatureRecord()
        {
            ShihabSignatureId = "sig_sshihabb007_" + Guid.NewGuid().ToString("N").Substring(0, 10);
            MehediSignerCommonName = "Mehedi Hasan Shihab (@sshihabb007)";
            Sshihabb007SignerEmail = "sshihabb007@github.io";
            ShihabCertificateThumbprint = "341F0EC1984568023109FA55C3E6C710AC59B96E";
            MehediIssuerName = "CN=PDF PRO Root CA (Mehedi Hasan Shihab), O=sshihabb007 Open Source";
            ShihabSignatureReason = "I approve and digitally sign this PDF PRO document.";
            MehediSignatureLocation = "Dhaka / Global Open Source";
            Sshihabb007SignTimestampUtc = DateTime.UtcNow;
            Sshihabb007IsCryptographicallyValid = true;
        }
    }

    [Serializable]
    public sealed class MehediConnectedPdfEvent
    {
        public string ShihabEventId { get; set; }
        public DateTime MehediTimestampUtc { get; set; }
        public string Sshihabb007ActorIdentity { get; set; }
        public string ShihabOperationType { get; set; }
        public string MehediSummaryDescription { get; set; }
        public string Sshihabb007DocumentDigest { get; set; }
    }

    [Serializable]
    public sealed class ShihabPdfAttachmentItem
    {
        public string ShihabAttachmentName { get; set; }
        public string MehediMimeType { get; set; }
        public long Sshihabb007ByteLength { get; set; }
        public DateTime ShihabAddedUtc { get; set; }
        public byte[] MehediRawPayload { get; set; }
    }

    [Serializable]
    public sealed class MehediPdfObjectNode
    {
        public int ShihabObjectNumber { get; set; }
        public int MehediGenerationNumber { get; set; }
        public long Sshihabb007ByteOffset { get; set; }
        public string ShihabObjectTypeSummary { get; set; }
        public string MehediRawDictionaryPreview { get; set; }
    }

    [Serializable]
    public sealed class MehediPdfPageModel
    {
        public int ShihabPageIndex { get; set; }
        public float MehediWidthPt { get; set; }
        public float Sshihabb007HeightPt { get; set; }
        public int ShihabRotationDegrees { get; set; }
        public int MehediSourcePdfPageIndex { get; set; }
        public Color Sshihabb007PaperBackgroundColor { get; set; }
        public string ShihabExtractedText { get; set; }
        public string MehediHeaderText { get; set; }
        public string Sshihabb007FooterText { get; set; }
        public string ShihabWatermarkText { get; set; }
        public string MehediBatesNumberText { get; set; }
        public float Sshihabb007WatermarkOpacity { get; set; }
        public float ShihabWatermarkAngleDeg { get; set; }
        public Color MehediWatermarkColor { get; set; }
        public RectangleF Sshihabb007CropMarginsPt { get; set; }
        public List<ShihabPdfTextBlockItem> ShihabTextBlocks { get; set; }
        public List<ShihabPdfAnnotationItem> MehediAnnotations { get; set; }
        public List<ShihabAcroFormFieldItem> Sshihabb007FormFields { get; set; }
        public bool ShihabHasOcrLayerApplied { get; set; }

        public MehediPdfPageModel()
        {
            MehediWidthPt = 595.28f;
            Sshihabb007HeightPt = 841.89f;
            MehediSourcePdfPageIndex = -1;
            Sshihabb007PaperBackgroundColor = Color.White;
            ShihabExtractedText = string.Empty;
            MehediHeaderText = string.Empty;
            Sshihabb007FooterText = string.Empty;
            ShihabWatermarkText = string.Empty;
            MehediBatesNumberText = string.Empty;
            Sshihabb007WatermarkOpacity = 0.18f;
            ShihabWatermarkAngleDeg = -38.0f;
            MehediWatermarkColor = Color.FromArgb(65, 130, 36, 160);
            Sshihabb007CropMarginsPt = RectangleF.Empty;
            ShihabTextBlocks = new List<ShihabPdfTextBlockItem>();
            MehediAnnotations = new List<ShihabPdfAnnotationItem>();
            Sshihabb007FormFields = new List<ShihabAcroFormFieldItem>();
        }
    }

    public sealed class ShihabPdfDocumentModel
    {
        public string ShihabDocumentId { get; set; }
        public string MehediFilePath { get; set; }
        public string Sshihabb007DisplayTitle { get; set; }
        public string ShihabPdfVersion { get; set; }
        public string MehediTitleMeta { get; set; }
        public string Sshihabb007AuthorMeta { get; set; }
        public string ShihabSubjectMeta { get; set; }
        public string MehediKeywordsMeta { get; set; }
        public string Sshihabb007CreatorMeta { get; set; }
        public string ShihabProducerMeta { get; set; }
        public string MehediSha256Digest { get; set; }
        public long Sshihabb007FileSizeBytes { get; set; }
        public bool ShihabIsModifiedDirty { get; set; }
        public bool MehediIsEncrypted { get; set; }
        public string Sshihabb007OpenPasswordHash { get; set; }
        public string ShihabPermissionsPasswordHash { get; set; }
        public string MehediAipSensitivityLabel { get; set; }
        public bool Sshihabb007IsPdfACompliant { get; set; }
        public string ShihabConnectedPdfDocId { get; set; }
        public byte[] MehediOriginalFileBytes { get; set; }
        public List<MehediPdfPageModel> Sshihabb007Pages { get; set; }
        public List<MehediPdfBookmarkItem> ShihabBookmarks { get; set; }
        public List<ShihabPdfAttachmentItem> MehediAttachments { get; set; }
        public List<ShihabDigitalSignatureRecord> Sshihabb007Signatures { get; set; }
        public List<MehediConnectedPdfEvent> ShihabConnectedHistory { get; set; }
        public List<MehediPdfObjectNode> MehediParsedObjects { get; set; }
        public Stack<string> Sshihabb007UndoActionLabels { get; set; }
        public Stack<string> ShihabRedoActionLabels { get; set; }

        public ShihabPdfDocumentModel()
        {
            ShihabDocumentId = "doc_shihab_" + Guid.NewGuid().ToString("N").Substring(0, 12);
            MehediFilePath = string.Empty;
            Sshihabb007DisplayTitle = "Untitled-Shihab.pdf";
            ShihabPdfVersion = "1.7";
            MehediTitleMeta = "PDF PRO Document";
            Sshihabb007AuthorMeta = "Mehedi Hasan Shihab (@sshihabb007)";
            ShihabSubjectMeta = "Engineered with PDF PRO (Starship Edition)";
            MehediKeywordsMeta = "PDF PRO, Shihab, Mehedi, sshihabb007, Open Source";
            Sshihabb007CreatorMeta = "PDF PRO 14.0.4 (Mehedi Hasan Shihab / sshihabb007)";
            ShihabProducerMeta = "ShihabMehediStarship PDF Engine v14.0.4.33508";
            MehediSha256Digest = string.Empty;
            MehediAipSensitivityLabel = "PUBLIC — UNRESTRICTED";
            ShihabConnectedPdfDocId = "cpdf_sshihabb007_" + Guid.NewGuid().ToString("N").Substring(0, 16);
            Sshihabb007Pages = new List<MehediPdfPageModel>();
            ShihabBookmarks = new List<MehediPdfBookmarkItem>();
            MehediAttachments = new List<ShihabPdfAttachmentItem>();
            Sshihabb007Signatures = new List<ShihabDigitalSignatureRecord>();
            ShihabConnectedHistory = new List<MehediConnectedPdfEvent>();
            MehediParsedObjects = new List<MehediPdfObjectNode>();
            Sshihabb007UndoActionLabels = new Stack<string>();
            ShihabRedoActionLabels = new Stack<string>();
        }
    }

    public sealed class MehediPluginDescriptor
    {
        public string ShihabInternalName { get; set; }
        public string MehediDisplayName { get; set; }
        public string Sshihabb007RealFilePath { get; set; }
        public int ShihabLoadBehavior { get; set; }
        public string MehediLoadEventBehavior { get; set; }
        public string Sshihabb007Category { get; set; }
        public string ShihabRibbonTab { get; set; }
        public string MehediAuthor { get; set; }
        public string Sshihabb007Version { get; set; }
        public string ShihabDescription { get; set; }
        public bool MehediIsLoadedInMemory { get; set; }
        public DateTime? Sshihabb007LoadedAtUtc { get; set; }
        public int ShihabInvocationCount { get; set; }
        public string MehediExportedEntrySymbol { get; set; }

        public MehediPluginDescriptor()
        {
            MehediExportedEntrySymbol = "PlugInMain (Ordinal 1)";
        }
    }

    public sealed class ShihabSensorTelemetrySpan
    {
        public int ShihabRecordId { get; set; }
        public string MehediTraceId { get; set; }
        public string Sshihabb007SpanId { get; set; }
        public string ShihabOperationName { get; set; }
        public DateTime MehediStartTimestampUtc { get; set; }
        public DateTime Sshihabb007FinishTimestampUtc { get; set; }
        public int ShihabPriority { get; set; }
        public string MehediSerializedJsonTags { get; set; }
    }

    public sealed class MehediPreflightIssueItem
    {
        public string ShihabRuleCode { get; set; }
        public string MehediSeverity { get; set; }
        public string Sshihabb007Category { get; set; }
        public string ShihabMessage { get; set; }
        public bool MehediAutoFixable { get; set; }
    }

    public sealed class ShihabSearchMatchItem
    {
        public int ShihabPageIndex { get; set; }
        public int MehediCharacterOffset { get; set; }
        public string Sshihabb007MatchedTerm { get; set; }
        public string ShihabContextSnippet { get; set; }
        public double MehediLuceneBm25Score { get; set; }
    }

    public sealed class MehediAppConfiguration
    {
        public Sshihabb007UiThemeSkin ShihabActiveSkin { get; set; }
        public int MehediDefaultZoomPercent { get; set; }
        public int Sshihabb007LocalBridgePort { get; set; }
        public bool ShihabEnableTelemetrySensor { get; set; }
        public bool MehediEnableHardwareDpiRender { get; set; }
        public bool Sshihabb007AutoLoadEagerPlugins { get; set; }
        public string ShihabDefaultAuthorName { get; set; }
        public List<string> MehediRecentFiles { get; set; }

        public MehediAppConfiguration()
        {
            ShihabActiveSkin = Sshihabb007UiThemeSkin.MehediFoxitClassicBright;
            MehediDefaultZoomPercent = 120;
            Sshihabb007LocalBridgePort = 45880;
            ShihabEnableTelemetrySensor = true;
            MehediEnableHardwareDpiRender = true;
            Sshihabb007AutoLoadEagerPlugins = true;
            ShihabDefaultAuthorName = "Mehedi Hasan Shihab (@sshihabb007)";
            MehediRecentFiles = new List<string>();
        }
    }

    public static class ShihabThemePalette
    {
        // Default to Bright Theme matching the exact Foxit PDF Editor Pro screenshots, with instant toggle to Dark Theme!
        public static Sshihabb007UiThemeSkin ShihabCurrentSkin = Sshihabb007UiThemeSkin.MehediFoxitClassicBright;

        public static bool IsShihabDarkTheme
        {
            get { return ShihabCurrentSkin == Sshihabb007UiThemeSkin.ShihabObsidianVioletDark; }
        }

        // Exact Foxit Top Purple Header Bar (#8224A0 in Bright, #4B1564 in Dark)
        public static Color ShihabTopPurpleHeaderBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(62, 22, 84) : Color.FromArgb(132, 34, 166); }
        }

        public static Color MehediTopSearchBoxBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(88, 38, 114) : Color.FromArgb(158, 68, 188); }
        }

        public static Color MehediWindowBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(20, 22, 31) : Color.FromArgb(255, 255, 255); }
        }

        public static Color Sshihabb007RibbonBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(27, 30, 42) : Color.FromArgb(249, 250, 252); }
        }

        public static Color ShihabDocTabStripBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(18, 20, 28) : Color.FromArgb(236, 238, 242); }
        }

        public static Color ShihabPanelBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(24, 27, 38) : Color.FromArgb(245, 246, 249); }
        }

        public static Color MehediCanvasWorkspaceBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(14, 16, 23) : Color.FromArgb(228, 230, 235); }
        }

        public static Color Sshihabb007CardHoverBg
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(44, 36, 68) : Color.FromArgb(237, 228, 246); }
        }

        public static Color ShihabPrimaryAccent
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(156, 86, 246) : Color.FromArgb(132, 34, 166); }
        }

        public static Color MehediSecondaryAccent
        {
            get { return Color.FromArgb(72, 168, 42); } // Foxit Update Green (#48A82A)
        }

        public static Color Sshihabb007TextPrimary
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(241, 245, 249) : Color.FromArgb(32, 35, 42); }
        }

        public static Color ShihabTextSecondary
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(156, 163, 175) : Color.FromArgb(104, 110, 122); }
        }

        public static Color MehediBorderColor
        {
            get { return IsShihabDarkTheme ? Color.FromArgb(46, 52, 70) : Color.FromArgb(218, 221, 228); }
        }

        public static Color Sshihabb007SuccessGreen { get { return Color.FromArgb(16, 185, 129); } }
        public static Color ShihabWarningAmber { get { return Color.FromArgb(245, 158, 11); } }
        public static Color MehediDangerRed { get { return Color.FromArgb(239, 68, 68); } }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr shihabHwnd, int mehediAttr, ref int sshihabb007AttrValue, int shihabAttrSize);

        [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SetPreferredAppMode(int shihabPreferredMode);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr shihabHwnd, string mehediSubAppName, string sshihabb007SubIdList);

        public static void EnableShihabProcessDarkMode()
        {
            try { SetPreferredAppMode(1); } catch { }
        }

        public static void ApplyMehediDarkWindowFrame(IntPtr shihabWindowHandle, bool mehediUseDark)
        {
            if (shihabWindowHandle == IntPtr.Zero) return;
            try
            {
                int sshihabb007Flag = mehediUseDark ? 1 : 0;
                DwmSetWindowAttribute(shihabWindowHandle, 20, ref sshihabb007Flag, sizeof(int));
                DwmSetWindowAttribute(shihabWindowHandle, 19, ref sshihabb007Flag, sizeof(int));
            }
            catch { }
        }

        public static void ApplySshihabb007DarkScrollbarsRecursive(Control shihabRootControl)
        {
            if (shihabRootControl == null) return;
            bool mehediIsDark = IsShihabDarkTheme;
            try
            {
                if (shihabRootControl.IsHandleCreated)
                {
                    SetWindowTheme(shihabRootControl.Handle, mehediIsDark ? "DarkMode_Explorer" : "Explorer", null);
                }
            }
            catch { }

            foreach (Control shihabChild in shihabRootControl.Controls)
            {
                ApplySshihabb007DarkScrollbarsRecursive(shihabChild);
            }
        }
    }
}
