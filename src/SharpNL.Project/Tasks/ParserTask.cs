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
using System.Xml;
using System.ComponentModel;

using SharpNL.Text;
using SharpNL.Parser;
using SharpNL.Project.Design;


namespace SharpNL.Project.Tasks {
    public class ParserTask : ProjectTask {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTask"/>.
        /// </summary>
        public ParserTask() : base(7f) {}

        #region . Input .

        /// <summary>
        /// Gets the input types of this <see cref="ParserTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="ParserTask"/>.</value>
        [Browsable(false)]
        public override Type[] Input {
            get { return new[] {typeof (Document)}; }
        }

        #endregion

        #region . Output .

        /// <summary>
        /// Gets the output types of this <see cref="ParserTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="ParserTask"/>.</value>
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
        [TypeConverter(typeof(NodeModelConverter<ParserModel>))]
        public string Model {
            get { return model; }
            set {
                model = value;
                Project.IsDirty = true;
            }
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

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this node.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var list = new List<ProjectProblem>();

            if (Project.Factory == null)
                list.Add(new ProjectProblem("The factory is not specified in the project."));

            if (string.IsNullOrEmpty(Model))
                list.Add(new ProjectProblem(this, "The parser model is not specified."));

            return list.Count > 0 ? list.ToArray() : null;
        }
        #endregion

        #region . Execute .
        /// <summary>
        /// Executes the derived node task.
        /// </summary>
        protected override object[] Execute() {

            IDocument doc = null;

            if (Parent != null)
                doc = Parent.GetOutput<IDocument>();

            if (doc == null)
                throw new NotSupportedException("Unable to retrieve the document from the parent node.");

            var sentences = doc.Sentences;
            if (sentences == null || sentences.Count == 0)
                throw new InvalidOperationException("The sentences are not detected on the specified document.");

            var parserModel = Project.Manager.GetModel<ParserModel>(Model);
            var parser = ParserFactory.Create(parserModel);

            foreach (var sentence in sentences) {
                var parse = ParserTool.ParseLine(sentence.Text, parser, 1);

                if (parse.Length == 1) {
                    sentence.Parse = parse[0];
                }
            }

            LogMessage(string.Format("{0} parsed sentences", sentences.Count));

            return new object[] { doc };
        }
        #endregion

    }
}