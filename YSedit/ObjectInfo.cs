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
        static Dictionary<uint, List<ushort>> funcMap;

        const string directory = "ObjectInfo/"; //@"C:\Documents and Settings\test\My Documents\Visual Studio 2010\Projects\YSedit\YSedit\ObjectInfo\";
        static public void init()
        {
            infos = new Info[0x10000];
            funcMap = new Dictionary<uint, List<ushort>>();

            loadAllObjects();
            loadCanPlace();
            loadObjectName(Language.Japanese, "ja");
            loadObjectName(Language.English, "en");
            loadObjectDependents();
            transitiveClosureObjectDependents();
            loadObjectFuncs();
        }

        static string[] getLines(string path)
        {
            using (var f = new StreamReader(path)) {
                return f
                    .ReadToEnd()
                    .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x[0] != '#').ToArray();
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
                if (line.Trim() == "") continue;
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

        /// <summary>
        /// あるオブジェクトに依存してるならそれが依存してるものに依存してるのは当たり前なので、推移閉包を取る
        /// </summary>
        //っと思ったけどそうでないもの
        //(リフトさんの種は？シャボン全般の中身に用いられるが、その場合リフトさんには依存しない)
        //が存在する…
        //とりあえずハードコーディング？
        static void transitiveClosureObjectDependents()
        {
            //Floyd-Warshall's のO(n^3)は掛かり過ぎるけど、
            //この場合連結成分数が多いので、
            //連結成分ごとに分解してからやればいいかな。
            //'強'連結成分分解するヒューステリクスが有名かつ実用的なようだ。

            Func<ushort,IEnumerable<ushort>> edges = (ushort i) =>
                infos[i].dependents.SelectMany((Dependent d) => {
                        switch(d.c) {
                            case Dependent.Case.ObjCfg:
                                return Enumerable.Repeat((ushort)d.a, 1);
                            case Dependent.Case.ObjFunc:
                                if (funcMap.ContainsKey(d.a) && funcMap[d.a].Count <= 1)
                                    return funcMap[d.a];
                                else
                                    return Enumerable.Empty<ushort>();
                            default:
                                throw new Exception();
                        }
                    }
                ).Where(j => infos[j] != null);
            List<ushort>[] invedges = new List<ushort>[0x10000].Populate();
            
            for (var i = 0; i < 0x10000; i++)
                if (infos[i] != null)
                    foreach (var j in edges((ushort)i))
                        invedges[j].Add((ushort)i);

            bool[] visited = new bool[0x10000];
            List<ushort> connected_component = new List<ushort>();
            Action<ushort> dfs = null;
            dfs = i =>
            {
                if (visited[i])
                    return;
                visited[i] = true;
                connected_component.Add(i);
                foreach (var j in edges(i))
                    dfs(j);
                foreach (var j in invedges[i])
                    dfs(j);
            };

            int[] index = new int[0x10000];
            for (int i = 0; i < 0x10000; i++)
                if (infos[i] != null && !visited[i])
                {
                    connected_component.Clear();
                    dfs((ushort)i);
                    int size = connected_component.Count;
                    bool[,]
                        orggraph = new bool[size, size],
                        graph = new bool[size, size];
                    for (var v = 0; v < size; v++)
                        index[connected_component[v]] = v;
                    foreach (var v in connected_component)
                        foreach (var u in edges(v))
                        {
                            int j = index[v], k = index[u];
                            orggraph[j, k] = graph[j, k] = true;
                        }
                    for (var k = 0; k < size; k++)
                        if (connected_component[k] != 0x41ce)   //リフトさんの種
                            for (var v = 0; v < size; v++)
                                for (var u = 0; u < size; u++)
                                    graph[v, u] |= graph[v, k] && graph[k, u];
                    for (var v = 0; v < size; v++)
                        for (var u = 0; u < size; u++)
                            if (graph[v, u] && !orggraph[v, u] && v != u)
                                infos[connected_component[v]].dependents.Add(
                                    Dependent.ObjCfg(connected_component[u]));
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
