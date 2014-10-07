using System.Text;
using System.Text.RegularExpressions;
using SharpNL.Utility;

namespace SharpNL.Formats.Ad {
    internal class AdSentenceStream : FilterObjectStream<string, AdSentence> {

        private static readonly Regex sentStart;
        private static readonly Regex sentEnd;
        private static readonly Regex extEnd;
        private static readonly Regex titleStart;
        private static readonly Regex titleEnd;
        private static readonly Regex boxStart;
        private static readonly Regex boxEnd;
        private static readonly Regex paraStart;
        private static readonly Regex textStart;

        static AdSentenceStream() {

            sentStart = new Regex("<s[^>]*>");
            sentEnd = new Regex("</s>");
            extEnd = new Regex("</ext>");
            titleStart = new Regex("<t[^>]*>");
            titleEnd = new Regex("</t>");
            boxStart = new Regex("<caixa[^>]*>");
            boxEnd = new Regex("</caixa>");
            paraStart = new Regex("<p[^>]*>");
            textStart = new Regex("<ext[^>]*>");

        }

        private int paraID;
        private bool isTitle;
        private bool isBox;

        private readonly bool safeParse;

        public AdSentenceStream(IObjectStream<string> samples, bool safeParse) : base(samples) {
            this.safeParse = safeParse;
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override AdSentence Read() {

            var sb = new StringBuilder();
            var sentenceStarted = false;

            while (true) {
                var line = Samples.Read();
                if (line != null) {

                    if (sentenceStarted) {
                        if (sentEnd.IsMatch(line) || extEnd.IsMatch(line)) {
                            sentenceStarted = false;
                        } else if (line.StartsWith("A1")) {
                            // skip
                        } else {
                            sb.AppendLine(line);
                        }
                    } else {
                        if (sentStart.IsMatch(line)) {
                            sentenceStarted = true;
                        } else if (paraStart.IsMatch(line)) {
                            paraID++;
                        } else if (titleStart.IsMatch(line)) {
                            isTitle = true;
                        } else if (titleEnd.IsMatch(line)) {
                            isTitle = false;
                        } else if (textStart.IsMatch(line)) {
                            paraID = 0;
                        } else if (boxStart.IsMatch(line)) {
                            isBox = true;
                        } else if (boxEnd.IsMatch(line)) {
                            isBox = false;
                        }
                    }

                    if (!sentenceStarted && sb.Length > 0) {
                        return AdSentenceParser.Parse(sb.ToString(), paraID, isTitle, isBox, safeParse);
                    }
                } else {
                    // handle end of file
                    if (sentenceStarted && sb.Length > 0) {
                        return AdSentenceParser.Parse(sb.ToString(), paraID, isTitle, isBox, safeParse);
                    }
                    return null;
                }
            }
        }
    }
}
