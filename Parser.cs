using DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coravel;
using Coravel.Invocable;
using DownloadService.Models;

namespace DownloadService
{
    class Parser
    {
        private IParseService _cellTowerParseService;
        public Parser(IParseService parseService)
        {
            _cellTowerParseService = parseService;
        }

        public IEnumerable<CellInfo> parseFile(Stream dataStream)
        {
            return _cellTowerParseService.parse(dataStream);
        }
    }
}
