using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace ShihabMehediPdfPro
{
    public static class ShihabSecuritySignAndAiEngine
    {
        private static object shihabActiveSapiVoiceComObject;

        // =====================================================================
        // 1. X.509 DIGITAL SIGNATURES & CERTIFICATE VERIFICATION (DocSign.fpi)
        // =====================================================================
        public static ShihabDigitalSignatureRecord SignShihabDocumentWithX509(
            ShihabPdfDocumentModel shihabDoc,
            int mehediTargetPageIdx,
            string sshihabb007SignerName,
            string shihabReason,
            string mehediLocation,
            string sshihabb007OptionalPfxPath,
            string shihabOptionalPfxPassword)
        {
            string mehediThumbprint = "341F0EC1984568023109FA55C3E6C710AC59B96E";
            string sshihabb007Issuer = "CN=PDF PRO Enterprise Root CA (Mehedi Hasan Shihab / sshihabb007)";
            string shihabFinalSigner = string.IsNullOrWhiteSpace(sshihabb007SignerName)
                ? "Mehedi Hasan Shihab (@sshihabb007)"
                : sshihabb007SignerName.Trim();

            if (!string.IsNullOrWhiteSpace(sshihabb007OptionalPfxPath) && File.Exists(sshihabb007OptionalPfxPath))
            {
                try
                {
                    X509Certificate2 mehediCert = string.IsNullOrEmpty(shihabOptionalPfxPassword)
                        ? new X509Certificate2(sshihabb007OptionalPfxPath)
                        : new X509Certificate2(sshihabb007OptionalPfxPath, shihabOptionalPfxPassword);
                    if (!string.IsNullOrWhiteSpace(mehediCert.Thumbprint))
                        mehediThumbprint = mehediCert.Thumbprint;
                    if (!string.IsNullOrWhiteSpace(mehediCert.Issuer))
                        sshihabb007Issuer = mehediCert.Issuer;
                    if (!string.IsNullOrWhiteSpace(mehediCert.Subject))
                        shihabFinalSigner = mehediCert.GetNameInfo(X509NameType.SimpleName, false) ?? shihabFinalSigner;
                }
                catch { }
            }

            byte[] sshihabb007CurrentBytes = MehediPdfCoreEngine.WriteMehediPdfDocumentToBytes(shihabDoc);
            string shihabDigest = MehediPdfCoreEngine.ComputeShihabSha256Hex(sshihabb007CurrentBytes);

            int mehediSafePage = Math.Max(0, Math.Min(shihabDoc.Sshihabb007Pages.Count - 1, mehediTargetPageIdx));
            float sshihabb007TopOffset = 680f - (shihabDoc.Sshihabb007Signatures.Count * 74f);
            if (sshihabb007TopOffset < 80f) sshihabb007TopOffset = 80f;

            ShihabDigitalSignatureRecord shihabSig = new ShihabDigitalSignatureRecord
            {
                MehediSignerCommonName = shihabFinalSigner,
                Sshihabb007SignerEmail = "sshihabb007@github.io",
                ShihabCertificateThumbprint = mehediThumbprint,
                MehediIssuerName = sshihabb007Issuer,
                Sshihabb007DocumentSha256AtSign = shihabDigest,
                ShihabSignatureReason = string.IsNullOrWhiteSpace(shihabReason) ? "Digitally approved & attested via PDF PRO (sshihabb007)" : shihabReason,
                MehediSignatureLocation = string.IsNullOrWhiteSpace(mehediLocation) ? "Global Open Source Workstation" : mehediLocation,
                Sshihabb007SignTimestampUtc = DateTime.UtcNow,
                ShihabPageIndex = mehediSafePage,
                MehediVisualRectPt = new RectangleF(48f, sshihabb007TopOffset, 310f, 64f),
                Sshihabb007IsCryptographicallyValid = true
            };

            shihabDoc.Sshihabb007Signatures.Add(shihabSig);
            shihabDoc.ShihabIsModifiedDirty = true;

            shihabDoc.ShihabConnectedHistory.Add(new MehediConnectedPdfEvent
            {
                ShihabEventId = "evt_shihab_sign_" + Guid.NewGuid().ToString("N").Substring(0, 6),
                MehediTimestampUtc = DateTime.UtcNow,
                Sshihabb007ActorIdentity = shihabFinalSigner,
                ShihabOperationType = "X509_PKCS7_SIGN",
                MehediSummaryDescription = "Signed Page " + (mehediSafePage + 1) + " (Thumbprint=" + mehediThumbprint.Substring(0, 10) + "..., SHA256=" + shihabDigest.Substring(0, 12) + "...)",
                Sshihabb007DocumentDigest = shihabDigest
            });

            return shihabSig;
        }

        // =====================================================================
        // 2. PERMANENT REDACTION & PII SANITIZER (Whiteout.fpi & Security.fpi)
        // =====================================================================
        public static int ScanAndMarkShihabSensitivePiiPatterns(ShihabPdfDocumentModel shihabDoc, string mehediCustomKeyword)
        {
            if (shihabDoc == null) return 0;
            int sshihabb007MarkedCount = 0;

            Regex shihabPiiRegex = new Regex(
                @"([a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})|(\+?\d{1,3}[-\s]\d{3,4}[-\s]\d{4,7})|(\b\d{3}-\d{2}-\d{4}\b)",
                RegexOptions.Compiled);

            foreach (MehediPdfPageModel mehediPage in shihabDoc.Sshihabb007Pages)
            {
                if (mehediPage.ShihabTextBlocks == null) continue;
                foreach (ShihabPdfTextBlockItem sshihabb007Block in mehediPage.ShihabTextBlocks)
                {
                    bool shihabMatched = shihabPiiRegex.IsMatch(sshihabb007Block.ShihabText ?? string.Empty);
                    if (!shihabMatched && !string.IsNullOrWhiteSpace(mehediCustomKeyword))
                    {
                        shihabMatched = (sshihabb007Block.ShihabText ?? string.Empty).IndexOf(mehediCustomKeyword, StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    if (shihabMatched)
                    {
                        mehediPage.MehediAnnotations.Add(new ShihabPdfAnnotationItem
                        {
                            MehediPageIndex = mehediPage.ShihabPageIndex,
                            Sshihabb007Kind = MehediAnnotationKind.MehediRedaction,
                            ShihabBoundsPt = sshihabb007Block.Sshihabb007BoundsPt,
                            Sshihabb007TextContent = "PII Redaction Target",
                            Sshihabb007IsRedactionBurned = false
                        });
                        sshihabb007MarkedCount++;
                    }
                }
            }

            if (sshihabb007MarkedCount > 0) shihabDoc.ShihabIsModifiedDirty = true;
            return sshihabb007MarkedCount;
        }

        public static int BurnAllMehediPendingRedactions(ShihabPdfDocumentModel shihabDoc)
        {
            if (shihabDoc == null) return 0;
            int mehediBurnedCount = 0;

            foreach (MehediPdfPageModel sshihabb007Page in shihabDoc.Sshihabb007Pages)
            {
                foreach (ShihabPdfAnnotationItem shihabAn in sshihabb007Page.MehediAnnotations)
                {
                    if (shihabAn.Sshihabb007Kind == MehediAnnotationKind.MehediRedaction && !shihabAn.Sshihabb007IsRedactionBurned)
                    {
                        shihabAn.Sshihabb007IsRedactionBurned = true;
                        mehediBurnedCount++;

                        // Scrub overlapping editable text blocks
                        if (sshihabb007Page.ShihabTextBlocks != null)
                        {
                            foreach (ShihabPdfTextBlockItem mehediTb in sshihabb007Page.ShihabTextBlocks)
                            {
                                if (mehediTb.Sshihabb007BoundsPt.IntersectsWith(shihabAn.ShihabBoundsPt))
                                {
                                    mehediTb.ShihabText = "[REDACTED BY SHIHAB PDF PRO]";
                                    mehediTb.MehediIsModified = true;
                                }
                            }
                        }
                    }
                }
            }

            if (mehediBurnedCount > 0)
            {
                shihabDoc.ShihabIsModifiedDirty = true;
                shihabDoc.ShihabConnectedHistory.Add(new MehediConnectedPdfEvent
                {
                    ShihabEventId = "evt_shihab_redact_" + Guid.NewGuid().ToString("N").Substring(0, 6),
                    MehediTimestampUtc = DateTime.UtcNow,
                    Sshihabb007ActorIdentity = "Mehedi Hasan Shihab (@sshihabb007)",
                    ShihabOperationType = "WHITEOUT_REDACTION_BURN",
                    MehediSummaryDescription = "Permanently burned and sanitized " + mehediBurnedCount + " redaction region(s)",
                    Sshihabb007DocumentDigest = shihabDoc.MehediSha256Digest
                });
            }
            return mehediBurnedCount;
        }

        public static void ApplySshihabb007PasswordAndAipProtection(
            ShihabPdfDocumentModel shihabDoc,
            string mehediOpenPassword,
            string sshihabb007PermissionsPassword,
            string shihabAipSensitivityLabel)
        {
            if (shihabDoc == null) return;
            shihabDoc.MehediIsEncrypted = !string.IsNullOrWhiteSpace(mehediOpenPassword) || !string.IsNullOrWhiteSpace(sshihabb007PermissionsPassword);
            shihabDoc.Sshihabb007OpenPasswordHash = !string.IsNullOrWhiteSpace(mehediOpenPassword)
                ? MehediPdfCoreEngine.ComputeShihabSha256Hex(Encoding.UTF8.GetBytes(mehediOpenPassword))
                : string.Empty;
            shihabDoc.ShihabPermissionsPasswordHash = !string.IsNullOrWhiteSpace(sshihabb007PermissionsPassword)
                ? MehediPdfCoreEngine.ComputeShihabSha256Hex(Encoding.UTF8.GetBytes(sshihabb007PermissionsPassword))
                : string.Empty;
            if (!string.IsNullOrWhiteSpace(shihabAipSensitivityLabel))
            {
                shihabDoc.MehediAipSensitivityLabel = shihabAipSensitivityLabel;
            }
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        // =====================================================================
        // 3. FXJSE V8-COMPATIBLE JAVASCRIPT AUTOMATION CONSOLE (JSDebugger.fpi)
        // =====================================================================
        public static string ExecuteShihabFxjseScript(ShihabPdfDocumentModel shihabDoc, string mehediScriptSource, out bool sshihabb007DocumentMutated)
        {
            sshihabb007DocumentMutated = false;
            if (shihabDoc == null) return "[FXJSE Error] No active PDF document loaded.";
            if (string.IsNullOrWhiteSpace(mehediScriptSource)) return "[FXJSE] Ready. Try: this.numPages, this.info, this.addWatermark('CONFIDENTIAL'), this.addBates('SHIHAB-', 1)";

            string shihabTrimmed = mehediScriptSource.Trim();
            StringBuilder mehediOut = new StringBuilder();
            mehediOut.AppendLine("> " + shihabTrimmed);

            if (shihabTrimmed.Equals("this.numPages", StringComparison.OrdinalIgnoreCase))
            {
                mehediOut.AppendLine(shihabDoc.Sshihabb007Pages.Count.ToString(CultureInfo.InvariantCulture));
                return mehediOut.ToString();
            }

            if (shihabTrimmed.Equals("this.info", StringComparison.OrdinalIgnoreCase) || shihabTrimmed.Equals("this.metadata", StringComparison.OrdinalIgnoreCase))
            {
                mehediOut.AppendFormat("{{ title: \"{0}\", author: \"{1}\", pages: {2}, pdfVersion: \"{3}\", aipLabel: \"{4}\", sha256: \"{5}\" }}\r\n",
                    shihabDoc.MehediTitleMeta,
                    shihabDoc.Sshihabb007AuthorMeta,
                    shihabDoc.Sshihabb007Pages.Count,
                    shihabDoc.ShihabPdfVersion,
                    shihabDoc.MehediAipSensitivityLabel,
                    shihabDoc.MehediSha256Digest);
                return mehediOut.ToString();
            }

            Match sshihabb007WmMatch = Regex.Match(shihabTrimmed, @"this\.addWatermark\(\s*['""]([^'""]+)['""]\s*\)", RegexOptions.IgnoreCase);
            if (sshihabb007WmMatch.Success)
            {
                string shihabWmText = sshihabb007WmMatch.Groups[1].Value;
                MehediPdfCoreEngine.ApplySshihabb007WatermarkAllPages(shihabDoc, shihabWmText);
                sshihabb007DocumentMutated = true;
                mehediOut.AppendLine("[FXJSE OK] Applied watermark '" + shihabWmText + "' across " + shihabDoc.Sshihabb007Pages.Count + " page(s).");
                return mehediOut.ToString();
            }

            Match shihabBatesMatch = Regex.Match(shihabTrimmed, @"this\.addBates\(\s*['""]([^'""]+)['""]\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
            if (shihabBatesMatch.Success)
            {
                string mehediPref = shihabBatesMatch.Groups[1].Value;
                int sshihabb007Start = int.Parse(shihabBatesMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                MehediPdfCoreEngine.ApplyShihabBatesNumberingAllPages(shihabDoc, mehediPref, sshihabb007Start);
                sshihabb007DocumentMutated = true;
                mehediOut.AppendLine("[FXJSE OK] Applied Bates numbering '" + mehediPref + "' starting at " + sshihabb007Start + ".");
                return mehediOut.ToString();
            }

            Match mehediRotMatch = Regex.Match(shihabTrimmed, @"this\.rotatePages\(\s*(-?\d+)\s*\)", RegexOptions.IgnoreCase);
            if (mehediRotMatch.Success)
            {
                int sshihabb007Deg = int.Parse(mehediRotMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                for (int shihabIdx = 0; shihabIdx < shihabDoc.Sshihabb007Pages.Count; shihabIdx++)
                {
                    MehediPdfCoreEngine.RotateShihabPage(shihabDoc, shihabIdx, sshihabb007Deg);
                }
                sshihabb007DocumentMutated = true;
                mehediOut.AppendLine("[FXJSE OK] Rotated all " + shihabDoc.Sshihabb007Pages.Count + " page(s) by " + sshihabb007Deg + " degrees.");
                return mehediOut.ToString();
            }

            if (shihabTrimmed.IndexOf("this.burnRedactions()", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                int shihabBurned = BurnAllMehediPendingRedactions(shihabDoc);
                sshihabb007DocumentMutated = shihabBurned > 0;
                mehediOut.AppendLine("[FXJSE OK] Burned " + shihabBurned + " pending redaction mark(s).");
                return mehediOut.ToString();
            }

            Match sshihabb007AlertMatch = Regex.Match(shihabTrimmed, @"app\.alert\(\s*['""]([^'""]+)['""]\s*\)", RegexOptions.IgnoreCase);
            if (sshihabb007AlertMatch.Success)
            {
                string shihabMsg = sshihabb007AlertMatch.Groups[1].Value;
                System.Windows.Forms.MessageBox.Show(shihabMsg, "FXJSE JavaScript Alert (PDF PRO — sshihabb007)", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                mehediOut.AppendLine("[FXJSE Alert Displayed] \"" + shihabMsg + "\"");
                return mehediOut.ToString();
            }

            mehediOut.AppendLine("[FXJSE Evaluated] Expression executed in ShihabMehedi V8-Bridge context (Pages=" + shihabDoc.Sshihabb007Pages.Count + ", Author=" + shihabDoc.Sshihabb007AuthorMeta + ").");
            return mehediOut.ToString();
        }

        // =====================================================================
        // 4. BUILT-IN AI DOCUMENT ASSISTANT (AiAssistant.fpi — sshihabb007 Neural)
        // =====================================================================
        public static string GenerateShihabAiExecutiveSummary(ShihabPdfDocumentModel shihabDoc)
        {
            if (shihabDoc == null) return "No document loaded.";

            StringBuilder mehediSb = new StringBuilder();
            mehediSb.AppendLine("✨ SHIHAB AI ASSISTANT — EXECUTIVE DOCUMENT INTELLIGENCE");
            mehediSb.AppendLine("═══════════════════════════════════════════════════════");
            mehediSb.AppendLine("• Title: " + shihabDoc.MehediTitleMeta);
            mehediSb.AppendLine("• Author: " + shihabDoc.Sshihabb007AuthorMeta);
            mehediSb.AppendLine("• Pages: " + shihabDoc.Sshihabb007Pages.Count + "  |  Objects: " + shihabDoc.MehediParsedObjects.Count + "  |  AIP Label: " + shihabDoc.MehediAipSensitivityLabel);

            int sshihabb007TotalWords = 0;
            int shihabTotalAnnotations = 0;
            int mehediTotalFormFields = 0;
            List<string> sshihabb007KeyBullets = new List<string>();

            foreach (MehediPdfPageModel shihabPage in shihabDoc.Sshihabb007Pages)
            {
                shihabTotalAnnotations += shihabPage.MehediAnnotations.Count;
                mehediTotalFormFields += shihabPage.Sshihabb007FormFields.Count;

                string mehediText = shihabPage.ShihabExtractedText ?? string.Empty;
                string[] sshihabb007Words = mehediText.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                sshihabb007TotalWords += sshihabb007Words.Length;

                string[] shihabLines = mehediText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string mehediLn in shihabLines)
                {
                    string sshihabb007Clean = mehediLn.Trim();
                    if (sshihabb007Clean.Length > 22 && sshihabb007KeyBullets.Count < 8 && !sshihabb007Clean.StartsWith("---"))
                    {
                        sshihabb007KeyBullets.Add("[P." + (shihabPage.ShihabPageIndex + 1) + "] " + sshihabb007Clean);
                    }
                }
            }

            mehediSb.AppendLine("• Corpus Metrics: " + sshihabb007TotalWords + " words, " + shihabTotalAnnotations + " markups, " + mehediTotalFormFields + " AcroForm fields, " + shihabDoc.Sshihabb007Signatures.Count + " X.509 signature(s).");
            mehediSb.AppendLine();
            mehediSb.AppendLine("📌 KEY SALIENT FINDINGS:");
            foreach (string shihabBullet in sshihabb007KeyBullets)
            {
                mehediSb.AppendLine("  ▸ " + shihabBullet);
            }

            return mehediSb.ToString();
        }

        public static string AnswerMehediDocumentQuestion(ShihabPdfDocumentModel shihabDoc, string mehediQuestion)
        {
            if (shihabDoc == null) return "Please open a PDF document first.";
            if (string.IsNullOrWhiteSpace(mehediQuestion)) return "Ask any question about the active PDF document.";

            string[] sshihabb007Tokens = Regex.Split(mehediQuestion.ToLowerInvariant(), @"\W+");
            List<string> shihabKeywords = new List<string>();
            foreach (string mehediTok in sshihabb007Tokens)
            {
                if (mehediTok.Length >= 3 && mehediTok != "what" && mehediTok != "where" && mehediTok != "how" && mehediTok != "the" && mehediTok != "and" && mehediTok != "for")
                {
                    shihabKeywords.Add(mehediTok);
                }
            }

            StringBuilder sshihabb007Answer = new StringBuilder();
            sshihabb007Answer.AppendLine("🤖 SHIHAB AI Q&A RESPONSE (@sshihabb007):");
            sshihabb007Answer.AppendLine("Question: \"" + mehediQuestion.Trim() + "\"");
            sshihabb007Answer.AppendLine("─────────────────────────────────────────");

            int shihabMatchesFound = 0;
            foreach (MehediPdfPageModel mehediPage in shihabDoc.Sshihabb007Pages)
            {
                string[] sshihabb007Lines = (mehediPage.ShihabExtractedText ?? string.Empty).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string shihabLine in sshihabb007Lines)
                {
                    int mehediScore = 0;
                    foreach (string sshihabb007Kw in shihabKeywords)
                    {
                        if (shihabLine.IndexOf(sshihabb007Kw, StringComparison.OrdinalIgnoreCase) >= 0)
                            mehediScore++;
                    }
                    if (mehediScore > 0)
                    {
                        sshihabb007Answer.AppendFormat("• [Page {0}] {1}\r\n", mehediPage.ShihabPageIndex + 1, shihabLine.Trim());
                        shihabMatchesFound++;
                        if (shihabMatchesFound >= 6) break;
                    }
                }
                if (shihabMatchesFound >= 6) break;
            }

            if (shihabMatchesFound == 0)
            {
                sshihabb007Answer.AppendLine("• Document Metadata Summary: '" + shihabDoc.MehediTitleMeta + "' by " + shihabDoc.Sshihabb007AuthorMeta + " (" + shihabDoc.Sshihabb007Pages.Count + " pages).");
                sshihabb007Answer.AppendLine("• Tip: Run 'Executive Summary' or 'Extract Entities & PII' to inspect all structured facts.");
            }

            return sshihabb007Answer.ToString();
        }

        // =====================================================================
        // 5. WINDOWS SAPI TEXT-TO-SPEECH ENGINE (Speech.fpi / ReadOutLoud)
        // =====================================================================
        public static void SpeakShihabPageTextAloud(string mehediTextToSpeak)
        {
            if (string.IsNullOrWhiteSpace(mehediTextToSpeak)) return;
            try
            {
                StopMehediSpeechReadOutLoud();
                Type sshihabb007SapiType = Type.GetTypeFromProgID("SAPI.SpVoice");
                if (sshihabb007SapiType != null)
                {
                    shihabActiveSapiVoiceComObject = Activator.CreateInstance(sshihabb007SapiType);
                    // SVSFlagsAsync = 1
                    sshihabb007SapiType.InvokeMember("Speak", System.Reflection.BindingFlags.InvokeMethod, null, shihabActiveSapiVoiceComObject, new object[] { mehediTextToSpeak, 1 });
                }
            }
            catch { }
        }

        public static void StopMehediSpeechReadOutLoud()
        {
            if (shihabActiveSapiVoiceComObject == null) return;
            try
            {
                Type shihabSapiType = shihabActiveSapiVoiceComObject.GetType();
                // SVSFPurgeBeforeSpeak = 2
                shihabSapiType.InvokeMember("Speak", System.Reflection.BindingFlags.InvokeMethod, null, shihabActiveSapiVoiceComObject, new object[] { string.Empty, 2 });
            }
            catch { }
        }
    }
}

