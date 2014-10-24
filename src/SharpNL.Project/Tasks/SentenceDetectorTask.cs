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
using System.Linq;
using System.Xml;
using SharpNL.Project.Design;
using SharpNL.Sentence;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Project.Tasks {
    /// <summary>
    /// Represents a sentence detector node, that identifies sentences in a text document.
    /// </summary>
    public class SentenceDetectorTask : ProjectTask {

        public SentenceDetectorTask() : base(0f) { }

        #region . Input .

        /// <summary>
        /// Gets the input types of this <see cref="SentenceDetectorTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="SentenceDetectorTask"/>.</value>
        public override Type[] Input {
            get { return new[] {typeof (Document)}; }
        }

        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="SentenceDetectorTask" />.
        /// </summary>
        /// <value>The output types of this <see cref="SentenceDetectorTask" />.</value>
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
        [TypeConverter(typeof (NodeModelConverter<SentenceModel>))]
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

            var sentenceModel = Project.Manager.GetModel<SentenceModel>(Model);
            if (sentenceModel == null)
                throw new InvalidOperationException("The model manager does not contain the model " + Model);
            
            var sentenceDetector = new SentenceDetectorME(sentenceModel);

            Span[] spans;
            lock (sentenceDetector) {
                spans = sentenceDetector.SentPosDetect(doc.Text);
            }

            var sentences = new List<Text.Sentence>(spans.Length);
            sentences.AddRange(
                spans.Select(span => new Text.Sentence(span.Start, span.End, doc))
                );
            doc.Sentences = new ReadOnlyCollection<Text.Sentence>(sentences);


            return new object[] { doc };
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
            }
        }
        #endregion

        #region . SerializeTask .
        protected override void SerializeTask(XmlWriter writer) {
            writer.WriteAttributeString("Model", Model);
        }
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            return string.IsNullOrEmpty(Model)
                ? new[] { new ProjectProblem(this, "No model selected.") } 
                : null;
        }
        #endregion

    }
}