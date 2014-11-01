using SharpNL.Gui.Controls;

namespace SharpNL.Gui.Forms {
    partial class frmProject {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProject));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView = new SharpNL.Gui.Controls.Tree();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.splitProject = new System.Windows.Forms.SplitContainer();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabLog = new System.Windows.Forms.TabPage();
            this.richLog = new SharpNL.Gui.Controls.RichLog();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.listErrors = new System.Windows.Forms.ListView();
            this.chErrDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chErrObj = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolNewProject = new System.Windows.Forms.ToolStripButton();
            this.toolSaveProject = new System.Windows.Forms.ToolStripButton();
            this.toolProjectOpen = new System.Windows.Forms.ToolStripButton();
            this.toolValidate = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStart = new System.Windows.Forms.ToolStripButton();
            this.toolStop = new System.Windows.Forms.ToolStripButton();
            this.mnuProject = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddModel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAddTextFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddTextInput = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddTrainInput = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddWebPage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuProjectRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.validateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTask = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddChunking = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChunkingChunk = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddEntity = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEntityRecognize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddParsing = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocParser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddPOS = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocTagger = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddSD = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSDDetect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSDEvaluate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSDTrain = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddTokenizer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTokTokenize = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuIO = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIOSaveModel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddNodeTextFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddNodeTextInput = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddNodeTrainInput = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAddNodeWebPage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTokTrain = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitProject)).BeginInit();
            this.splitProject.Panel2.SuspendLayout();
            this.splitProject.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabLog.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.mnuProject.SuspendLayout();
            this.mnuTask.SuspendLayout();
            this.mnuNode.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.splitContainer.Panel1, "splitContainer.Panel1");
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.splitProject);
            this.splitContainer.Panel2.Controls.Add(this.toolStrip);
            resources.ApplyResources(this.splitContainer.Panel2, "splitContainer.Panel2");
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid);
            // 
            // treeView
            // 
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.ImageList = this.imageList;
            this.treeView.LabelEdit = true;
            this.treeView.Name = "treeView";
            this.treeView.ShowRootLines = false;
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
            this.treeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView_KeyUp);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "project");
            this.imageList.Images.SetKeyName(1, "project_error");
            this.imageList.Images.SetKeyName(2, "model");
            this.imageList.Images.SetKeyName(3, "model_error");
            this.imageList.Images.SetKeyName(4, "model_save");
            this.imageList.Images.SetKeyName(5, "model_warning");
            this.imageList.Images.SetKeyName(6, "models");
            this.imageList.Images.SetKeyName(7, "models_error");
            this.imageList.Images.SetKeyName(8, "models_warning");
            this.imageList.Images.SetKeyName(9, "textfile");
            this.imageList.Images.SetKeyName(10, "web");
            this.imageList.Images.SetKeyName(11, "task");
            this.imageList.Images.SetKeyName(12, "task_error");
            this.imageList.Images.SetKeyName(13, "task_warning");
            this.imageList.Images.SetKeyName(14, "tasks");
            this.imageList.Images.SetKeyName(15, "tasks_error");
            this.imageList.Images.SetKeyName(16, "tasks_warning");
            this.imageList.Images.SetKeyName(17, "nodes");
            this.imageList.Images.SetKeyName(18, "nodes_error");
            this.imageList.Images.SetKeyName(19, "nodes_warning");
            this.imageList.Images.SetKeyName(20, "sd");
            this.imageList.Images.SetKeyName(21, "console");
            this.imageList.Images.SetKeyName(22, "error");
            this.imageList.Images.SetKeyName(23, "outputs");
            this.imageList.Images.SetKeyName(24, "tokenize");
            this.imageList.Images.SetKeyName(25, "entity_recognize");
            this.imageList.Images.SetKeyName(26, "chunk");
            this.imageList.Images.SetKeyName(27, "postag");
            this.imageList.Images.SetKeyName(28, "textinput");
            this.imageList.Images.SetKeyName(29, "parse");
            this.imageList.Images.SetKeyName(30, "document");
            this.imageList.Images.SetKeyName(31, "train");
            this.imageList.Images.SetKeyName(32, "train_input");
            // 
            // propertyGrid
            // 
            resources.ApplyResources(this.propertyGrid, "propertyGrid");
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // splitProject
            // 
            resources.ApplyResources(this.splitProject, "splitProject");
            this.splitProject.Name = "splitProject";
            // 
            // splitProject.Panel1
            // 
            resources.ApplyResources(this.splitProject.Panel1, "splitProject.Panel1");
            // 
            // splitProject.Panel2
            // 
            this.splitProject.Panel2.Controls.Add(this.tabControl);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabLog);
            this.tabControl.Controls.Add(this.tabErrors);
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.ImageList = this.imageList;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // tabLog
            // 
            this.tabLog.Controls.Add(this.richLog);
            resources.ApplyResources(this.tabLog, "tabLog");
            this.tabLog.Name = "tabLog";
            this.tabLog.UseVisualStyleBackColor = true;
            // 
            // richLog
            // 
            this.richLog.BackColor = System.Drawing.Color.White;
            this.richLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.richLog, "richLog");
            this.richLog.Name = "richLog";
            this.richLog.ReadOnly = true;
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.listErrors);
            resources.ApplyResources(this.tabErrors, "tabErrors");
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // listErrors
            // 
            this.listErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chErrDesc,
            this.chErrObj});
            resources.ApplyResources(this.listErrors, "listErrors");
            this.listErrors.FullRowSelect = true;
            this.listErrors.Name = "listErrors";
            this.listErrors.ShowGroups = false;
            this.listErrors.UseCompatibleStateImageBehavior = false;
            this.listErrors.View = System.Windows.Forms.View.Details;
            this.listErrors.ItemActivate += new System.EventHandler(this.listErrors_ItemActivate);
            // 
            // chErrDesc
            // 
            resources.ApplyResources(this.chErrDesc, "chErrDesc");
            // 
            // chErrObj
            // 
            resources.ApplyResources(this.chErrObj, "chErrObj");
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolNewProject,
            this.toolSaveProject,
            this.toolProjectOpen,
            this.toolValidate,
            this.toolStripSeparator1,
            this.toolStart,
            this.toolStop});
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            // 
            // toolNewProject
            // 
            this.toolNewProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolNewProject.Image = global::SharpNL.Gui.Properties.Resources.page;
            resources.ApplyResources(this.toolNewProject, "toolNewProject");
            this.toolNewProject.Name = "toolNewProject";
            this.toolNewProject.Click += new System.EventHandler(this.toolNewProject_Click);
            // 
            // toolSaveProject
            // 
            this.toolSaveProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolSaveProject, "toolSaveProject");
            this.toolSaveProject.Image = global::SharpNL.Gui.Properties.Resources.page_save;
            this.toolSaveProject.Name = "toolSaveProject";
            this.toolSaveProject.Click += new System.EventHandler(this.toolSaveProject_Click);
            // 
            // toolProjectOpen
            // 
            this.toolProjectOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolProjectOpen.Image = global::SharpNL.Gui.Properties.Resources.folder_page;
            resources.ApplyResources(this.toolProjectOpen, "toolProjectOpen");
            this.toolProjectOpen.Name = "toolProjectOpen";
            this.toolProjectOpen.Click += new System.EventHandler(this.toolProjectOpen_Click);
            // 
            // toolValidate
            // 
            this.toolValidate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolValidate, "toolValidate");
            this.toolValidate.Image = global::SharpNL.Gui.Properties.Resources.check;
            this.toolValidate.Name = "toolValidate";
            this.toolValidate.Click += new System.EventHandler(this.toolValidate_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStart
            // 
            resources.ApplyResources(this.toolStart, "toolStart");
            this.toolStart.Image = global::SharpNL.Gui.Properties.Resources.play;
            this.toolStart.Name = "toolStart";
            this.toolStart.Click += new System.EventHandler(this.toolStart_Click);
            // 
            // toolStop
            // 
            this.toolStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.toolStop, "toolStop");
            this.toolStop.Image = global::SharpNL.Gui.Properties.Resources.stop;
            this.toolStop.Name = "toolStop";
            this.toolStop.Click += new System.EventHandler(this.toolStop_Click);
            // 
            // mnuProject
            // 
            this.mnuProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAdd,
            this.toolStripMenuItem1,
            this.mnuProjectRefresh,
            this.validateToolStripMenuItem});
            this.mnuProject.Name = "contextMenuStrip1";
            resources.ApplyResources(this.mnuProject, "mnuProject");
            // 
            // mnuAdd
            // 
            this.mnuAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddModel,
            this.toolStripMenuItem3,
            this.mnuAddTextFile,
            this.mnuAddTextInput,
            this.mnuAddTrainInput,
            this.mnuAddWebPage});
            this.mnuAdd.Image = global::SharpNL.Gui.Properties.Resources.add;
            this.mnuAdd.Name = "mnuAdd";
            resources.ApplyResources(this.mnuAdd, "mnuAdd");
            // 
            // mnuAddModel
            // 
            this.mnuAddModel.Image = global::SharpNL.Gui.Properties.Resources.brick;
            this.mnuAddModel.Name = "mnuAddModel";
            resources.ApplyResources(this.mnuAddModel, "mnuAddModel");
            this.mnuAddModel.Click += new System.EventHandler(this.mnuAddModel_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // mnuAddTextFile
            // 
            this.mnuAddTextFile.Image = global::SharpNL.Gui.Properties.Resources.text_dropcaps;
            this.mnuAddTextFile.Name = "mnuAddTextFile";
            resources.ApplyResources(this.mnuAddTextFile, "mnuAddTextFile");
            this.mnuAddTextFile.Click += new System.EventHandler(this.mnuAddNodeTextFile_Click);
            // 
            // mnuAddTextInput
            // 
            this.mnuAddTextInput.Image = global::SharpNL.Gui.Properties.Resources.text_align_justify;
            this.mnuAddTextInput.Name = "mnuAddTextInput";
            resources.ApplyResources(this.mnuAddTextInput, "mnuAddTextInput");
            this.mnuAddTextInput.Click += new System.EventHandler(this.mnuAddTextInput_Click);
            // 
            // mnuAddTrainInput
            // 
            this.mnuAddTrainInput.Image = global::SharpNL.Gui.Properties.Resources.computer_go;
            this.mnuAddTrainInput.Name = "mnuAddTrainInput";
            resources.ApplyResources(this.mnuAddTrainInput, "mnuAddTrainInput");
            this.mnuAddTrainInput.Click += new System.EventHandler(this.mnuAddTrainInput_Click);
            // 
            // mnuAddWebPage
            // 
            this.mnuAddWebPage.Image = global::SharpNL.Gui.Properties.Resources.world;
            this.mnuAddWebPage.Name = "mnuAddWebPage";
            resources.ApplyResources(this.mnuAddWebPage, "mnuAddWebPage");
            this.mnuAddWebPage.Click += new System.EventHandler(this.mnuAddNodeWebPage_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // mnuProjectRefresh
            // 
            this.mnuProjectRefresh.Image = global::SharpNL.Gui.Properties.Resources.arrow_refresh;
            this.mnuProjectRefresh.Name = "mnuProjectRefresh";
            resources.ApplyResources(this.mnuProjectRefresh, "mnuProjectRefresh");
            this.mnuProjectRefresh.Click += new System.EventHandler(this.mnuProjectRefresh_Click);
            // 
            // validateToolStripMenuItem
            // 
            this.validateToolStripMenuItem.Image = global::SharpNL.Gui.Properties.Resources.check;
            this.validateToolStripMenuItem.Name = "validateToolStripMenuItem";
            resources.ApplyResources(this.validateToolStripMenuItem, "validateToolStripMenuItem");
            this.validateToolStripMenuItem.Click += new System.EventHandler(this.validateToolStripMenuItem_Click);
            // 
            // mnuTask
            // 
            this.mnuTask.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddChunking,
            this.mnuAddEntity,
            this.mnuAddParsing,
            this.mnuAddPOS,
            this.mnuAddSD,
            this.mnuAddTokenizer,
            this.toolStripMenuItem2,
            this.mnuIO});
            this.mnuTask.Name = "mnuTask";
            resources.ApplyResources(this.mnuTask, "mnuTask");
            this.mnuTask.Opening += new System.ComponentModel.CancelEventHandler(this.mnuTask_Opening);
            // 
            // mnuAddChunking
            // 
            this.mnuAddChunking.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuChunkingChunk});
            this.mnuAddChunking.Image = global::SharpNL.Gui.Properties.Resources.timeline_marker;
            this.mnuAddChunking.Name = "mnuAddChunking";
            resources.ApplyResources(this.mnuAddChunking, "mnuAddChunking");
            // 
            // mnuChunkingChunk
            // 
            this.mnuChunkingChunk.Image = global::SharpNL.Gui.Properties.Resources.zoom;
            this.mnuChunkingChunk.Name = "mnuChunkingChunk";
            resources.ApplyResources(this.mnuChunkingChunk, "mnuChunkingChunk");
            this.mnuChunkingChunk.Click += new System.EventHandler(this.mnuChunkingChunk_Click);
            // 
            // mnuAddEntity
            // 
            this.mnuAddEntity.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEntityRecognize});
            this.mnuAddEntity.Image = global::SharpNL.Gui.Properties.Resources.text_signature;
            this.mnuAddEntity.Name = "mnuAddEntity";
            resources.ApplyResources(this.mnuAddEntity, "mnuAddEntity");
            // 
            // mnuEntityRecognize
            // 
            this.mnuEntityRecognize.Image = global::SharpNL.Gui.Properties.Resources.zoom;
            this.mnuEntityRecognize.Name = "mnuEntityRecognize";
            resources.ApplyResources(this.mnuEntityRecognize, "mnuEntityRecognize");
            this.mnuEntityRecognize.Click += new System.EventHandler(this.mnuEntityRecognize_Click);
            // 
            // mnuAddParsing
            // 
            this.mnuAddParsing.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDocParser});
            this.mnuAddParsing.Image = global::SharpNL.Gui.Properties.Resources.text_uppercase;
            this.mnuAddParsing.Name = "mnuAddParsing";
            resources.ApplyResources(this.mnuAddParsing, "mnuAddParsing");
            // 
            // mnuDocParser
            // 
            this.mnuDocParser.Image = global::SharpNL.Gui.Properties.Resources.zoom;
            this.mnuDocParser.Name = "mnuDocParser";
            resources.ApplyResources(this.mnuDocParser, "mnuDocParser");
            this.mnuDocParser.Click += new System.EventHandler(this.mnuDocParser_Click);
            // 
            // mnuAddPOS
            // 
            this.mnuAddPOS.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDocTagger});
            this.mnuAddPOS.Image = global::SharpNL.Gui.Properties.Resources.sitemap_color;
            this.mnuAddPOS.Name = "mnuAddPOS";
            resources.ApplyResources(this.mnuAddPOS, "mnuAddPOS");
            // 
            // mnuDocTagger
            // 
            this.mnuDocTagger.Image = global::SharpNL.Gui.Properties.Resources.zoom;
            this.mnuDocTagger.Name = "mnuDocTagger";
            resources.ApplyResources(this.mnuDocTagger, "mnuDocTagger");
            this.mnuDocTagger.Click += new System.EventHandler(this.mnuDocTagger_Click);
            // 
            // mnuAddSD
            // 
            this.mnuAddSD.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSDDetect,
            this.mnuSDEvaluate,
            this.mnuSDTrain});
            this.mnuAddSD.Image = global::SharpNL.Gui.Properties.Resources.text_horizontalrule;
            this.mnuAddSD.Name = "mnuAddSD";
            resources.ApplyResources(this.mnuAddSD, "mnuAddSD");
            this.mnuAddSD.DropDownOpening += new System.EventHandler(this.mnuAddSD_DropDownOpening);
            // 
            // mnuSDDetect
            // 
            this.mnuSDDetect.Image = global::SharpNL.Gui.Properties.Resources.zoom;
            this.mnuSDDetect.Name = "mnuSDDetect";
            resources.ApplyResources(this.mnuSDDetect, "mnuSDDetect");
            this.mnuSDDetect.Click += new System.EventHandler(this.mnuSDDetect_Click);
            // 
            // mnuSDEvaluate
            // 
            this.mnuSDEvaluate.Image = global::SharpNL.Gui.Properties.Resources.chart_curve;
            this.mnuSDEvaluate.Name = "mnuSDEvaluate";
            resources.ApplyResources(this.mnuSDEvaluate, "mnuSDEvaluate");
            // 
            // mnuSDTrain
            // 
            this.mnuSDTrain.Image = global::SharpNL.Gui.Properties.Resources.computer;
            this.mnuSDTrain.Name = "mnuSDTrain";
            resources.ApplyResources(this.mnuSDTrain, "mnuSDTrain");
            this.mnuSDTrain.Click += new System.EventHandler(this.mnuSDTrain_Click);
            // 
            // mnuAddTokenizer
            // 
            this.mnuAddTokenizer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTokTokenize,
            this.mnuTokTrain});
            this.mnuAddTokenizer.Image = global::SharpNL.Gui.Properties.Resources.text_allcaps;
            this.mnuAddTokenizer.Name = "mnuAddTokenizer";
            resources.ApplyResources(this.mnuAddTokenizer, "mnuAddTokenizer");
            // 
            // mnuTokTokenize
            // 
            this.mnuTokTokenize.Image = global::SharpNL.Gui.Properties.Resources.zoom;
            this.mnuTokTokenize.Name = "mnuTokTokenize";
            resources.ApplyResources(this.mnuTokTokenize, "mnuTokTokenize");
            this.mnuTokTokenize.Click += new System.EventHandler(this.mnuTokTokenize_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // mnuIO
            // 
            this.mnuIO.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuIOSaveModel});
            this.mnuIO.Image = global::SharpNL.Gui.Properties.Resources.folder_wrench;
            this.mnuIO.Name = "mnuIO";
            resources.ApplyResources(this.mnuIO, "mnuIO");
            // 
            // mnuIOSaveModel
            // 
            this.mnuIOSaveModel.Image = global::SharpNL.Gui.Properties.Resources.brick_go;
            this.mnuIOSaveModel.Name = "mnuIOSaveModel";
            resources.ApplyResources(this.mnuIOSaveModel, "mnuIOSaveModel");
            this.mnuIOSaveModel.Click += new System.EventHandler(this.mnuIOSaveModel_Click);
            // 
            // mnuNode
            // 
            this.mnuNode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.mnuNode.Name = "mnuNode";
            resources.ApplyResources(this.mnuNode, "mnuNode");
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddNodeTextFile,
            this.mnuAddNodeTextInput,
            this.mnuAddNodeTrainInput,
            this.mnuAddNodeWebPage});
            this.addToolStripMenuItem.Image = global::SharpNL.Gui.Properties.Resources.add;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            // 
            // mnuAddNodeTextFile
            // 
            this.mnuAddNodeTextFile.Image = global::SharpNL.Gui.Properties.Resources.text_dropcaps;
            this.mnuAddNodeTextFile.Name = "mnuAddNodeTextFile";
            resources.ApplyResources(this.mnuAddNodeTextFile, "mnuAddNodeTextFile");
            this.mnuAddNodeTextFile.Click += new System.EventHandler(this.mnuAddNodeTextFile_Click);
            // 
            // mnuAddNodeTextInput
            // 
            this.mnuAddNodeTextInput.Image = global::SharpNL.Gui.Properties.Resources.text_align_justify;
            this.mnuAddNodeTextInput.Name = "mnuAddNodeTextInput";
            resources.ApplyResources(this.mnuAddNodeTextInput, "mnuAddNodeTextInput");
            this.mnuAddNodeTextInput.Click += new System.EventHandler(this.mnuAddTextInput_Click);
            // 
            // mnuAddNodeTrainInput
            // 
            this.mnuAddNodeTrainInput.Image = global::SharpNL.Gui.Properties.Resources.computer_go;
            this.mnuAddNodeTrainInput.Name = "mnuAddNodeTrainInput";
            resources.ApplyResources(this.mnuAddNodeTrainInput, "mnuAddNodeTrainInput");
            this.mnuAddNodeTrainInput.Click += new System.EventHandler(this.mnuAddTrainInput_Click);
            // 
            // mnuAddNodeWebPage
            // 
            this.mnuAddNodeWebPage.Image = global::SharpNL.Gui.Properties.Resources.world;
            this.mnuAddNodeWebPage.Name = "mnuAddNodeWebPage";
            resources.ApplyResources(this.mnuAddNodeWebPage, "mnuAddNodeWebPage");
            this.mnuAddNodeWebPage.Click += new System.EventHandler(this.mnuAddNodeWebPage_Click);
            // 
            // mnuTokTrain
            // 
            this.mnuTokTrain.Image = global::SharpNL.Gui.Properties.Resources.computer;
            this.mnuTokTrain.Name = "mnuTokTrain";
            resources.ApplyResources(this.mnuTokTrain, "mnuTokTrain");
            this.mnuTokTrain.Click += new System.EventHandler(this.mnuTokTrain_Click);
            // 
            // frmProject
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer);
            this.Name = "frmProject";
            this.Load += new System.EventHandler(this.frmProject_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitProject.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitProject)).EndInit();
            this.splitProject.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabLog.ResumeLayout(false);
            this.tabErrors.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.mnuProject.ResumeLayout(false);
            this.mnuTask.ResumeLayout(false);
            this.mnuNode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ContextMenuStrip mnuProject;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Tree treeView;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ContextMenuStrip mnuTask;
        private System.Windows.Forms.ToolStripMenuItem mnuAddChunking;
        private System.Windows.Forms.ToolStripMenuItem mnuAddEntity;
        private System.Windows.Forms.ToolStripMenuItem mnuAddParsing;
        private System.Windows.Forms.ToolStripMenuItem mnuAddSD;
        private System.Windows.Forms.ToolStripMenuItem mnuAddTokenizer;
        private System.Windows.Forms.ToolStripMenuItem mnuTokTokenize;
        private System.Windows.Forms.ToolStripMenuItem mnuAddPOS;
        private System.Windows.Forms.ToolStripMenuItem mnuSDDetect;
        private System.Windows.Forms.ToolStripMenuItem mnuSDEvaluate;
        private System.Windows.Forms.ToolStripMenuItem mnuSDTrain;
        private System.Windows.Forms.ContextMenuStrip mnuNode;
        private System.Windows.Forms.ToolStripMenuItem mnuAdd;
        private System.Windows.Forms.ToolStripMenuItem mnuAddModel;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem mnuAddTextFile;
        private System.Windows.Forms.ToolStripMenuItem mnuAddWebPage;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuAddNodeTextFile;
        private System.Windows.Forms.ToolStripMenuItem mnuAddNodeWebPage;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuProjectRefresh;
        private System.Windows.Forms.ToolStripButton toolSaveProject;
        private System.Windows.Forms.ToolStripButton toolNewProject;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStart;
        private System.Windows.Forms.ToolStripButton toolStop;
        private System.Windows.Forms.SplitContainer splitProject;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabLog;
        private RichLog richLog;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.ListView listErrors;
        private System.Windows.Forms.ColumnHeader chErrDesc;
        private System.Windows.Forms.ColumnHeader chErrObj;
        private System.Windows.Forms.ToolStripMenuItem validateToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolValidate;
        private System.Windows.Forms.ToolStripButton toolProjectOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuEntityRecognize;
        private System.Windows.Forms.ToolStripMenuItem mnuChunkingChunk;
        private System.Windows.Forms.ToolStripMenuItem mnuDocTagger;
        private System.Windows.Forms.ToolStripMenuItem mnuDocParser;
        private System.Windows.Forms.ToolStripMenuItem mnuAddTextInput;
        internal System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStripMenuItem mnuAddNodeTextInput;
        private System.Windows.Forms.ToolStripMenuItem mnuAddNodeTrainInput;
        private System.Windows.Forms.ToolStripMenuItem mnuAddTrainInput;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuIO;
        private System.Windows.Forms.ToolStripMenuItem mnuIOSaveModel;
        private System.Windows.Forms.ToolStripMenuItem mnuTokTrain;
    }
}