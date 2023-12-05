using DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;

namespace DownloadService
{
    class Parser
    {
        private IParseService cellTowerParseService;
        public Parser(IParseService parseService)
        {
            this.cellTowerParseService = parseService;
        }

        public void parseFile(string uri, string outputPath)
        {
            cellTowerParseService.parse(uri, outputPath);
        }
    }
}
