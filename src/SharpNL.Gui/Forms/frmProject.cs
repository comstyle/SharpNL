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
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SharpNL.Chunker;
using SharpNL.DocumentCategorizer;
using SharpNL.Gui.Properties;
using SharpNL.NameFind;
using SharpNL.Parser;
using SharpNL.POSTag;
using SharpNL.Project;
using SharpNL.Project.Nodes;
using SharpNL.Project.Tasks;
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility.Model;

using SharpNL.Gui.Viewers;

namespace SharpNL.Gui.Forms {
    public partial class frmProject : Form {

        internal Project.Project project;

        private TreeNode nodProject;
        private TreeNode nodModels;
        private Viewer activeViewer;

        public frmProject() {
            InitializeComponent();          
        }

        public frmProject(Project.Project project) {
            InitializeComponent(); 

            this.project = project;
        }

        private void LoadProject(Project.Project newProject) {
            Clean();

            project = newProject;
            project.Message += ProjectOnMessage;
            project.Warning += ProjectOnWarning;
            project.Exception += ProjectOnException;

            project.Renamed += ProjectOnRenamed;
            project.Modified += ProjectOnModified;
            project.Validated += ProjectOnValidated;
            project.Completed += ProjectOnCompleted;

            project.TaskStarted += ProjectOnTaskStarted;
            project.TaskFinished += ProjectOnTaskFinished;

            treeView.Tag = project;

            toolValidate.Enabled = true;
            toolStart.Enabled = true;

            ProjectOnRenamed(this, EventArgs.Empty);

            RefreshTree();
        }



        private void ProjectOnTaskStarted(object sender, TaskEventArgs args) {
            var watch = new Stopwatch();
            try {
                args.Task.Tag = watch;

                richLog.InvokeIfRequired(() =>
                    richLog.AppendText(string.Format("# {0} started.\n\n", args.Task.GetType().Name), Color.SlateGray)
                    );

            } finally {
                watch.Start();
            }
        }
        private void ProjectOnTaskFinished(object sender, TaskEventArgs args) {
            var watch = args.Task.Tag as Stopwatch;
            if (watch != null) {
                watch.Stop();

                richLog.InvokeIfRequired(() =>
                    richLog.AppendText(string.Format("\n# {0} finished.\n#   Time taken: {1:c}\n", args.Task.GetType().Name, TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds)), Color.SlateGray)
                    );

            }
        }


        private void ProjectOnRenamed(object sender, EventArgs eventArgs) {
            Text = string.IsNullOrWhiteSpace(project.Name)
                ? Resources.ProjectUnamed
                : string.Format("{0} - {1}", project.Name, Resources.Project);
        }

        private void ProjectOnCompleted(object sender, EventArgs eventArgs) {
            toolStrip.InvokeIfRequired(() => {
                toolStart.Enabled = true;
                toolStop.Enabled = false;
                RefreshTree();
            });

        }

        private void ProjectOnModified(object sender, EventArgs eventArgs) {
            toolSaveProject.Enabled = true;

            RefreshTree();
        }


        private void ProjectOnException(object sender, MonitorExceptionEventArgs e) {
            richLog.InvokeIfRequired(() => {
                tabControl.SelectTab(tabLog);
                richLog.AppendText(string.Format("An exception has been thrown in the project.\n\nInformation for nerds:\n{0}", e.Exception) + "\n", Color.Red);
            });
        }
        private void ProjectOnWarning(object sender, MonitorMessageEventArgs e) {
            richLog.InvokeIfRequired(() => richLog.AppendText(e.Message + "\n", Color.Tomato));
        }
        private void ProjectOnMessage(object sender, MonitorMessageEventArgs e) {
            richLog.InvokeIfRequired(() => richLog.AppendText("  " + e.Message.TrimStart(' ') + "\n", SystemColors.WindowText));
        }

        private void ProjectOnValidated(object sender, EventArgs eventArgs) {
            
            listErrors.InvokeIfRequired(() => {
                listErrors.BeginUpdate();
                try {
                    listErrors.Items.Clear();

                    foreach (var problem in project.Problems) {
                        var item = listErrors.Items.Add(problem.Description);
                        if (problem.Object != null)
                            item.SubItems.Add(problem.Object.GetType().Name);

                        item.Tag = problem;
                    }

                    if (project.Problems.Count > 0) 
                        tabControl.SelectTab(tabErrors);
                                       
                } finally {
                    listErrors.EndUpdate();
                }

            });

        }

        /// <summary>
        /// Cleans this the interface.
        /// </summary>
        private void Clean() {
            if (activeViewer != null) {
                splitProject.Panel1.Controls.Remove(activeViewer);

                if (activeViewer.merged)
                    ToolStripManager.RevertMerge(toolStrip);

                activeViewer.Dispose();
                activeViewer = null;
            }

            propertyGrid.SelectedObject = null;

            richLog.Clear();
            treeView.Nodes.Clear();
            listErrors.Items.Clear();
           
            // reset the tasks to the default state
            toolNewProject.Enabled = true;
            toolSaveProject.Enabled = false;
            toolProjectOpen.Enabled = true;
            toolValidate.Enabled = false;

            toolStart.Enabled = false;
            toolStop.Enabled = false;
        }

        private void frmProject_Load(object sender, EventArgs e) {
            
            treeView.TreeViewNodeSorter = new ProjectSorter();
            treeView.Sorted = true;
            
            LoadProject(project ?? new Project.Project());           
        }

        private void mnuAddModel_Click(object sender, EventArgs e) {
            var open = new OpenFileDialog {
                Title = Resources.ProjectMenuModelAdd,
                Filter = Resources.ProjectMenuModelAddFilter,
                Multiselect = true
            };

            if (open.ShowDialog() != DialogResult.OK) return;

            foreach (var fileName in open.FileNames) {
                var info = new ModelInfo(fileName);
                if (info.ModelType == Models.Unknown) {
                    MessageBox.Show(
                        Resources.ProjectMenuModelAddUnknown,
                        Resources.ProjectMenuModelAddUnknownTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (nodModels.Nodes.ContainsKey(info.Name))
                    continue;

                project.Manager.Add(info);                
            }

            RefreshTree();

        }

        private void RefreshTree() {
            treeView.BeginUpdate();

            TreeNode selected = null;
            var expand = false;
            try {

                // checks if the project is the same
                if (treeView.Nodes.Count == 0 || treeView.Tag == null || treeView.Tag != project) {

                    //newProject = true;

                    treeView.Nodes.Clear();

                    var title = string.IsNullOrEmpty(project.Name) ? Resources.Project : project.Name;

                    nodProject = treeView.Nodes.Add("nodProject", title, "project", "project");
                    nodProject.ContextMenuStrip = mnuProject;
                    nodProject.Tag = project;

                    nodModels = nodProject.Nodes.Add("nodModels", Resources.ProjectModels, "models", "models");
                    nodModels.ContextMenuStrip = mnuProject;

                    expand = true;
                } else
                    selected = treeView.SelectedNode;


                var stack = new Stack();

                foreach (TreeNode node in nodModels.Nodes)
                    stack.Push(node);

                foreach (var model in project.Manager)
                    stack.Push(model);

                while (stack.Count > 0) {
                    var item = stack.Pop();

                    var node = item as TreeNode;
                    if (node != null) {
                        if (!project.Manager.Contains(node.Name))
                            node.Remove();

                        continue;
                    }

                    var info = item as ModelInfo;
                    if (info != null) {
                        if (!nodModels.Nodes.ContainsKey(info.Name)) {
                            // new model
                            nodModels.Nodes.Add(new TreeNode {
                                Text = info.Name,
                                Name = info.Name,
                                Tag = info,
                                ImageKey = @"model",
                                SelectedImageKey = @"model"
                            });

                            expand = true;
                        }

                        continue;
                    }
                    throw new InvalidOperationException();
                }

                foreach (TreeNode node in nodProject.Nodes) 
                    if (node.Name != "nodModels")
                        stack.Push(node);

                foreach (var node in project.Nodes) 
                    stack.Push(node);
               
                RefreshNode(nodProject.Nodes, project.Nodes, stack);

            } finally {
                treeView.Sort();

                // keep the previous selection (prevent flickering)
                if (selected != null && treeView.Contains(selected)) {
                    treeView.SelectedNode = selected;
                    treeView.SelectedNode.EnsureVisible();
                }
                    

                treeView.EndUpdate();
                if (expand)
                    treeView.ExpandAll();
            }

            propertyGrid.Refresh(); // make the property grid to refresh the properties


        }

        private void RefreshNode(TreeNodeCollection treeNodes, ReadOnlyCollection<ProjectNode> projectNodes, Stack stack) {
            while (stack.Count > 0) {
                var item = stack.Pop();

                ProjectNode pn;
                var tn = item as TreeNode;
                if (tn != null) {
                    pn = tn.Tag as ProjectNode;
                    if (pn != null) {
                        if (!projectNodes.Contains(pn))
                            tn.Remove();
                        else {

                            RefreshOutputs(tn.Nodes["outputs"], pn);

                            RefreshTasks(tn.Nodes["tasks"], pn);

                            // child nodes
                            var pcs = new Stack();
                            foreach (TreeNode node in tn.Nodes)
                                pcs.Push(node);

                            foreach (var node in pn.Children)
                                pcs.Push(node);

                            RefreshNode(tn.Nodes["nodes"].Nodes, pn.Children, pcs);

                        }
                    }
                    continue;
                }

                pn = item as ProjectNode;
                if (pn != null) {
                    var found = treeNodes.Cast<TreeNode>().Any(treeNode => treeNode.Tag == pn);
                    if (!found) {

                        tn = CreateProjectNode(pn);

                        var result = tn.Nodes.Add("outputs", Resources.ProjectNodeOutputs, "outputs", "outputs");
                        RefreshOutputs(result, pn);

                        var tasks = tn.Nodes.Add("tasks", Resources.ProjectNodeTasks, "tasks", "tasks");
                        tasks.ContextMenuStrip = mnuTask;

                        RefreshTasks(tasks, pn);

                        var nodes = tn.Nodes.Add("nodes", Resources.ProjectNodeNodes, "nodes", "nodes");
                        nodes.ContextMenuStrip = mnuNode;

                        RefreshNodes(nodes, pn);

                        treeNodes.Add(tn);

                        tn.Expand();
                    }
                    continue;
                }


                throw new InvalidOperationException();
            }
        }

        private void RefreshOutputs(TreeNode treeNode, ProjectNode projectNode) {
            var stack = new Stack();
            foreach (TreeNode n in treeNode.Nodes)
                stack.Push(n);

            foreach (var o in projectNode.Outputs)
                stack.Push(o);

            while (stack.Count > 0) {
                var item = stack.Pop();

                var tn = item as TreeNode;
                if (tn != null) {
                    if (tn.Tag == null || !projectNode.Outputs.Contains(tn.Tag))
                        tn.Remove();

                    continue;
                }

                foreach (TreeNode n in treeNode.Nodes) {
                    if (n.Tag == item)
                        goto next;
                }


                treeNode.Nodes.Add(CreateOutputNode(item));

            next:
                ;
            }

        }
        private void RefreshTasks(TreeNode treeTasks, ProjectNode node) {
            var stack = new Stack();
            foreach (TreeNode n in treeTasks.Nodes)
                stack.Push(n);

            foreach (var t in node.Tasks)
                stack.Push(t);

            while (stack.Count > 0) {
                var item = stack.Pop();

                ProjectTask projectTask;
                var tn = item as TreeNode;
                if (tn != null) {
                    projectTask = tn.Tag as ProjectTask;
                    if (projectTask == null || !node.Tasks.Contains(projectTask)) {
                        tn.Remove();                       
                    }
                    continue;
                }

                projectTask = item as ProjectTask;
                if (projectTask != null) {
                    foreach (TreeNode n in treeTasks.Nodes) {
                        if ((n.Tag as ProjectTask) == projectTask) {
                            goto next;
                        }
                    }

                    tn = CreateTaskNode(projectTask);

                    if (tn != null) {
                        tn.Tag = projectTask;

                        treeTasks.Nodes.Add(tn);
                    } else {
                        richLog.AppendText(string.Format(Resources.ErrorUnknownTask, projectTask.GetType().Name), Color.Red);
                    }
                }
            next:
                ;
            }
            
        }

        #region . CreateProjectNode .
        private static TreeNode CreateProjectNode(ProjectNode pn) {
            if (pn.GetType() == typeof(TextFileNode)) {
                return new TreeNode {
                    Text = Resources.ProjectNodeTextFile,
                    ImageKey = @"textfile",
                    SelectedImageKey = @"textfile",
                    Tag = pn
                };

            }
            if (pn.GetType() == typeof(WebGetNode)) {
                return new TreeNode {
                    Text = Resources.ProjectNodeWebGet,
                    ImageKey = @"textfile",
                    SelectedImageKey = @"textfile",
                    Tag = pn
                };
            }
            if (pn.GetType() == typeof(TextInputNode)) {
                return new TreeNode {
                    Text = Resources.ProjectNodeTextInput,
                    ImageKey = @"textinput",
                    SelectedImageKey = @"textinput",
                    Tag = pn
                };
            }
            if (pn.GetType() == typeof(TrainInputNode)) {
                return new TreeNode {
                    Text = Resources.ProjectNodeTrainInput,
                    ImageKey = @"train_input",
                    SelectedImageKey = @"train_input",
                    Tag = pn
                };
            }

            throw new InvalidOperationException();
        }
        #endregion

        #region . CreateOutputNode .
        private static TreeNode CreateOutputNode(object output) {

            return new TreeNode {
                Text = output.GetType().Name,
                ImageKey = @"document",
                SelectedImageKey = @"document",
                Tag = output
            };
        }
        #endregion

        private static TreeNode CreateTaskNode(ProjectTask task) {

            if (task is SentenceDetectorTask)
                return new TreeNode(Resources.ProjectTaskSD) {
                    SelectedImageKey = @"sd",
                    ImageKey = @"sd"                   
                };

            if (task is SentenceDetectorTrainTask) 
                return new TreeNode(Resources.ProjectTaskSDTrain) {
                    SelectedImageKey = @"train",
                    ImageKey = @"train"  
                };

            if (task is TokenizerTask)
                return new TreeNode(Resources.ProjectTaskTokenize) {
                    SelectedImageKey = @"tokenize",
                    ImageKey = @"tokenize"
                };

            if (task is TokenizerTrainTask)
                return new TreeNode(Resources.ProjectTaskTokenizeTrain) {
                    SelectedImageKey = @"train",
                    ImageKey = @"train"
                };

            if (task is EntityFinderTask)
                return new TreeNode(Resources.ProjectNodeEntityRecognize) {
                    SelectedImageKey = @"entity_recognize",
                    ImageKey = @"entity_recognize"
                };

            if (task is ChunkerTask)
                return new TreeNode(Resources.ProjectTaskChunk) {
                    SelectedImageKey = @"chunk",
                    ImageKey = @"chunk"
                };

            if (task is POSTagTask)
                return new TreeNode(Resources.ProjectTaskPOSTag) {
                    SelectedImageKey = @"postag",
                    ImageKey = @"postag"
                };

            if (task is ParserTask)
                return new TreeNode(Resources.ProjectTaskParse) {
                    SelectedImageKey = @"parse",
                    ImageKey = @"parse"
                };

            if (task is ModelWriterTask)
                return new TreeNode(Resources.ProjectTaskSaveModelFile) {
                    SelectedImageKey = @"model_save",
                    ImageKey = @"model_save"
                };

            // unknown task node. 
            return null;
        }

        private void RefreshNodes(TreeNode treeNodes, ProjectNode node) {
            var stack = new Stack();
            foreach (TreeNode n in treeNodes.Nodes)
                stack.Push(n);

            foreach (var t in node.Children)
                stack.Push(t);

            while (stack.Count > 0) {
                var item = stack.Pop();

                ProjectNode projectNode;
                var tn = item as TreeNode;
                if (tn != null) {
                    projectNode = tn.Tag as ProjectNode;
                    if (projectNode == null || !node.Children.Contains(projectNode)) {
                        tn.Remove();
                    }
                    continue;
                }

                projectNode = item as ProjectNode;
                if (projectNode != null) {
                    foreach (TreeNode n in treeNodes.Nodes) {
                        if ((n.Tag as ProjectNode) == projectNode) {
                            goto next;
                        }
                    }

                    tn = CreateProjectNode(projectNode);
                    
                    var objects = tn.Nodes.Add("outputs", Resources.ProjectNodeOutputs, "outputs", "outputs");

                    RefreshOutputs(objects, projectNode);

                    var tasks = tn.Nodes.Add("tasks", Resources.ProjectNodeTasks, "tasks", "tasks");
                    tasks.ContextMenuStrip = mnuTask;

                    RefreshTasks(tasks, projectNode);

                    var nodes = tn.Nodes.Add("nodes", Resources.ProjectNodeNodes, "nodes", "nodes");
                    nodes.ContextMenuStrip = mnuNode;

                    RefreshNodes(nodes, projectNode);

                    tn.Expand();

                    treeNodes.Nodes.Add(tn);
                }
                next:
                ;
            }
        }


        private class ProjectSorter : IComparer {
            public int Compare(object x, object y) {
                var one = x as TreeNode;
                var two = y as TreeNode;
                if (one != null && two != null) {
                    if (one.Parent != null && one.Parent == two.Parent && one.Parent.Name == "nodModels")
                        return string.Compare(one.Text, two.Text, StringComparison.OrdinalIgnoreCase);

                    if (one.Parent != null && one.Parent == two.Parent && one.Parent.Name == "tasks") {
                        var t1 = one.Tag as ProjectTask;
                        var t2 = two.Tag as ProjectTask;
                        if (t1 != null && t2 != null) {
                            return t1.CompareTo(t2);
                        }
                    }
 
                    return one.Index.CompareTo(two.Index); // keep the index order
                }
                return 0;
            }
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            // fix the selection bug
            if (e.Button == MouseButtons.Right) {
                treeView.SelectedNode = e.Node;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node.Tag != null) {

                propertyGrid.SelectedObject = e.Node.Tag;
                return;
            }
            propertyGrid.SelectedObject = null;
        }

        private void treeView_KeyUp(object sender, KeyEventArgs e) {
            if (treeView.SelectedNode != null) {
                if (treeView.SelectedNode == nodProject) {
                    if (e.KeyCode == Keys.F2) {
                        nodProject.BeginEdit();
                        return;
                    }
                }

                if (treeView.SelectedNode.Parent == null)
                    return;

                if (treeView.SelectedNode.Parent == nodModels) {
                    if (e.KeyCode == Keys.Delete) {
                        var info = treeView.SelectedNode.Tag as ModelInfo;
                        if (info != null) {

                            treeView.SelectedNode = treeView.SelectedNode.Parent;

                            propertyGrid.SelectedObject = null;                           

                            project.Manager.Remove(info);                         
                        }
                        return;
                    }
                    if (e.KeyCode == Keys.Insert)
                        mnuAddModel_Click(this, EventArgs.Empty);

                    return;
                }

                if (treeView.SelectedNode.Parent.Name == "tasks") {
                    var task = treeView.SelectedNode.Tag as ProjectTask;
                    var node = treeView.SelectedNode.Parent.Parent.Tag as ProjectNode;

                    if (node == null || task == null)
                        return;

                    if (e.KeyCode == Keys.Delete) {
                        treeView.SelectedNode = treeView.SelectedNode.Parent;

                        node.Remove(task);                      
                    }
                }
            }
        }

        private void mnuAddTextInput_Click(object sender, EventArgs e) {
            AddNode(new TextInputNode());
        }
        private void mnuAddTrainInput_Click(object sender, EventArgs e) {
            AddNode(new TrainInputNode());
        }
        private void mnuAddNodeTextFile_Click(object sender, EventArgs e) {
            AddNode(new TextFileNode());
        }
        private void mnuAddNodeWebPage_Click(object sender, EventArgs e) {
            AddNode(new WebGetNode());
        }
        private void AddNode(ProjectNode node) {
            var ptn = treeView.SelectedNode ?? nodProject;

            if (ptn.Name == "nodes")
                ptn = ptn.Parent;

            var pn = ptn.Tag as ProjectNode;

            if (pn == null) {
                project.Add(node);
            } else {
                pn.Add(node);
            }
            RefreshTree();
        }



        private void mnuTask_Opening(object sender, System.ComponentModel.CancelEventArgs e) {
            
        }

        private void mnuAddSD_DropDownOpening(object sender, EventArgs e) {
            var modelAvailable = project.Manager.Available.Contains(Models.SentenceDetector);
            
            mnuSDDetect.Enabled = modelAvailable;
            mnuSDEvaluate.Enabled = modelAvailable;
        }

        private void mnuProjectRefresh_Click(object sender, EventArgs e) {
            RefreshTree();
        }



        private void toolNewProject_Click(object sender, EventArgs e) {
            LoadProject(new Project.Project());
        }

        private void toolStart_Click(object sender, EventArgs e) {
            if (project.Validate()) {
                toolStart.Enabled = false;
                toolStop.Enabled = true;

                project.Run();
            }
                
        }

        private void validateToolStripMenuItem_Click(object sender, EventArgs e) {
            project.Validate();
        }

        private void listErrors_ItemActivate(object sender, EventArgs e) {
            if (listErrors.SelectedItems.Count > 0 && listErrors.SelectedItems[0].Tag != null)
                propertyGrid.SelectedObject = listErrors.SelectedItems[0].Tag;

        }

        private void toolValidate_Click(object sender, EventArgs e) {
            project.Validate();
        }

        private void toolSaveProject_Click(object sender, EventArgs e) {

            var save = new SaveFileDialog {
                Filter = @"SharpNL Project File (*.snl)|*.snl"
            };

            if (save.ShowDialog() != DialogResult.OK) return;

            using (var file = new FileStream(save.FileName, FileMode.Create, FileAccess.Write)) {
                project.Serialize(file);
                project.IsDirty = false;
            }
        }

        private void toolProjectOpen_Click(object sender, EventArgs e) {
            var open = new OpenFileDialog {
                Filter = @"SharpNL Project File (*.snl)|*.snl"
            };

            if (open.ShowDialog() != DialogResult.OK) return;

            using (var file = new FileStream(open.FileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                LoadProject(Project.Project.Deserialize(file));
            }
        }

        private void treeView_DoubleClick(object sender, EventArgs e) {
            if (treeView.SelectedNode == null || treeView.SelectedNode.Parent == null)
                return;

            if (treeView.SelectedNode.Parent.Name == "outputs" && treeView.SelectedNode.Tag != null) {
                ShowViewer(treeView.SelectedNode.Tag);
            }



        }

        private void ShowViewer(object selectedObject) {
            if (selectedObject == null)
                return;

            if (activeViewer != null) {
                if (activeViewer.SelectedObject == selectedObject)
                    return;

                // remove old viewer
                splitProject.Panel1.Controls.Remove(activeViewer);

                if (activeViewer.merged)
                    ToolStripManager.RevertMerge(toolStrip);

                activeViewer.Dispose();
            }

            var type = Manager.GetViewer(selectedObject.GetType());
            if (type != null) {
                activeViewer = (Viewer)Activator.CreateInstance(type, new[] {selectedObject});               
                activeViewer.Dock = DockStyle.Fill;
                activeViewer.Visible = true;

                // search for toolStrip
                foreach (var control in activeViewer.Controls) {
                    var strip = control as ToolStrip;
                    if (strip != null) {                       
                        if (!strip.AllowMerge)
                            continue;

                        activeViewer.merged = true;

                        ToolStripManager.Merge(strip, toolStrip);
                        strip.Visible = false;
                    }
                }

                splitProject.Panel1.Controls.Add(activeViewer);
            }
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
            toolSaveProject.Enabled = project.IsDirty;
        }

        private void mnuSDDetect_Click(object sender, EventArgs e) {
            AddTask(new SentenceDetectorTask());
        }
        private void mnuSDTrain_Click(object sender, EventArgs e) {
            if (SelectedNode != null) {
                AddTask(new SentenceDetectorTrainTask(project) {
                    Language = SelectedNode.GetProperty("Language", Thread.CurrentThread.CurrentCulture.Name)
                });
            }
        }
        private void mnuEntityRecognize_Click(object sender, EventArgs e) {
            AddTask(new EntityFinderTask());
        }
        private void mnuTokTokenize_Click(object sender, EventArgs e) {
            AddTask(new TokenizerTask());
        }
        private void mnuTokTrain_Click(object sender, EventArgs e) {
            if (SelectedNode != null) {
                AddTask(new TokenizerTrainTask {
                    Language = SelectedNode.GetProperty("Language", Thread.CurrentThread.CurrentCulture.Name)
                });
            }           
        }

        private void mnuChunkingChunk_Click(object sender, EventArgs e) {
            AddTask(new ChunkerTask());
        }
        private void mnuDocTagger_Click(object sender, EventArgs e) {
            AddTask(new POSTagTask());
        }
        private void mnuDocParser_Click(object sender, EventArgs e) {
            AddTask(new ParserTask());
        }

        private T AddTask<T>(T task) where T : ProjectTask {
            var ptn = treeView.SelectedNode ?? nodProject;

            if (ptn.Name == "tasks")
                ptn = ptn.Parent;

            var pn = ptn.Tag as ProjectNode;
            if (pn != null) {
                pn.Add(task);
            }
            RefreshTree();

            return task;
        }

        private ProjectNode SelectedNode {
            get {
                if (treeView.SelectedNode == null)
                    return null;

                if (treeView.SelectedNode.Tag is ProjectNode)
                    return treeView.SelectedNode.Tag as ProjectNode;
                
                if (treeView.SelectedNode.Name == "nodes" || treeView.SelectedNode.Name == "tasks") 
                    return treeView.SelectedNode.Parent.Tag as ProjectNode;

                return null;
            }
        }

        private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (e.Node != nodProject)
                e.CancelEdit = true;

        }

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            if (e.Node == nodProject) {
                project.Name = e.Label == Resources.Project 
                    ? string.Empty 
                    : e.Label;
            }
        }

        private void toolStop_Click(object sender, EventArgs e) {
            project.Stop();
        }

        private void mnuIOSaveModel_Click(object sender, EventArgs e) {
            if (SelectedNode != null) {
                var type = ModelWriterTask.ModelType.None;

                // automatically detect the model type ;)
                var outputs = SelectedNode.GetTasksOutputs();
                foreach (var output in outputs) {
                    if (output == typeof(ChunkerModel)) {
                        type = ModelWriterTask.ModelType.Chunker;
                        break;
                    }
                    if (output == typeof (DocumentCategorizerModel)) {
                        type = ModelWriterTask.ModelType.DocumentCategorizer;
                        break;
                    }
                    if (output == typeof (TokenNameFinderModel)) {
                        type = ModelWriterTask.ModelType.NameFinder;
                        break;
                    }
                    if (output == typeof (ParserModel)) {
                        type = ModelWriterTask.ModelType.Parser;
                        break;
                    }
                    if (output == typeof (SentenceModel)) {
                        type = ModelWriterTask.ModelType.SentenceDetector;
                        break;
                    }
                    if (output == typeof (POSModel)) {
                        type = ModelWriterTask.ModelType.PoS;
                        break;
                    }
                    if (output == typeof (TokenizerModel)) {
                        type = ModelWriterTask.ModelType.Tokenizer;
                        break;
                    }
                }

                var task = AddTask(new ModelWriterTask());

                task.Type = type;
            }
            
        }






    }
}