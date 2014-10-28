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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;

namespace SharpNL.Tests.Project {
    [TestFixture]
    internal class ProjectTest {


        [Test]
        public void TestSomething() {
 
            /*
            Type type = null;

            type = System.Type.GetTypeFromProgID("htmlfile");
            dynamic doc = Activator.CreateInstance(type);

            
            doc.open();
            doc.write("<html><body><p>teste</p></body></html>");
            doc.close();

            






            Marshal.ReleaseComObject(doc);
            

            Assert.NotNull(type);
             *  */

        }

        /*
        [Test]
        public void TestSerialization() {

            var project = new SharpNL.Project {Name = "bla"};

            var file = new TextFileNode(@"F:\Projetos\SharpNL\resources\opennlp\tools\sentdetect\Sentences.txt") {
                Encoding = Encoding.GetEncoding("ISO-8859-1")
            };
            var input = project.Add(file);

            var doc = input.Add(new DocumentNode("en"));

            doc.Run();

            string xml;
            using (var data = new MemoryStream()) {

                project.Serialize(data, true);

                data.Seek(0, SeekOrigin.Begin);


                using (var reader = new StreamReader(data)) {

                    xml = reader.ReadToEnd();
                }
            }

            Assert.IsNotNullOrEmpty(xml);
        }
         * */

    }
}