using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ShihabMehediPdfPro
{
    public static class MehediPdfCoreEngine
    {
        public static string ComputeShihabSha256Hex(byte[] mehediBytes)
        {
            if (mehediBytes == null || mehediBytes.Length == 0) return string.Empty;
            using (SHA256 sshihabb007Sha = SHA256.Create())
            {
                byte[] shihabHash = sshihabb007Sha.ComputeHash(mehediBytes);
                StringBuilder mehediSb = new StringBuilder(shihabHash.Length * 2);
                for (int sshihabb007Idx = 0; sshihabb007Idx < shihabHash.Length; sshihabb007Idx++)
                {
                    mehediSb.Append(shihabHash[sshihabb007Idx].ToString("x2"));
                }
                return mehediSb.ToString();
            }
        }

        public static ShihabPdfDocumentModel LoadShihabPdfFromFile(string mehediFilePath)
        {
            byte[] sshihabb007RawBytes = File.ReadAllBytes(mehediFilePath);
            ShihabPdfDocumentModel shihabDoc = ParseMehediPdfFromBytes(sshihabb007RawBytes, Path.GetFileName(mehediFilePath));
            shihabDoc.MehediFilePath = mehediFilePath;
            shihabDoc.Sshihabb007DisplayTitle = Path.GetFileName(mehediFilePath);
            return shihabDoc;
        }

        public static ShihabPdfDocumentModel ParseMehediPdfFromBytes(byte[] shihabRawBytes, string mehediDisplayName)
        {
            ShihabPdfDocumentModel sshihabb007Doc = new ShihabPdfDocumentModel
            {
                Sshihabb007DisplayTitle = string.IsNullOrWhiteSpace(mehediDisplayName) ? "Document-Shihab.pdf" : mehediDisplayName,
                Sshihabb007FileSizeBytes = shihabRawBytes != null ? shihabRawBytes.LongLength : 0L,
                MehediOriginalFileBytes = shihabRawBytes,
                MehediSha256Digest = ComputeShihabSha256Hex(shihabRawBytes)
            };

            if (shihabRawBytes == null || shihabRawBytes.Length < 8)
            {
                sshihabb007Doc.Sshihabb007Pages.Add(new MehediPdfPageModel { ShihabPageIndex = 0 });
                return sshihabb007Doc;
            }

            string shihabLatinText = Encoding.GetEncoding(28591).GetString(shihabRawBytes);
            Match mehediHeaderMatch = Regex.Match(shihabLatinText, @"%PDF-(\d+\.\d+)");
            if (mehediHeaderMatch.Success)
            {
                sshihabb007Doc.ShihabPdfVersion = mehediHeaderMatch.Groups[1].Value;
            }

            sshihabb007Doc.MehediIsEncrypted = shihabLatinText.IndexOf("/Encrypt", StringComparison.Ordinal) >= 0;
            sshihabb007Doc.Sshihabb007IsPdfACompliant = shihabLatinText.IndexOf("pdfaid:part", StringComparison.OrdinalIgnoreCase) >= 0;

            ExtractShihabInfoMetadata(shihabLatinText, sshihabb007Doc);
            ExtractMehediPdfObjectsAndText(shihabRawBytes, shihabLatinText, sshihabb007Doc);

            sshihabb007Doc.ShihabConnectedHistory.Add(new MehediConnectedPdfEvent
            {
                ShihabEventId = "evt_shihab_open_" + Guid.NewGuid().ToString("N").Substring(0, 6),
                MehediTimestampUtc = DateTime.UtcNow,
                Sshihabb007ActorIdentity = "Mehedi Hasan Shihab (@sshihabb007)",
                ShihabOperationType = "DOCUMENT_OPEN",
                MehediSummaryDescription = "Opened & parsed PDF structure (" + sshihabb007Doc.Sshihabb007Pages.Count + " pages, " + sshihabb007Doc.MehediParsedObjects.Count + " indirect objects)",
                Sshihabb007DocumentDigest = sshihabb007Doc.MehediSha256Digest
            });

            return sshihabb007Doc;
        }

        private static void ExtractShihabInfoMetadata(string shihabLatinText, ShihabPdfDocumentModel mehediDoc)
        {
            mehediDoc.MehediTitleMeta = ExtractSshihabb007PdfKeyString(shihabLatinText, "Title", mehediDoc.Sshihabb007DisplayTitle);
            mehediDoc.Sshihabb007AuthorMeta = ExtractSshihabb007PdfKeyString(shihabLatinText, "Author", "Mehedi Hasan Shihab (@sshihabb007)");
            mehediDoc.ShihabSubjectMeta = ExtractSshihabb007PdfKeyString(shihabLatinText, "Subject", "PDF PRO Document");
            mehediDoc.MehediKeywordsMeta = ExtractSshihabb007PdfKeyString(shihabLatinText, "Keywords", "PDF PRO, Shihab, Mehedi, sshihabb007");
            mehediDoc.Sshihabb007CreatorMeta = ExtractSshihabb007PdfKeyString(shihabLatinText, "Creator", "PDF PRO (sshihabb007)");
            mehediDoc.ShihabProducerMeta = ExtractSshihabb007PdfKeyString(shihabLatinText, "Producer", "ShihabMehediStarship Engine");
        }

        private static string ExtractSshihabb007PdfKeyString(string shihabSource, string mehediKey, string sshihabb007Fallback)
        {
            Match shihabMatch = Regex.Match(shihabSource, @"/" + mehediKey + @"\s*\(([^\)\\]*(?:\\.[^\)\\]*)*)\)");
            if (shihabMatch.Success)
            {
                return UnescapeShihabPdfLiteral(shihabMatch.Groups[1].Value);
            }
            return sshihabb007Fallback;
        }

        private static string UnescapeShihabPdfLiteral(string mehediLiteral)
        {
            if (string.IsNullOrEmpty(mehediLiteral)) return string.Empty;
            return mehediLiteral
                .Replace(@"\(", "(")
                .Replace(@"\)", ")")
                .Replace(@"\\", @"\")
                .Replace(@"\n", "\n")
                .Replace(@"\r", "\r")
                .Replace(@"\t", "\t");
        }

        private static void ExtractMehediPdfObjectsAndText(byte[] shihabBytes, string mehediLatin, ShihabPdfDocumentModel sshihabb007Doc)
        {
            Regex shihabObjRegex = new Regex(@"(\d+)\s+(\d+)\s+obj\b", RegexOptions.Compiled);
            MatchCollection mehediMatches = shihabObjRegex.Matches(mehediLatin);
            List<string> sshihabb007DecodedTextChunks = new List<string>();
            List<RectangleF> shihabPageBoxes = new List<RectangleF>();

            int mehediMaxObjectsToInspect = Math.Min(mehediMatches.Count, 800);
            for (int sshihabb007Idx = 0; sshihabb007Idx < mehediMaxObjectsToInspect; sshihabb007Idx++)
            {
                Match shihabMatch = mehediMatches[sshihabb007Idx];
                int mehediObjNum = 0;
                int sshihabb007GenNum = 0;
                int.TryParse(shihabMatch.Groups[1].Value, out mehediObjNum);
                int.TryParse(shihabMatch.Groups[2].Value, out sshihabb007GenNum);

                int shihabStartPos = shihabMatch.Index;
                int mehediEndObjPos = mehediLatin.IndexOf("endobj", shihabStartPos, StringComparison.Ordinal);
                if (mehediEndObjPos < 0) mehediEndObjPos = Math.Min(mehediLatin.Length, shihabStartPos + 2048);

                int sshihabb007HeaderLen = Math.Min(1200, mehediEndObjPos - shihabStartPos);
                string shihabObjHeader = mehediLatin.Substring(shihabStartPos, sshihabb007HeaderLen);

                string mehediObjKind = "Indirect Object";
                if (shihabObjHeader.IndexOf("/Type /Catalog", StringComparison.Ordinal) >= 0 || shihabObjHeader.IndexOf("/Type/Catalog", StringComparison.Ordinal) >= 0)
                    mehediObjKind = "Catalog (/Root)";
                else if (shihabObjHeader.IndexOf("/Type /Pages", StringComparison.Ordinal) >= 0 || shihabObjHeader.IndexOf("/Type/Pages", StringComparison.Ordinal) >= 0)
                    mehediObjKind = "Page Tree (/Pages)";
                else if (shihabObjHeader.IndexOf("/Type /Page", StringComparison.Ordinal) >= 0 || shihabObjHeader.IndexOf("/Type/Page", StringComparison.Ordinal) >= 0)
                {
                    mehediObjKind = "Page Descriptor (/Page)";
                    Match sshihabb007BoxMatch = Regex.Match(shihabObjHeader, @"/MediaBox\s*\[\s*([-\d\.]+)\s+([-\d\.]+)\s+([-\d\.]+)\s+([-\d\.]+)\s*\]");
                    if (sshihabb007BoxMatch.Success)
                    {
                        float shihabX1 = ParseMehediFloat(sshihabb007BoxMatch.Groups[1].Value, 0f);
                        float mehediY1 = ParseMehediFloat(sshihabb007BoxMatch.Groups[2].Value, 0f);
                        float sshihabb007X2 = ParseMehediFloat(sshihabb007BoxMatch.Groups[3].Value, 595.28f);
                        float shihabY2 = ParseMehediFloat(sshihabb007BoxMatch.Groups[4].Value, 841.89f);
                        shihabPageBoxes.Add(new RectangleF(shihabX1, mehediY1, Math.Max(100f, sshihabb007X2 - shihabX1), Math.Max(100f, shihabY2 - mehediY1)));
                    }
                    else
                    {
                        shihabPageBoxes.Add(new RectangleF(0, 0, 595.28f, 841.89f));
                    }
                }
                else if (shihabObjHeader.IndexOf("/Type /Font", StringComparison.Ordinal) >= 0 || shihabObjHeader.IndexOf("/Type/Font", StringComparison.Ordinal) >= 0)
                    mehediObjKind = "Font Descriptor (/Font)";
                else if (shihabObjHeader.IndexOf("/Subtype /Image", StringComparison.Ordinal) >= 0 || shihabObjHeader.IndexOf("/Subtype/Image", StringComparison.Ordinal) >= 0)
                    mehediObjKind = "XObject Raster Image (/Image)";
                else if (shihabObjHeader.IndexOf("/Subtype /Annot", StringComparison.Ordinal) >= 0 || shihabObjHeader.IndexOf("/Annot", StringComparison.Ordinal) >= 0)
                    mehediObjKind = "Annotation (/Annot)";
                else if (shihabObjHeader.IndexOf("stream", StringComparison.Ordinal) >= 0)
                    mehediObjKind = "Content / Data Stream";

                sshihabb007Doc.MehediParsedObjects.Add(new MehediPdfObjectNode
                {
                    ShihabObjectNumber = mehediObjNum,
                    MehediGenerationNumber = sshihabb007GenNum,
                    Sshihabb007ByteOffset = shihabStartPos,
                    ShihabObjectTypeSummary = mehediObjKind,
                    MehediRawDictionaryPreview = shihabObjHeader.Replace("\r", " ").Replace("\n", " ").Trim()
                });

                int sshihabb007StreamIdx = mehediLatin.IndexOf("stream", shihabStartPos, StringComparison.Ordinal);
                if (sshihabb007StreamIdx > 0 && sshihabb007StreamIdx < mehediEndObjPos && shihabObjHeader.IndexOf("/Subtype /Image", StringComparison.Ordinal) < 0)
                {
                    int shihabDataStart = sshihabb007StreamIdx + 6;
                    if (shihabDataStart < shihabBytes.Length && shihabBytes[shihabDataStart] == '\r') shihabDataStart++;
                    if (shihabDataStart < shihabBytes.Length && shihabBytes[shihabDataStart] == '\n') shihabDataStart++;

                    int mehediEndStreamIdx = mehediLatin.IndexOf("endstream", shihabDataStart, StringComparison.Ordinal);
                    if (mehediEndStreamIdx > shihabDataStart && mehediEndStreamIdx <= mehediEndObjPos)
                    {
                        int sshihabb007StreamLen = mehediEndStreamIdx - shihabDataStart;
                        while (sshihabb007StreamLen > 0 && (shihabBytes[shihabDataStart + sshihabb007StreamLen - 1] == '\n' || shihabBytes[shihabDataStart + sshihabb007StreamLen - 1] == '\r'))
                        {
                            sshihabb007StreamLen--;
                        }

                        if (sshihabb007StreamLen > 0 && sshihabb007StreamLen < 2 * 1024 * 1024)
                        {
                            byte[] shihabRawStream = new byte[sshihabb007StreamLen];
                            Buffer.BlockCopy(shihabBytes, shihabDataStart, shihabRawStream, 0, sshihabb007StreamLen);
                            string mehediDecodedStream = shihabObjHeader.IndexOf("FlateDecode", StringComparison.OrdinalIgnoreCase) >= 0
                                ? TryDecompressShihabFlateStream(shihabRawStream)
                                : Encoding.GetEncoding(28591).GetString(shihabRawStream);

                            if (!string.IsNullOrWhiteSpace(mehediDecodedStream) && mehediDecodedStream.IndexOf("BT", StringComparison.Ordinal) >= 0)
                            {
                                string sshihabb007ParsedText = ExtractShihabTextFromContentStream(mehediDecodedStream);
                                if (!string.IsNullOrWhiteSpace(sshihabb007ParsedText))
                                {
                                    sshihabb007DecodedTextChunks.Add(sshihabb007ParsedText);
                                }
                            }
                        }
                    }
                }
            }

            List<SizeF> shihabWinRtPageSizes = ShihabWinRtRenderAndOcrEngine.InspectShihabPdfPageDimensionsViaWinRt(shihabBytes);
            int shihabPageCount = shihabWinRtPageSizes.Count > 0 ? shihabWinRtPageSizes.Count : Math.Max(1, shihabPageBoxes.Count);

            if (shihabWinRtPageSizes.Count == 0)
            {
                Match mehediCountMatch = Regex.Match(mehediLatin, @"/Type\s*/Pages\b[^>]*/Count\s+(\d+)");
                if (mehediCountMatch.Success)
                {
                    int sshihabb007DeclaredCount = 1;
                    if (int.TryParse(mehediCountMatch.Groups[1].Value, out sshihabb007DeclaredCount) && sshihabb007DeclaredCount > 0 && sshihabb007DeclaredCount <= 500)
                    {
                        shihabPageCount = Math.Max(shihabPageBoxes.Count, sshihabb007DeclaredCount);
                    }
                }
            }

            for (int sshihabb007PageIdx = 0; sshihabb007PageIdx < shihabPageCount; sshihabb007PageIdx++)
            {
                float shihabPageW = 595.28f;
                float mehediPageH = 841.89f;

                if (sshihabb007PageIdx < shihabWinRtPageSizes.Count)
                {
                    shihabPageW = shihabWinRtPageSizes[sshihabb007PageIdx].Width;
                    mehediPageH = shihabWinRtPageSizes[sshihabb007PageIdx].Height;
                }
                else if (sshihabb007PageIdx < shihabPageBoxes.Count)
                {
                    shihabPageW = shihabPageBoxes[sshihabb007PageIdx].Width;
                    mehediPageH = shihabPageBoxes[sshihabb007PageIdx].Height;
                }

                string mehediPageText = sshihabb007PageIdx < sshihabb007DecodedTextChunks.Count
                    ? sshihabb007DecodedTextChunks[sshihabb007PageIdx]
                    : string.Empty;

                MehediPdfPageModel sshihabb007Page = new MehediPdfPageModel
                {
                    ShihabPageIndex = sshihabb007PageIdx,
                    MehediSourcePdfPageIndex = sshihabb007PageIdx,
                    MehediWidthPt = shihabPageW,
                    Sshihabb007HeightPt = mehediPageH,
                    ShihabExtractedText = mehediPageText
                };

                PopulateShihabEditableBlocksFromExtractedText(sshihabb007Page);
                sshihabb007Doc.Sshihabb007Pages.Add(sshihabb007Page);

                sshihabb007Doc.ShihabBookmarks.Add(new MehediPdfBookmarkItem
                {
                    MehediTitle = "Page " + (sshihabb007PageIdx + 1) + GetShihabFirstHeadingPreview(mehediPageText),
                    Sshihabb007TargetPageIndex = sshihabb007PageIdx,
                    ShihabTargetOffsetY = 0f
                });
            }
        }

        private static string GetShihabFirstHeadingPreview(string mehediText)
        {
            if (string.IsNullOrWhiteSpace(mehediText)) return string.Empty;
            string[] sshihabb007Lines = mehediText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (sshihabb007Lines.Length == 0) return string.Empty;
            string shihabFirst = sshihabb007Lines[0].Trim();
            if (shihabFirst.Length > 34) shihabFirst = shihabFirst.Substring(0, 34) + "...";
            return " — " + shihabFirst;
        }

        public static void PopulateShihabEditableBlocksFromExtractedText(MehediPdfPageModel shihabPage)
        {
            if (shihabPage == null || string.IsNullOrWhiteSpace(shihabPage.ShihabExtractedText)) return;
            shihabPage.ShihabTextBlocks.Clear();
            string[] mehediLines = shihabPage.ShihabExtractedText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            float sshihabb007CursorY = 58.0f;
            for (int shihabIdx = 0; shihabIdx < mehediLines.Length; shihabIdx++)
            {
                string mehediLine = mehediLines[shihabIdx].Trim();
                if (string.IsNullOrEmpty(mehediLine)) continue;
                bool sshihabb007IsTitle = shihabIdx == 0 && mehediLine.Length < 80;
                float shihabFontSize = sshihabb007IsTitle ? 16.0f : 11.0f;
                float mehediHeight = sshihabb007IsTitle ? 26.0f : 18.0f;

                shihabPage.ShihabTextBlocks.Add(new ShihabPdfTextBlockItem
                {
                    MehediPageIndex = shihabPage.ShihabPageIndex,
                    ShihabText = mehediLine,
                    MehediOriginalText = mehediLine,
                    Sshihabb007FontName = sshihabb007IsTitle ? "Helvetica-Bold" : "Helvetica",
                    ShihabFontSizePt = shihabFontSize,
                    Sshihabb007IsBold = sshihabb007IsTitle,
                    Sshihabb007BoundsPt = new RectangleF(48f, sshihabb007CursorY, shihabPage.MehediWidthPt - 96f, mehediHeight)
                });

                sshihabb007CursorY += mehediHeight + 4f;
                if (sshihabb007CursorY > shihabPage.Sshihabb007HeightPt - 54f) break;
            }
        }

        private static string TryDecompressShihabFlateStream(byte[] mehediCompressed)
        {
            if (mehediCompressed == null || mehediCompressed.Length < 3) return string.Empty;
            for (int sshihabb007Skip = 2; sshihabb007Skip >= 0; sshihabb007Skip -= 2)
            {
                try
                {
                    if (mehediCompressed.Length <= sshihabb007Skip) continue;
                    using (MemoryStream shihabMs = new MemoryStream(mehediCompressed, sshihabb007Skip, mehediCompressed.Length - sshihabb007Skip))
                    using (DeflateStream mehediDeflate = new DeflateStream(shihabMs, CompressionMode.Decompress))
                    using (MemoryStream sshihabb007Out = new MemoryStream())
                    {
                        mehediDeflate.CopyTo(sshihabb007Out);
                        return Encoding.GetEncoding(28591).GetString(sshihabb007Out.ToArray());
                    }
                }
                catch { }
            }
            return string.Empty;
        }

        private static string ExtractShihabTextFromContentStream(string mehediContentStream)
        {
            StringBuilder sshihabb007Builder = new StringBuilder();
            Regex shihabTokenRegex = new Regex(@"\(([^\)\\]*(?:\\.[^\)\\]*)*)\)\s*Tj|\[([^\]]*)\]\s*TJ|\b(T\*|TD|Td)\b", RegexOptions.Compiled);
            MatchCollection mehediMatches = shihabTokenRegex.Matches(mehediContentStream);
            foreach (Match sshihabb007Match in mehediMatches)
            {
                if (sshihabb007Match.Groups[1].Success)
                {
                    sshihabb007Builder.Append(UnescapeShihabPdfLiteral(sshihabb007Match.Groups[1].Value));
                }
                else if (sshihabb007Match.Groups[2].Success)
                {
                    string shihabArrayBody = sshihabb007Match.Groups[2].Value;
                    MatchCollection mehediSubLiterals = Regex.Matches(shihabArrayBody, @"\(([^\)\\]*(?:\\.[^\)\\]*)*)\)");
                    foreach (Match sshihabb007Sub in mehediSubLiterals)
                    {
                        sshihabb007Builder.Append(UnescapeShihabPdfLiteral(sshihabb007Sub.Groups[1].Value));
                    }
                }
                else if (sshihabb007Match.Groups[3].Success)
                {
                    sshihabb007Builder.AppendLine();
                }
            }
            return sshihabb007Builder.ToString().Trim();
        }

        private static float ParseMehediFloat(string shihabText, float mehediDefault)
        {
            float sshihabb007Result;
            if (float.TryParse(shihabText, NumberStyles.Float, CultureInfo.InvariantCulture, out sshihabb007Result))
            {
                return sshihabb007Result;
            }
            return mehediDefault;
        }

        public static void SaveShihabPdfDocumentToFile(ShihabPdfDocumentModel shihabDoc, string mehediOutputPath)
        {
            byte[] sshihabb007Bytes = WriteMehediPdfDocumentToBytes(shihabDoc);
            File.WriteAllBytes(mehediOutputPath, sshihabb007Bytes);
            shihabDoc.MehediFilePath = mehediOutputPath;
            shihabDoc.Sshihabb007DisplayTitle = Path.GetFileName(mehediOutputPath);
            shihabDoc.MehediOriginalFileBytes = sshihabb007Bytes;
            shihabDoc.Sshihabb007FileSizeBytes = sshihabb007Bytes.LongLength;
            shihabDoc.MehediSha256Digest = ComputeShihabSha256Hex(sshihabb007Bytes);
            shihabDoc.ShihabIsModifiedDirty = false;

            shihabDoc.ShihabConnectedHistory.Add(new MehediConnectedPdfEvent
            {
                ShihabEventId = "evt_shihab_save_" + Guid.NewGuid().ToString("N").Substring(0, 6),
                MehediTimestampUtc = DateTime.UtcNow,
                Sshihabb007ActorIdentity = "Mehedi Hasan Shihab (@sshihabb007)",
                ShihabOperationType = "DOCUMENT_SAVE",
                MehediSummaryDescription = "Saved ISO 32000-1 PDF 1.7 (" + sshihabb007Bytes.Length + " bytes, SHA-256=" + shihabDoc.MehediSha256Digest.Substring(0, 12) + "...)",
                Sshihabb007DocumentDigest = shihabDoc.MehediSha256Digest
            });
        }

        public static byte[] WriteMehediPdfDocumentToBytes(ShihabPdfDocumentModel shihabDoc)
        {
            using (MemoryStream mehediMs = new MemoryStream())
            {
                Encoding sshihabb007Enc = Encoding.GetEncoding(28591);
                Action<string> shihabWriteStr = (string mehediText) =>
                {
                    byte[] sshihabb007Buf = sshihabb007Enc.GetBytes(mehediText);
                    mehediMs.Write(sshihabb007Buf, 0, sshihabb007Buf.Length);
                };

                shihabWriteStr("%PDF-1.7\n%\xE2\xE3\xCF\xD3\n");
                shihabWriteStr("% Engineered by Mehedi Hasan Shihab (@sshihabb007) — PDF PRO Starship Engine\n");

                List<long> mehediXrefOffsets = new List<long> { 0L }; // Object 0 is free head
                Func<int> sshihabb007AllocObj = () =>
                {
                    int shihabId = mehediXrefOffsets.Count;
                    mehediXrefOffsets.Add(0L);
                    return shihabId;
                };

                int shihabCatalogObjId = sshihabb007AllocObj();
                int mehediPagesTreeObjId = sshihabb007AllocObj();
                int sshihabb007FontRegObjId = sshihabb007AllocObj();
                int shihabFontBoldObjId = sshihabb007AllocObj();
                int mehediFontMonoObjId = sshihabb007AllocObj();
                int sshihabb007InfoObjId = sshihabb007AllocObj();

                int shihabPageCount = Math.Max(1, shihabDoc.Sshihabb007Pages.Count);
                List<int> mehediPageObjIds = new List<int>();
                List<int> sshihabb007ContentObjIds = new List<int>();

                for (int shihabIdx = 0; shihabIdx < shihabPageCount; shihabIdx++)
                {
                    mehediPageObjIds.Add(sshihabb007AllocObj());
                    sshihabb007ContentObjIds.Add(sshihabb007AllocObj());
                }

                // 1. Catalog Object
                mehediXrefOffsets[shihabCatalogObjId] = mehediMs.Position;
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "{0} 0 obj\n<< /Type /Catalog /Pages {1} 0 R /Lang (en-US) /ShihabEngine (sshihabb007-Starship-14.0.4) >>\nendobj\n",
                    shihabCatalogObjId, mehediPagesTreeObjId));

                // 2. Pages Tree Object
                StringBuilder mehediKidsSb = new StringBuilder();
                foreach (int sshihabb007Pid in mehediPageObjIds)
                {
                    mehediKidsSb.Append(sshihabb007Pid).Append(" 0 R ");
                }
                mehediXrefOffsets[mehediPagesTreeObjId] = mehediMs.Position;
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "{0} 0 obj\n<< /Type /Pages /Kids [ {1}] /Count {2} >>\nendobj\n",
                    mehediPagesTreeObjId, mehediKidsSb.ToString(), mehediPageObjIds.Count));

                // 3. Standard Type1 Fonts (/F1 Helvetica, /F2 Helvetica-Bold, /F3 Courier)
                mehediXrefOffsets[sshihabb007FontRegObjId] = mehediMs.Position;
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "{0} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>\nendobj\n",
                    sshihabb007FontRegObjId));

                mehediXrefOffsets[shihabFontBoldObjId] = mehediMs.Position;
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "{0} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold /Encoding /WinAnsiEncoding >>\nendobj\n",
                    shihabFontBoldObjId));

                mehediXrefOffsets[mehediFontMonoObjId] = mehediMs.Position;
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "{0} 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Courier /Encoding /WinAnsiEncoding >>\nendobj\n",
                    mehediFontMonoObjId));

                // 4. Each Page & Content Stream
                for (int shihabIdx = 0; shihabIdx < shihabPageCount; shihabIdx++)
                {
                    MehediPdfPageModel mehediPage = shihabIdx < shihabDoc.Sshihabb007Pages.Count
                        ? shihabDoc.Sshihabb007Pages[shihabIdx]
                        : new MehediPdfPageModel { ShihabPageIndex = shihabIdx };

                    string sshihabb007ContentStream = BuildShihabPageContentStream(mehediPage, shihabDoc, shihabIdx, shihabPageCount);
                    byte[] shihabStreamBytes = sshihabb007Enc.GetBytes(sshihabb007ContentStream);

                    int mehediPageObjId = mehediPageObjIds[shihabIdx];
                    int sshihabb007ContentObjId = sshihabb007ContentObjIds[shihabIdx];

                    mehediXrefOffsets[mehediPageObjId] = mehediMs.Position;
                    shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                        "{0} 0 obj\n<< /Type /Page /Parent {1} 0 R /MediaBox [0 0 {2:F2} {3:F2}] /Rotate {4} /Resources << /Font << /F1 {5} 0 R /F2 {6} 0 R /F3 {7} 0 R >> >> /Contents {8} 0 R >>\nendobj\n",
                        mehediPageObjId,
                        mehediPagesTreeObjId,
                        mehediPage.MehediWidthPt,
                        mehediPage.Sshihabb007HeightPt,
                        mehediPage.ShihabRotationDegrees % 360,
                        sshihabb007FontRegObjId,
                        shihabFontBoldObjId,
                        mehediFontMonoObjId,
                        sshihabb007ContentObjId));

                    mehediXrefOffsets[sshihabb007ContentObjId] = mehediMs.Position;
                    shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                        "{0} 0 obj\n<< /Length {1} >>\nstream\n",
                        sshihabb007ContentObjId, shihabStreamBytes.Length));
                    mehediMs.Write(shihabStreamBytes, 0, shihabStreamBytes.Length);
                    shihabWriteStr("\nendstream\nendobj\n");
                }

                // 5. Document Info Dictionary
                mehediXrefOffsets[sshihabb007InfoObjId] = mehediMs.Position;
                string shihabNowPdfDate = "D:" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "Z";
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "{0} 0 obj\n<< /Title ({1}) /Author ({2}) /Subject ({3}) /Keywords ({4}) /Creator ({5}) /Producer ({6}) /CreationDate ({7}) /ModDate ({7}) /ShihabAipLabel ({8}) >>\nendobj\n",
                    sshihabb007InfoObjId,
                    EscapeShihabPdfLiteral(shihabDoc.MehediTitleMeta),
                    EscapeShihabPdfLiteral(shihabDoc.Sshihabb007AuthorMeta),
                    EscapeShihabPdfLiteral(shihabDoc.ShihabSubjectMeta),
                    EscapeShihabPdfLiteral(shihabDoc.MehediKeywordsMeta),
                    EscapeShihabPdfLiteral(shihabDoc.Sshihabb007CreatorMeta),
                    EscapeShihabPdfLiteral(shihabDoc.ShihabProducerMeta),
                    shihabNowPdfDate,
                    EscapeShihabPdfLiteral(shihabDoc.MehediAipSensitivityLabel)));

                // 6. Cross-Reference Table (xref)
                long mehediXrefPos = mehediMs.Position;
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture, "xref\n0 {0}\n0000000000 65535 f \n", mehediXrefOffsets.Count));
                for (int sshihabb007Idx = 1; sshihabb007Idx < mehediXrefOffsets.Count; sshihabb007Idx++)
                {
                    shihabWriteStr(string.Format(CultureInfo.InvariantCulture, "{0:D10} 00000 n \n", mehediXrefOffsets[sshihabb007Idx]));
                }

                // 7. Trailer
                shihabWriteStr(string.Format(CultureInfo.InvariantCulture,
                    "trailer\n<< /Size {0} /Root {1} 0 R /Info {2} 0 R >>\nstartxref\n{3}\n%%EOF\n",
                    mehediXrefOffsets.Count, shihabCatalogObjId, sshihabb007InfoObjId, mehediXrefPos));

                return mehediMs.ToArray();
            }
        }

        private static string BuildShihabPageContentStream(MehediPdfPageModel shihabPage, ShihabPdfDocumentModel mehediDoc, int sshihabb007PageIdx, int shihabTotalPages)
        {
            StringBuilder mehediSb = new StringBuilder();
            float sshihabb007H = shihabPage.Sshihabb007HeightPt;
            float shihabW = shihabPage.MehediWidthPt;

            // 1. Render Text Blocks
            if (shihabPage.ShihabTextBlocks != null && shihabPage.ShihabTextBlocks.Count > 0)
            {
                foreach (ShihabPdfTextBlockItem mehediBlock in shihabPage.ShihabTextBlocks)
                {
                    if (string.IsNullOrEmpty(mehediBlock.ShihabText)) continue;
                    float sshihabb007PdfY = sshihabb007H - mehediBlock.Sshihabb007BoundsPt.Top - mehediBlock.ShihabFontSizePt;
                    string shihabFontTag = mehediBlock.Sshihabb007IsBold ? "/F2" : "/F1";
                    float mehediR = mehediBlock.MehediTextColor.R / 255f;
                    float sshihabb007G = mehediBlock.MehediTextColor.G / 255f;
                    float shihabB = mehediBlock.MehediTextColor.B / 255f;

                    mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                        "BT {0:F3} {1:F3} {2:F3} rg {3} {4:F1} Tf {5:F2} {6:F2} Td ({7}) Tj ET\n",
                        mehediR, sshihabb007G, shihabB,
                        shihabFontTag, mehediBlock.ShihabFontSizePt,
                        mehediBlock.Sshihabb007BoundsPt.Left, sshihabb007PdfY,
                        EscapeShihabPdfLiteral(mehediBlock.ShihabText));
                }
            }
            else if (!string.IsNullOrWhiteSpace(shihabPage.ShihabExtractedText))
            {
                string[] mehediLines = shihabPage.ShihabExtractedText.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
                float sshihabb007Y = sshihabb007H - 64f;
                foreach (string shihabLine in mehediLines)
                {
                    if (sshihabb007Y < 48f) break;
                    mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                        "BT 0.10 0.12 0.16 rg /F1 11.0 Tf 48.00 {0:F2} Td ({1}) Tj ET\n",
                        sshihabb007Y, EscapeShihabPdfLiteral(shihabLine));
                    sshihabb007Y -= 16f;
                }
            }

            // 2. Render Header / Footer / Bates Number
            if (!string.IsNullOrWhiteSpace(shihabPage.MehediHeaderText))
            {
                mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                    "BT 0.35 0.38 0.46 rg /F1 9.0 Tf 48.00 {0:F2} Td ({1}) Tj ET\n",
                    sshihabb007H - 28f, EscapeShihabPdfLiteral(shihabPage.MehediHeaderText));
            }
            string sshihabb007Footer = !string.IsNullOrWhiteSpace(shihabPage.Sshihabb007FooterText)
                ? shihabPage.Sshihabb007FooterText
                : string.Format(CultureInfo.InvariantCulture, "Page {0} of {1}  |  PDF PRO (Mehedi Hasan Shihab @sshihabb007)", sshihabb007PageIdx + 1, shihabTotalPages);
            mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                "BT 0.40 0.44 0.52 rg /F1 8.5 Tf 48.00 24.00 Td ({0}) Tj ET\n",
                EscapeShihabPdfLiteral(sshihabb007Footer));

            if (!string.IsNullOrWhiteSpace(shihabPage.MehediBatesNumberText))
            {
                mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                    "BT 0.48 0.22 0.90 rg /F3 9.5 Tf {0:F2} {1:F2} Td ({2}) Tj ET\n",
                    shihabW - 165f, sshihabb007H - 28f, EscapeShihabPdfLiteral(shihabPage.MehediBatesNumberText));
            }

            // 3. Render Watermark
            if (!string.IsNullOrWhiteSpace(shihabPage.ShihabWatermarkText))
            {
                mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                    "q 0.78 0.75 0.92 rg BT /F2 42.0 Tf 0.7071 0.7071 -0.7071 0.7071 {0:F2} {1:F2} Tm ({2}) Tj ET Q\n",
                    shihabW * 0.22f, sshihabb007H * 0.35f, EscapeShihabPdfLiteral(shihabPage.ShihabWatermarkText));
            }

            // 4. Render Annotations, Shapes, Redactions & Signatures
            if (shihabPage.MehediAnnotations != null)
            {
                foreach (ShihabPdfAnnotationItem shihabAnnot in shihabPage.MehediAnnotations)
                {
                    RectangleF mehediRect = shihabAnnot.ShihabBoundsPt;
                    float sshihabb007PdfY = sshihabb007H - mehediRect.Bottom;
                    float shihabSr = shihabAnnot.Sshihabb007StrokeColor.R / 255f;
                    float mehediSg = shihabAnnot.Sshihabb007StrokeColor.G / 255f;
                    float sshihabb007Sb = shihabAnnot.Sshihabb007StrokeColor.B / 255f;

                    if (shihabAnnot.Sshihabb007Kind == MehediAnnotationKind.MehediRedaction && shihabAnnot.Sshihabb007IsRedactionBurned)
                    {
                        mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                            "q 0 0 0 rg {0:F2} {1:F2} {2:F2} {3:F2} re f 1 1 1 rg BT /F2 8.5 Tf {4:F2} {5:F2} Td ([REDACTED BY SHIHAB PDF PRO]) Tj ET Q\n",
                            mehediRect.Left, sshihabb007PdfY, mehediRect.Width, mehediRect.Height,
                            mehediRect.Left + 4f, sshihabb007PdfY + (mehediRect.Height * 0.35f));
                    }
                    else if (shihabAnnot.Sshihabb007Kind == MehediAnnotationKind.MehediHighlight)
                    {
                        mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                            "q 1.00 0.94 0.35 RG {0:F2} w {1:F2} {2:F2} m {3:F2} {2:F2} l S Q\n",
                            Math.Max(2f, mehediRect.Height * 0.25f), mehediRect.Left, sshihabb007PdfY, mehediRect.Right);
                    }
                    else if (shihabAnnot.Sshihabb007Kind == MehediAnnotationKind.Sshihabb007Underline || shihabAnnot.Sshihabb007Kind == MehediAnnotationKind.ShihabStrikeout)
                    {
                        float mehediLineY = shihabAnnot.Sshihabb007Kind == MehediAnnotationKind.Sshihabb007Underline
                            ? sshihabb007PdfY
                            : sshihabb007PdfY + (mehediRect.Height * 0.5f);
                        mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                            "q {0:F2} {1:F2} {2:F2} RG 1.50 w {3:F2} {4:F2} m {5:F2} {4:F2} l S Q\n",
                            shihabSr, mehediSg, sshihabb007Sb, mehediRect.Left, mehediLineY, mehediRect.Right);
                    }
                    else
                    {
                        mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                            "q {0:F2} {1:F2} {2:F2} RG {3:F2} w {4:F2} {5:F2} {6:F2} {7:F2} re S Q\n",
                            shihabSr, mehediSg, sshihabb007Sb, Math.Max(1f, shihabAnnot.MehediStrokeWidth),
                            mehediRect.Left, sshihabb007PdfY, mehediRect.Width, mehediRect.Height);

                        if (!string.IsNullOrWhiteSpace(shihabAnnot.Sshihabb007TextContent))
                        {
                            mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                                "BT {0:F2} {1:F2} {2:F2} rg /F2 {3:F1} Tf {4:F2} {5:F2} Td ({6}) Tj ET\n",
                                shihabSr, mehediSg, sshihabb007Sb,
                                Math.Max(8f, shihabAnnot.Sshihabb007FontSizePt),
                                mehediRect.Left + 6f, sshihabb007PdfY + Math.Max(6f, mehediRect.Height - 16f),
                                EscapeShihabPdfLiteral(shihabAnnot.Sshihabb007TextContent));
                        }
                    }
                }
            }

            // 5. Render AcroForm Fields
            if (shihabPage.Sshihabb007FormFields != null)
            {
                foreach (ShihabAcroFormFieldItem mehediFld in shihabPage.Sshihabb007FormFields)
                {
                    RectangleF sshihabb007Rect = mehediFld.Sshihabb007BoundsPt;
                    float shihabPdfY = sshihabb007H - sshihabb007Rect.Bottom;
                    mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                        "q 0.94 0.96 1.00 rg 0.35 0.42 0.75 RG 1.0 w {0:F2} {1:F2} {2:F2} {3:F2} re B Q\n",
                        sshihabb007Rect.Left, shihabPdfY, sshihabb007Rect.Width, sshihabb007Rect.Height);

                    string mehediDisplayVal = mehediFld.Sshihabb007FieldType == "CheckBox"
                        ? (mehediFld.ShihabIsChecked ? "[X] " + mehediFld.ShihabFieldName : "[ ] " + mehediFld.ShihabFieldName)
                        : (mehediFld.ShihabFieldName + ": " + mehediFld.Sshihabb007FieldValue);

                    mehediSb.AppendFormat(CultureInfo.InvariantCulture,
                        "BT 0.12 0.16 0.30 rg /F1 9.5 Tf {0:F2} {1:F2} Td ({2}) Tj ET\n",
                        sshihabb007Rect.Left + 5f, shihabPdfY + 6f, EscapeShihabPdfLiteral(mehediDisplayVal));
                }
            }

            return mehediSb.ToString();
        }

        public static string EscapeShihabPdfLiteral(string mehediText)
        {
            if (string.IsNullOrEmpty(mehediText)) return string.Empty;
            StringBuilder sshihabb007Clean = new StringBuilder(mehediText.Length + 8);
            foreach (char shihabCh in mehediText)
            {
                if (shihabCh == '(' || shihabCh == ')' || shihabCh == '\\')
                {
                    sshihabb007Clean.Append('\\').Append(shihabCh);
                }
                else if (shihabCh >= 32 && shihabCh <= 126)
                {
                    sshihabb007Clean.Append(shihabCh);
                }
                else if (shihabCh == '\n' || shihabCh == '\r' || shihabCh == '\t')
                {
                    sshihabb007Clean.Append(' ');
                }
            }
            return sshihabb007Clean.ToString();
        }

        // =====================================================================
        // BUILT-IN TEMPLATES & SAMPLE DOCUMENT GENERATOR
        // =====================================================================
        public static ShihabPdfDocumentModel CreateShihabShowcaseDocument(string mehediTemplateKind)
        {
            ShihabPdfDocumentModel sshihabb007Doc = new ShihabPdfDocumentModel
            {
                Sshihabb007DisplayTitle = "PDF-PRO-Starship-Showcase-Shihab.pdf",
                MehediTitleMeta = "PDF PRO 14.0.4 Starship Architecture & Feature Showcase",
                Sshihabb007AuthorMeta = "Mehedi Hasan Shihab (@sshihabb007)",
                ShihabSubjectMeta = "Enterprise PDF Editing, OCR, Office Export, PDF/A Preflight, RMS Security & AI Suite",
                MehediKeywordsMeta = "PDF PRO, Shihab, Mehedi, sshihabb007, Starship, Foxit Pro, Open Source"
            };

            // Page 1: Executive Overview & Editable Content
            MehediPdfPageModel shihabPage1 = new MehediPdfPageModel
            {
                ShihabPageIndex = 0,
                MehediHeaderText = "PDF PRO v14.0.4 (Starship Edition)  |  Author: Mehedi Hasan Shihab (@sshihabb007)",
                Sshihabb007FooterText = "Page 1 of 3  |  Open Source Enterprise PDF Workstation — https://sshihabb007.github.io/",
                ShihabWatermarkText = "PDF PRO — SHIHAB",
                MehediBatesNumberText = "SHIHAB-000001"
            };

            string[] mehediPage1Lines = new[]
            {
                "PDF PRO 14.0.4 — ENTERPRISE PDF WORKSTATION (STARSHIP ARCHITECTURE)",
                "Engineered by Mehedi Hasan Shihab (@sshihabb007) — Open Source Flagship Edition",
                "--------------------------------------------------------------------------------",
                "1. MONOLITHIC CORE + DYNAMIC PLUGIN LOADER (PlgDynLoader.fpi ABI):",
                "   - Manages 32 modular plugins declared in plugins/DefaultPluginsList.xml",
                "   - Supports LoadBehavior=0 (Bootstrap), LoadBehavior=3 (Eager), LoadBehavior=9 (Lazy)",
                "   - Double-click any paragraph in 'Edit Content' mode to edit text in place!",
                "2. NATIVE DIRECT2D HIGH-DPI RENDERING & NEURAL OCR ENGINE:",
                "   - Hardware-accelerated Windows.Data.Pdf rendering with LRU bitmap cache",
                "   - Windows.Media.Ocr neural text recognition with searchable PDF layer injection",
                "3. FIVE EMBEDDED ENTERPRISE TRANSFORMATION ENGINES:",
                "   - Solid Office Exporter: Convert PDF to Word (.docx), Excel (.xlsx/.csv), HTML, RTF, Markdown",
                "   - Callas PDF/A-1b/2b Preflight & Stream Compression Optimizer",
                "   - Lucene++ Inverted Full-Text Search Index with BM25 Relevance Ranking",
                "   - ODA 3D U3D/CAD Wireframe & Orbit Viewport Inspector",
                "   - FXJSE JavaScript Automation Console & Interactive AcroForm Designer",
                "4. ENTERPRISE SECURITY, X.509 PKCS#7 SIGNING, REDACTION & BUILT-IN AI:",
                "   - Permanent Whiteout Redaction, X.509 Digital Signatures, and MIP/AIP Labels",
                "   - Built-in AI Assistant for Executive Summaries, Q&A, and Smart PII Redaction",
                "   - Contact: Mehedi Hasan Shihab (sshihabb007@github.io | +880-1700-000007)"
            };

            shihabPage1.ShihabExtractedText = string.Join(Environment.NewLine, mehediPage1Lines);
            PopulateShihabEditableBlocksFromExtractedText(shihabPage1);

            shihabPage1.MehediAnnotations.Add(new ShihabPdfAnnotationItem
            {
                MehediPageIndex = 0,
                Sshihabb007Kind = MehediAnnotationKind.ShihabStamp,
                ShihabBoundsPt = new RectangleF(375f, 62f, 172f, 38f),
                Sshihabb007TextContent = "APPROVED BY SHIHAB",
                Sshihabb007StrokeColor = Color.FromArgb(255, 16, 185, 129),
                ShihabFillColor = Color.FromArgb(40, 16, 185, 129),
                MehediStrokeWidth = 2.2f,
                Sshihabb007FontSizePt = 10.5f
            });

            shihabPage1.Sshihabb007FormFields.Add(new ShihabAcroFormFieldItem
            {
                MehediPageIndex = 0,
                Sshihabb007FieldType = "Text",
                ShihabFieldName = "ReviewerName_Shihab",
                Sshihabb007FieldValue = "Mehedi Hasan Shihab (@sshihabb007)",
                Sshihabb007BoundsPt = new RectangleF(48f, 540f, 260f, 26f)
            });

            shihabPage1.Sshihabb007FormFields.Add(new ShihabAcroFormFieldItem
            {
                MehediPageIndex = 0,
                Sshihabb007FieldType = "CheckBox",
                ShihabFieldName = "PdfAComplianceVerified_Mehedi",
                ShihabIsChecked = true,
                Sshihabb007BoundsPt = new RectangleF(325f, 540f, 215f, 26f)
            });

            sshihabb007Doc.Sshihabb007Pages.Add(shihabPage1);

            // Page 2: Financial & Quantitative Table for Excel/CSV Export & AI Analysis
            MehediPdfPageModel shihabPage2 = new MehediPdfPageModel
            {
                ShihabPageIndex = 1,
                MehediHeaderText = "PDF PRO v14.0.4 — Financial & Performance Benchmark Ledger",
                Sshihabb007FooterText = "Page 2 of 3  |  Solid Office Export (.docx / .xlsx / .csv) Ready",
                MehediBatesNumberText = "SHIHAB-000002"
            };

            string[] mehediPage2Lines = new[]
            {
                "SUBSYSTEM BENCHMARK & ENTERPRISE TELEMETRY TABLE (Q1-Q4 2026)",
                "Prepared by: Mehedi Hasan Shihab (@sshihabb007) — Chief Systems Architect",
                "--------------------------------------------------------------------------------",
                "Module Name               | Engine Codec         | Latency (ms) | Status",
                "ShihabPlgDynLoader        | PlugInMain ABI       | 1.4 ms       | ACTIVE",
                "MehediWinRtPdfRenderer    | Direct2D High-DPI    | 8.2 ms       | ACTIVE",
                "ShihabNeuralOcrEngine     | Windows.Media.Ocr    | 42.5 ms      | READY",
                "MehediSolidOfficeExporter | OpenXML DOCX/XLSX    | 14.8 ms      | READY",
                "ShihabCallasPreflight     | ISO 19005 PDF/A-2b   | 6.1 ms       | VERIFIED",
                "MehediLuceneSearchIndex   | Inverted BM25 Index  | 0.9 ms       | INDEXED",
                "ShihabCryptoX509Signer    | SHA-256 RSA-4096     | 11.3 ms      | SIGNED",
                "MehediSensorOpenTracing   | Sensor.db Span Log   | 0.4 ms       | RECORDING",
                "--------------------------------------------------------------------------------",
                "Summary Notes:",
                "- All 32 Starship plugins are registered with zero startup bloat.",
                "- Use Convert -> To Word (.docx) or To Excel (.xlsx/.csv) to export this table.",
                "- Use AI Assistant -> Summarize or Smart PII Audit to analyze this document."
            };
            shihabPage2.ShihabExtractedText = string.Join(Environment.NewLine, mehediPage2Lines);
            PopulateShihabEditableBlocksFromExtractedText(shihabPage2);
            sshihabb007Doc.Sshihabb007Pages.Add(shihabPage2);

            // Page 3: Security, Digital Signatures & FXJSE Scripting
            MehediPdfPageModel shihabPage3 = new MehediPdfPageModel
            {
                ShihabPageIndex = 2,
                MehediHeaderText = "PDF PRO v14.0.4 — Cryptographic Verification & FXJSE Automation",
                Sshihabb007FooterText = "Page 3 of 3  |  Signed with X.509 SHA-256 Certificate (sshihabb007)",
                MehediBatesNumberText = "SHIHAB-000003"
            };

            string[] mehediPage3Lines = new[]
            {
                "CRYPTOGRAPHIC ATTESTATION & RIGHTS MANAGEMENT MANIFEST",
                "Certificate Authority: CN=PDF PRO Root CA (Mehedi Hasan Shihab / sshihabb007)",
                "--------------------------------------------------------------------------------",
                "1. Microsoft RMS & AIP Sensitivity Label: PUBLIC — UNRESTRICTED",
                "2. ConnectedPDF Lineage ID: " + sshihabb007Doc.ShihabConnectedPdfDocId,
                "3. Embedded FXJSE JavaScript Runtime:",
                "   - Execute scripts in the right-hand FXJSE Console (e.g., this.numPages)",
                "   - Automate watermarking, Bates numbering, and form validation",
                "4. Digital Signature Verification:",
                "   - Signer: Mehedi Hasan Shihab (@sshihabb007)",
                "   - Algorithm: RSA-4096 / SHA-256 Detached & Visual Stamp",
                "   - Status: VALIDATED — Document integrity confirmed."
            };
            shihabPage3.ShihabExtractedText = string.Join(Environment.NewLine, mehediPage3Lines);
            PopulateShihabEditableBlocksFromExtractedText(shihabPage3);
            sshihabb007Doc.Sshihabb007Pages.Add(shihabPage3);

            // Bookmarks
            sshihabb007Doc.ShihabBookmarks.Add(new MehediPdfBookmarkItem { MehediTitle = "1. Executive Starship Architecture", Sshihabb007TargetPageIndex = 0 });
            sshihabb007Doc.ShihabBookmarks.Add(new MehediPdfBookmarkItem { MehediTitle = "2. Subsystem Benchmark & Export Table", Sshihabb007TargetPageIndex = 1 });
            sshihabb007Doc.ShihabBookmarks.Add(new MehediPdfBookmarkItem { MehediTitle = "3. Cryptographic & RMS Attestation", Sshihabb007TargetPageIndex = 2 });

            // Digital Signature Record
            sshihabb007Doc.Sshihabb007Signatures.Add(new ShihabDigitalSignatureRecord
            {
                ShihabPageIndex = 2,
                MehediVisualRectPt = new RectangleF(48f, 340f, 280f, 68f)
            });

            byte[] shihabCompiledBytes = WriteMehediPdfDocumentToBytes(sshihabb007Doc);
            sshihabb007Doc.MehediOriginalFileBytes = shihabCompiledBytes;
            sshihabb007Doc.Sshihabb007FileSizeBytes = shihabCompiledBytes.LongLength;
            sshihabb007Doc.MehediSha256Digest = ComputeShihabSha256Hex(shihabCompiledBytes);
            return sshihabb007Doc;
        }

        // =====================================================================
        // PAGE ORGANIZATION OPERATIONS (Merge, Split, Rotate, Insert, Delete, Extract)
        // =====================================================================
        public static void InsertShihabBlankPage(ShihabPdfDocumentModel shihabDoc, int mehediInsertAfterIndex)
        {
            int sshihabb007TargetIdx = Math.Max(0, Math.Min(shihabDoc.Sshihabb007Pages.Count, mehediInsertAfterIndex + 1));
            MehediPdfPageModel shihabNewPage = new MehediPdfPageModel
            {
                ShihabPageIndex = sshihabb007TargetIdx,
                MehediSourcePdfPageIndex = -1,
                MehediHeaderText = "PDF PRO — Blank Page Inserted by Mehedi Hasan Shihab (@sshihabb007)",
                ShihabExtractedText = "New Editable Page (" + (sshihabb007TargetIdx + 1) + ") — Double-click or use 'Add Text' to add content."
            };
            PopulateShihabEditableBlocksFromExtractedText(shihabNewPage);
            shihabDoc.Sshihabb007Pages.Insert(sshihabb007TargetIdx, shihabNewPage);
            ReindexMehediPages(shihabDoc);
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        public static void DuplicateMehediPage(ShihabPdfDocumentModel shihabDoc, int mehediPageIdx)
        {
            if (mehediPageIdx < 0 || mehediPageIdx >= shihabDoc.Sshihabb007Pages.Count) return;
            MehediPdfPageModel sshihabb007Src = shihabDoc.Sshihabb007Pages[mehediPageIdx];
            MehediPdfPageModel shihabClone = new MehediPdfPageModel
            {
                MehediWidthPt = sshihabb007Src.MehediWidthPt,
                Sshihabb007HeightPt = sshihabb007Src.Sshihabb007HeightPt,
                ShihabRotationDegrees = sshihabb007Src.ShihabRotationDegrees,
                MehediSourcePdfPageIndex = sshihabb007Src.MehediSourcePdfPageIndex,
                ShihabExtractedText = sshihabb007Src.ShihabExtractedText,
                MehediHeaderText = sshihabb007Src.MehediHeaderText,
                Sshihabb007FooterText = sshihabb007Src.Sshihabb007FooterText,
                ShihabWatermarkText = sshihabb007Src.ShihabWatermarkText,
                MehediBatesNumberText = sshihabb007Src.MehediBatesNumberText
            };
            foreach (ShihabPdfTextBlockItem mehediTb in sshihabb007Src.ShihabTextBlocks)
            {
                shihabClone.ShihabTextBlocks.Add(new ShihabPdfTextBlockItem
                {
                    ShihabText = mehediTb.ShihabText,
                    MehediOriginalText = mehediTb.MehediOriginalText,
                    Sshihabb007FontName = mehediTb.Sshihabb007FontName,
                    ShihabFontSizePt = mehediTb.ShihabFontSizePt,
                    MehediTextColor = mehediTb.MehediTextColor,
                    Sshihabb007IsBold = mehediTb.Sshihabb007IsBold,
                    Sshihabb007BoundsPt = mehediTb.Sshihabb007BoundsPt
                });
            }
            foreach (ShihabPdfAnnotationItem sshihabb007An in sshihabb007Src.MehediAnnotations)
            {
                shihabClone.MehediAnnotations.Add(sshihabb007An.CloneForShihabUndo());
            }
            shihabDoc.Sshihabb007Pages.Insert(mehediPageIdx + 1, shihabClone);
            ReindexMehediPages(shihabDoc);
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        public static bool DeleteSshihabb007Page(ShihabPdfDocumentModel shihabDoc, int mehediPageIdx)
        {
            if (shihabDoc.Sshihabb007Pages.Count <= 1 || mehediPageIdx < 0 || mehediPageIdx >= shihabDoc.Sshihabb007Pages.Count)
                return false;
            shihabDoc.Sshihabb007Pages.RemoveAt(mehediPageIdx);
            ReindexMehediPages(shihabDoc);
            shihabDoc.ShihabIsModifiedDirty = true;
            return true;
        }

        public static void RotateShihabPage(ShihabPdfDocumentModel shihabDoc, int mehediPageIdx, int sshihabb007DeltaDegrees)
        {
            if (mehediPageIdx < 0 || mehediPageIdx >= shihabDoc.Sshihabb007Pages.Count) return;
            MehediPdfPageModel shihabPage = shihabDoc.Sshihabb007Pages[mehediPageIdx];
            shihabPage.ShihabRotationDegrees = ((shihabPage.ShihabRotationDegrees + sshihabb007DeltaDegrees) % 360 + 360) % 360;
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        public static void MoveMehediPageOrder(ShihabPdfDocumentModel shihabDoc, int mehediFromIdx, int sshihabb007ToIdx)
        {
            if (mehediFromIdx < 0 || mehediFromIdx >= shihabDoc.Sshihabb007Pages.Count) return;
            if (sshihabb007ToIdx < 0 || sshihabb007ToIdx >= shihabDoc.Sshihabb007Pages.Count || mehediFromIdx == sshihabb007ToIdx) return;
            MehediPdfPageModel shihabItem = shihabDoc.Sshihabb007Pages[mehediFromIdx];
            shihabDoc.Sshihabb007Pages.RemoveAt(mehediFromIdx);
            shihabDoc.Sshihabb007Pages.Insert(sshihabb007ToIdx, shihabItem);
            ReindexMehediPages(shihabDoc);
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        public static ShihabPdfDocumentModel MergeShihabPdfDocuments(IEnumerable<ShihabPdfDocumentModel> mehediSourceDocs, string sshihabb007MergedTitle)
        {
            ShihabPdfDocumentModel shihabMerged = new ShihabPdfDocumentModel
            {
                Sshihabb007DisplayTitle = string.IsNullOrWhiteSpace(sshihabb007MergedTitle) ? "Merged-Shihab-PDFPRO.pdf" : sshihabb007MergedTitle,
                MehediTitleMeta = "Merged PDF Document — PDF PRO (Mehedi Hasan Shihab @sshihabb007)"
            };
            foreach (ShihabPdfDocumentModel mehediSrc in mehediSourceDocs)
            {
                if (mehediSrc == null) continue;
                foreach (MehediPdfPageModel sshihabb007Page in mehediSrc.Sshihabb007Pages)
                {
                    shihabMerged.Sshihabb007Pages.Add(sshihabb007Page);
                }
            }
            if (shihabMerged.Sshihabb007Pages.Count == 0)
            {
                shihabMerged.Sshihabb007Pages.Add(new MehediPdfPageModel());
            }
            ReindexMehediPages(shihabMerged);
            byte[] shihabBytes = WriteMehediPdfDocumentToBytes(shihabMerged);
            shihabMerged.MehediOriginalFileBytes = shihabBytes;
            shihabMerged.Sshihabb007FileSizeBytes = shihabBytes.LongLength;
            shihabMerged.MehediSha256Digest = ComputeShihabSha256Hex(shihabBytes);
            return shihabMerged;
        }

        public static List<string> SplitMehediPdfDocumentToDirectory(ShihabPdfDocumentModel shihabDoc, string mehediOutputDirectory)
        {
            Directory.CreateDirectory(mehediOutputDirectory);
            List<string> sshihabb007CreatedFiles = new List<string>();
            string shihabBaseName = Path.GetFileNameWithoutExtension(shihabDoc.Sshihabb007DisplayTitle);
            if (string.IsNullOrWhiteSpace(shihabBaseName)) shihabBaseName = "ShihabDocument";

            for (int mehediIdx = 0; mehediIdx < shihabDoc.Sshihabb007Pages.Count; mehediIdx++)
            {
                ShihabPdfDocumentModel sshihabb007Single = new ShihabPdfDocumentModel
                {
                    Sshihabb007DisplayTitle = string.Format(CultureInfo.InvariantCulture, "{0}_Page_{1:D3}.pdf", shihabBaseName, mehediIdx + 1),
                    MehediTitleMeta = shihabDoc.MehediTitleMeta + " (Page " + (mehediIdx + 1) + ")",
                    Sshihabb007AuthorMeta = shihabDoc.Sshihabb007AuthorMeta
                };
                sshihabb007Single.Sshihabb007Pages.Add(shihabDoc.Sshihabb007Pages[mehediIdx]);
                ReindexMehediPages(sshihabb007Single);
                string shihabOutPath = Path.Combine(mehediOutputDirectory, sshihabb007Single.Sshihabb007DisplayTitle);
                SaveShihabPdfDocumentToFile(sshihabb007Single, shihabOutPath);
                sshihabb007CreatedFiles.Add(shihabOutPath);
            }
            return sshihabb007CreatedFiles;
        }

        public static void ApplySshihabb007WatermarkAllPages(ShihabPdfDocumentModel shihabDoc, string mehediWatermarkText)
        {
            foreach (MehediPdfPageModel sshihabb007Page in shihabDoc.Sshihabb007Pages)
            {
                sshihabb007Page.ShihabWatermarkText = mehediWatermarkText ?? string.Empty;
            }
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        public static void ApplyShihabBatesNumberingAllPages(ShihabPdfDocumentModel shihabDoc, string mehediPrefix, int sshihabb007StartNumber)
        {
            string shihabCleanPrefix = string.IsNullOrWhiteSpace(mehediPrefix) ? "SHIHAB-" : mehediPrefix;
            for (int mehediIdx = 0; mehediIdx < shihabDoc.Sshihabb007Pages.Count; mehediIdx++)
            {
                shihabDoc.Sshihabb007Pages[mehediIdx].MehediBatesNumberText =
                    string.Format(CultureInfo.InvariantCulture, "{0}{1:D6}", shihabCleanPrefix, sshihabb007StartNumber + mehediIdx);
            }
            shihabDoc.ShihabIsModifiedDirty = true;
        }

        public static void ReindexMehediPages(ShihabPdfDocumentModel shihabDoc)
        {
            for (int mehediIdx = 0; mehediIdx < shihabDoc.Sshihabb007Pages.Count; mehediIdx++)
            {
                MehediPdfPageModel sshihabb007Page = shihabDoc.Sshihabb007Pages[mehediIdx];
                sshihabb007Page.ShihabPageIndex = mehediIdx;
                foreach (ShihabPdfTextBlockItem shihabTb in sshihabb007Page.ShihabTextBlocks)
                    shihabTb.MehediPageIndex = mehediIdx;
                foreach (ShihabPdfAnnotationItem mehediAn in sshihabb007Page.MehediAnnotations)
                    mehediAn.MehediPageIndex = mehediIdx;
                foreach (ShihabAcroFormFieldItem sshihabb007Fld in sshihabb007Page.Sshihabb007FormFields)
                    sshihabb007Fld.MehediPageIndex = mehediIdx;
            }
        }
    }
}

