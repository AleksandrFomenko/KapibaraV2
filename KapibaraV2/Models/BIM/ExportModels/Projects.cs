using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.Models.BIM.ExportModels
{
    public class Project
    {
        public string Name { get; set; }
        public string SavePath { get; set; }
        public List<string> Paths { get; set; }
        public string badNameWorkset { get; set; }
    }

}
