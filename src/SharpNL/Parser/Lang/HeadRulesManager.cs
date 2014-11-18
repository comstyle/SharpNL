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
using System.IO;
using System.Text;
using SharpNL.Parser.Lang.en;

namespace SharpNL.Parser.Lang {
    internal static class HeadRulesManager {
        
        public static void Serialize(AbstractHeadRules headRules, Stream outputStream) {
            if (headRules == null)
                throw new ArgumentNullException("headRules");

            if (outputStream == null)
                throw new ArgumentNullException("outputStream");

            if (!outputStream.CanWrite)
                throw new ArgumentException(@"Stream was not writable.", "outputStream");


            headRules.Serialize(new StreamWriter(outputStream, Encoding.UTF8));
        }

        public static AbstractHeadRules Deserialize(string languageCode, Stream inputStream) {
            return new en.HeadRules(new StreamReader(inputStream, Encoding.UTF8));
            /*
            switch (languageCode) {
                case "pt":
                    ...
                    break;
                case "en":
                default:
                    return new HeadRules(new StreamReader(inputStream, Encoding.UTF8));
            }
            */
        }
    }
}