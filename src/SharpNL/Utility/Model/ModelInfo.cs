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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using SharpNL.Chunker;
using SharpNL.NameFind;
using SharpNL.Parser;
using SharpNL.POSTag;
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility.Serialization;

namespace SharpNL.Utility.Model {
    /// <summary>
    /// Provides basic information about a model file. This class cannot be inherited.
    /// </summary>
    public sealed class ModelInfo {

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInfo"/> class, which provides information about the model.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <exception cref="System.IO.FileNotFoundException">The specified file does not exist.</exception>
        /// <exception cref="InvalidFormatException">Invalid model format.</exception>
        public ModelInfo(string fileName) {
            File = new FileInfo(fileName);
            if (!File.Exists)
                throw new FileNotFoundException("The specified file does not exist.", fileName);

            Name = Path.GetFileNameWithoutExtension(File.Name);

            try {
                using (var zip = new ZipInputStream(new FileStream(fileName, FileMode.Open, FileAccess.Read))) {
                    ZipEntry entry;
                    while ((entry = zip.GetNextEntry()) != null) {
                        if (entry.Name == ArtifactProvider.MANIFEST_ENTRY) {
                            Manifest = (Properties)Properties.Deserialize(new UnclosableStream(zip));
                            zip.CloseEntry();
                            break;
                        }
                        zip.CloseEntry();
                    }

                    zip.Close();
                }
            } catch (Exception ex) {
                throw new InvalidFormatException("Invalid model format.", ex);
            }
        }

        #region + Properties .

        #region . File .
        /// <summary>
        /// Gets the file info.
        /// </summary>
        /// <value>The file info.</value>
        [Description("The file name of the associated model.")]
        public FileInfo File { get; private set; }
        #endregion

        #region . Language .
        /// <summary>
        /// Gets the language of the model.
        /// </summary>
        /// <value>The language of the model.</value>
        [Description("The language of the associated model.")]
        public string Language {
            get { return Manifest[ArtifactProvider.LanguageEntry]; }
        }
        #endregion

        #region . ModelType .
        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        [Description("The model type of the associated model.")]
        public Models ModelType {
            get {
                switch (Manifest[ArtifactProvider.ComponentNameEntry]) {
                    case "ChunkerME":
                        return Models.Chunker;
                    case "NameFinderME":
                        return Models.NameFind;
                    case "Parser":
                        return Models.Parser;
                    case "POSTaggerME":
                        return Models.POSTag;
                    case "SentenceDetectorME":
                        return Models.SentenceDetector;
                    case "TokenizerME":
                        return Models.Tokenizer;
                    default:
                        return Models.Unknown;
                }
            }
        }
        #endregion

        #region . Manifest .
        /// <summary>
        /// Gets the manifest.
        /// </summary>
        /// <value>The manifest.</value>
        public Properties Manifest { get; private set; }
        #endregion

        #region . Name .
        /// <summary>
        /// Gets the model name.
        /// </summary>
        /// <value>The model name.</value>
        [Description("The name of the associated model.")]
        public string Name { get; private set; }
        #endregion

        #region . Timestamp .
        /// <summary>
        /// Gets the training timestamp.
        /// </summary>
        /// <value>The training timestamp.</value>
        [Description("When the model was trained model.")]
        public DateTime Timestamp {
            get {
                try {
                    var millis = long.Parse(Manifest[ArtifactProvider.TimestampEntry]);
                    return Library.Jan1st1970.AddMilliseconds(millis);
                } catch (Exception) {
                    return DateTime.MinValue;
                }
            }
        }
        #endregion

        #region . Type .
        /// <summary>
        /// Gets the <see cref="Type"/> of the model.
        /// </summary>
        /// <value>The <see cref="Type"/> of the model.</value>
        [Browsable(false)]
        public Type Type {
            get {
                switch (ModelType) {
                    case Models.Chunker:
                        return typeof (ChunkerModel);
                    case Models.Tokenizer:
                        return typeof (TokenizerModel);
                    case Models.NameFind:
                        return typeof (TokenNameFinderModel);
                    case Models.Parser:
                        return typeof (ParserModel);
                    case Models.POSTag:
                        return typeof (POSModel);
                    case Models.SentenceDetector:
                        return typeof (SentenceModel);
                }
                return null;
            }
        }

        #endregion

        #endregion

        #region . OpenModel .
        /// <summary>
        /// Opens the model file and returns the respective model object.
        /// </summary>
        /// <returns>A respective model object.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The model file does not exist.</exception>
        /// <exception cref="System.InvalidOperationException">Unable to detect the model type.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public BaseModel OpenModel() {
            if (!File.Exists)
                throw new FileNotFoundException("The model file does not exist.", File.FullName);

            using (var file = File.OpenRead()) {
                switch (ModelType) {
                    case Models.Chunker:
                        return new ChunkerModel(file);
                    case Models.Tokenizer:
                        return new TokenizerModel(file);
                    case Models.NameFind:
                        return new TokenNameFinderModel(file);
                    case Models.Parser:
                        return new ParserModel(file);
                    case Models.POSTag:
                        return new POSModel(file);
                    case Models.SentenceDetector:
                        return new SentenceModel(file);
                    case Models.Unknown:
                        throw new InvalidOperationException("Unable to detect the model type.");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }          
        }

        #endregion


    }
}