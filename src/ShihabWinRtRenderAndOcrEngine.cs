using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace ShihabMehediPdfPro
{
    public static class ShihabWinRtRenderAndOcrEngine
    {
        private static readonly object shihabCacheLock = new object();
        private static readonly Dictionary<string, Bitmap> mehediLruBitmapCache = new Dictionary<string, Bitmap>();
        private static readonly LinkedList<string> sshihabb007LruOrder = new LinkedList<string>();
        private const int ShihabMaxCachedBitmaps = 36;

        public static void InvalidateShihabDocumentCache(string mehediDocumentId)
        {
            if (string.IsNullOrEmpty(mehediDocumentId)) return;
            lock (shihabCacheLock)
            {
                List<string> sshihabb007KeysToRemove = new List<string>();
                foreach (string shihabKey in mehediLruBitmapCache.Keys)
                {
                    if (shihabKey.StartsWith(mehediDocumentId + "|", StringComparison.Ordinal))
                    {
                        sshihabb007KeysToRemove.Add(shihabKey);
                    }
                }
                foreach (string mehediKey in sshihabb007KeysToRemove)
                {
                    Bitmap sshihabb007Old;
                    if (mehediLruBitmapCache.TryGetValue(mehediKey, out sshihabb007Old))
                    {
                        try { sshihabb007Old.Dispose(); } catch { }
                        mehediLruBitmapCache.Remove(mehediKey);
                    }
                    sshihabb007LruOrder.Remove(mehediKey);
                }
            }
        }

        private static TShihabResult WaitShihabWinRtAsyncOp<TShihabResult>(IAsyncOperation<TShihabResult> mehediOp)
        {
            while (mehediOp.Status == AsyncStatus.Started)
            {
                Thread.Sleep(1);
            }
            if (mehediOp.Status == AsyncStatus.Error && mehediOp.ErrorCode != null)
            {
                throw mehediOp.ErrorCode;
            }
            return mehediOp.GetResults();
        }

        private static void WaitMehediWinRtAsyncAction(IAsyncAction shihabAction)
        {
            while (shihabAction.Status == AsyncStatus.Started)
            {
                Thread.Sleep(1);
            }
            if (shihabAction.Status == AsyncStatus.Error && shihabAction.ErrorCode != null)
            {
                throw shihabAction.ErrorCode;
            }
            shihabAction.GetResults();
        }

        private static void WriteSshihabb007BytesToWinRtStream(InMemoryRandomAccessStream shihabStream, byte[] mehediBytes)
        {
            using (DataWriter sshihabb007Writer = new DataWriter(shihabStream.GetOutputStreamAt(0)))
            {
                sshihabb007Writer.WriteBytes(mehediBytes);
                WaitShihabWinRtAsyncOp(sshihabb007Writer.StoreAsync());
                WaitShihabWinRtAsyncOp(sshihabb007Writer.FlushAsync());
                sshihabb007Writer.DetachStream();
            }
            shihabStream.Seek(0);
        }

        private static byte[] ReadShihabBytesFromWinRtStream(InMemoryRandomAccessStream mehediStream)
        {
            uint sshihabb007Size = (uint)mehediStream.Size;
            if (sshihabb007Size == 0) return new byte[0];
            using (DataReader shihabReader = new DataReader(mehediStream.GetInputStreamAt(0)))
            {
                WaitShihabWinRtAsyncOp(shihabReader.LoadAsync(sshihabb007Size));
                byte[] mehediOut = new byte[sshihabb007Size];
                shihabReader.ReadBytes(mehediOut);
                shihabReader.DetachStream();
                return mehediOut;
            }
        }

        /// <summary>
        /// Runs on an MTA worker thread via Task.Run so it never deadlocks the WinForms [STAThread] UI thread,
        /// and accurately inspects the exact page count and page dimensions of ANY PDF (including compressed PDF 1.5-2.0 /ObjStm files).
        /// </summary>
        public static List<SizeF> InspectShihabPdfPageDimensionsViaWinRt(byte[] shihabPdfBytes)
        {
            List<SizeF> mehediPageSizesPt = new List<SizeF>();
            if (shihabPdfBytes == null || shihabPdfBytes.Length < 16) return mehediPageSizesPt;

            try
            {
                return Task.Run(() =>
                {
                    List<SizeF> sshihabb007List = new List<SizeF>();
                    using (InMemoryRandomAccessStream shihabInputStream = new InMemoryRandomAccessStream())
                    {
                        WriteSshihabb007BytesToWinRtStream(shihabInputStream, shihabPdfBytes);
                        PdfDocument mehediPdfDoc = WaitShihabWinRtAsyncOp(PdfDocument.LoadFromStreamAsync(shihabInputStream));
                        uint sshihabb007Count = mehediPdfDoc.PageCount;
                        for (uint shihabIdx = 0; shihabIdx < sshihabb007Count; shihabIdx++)
                        {
                            using (PdfPage mehediPg = mehediPdfDoc.GetPage(shihabIdx))
                            {
                                // WinRT PdfPage.Size is in DIPs (1/96 inch); convert to PDF Points (1/72 inch) via * 0.75f
                                float sshihabb007W = Math.Max(100f, (float)(mehediPg.Size.Width * 0.75));
                                float shihabH = Math.Max(100f, (float)(mehediPg.Size.Height * 0.75));
                                sshihabb007List.Add(new SizeF(sshihabb007W, shihabH));
                            }
                        }
                    }
                    return sshihabb007List;
                }).GetAwaiter().GetResult();
            }
            catch
            {
                return mehediPageSizesPt;
            }
        }

        public static Bitmap RenderShihabCompositePage(
            ShihabPdfDocumentModel shihabDoc,
            int mehediPageIdx,
            float sshihabb007ZoomScale,
            int shihabSelectedAnnotationIdx,
            int mehediSelectedTextBlockIdx)
        {
            if (shihabDoc == null || mehediPageIdx < 0 || mehediPageIdx >= shihabDoc.Sshihabb007Pages.Count)
            {
                return new Bitmap(600, 840);
            }

            MehediPdfPageModel sshihabb007Page = shihabDoc.Sshihabb007Pages[mehediPageIdx];
            bool shihabSwapDims = (sshihabb007Page.ShihabRotationDegrees % 180) != 0;
            float mehediLogicalW = shihabSwapDims ? sshihabb007Page.Sshihabb007HeightPt : sshihabb007Page.MehediWidthPt;
            float sshihabb007LogicalH = shihabSwapDims ? sshihabb007Page.MehediWidthPt : sshihabb007Page.Sshihabb007HeightPt;

            int shihabTargetPixelW = Math.Max(90, Math.Min(3200, (int)Math.Round(mehediLogicalW * sshihabb007ZoomScale)));
            int mehediTargetPixelH = Math.Max(120, Math.Min(4200, (int)Math.Round(sshihabb007LogicalH * sshihabb007ZoomScale)));

            Bitmap sshihabb007BaseBitmap = GetOrRenderMehediBasePageBitmap(shihabDoc, sshihabb007Page, shihabTargetPixelW, mehediTargetPixelH);
            Bitmap shihabFinalCanvas = new Bitmap(shihabTargetPixelW, mehediTargetPixelH, PixelFormat.Format32bppPArgb);

            using (Graphics mehediG = Graphics.FromImage(shihabFinalCanvas))
            {
                mehediG.SmoothingMode = SmoothingMode.AntiAlias;
                mehediG.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                mehediG.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (sshihabb007BaseBitmap != null)
                {
                    mehediG.DrawImage(sshihabb007BaseBitmap, 0, 0, shihabTargetPixelW, mehediTargetPixelH);
                }
                else
                {
                    using (SolidBrush sshihabb007PaperBrush = new SolidBrush(sshihabb007Page.Sshihabb007PaperBackgroundColor))
                    {
                        mehediG.FillRectangle(sshihabb007PaperBrush, 0, 0, shihabTargetPixelW, mehediTargetPixelH);
                    }
                    DrawShihabFallbackTextBlocks(mehediG, sshihabb007Page, sshihabb007ZoomScale, mehediSelectedTextBlockIdx);
                }

                DrawMehediOverlaysAndAnnotations(
                    mehediG,
                    shihabDoc,
                    sshihabb007Page,
                    sshihabb007ZoomScale,
                    shihabSelectedAnnotationIdx,
                    mehediSelectedTextBlockIdx,
                    sshihabb007BaseBitmap != null);
            }

            return shihabFinalCanvas;
        }

        private static Bitmap GetOrRenderMehediBasePageBitmap(
            ShihabPdfDocumentModel shihabDoc,
            MehediPdfPageModel mehediPage,
            int sshihabb007PixelW,
            int shihabPixelH)
        {
            bool sshihabb007HasInPlaceModifiedText = false;
            if (mehediPage.ShihabTextBlocks != null)
            {
                foreach (ShihabPdfTextBlockItem shihabTb in mehediPage.ShihabTextBlocks)
                {
                    if (shihabTb.MehediIsModified)
                    {
                        sshihabb007HasInPlaceModifiedText = true;
                        break;
                    }
                }
            }

            if (sshihabb007HasInPlaceModifiedText ||
                mehediPage.MehediSourcePdfPageIndex < 0 ||
                shihabDoc.MehediOriginalFileBytes == null ||
                shihabDoc.MehediOriginalFileBytes.Length < 16)
            {
                return null;
            }

            string mehediCacheKey = string.Format(
                "{0}|p{1}|r{2}|w{3}|h{4}",
                shihabDoc.ShihabDocumentId,
                mehediPage.MehediSourcePdfPageIndex,
                mehediPage.ShihabRotationDegrees,
                sshihabb007PixelW,
                shihabPixelH);

            lock (shihabCacheLock)
            {
                Bitmap sshihabb007Cached;
                if (mehediLruBitmapCache.TryGetValue(mehediCacheKey, out sshihabb007Cached) && sshihabb007Cached != null)
                {
                    sshihabb007LruOrder.Remove(mehediCacheKey);
                    sshihabb007LruOrder.AddFirst(mehediCacheKey);
                    return sshihabb007Cached;
                }
            }

            Bitmap shihabWinRtRendered = TryRenderSshihabb007PageViaWinRtPdf(
                shihabDoc.MehediOriginalFileBytes,
                mehediPage.MehediSourcePdfPageIndex,
                sshihabb007PixelW,
                shihabPixelH);

            if (shihabWinRtRendered != null)
            {
                if (mehediPage.ShihabRotationDegrees == 90)
                    shihabWinRtRendered.RotateFlip(RotateFlipType.Rotate90FlipNone);
                else if (mehediPage.ShihabRotationDegrees == 180)
                    shihabWinRtRendered.RotateFlip(RotateFlipType.Rotate180FlipNone);
                else if (mehediPage.ShihabRotationDegrees == 270)
                    shihabWinRtRendered.RotateFlip(RotateFlipType.Rotate270FlipNone);

                lock (shihabCacheLock)
                {
                    if (!mehediLruBitmapCache.ContainsKey(mehediCacheKey))
                    {
                        while (mehediLruBitmapCache.Count >= ShihabMaxCachedBitmaps && sshihabb007LruOrder.Last != null)
                        {
                            string shihabEvictKey = sshihabb007LruOrder.Last.Value;
                            sshihabb007LruOrder.RemoveLast();
                            Bitmap mehediEvictBmp;
                            if (mehediLruBitmapCache.TryGetValue(shihabEvictKey, out mehediEvictBmp))
                            {
                                try { mehediEvictBmp.Dispose(); } catch { }
                                mehediLruBitmapCache.Remove(shihabEvictKey);
                            }
                        }
                        mehediLruBitmapCache[mehediCacheKey] = shihabWinRtRendered;
                        sshihabb007LruOrder.AddFirst(mehediCacheKey);
                    }
                }
            }

            return shihabWinRtRendered;
        }

        private static Bitmap TryRenderSshihabb007PageViaWinRtPdf(byte[] shihabPdfBytes, int mehediSourcePageIdx, int sshihabb007TargetW, int shihabTargetH)
        {
            try
            {
                // Execute inside Task.Run on an MTA thread-pool worker so WinForms [STAThread] never deadlocks!
                return Task.Run(() =>
                {
                    using (InMemoryRandomAccessStream mehediInputStream = new InMemoryRandomAccessStream())
                    {
                        WriteSshihabb007BytesToWinRtStream(mehediInputStream, shihabPdfBytes);

                        PdfDocument sshihabb007PdfDoc = WaitShihabWinRtAsyncOp(PdfDocument.LoadFromStreamAsync(mehediInputStream));
                        if (mehediSourcePageIdx < 0 || (uint)mehediSourcePageIdx >= sshihabb007PdfDoc.PageCount)
                        {
                            return null;
                        }

                        using (PdfPage shihabPdfPage = sshihabb007PdfDoc.GetPage((uint)mehediSourcePageIdx))
                        using (InMemoryRandomAccessStream mehediRenderStream = new InMemoryRandomAccessStream())
                        {
                            PdfPageRenderOptions sshihabb007RenderOpts = new PdfPageRenderOptions
                            {
                                DestinationWidth = (uint)Math.Max(64, sshihabb007TargetW),
                                DestinationHeight = (uint)Math.Max(64, shihabTargetH),
                                BackgroundColor = Windows.UI.Color.FromArgb(255, 255, 255, 255)
                            };

                            WaitMehediWinRtAsyncAction(shihabPdfPage.RenderToStreamAsync(mehediRenderStream, sshihabb007RenderOpts));
                            byte[] shihabRenderedBytes = ReadShihabBytesFromWinRtStream(mehediRenderStream);
                            if (shihabRenderedBytes == null || shihabRenderedBytes.Length == 0) return null;

                            using (MemoryStream mehediNetMs = new MemoryStream(shihabRenderedBytes))
                            using (Bitmap sshihabb007TempBmp = new Bitmap(mehediNetMs))
                            {
                                return new Bitmap(sshihabb007TempBmp);
                            }
                        }
                    }
                }).GetAwaiter().GetResult();
            }
            catch
            {
                return null;
            }
        }

        private static void DrawShihabFallbackTextBlocks(
            Graphics shihabG,
            MehediPdfPageModel mehediPage,
            float sshihabb007Scale,
            int shihabSelectedBlockIdx)
        {
            if (mehediPage.ShihabTextBlocks != null && mehediPage.ShihabTextBlocks.Count > 0)
            {
                for (int mehediIdx = 0; mehediIdx < mehediPage.ShihabTextBlocks.Count; mehediIdx++)
                {
                    ShihabPdfTextBlockItem sshihabb007Block = mehediPage.ShihabTextBlocks[mehediIdx];
                    RectangleF shihabScaledRect = new RectangleF(
                        sshihabb007Block.Sshihabb007BoundsPt.Left * sshihabb007Scale,
                        sshihabb007Block.Sshihabb007BoundsPt.Top * sshihabb007Scale,
                        sshihabb007Block.Sshihabb007BoundsPt.Width * sshihabb007Scale,
                        sshihabb007Block.Sshihabb007BoundsPt.Height * sshihabb007Scale);

                    FontStyle mehediStyle = FontStyle.Regular;
                    if (sshihabb007Block.Sshihabb007IsBold) mehediStyle |= FontStyle.Bold;
                    if (sshihabb007Block.ShihabIsItalic) mehediStyle |= FontStyle.Italic;

                    string sshihabb007Family = sshihabb007Block.Sshihabb007FontName != null && sshihabb007Block.Sshihabb007FontName.IndexOf("Courier", StringComparison.OrdinalIgnoreCase) >= 0
                        ? "Consolas"
                        : "Segoe UI";

                    using (Font shihabFont = new Font(sshihabb007Family, Math.Max(6f, sshihabb007Block.ShihabFontSizePt * sshihabb007Scale * 0.85f), mehediStyle))
                    using (SolidBrush mehediBrush = new SolidBrush(sshihabb007Block.MehediTextColor))
                    {
                        shihabG.DrawString(sshihabb007Block.ShihabText, shihabFont, mehediBrush, shihabScaledRect.Location);
                    }

                    if (mehediIdx == shihabSelectedBlockIdx)
                    {
                        using (Pen sshihabb007SelPen = new Pen(Color.FromArgb(220, 132, 34, 166), 1.5f) { DashStyle = DashStyle.Dash })
                        {
                            shihabG.DrawRectangle(sshihabb007SelPen, shihabScaledRect.X - 2, shihabScaledRect.Y - 2, shihabScaledRect.Width + 4, shihabScaledRect.Height + 4);
                        }
                    }
                }
            }
        }

        private static void DrawMehediOverlaysAndAnnotations(
            Graphics shihabG,
            ShihabPdfDocumentModel mehediDoc,
            MehediPdfPageModel sshihabb007Page,
            float shihabScale,
            int mehediSelectedAnnotIdx,
            int sshihabb007SelectedBlockIdx,
            bool shihabUsedWinRtBase)
        {
            if (shihabUsedWinRtBase && sshihabb007Page.ShihabTextBlocks != null)
            {
                for (int mehediIdx = 0; mehediIdx < sshihabb007Page.ShihabTextBlocks.Count; mehediIdx++)
                {
                    ShihabPdfTextBlockItem sshihabb007Block = sshihabb007Page.ShihabTextBlocks[mehediIdx];
                    if (mehediIdx == sshihabb007SelectedBlockIdx)
                    {
                        RectangleF shihabRect = ScaleShihabRect(sshihabb007Block.Sshihabb007BoundsPt, shihabScale);
                        using (SolidBrush mehediTint = new SolidBrush(Color.FromArgb(28, 132, 34, 166)))
                        using (Pen sshihabb007Pen = new Pen(Color.FromArgb(210, 132, 34, 166), 1.5f) { DashStyle = DashStyle.Dash })
                        {
                            shihabG.FillRectangle(mehediTint, shihabRect);
                            shihabG.DrawRectangle(sshihabb007Pen, shihabRect.X, shihabRect.Y, shihabRect.Width, shihabRect.Height);
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(sshihabb007Page.ShihabWatermarkText) && !shihabUsedWinRtBase)
            {
                GraphicsState mehediState = shihabG.Save();
                shihabG.TranslateTransform(sshihabb007Page.MehediWidthPt * shihabScale * 0.5f, sshihabb007Page.Sshihabb007HeightPt * shihabScale * 0.5f);
                shihabG.RotateTransform(sshihabb007Page.ShihabWatermarkAngleDeg);
                using (Font sshihabb007WmFont = new Font("Segoe UI", Math.Max(14f, 38f * shihabScale), FontStyle.Bold))
                using (SolidBrush shihabWmBrush = new SolidBrush(sshihabb007Page.MehediWatermarkColor))
                {
                    SizeF mehediSz = shihabG.MeasureString(sshihabb007Page.ShihabWatermarkText, sshihabb007WmFont);
                    shihabG.DrawString(sshihabb007Page.ShihabWatermarkText, sshihabb007WmFont, shihabWmBrush, -mehediSz.Width * 0.5f, -mehediSz.Height * 0.5f);
                }
                shihabG.Restore(mehediState);
            }

            if (sshihabb007Page.MehediAnnotations != null)
            {
                for (int shihabIdx = 0; shihabIdx < sshihabb007Page.MehediAnnotations.Count; shihabIdx++)
                {
                    ShihabPdfAnnotationItem mehediAnnot = sshihabb007Page.MehediAnnotations[shihabIdx];
                    RectangleF sshihabb007Rect = ScaleShihabRect(mehediAnnot.ShihabBoundsPt, shihabScale);

                    switch (mehediAnnot.Sshihabb007Kind)
                    {
                        case MehediAnnotationKind.MehediHighlight:
                            using (SolidBrush shihabHlBrush = new SolidBrush(Color.FromArgb(95, 255, 235, 59)))
                            {
                                shihabG.FillRectangle(shihabHlBrush, sshihabb007Rect);
                            }
                            break;

                        case MehediAnnotationKind.Sshihabb007Underline:
                            using (Pen mehediUlPen = new Pen(mehediAnnot.Sshihabb007StrokeColor, Math.Max(1.5f, 2f * shihabScale)))
                            {
                                shihabG.DrawLine(mehediUlPen, sshihabb007Rect.Left, sshihabb007Rect.Bottom - 2, sshihabb007Rect.Right, sshihabb007Rect.Bottom - 2);
                            }
                            break;

                        case MehediAnnotationKind.ShihabStrikeout:
                            using (Pen sshihabb007StPen = new Pen(mehediAnnot.Sshihabb007StrokeColor, Math.Max(1.5f, 2f * shihabScale)))
                            {
                                float shihabMidY = sshihabb007Rect.Top + (sshihabb007Rect.Height * 0.5f);
                                shihabG.DrawLine(sshihabb007StPen, sshihabb007Rect.Left, shihabMidY, sshihabb007Rect.Right, shihabMidY);
                            }
                            break;

                        case MehediAnnotationKind.MehediRedaction:
                            if (mehediAnnot.Sshihabb007IsRedactionBurned)
                            {
                                using (SolidBrush mehediBlk = new SolidBrush(Color.Black))
                                using (Font sshihabb007RedFont = new Font("Consolas", Math.Max(6.5f, 8f * shihabScale), FontStyle.Bold))
                                {
                                    shihabG.FillRectangle(mehediBlk, sshihabb007Rect);
                                    shihabG.DrawString("[REDACTED — SHIHAB PDF PRO]", sshihabb007RedFont, Brushes.White, sshihabb007Rect.Left + 4, sshihabb007Rect.Top + 3);
                                }
                            }
                            else
                            {
                                using (SolidBrush shihabMarkFill = new SolidBrush(Color.FromArgb(65, 239, 68, 68)))
                                using (Pen mehediMarkPen = new Pen(Color.FromArgb(230, 239, 68, 68), 1.8f))
                                {
                                    shihabG.FillRectangle(shihabMarkFill, sshihabb007Rect);
                                    shihabG.DrawRectangle(mehediMarkPen, sshihabb007Rect.X, sshihabb007Rect.Y, sshihabb007Rect.Width, sshihabb007Rect.Height);
                                }
                            }
                            break;

                        case MehediAnnotationKind.Sshihabb007FreehandInk:
                            if (mehediAnnot.MehediInkPointsPt != null && mehediAnnot.MehediInkPointsPt.Count > 1)
                            {
                                PointF[] sshihabb007Pts = new PointF[mehediAnnot.MehediInkPointsPt.Count];
                                for (int shihabP = 0; shihabP < mehediAnnot.MehediInkPointsPt.Count; shihabP++)
                                {
                                    sshihabb007Pts[shihabP] = new PointF(
                                        mehediAnnot.MehediInkPointsPt[shihabP].X * shihabScale,
                                        mehediAnnot.MehediInkPointsPt[shihabP].Y * shihabScale);
                                }
                                using (Pen mehediInkPen = new Pen(mehediAnnot.Sshihabb007StrokeColor, Math.Max(1.5f, mehediAnnot.MehediStrokeWidth * shihabScale)))
                                {
                                    mehediInkPen.StartCap = LineCap.Round;
                                    mehediInkPen.EndCap = LineCap.Round;
                                    mehediInkPen.LineJoin = LineJoin.Round;
                                    shihabG.DrawLines(mehediInkPen, sshihabb007Pts);
                                }
                            }
                            break;

                        case MehediAnnotationKind.MehediEllipse:
                            using (SolidBrush sshihabb007ElFill = new SolidBrush(mehediAnnot.ShihabFillColor))
                            using (Pen shihabElPen = new Pen(mehediAnnot.Sshihabb007StrokeColor, Math.Max(1.2f, mehediAnnot.MehediStrokeWidth * shihabScale)))
                            {
                                shihabG.FillEllipse(sshihabb007ElFill, sshihabb007Rect);
                                shihabG.DrawEllipse(shihabElPen, sshihabb007Rect);
                            }
                            break;

                        case MehediAnnotationKind.Sshihabb007Arrow:
                            using (Pen mehediArrPen = new Pen(mehediAnnot.Sshihabb007StrokeColor, Math.Max(1.8f, mehediAnnot.MehediStrokeWidth * shihabScale)))
                            {
                                mehediArrPen.CustomEndCap = new AdjustableArrowCap(5f, 5f, true);
                                shihabG.DrawLine(mehediArrPen, sshihabb007Rect.Left, sshihabb007Rect.Top, sshihabb007Rect.Right, sshihabb007Rect.Bottom);
                            }
                            break;

                        case MehediAnnotationKind.MehediImageOverlay:
                            if (mehediAnnot.Sshihabb007EmbeddedImageBytes != null && mehediAnnot.Sshihabb007EmbeddedImageBytes.Length > 0)
                            {
                                try
                                {
                                    using (MemoryStream sshihabb007ImgMs = new MemoryStream(mehediAnnot.Sshihabb007EmbeddedImageBytes))
                                    using (Image shihabImg = Image.FromStream(sshihabb007ImgMs))
                                    {
                                        shihabG.DrawImage(shihabImg, sshihabb007Rect);
                                    }
                                }
                                catch { }
                            }
                            break;

                        default:
                            using (SolidBrush mehediBoxFill = new SolidBrush(mehediAnnot.ShihabFillColor))
                            using (Pen sshihabb007BoxPen = new Pen(mehediAnnot.Sshihabb007StrokeColor, Math.Max(1.2f, mehediAnnot.MehediStrokeWidth * shihabScale)))
                            {
                                shihabG.FillRectangle(mehediBoxFill, sshihabb007Rect);
                                shihabG.DrawRectangle(sshihabb007BoxPen, sshihabb007Rect.X, sshihabb007Rect.Y, sshihabb007Rect.Width, sshihabb007Rect.Height);
                            }
                            if (!string.IsNullOrWhiteSpace(mehediAnnot.Sshihabb007TextContent))
                            {
                                using (Font shihabTxtFont = new Font(mehediAnnot.ShihabFontFamily ?? "Segoe UI", Math.Max(7f, mehediAnnot.Sshihabb007FontSizePt * shihabScale * 0.85f), FontStyle.Bold))
                                using (SolidBrush mehediTxtBrush = new SolidBrush(mehediAnnot.Sshihabb007StrokeColor))
                                {
                                    RectangleF sshihabb007PadRect = new RectangleF(sshihabb007Rect.X + 5, sshihabb007Rect.Y + 4, Math.Max(20, sshihabb007Rect.Width - 10), Math.Max(14, sshihabb007Rect.Height - 8));
                                    shihabG.DrawString(mehediAnnot.Sshihabb007TextContent, shihabTxtFont, mehediTxtBrush, sshihabb007PadRect);
                                }
                            }
                            break;
                    }

                    if (shihabIdx == mehediSelectedAnnotIdx)
                    {
                        using (Pen shihabSelPen = new Pen(Color.FromArgb(255, 132, 34, 166), 2.0f) { DashStyle = DashStyle.Dot })
                        {
                            shihabG.DrawRectangle(shihabSelPen, sshihabb007Rect.X - 3, sshihabb007Rect.Y - 3, sshihabb007Rect.Width + 6, sshihabb007Rect.Height + 6);
                        }
                    }
                }
            }

            if (sshihabb007Page.Sshihabb007FormFields != null)
            {
                foreach (ShihabAcroFormFieldItem mehediFld in sshihabb007Page.Sshihabb007FormFields)
                {
                    RectangleF sshihabb007FldRect = ScaleShihabRect(mehediFld.Sshihabb007BoundsPt, shihabScale);
                    using (SolidBrush shihabFldBg = new SolidBrush(Color.FromArgb(215, 236, 242, 255)))
                    using (Pen mehediFldPen = new Pen(Color.FromArgb(220, 132, 34, 166), 1.4f))
                    using (Font sshihabb007FldFont = new Font("Segoe UI", Math.Max(7f, 9.2f * shihabScale), FontStyle.Regular))
                    using (SolidBrush shihabFldText = new SolidBrush(Color.FromArgb(255, 30, 41, 59)))
                    {
                        shihabG.FillRectangle(shihabFldBg, sshihabb007FldRect);
                        shihabG.DrawRectangle(mehediFldPen, sshihabb007FldRect.X, sshihabb007FldRect.Y, sshihabb007FldRect.Width, sshihabb007FldRect.Height);

                        string mehediLabel = mehediFld.Sshihabb007FieldType == "CheckBox"
                            ? (mehediFld.ShihabIsChecked ? "☑ " : "☐ ") + mehediFld.ShihabFieldName
                            : mehediFld.ShihabFieldName + ": " + mehediFld.Sshihabb007FieldValue;

                        shihabG.DrawString(mehediLabel, sshihabb007FldFont, shihabFldText, new RectangleF(sshihabb007FldRect.X + 5, sshihabb007FldRect.Y + 4, sshihabb007FldRect.Width - 10, sshihabb007FldRect.Height - 6));
                    }
                }
            }

            if (mehediDoc.Sshihabb007Signatures != null)
            {
                foreach (ShihabDigitalSignatureRecord sshihabb007Sig in mehediDoc.Sshihabb007Signatures)
                {
                    if (sshihabb007Sig.ShihabPageIndex != sshihabb007Page.ShihabPageIndex) continue;
                    RectangleF shihabSigRect = ScaleShihabRect(sshihabb007Sig.MehediVisualRectPt, shihabScale);
                    using (SolidBrush mehediSigBg = new SolidBrush(Color.FromArgb(235, 240, 253, 244)))
                    using (Pen sshihabb007SigBorder = new Pen(Color.FromArgb(255, 16, 185, 129), 1.8f))
                    using (Font shihabTitleFont = new Font("Segoe UI", Math.Max(7f, 8.8f * shihabScale), FontStyle.Bold))
                    using (Font mehediSubFont = new Font("Consolas", Math.Max(6f, 7.2f * shihabScale), FontStyle.Regular))
                    {
                        shihabG.FillRectangle(mehediSigBg, shihabSigRect);
                        shihabG.DrawRectangle(sshihabb007SigBorder, shihabSigRect.X, shihabSigRect.Y, shihabSigRect.Width, shihabSigRect.Height);
                        shihabG.DrawString("✔ DIGITALLY SIGNED — " + sshihabb007Sig.MehediSignerCommonName, shihabTitleFont, Brushes.DarkGreen, shihabSigRect.X + 6, shihabSigRect.Y + 5);
                        shihabG.DrawString("Reason: " + sshihabb007Sig.ShihabSignatureReason, mehediSubFont, Brushes.DarkSlateGray, shihabSigRect.X + 6, shihabSigRect.Y + (22f * shihabScale));
                        shihabG.DrawString("UTC: " + sshihabb007Sig.Sshihabb007SignTimestampUtc.ToString("yyyy-MM-dd HH:mm:ss") + " | Cert: " + sshihabb007Sig.ShihabCertificateThumbprint.Substring(0, 12), mehediSubFont, Brushes.DimGray, shihabSigRect.X + 6, shihabSigRect.Y + (38f * shihabScale));
                    }
                }
            }
        }

        private static RectangleF ScaleShihabRect(RectangleF shihabPt, float mehediScale)
        {
            return new RectangleF(
                shihabPt.Left * mehediScale,
                shihabPt.Top * mehediScale,
                shihabPt.Width * mehediScale,
                shihabPt.Height * mehediScale);
        }

        // =====================================================================
        // WINDOWS 10/11 NEURAL OCR ENGINE (Windows.Media.Ocr.OcrEngine via Task.Run)
        // =====================================================================
        public static string RunMehediWinRtOcrOnPage(ShihabPdfDocumentModel shihabDoc, int mehediPageIdx, out int sshihabb007RecognizedWordCount)
        {
            sshihabb007RecognizedWordCount = 0;
            if (shihabDoc == null || mehediPageIdx < 0 || mehediPageIdx >= shihabDoc.Sshihabb007Pages.Count)
            {
                return string.Empty;
            }

            MehediPdfPageModel shihabPage = shihabDoc.Sshihabb007Pages[mehediPageIdx];
            using (Bitmap mehediHighResBmp = RenderShihabCompositePage(shihabDoc, mehediPageIdx, 2.0f, -1, -1))
            {
                List<ShihabPdfTextBlockItem> sshihabb007WordBlocks;
                string shihabOcrText = RunShihabNeuralOcrOnBitmap(mehediHighResBmp, mehediPageIdx, 2.0f, out sshihabb007WordBlocks, out sshihabb007RecognizedWordCount);

                if (!string.IsNullOrWhiteSpace(shihabOcrText))
                {
                    shihabPage.ShihabExtractedText = shihabOcrText;
                    shihabPage.ShihabHasOcrLayerApplied = true;
                    if (sshihabb007WordBlocks != null && sshihabb007WordBlocks.Count > 0)
                    {
                        shihabPage.ShihabTextBlocks = sshihabb007WordBlocks;
                    }
                    else
                    {
                        MehediPdfCoreEngine.PopulateShihabEditableBlocksFromExtractedText(shihabPage);
                    }
                    shihabDoc.ShihabIsModifiedDirty = true;
                }
                return shihabOcrText;
            }
        }

        public static string RunShihabNeuralOcrOnBitmap(
            Bitmap shihabBitmap,
            int mehediPageIdx,
            float sshihabb007RenderScale,
            out List<ShihabPdfTextBlockItem> shihabExtractedLineBlocks,
            out int mehediTotalWords)
        {
            List<ShihabPdfTextBlockItem> sshihabb007LocalBlocks = new List<ShihabPdfTextBlockItem>();
            int shihabLocalWords = 0;

            string mehediFullText = string.Empty;
            try
            {
                byte[] sshihabb007PngBytes;
                using (MemoryStream shihabMs = new MemoryStream())
                {
                    shihabBitmap.Save(shihabMs, ImageFormat.Png);
                    sshihabb007PngBytes = shihabMs.ToArray();
                }

                // Run on MTA thread pool worker via Task.Run so it never deadlocks the STA UI thread!
                mehediFullText = Task.Run(() =>
                {
                    OcrEngine shihabOcrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                    if (shihabOcrEngine == null) return string.Empty;

                    using (InMemoryRandomAccessStream mehediRandomStream = new InMemoryRandomAccessStream())
                    {
                        WriteSshihabb007BytesToWinRtStream(mehediRandomStream, sshihabb007PngBytes);

                        BitmapDecoder sshihabb007Decoder = WaitShihabWinRtAsyncOp(BitmapDecoder.CreateAsync(mehediRandomStream));
                        using (SoftwareBitmap shihabSoftBmp = WaitShihabWinRtAsyncOp(sshihabb007Decoder.GetSoftwareBitmapAsync()))
                        {
                            OcrResult mehediOcrResult = WaitShihabWinRtAsyncOp(shihabOcrEngine.RecognizeAsync(shihabSoftBmp));
                            if (mehediOcrResult == null) return string.Empty;

                            List<string> sshihabb007LinesText = new List<string>();
                            foreach (OcrLine shihabLine in mehediOcrResult.Lines)
                            {
                                sshihabb007LinesText.Add(shihabLine.Text);
                                float mehediMinX = float.MaxValue;
                                float sshihabb007MinY = float.MaxValue;
                                float shihabMaxRight = 0f;
                                float mehediMaxBottom = 0f;

                                foreach (OcrWord sshihabb007Word in shihabLine.Words)
                                {
                                    shihabLocalWords++;
                                    Windows.Foundation.Rect shihabWr = sshihabb007Word.BoundingRect;
                                    if ((float)shihabWr.X < mehediMinX) mehediMinX = (float)shihabWr.X;
                                    if ((float)shihabWr.Y < sshihabb007MinY) sshihabb007MinY = (float)shihabWr.Y;
                                    if ((float)(shihabWr.X + shihabWr.Width) > shihabMaxRight) shihabMaxRight = (float)(shihabWr.X + shihabWr.Width);
                                    if ((float)(shihabWr.Y + shihabWr.Height) > mehediMaxBottom) mehediMaxBottom = (float)(shihabWr.Y + shihabWr.Height);
                                }

                                if (mehediMinX < float.MaxValue)
                                {
                                    RectangleF sshihabb007BoundsPt = new RectangleF(
                                        mehediMinX / sshihabb007RenderScale,
                                        sshihabb007MinY / sshihabb007RenderScale,
                                        Math.Max(24f, (shihabMaxRight - mehediMinX) / sshihabb007RenderScale),
                                        Math.Max(12f, (mehediMaxBottom - sshihabb007MinY) / sshihabb007RenderScale));

                                    sshihabb007LocalBlocks.Add(new ShihabPdfTextBlockItem
                                    {
                                        MehediPageIndex = mehediPageIdx,
                                        ShihabText = shihabLine.Text,
                                        MehediOriginalText = shihabLine.Text,
                                        Sshihabb007BoundsPt = sshihabb007BoundsPt,
                                        ShihabFontSizePt = Math.Max(9f, Math.Min(18f, sshihabb007BoundsPt.Height * 0.72f)),
                                        Sshihabb007IsOcrDerived = true
                                    });
                                }
                            }
                            return string.Join(Environment.NewLine, sshihabb007LinesText);
                        }
                    }
                }).GetAwaiter().GetResult();
            }
            catch
            {
                mehediFullText = string.Empty;
            }

            shihabExtractedLineBlocks = sshihabb007LocalBlocks;
            mehediTotalWords = shihabLocalWords;
            return mehediFullText;
        }
    }
}
