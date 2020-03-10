using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVTool
{
    class Config
    {
        public string EsClientAddress { get; set; }
        public string TenantID { get; set; }
        public string[] ExcludeFields { get; set; }
        public string[] IncludeFields { get; set; }
        public string OutputFilepath { get; set; }
        public int BatchSize { get; set; }
        public string Query { get; set; }
        public string IndexType { get; set; }

    }
}
