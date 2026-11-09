using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace ShihabMehediPdfPro
{
    public sealed class ShihabPlgDynLoader
    {
        private readonly List<MehediPluginDescriptor> shihabPlugins = new List<MehediPluginDescriptor>();
        private readonly MehediSensorOpenTracingEngine mehediSensor;

        public IList<MehediPluginDescriptor> ShihabAllPlugins { get { return shihabPlugins; } }

        public int MehediLoadedPluginCount
        {
            get
            {
                int sshihabb007Count = 0;
                foreach (MehediPluginDescriptor shihabPlg in shihabPlugins)
                {
                    if (shihabPlg.MehediIsLoadedInMemory) sshihabb007Count++;
                }
                return sshihabb007Count;
            }
        }

        public ShihabPlgDynLoader(string shihabManifestXmlPath, MehediSensorOpenTracingEngine sshihabb007Sensor)
        {
            this.mehediSensor = sshihabb007Sensor;
            LoadShihabPluginManifest(shihabManifestXmlPath);
        }

        private void LoadShihabPluginManifest(string mehediXmlPath)
        {
            shihabPlugins.Clear();
            if (!string.IsNullOrWhiteSpace(mehediXmlPath) && File.Exists(mehediXmlPath))
            {
                try
                {
                    XmlDocument sshihabb007Xml = new XmlDocument();
                    sshihabb007Xml.Load(mehediXmlPath);
                    XmlNodeList shihabNodes = sshihabb007Xml.SelectNodes("//PluginInfo");
                    if (shihabNodes != null)
                    {
                        foreach (XmlNode mehediNode in shihabNodes)
                        {
                            if (mehediNode.Attributes == null) continue;
                            int sshihabb007Behavior = 9;
                            if (mehediNode.Attributes["LoadBehavior"] != null)
                            {
                                int.TryParse(mehediNode.Attributes["LoadBehavior"].Value, out sshihabb007Behavior);
                            }

                            bool shihabEager = (sshihabb007Behavior == 0 || sshihabb007Behavior == 3);
                            MehediPluginDescriptor mehediDesc = new MehediPluginDescriptor
                            {
                                ShihabInternalName = GetSshihabb007Attr(mehediNode, "InternalName", "ShihabPlugin"),
                                MehediDisplayName = GetSshihabb007Attr(mehediNode, "DisplayName", "Shihab Dynamic Plugin"),
                                Sshihabb007RealFilePath = GetSshihabb007Attr(mehediNode, "RealFilePath", "plugins\\Plugin.fpi"),
                                ShihabLoadBehavior = sshihabb007Behavior,
                                MehediLoadEventBehavior = GetSshihabb007Attr(mehediNode, "LoadEventBehavior", "0"),
                                Sshihabb007Category = GetSshihabb007Attr(mehediNode, "Category", "Core"),
                                ShihabRibbonTab = GetSshihabb007Attr(mehediNode, "RibbonTab", "Home"),
                                MehediAuthor = GetSshihabb007Attr(mehediNode, "Author", "Mehedi Hasan Shihab (@sshihabb007)"),
                                Sshihabb007Version = GetSshihabb007Attr(mehediNode, "Version", "14.0.4.33508"),
                                ShihabDescription = GetSshihabb007Attr(mehediNode, "Description", ""),
                                MehediIsLoadedInMemory = shihabEager,
                                Sshihabb007LoadedAtUtc = shihabEager ? (DateTime?)DateTime.UtcNow : null,
                                ShihabInvocationCount = shihabEager ? 1 : 0
                            };
                            shihabPlugins.Add(mehediDesc);
                        }
                    }
                }
                catch { }
            }

            if (shihabPlugins.Count == 0)
            {
                SeedMehediBuiltInDefaultPlugins();
            }
        }

        private static string GetSshihabb007Attr(XmlNode shihabNode, string mehediAttrName, string sshihabb007Fallback)
        {
            if (shihabNode.Attributes != null && shihabNode.Attributes[mehediAttrName] != null)
            {
                return shihabNode.Attributes[mehediAttrName].Value;
            }
            return sshihabb007Fallback;
        }

        private void SeedMehediBuiltInDefaultPlugins()
        {
            string[,] sshihabb007Seeds = new string[,]
            {
                {"ShihabPlgDynLoader", "Shihab Dynamic Plugin Loader", "plugins\\PlgDynLoader.fpi", "0", "CoreBootstrap", "Home"},
                {"MehediPageEditor", "Mehedi Direct Page & Content Stream Editor", "plugins\\PageEditor.fpi", "9", "CoreEditing", "Edit"},
                {"ShihabAiAssistant", "Shihab AI Document Assistant (sshihabb007 Neural)", "plugins\\AiAssistant.fpi", "9", "ArtificialIntelligence", "AI Assistant"},
                {"MehediOcrRecognition", "Mehedi Neural OCR Engine (WinRT + ABBYY Bridge)", "plugins\\OCRRecognition.fpi", "9", "OcrAndConversion", "Convert"},
                {"ShihabFxExport", "Shihab Solid Office & Multi-Format Exporter", "plugins\\FXExport.fpi", "9", "OcrAndConversion", "Convert"},
                {"MehediPdfaPreflight", "Mehedi Callas PDF/A & ISO Preflight Engine", "plugins\\PDFA.fpi", "9", "StandardsAndCompliance", "Convert"},
                {"MehediDocSign", "Mehedi X.509 PKCS#7 & Ink Signature Studio", "plugins\\DocSign.fpi", "9", "SecurityAndSigning", "Sign"},
                {"MehediSecurityPlg", "Mehedi AES-256 Encryption & Permission Guard", "plugins\\Security.fpi", "9", "SecurityAndSigning", "Protect"},
                {"ShihabWhiteoutRedaction", "Shihab Permanent Redaction & Sanitizer (Whiteout)", "plugins\\Whiteout.fpi", "9", "SecurityAndSigning", "Protect"},
                {"MehediFormDesigner", "Mehedi AcroForm Interactive Field Designer", "plugins\\FormDesigner.fpi", "9", "FormsAndScripting", "Form"},
                {"ShihabJsDebugger", "Shihab FXJSE V8 JavaScript Console & Debugger", "plugins\\JSDebugger.fpi", "9", "FormsAndScripting", "Form"},
                {"MehediComparePdf", "Mehedi Side-by-Side Visual & Text Diff Engine", "plugins\\ComparePDF.fpi", "9", "ReviewAndCollaboration", "View"},
                {"ShihabU3dBrowser", "Shihab ODA 3D (U3D / PRC / CAD) Model Viewport", "plugins\\U3DBrowser.fpi", "3", "MultimediaAnd3D", "View"},
                {"MehediSpeechReadOutLoud", "Mehedi SAPI Text-to-Speech (Read Out Loud)", "plugins\\Speech.fpi", "3", "Accessibility", "Accessibility"}
            };

            for (int shihabIdx = 0; shihabIdx < sshihabb007Seeds.GetLength(0); shihabIdx++)
            {
                int mehediBeh = int.Parse(sshihabb007Seeds[shihabIdx, 3], CultureInfo.InvariantCulture);
                bool sshihabb007Eager = (mehediBeh == 0 || mehediBeh == 3);
                shihabPlugins.Add(new MehediPluginDescriptor
                {
                    ShihabInternalName = sshihabb007Seeds[shihabIdx, 0],
                    MehediDisplayName = sshihabb007Seeds[shihabIdx, 1],
                    Sshihabb007RealFilePath = sshihabb007Seeds[shihabIdx, 2],
                    ShihabLoadBehavior = mehediBeh,
                    Sshihabb007Category = sshihabb007Seeds[shihabIdx, 4],
                    ShihabRibbonTab = sshihabb007Seeds[shihabIdx, 5],
                    MehediAuthor = "Mehedi Hasan Shihab (@sshihabb007)",
                    Sshihabb007Version = "14.0.4.33508",
                    MehediIsLoadedInMemory = sshihabb007Eager,
                    Sshihabb007LoadedAtUtc = sshihabb007Eager ? (DateTime?)DateTime.UtcNow : null,
                    ShihabInvocationCount = sshihabb007Eager ? 1 : 0
                });
            }
        }

        public MehediPluginDescriptor TriggerShihabPluginOnDemand(string mehediInternalName)
        {
            foreach (MehediPluginDescriptor sshihabb007Plg in shihabPlugins)
            {
                if (string.Equals(sshihabb007Plg.ShihabInternalName, mehediInternalName, StringComparison.OrdinalIgnoreCase))
                {
                    bool shihabWasUnloaded = !sshihabb007Plg.MehediIsLoadedInMemory;
                    sshihabb007Plg.MehediIsLoadedInMemory = true;
                    if (sshihabb007Plg.Sshihabb007LoadedAtUtc == null)
                    {
                        sshihabb007Plg.Sshihabb007LoadedAtUtc = DateTime.UtcNow;
                    }
                    sshihabb007Plg.ShihabInvocationCount++;

                    if (mehediSensor != null)
                    {
                        mehediSensor.RecordShihabOpenTracingSpan(
                            shihabWasUnloaded ? "PlgDynLoader.LazyMap:" + sshihabb007Plg.ShihabInternalName : "PlugInMain.Invoke:" + sshihabb007Plg.ShihabInternalName,
                            "plugin=" + sshihabb007Plg.Sshihabb007RealFilePath + ",loadBehavior=" + sshihabb007Plg.ShihabLoadBehavior + ",count=" + sshihabb007Plg.ShihabInvocationCount);
                    }
                    return sshihabb007Plg;
                }
            }
            return null;
        }
    }

    // =====================================================================
    // OPENTRACING SENSOR TELEMETRY ENGINE (Sensor.dll + Sensor.db Equivalent)
    // =====================================================================
    public sealed class MehediSensorOpenTracingEngine
    {
        private readonly string shihabSensorStoragePath;
        private readonly List<ShihabSensorTelemetrySpan> mehediSpans = new List<ShihabSensorTelemetrySpan>();

        public IList<ShihabSensorTelemetrySpan> ShihabRecordedSpans { get { return mehediSpans; } }

        public MehediSensorOpenTracingEngine(string sshihabb007ConfigDir)
        {
            string shihabDir = string.IsNullOrWhiteSpace(sshihabb007ConfigDir)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PDF PRO", "FoxitSensor")
                : Path.Combine(sshihabb007ConfigDir, "FoxitSensor");
            try { Directory.CreateDirectory(shihabDir); } catch { }
            this.shihabSensorStoragePath = Path.Combine(shihabDir, "Sensor_Shihab.jsonl");
            RecordShihabOpenTracingSpan("Application.Startup", "productName=PDF PRO (Starship),productVersion=14.0.4.33508,productType=Business_OpenSource,author=Mehedi Hasan Shihab (@sshihabb007)");
        }

        public ShihabSensorTelemetrySpan RecordShihabOpenTracingSpan(string mehediOperationName, string sshihabb007TagSummary)
        {
            ShihabSensorTelemetrySpan shihabSpan = new ShihabSensorTelemetrySpan
            {
                ShihabRecordId = mehediSpans.Count + 1,
                MehediTraceId = Guid.NewGuid().ToString("N").Substring(0, 16),
                Sshihabb007SpanId = Guid.NewGuid().ToString("N").Substring(0, 16),
                ShihabOperationName = mehediOperationName,
                MehediStartTimestampUtc = DateTime.UtcNow,
                Sshihabb007FinishTimestampUtc = DateTime.UtcNow.AddMilliseconds(2),
                ShihabPriority = 2,
                MehediSerializedJsonTags = string.Format(
                    CultureInfo.InvariantCulture,
                    "{{\"operation_name\":\"{0}\",\"traceId\":\"{1}\",\"productName\":\"PDF PRO\",\"productVersion\":\"14.0.4.33508\",\"author\":\"Mehedi Hasan Shihab (@sshihabb007)\",\"tags\":\"{2}\"}}",
                    mehediOperationName,
                    Guid.NewGuid().ToString("N").Substring(0, 12),
                    (sshihabb007TagSummary ?? string.Empty).Replace("\"", "'"))
            };

            mehediSpans.Add(shihabSpan);
            try
            {
                File.AppendAllText(shihabSensorStoragePath, shihabSpan.MehediSerializedJsonTags + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
            return shihabSpan;
        }
    }

    // =====================================================================
    // LOOPBACK HTTP + NATIVE MESSAGING BROWSER BRIDGE (NativeMessagingEXE.exe)
    // =====================================================================
    public sealed class ShihabBrowserBridgeServer : IDisposable
    {
        private readonly int shihabPort;
        private readonly Action<string, string> mehediOnWebPageReceived;
        private HttpListener sshihabb007Listener;
        private Thread shihabWorkerThread;
        private volatile bool mehediIsRunning;

        public ShihabBrowserBridgeServer(int sshihabb007BridgePort, Action<string, string> shihabCallback)
        {
            this.shihabPort = sshihabb007BridgePort;
            this.mehediOnWebPageReceived = shihabCallback;
            StartMehediLoopbackServer();
        }

        private void StartMehediLoopbackServer()
        {
            try
            {
                sshihabb007Listener = new HttpListener();
                sshihabb007Listener.Prefixes.Add("http://127.0.0.1:" + shihabPort + "/");
                sshihabb007Listener.Start();
                mehediIsRunning = true;
                shihabWorkerThread = new Thread(RunSshihabb007ListenLoop) { IsBackground = true, Name = "ShihabPdfProBridgeThread" };
                shihabWorkerThread.Start();
            }
            catch { }
        }

        private void RunSshihabb007ListenLoop()
        {
            while (mehediIsRunning && sshihabb007Listener != null && sshihabb007Listener.IsListening)
            {
                try
                {
                    HttpListenerContext shihabCtx = sshihabb007Listener.GetContext();
                    ThreadPool.QueueUserWorkItem(_ => HandleMehediHttpRequest(shihabCtx));
                }
                catch
                {
                    if (!mehediIsRunning) break;
                }
            }
        }

        private void HandleMehediHttpRequest(HttpListenerContext shihabCtx)
        {
            try
            {
                shihabCtx.Response.Headers["Access-Control-Allow-Origin"] = "*";
                shihabCtx.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
                shihabCtx.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, OPTIONS";

                if (shihabCtx.Request.HttpMethod == "OPTIONS")
                {
                    shihabCtx.Response.StatusCode = 204;
                    shihabCtx.Response.Close();
                    return;
                }

                string mehediPath = shihabCtx.Request.Url.AbsolutePath.ToLowerInvariant();
                if (mehediPath == "/ping" || mehediPath == "/status")
                {
                    WriteSshihabb007JsonResponse(shihabCtx, "{\"status\":\"ok\",\"app\":\"PDF PRO 14.0.4\",\"author\":\"Mehedi Hasan Shihab (@sshihabb007)\"}");
                    return;
                }

                if (mehediPath == "/convert-web" && shihabCtx.Request.HttpMethod == "POST")
                {
                    string sshihabb007Body;
                    using (StreamReader shihabReader = new StreamReader(shihabCtx.Request.InputStream, Encoding.UTF8))
                    {
                        sshihabb007Body = shihabReader.ReadToEnd();
                    }
                    if (mehediOnWebPageReceived != null)
                    {
                        mehediOnWebPageReceived("WebPage-Capture-Shihab.pdf", sshihabb007Body);
                    }
                    WriteSshihabb007JsonResponse(shihabCtx, "{\"status\":\"converted\",\"engine\":\"ShihabMehediStarship\"}");
                    return;
                }

                WriteSshihabb007JsonResponse(shihabCtx, "{\"status\":\"ready\",\"author\":\"sshihabb007\"}");
            }
            catch { }
        }

        private static void WriteSshihabb007JsonResponse(HttpListenerContext shihabCtx, string mehediJson)
        {
            byte[] sshihabb007Bytes = Encoding.UTF8.GetBytes(mehediJson);
            shihabCtx.Response.ContentType = "application/json; charset=utf-8";
            shihabCtx.Response.ContentLength64 = sshihabb007Bytes.Length;
            shihabCtx.Response.OutputStream.Write(sshihabb007Bytes, 0, sshihabb007Bytes.Length);
            shihabCtx.Response.OutputStream.Close();
        }

        public void Dispose()
        {
            mehediIsRunning = false;
            try { if (sshihabb007Listener != null) sshihabb007Listener.Stop(); } catch { }
        }
    }
}

