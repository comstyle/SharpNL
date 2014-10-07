using SharpNL.Gui.Controls;

namespace SharpNL.Gui.Forms {
    partial class frmMain {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pnNav = new System.Windows.Forms.Panel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabSentence = new System.Windows.Forms.TabPage();
            this.btnSentExec = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSentIncTitles = new System.Windows.Forms.CheckBox();
            this.cmbSentEncoding = new System.Windows.Forms.ComboBox();
            this.cmbSentLang = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbSentValidate = new System.Windows.Forms.RadioButton();
            this.rdbSentConvert = new System.Windows.Forms.RadioButton();
            this.rdbSentEval = new System.Windows.Forms.RadioButton();
            this.rdbSentTraining = new System.Windows.Forms.RadioButton();
            this.rdbSentDetection = new System.Windows.Forms.RadioButton();
            this.btnSentData = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.pnLog = new System.Windows.Forms.Panel();
            this.richLog = new SharpNL.Gui.Controls.RichLog();
            this.lnkAbout = new System.Windows.Forms.LinkLabel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnNav.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabSentence.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 511);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(740, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // pnNav
            // 
            this.pnNav.Controls.Add(this.tabs);
            this.pnNav.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnNav.Location = new System.Drawing.Point(0, 70);
            this.pnNav.Name = "pnNav";
            this.pnNav.Padding = new System.Windows.Forms.Padding(3);
            this.pnNav.Size = new System.Drawing.Size(740, 180);
            this.pnNav.TabIndex = 4;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabSentence);
            this.tabs.Controls.Add(this.tabPage2);
            this.tabs.Controls.Add(this.tabPage1);
            this.tabs.Controls.Add(this.tabPage3);
            this.tabs.Controls.Add(this.tabPage4);
            this.tabs.Controls.Add(this.tabPage5);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(3, 3);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(734, 174);
            this.tabs.TabIndex = 3;
            // 
            // tabSentence
            // 
            this.tabSentence.Controls.Add(this.btnSentExec);
            this.tabSentence.Controls.Add(this.label3);
            this.tabSentence.Controls.Add(this.label2);
            this.tabSentence.Controls.Add(this.chkSentIncTitles);
            this.tabSentence.Controls.Add(this.cmbSentEncoding);
            this.tabSentence.Controls.Add(this.cmbSentLang);
            this.tabSentence.Controls.Add(this.groupBox1);
            this.tabSentence.Controls.Add(this.btnSentData);
            this.tabSentence.Location = new System.Drawing.Point(4, 24);
            this.tabSentence.Name = "tabSentence";
            this.tabSentence.Padding = new System.Windows.Forms.Padding(3);
            this.tabSentence.Size = new System.Drawing.Size(726, 146);
            this.tabSentence.TabIndex = 0;
            this.tabSentence.Text = "Sentence Detection";
            this.tabSentence.UseVisualStyleBackColor = true;
            // 
            // btnSentExec
            // 
            this.btnSentExec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSentExec.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSentExec.Image = global::SharpNL.Gui.Properties.Resources.ok;
            this.btnSentExec.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSentExec.Location = new System.Drawing.Point(614, 102);
            this.btnSentExec.Name = "btnSentExec";
            this.btnSentExec.Size = new System.Drawing.Size(103, 27);
            this.btnSentExec.TabIndex = 9;
            this.btnSentExec.Text = "&Execute";
            this.btnSentExec.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSentExec.UseVisualStyleBackColor = true;
            this.btnSentExec.Click += new System.EventHandler(this.btnSentExec_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(344, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 15);
            this.label3.TabIndex = 23;
            this.label3.Text = "Language";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(157, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 22;
            this.label2.Text = "Encoding";
            // 
            // chkSentIncTitles
            // 
            this.chkSentIncTitles.AutoSize = true;
            this.chkSentIncTitles.Location = new System.Drawing.Point(160, 91);
            this.chkSentIncTitles.Name = "chkSentIncTitles";
            this.chkSentIncTitles.Size = new System.Drawing.Size(97, 19);
            this.chkSentIncTitles.TabIndex = 8;
            this.chkSentIncTitles.Text = "Include Titles";
            this.chkSentIncTitles.UseVisualStyleBackColor = true;
            // 
            // cmbSentEncoding
            // 
            this.cmbSentEncoding.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbSentEncoding.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSentEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSentEncoding.Location = new System.Drawing.Point(160, 61);
            this.cmbSentEncoding.Name = "cmbSentEncoding";
            this.cmbSentEncoding.Size = new System.Drawing.Size(181, 23);
            this.cmbSentEncoding.TabIndex = 6;
            // 
            // cmbSentLang
            // 
            this.cmbSentLang.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmbSentLang.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbSentLang.Items.AddRange(new object[] {
            "af Afrikaans\r",
            "af-ZA Afrikaans (South Africa)\r",
            "ar Arabic\r",
            "ar-AE Arabic (U.A.E.)\r",
            "ar-BH Arabic (Bahrain)\r",
            "ar-DZ Arabic (Algeria)\r",
            "ar-EG Arabic (Egypt)\r",
            "ar-IQ Arabic (Iraq)\r",
            "ar-JO Arabic (Jordan)\r",
            "ar-KW Arabic (Kuwait)\r",
            "ar-LB Arabic (Lebanon)\r",
            "ar-LY Arabic (Libya)\r",
            "ar-MA Arabic (Morocco)\r",
            "ar-OM Arabic (Oman)\r",
            "ar-QA Arabic (Qatar)\r",
            "ar-SA Arabic (Saudi Arabia)\r",
            "ar-SY Arabic (Syria)\r",
            "ar-TN Arabic (Tunisia)\r",
            "ar-YE Arabic (Yemen)\r",
            "az Azeri (Latin)\r",
            "az-AZ Azeri (Latin) (Azerbaijan)\r",
            "az-AZ Azeri (Cyrillic) (Azerbaijan)\r",
            "be Belarusian\r",
            "be-BY Belarusian (Belarus)\r",
            "bg Bulgarian\r",
            "bg-BG Bulgarian (Bulgaria)\r",
            "bs-BA Bosnian (Bosnia and Herzegovina)\r",
            "ca Catalan\r",
            "ca-ES Catalan (Spain)\r",
            "cs Czech\r",
            "cs-CZ Czech (Czech Republic)\r",
            "cy Welsh\r",
            "cy-GB Welsh (United Kingdom)\r",
            "da Danish\r",
            "da-DK Danish (Denmark)\r",
            "de German\r",
            "de-AT German (Austria)\r",
            "de-CH German (Switzerland)\r",
            "de-DE German (Germany)\r",
            "de-LI German (Liechtenstein)\r",
            "de-LU German (Luxembourg)\r",
            "dv Divehi\r",
            "dv-MV Divehi (Maldives)\r",
            "el Greek\r",
            "el-GR Greek (Greece)\r",
            "en English\r",
            "en-AU English (Australia)\r",
            "en-BZ English (Belize)\r",
            "en-CA English (Canada)\r",
            "en-CB English (Caribbean)\r",
            "en-GB English (United Kingdom)\r",
            "en-IE English (Ireland)\r",
            "en-JM English (Jamaica)\r",
            "en-NZ English (New Zealand)\r",
            "en-PH English (Republic of the Philippines)\r",
            "en-TT English (Trinidad and Tobago)\r",
            "en-US English (United States)\r",
            "en-ZA English (South Africa)\r",
            "en-ZW English (Zimbabwe)\r",
            "eo Esperanto\r",
            "es Spanish\r",
            "es-AR Spanish (Argentina)\r",
            "es-BO Spanish (Bolivia)\r",
            "es-CL Spanish (Chile)\r",
            "es-CO Spanish (Colombia)\r",
            "es-CR Spanish (Costa Rica)\r",
            "es-DO Spanish (Dominican Republic)\r",
            "es-EC Spanish (Ecuador)\r",
            "es-ES Spanish (Castilian)\r",
            "es-ES Spanish (Spain)\r",
            "es-GT Spanish (Guatemala)\r",
            "es-HN Spanish (Honduras)\r",
            "es-MX Spanish (Mexico)\r",
            "es-NI Spanish (Nicaragua)\r",
            "es-PA Spanish (Panama)\r",
            "es-PE Spanish (Peru)\r",
            "es-PR Spanish (Puerto Rico)\r",
            "es-PY Spanish (Paraguay)\r",
            "es-SV Spanish (El Salvador)\r",
            "es-UY Spanish (Uruguay)\r",
            "es-VE Spanish (Venezuela)\r",
            "et Estonian\r",
            "et-EE Estonian (Estonia)\r",
            "eu Basque\r",
            "eu-ES Basque (Spain)\r",
            "fa Farsi\r",
            "fa-IR Farsi (Iran)\r",
            "fi Finnish\r",
            "fi-FI Finnish (Finland)\r",
            "fo Faroese\r",
            "fo-FO Faroese (Faroe Islands)\r",
            "fr French\r",
            "fr-BE French (Belgium)\r",
            "fr-CA French (Canada)\r",
            "fr-CH French (Switzerland)\r",
            "fr-FR French (France)\r",
            "fr-LU French (Luxembourg)\r",
            "fr-MC French (Principality of Monaco)\r",
            "gl Galician\r",
            "gl-ES Galician (Spain)\r",
            "gu Gujarati\r",
            "gu-IN Gujarati (India)\r",
            "he Hebrew\r",
            "he-IL Hebrew (Israel)\r",
            "hi Hindi\r",
            "hi-IN Hindi (India)\r",
            "hr Croatian\r",
            "hr-BA Croatian (Bosnia and Herzegovina)\r",
            "hr-HR Croatian (Croatia)\r",
            "hu Hungarian\r",
            "hu-HU Hungarian (Hungary)\r",
            "hy Armenian\r",
            "hy-AM Armenian (Armenia)\r",
            "id Indonesian\r",
            "id-ID Indonesian (Indonesia)\r",
            "is Icelandic\r",
            "is-IS Icelandic (Iceland)\r",
            "it Italian\r",
            "it-CH Italian (Switzerland)\r",
            "it-IT Italian (Italy)\r",
            "ja Japanese\r",
            "ja-JP Japanese (Japan)\r",
            "ka Georgian\r",
            "ka-GE Georgian (Georgia)\r",
            "kk Kazakh\r",
            "kk-KZ Kazakh (Kazakhstan)\r",
            "kn Kannada\r",
            "kn-IN Kannada (India)\r",
            "ko Korean\r",
            "ko-KR Korean (Korea)\r",
            "kok Konkani\r",
            "kok-IN Konkani (India)\r",
            "ky Kyrgyz\r",
            "ky-KG Kyrgyz (Kyrgyzstan)\r",
            "lt Lithuanian\r",
            "lt-LT Lithuanian (Lithuania)\r",
            "lv Latvian\r",
            "lv-LV Latvian (Latvia)\r",
            "mi Maori\r",
            "mi-NZ Maori (New Zealand)\r",
            "mk FYRO Macedonian\r",
            "mk-MK FYRO Macedonian (Former Yugoslav Republic of Macedonia)\r",
            "mn Mongolian\r",
            "mn-MN Mongolian (Mongolia)\r",
            "mr Marathi\r",
            "mr-IN Marathi (India)\r",
            "ms Malay\r",
            "ms-BN Malay (Brunei Darussalam)\r",
            "ms-MY Malay (Malaysia)\r",
            "mt Maltese\r",
            "mt-MT Maltese (Malta)\r",
            "nb Norwegian (Bokm?l)\r",
            "nb-NO Norwegian (Bokm?l) (Norway)\r",
            "nl Dutch\r",
            "nl-BE Dutch (Belgium)\r",
            "nl-NL Dutch (Netherlands)\r",
            "nn-NO Norwegian (Nynorsk) (Norway)\r",
            "ns Northern Sotho\r",
            "ns-ZA Northern Sotho (South Africa)\r",
            "pa Punjabi\r",
            "pa-IN Punjabi (India)\r",
            "pl Polish\r",
            "pl-PL Polish (Poland)\r",
            "ps Pashto\r",
            "ps-AR Pashto (Afghanistan)\r",
            "pt Portuguese\r",
            "pt-BR Portuguese (Brazil)\r",
            "pt-PT Portuguese (Portugal)\r",
            "qu Quechua\r",
            "qu-BO Quechua (Bolivia)\r",
            "qu-EC Quechua (Ecuador)\r",
            "qu-PE Quechua (Peru)\r",
            "ro Romanian\r",
            "ro-RO Romanian (Romania)\r",
            "ru Russian\r",
            "ru-RU Russian (Russia)\r",
            "sa Sanskrit\r",
            "sa-IN Sanskrit (India)\r",
            "se Sami (Northern)\r",
            "se-FI Sami (Northern) (Finland)\r",
            "se-FI Sami (Skolt) (Finland)\r",
            "se-FI Sami (Inari) (Finland)\r",
            "se-NO Sami (Northern) (Norway)\r",
            "se-NO Sami (Lule) (Norway)\r",
            "se-NO Sami (Southern) (Norway)\r",
            "se-SE Sami (Northern) (Sweden)\r",
            "se-SE Sami (Lule) (Sweden)\r",
            "se-SE Sami (Southern) (Sweden)\r",
            "sk Slovak\r",
            "sk-SK Slovak (Slovakia)\r",
            "sl Slovenian\r",
            "sl-SI Slovenian (Slovenia)\r",
            "sq Albanian\r",
            "sq-AL Albanian (Albania)\r",
            "sr-BA Serbian (Latin) (Bosnia and Herzegovina)\r",
            "sr-BA Serbian (Cyrillic) (Bosnia and Herzegovina)\r",
            "sr-SP Serbian (Latin) (Serbia and Montenegro)\r",
            "sr-SP Serbian (Cyrillic) (Serbia and Montenegro)\r",
            "sv Swedish\r",
            "sv-FI Swedish (Finland)\r",
            "sv-SE Swedish (Sweden)\r",
            "sw Swahili\r",
            "sw-KE Swahili (Kenya)\r",
            "syr Syriac\r",
            "syr-SY Syriac (Syria)\r",
            "ta Tamil\r",
            "ta-IN Tamil (India)\r",
            "te Telugu\r",
            "te-IN Telugu (India)\r",
            "th Thai\r",
            "th-TH Thai (Thailand)\r",
            "tl Tagalog\r",
            "tl-PH Tagalog (Philippines)\r",
            "tn Tswana\r",
            "tn-ZA Tswana (South Africa)\r",
            "tr Turkish\r",
            "tr-TR Turkish (Turkey)\r",
            "tt Tatar\r",
            "tt-RU Tatar (Russia)\r",
            "ts Tsonga\r",
            "uk Ukrainian\r",
            "uk-UA Ukrainian (Ukraine)\r",
            "ur Urdu\r",
            "ur-PK Urdu (Islamic Republic of Pakistan)\r",
            "uz Uzbek (Latin)\r",
            "uz-UZ Uzbek (Latin) (Uzbekistan)\r",
            "uz-UZ Uzbek (Cyrillic) (Uzbekistan)\r",
            "vi Vietnamese\r",
            "vi-VN Vietnamese (Viet Nam)\r",
            "xh Xhosa\r",
            "xh-ZA Xhosa (South Africa)\r",
            "zh Chinese\r",
            "zh-CN Chinese (S)\r",
            "zh-HK Chinese (Hong Kong)\r",
            "zh-MO Chinese (Macau)\r",
            "zh-SG Chinese (Singapore)\r",
            "zh-TW Chinese (T)\r",
            "zu Zulu\r",
            "zu-ZA Zulu (South Africa)"});
            this.cmbSentLang.Location = new System.Drawing.Point(347, 61);
            this.cmbSentLang.Name = "cmbSentLang";
            this.cmbSentLang.Size = new System.Drawing.Size(181, 23);
            this.cmbSentLang.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.rdbSentValidate);
            this.groupBox1.Controls.Add(this.rdbSentConvert);
            this.groupBox1.Controls.Add(this.rdbSentEval);
            this.groupBox1.Controls.Add(this.rdbSentTraining);
            this.groupBox1.Controls.Add(this.rdbSentDetection);
            this.groupBox1.Location = new System.Drawing.Point(4, -3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(141, 138);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            // 
            // rdbSentValidate
            // 
            this.rdbSentValidate.AutoSize = true;
            this.rdbSentValidate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbSentValidate.Location = new System.Drawing.Point(10, 113);
            this.rdbSentValidate.Name = "rdbSentValidate";
            this.rdbSentValidate.Size = new System.Drawing.Size(73, 19);
            this.rdbSentValidate.TabIndex = 4;
            this.rdbSentValidate.Text = "&Validator";
            this.rdbSentValidate.UseVisualStyleBackColor = true;
            // 
            // rdbSentConvert
            // 
            this.rdbSentConvert.AutoSize = true;
            this.rdbSentConvert.Checked = true;
            this.rdbSentConvert.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbSentConvert.Location = new System.Drawing.Point(10, 17);
            this.rdbSentConvert.Name = "rdbSentConvert";
            this.rdbSentConvert.Size = new System.Drawing.Size(79, 19);
            this.rdbSentConvert.TabIndex = 0;
            this.rdbSentConvert.TabStop = true;
            this.rdbSentConvert.Text = "&Converter";
            this.rdbSentConvert.UseVisualStyleBackColor = true;
            // 
            // rdbSentEval
            // 
            this.rdbSentEval.AutoSize = true;
            this.rdbSentEval.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbSentEval.Location = new System.Drawing.Point(10, 65);
            this.rdbSentEval.Name = "rdbSentEval";
            this.rdbSentEval.Size = new System.Drawing.Size(80, 19);
            this.rdbSentEval.TabIndex = 2;
            this.rdbSentEval.Text = "&Evaluation";
            this.rdbSentEval.UseVisualStyleBackColor = true;
            // 
            // rdbSentTraining
            // 
            this.rdbSentTraining.AutoSize = true;
            this.rdbSentTraining.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbSentTraining.Location = new System.Drawing.Point(10, 89);
            this.rdbSentTraining.Name = "rdbSentTraining";
            this.rdbSentTraining.Size = new System.Drawing.Size(70, 19);
            this.rdbSentTraining.TabIndex = 3;
            this.rdbSentTraining.Text = "&Training";
            this.rdbSentTraining.UseVisualStyleBackColor = true;
            // 
            // rdbSentDetection
            // 
            this.rdbSentDetection.AutoSize = true;
            this.rdbSentDetection.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rdbSentDetection.Location = new System.Drawing.Point(10, 41);
            this.rdbSentDetection.Name = "rdbSentDetection";
            this.rdbSentDetection.Size = new System.Drawing.Size(76, 19);
            this.rdbSentDetection.TabIndex = 1;
            this.rdbSentDetection.Text = "&Detection";
            this.rdbSentDetection.UseVisualStyleBackColor = true;
            // 
            // btnSentData
            // 
            this.btnSentData.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSentData.FlatAppearance.BorderSize = 0;
            this.btnSentData.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.btnSentData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSentData.Image = global::SharpNL.Gui.Properties.Resources.Open;
            this.btnSentData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSentData.Location = new System.Drawing.Point(160, 12);
            this.btnSentData.Name = "btnSentData";
            this.btnSentData.Size = new System.Drawing.Size(497, 25);
            this.btnSentData.TabIndex = 5;
            this.btnSentData.Text = " Select the data file...";
            this.btnSentData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSentData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSentData.UseVisualStyleBackColor = false;
            this.btnSentData.Click += new System.EventHandler(this.btnSentData_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(726, 146);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tokenizer";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(726, 146);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Name Finder";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(726, 146);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "POS Tagger";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(726, 146);
            this.tabPage4.TabIndex = 4;
            this.tabPage4.Text = "Chunker";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 24);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(726, 146);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "Parser";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Sans Unicode", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(85, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 34);
            this.label1.TabIndex = 6;
            this.label1.Text = "SharpNL";
            // 
            // pnLog
            // 
            this.pnLog.Controls.Add(this.richLog);
            this.pnLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnLog.Location = new System.Drawing.Point(0, 250);
            this.pnLog.Name = "pnLog";
            this.pnLog.Padding = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.pnLog.Size = new System.Drawing.Size(740, 261);
            this.pnLog.TabIndex = 10;
            // 
            // richLog
            // 
            this.richLog.BackColor = System.Drawing.Color.White;
            this.richLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richLog.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richLog.Location = new System.Drawing.Point(3, 0);
            this.richLog.Name = "richLog";
            this.richLog.ReadOnly = true;
            this.richLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richLog.Size = new System.Drawing.Size(734, 259);
            this.richLog.TabIndex = 10;
            this.richLog.Text = "";
            // 
            // lnkAbout
            // 
            this.lnkAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkAbout.AutoSize = true;
            this.lnkAbout.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkAbout.Location = new System.Drawing.Point(694, 49);
            this.lnkAbout.Name = "lnkAbout";
            this.lnkAbout.Size = new System.Drawing.Size(40, 15);
            this.lnkAbout.TabIndex = 11;
            this.lnkAbout.TabStop = true;
            this.lnkAbout.Text = "About";
            this.lnkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAbout_LinkClicked);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 64);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(740, 70);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(740, 533);
            this.Controls.Add(this.lnkAbout);
            this.Controls.Add(this.pnLog);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnNav);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(756, 571);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SharpNL.Gui";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.pnNav.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabSentence.ResumeLayout(false);
            this.tabSentence.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnLog.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel pnNav;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabSentence;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Panel pnLog;
        private RichLog richLog;
        private System.Windows.Forms.LinkLabel lnkAbout;
        private System.Windows.Forms.Button btnSentData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdbSentEval;
        private System.Windows.Forms.RadioButton rdbSentTraining;
        private System.Windows.Forms.RadioButton rdbSentDetection;
        private System.Windows.Forms.RadioButton rdbSentValidate;
        private System.Windows.Forms.RadioButton rdbSentConvert;
        private System.Windows.Forms.ComboBox cmbSentLang;
        private System.Windows.Forms.ComboBox cmbSentEncoding;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkSentIncTitles;
        private System.Windows.Forms.Button btnSentExec;
    }
}

