using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PluginScribens.Common.ExtensionMethods
{
    public static class DocumentExtensions
    {
        public static string GetText(this Document document)
        {
            try
            {
                string text = "";
                foreach (Paragraph paragraph in document.Paragraphs)
                {
                    var content = paragraph.Range.Text.RemoveSpecialCharacters();
                    if (!string.IsNullOrEmpty(content))
                        text += $"<p>{content}</p>";

                    Marshal.ReleaseComObject(paragraph);
                }

                return text;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public static Tuple<Range, int> GetRangeInParagraph(this Document document, int paragraphIndex, int start, int end)
        {
            try
            {
                Range rangeP = document.Paragraphs[paragraphIndex].Range;
                int pStart = rangeP.Start;
                int oStart = pStart + start;
                int oEnd = pStart + end;

                // It can have hyperlinks before. They have in general a range of lenght superior to the text of the hyperlink.
                Hyperlinks hls = rangeP.Hyperlinks;

                for (int i = 1; i <= hls.Count; i++)
                {
                    object index = i;
                    Hyperlink hyperlink = hls.get_Item(ref index);

                    //if (hyperlink.TextToDisplay.Length > 0)   // As if textToDisplay == 0, strangely, it crashes the app ???
                    {
                        Range rangeHl = hyperlink.Range;
                        int startH = rangeHl.Start;
                        int endH = rangeHl.End;
                        int lengthH = (endH - startH);

                        int lgthStLink = hyperlink.TextToDisplay.Length;

                        if (oStart > startH)
                        {
                            // If the solution is integrated in the hyperlink, decrease of 1. Why???
                            Boolean isInsideHyperlink = false;
                            if (oEnd <= (startH + lgthStLink))
                            {
                                isInsideHyperlink = true;
                            }

                            oStart = oStart + (lengthH - lgthStLink);
                            oEnd = oEnd + (lengthH - lgthStLink);

                            if (isInsideHyperlink == true)
                            {
                                oStart = oStart - 1;
                                oEnd = oEnd - 1;
                            }
                        }
                    }
                }

                return new Tuple<Range, int>(document.Range(oStart, oEnd), oStart);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        public static List<ParagraphInfo> GetParagraphs(this Document document)
        {
            return document.Content.Text.ToParagraphs();
        }

        public static Tuple<List<ParagraphInfo>, int, int> GetDiffs(this Document document, List<ParagraphInfo> target)
        {
            List<ParagraphInfo> listParagraphsInfo = new List<ParagraphInfo>();

            var paragraphs = document.GetParagraphs();
            
            var nbPar = paragraphs.Count;
            var nbPar_Prec = target.Count;

            var indStartM = -1;
            var indEndM = nbPar;

            // 1. Search for the first indM
            var i = 0;
            for (i = 0; i < nbPar; i++)
            {
                if (i < nbPar_Prec)
                {
                    var textP = paragraphs[i].Text;
                    var textP_Prec = target[i].Text;

                    if (!textP.Equals(textP_Prec))
                    {
                        indStartM = i + 1;
                        break;
                    }
                }
                else
                {
                    indStartM = i + 1;
                    break;
                }
            }

            // 2. Search for the last indM
            if (nbPar_Prec > 0)
            {
                for (i = (nbPar - 1); i >= 0; i--)
                {
                    var indParPrec = (nbPar_Prec - 1) - ((nbPar - 1) - i);
                    if (indParPrec >= 0)
                    {
                        var textP1 = paragraphs[i].Text;
                        var textP_Prec1 = target[indParPrec].Text;

                        if (!textP1.Equals(textP_Prec1))
                        {
                            indEndM = i + 1;

                            break;
                        }
                    }
                    else break;
                }

                // Case of removing first line.
                // Ex : Il manges.        ->    Il arrives.
                //      Il arrives.             Il marches.
                //      Il marches.             Il lances.
                //      Il lances.
                if (i == -1) indEndM = -1;
            }

            int indPSupp = -1;

            // Normal case
            if (indStartM != -1 && indEndM != -1 && indEndM >= indStartM)
            {
                for (var ind = indStartM; ind <= indEndM; ind++)
                {
                    listParagraphsInfo.Add(paragraphs[ind - 1]);
                }
            }

            // Removing case without modyfing lines.
            // Suppression case.
            // Ex : Il manges       ->      Il manges       -> 2, 1 -> 2, 2
            //      Il arrives              Il marches
            //      Il marches              Il lances.
            //      Il lances.
            if (indEndM < indStartM)
            {
                indPSupp = indStartM;
            }

            // Difference fo number of paragraphs
            int diffNumberPar = nbPar - nbPar_Prec;

            return new Tuple<List<ParagraphInfo>, int, int>(listParagraphsInfo, indPSupp, diffNumberPar);
        }

        public static Tuple<List<ParagraphInfo>, int, int> GetDiffs(this Document document, string snapshot)
        {
            var snapshotParagraphs = snapshot.ToParagraphs();
            return document.GetDiffs(snapshotParagraphs);
        }

    }
}
