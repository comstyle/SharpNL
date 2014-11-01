// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;
using SharpNL.Project.Design;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Project.Nodes {
    /// <summary>
    /// Represents a HTTP GET method into a specified website.
    /// </summary>
    public class WebGetNode : ProjectNode {
        private const char Space = ' ';
        private string html;
        private string text;

        public WebGetNode() {
            Clean = true;
        }

        #region . Clean .
        /// <summary>
        /// Gets or sets a value indicating whether text from the page should be cleaned.
        /// </summary>
        /// <value><c>true</c> if should be cleaned; otherwise, <c>false</c>.</value>
        /// <remarks>The clean process removes multiple new lines and tons of spaces.</remarks>
        [DefaultValue(true)]
        public bool Clean { get; set; }
        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="WebGetNode" />.
        /// </summary>
        /// <value>The output types of this <see cref="WebGetNode" />.</value>
        public override Type[] Output {
            get { return new[] {typeof (Document)}; }
        }

        #endregion

        #region . Language .
        /// <summary>
        /// Gets or sets the website language.
        /// </summary>
        /// <value>The website language.</value>
        [Description("Specifies the language which is being processed."), TypeConverter(typeof(LanguageConverter))]
        public string Language { get; set; }
        #endregion


        public Uri URL { get; set; }

        /// <summary>
        /// Prepares for execution.
        /// </summary>
        protected override object[] Prepare() {
            var request = (HttpWebRequest)WebRequest.Create(URL);

            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var response = (HttpWebResponse) request.GetResponse();

            html = null;
            text = null;

            using (var stream = response.GetResponseStream()) {
                if (stream != null) {
                    using (var reader = new StreamReader(stream)) html = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrWhiteSpace(html))
                throw new WebException("Unable to read the remote source.");

            if (string.IsNullOrEmpty(Language)) {
                var value = html.RegExMatch(@"<html.+?\s*lang\s*=\s*[""\']?([^\""\'\s>]+)[\""\']?");
                if (!string.IsNullOrEmpty(value)) {
                    Language = value;
                }
            }

            text = Convert(html);
 
            if (Clean)
                text = CleanUp(text);

            if (string.IsNullOrEmpty(Language)) {
                throw new InvalidOperationException("Unable to determine the language of the website.");
            }

            return new object[] { new Document(Language, text) };
        }


        #region . Convert .
        private static string Convert(string html) {

            // Knuppe:
            //
            // Ok, I have some options here, the best solution is to use HtmlAgilityPack but honestly 
            // I don't want any additional library...
            //
            // So I implement the innerText as follows:
            //  1. Try to use a Windows HTML handler to decode the html... This thing was
            //     implemented by MS in IE 5.5 and probably they will never remove, 
            //     and does not matter if the user remove IE, the DLL is part of the window$
            //  2. If the previous solution fails, we try a manually decoding process.
            //
            // I'm sure this should work in almost all windows, other platforms we can implement 
            // something better later :)
           

            var text = string.Empty;

            var type = Type.GetTypeFromProgID("htmlfile"); // safe alias for HTMLDocumentClass
            if (type != null) {
                dynamic doc = null;
                try {
                    doc = Activator.CreateInstance(type);
                    doc.open();
                    doc.write(html);
                    doc.close();

                    text = doc.body.innerText;
                } catch {
                    // I don't care :)
                } finally {
                    if (doc != null)
                        Marshal.ReleaseComObject(doc);
                }
            }

            if (string.IsNullOrEmpty(text))
                text = HtmlToString(text);           

            return !string.IsNullOrWhiteSpace(text) ? text : null;
        }
        #endregion

        #region . CleanUp .

        /// <summary>
        /// Performs necessary cleanup of the text.
        /// </summary>
        /// <param name="text">The text to be clean.</param>
        /// <returns>The cleaned string.</returns>
        private static string CleanUp(string text) {

            // remove multiple spaces
            text = Regex.Replace(text, @"[ ]+", " ");

            // group new lines
            text = Regex.Replace(text, @"(\r|\n)[ ]+(\r|\n)", "$1$2");

            // remove redundant tabs
            text = Regex.Replace(text, "[\r][\t]+[\r]", "\r\r", RegexOptions.IgnoreCase);

            // more then two new lines into one
            text = Regex.Replace(text, @"([\r]{2,}|[\n]{2,})", "$1");

            return text;
        }

        #endregion

        #region . HtmlToString .

        private static string HtmlToString(string html) {
            try {
                string result = html.Replace('\r', Space).Replace('\n', Space).Replace("\t", string.Empty);

                result = Regex.Replace(result,@"[ ]+", " ");

                // keep the title tag
                var title = result.RegExMatch(@"<[ ]*title[^>]*>(.*)<\/title[ ]*>");

                // Remove the header
                result = Regex.Replace(result, @"<[ ]*head[^>]*>.*<\/head[ ]*>", string.Empty, RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<[ ]*script[^>]*>.*<\/script[ ]*>", string.Empty, RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<[ ]*style[^>]*>.*<\/style[ ]*>", string.Empty, RegexOptions.IgnoreCase);

                // td -> tab
                result = Regex.Replace(result, @"<[ ]*td[^>]*>", "\t", RegexOptions.IgnoreCase);

                // br, li -> \r
                result = Regex.Replace(result, @"<[ ]*br[ ]*[\/]*>", "\r", RegexOptions.IgnoreCase);
                result = Regex.Replace(result, @"<[ ]*li[ ]*>", "\r", RegexOptions.IgnoreCase);

                // p, div, tr -> double line breaks
                result = Regex.Replace(result, @"<[ ]*(div|tr|p)([^>])*>", "\r\r", RegexOptions.IgnoreCase);

                // remove remaining tags 
                result = Regex.Replace(result, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);

                // replace non-breaking space
                result = Regex.Replace(result, @"\u00A0", " ", RegexOptions.IgnoreCase);

                // decode html characters
                return System.Web.HttpUtility.HtmlDecode(title + result);
            } catch {
                return html;
            }
        }

        #endregion

        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            if (URL == null)
                return new[] { new ProjectProblem(this, "The URL is not specified.") };

            if (!URL.IsAbsoluteUri || !URL.Scheme.In(Uri.UriSchemeHttp, Uri.UriSchemeHttps))
                return new[] { new ProjectProblem(this, "The URL is invalid.") };

            return null;
        }

        /// <summary>
        /// Deserializes the node from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="xmlNode">The node.</param>
        protected override void Deserialize(XmlNode xmlNode) {
            if (xmlNode.Attributes == null)
                throw new InvalidFormatException("The node has no attributes.");

            var attClean = xmlNode.Attributes["clean"];
            if (attClean != null)
                Clean = attClean.Value == "true";

            var attLang = xmlNode.Attributes["lang"];
            if (attLang != null)
                Language = attLang.Value;

            var attUrl = xmlNode.Attributes["url"];
            if (attUrl != null)
                URL = new Uri(attUrl.Value);
        }

        protected override void SerializeProjectNode(XmlWriter writer, bool content) {
            writer.WriteAttributeString("url", URL.ToString());
            writer.WriteAttributeString("clean", Clean ? "true" : "false");
            writer.WriteAttributeString("lang", Language);
        }
    }
}