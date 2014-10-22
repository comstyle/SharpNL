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
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using SharpNL.Project.Design;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Project.Nodes {
    /// <summary>
    /// Represents a plain text file project node.
    /// </summary>
    public class TextFileNode : ProjectNode {
        public TextFileNode() {
            Language = Thread.CurrentThread.CurrentCulture.Name;
            Encoding = Encoding.UTF8;
        }

        #region + Properties .

        #region . Encoding .

        /// <summary>
        /// Gets or sets the file encoding. 
        /// </summary>
        /// <value>The file encoding. The default value is <see cref="System.Text.Encoding.UTF8"/>.</value>
        [Description("The file encoding."), DefaultValue(typeof(Encoding), "System.Text.UTF8Encoding")]
        [TypeConverter(typeof(EncodingConverter))]
        public Encoding Encoding { get; set; }

        #endregion

        #region . FileName .
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        [Description("The file name of the text file."), 
        EditorAttribute(typeof(System.Windows.Forms.Design.FileNameEditor), typeof(UITypeEditor))]
        public string FileName { get; set; }

        #endregion

        #region . Language .
        /// <summary>
        /// Gets the language of this file.
        /// </summary>
        /// <value>The language of this file.</value>
        [TypeConverter(typeof(LanguageConverter))]
        public string Language { get; set; }
        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="StreamReader" />.
        /// </summary>
        /// <value>The output types of this <see cref="StreamReader" />.</value>
        public override Type[] Output {
            get { return new[] {typeof (StreamReader)}; }
        }

        #endregion

        #endregion

        #region . Deserialize .
        /// <summary>
        /// Deserializes the node from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="xmlNode">The xml node.</param>
        protected override void Deserialize(XmlNode xmlNode) {
            if (xmlNode.Attributes == null)
                throw new InvalidFormatException("The node has no attributes.");

            var attFile = xmlNode.Attributes["file"];
            if (attFile != null)
                FileName = attFile.Value;

            var attLang = xmlNode.Attributes["lang"];
            if (attLang != null)
                Language = attLang.Value;

            var attEnc = xmlNode.Attributes["encoding"];
            if (attEnc != null) {
                Encoding = Encoding.GetEncoding(attEnc.Value);
            }
        }
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            if (string.IsNullOrEmpty(FileName))
                return new[] { new ProjectProblem(this, "The filename is not specified.") };

            if (!File.Exists(FileName))
                return new[] { new ProjectProblem(this, "The specified file does not exist.") };

            return null;
        }
        #endregion

        #region . Prepare .
        protected override object[] Prepare() {
            var fileInfo = new FileInfo(FileName);
            if (!fileInfo.Exists)
                throw new FileNotFoundException("File not found!", FileName);

            
            using (var reader = new StreamReader(fileInfo.OpenRead(), Encoding)) {
                return new object[] { new Document(Language, reader.ReadToEnd()) };
            }
        }
        #endregion

        #region . SerializeNode .
        /// <summary>
        /// Serializes the content of the node.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter" /> object to used to serialize the node.</param>
        /// <param name="content">Determines whether the content should be saved.</param>
        protected override void SerializeProjectNode(XmlWriter writer, bool content) {
            writer.WriteAttributeString("file", FileName);
            writer.WriteAttributeString("lang", Language);

            if (Encoding != null && Encoding.HeaderName != "utf-8") {
                writer.WriteAttributeString("encoding", Encoding.HeaderName);
            }
        }
        #endregion

    }
}