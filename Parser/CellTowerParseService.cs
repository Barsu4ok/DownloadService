using DownloadService.Interfaces;
using DownloadService.Models;
using DownloadService.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadService.Parser
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
                try
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
                                            info.Radio = extracted.ToString();
                                            break;
                                        case 2:
                                            if (int.TryParse(extracted, out int mcc))
                                                info.MCC = mcc;
                                            else throw new FormatException();
                                            break;
                                        case 3:
                                            if (int.TryParse(extracted, out int mnc))
                                                info.MNC = mnc;
                                            else throw new FormatException();
                                            break;
                                        case 4:
                                            if (int.TryParse(extracted, out int lac))
                                                info.LAC = lac;
                                            else throw new FormatException();
                                            break;
                                        case 5:
                                            if (int.TryParse(extracted, out int cid))
                                                info.CID = cid;
                                            else throw new FormatException();
                                            break;
                                        case 7:
                                            if (double.TryParse(extracted, NumberStyles.Any, CultureInfo.InvariantCulture, out double lon))
                                                info.LON = lon;
                                            else throw new FormatException();
                                            break;
                                        case 8:
                                            if (double.TryParse(extracted, NumberStyles.Any, CultureInfo.InvariantCulture, out double lan))
                                                info.LAN = lan;
                                            else throw new FormatException();
                                            break;
                                    }
                                }
                                startIndex = i + 1;
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    continue;
                }
                yield return info;
            }
        }
    }
}
