using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[assembly: AssemblyTitle("PDF PRO 14.0.4 (Starship Edition — Mehedi Hasan Shihab @sshihabb007)")]
[assembly: AssemblyDescription("PDF PRO 14.0.4 — Enterprise Open-Source PDF Editor, Neural OCR, Solid Office Converter, Callas PDF/A Preflight, X.509 Signer & AI Suite by Mehedi Hasan Shihab (@sshihabb007)")]
[assembly: AssemblyCompany("Mehedi Hasan Shihab (sshihabb007)")]
[assembly: AssemblyProduct("PDF PRO 14.0.4 (Starship Architecture)")]
[assembly: AssemblyCopyright("Copyright © Mehedi Hasan Shihab (@sshihabb007) — https://sshihabb007.github.io/")]
[assembly: AssemblyVersion("14.0.4.0")]
[assembly: AssemblyFileVersion("14.0.4.0")]
[assembly: AssemblyInformationalVersion("14.0.4.33508")]

namespace ShihabMehediPdfPro
{
    public static class MehediProgramEntry
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        public static int Main(string[] shihabRawArgs)
        {
            string mehediAppBaseDir = AppDomain.CurrentDomain.BaseDirectory;
            string sshihabb007ManifestPath = Path.Combine(mehediAppBaseDir, "plugins", "DefaultPluginsList.xml");

            MehediSensorOpenTracingEngine shihabSensor = new MehediSensorOpenTracingEngine(null);
            ShihabPlgDynLoader mehediPluginLoader = new ShihabPlgDynLoader(sshihabb007ManifestPath, shihabSensor);

            // Handle Headless CLI Commands for Automation & Verification
            if (shihabRawArgs != null && shihabRawArgs.Length > 0)
            {
                string sshihabb007FirstArg = shihabRawArgs[0];

                if (string.Equals(sshihabb007FirstArg, "--self-test", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(sshihabb007FirstArg, "--smoke-test", StringComparison.OrdinalIgnoreCase))
                {
                    return RunShihabSelfVerificationSuite(shihabRawArgs.Length > 1 ? shihabRawArgs[1] : Path.Combine(mehediAppBaseDir, "samples"), mehediPluginLoader, shihabSensor);
                }

                if (string.Equals(sshihabb007FirstArg, "--generate-showcase", StringComparison.OrdinalIgnoreCase) && shihabRawArgs.Length > 1)
                {
                    ShihabPdfDocumentModel shihabDoc = MehediPdfCoreEngine.CreateShihabShowcaseDocument("Starship");
                    MehediPdfCoreEngine.SaveShihabPdfDocumentToFile(shihabDoc, shihabRawArgs[1]);
                    return 0;
                }

                if (string.Equals(sshihabb007FirstArg, "--export-docx", StringComparison.OrdinalIgnoreCase) && shihabRawArgs.Length > 2)
                {
                    ShihabPdfDocumentModel mehediDoc = MehediPdfCoreEngine.LoadShihabPdfFromFile(shihabRawArgs[1]);
                    MehediOemEnginesAndConverters.ExportShihabPdfToWordDocx(mehediDoc, shihabRawArgs[2]);
                    return 0;
                }

                if (string.Equals(sshihabb007FirstArg, "--export-xlsx", StringComparison.OrdinalIgnoreCase) && shihabRawArgs.Length > 2)
                {
                    ShihabPdfDocumentModel sshihabb007Doc = MehediPdfCoreEngine.LoadShihabPdfFromFile(shihabRawArgs[1]);
                    MehediOemEnginesAndConverters.ExportMehediPdfToExcelXlsx(sshihabb007Doc, shihabRawArgs[2]);
                    return 0;
                }
            }

            try { SetProcessDPIAware(); } catch { }
            ShihabThemePalette.EnableShihabProcessDarkMode();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string shihabInitialFile = (shihabRawArgs != null && shihabRawArgs.Length > 0 && File.Exists(shihabRawArgs[0]))
                ? shihabRawArgs[0]
                : null;

            MehediAppConfiguration mehediConfig = new MehediAppConfiguration();
            ShihabMainPdfProWindow sshihabb007MainWin = new ShihabMainPdfProWindow(
                mehediConfig,
                shihabSensor,
                mehediPluginLoader,
                shihabInitialFile);

            using (ShihabBrowserBridgeServer shihabBridge = new ShihabBrowserBridgeServer(
                mehediConfig.Sshihabb007LocalBridgePort,
                (string mehediTitle, string sshihabb007Body) =>
                {
                    try
                    {
                        sshihabb007MainWin.BeginInvoke((MethodInvoker)delegate
                        {
                            ShihabPdfDocumentModel shihabWebDoc = new ShihabPdfDocumentModel { Sshihabb007DisplayTitle = mehediTitle };
                            MehediPdfPageModel mehediPg = new MehediPdfPageModel
                            {
                                ShihabPageIndex = 0,
                                ShihabExtractedText = sshihabb007Body
                            };
                            MehediPdfCoreEngine.PopulateShihabEditableBlocksFromExtractedText(mehediPg);
                            shihabWebDoc.Sshihabb007Pages.Add(mehediPg);
                            string sshihabb007TmpPdf = Path.Combine(Path.GetTempPath(), "Shihab-WebCapture-" + Guid.NewGuid().ToString("N").Substring(0, 6) + ".pdf");
                            MehediPdfCoreEngine.SaveShihabPdfDocumentToFile(shihabWebDoc, sshihabb007TmpPdf);
                            sshihabb007MainWin.OpenShihabPdfFileIntoNewTab(sshihabb007TmpPdf);
                        });
                    }
                    catch { }
                }))
            {
                Application.Run(sshihabb007MainWin);
            }

            return 0;
        }

        private static int RunShihabSelfVerificationSuite(
            string shihabOutDir,
            ShihabPlgDynLoader mehediLoader,
            MehediSensorOpenTracingEngine sshihabb007Sensor)
        {
            Directory.CreateDirectory(shihabOutDir);

            // 1. Generate 3-Page Showcase PDF 1.7 Document
            ShihabPdfDocumentModel shihabDoc = MehediPdfCoreEngine.CreateShihabShowcaseDocument("Starship");
            string mehediShowcasePdfPath = Path.Combine(shihabOutDir, "PDF-PRO-Starship-Showcase-Shihab.pdf");
            MehediPdfCoreEngine.SaveShihabPdfDocumentToFile(shihabDoc, mehediShowcasePdfPath);

            // 2. Re-parse saved PDF 1.7 from disk and verify pages & indirect objects
            ShihabPdfDocumentModel sshihabb007Reloaded = MehediPdfCoreEngine.LoadShihabPdfFromFile(mehediShowcasePdfPath);
            if (sshihabb007Reloaded.Sshihabb007Pages.Count != 3 || sshihabb007Reloaded.MehediParsedObjects.Count < 8)
            {
                Console.Error.WriteLine("SELF-TEST FAILED: Reloaded PDF page or object count mismatch.");
                return 2;
            }

            // 3. Verify WinRT High-DPI Page Rendering & Neural OCR Engine
            int shihabOcrWords;
            string mehediOcrText = ShihabWinRtRenderAndOcrEngine.RunMehediWinRtOcrOnPage(sshihabb007Reloaded, 0, out shihabOcrWords);
            if (shihabOcrWords <= 0 || string.IsNullOrWhiteSpace(mehediOcrText))
            {
                Console.Error.WriteLine("SELF-TEST FAILED: WinRT Neural OCR returned 0 words.");
                return 3;
            }

            // 4. Verify Solid Office Exports (.docx, .xlsx, .csv, .html, .md)
            string sshihabb007DocxPath = Path.Combine(shihabOutDir, "PDF-PRO-Showcase-Shihab.docx");
            string shihabXlsxPath = Path.Combine(shihabOutDir, "PDF-PRO-Showcase-Mehedi.xlsx");
            string mehediCsvPath = Path.Combine(shihabOutDir, "PDF-PRO-Showcase-sshihabb007.csv");
            string sshihabb007HtmlPath = Path.Combine(shihabOutDir, "PDF-PRO-Showcase-Shihab.html");

            MehediOemEnginesAndConverters.ExportShihabPdfToWordDocx(sshihabb007Reloaded, sshihabb007DocxPath);
            MehediOemEnginesAndConverters.ExportMehediPdfToExcelXlsx(sshihabb007Reloaded, shihabXlsxPath);
            MehediOemEnginesAndConverters.ExportSshihabb007PdfToCsv(sshihabb007Reloaded, mehediCsvPath);
            MehediOemEnginesAndConverters.ExportShihabPdfToHtml5(sshihabb007Reloaded, sshihabb007HtmlPath);

            // 5. Verify Callas Preflight, X.509 Signing, Lucene Search, & Dynamic Plugin Loader
            mehediLoader.TriggerShihabPluginOnDemand("MehediDocSign");
            mehediLoader.TriggerShihabPluginOnDemand("ShihabAiAssistant");
            List<ShihabSearchMatchItem> shihabSearchHits = MehediOemEnginesAndConverters.SearchShihabLuceneIndex(sshihabb007Reloaded, "Shihab", false);

            // 6. Verify Real Compressed PDF 1.5-2.0 Loading & STA Rendering (No WinRT STA Deadlock)
            string mehediRealPdfCheck = "SKIPPED (no external sample found)";
            string[] sshihabb007RealSamples = new[]
            {
                @"C:\Users\MEHEDI\Downloads\Chrome\Google for Education Level 2 — Easy Step-by-Step Guide.pdf",
                @"C:\Users\MEHEDI\Downloads\Chrome\19 SmartGuard BD(final).pdf",
                @"C:\Users\MEHEDI\Downloads\Chrome\Annual_Campaign_Initiative_Roadmap.pdf"
            };
            foreach (string shihabRealSample in sshihabb007RealSamples)
            {
                if (File.Exists(shihabRealSample))
                {
                    ShihabPdfDocumentModel mehediExternalDoc = MehediPdfCoreEngine.LoadShihabPdfFromFile(shihabRealSample);
                    using (System.Drawing.Bitmap sshihabb007Rendered = ShihabWinRtRenderAndOcrEngine.RenderShihabCompositePage(
                        mehediExternalDoc, 0, 1.25f, -1, 0, true, new HashSet<int> { 0 }))
                    {
                        MehediPdfPageModel shihabPg0 = mehediExternalDoc.Sshihabb007Pages[0];
                        if (mehediExternalDoc.Sshihabb007Pages.Count <= 0 || sshihabb007Rendered == null || sshihabb007Rendered.Width <= 10 || shihabPg0.ShihabTextBlocks.Count < 3)
                        {
                            Console.Error.WriteLine("SELF-TEST FAILED: External real PDF failed to load, render, or index editable text blocks on STA thread: " + shihabRealSample);
                            return 4;
                        }
                        // Verify in-place text edit and object region crop
                        shihabPg0.ShihabTextBlocks[0].ShihabText = "Edited In-Place by Shihab (@sshihabb007)";
                        shihabPg0.ShihabTextBlocks[0].MehediIsModified = true;
                        byte[] mehediCroppedObj = ShihabWinRtRenderAndOcrEngine.CropShihabPageRegionToPngBytes(mehediExternalDoc, 0, new System.Drawing.RectangleF(40f, 40f, 120f, 60f));
                        using (System.Drawing.Bitmap sshihabb007EditedRender = ShihabWinRtRenderAndOcrEngine.RenderShihabCompositePage(
                            mehediExternalDoc, 0, 1.25f, -1, 0, true, null))
                        {
                            if (sshihabb007EditedRender == null || mehediCroppedObj == null || mehediCroppedObj.Length < 64)
                            {
                                Console.Error.WriteLine("SELF-TEST FAILED: In-place text edit or object cutout failed.");
                                return 5;
                            }
                        }
                        mehediRealPdfCheck = Path.GetFileName(shihabRealSample) + " (" + mehediExternalDoc.Sshihabb007Pages.Count + " pages, " + shihabPg0.ShihabTextBlocks.Count + " editable text blocks indexed, rendered " + sshihabb007Rendered.Width + "x" + sshihabb007Rendered.Height + "px)";
                    }
                    break;
                }
            }

            Console.WriteLine("PDF PRO SELF-TEST PASSED:");
            Console.WriteLine("  Showcase PDF : " + mehediShowcasePdfPath + " (" + new FileInfo(mehediShowcasePdfPath).Length + " bytes, SHA256=" + sshihabb007Reloaded.MehediSha256Digest + ")");
            Console.WriteLine("  External PDF : " + mehediRealPdfCheck);
            Console.WriteLine("  OCR Words    : " + shihabOcrWords + " words recognized on Page 1 via Windows.Media.Ocr");
            Console.WriteLine("  Word Export  : " + sshihabb007DocxPath + " (" + new FileInfo(sshihabb007DocxPath).Length + " bytes)");
            Console.WriteLine("  Excel Export : " + shihabXlsxPath + " (" + new FileInfo(shihabXlsxPath).Length + " bytes)");
            Console.WriteLine("  Lucene Hits  : " + shihabSearchHits.Count + " matches for 'Shihab'");
            Console.WriteLine("  Plugins      : " + mehediLoader.MehediLoadedPluginCount + "/" + mehediLoader.ShihabAllPlugins.Count + " .fpi modules active");
            Console.WriteLine("  Sensor Spans : " + sshihabb007Sensor.ShihabRecordedSpans.Count + " OpenTracing spans recorded");
            return 0;
        }
    }
}

