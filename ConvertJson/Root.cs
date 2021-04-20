using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertJson
{
    public class Box
    {
        public double x1 { get; set; }
        public double y1 { get; set; }
        public double x2 { get; set; }
        public double y2 { get; set; }
    }

    public class Coordinates
    {
        public double x1 { get; set; }
        public double y1 { get; set; }
        public double x2 { get; set; }
        public double y2 { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Box box { get; set; }
        public string UID { get; set; }
        public int id { get; set; }
        public string type { get; set; }
        public string[] tags { get; set; }
        public int name { get; set; }
    }

    public class Frames
    {
        public List<Coordinates> coordinates { get; set; }
    }

    public class Root
    {
        public string frames { get; set; }
        public string framerate { get; set; }
        public string inputTags { get; set; }
        public string suggestiontype { get; set; }
        public bool scd { get; set; }
        public int[] visitedFrames { get; set; }
        public string[] tag_colors { get; set; }
    }
}
