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

        public class Dependent
        {
            public enum Case
            {
                ObjCfg,
                ObjFunc
            }

            public Case c;
            public uint a;

            public static Dependent ObjCfg(uint kind)
            {
                Dependent d = new Dependent();
                d.c = Case.ObjCfg;
                d.a = kind;
                return d;
            }

            public static Dependent ObjFunc(uint addr)
            {
                Dependent d = new Dependent();
                d.c = Case.ObjFunc;
                d.a = addr;
                return d;
            }
        }

        class Info
        {
            public bool canPlace;
            public string[] names, descriptions;
            public List<Dependent> dependents;
            public List<uint> funcs;

            public Info()
            {
                canPlace = false;
                names = Enumerable.Repeat("", (int)Language.Count).ToArray();
                descriptions = Enumerable.Repeat("", (int)Language.Count).ToArray();
                dependents = new List<Dependent>();
                funcs = new List<uint>();
            }
        }

        static Info[] infos;
        static Dictionary<uint, List<ushort>> funcMap = new Dictionary<uint,List<ushort>>();

        const string directory = "ObjectInfo/";
        static public void init()
        {
            infos = new Info[0x10000];
            loadAllObjects();
            loadCanPlace();
            loadObjectName(Language.Japanese, "ja");
            loadObjectName(Language.English, "en");
            loadObjectDependents();
            loadObjectFuncs();
        }

        static string[] getLines(string path)
        {
            using (var f = new StreamReader(path)) {
                return f.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        static void loadAllObjects()
        {
            var a = getLines(directory + "AllObjects.txt");
            foreach (var x in a)
                infos[x.parseHex(0xffff)] = new Info();
        }

        static void loadCanPlace()
        {
            var a = getLines(directory + "CanPlaceObjects.txt");
            foreach (var x in a)
                infos[x.parseHex(0xffff)].canPlace = true;
        }

        static void loadObjectName(Language lang, string suf)
        {
            var a = getLines(directory + "ObjectName_" + suf + ".txt");
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

        static void loadObjectDependents()
        {
            var a = getLines(directory + "ObjectDependents.txt");
            foreach (var line in a)
            {
                var t = line.Split('\t');
                var kind = t[0].parseHex(0xffff);
                var deps = t[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                foreach (var dep in deps)
                {
                    if (dep[0] == '!')
                        infos[kind].dependents.Add(
                            Dependent.ObjFunc(dep.Substring(1).parseHex()));
                    else
                        infos[kind].dependents.Add(
                            Dependent.ObjCfg(dep.parseHex(0xffff)));
                }
            }
        }

        static void loadObjectFuncs()
        {
            var a = getLines(directory + "ObjectFuncs.txt");
            foreach (var line in a)
            {
                var t = line.Split('\t');
                var kind = t[0].parseHex(0xffff);
                var funcs = t[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
                foreach (var func in funcs)
                {
                    var f = func.parseHex();
                    infos[kind].funcs.Add(f);
                    if (!funcMap.ContainsKey(f))
                        funcMap[f] = new List<ushort>();
                    funcMap[f].Add((ushort)kind);
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

        public static List<Dependent> getObjectDependents(ushort kind)
        {
            if (infos[kind] == null)
                return new List<Dependent>();
            else
                return infos[kind].dependents;
        }

        public static List<uint> getObjectFuncs(ushort kind)
        {
            if (infos[kind] == null)
                return new List<uint>();
            else
                return infos[kind].funcs;
        }

        public static List<ushort> getFuncObjects(uint f)
        {
            if (!funcMap.ContainsKey(f))
                return new List<ushort>();
            else
                return funcMap[f];
        }
    }
}
