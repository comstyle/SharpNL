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
using System.Linq;
using System.Threading;
using System.Windows.Forms.Design;
using System.Xml;
using SharpNL.Chunker;
using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Parser;
using SharpNL.POSTag;
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility.Model;

namespace SharpNL.Project.Tasks {
    public class ModelWriterTask : ProjectTask {

        public enum ModelType {
            None = 0,
            Chunker,
            DocumentCategorizer,
            NameFinder,
            Parser,
            PoS,
            SentenceDetector,           
            Tokenizer,           
        }

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTask"/>.
        /// </summary>
        public ModelWriterTask() : base(99) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelWriterTask"/> class with the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        public ModelWriterTask(Project project) : base(99) {
            Project = project;
        }
        #endregion

        #region + Properties .

        #region . Type .
        private ModelType modelType = ModelType.None;

        /// <summary>
        /// Gets or sets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        /// <exception cref="System.ArgumentException"></exception>
        [Category("Model"), Description("Specified the type of the model to be saved.")]
        public ModelType Type {
            get { return modelType; }
            set {
                if (!Enum.IsDefined(typeof(ModelType), value))
                    throw new ArgumentException();

                modelType = value;

                // if the filename is not specified we automatically define with the default.
                if (string.IsNullOrEmpty(fileName) && Project != null && !Project.IsLoading) {

                    var lang = Parent.GetProperty("Language", Thread.CurrentThread.CurrentCulture.Name);

                    // remove region code
                    if (lang.Contains("/"))
                        lang = lang.Substring(0, lang.IndexOf("/", StringComparison.Ordinal));

                    switch (Type) {
                       case ModelType.Chunker:
                            fileName = string.Format("{0}-chunker.bin", lang);
                            break;
                        case ModelType.DocumentCategorizer:
                            fileName = string.Format("{0}-doccat.bin", lang);
                            break;
                        case ModelType.NameFinder:
                            fileName = string.Format("{0}-namefind.bin", lang);
                            break;
                        case ModelType.Parser:
                            fileName = string.Format("{0}-parser.bin", lang);
                            break;
                        case ModelType.PoS:
                            fileName = string.Format("{0}-pos.bin", lang);
                            break;
                        case ModelType.SentenceDetector:
                            fileName = string.Format("{0}-sent.bin", lang);
                            break;
                        case ModelType.Tokenizer:
                            fileName = string.Format("{0}-token.bin", lang);
                            break;
                    }
                }

                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #region . FileName .

        private string fileName;
        /// <summary>
        /// Gets or sets the filename of the model.
        /// </summary>
        /// <value>The filename of the model.</value>
        [Category("Model"), Description("The model filename."),
        EditorAttribute(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string FileName {
            get { return fileName; }
            set {
                fileName = value;
                Project.IsDirty = true;
            }
        }
        #endregion

        #endregion

        #region . Execute .
        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected override object[] Execute() {
            if (string.IsNullOrEmpty(fileName))
                throw new InvalidOperationException("The filename is not specified.");

            Type type;
            switch (Type) {
                case ModelType.Chunker:
                    type = typeof (ChunkerModel);
                    break;
                case ModelType.DocumentCategorizer:
                    type = typeof(DocumentCategorizerModel);
                    break;
                case ModelType.NameFinder:
                    type = typeof(TokenNameFinderModel);
                    break;
                case ModelType.Parser:
                    type = typeof(ParserModel);
                    break;
                case ModelType.PoS:
                    type = typeof(POSModel);
                    break;
                case ModelType.SentenceDetector:
                    type = typeof(SentenceModel);
                    break;
                case ModelType.Tokenizer:
                    type = typeof(TokenizerModel);
                    break;
                default:
                    throw new InvalidOperationException("The model type is not specified or is invalid.");
            }

            var model = Parent.Outputs
                .Where(output => output != null && output.GetType() == type)
                .Cast<BaseModel>()
                .FirstOrDefault();

            if (model == null)
                throw new TaskException(this, "Unable to retrieve the model from the " + Parent.GetType().Name + " node.");

            using (var writer = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                model.Serialize(writer); 
            }

            return new object[] { };
        }
        #endregion

        #region . DeserializeTask .

        /// <summary>
        /// Deserializes the task from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="node">The xml node.</param>
        protected override void DeserializeTask(XmlNode node) {
            if (node.Attributes == null)
                return;

            var att = node.Attributes["ModelType"];
            if (att != null && !Enum.TryParse(att.Value, true, out modelType))
                modelType = ModelType.None;
               
        }

        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var list = new List<ProjectProblem>();

            if (Type == ModelType.None)
                list.Add(new ProjectProblem(this, "The model type is not specified."));

            if (string.IsNullOrEmpty(fileName))
                list.Add(new ProjectProblem(this, "The model filename os not specified."));

            return list.Count > 0
                ? list.ToArray()
                : null;
        }
        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="ModelWriterTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="ModelWriterTask"/>.</value>
        public override Type[] Output {
            get { return null; }
        }

        #endregion

        #region . Input .

        /// <summary>
        /// Gets the input types of this <see cref="ModelWriterTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="ModelWriterTask"/>.</value>
        [Browsable(false)]
        public override Type[] Input {
            get {
                switch (Type) {
                    case ModelType.Chunker:
                        return new[] {typeof (ChunkerModel)};
                    case ModelType.DocumentCategorizer:
                        return new[] {typeof (DocumentCategorizerModel)};
                    case ModelType.NameFinder:
                        return new[] {typeof (TokenNameFinderModel)};
                    case ModelType.Parser:
                        return new[] {typeof (ParserModel)};
                    case ModelType.PoS:
                        return new[] {typeof (POSModel)};
                    case ModelType.SentenceDetector:
                        return new[] {typeof (SentenceModel)};
                    case ModelType.Tokenizer:
                        return new[] {typeof (TokenizerModel)};
                    default:
                        // everything, otherwise the we can't add in the project using the gui
                        return new[] {
                            typeof (ChunkerModel),
                            typeof (DocumentCategorizerModel),
                            typeof (TokenNameFinderModel),
                            typeof (ParserModel),
                            typeof (POSModel),
                            typeof (TokenizerModel),
                            typeof (SentenceModel)
                        };
                }

            }
        }

        #endregion

        #region . SerializeTask .
        protected override void SerializeTask(XmlWriter writer) {
            writer.WriteAttributeString("ModelType", Type.ToString());
        }
        #endregion

    }
}