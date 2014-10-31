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
        private readonly Monitor monitor;

        public AdSentenceStream(IObjectStream<string> samples) : base(samples) { }

        public AdSentenceStream(IObjectStream<string> samples, Monitor monitor) : base(samples) {
            this.monitor = monitor;
        }
        public AdSentenceStream(IObjectStream<string> samples, bool safeParse) : base(samples) {
            this.safeParse = safeParse;
        }
        public AdSentenceStream(IObjectStream<string> samples, bool safeParse, Monitor monitor) : base(samples) {
            this.safeParse = safeParse;
            this.monitor = monitor;
        }

        /// <summary>
        /// Returns the valid <see cref="AdSentence"/> object. 
        /// </summary>
        /// <returns>
        /// Calling this method repeatedly until it returns, <c>null</c> will return each object from the underlying source exactly once.
        /// </returns>
        public override AdSentence Read() {
            
            retry:

            var sb = new StringBuilder();
            var sentenceStarted = false;

            while (true) {
                AdSentence sentence;
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
                        if (AdSentenceParser.TryParse(out sentence, sb.ToString(), paraID, isTitle, isBox, safeParse, monitor)) {
                            return sentence;
                        }

                        goto retry; // invalid sentence, so we try again...
                    }
                } else {
                    // handle end of file
                    if (sentenceStarted && sb.Length > 0 && AdSentenceParser.TryParse(out sentence, sb.ToString(), paraID, isTitle, isBox, safeParse, monitor)) {
                        return sentence;
                    }
                    return null;
                }
            }
        }
    }
}
