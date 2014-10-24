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
using System.Xml;
using SharpNL.POSTag.Language.pt;
using SharpNL.Text;
using SharpNL.POSTag;
using SharpNL.Project.Design;
using SharpNL.Utility;


namespace SharpNL.Project.Tasks {
    /// <summary>
    /// Represents a Part-Of-Speech tagger task.
    /// </summary>
    public class POSTagTask : ProjectTask {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTask"/>.
        /// </summary>
        public POSTagTask() : base(5f) {}

        #region . Input .

        /// <summary>
        /// Gets the input types of this <see cref="POSTagTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="POSTagTask"/>.</value>
        [Browsable(false)]
        public override Type[] Input {
            get { return new[] {typeof (Document)}; }
        }

        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="POSTagTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="POSTagTask"/>.</value>
        [Browsable(false)]
        public override Type[] Output {
            get { return new[] {typeof (Document)}; }
        }

        #endregion

        #region . Model .

        private string model;
        /// <summary>
        /// Gets or sets the model name used in this task.
        /// </summary>
        /// <value>The model name used in this task.</value>
        [TypeConverter(typeof(NodeModelConverter<POSModel>))]
        public string Model {
            get { return model; }
            set {
                model = value;
                Project.IsDirty = true;
            }
        }
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

            if (!doc.Tokenized)
                throw new InvalidOperationException("The document is not tokenized.");


            var sentences = doc.Sentences;

            if (sentences == null || sentences.Count == 0)
                throw new InvalidOperationException("The sentences are not detected on the specified document.");

            var posModel = Project.Manager.GetModel<POSModel>(Model);
            var tagger = new POSTaggerME(posModel);

            foreach (var sentence in sentences) {

                string[] tags;
                double[] probs;

                var tokens = TextUtils.TokensToString(sentence.Tokens);

                lock (tagger) {
                    tags = tagger.Tag(tokens);
                    probs = tagger.Probabilities;
                }

                sentence.TokensProbability = TokenProb(probs);

                if (doc.Language == "pt" || doc.Language.StartsWith("pt")) {
                    tags = GenderUtil.RemoveGender(tags);
                }

                for (var i = 0; i < tokens.Length; i++) {
                    sentence.Tokens[i].POSTag = tags[i];
                    sentence.Tokens[i].POSTagProbability = probs[i];
                }

            }

            doc.PoS = true;

            return new object[] {doc};
        }


        #endregion

        #region . TokenProb .
        private static double TokenProb(double[] probs) {
            var finalProb = 0d;

            for (var i = 0; i < probs.Length; i++)
                finalProb += probs[i]; //Math.Log(probs[i]);

            if (probs.Length > 0)
                finalProb = finalProb / probs.Length;

            return finalProb;
        }
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            if (string.IsNullOrEmpty(Model))
                return new[] { new ProjectProblem(this, "The POS model is not specified.") };

            return null;
        }
        #endregion

        #region . SerializeTask .
        protected override void SerializeTask(XmlWriter writer) {
            writer.WriteAttributeString("Model", Model);
        }
        #endregion

        #region . DeserializeTask .
        protected override void DeserializeTask(XmlNode node) {
            if (node.Attributes != null) {
                var attModel = node.Attributes["Model"];
                if (attModel != null)
                    Model = attModel.Value;

            }
        }
        #endregion

    }
}