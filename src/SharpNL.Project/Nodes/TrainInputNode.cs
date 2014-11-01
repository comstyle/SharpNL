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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml;
using SharpNL.Formats.Ad;
using SharpNL.Project.Design;
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Project.Nodes {
    /// <summary>
    /// Represents a data node to be used during a model training.
    /// </summary>
    public class TrainInputNode : ProjectNode {

        public enum DataTypes {
            Unknown,
            PlainText,
            Ad,
            Brat,
            Muc,
        }

        public enum SampleTypes {
            None,
            SentenceSample,
            TokenSample
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainInputNode"/> class.
        /// </summary>
        public TrainInputNode() {
            Language = Thread.CurrentThread.CurrentCulture.Name;
            Encoding = Encoding.UTF8;
        }

        #region + Properties .

        #region . Encoding .

        private Encoding encoding = Encoding.UTF8;
        /// <summary>
        /// Gets or sets the data encoding. 
        /// </summary>
        /// <value>The data encoding. The default value is <see cref="System.Text.Encoding.UTF8"/>.</value>
        [Category("Data"), Description("The file encoding."), DefaultValue("utf-8")]
        [TypeConverter(typeof (EncodingConverter))]
        public Encoding Encoding {
            get { return encoding; }
            set {
                encoding = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . FileName .

        private string fileName;

        /// <summary>
        /// Gets the name of the data file.
        /// </summary>
        /// <value>The name of the data file.</value>
        [Category("Data"), Description("The file name of the data file."), Editor(typeof (FileNameEditor), typeof (UITypeEditor))]
        public string FileName {
            get { return fileName; }
            set {
                fileName = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . DataType .
        private DataTypes dataType = DataTypes.Unknown;
        /// <summary>
        /// Gets or sets the data type of the input.
        /// </summary>
        /// <value>The data type of the input.</value>
        [Category("Data"), Description("Defines the data type of the file."), DefaultValue(typeof(DataTypes), "Unknown")]
        public DataTypes DataType {
            get { return dataType; }
            set {
                dataType = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . DetokenizationDictionary .

        private string detokenizationDictionary;
        /// <summary>
        /// Gets or sets the detokenization dictionary in XML format.
        /// </summary>
        /// <value>The detokenization dictionary in XML format.</value>
        [Category("Data"), Description("The detokenization dictionary XML file."),
        Editor(typeof(XmlFileNameEditor), typeof(UITypeEditor))]
        public string DetokenizationDictionary {
            get { return detokenizationDictionary; }
            set {
                detokenizationDictionary = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }
        #endregion

        #region . IncludeHeadlines .
        private bool includeHeadlines = true;
        /// <summary>
        /// Gets or sets a value indicating whether output the sentences will be marked as news headlines.
        /// </summary>
        /// <value><c>true</c> if  output the sentences will be marked as news headlines; otherwise, <c>false</c>.</value>
        [Category("Data"), Description("Defines if the output the sentences will be marked as news headlines."), DefaultValue(true)]
        public bool IncludeHeadlines {
            get { return includeHeadlines; }
            set {
                includeHeadlines = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . Language .
        /// <summary>
        /// Gets the language of this data file.
        /// </summary>
        /// <value>The language of this data file.</value>
        [Category("Data"), Description("Specifies the language which is being processed."), TypeConverter(typeof (LanguageConverter))]
        public string Language { get; set; }

        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="TrainInputNode" />.
        /// </summary>
        /// <value>The output types of this <see cref="TrainInputNode" />.</value>
        [Browsable(false)]
        public override Type[] Output {
            get {
                switch (SampleType) {
                    case SampleTypes.SentenceSample:
                        return new[] { typeof(IObjectStream<SentenceSample>) };
                    case SampleTypes.TokenSample:
                        return new[] { typeof(IObjectStream<TokenSample>) };
                    default:
                        return new Type[] {};
                }               
            }
        }

        #endregion

        #region . SafeParse .

        private bool safeParse = true;

        /// <summary>
        /// Gets or sets a value indicating whether invalid data will be skipped.
        /// </summary>
        /// <value><c>true</c> if the invalid data will be skipped; otherwise, <c>false</c>.</value>
        [Category("Data"), Description("Defines if the invalid data will be skipped."), DefaultValue(true)]
        public bool SafeParse {
            get { return safeParse; }
            set {
                safeParse = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . SplitHyphenated .

        private bool splitHyphenatedTokens = true;

        /// <summary>
        /// Gets or sets a value indicating whether the hyphenated tokens will be separated: "carros-monstro" &gt; "carros" "-" "monstro".
        /// </summary>
        /// <value><c>true</c> if hyphenated tokens will be separated: "carros-monstro" &gt; "carros" "-" "monstro"; otherwise, <c>false</c>.</value>
        [Category("Tokens"), Description("Defines whether the hyphenated tokens will be separated: \"carros-monstro\" > \"carros\" \"-\" \"monstro\"."), DefaultValue(true)]
        public bool SplitHyphenatedTokens {
            get {
                return splitHyphenatedTokens;                
            }
            set {
                splitHyphenatedTokens = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . SampleType .
        private SampleTypes sampleType = SampleTypes.None;

        /// <summary>
        /// Gets or sets the type of the sample.
        /// </summary>
        /// <value>The type of the sample.</value>
        [Category("Data"), Description("Describes the output sample type."), DefaultValue(typeof(SampleTypes), "None")]
        public SampleTypes SampleType {
            get { return sampleType; }
            set {
                sampleType = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
            
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

            var att = xmlNode.Attributes["File"];
            if (att != null)
                FileName = att.Value;

            att = xmlNode.Attributes["Lang"];
            if (att != null)
                Language = att.Value;

            DataTypes dt;
            att = xmlNode.Attributes["DataType"];
            if (att != null && Enum.TryParse(att.Value, out dt)) {
                DataType = dt;
            }

            SampleTypes st;
            att = xmlNode.Attributes["Sample"];
            if (att != null && Enum.TryParse(att.Value, out st)) {
                SampleType = st;
            }

            att = xmlNode.Attributes["Encoding"];
            if (att != null) {
                Encoding = Encoding.GetEncoding(att.Value);
            }

            att = xmlNode.Attributes["DetokenizationDictionary"];
            if (att != null) {
                DetokenizationDictionary = att.Value;
            }

            att = xmlNode.Attributes["SplitHyphenatedTokens"];
            if (att != null && !bool.TryParse(att.Value, out splitHyphenatedTokens))
                splitHyphenatedTokens = true; // default value if not successfully parsed  

        }

        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var list = new List<ProjectProblem>();

            if (string.IsNullOrEmpty(FileName))
                list.Add(new ProjectProblem(this, "The filename is not specified."));
            else if (!File.Exists(FileName))
                list.Add(new ProjectProblem(this, "The specified file does not exist."));
            
            if (DataType == DataTypes.Unknown)
                list.Add(new ProjectProblem(this, "The data type is not specified."));

            if (SampleType == SampleTypes.None)
                list.Add(new ProjectProblem(this, "The sample type is not specified."));

            if (DataType == DataTypes.Ad && SampleType == SampleTypes.TokenSample) {               
                if (string.IsNullOrEmpty(detokenizationDictionary))
                    list.Add(new ProjectProblem(this, "The detokenization dictionary is required to create token samples."));
                else if (!File.Exists(detokenizationDictionary))
                    list.Add(new ProjectProblem(this, "The detokenization dictionary does not exist."));
            }


            return list.Count > 0 ? list.ToArray() : null;
        }

        #endregion

        #region . Prepare .

        protected override object[] Prepare() {
            var fileInfo = new FileInfo(FileName);
            if (!fileInfo.Exists)
                throw new FileNotFoundException("File not found!", FileName);

            if (DataType == DataTypes.Unknown)
                throw new InvalidOperationException("The data type is not specified.");

            if (SampleType == SampleTypes.None)
                throw new InvalidOperationException("The sample type is not specified.");


            StreamReader reader;
            switch (SampleType) {
                case SampleTypes.SentenceSample:                    
                    switch (DataType) {
                        case DataTypes.PlainText:
                            reader = new StreamReader(fileInfo.OpenRead(), Encoding);
                            return new object[] { new SentenceSampleStream(new PlainTextByLineStream(reader)) };
                        case DataTypes.Ad:
                            reader = new StreamReader(fileInfo.OpenRead(), Encoding);
                            return new object[] {
                                new Formats.Ad.AdSentenceSampleStream(
                                    new PlainTextByLineStream(reader), includeHeadlines, safeParse, Project.Monitor)
                            };
                        default:
                            throw new NotSupportedException("The data type " + dataType + " is not supported as a sentence sample stream.");
                    }
                case SampleTypes.TokenSample:
                    switch (DataType) {
                        case DataTypes.PlainText:
                            reader = new StreamReader(fileInfo.OpenRead(), Encoding);
                            return new object[] { new TokenSampleStream(new PlainTextByLineStream(reader)) };
                        case DataTypes.Ad:
                            if (string.IsNullOrEmpty(detokenizationDictionary))
                                throw new InvalidOperationException("The detokenization dictionary is not specified.");

                            if (!File.Exists(detokenizationDictionary))
                                throw new FileNotFoundException("The detokenization dictionary does not exist.", detokenizationDictionary);

                            var dict = new DetokenizationDictionary(File.OpenRead(detokenizationDictionary));

                            reader = new StreamReader(fileInfo.OpenRead(), Encoding);

                            return new object[] {
                                new AdTokenSampleStream(
                                    new PlainTextByLineStream(reader), 
                                    new DictionaryDetokenizer(dict),
                                    splitHyphenatedTokens, 
                                    safeParse)
                            };
                        default:
                            throw new NotSupportedException("The data type " + dataType + " is not supported as a token sample stream.");                           
                    }
                default:
                    throw new InvalidOperationException();
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
            writer.WriteAttributeString("File", FileName);
            writer.WriteAttributeString("Lang", Language);
            writer.WriteAttributeString("DataType", DataType.ToString());
            writer.WriteAttributeString("Sample", SampleType.ToString());

            if (Encoding != null && Encoding.HeaderName != "utf-8") {
                writer.WriteAttributeString("Encoding", Encoding.HeaderName);
            }

            if (detokenizationDictionary != null) {
                writer.WriteAttributeString("DetokenizationDictionary", detokenizationDictionary);
            }

            if (!splitHyphenatedTokens) {
                writer.WriteAttributeString("SplitHyphenatedTokens", "false");
            }


        }

        #endregion

    }
}