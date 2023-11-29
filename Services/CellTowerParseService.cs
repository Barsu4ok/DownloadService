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
        public void parse(HttpResponseMessage response, string outputPath)
        {
            if (response.IsSuccessStatusCode)
            {
                using Stream dataStream = response.Content.ReadAsStream();
                using StreamReader reader = new StreamReader(dataStream);
                using StreamWriter writer = new StreamWriter(outputPath, false);
                ReadOnlySpan<char> str;
                ReadOnlySpan<char> extracted;
                int count = 0;
                int startIndex = 0;
                /*
                 * The source line in the file is represented in the following format:
                 * "GSM,257,2,84,55722,0,29.478378,54.703674,1000,2,1,1459770590,1459770590,0"
                 * ',' is used as a delimiter for the source string
                 * The values at the 1,5,7,8 positions are extracted from the source string:
                 * first - Type of communication,
                 * fifth - Cell tower ID,
                 * seventh - longitude,
                 * eighth - latitude
                 * Output string format:
                 * "GSM,55722,29.478378,54.703674,"
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
                                if (count == 1 || count == 5 || count == 7 || count == 8)
                                {
                                    extracted = str.Slice(startIndex, i - startIndex);
                                    writer.Write(extracted);
                                    writer.Write(',');
                                }
                                startIndex = i + 1;
                            }
                        }
                        writer.WriteLine("");
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to execute the request. Error code: " + response.StatusCode);
            }
        }
    }
}
