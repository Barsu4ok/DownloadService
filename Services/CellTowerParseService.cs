using DownloadService.Models;
using DownloadService.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Services
{
    public class CellTowerParseService : IParseService
    {
        public IEnumerable<CellInfo> parse(Stream data)
        {
            using StreamReader reader = new StreamReader(data);
            ReadOnlySpan<char> str;
            ReadOnlySpan<char> extracted;
            CellInfo info = new CellInfo();
            int count = 0;
            int startIndex = 0;
            /*
                * The source line in the file is represented in the following format:
                * "GSM,257,2,84,55722,0,29.478378,54.703674,1000,2,1,1459770590,1459770590,0"
                * ',' is used as a delimiter for the source string
                * The values at the 1,5,7,8 positions are extracted from the source string:
                * first - Type of communication,
                * second - Mobile country code,
                * third - Mobile network code,
                * fourth - Location Area Code,
                * fifth - Cell tower ID,
                * seventh - longitude,
                * eighth - latitude
                * Output string format:
                * "GSM,257,2,84,55722,29.478378,54.703674,"
                * ',' is used as a delimiter
                */
            while ((str = reader.ReadLine()) != null)
            {
                if (str.StartsWith("GSM") || str.StartsWith("UMTS"))
                {
                    count = 0;
                    startIndex = 0;
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (str[i] == ',')
                        {
                            count++;
                            if (count == 1 || count == 2 || count == 3 || count == 4 || count == 5 || count == 7 || count == 8)
                            {
                                extracted = str.Slice(startIndex, i - startIndex);
                                switch (count)
                                {
                                    case 1:
                                        info.type = extracted.ToString();
                                        break;
                                    case 2:
                                        info.countryCode = extracted.ToString();
                                        break;
                                    case 3:
                                        info.networkCode = extracted.ToString();
                                        break;
                                    case 4:
                                        info.lac = extracted.ToString();
                                        break;
                                    case 5:
                                        info.cellId = extracted.ToString();
                                        break;
                                    case 7:
                                        info.lon = extracted.ToString();
                                        break;
                                    case 8:
                                        info.lan = extracted.ToString();
                                        break;
                                }    
                            }
                            startIndex = i + 1;
                        }
                    }
                }
                yield return info;
            }
        }
    }
}
