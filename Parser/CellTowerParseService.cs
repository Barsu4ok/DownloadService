using DownloadService.Interfaces;
using DownloadService.Models;
using System.Globalization;
using System.Text;

namespace DownloadService.Parser
{
    public class CellTowerParseService : IParseService
    {
        public IEnumerable<CellInfo> Parse(Stream data)
        {
            var info = new CellInfo();
            using var reader = new StreamReader(data);
            var sb = new StringBuilder();
            while (!reader.EndOfStream)
            {
                sb.Clear();
                var counterLbsParameters = 0;
                var counterLatAndLon = 0;
                if(
                TryReadType() &&
                TryReadInt32() &&
                TryReadInt32() &&
                TryReadInt32() &&
                TryReadInt32() &&
                TrySkip() &&
                TryReadDecimal() &&
                TryReadDecimal())
                {
                    yield return info;
                }
                SkipLine();

                bool TryReadType()
                {
                    sb.Clear();
                    while (true)
                    {
                        sb.Append((char)reader.Read());
                        if ((sb.Length == 4 && sb.ToString() == "GSM,") || (sb.Length == 5 && sb.ToString() == "UMTS,"))
                        {
                            sb.Remove(sb.Length - 1, 1);
                            info.Act = sb.ToString();
                            return true;
                        }
                        if (sb.Length == 4 && sb.ToString() == "LTE,")
                        {
                            return false;
                        }
                    }
                }

                bool TryReadInt32()
                {
                    sb.Clear();
                    while (true)
                    {
                        try
                        {
                            var c = (char)reader.Read();
                            if (c == ',')
                            {
                                ++counterLbsParameters;
                                switch (counterLbsParameters)
                                {
                                    case 1:
                                        if (int.TryParse(sb.ToString(), out var mcc))
                                        {
                                            info.Mcc = mcc;
                                        }
                                        else throw new FormatException();

                                        break;
                                    case 2:
                                        if (int.TryParse(sb.ToString(), out var mnc))
                                        {
                                            info.Mnc = mnc;
                                        }
                                        else throw new FormatException();

                                        break;
                                    case 3:
                                        if (int.TryParse(sb.ToString(), out var lac))
                                        {
                                            info.Lac = lac;
                                        }
                                        else throw new FormatException();

                                        break;
                                    case 4:
                                        if (int.TryParse(sb.ToString(), out var cid))
                                        {
                                            info.Cid = cid;
                                        }
                                        else throw new FormatException();

                                        break;
                                }

                                return true;
                            }


                            if (!char.IsDigit(c))
                            {
                                return false;
                            }

                            sb.Append(c);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e);
                            return false;
                        }
                    }
                }

                bool TrySkip()
                {
                    sb.Clear();
                    while (true)
                    {
                        if (reader.Read() == ',')
                        {
                            return true;
                        }
                    }
                }

                bool TryReadDecimal()
                {
                    sb.Clear();
                    while (true)
                    {
                        try
                        {
                            var c = (char)reader.Read();
                            if (c == ',')
                            {
                                ++counterLatAndLon;
                                switch (counterLatAndLon)
                                {
                                    case 1:
                                        if (double.TryParse(sb.ToString(), NumberStyles.Any,
                                                CultureInfo.InvariantCulture, out var lon))
                                        {
                                            info.Lon = lon;
                                        }
                                        else throw new FormatException();

                                        break;
                                    case 2:
                                        if (double.TryParse(sb.ToString(), NumberStyles.Any,
                                                CultureInfo.InvariantCulture, out var lat))
                                        {
                                            info.Lat = lat;
                                        }
                                        else throw new FormatException();

                                        break;
                                }

                                return true;
                            }

                            if (!char.IsDigit(c) && c != '.')
                            {
                                return false;
                            }

                            sb.Append(c);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine(e);
                            return false;
                        }
                    }
                }

                void SkipLine()
                {
                    sb.Clear();
                    while (!reader.EndOfStream)
                    {
                        if (reader.Read() == '\n')
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}

