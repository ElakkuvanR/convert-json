using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ConvertJson
{
    public partial class Form1 : Form
    {
        private readonly string X1 = ConfigurationManager.AppSettings["X1"];
        private readonly string X2 = ConfigurationManager.AppSettings["X2"];
        private readonly string WIDTH = ConfigurationManager.AppSettings["width"];
        private readonly string HEIGHT = ConfigurationManager.AppSettings["height"];
        private readonly string OPERATOR = ConfigurationManager.AppSettings["operator"];
        private string FilePath = string.Empty;
        private string FileName = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            var dialogresult = openFileDialog1.ShowDialog();
            if (dialogresult == DialogResult.OK)
            {
                var file = openFileDialog1.FileName;
                textBox1.Text = file;
                FilePath = file;
                FileName = openFileDialog1.SafeFileName;
            }
            this.btnConvert.Enabled = true;
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            string jsonValue = string.Empty;
            using (var sreamReader = new StreamReader(FilePath))
            {
                jsonValue = sreamReader.ReadToEnd();
            }
            var initialValue = JsonConvert.DeserializeObject<dynamic>(jsonValue);
            var firstLevel = JsonConvert.DeserializeObject<dynamic>(initialValue.frames.ToString());
            List<KeyValuePair<string, List<Coordinates>>> holdingObj = new List<KeyValuePair<string, List<Coordinates>>>();
            foreach (var d in firstLevel)
            {
                string key = d.Path;
                List<Coordinates> child = JsonConvert.DeserializeObject<List<Coordinates>>(d.Value.ToString());
                if (child.Count <= 0)
                {
                    holdingObj.Add(new KeyValuePair<string, List<Coordinates>>(key, child));
                    continue;
                }
                foreach (Coordinates cor in child)
                {
                    if (!string.IsNullOrEmpty(WIDTH))
                        cor.width = Convert.ToInt32(WIDTH);
                    if (!string.IsNullOrEmpty(HEIGHT))
                        cor.height = Convert.ToInt32(HEIGHT);
                    switch (OPERATOR)
                    {
                        case "+":
                            cor.x1 += Convert.ToInt32(X1);
                            cor.x2 += Convert.ToInt32(X2);
                            break;
                        case "-":
                            cor.x1 -= Convert.ToInt32(X1);
                            cor.x2 -= Convert.ToInt32(X2);
                            break;
                        default:
                            break;
                    }
                }
                holdingObj.Add(new KeyValuePair<string, List<Coordinates>>(key, child));
            }

            //Build the Result JSON Format 
            string resultFormat = "\"{0}\":{1}";
            StringBuilder builder = new StringBuilder();
            foreach (var item in holdingObj)
            {
                builder.AppendFormat(resultFormat
                                    , item.Key,
                                    item.Value.Count <= 0 ? "[]" : JsonConvert.SerializeObject(item.Value));
                builder.Append(",");
            }
            string finalResult = Convert.ToString(builder);
            WriteToFileSystem(finalResult, initialValue);

        }
        /// <summary>
        /// Write the processed json value to the file system
        /// </summary>
        /// <param name="finalResult">string value</param>
        /// <param name="initialValue">initial json value before processing</param>
        private void WriteToFileSystem(string finalResult, dynamic initialValue)
        {
            int[] visitedFrames = initialValue.visitedFrames?.Parent.Value.ToObject<int[]>();
            string[] tag_colors = initialValue.tag_colors?.Parent.Value.ToObject<string[]>();

            Root rootValue = new Root()
            {
                frames = "{" + finalResult + "}",
                framerate = initialValue.framerate.Value,
                inputTags = initialValue.inputTags.Value,
                suggestiontype = initialValue.suggestiontype.Value,
                scd = initialValue.scd.Value,
                visitedFrames = visitedFrames,
                tag_colors = tag_colors
            };


            string serializedObjToFile = JsonConvert.SerializeObject(rootValue);

            //Replace the unwanted endings due to manual processing 
            serializedObjToFile = serializedObjToFile.Replace(@"\", "");
            serializedObjToFile = serializedObjToFile.Replace("\"{", "{");
            serializedObjToFile = serializedObjToFile.Replace("}\"", "}");
            serializedObjToFile = serializedObjToFile.Replace(",}", "}");

            // Write to File System //bin

            File.WriteAllText(@".\" + FileName + "-result.json", serializedObjToFile);
        }
    }
}

