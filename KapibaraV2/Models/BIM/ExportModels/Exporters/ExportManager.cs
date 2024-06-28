using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapibaraV2.Models.BIM.ExportModels.Exporters
{
    internal class ExportManager
    {
        private readonly IExporter _exporter;

        public ExportManager(IExporter exporter)
        {
            _exporter = exporter;
        }

        public void ExecuteExport()
        {
            _exporter.Export();
        }
    }
}
