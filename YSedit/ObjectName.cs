using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YSedit
{
    static class ObjectName
    {
        class FileErrorException : ApplicationException
        {
            public FileErrorException() : base() { }
        }

        static List<uint> kinds = new List<uint>();
        static public List<uint> getKinds { get { return kinds; } }
        static List<string> enNames = new List<string>(), jaNames = new List<string>();
        static public List<string> getEnNames { get { return enNames; } }

        static Dictionary<uint, string>
            enMap = new Dictionary<uint, string>(), jaMap = new Dictionary<uint, string>();
        static public Dictionary<uint, string> getEnMap { get { return enMap; } }

        static public void init()
        {
            
            using (StreamReader s = new StreamReader("ObjectNames.txt"))
            {
                while (!s.EndOfStream)
                {
                    string line = s.ReadLine();
                    if (line.Length == 0) continue;
                    string[] ts = line.Split(new char[]{'\t'}, StringSplitOptions.None);
                    if (ts.Length != 3) throw new FileErrorException();
                    var rs = ts[0].Split(new string[] { ".." }, StringSplitOptions.None);
                    uint lower, upper;
                    bool range = false;
                    if (rs.Length == 2)
                    {
                        lower = rs[0].parseHex(0xffff);
                        upper = rs[1].parseHex(0xffff);
                        range = true;
                    }
                    else
                        lower = upper = ts[0].parseHex(0xffff);
                    if (lower > upper) throw new FileErrorException();

                    if (ts[1].Length == 0)
                        ts[1] = ts[2];
                    if (ts[2].Length == 0)
                        ts[2] = ts[1];

                    if (ts[1].Length != 0)
                    {
                        for (var kind = lower; kind <= upper; kind++)
                        {
                            string t1 = ts[1], t2 = ts[2];
                            if (range)
                            {
                                t1 = t1 + " " + kind.ToString("x4");
                                t2 = t2 + " " + kind.ToString("x4");
                            }
                            kinds.Add(kind);
                            enNames.Add(t1);
                            jaNames.Add(t2);
                            enMap[kind] = t1;
                            jaMap[kind] = t2;
                        }
                    }
                }
            }
        }

        public enum Language 
        {
            English, Japanese
        }

        /// <summary>
        /// kindで名前を引く。langでの名前を優先するが、もう片方にだけある場合はそれを返す
        /// </summary>
        /// <param name="kind">オブジェクトの種類</param>
        /// <param name="lang">言語</param>
        /// <returns>kindのオブジェクトの名前。見つからなかった場合はnull</returns>
        static public string getObjectName(uint kind, Language lang)
        {
            var map = lang == Language.Japanese ? jaMap : enMap;
            if (map.ContainsKey(kind))
                return map[kind];
            else
                return null;
        }
    }
}
