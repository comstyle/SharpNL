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

using System.ComponentModel;
using System.Text;

namespace ShapNL.Cmd.Sentence {
    [Help("Converts data files to native OpenNLP format.")]
    internal class SentenceDetectorConverter : Command {

        #region + Properties .

        [Argument(true, "charset", "charsetName"), Help("The encoding used to read and write the text.")]
        public Encoding Encoding { get; set; }

        [Argument(true, "data", "sampleData"), Help("The data file to be converted.")]
        public string Data { get; set; }

        [Argument(false, "includeTitles"), Help("Determines if the senteces are marked as headlines."), DefaultValue(false)]
        public bool IncludeTitles { get; set; }

        [Argument(true, "lang", "language"), Help("The language which is being processed.")]
        public string Language { get; set; }

        [Argument(true, "out", "output"), Help("The output file.")]
        public string Output { get; set; }

        #endregion

        /// <summary>
        /// Executes this instance.
        /// </summary>
        protected override void Execute() {
            
        }

    }
}