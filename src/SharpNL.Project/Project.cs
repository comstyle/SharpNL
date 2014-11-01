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
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpNL.Text;
using SharpNL.Utility;
using SharpNL.Utility.Model;

namespace SharpNL.Project {
    /// <summary>
    /// Represents a SharpNL project.
    /// </summary>
    [DefaultProperty("Name"), Description("Represents a SharpNL project."), TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Project : IDisposable, IXmlSerializable {
        private bool loading;

        private bool? serializeContent;
        private string name;

        private readonly ModelManager manager;
        private readonly List<ProjectNode> nodes;
        private readonly List<ProjectProblem> problems;

        #region + Events .

        /// <summary>
        /// Occurs when the object that is running the task sends an informational message.
        /// </summary>
        public event EventHandler<MonitorMessageEventArgs> Message;

        /// <summary>
        /// Occurs when the object that is running the task sends a warning message.
        /// </summary>
        public event EventHandler<MonitorMessageEventArgs> Warning;

        /// <summary>
        /// Occurs when an exception is throw during the task execution.
        /// </summary>
        public event EventHandler<MonitorExceptionEventArgs> Exception;

        /// <summary>
        /// Occurs when a <see cref="ProjectTask"/> begins execution.
        /// </summary>
        public event EventHandler<TaskEventArgs> TaskStarted;

        /// <summary>
        /// Occurs when a <see cref="ProjectTask"/> completes execution.
        /// </summary>
        public event EventHandler<TaskEventArgs> TaskFinished;

        /// <summary>
        /// Occurs when the project is modified.
        /// </summary>
        public event EventHandler Modified;

        /// <summary>
        /// Occurs when the project is renamed.
        /// </summary>
        public event EventHandler Renamed;

        /// <summary>
        /// Occurs when the project is validated.
        /// </summary>
        public event EventHandler Validated;

        /// <summary>
        /// Occurs when the project completes an execution.
        /// </summary>
        public event EventHandler Completed;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        public Project() {
            manager = new ModelManager();
            manager.Changed += ManagerChanged;
            Monitor = new Monitor();

            Monitor.Message += (sender, args) => {
                if (Message != null)
                    Message(this, args);
            };
            Monitor.Warning += (sender, args) => {
                if (Warning != null)
                    Warning(this, args);
            };
            Monitor.Exception += (sender, args) => {
                if (Exception != null)
                    Exception(this, args);
            };

            Monitor.Complete += (sender, args) => {
                if (Completed != null)
                    Completed(this, EventArgs.Empty);
            };

            nodes = new List<ProjectNode>();     
            problems = new List<ProjectProblem>();

            Factory = DefaultTextFactory.Instance;
        }

        private void ManagerChanged(object sender, ModelInfoEventArgs modelInfoEventArgs) {
            IsDirty = true;
        }

        #region + Properties .

        #region . Factory .
        /// <summary>
        /// Gets or sets the text factory.
        /// </summary>
        /// <value>The text factory.</value>
        [Description("The factory responsible to create the resources."), Browsable(false)]
        public ITextFactory Factory { get; set; }
        #endregion

        #region . Manager .
        /// <summary>
        /// Gets the model manager.
        /// </summary>
        /// <value>The model manager.</value>
        [Browsable(false)]
        public ModelManager Manager {
            get { return manager; }
        }
        #endregion

        #region . Monitor .

        /// <summary>
        /// Gets the execution monitor.
        /// </summary>
        /// <value>The execution monitor.</value>
        public Monitor Monitor { get; private set; }

        #endregion

        #region . Name .
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        [Category("Project"), Description("Describes the name of the project."), XmlIgnore]
        public string Name {
            get { return name; }
            set {
                if (string.IsNullOrWhiteSpace(value)) 
                    return;

                name = value;

                if (loading) 
                    return;

                OnRenamed(EventArgs.Empty);

                IsDirty = true;
            }
        }
        #endregion

        #region . Nodes .

        /// <summary>
        /// Gets the nodes in this <see cref="Project"/>.
        /// </summary>
        /// <value>The nodes in this <see cref="Project"/>.</value>
        public ReadOnlyCollection<ProjectNode> Nodes {
            get { return nodes.AsReadOnly(); }           
        }
        #endregion

        #region . IsDirty .

        private bool isDirty;
        /// <summary>
        /// Gets or sets a value indicating whether this instance has changed.
        /// </summary>
        /// <value><c>true</c> if this instance has changed; otherwise, <c>false</c>.</value>
        /// <remarks>When this property is set to <c>true</c>, the <see cref="Modified"/> event is rised.</remarks>
        [Description("Indicates whether the project has changed."), ReadOnly(true)]
        public bool IsDirty {
            get { return isDirty; }
            set {
                if (loading) return;

                isDirty = value;

                if (value) 
                    OnModified(EventArgs.Empty);
            }
        }
        #endregion

        #region . IsRunning .
        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        [Description("Determise whether this project is running.")]
        public bool IsRunning {
            get { return Monitor.IsRunning; }
        }
        #endregion

        #region . IsUntitled .
        /// <summary>
        /// Gets a value indicating whether this project is untitled.
        /// </summary>
        /// <value><c>true</c> if this project is untitled; otherwise, <c>false</c>.</value>
        public bool IsUntitled {
            get { return string.IsNullOrEmpty(Name); }
        }
        #endregion

        #region . IsLoading .
        /// <summary>
        /// Gets a value indicating whether this instance is loading.
        /// </summary>
        /// <value><c>true</c> if this instance is loading; otherwise, <c>false</c>.</value>
        internal bool IsLoading {
            get { return loading; }
        }
        #endregion

        #region . Problems .
        /// <summary>
        /// Gets the problems list from the last <see cref="Validate"/> call.
        /// </summary>
        /// <value>The problems list from the last <see cref="Validate"/> call.</value>
        public IReadOnlyCollection<ProjectProblem> Problems {
            get { return problems.AsReadOnly(); }
        }
        #endregion

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified <see cref="ProjectNode"/> to the project.
        /// </summary>
        /// <param name="node">The <see cref="ProjectNode"/> to be added.</param>
        /// <exception cref="ArgumentNullException">node</exception>
        public ProjectNode Add(ProjectNode node) {
            if (node == null)
                throw new ArgumentNullException("node");


            node.SetProject(this);
            nodes.Add(node);

            IsDirty = true;

            return node;
        }
        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            
        }
        #endregion

        #region . Deserialize .
        /// <summary>
        /// Deserializes the specified input stream into a <see cref="Project" /> instance.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <returns>The deserialized <see cref="Project" /> instance.</returns>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="System.ArgumentException">@The stream is not readable.;inputStream</exception>
        /// <exception cref="InvalidFormatException">
        /// The stream is empty.
        /// or
        /// Unable to deserialize the project.
        /// </exception>
        public static Project Deserialize(Stream inputStream) {           
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (!inputStream.CanRead)
                throw new ArgumentException(@"The stream is not readable.", "inputStream");

            if (inputStream.Length == 0)
                throw new InvalidFormatException("The stream is empty.");

            var serializer = new XmlSerializer(typeof(Project));
            Project project;
            try {               
                project = (Project)serializer.Deserialize(inputStream);    
            } catch (Exception ex) {
                throw new InvalidFormatException("Unable to deserialize the project.", ex);
            }

            return project;
        }
        #endregion

        #region . Runner .
        private void Runner(CancellationToken cancel) {
            foreach (var node in nodes)
                RunNode(cancel, node);

        }

        private static void RunNode(CancellationToken cancel, ProjectNode node) {
            cancel.ThrowIfCancellationRequested();
            node.Execute(false);

            // don't execute the children nodes when the node fails
            if (node.Status == NodeStatus.Success) {
                foreach (var child in node.Children) {
                    cancel.ThrowIfCancellationRequested();
                    child.Execute(false);
                }          
            }           
        }

        #endregion

        #region . OnRenamed .
        private void OnRenamed(EventArgs e) {
            if (Renamed != null)
                Renamed(this, e);
        }
        #endregion

        #region . OnModified .
        private void OnModified(EventArgs e) {
            if (loading) return;

            if (Modified != null)
                Modified(this, e);
        }

        #endregion

        #region . OnTaskStarted .
        internal void OnTaskStarted(ProjectTask task) {
            if (TaskStarted != null)
                TaskStarted(this, new TaskEventArgs(task));
        }
        #endregion

        #region . OnTaskFinished .
        internal void OnTaskFinished(ProjectTask task) {

            if (TaskFinished != null) {
                TaskFinished(this, new TaskEventArgs(task));
            }
        }
        #endregion

        #region . Run .
        /// <summary>
        /// Runs this project.
        /// </summary>
        public void Run() {
            if (IsRunning)
                throw new InvalidOperationException("This project is already running.");

            Monitor.Execute(Runner);
        }

        #endregion

        #region . Serialize .
        /// <summary>
        /// Serializes the project into the given output stream.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <param name="saveContent">Determines whether the content of the nodes should be saved.</param>
        /// <exception cref="ArgumentNullException">outputStream</exception>
        public void Serialize(Stream output, bool saveContent = false) {
            if (output == null)
                throw new ArgumentNullException("output");

            try {
                serializeContent = saveContent;

                var serializer = new XmlSerializer(typeof(Project));
                serializer.Serialize(output, this);    
            } finally {
                serializeContent = null;
            }
            
        }
        #endregion

        #region . Stop .
        /// <summary>
        /// Stops the execution of the project.
        /// </summary>
        public void Stop() {
            if (IsRunning) {
                Monitor.Cancel();
            }
        }
	    #endregion
        
        #region . GetSchema .
        [Browsable(false)]
        public XmlSchema GetSchema() {
            return null;
        }
        #endregion

        #region . ReadXml .
        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. </param>
        [Browsable(false)]
        public void ReadXml(XmlReader reader) {
            var doc = new XmlDocument();

            doc.Load(reader);

            loading = true;
 
            var root = doc.DocumentElement;

            if (root == null || root.Name != "Project")
                throw new InvalidOperationException("Invalid root element.");

            name = root.GetAttribute("Name");

            var mods = root.SelectNodes("/Project/Models/Model");

            if (mods != null) {
                foreach (XmlNode model in mods) {

                    if (model.Attributes == null) continue;                   
                    var mf = model.Attributes["file"];

                    if (!string.IsNullOrEmpty(mf.Value))
                        manager.Add(new ModelInfo(mf.Value));

                }    
            }

            var nods = root.SelectNodes("/Project/Nodes/Node");
            if (nods != null) {
                foreach (XmlNode node in nods) {
                    nodes.Add(ProjectNode.DeserializeNode(node, null, this));
                }
            }

            isDirty = false;
            loading = false;
        }
        #endregion

        #region . WriteXml .
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. </param>
        [Browsable(false)]
        public void WriteXml(XmlWriter writer) {
            writer.WriteAttributeString("Name", name);

            var content = serializeContent.HasValue && serializeContent.Value;

            writer.WriteStartElement("Models");
            foreach (var model in manager) {
                writer.WriteStartElement("Model");
                writer.WriteAttributeString("file", model.File.FullName);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();           

            writer.WriteStartElement("Nodes");
            foreach (var node in Nodes) {
                node.Serialize("Node", writer, content);
            }
            writer.WriteEndElement();
        }
        #endregion

        #region . Validates .
        /// <summary>
        /// Validates project and logs the problems into <see cref="Problems"/>, if any. Returns a <see cref="bool"/> value that indicates whether the project can be executed.
        /// </summary>
        /// <returns><c>true</c> if the project can be executed; otherwise, <c>false</c>.</returns>
        public bool Validate() {
            problems.Clear();
            
            if (nodes.Count == 0)
                problems.Add(new ProjectProblem(this, "The project has nothing to execute."));

            if (Factory == null)
                problems.Add(new ProjectProblem(this, "The factory is not specified."));

            if (problems.Count > 0)
                goto done;

            foreach (var node in Nodes)
                LookForTrouble(node);

            done:
            try {
                return problems.Count == 0;
            } finally {
                if (Validated != null)
                    Validated(this, EventArgs.Empty);               
            }           
        }
        #endregion

        #region . LookForTrouble .
        /// <summary>
        /// Looks for trouble in the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        private void LookForTrouble(ProjectNode node) {

            var np = node.GetProblems();
            if (np != null)
                problems.AddRange(np);

            foreach (var task in node.Tasks) {
                np = task.GetProblems();
                if (np != null)
                    problems.AddRange(np);
            }

            foreach (var child in node.Children)
                LookForTrouble(child);

        }
        #endregion

    }
}