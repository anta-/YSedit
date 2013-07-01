using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace YSedit
{
    static class ObjectInfo
    {
        public enum Language {
            Japanese,
            English,
            Count
        }

        class Dependent
        {
            public enum Case
            {
                ObjCfg,
                ObjFunc
            }

            public Case c;
            public uint a;
        }

        class Info
        {
            public bool canPlace;
            public string[] names, descriptions;
            public List<Dependent> dependents;

            public Info()
            {
                canPlace = false;
                names = Enumerable.Repeat("", (int)Language.Count).ToArray();
                descriptions = Enumerable.Repeat("", (int)Language.Count).ToArray();
                dependents = new List<Dependent>();
            }
        }

        static Info[] infos;

        const string directory = "ObjectInfo/";
        static public void init()
        {
            infos = new Info[0x10000];
            loadAllObjects();
            loadCanPlace();
            loadObjectName(Language.Japanese, "ja");
            loadObjectName(Language.English, "en");
        }

        static string[] getLines(string s)
        {
            return s.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        static void loadAllObjects()
        {
            using (var f = new StreamReader(directory + "AllObjects.txt"))
            {
                var a = getLines(f.ReadToEnd());
                foreach (var x in a)
                    infos[x.parseHex(0xffff)] = new Info();
            }
        }

        static void loadCanPlace()
        {
            using (var f = new StreamReader(directory + "CanPlaceObjects.txt"))
            {
                var a = getLines(f.ReadToEnd());
                foreach (var x in a)
                    infos[x.parseHex(0xffff)].canPlace = true;
            }
        }

        static void loadObjectName(Language lang, string suf)
        {
            using (var f = new StreamReader(directory + "ObjectName_" + suf + ".txt"))
            {
                var a = getLines(f.ReadToEnd());
                foreach (var line in a)
                {
                    var t = line.Split('\t');
                    var u = t[0].Split(new string[] { ".." }, StringSplitOptions.None);
                    uint lower, upper;
                    if (u.Length == 1)
                        lower = upper = t[0].parseHex(0xffff);
                    else if (u.Length == 2)
                    {
                        lower = u[0].parseHex(0xffff);
                        upper = u[1].parseHex(0xffff);
                    }
                    else throw new Exception();
                    if (lower > upper) throw new Exception();

                    for (var kind = lower; kind <= upper; kind++)
                        if (infos[kind] != null)
                        {
                            infos[kind].names[(int)lang] =
                                lower == upper ? t[1] : t[1] + " " + kind.ToString("x4");
                            infos[kind].descriptions[(int)lang] = t[2];
                        }
                }
            }
        }

        public class KindNameDesc
        {
            public KindNameDesc(uint k, string n, string d)
            {
                kind = k;
                name = n;
                desc = d;
            }
            public uint kind;
            public string name;
            public string desc;

            public uint Kind { get { return kind; } }
            public string Name { get { return name; } }
        }

        public static string getObjectName(uint kind, Language lang)
        {
            if (kind >= 0x10000 || infos[kind] == null)
                return "";
            return infos[kind].names[(int)lang];
        }

        public static string getObjectDescription(uint kind, Language lang)
        {
            if (kind >= 0x10000 || infos[kind] == null)
                return "";
            return infos[kind].descriptions[(int)lang];
        }

        public static List<KindNameDesc> getCanPlaceObjectNames(Language lang)
        {
            var r = new List<KindNameDesc>();

            for (var i = 0; i < 0x10000; i++)
            {
                if (infos[i] == null)
                    continue;
                if (!infos[i].canPlace)
                    continue;
                var name = infos[i].names[(int)lang];
                if (name == "")
                    name = infos[i].names[(int)Language.Japanese];
                if (name == "")
                    continue;
                var desc = infos[i].descriptions[(int)lang];
                if (desc == "")
                    desc = infos[i].descriptions[(int)Language.Japanese];
                r.Add(new KindNameDesc((uint)i, name, desc));
            }

            return r;
        }
    }
}
