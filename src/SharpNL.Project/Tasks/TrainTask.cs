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
using System.Globalization;
using System.Xml;
using SharpNL.Project.Design;

namespace SharpNL.Project.Tasks {
    /// <summary>
    /// Represents a abstract training task.
    /// </summary>
    public abstract class TrainTask : ProjectTask {

        public enum Algorithms {
            MaxEntropy,
            Perceptron
        }

        public enum DataIndexers {
            OnePass,
            TwoPass
        }

        /// <summary>
        /// The maximum threads
        /// </summary>
        private const int MaxThreads = 30;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainTask"/>.
        /// </summary>
        protected TrainTask() : base(0.9f) {}

        #region + Properties .

        #region . Algorithm .
        private Algorithms algorithm = Algorithms.MaxEntropy;
        /// <summary>
        /// Gets or sets the training algorithm.
        /// </summary>
        /// <value>The training algorithm.</value>
        [Category("Training"), Description("Specifies the training algorithm."), DefaultValue(typeof(Algorithms), "MaxEntropy")]
        public Algorithms Algorithm {
            get { return algorithm; }
            set {
                if (!Enum.IsDefined(typeof(Algorithms), value))
                    throw new ArgumentException();

                algorithm = value;

                if (Project != null)
                    Project.IsDirty = true;
            }
        }
        #endregion

        #region . Cutoff .

        private int cutoff = 5;

        /// <summary>
        /// Gets or sets the cutoff value.
        /// </summary>
        /// <value>The cutoff value.</value>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        /// <remarks>The <see cref="Cutoff" /> specifies the min number of times a feature must be seen.</remarks>
        [Category("Training"), Description("Specifies the min number of times a feature must be seen."), DefaultValue(5)]
        public int Cutoff {
            get { return cutoff; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                cutoff = value;
                Project.IsDirty = true;
            }
        }

        #endregion

        #region . DataIndexer .
        private DataIndexers dataIndexer = DataIndexers.TwoPass;

        [Category("Training"), Description("Specified the data indexer used during the training."), DefaultValue(typeof(DataIndexers), "TwoPass")]
        public DataIndexers DataIndexer {
            get { return dataIndexer; }
            set {
                if (!Enum.IsDefined(typeof(DataIndexers), value))
                    throw new ArgumentException();

                dataIndexer = value;

                if (Project != null)
                    Project.IsDirty = true;

            }
        }

        #endregion

        #region . Iterations .

        private int iterations = 100;

        /// <summary>
        /// Gets or sets the training iterations.
        /// </summary>
        /// <value>The training iterations.</value>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        [Category("Training"), Description("Specifies the number of training iterations."), DefaultValue(100)]
        public int Iterations {
            get { return iterations; }
            set {
                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                iterations = value;
                Project.IsDirty = true;
            }
        }

        #endregion

        #region . Language .
        private string language;
        /// <summary>
        /// Gets the language of this file.
        /// </summary>
        /// <value>The language of this file.</value>
        [Category("Training"), Description("Specifies the language which is being processed."), TypeConverter(typeof(LanguageConverter))]
        public string Language {
            get { return language; }
            set {
                language = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
            
        }
        #endregion

        #region . Threads .

        private int threads = 1;

        /// <summary>
        /// Gets or sets the number of threads used during the training.
        /// </summary>
        /// <value>The number of threads used during the training.</value>
        [Category("Training"), Description("Specifies the number of threads used during the training."), DefaultValue(1)]        
        public int Threads {
            get { return threads; }
            set {
                if (value < 1)
                    value = 1;

                if (value > MaxThreads)
                    value = MaxThreads;

                threads = value;
                if (Project != null)
                    Project.IsDirty = true;
            }
        }

        #endregion

        #endregion

        #region . Output .
        /// <summary>
        /// Gets the output types of this <see cref="TrainTask"/>.
        /// </summary>
        /// <value>The output types of this <see cref="TrainTask"/>.</value>
        public abstract override Type[] Output { get; }
        #endregion

        #region . Input .
        /// <summary>
        /// Gets the input types of this <see cref="ProjectTask"/>.
        /// </summary>
        /// <value>The input types of this <see cref="ProjectTask"/>.</value>
        public abstract override Type[] Input { get; }
        #endregion

        #region . SerializeTask .
        /// <summary>
        /// Serializes the train task.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void SerializeTask(XmlWriter writer) {
            writer.WriteAttributeString("Algorithm", Algorithm.ToString());
            writer.WriteAttributeString("Cutoff", Cutoff.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("DataIndexer", DataIndexer.ToString());
            writer.WriteAttributeString("Iterations", Iterations.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Language", Language);           
            if (Threads != 1)
                writer.WriteAttributeString("Threads", Threads.ToString(CultureInfo.InvariantCulture));

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

            int i;
            XmlAttribute att;
            
            att = node.Attributes["Algorithm"];
            if (att != null && !Enum.TryParse(att.Value, out algorithm))
                algorithm = Algorithms.MaxEntropy; // if can be parsed, set to default.

            att = node.Attributes["Cutoff"];
            if (att != null && int.TryParse(att.Value, out i) && i > 0)
                cutoff = i;

            att = node.Attributes["DataIndexer"];
            if (att != null && !Enum.TryParse(att.Value, out dataIndexer))
                dataIndexer = DataIndexers.TwoPass; // if can be parsed, set to default.

            att = node.Attributes["Iterations"];
            if (att != null && int.TryParse(att.Value, out i) && i > 0)
                iterations = i;

            att = node.Attributes["Language"];
            if (att != null)
                language = att.Value;

            att = node.Attributes["Threads"];
            if (att != null && int.TryParse(att.Value, out i) && i > 0 && i <= MaxThreads)
                threads = i;

        }
        #endregion

        #region . Execute .
        /// <summary>
        /// Executes the derived training task.
        /// </summary>
        protected abstract override object[] Execute();
        #endregion

        #region . GetProblems .
        /// <summary>
        /// Gets the problems with this training task.
        /// </summary>
        /// <returns>A array containing the problems or a <c>null</c> value, if any.</returns>
        public override ProjectProblem[] GetProblems() {
            var list = new List<ProjectProblem>();

            if (string.IsNullOrEmpty(Language))
                list.Add(new ProjectProblem(this, "The language is not specified."));

            return list.Count > 0 ? list.ToArray() : null;
        }
        #endregion

    }
}