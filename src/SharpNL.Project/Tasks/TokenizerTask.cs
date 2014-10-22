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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;
using SharpNL.Project.Design;
using SharpNL.Text;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Project.Tasks {
    public class TokenizerTask : ProjectTask {

        private static readonly char[] quote;
        static TokenizerTask() {
            quote = new[] { '«', '»', '“', '”' };
        }

        public TokenizerTask() : base(1f) {
            Normalize = true;
        }

        #region + Properties .

        #region . Input .
        /// <summary>
        /// Gets the input types of this <see cref="ProjectTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="ProjectTask"/>.</value>
        [Browsable(false)]
        public override Type[] Input {
            get { return new[] { typeof(Document) }; }
        }
        #endregion

        #region . Normalize .
        /// <summary>
        /// Gets or sets a value indicating whether string quotations must be normalized.
        /// </summary>
        /// <value><c>true</c> if the string quotations must be normalized; otherwise, <c>false</c>. The default value is <c>true</c>.</value>
        [Description("Determines if quotations must be normalized"), DefaultValue(true)]
        public bool Normalize { get; set; }

        #endregion

        #region . Model .
        /// <summary>
        /// Gets or sets the model name used in this task.
        /// </summary>
        /// <value>The model name used in this task.</value>
        [Description("The tokenizer model."), TypeConverter(typeof(NodeModelConverter<TokenizerModel>))]
        public string Model { get; set; }
        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="ProjectTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="ProjectTask"/>.</value>
        [Browsable(false)]
        public override Type[] Output {
            get { return new[] { typeof(Document) }; }
        }
        #endregion

        #endregion

        #region . Execute .
        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected override object[] Execute() {
            Document doc = null;

            if (Parent != null)
                doc = Parent.GetOutput<Document>();

            if (doc == null)
                throw new NotSupportedException("Unable to retrieve the document from the parent node.");

            var sentences = doc.Sentences;
            if (sentences == null || sentences.Count == 0)
                throw new InvalidOperationException("The sentences are not detected on the specified document.");

            foreach (var sentence in sentences) {
                var text = Normalize ? sentence.Text.Replace(quote, '"') : sentence.Text;

                var model = Project.Manager.GetModel<TokenizerModel>(Model);
                if (model == null)
                    throw new InvalidOperationException("The model manager does not contain the model " + Model);


                Span[] spans;
                double[] probs;
                var tokenizer = new TokenizerME(model);
                lock (tokenizer) {
                    spans = tokenizer.TokenizePos(text);
                    probs = tokenizer.TokenProbabilities;
                }

                var tokens = new List<Token>(spans.Length);

                // ReSharper disable once LoopCanBeConvertedToQuery
                for (var i = 0; i < spans.Length; i++) {
                    tokens.Add(new Token(spans[i].Start, spans[i].End, spans[i].GetCoveredText(text)) {
                        Probability = probs[i]
                    });
                }

                sentence.Tokens = new ReadOnlyCollection<Token>(tokens);
            }

            doc.Tokenized = true;

            return new object[] { doc };
        }
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            if (string.IsNullOrEmpty(Model))
                return new[] { new ProjectProblem(this, "The tokenizer model is not specified.") };

            return null;
        }
        #endregion

        #region . DeserializeTask .
        /// <summary>
        /// Deserializes the task from a given <see cref="XmlReader"/> object.
        /// </summary>
        /// <param name="node">The xml node.</param>
        protected override void DeserializeTask(XmlNode node) {
            if (node.Attributes != null) {
                var attModel = node.Attributes["Model"];
                if (attModel != null)
                    Model = attModel.Value;

                var attNormalize = node.Attributes["Normalize"];
                if (attNormalize != null)
                    Normalize = attNormalize.Value == "true";

            }
        }
        #endregion

        #region . SerializeTask .
        protected override void SerializeTask(XmlWriter writer) {
            writer.WriteAttributeString("Model", Model);
            writer.WriteAttributeString("Normalize", Normalize ? "true" : "false");
        }
        #endregion

    }
}