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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using SharpNL.Project.Design;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Project.Tasks {
    /// <summary>
    /// Represents a tokenizer train task.
    /// </summary>
    public class TokenizerTrainTask : TrainTask {

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerTrainTask"/> class.
        /// </summary>
        public TokenizerTrainTask() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerTrainTask"/> class assign 
        /// the specified <see cref="Project"/>.
        /// </summary>
        /// <param name="project">The project.</param>
        public TokenizerTrainTask(Project project) {
            Project = project;
        }
        #endregion

        #region . Properties .

        #region . AbbDictionary .

        private string abbDictionary;
        /// <summary>
        /// Gets or sets the abbreviation dictionary in XML format.
        /// </summary>
        /// <value>The abbreviation dictionary in XML format.</value>
        [Category("Training"), Description("The abbreviation dictionary XML file."),
        Editor(typeof(XmlFileNameEditor), typeof(UITypeEditor))]
        public string AbbDictionary {
            get { return abbDictionary; }
            set {
                abbDictionary = value;
                Project.IsDirty = true;
            }
        }
        #endregion

        #region . UseAlphaNumericOptimization .
        private bool useAlphaNumericOptimization;

        /// <summary>
        /// Gets or sets a value indicating whether to use alpha numeric optimization.
        /// </summary>
        /// <value><c>true</c> if alpha numeric optimization will be used; otherwise, <c>false</c>.</value>
        [Category("Training"), Description("Indicates whether to skip alpha numeric tokens for further tokenization."), DefaultValue(false)]
        public bool UseAlphaNumericOptimization {
            get { return useAlphaNumericOptimization; }
            set {
                useAlphaNumericOptimization = value;
                Project.IsDirty = true;
            }
        }
        #endregion

        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="TokenizerTrainTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="TokenizerTrainTask"/>.</value>
        public override Type[] Output {
            get { return new[] { typeof(TokenizerModel) }; }
        }
        #endregion

        #region . Input .
        /// <summary>
        /// Gets the input types of this <see cref="TokenizerTrainTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="TokenizerTrainTask"/>.</value>
        public override Type[] Input {
            get {
                return new[] {
                    typeof (TokenSampleStream),
                    typeof (IObjectStream<TokenSample>),
                    typeof (IObjectStream<string>),
                    typeof (StreamReader)
                };
            }
        }
        #endregion

        #region . SerializeTask .
        /// <summary>
        /// Serializes the train task.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeTask(XmlWriter writer) {
            base.SerializeTask(writer);

            writer.WriteAttributeString("AbbDict", AbbDictionary);
            writer.WriteAttributeString("AlphaNumOpt", useAlphaNumericOptimization ? "true" : "false");
        }
        #endregion

        #region . DeserializeTask .
        /// <summary>
        /// Deserializes the task from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="node">The xml node.</param>
        protected override void DeserializeTask(XmlNode node) {
            base.DeserializeTask(node);

            if (node.Attributes == null)
                return;

            var att = node.Attributes["AbbDict"];
            if (att != null) 
                abbDictionary = att.Value;

            att = node.Attributes["AlphaNumOpt"];
            if (att != null)
                useAlphaNumericOptimization = att.Value == "true";

        }
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this training task.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var probs = base.GetProblems();

            var list = probs == null
                ? new List<ProjectProblem>()
                : new List<ProjectProblem>(probs);

            if (AbbDictionary != null && !File.Exists(AbbDictionary)) {
                list.Add(new ProjectProblem(this, "The abbreviation dictionary file does not exist."));
            }

            return list.Count > 0
                ? list.ToArray()
                : null;
        }
        #endregion

        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected override object[] Execute() {
            Dictionary.Dictionary ad = null;
            if (!string.IsNullOrEmpty(AbbDictionary)) {
                if (!File.Exists(AbbDictionary))
                    throw new FileNotFoundException("The abbreviation dictionary file does not exist.");

                try {

                    using (var file = new FileStream(AbbDictionary, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                        ad = new Dictionary.Dictionary(file);
                    }

                } catch (Exception ex) {
                    throw new TaskException(this, "Unable load the abbreviation dictionary file.", ex);
                }
            }

            IObjectStream<TokenSample> stream = null;
            if (Parent != null) {

                if (Parent.Output.Contains(typeof (TokenSampleStream))) {
                    stream = Parent.GetOutput<TokenSampleStream>();
                } else if (Parent.Output.Contains(typeof (IObjectStream<TokenSample>))) {
                    stream = Parent.GetOutput<IObjectStream<TokenSample>>();
                } else if (Parent.Output.Contains(typeof (IObjectStream<string>))) {
                    var stringStream = Parent.GetOutput<IObjectStream<string>>();
                    if (stringStream == null)
                        throw new NotSupportedException("Unable to retrieve the string stream from the parent node.");

                    stream = new TokenSampleStream(stringStream);
                } else if (Parent.Output.Contains(typeof (StreamReader))) {
                    var reader = Parent.GetOutput<StreamReader>();

                    stream = new TokenSampleStream(new PlainTextByLineStream(reader));
                }
            }

            if (stream == null)
                throw new NotSupportedException("Unable to retrieve the sample stream from the parent node.");

            var parameters = TrainingParameters.DefaultParameters();

            switch (Algorithm) {
                case Algorithms.Perceptron:
                    parameters.Set(Parameters.Algorithm, Parameters.Algorithms.Perceptron);
                    break;
                default:
                    parameters.Set(Parameters.Algorithm, Parameters.Algorithms.MaxEnt);
                    break;
            }

            parameters.Set(Parameters.Cutoff, Cutoff.ToString(CultureInfo.InvariantCulture));
            parameters.Set(Parameters.Iterations, Iterations.ToString(CultureInfo.InvariantCulture));
            parameters.Set(Parameters.Threads, Threads.ToString(CultureInfo.InvariantCulture));

            switch (DataIndexer) {
                case DataIndexers.OnePass:
                    parameters.Set(Parameters.DataIndexer, Parameters.DataIndexers.OnePass);
                    break;
                case DataIndexers.TwoPass:
                    parameters.Set(Parameters.DataIndexer, Parameters.DataIndexers.TwoPass);
                    break;
                default:
                    throw new InvalidOperationException("Invalid data indexer.");
            }


            var factory = new TokenizerFactory(Language, ad, useAlphaNumericOptimization);

            var model = TokenizerME.Train(stream, factory, parameters, Project.Monitor);

            return new object[] { model };
        }
        
    }
}