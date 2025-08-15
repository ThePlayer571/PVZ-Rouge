using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.DataClasses.Item.PlantBook;

public class CheckJsonConfigEditor : EditorWindow
{
    private string plantValueListPath = "Assets/Resource/Data/ConfigDefintion/JsonConfigs/PlantValueList.json";
    private string plantBookValueListPath = "Assets/Resource/Data/ConfigDefintion/JsonConfigs/PlantBookValueList.json";
    private string plantValueListFullPath => Path.Combine(Application.dataPath, plantValueListPath.Substring(7));
    private string plantBookValueListFullPath => Path.Combine(Application.dataPath, plantBookValueListPath.Substring(7));

    private List<string> missingPlantIds = new List<string>();
    private List<string> missingBookIds = new List<string>();
    private Vector2 scrollPos;

    [MenuItem("PVZRouge/Check Json Config Editor")]
    public static void ShowWindow()
    {
        GetWindow<CheckJsonConfigEditor>("CheckJsonConfig");
    }

    private void OnGUI()
    {
        GUILayout.Label("Json配置完备性检查", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("PlantValueList.json 路径:");
        GUILayout.BeginHorizontal();
        plantValueListPath = GUILayout.TextField(plantValueListPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string path = EditorUtility.OpenFilePanel("选择 PlantValueList.json", Application.dataPath + "/Resource/Data/ConfigDefintion/JsonConfigs", "json");
            if (!string.IsNullOrEmpty(path))
            {
                // 转为相对路径
                if (path.StartsWith(Application.dataPath))
                    plantValueListPath = "Assets" + path.Substring(Application.dataPath.Length);
                else
                    plantValueListPath = path;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("PlantBookValueList.json 路径:");
        GUILayout.BeginHorizontal();
        plantBookValueListPath = GUILayout.TextField(plantBookValueListPath);
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string path = EditorUtility.OpenFilePanel("选择 PlantBookValueList.json", Application.dataPath + "/Resource/Data/ConfigDefintion/JsonConfigs", "json");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                    plantBookValueListPath = "Assets" + path.Substring(Application.dataPath.Length);
                else
                    plantBookValueListPath = path;
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (GUILayout.Button("检查"))
        {
            CheckConfig();
        }
        if (GUILayout.Button("补全缺失字段（值设为-1）"))
        {
            CompleteMissingFields();
            CheckConfig();
        }
        if (GUILayout.Button("字段排序（按首字母）"))
        {
            SortJsonFields();
            CheckConfig();
        }

        GUILayout.Space(10);
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        if (missingPlantIds.Count == 0 && missingBookIds.Count == 0)
        {
            GUILayout.Label("配置完备！", EditorStyles.helpBox);
        }
        else
        {
            if (missingPlantIds.Count > 0)
            {
                GUILayout.Label("PlantValueList.json 缺失字段:", EditorStyles.boldLabel);
                foreach (var id in missingPlantIds)
                {
                    GUILayout.Label("- " + id, EditorStyles.wordWrappedLabel);
                }
            }
            if (missingBookIds.Count > 0)
            {
                GUILayout.Label("PlantBookValueList.json 缺失字段:", EditorStyles.boldLabel);
                foreach (var id in missingBookIds)
                {
                    GUILayout.Label("- " + id, EditorStyles.wordWrappedLabel);
                }
            }
        }
        GUILayout.EndScrollView();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开 PlantValueList.json"))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(plantValueListFullPath, 1);
        }
        if (GUILayout.Button("打开 PlantBookValueList.json"))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(plantBookValueListFullPath, 1);
        }
        GUILayout.EndHorizontal();
    }

    private void CheckConfig()
    {
        missingPlantIds.Clear();
        missingBookIds.Clear();
        var plantJson = ReadJsonKeys(plantValueListFullPath);
        var bookJson = ReadJsonKeys(plantBookValueListFullPath);

        foreach (PlantId id in Enum.GetValues(typeof(PlantId)))
        {
            if (id == PlantId.NotSet) continue;
            if (!plantJson.Contains(id.ToString()))
                missingPlantIds.Add(id.ToString());
        }
        foreach (PlantBookId id in Enum.GetValues(typeof(PlantBookId)))
        {
            if (id == PlantBookId.NotSet) continue;
            if (!bookJson.Contains(id.ToString()))
                missingBookIds.Add(id.ToString());
        }
    }

    private void CompleteMissingFields()
    {
        // 补全 PlantValueList.json
        var plantJson = ReadJsonDict(plantValueListFullPath);
        foreach (PlantId id in Enum.GetValues(typeof(PlantId)))
        {
            if (id == PlantId.NotSet) continue;
            string key = id.ToString();
            if (!plantJson.ContainsKey(key))
                plantJson[key] = -1;
        }
        WriteJsonDict(plantValueListFullPath, plantJson);

        // 补全 PlantBookValueList.json
        var bookJson = ReadJsonDict(plantBookValueListFullPath);
        foreach (PlantBookId id in Enum.GetValues(typeof(PlantBookId)))
        {
            if (id == PlantBookId.NotSet) continue;
            string key = id.ToString();
            if (!bookJson.ContainsKey(key))
                bookJson[key] = -1;
        }
        WriteJsonDict(plantBookValueListFullPath, bookJson);
        AssetDatabase.Refresh();
    }

    private void SortJsonFields()
    {
        // PlantValueList.json
        var plantDict = ReadJsonDict(plantValueListFullPath);
        var sortedPlant = plantDict.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        WriteJsonDict(plantValueListFullPath, sortedPlant);

        // PlantBookValueList.json
        var bookDict = ReadJsonDict(plantBookValueListFullPath);
        var sortedBook = bookDict.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        WriteJsonDict(plantBookValueListFullPath, sortedBook);
        AssetDatabase.Refresh();
    }

    private HashSet<string> ReadJsonKeys(string path)
    {
        var keys = new HashSet<string>();
        if (!File.Exists(path)) return keys;
        var json = File.ReadAllText(path);
        int start = json.IndexOf('{');
        int end = json.LastIndexOf('}');
        if (start < 0 || end < 0) return keys;
        var body = json.Substring(start + 1, end - start - 1);
        var lines = body.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var kv = line.Trim().Split(':');
            if (kv.Length >= 2)
            {
                var key = kv[0].Trim().Trim('"');
                if (!string.IsNullOrEmpty(key))
                    keys.Add(key);
            }
        }
        return keys;
    }

    private Dictionary<string, int> ReadJsonDict(string path)
    {
        var dict = new Dictionary<string, int>();
        if (!File.Exists(path)) return dict;
        var json = File.ReadAllText(path);
        int start = json.IndexOf('{');
        int end = json.LastIndexOf('}');
        if (start < 0 || end < 0) return dict;
        var body = json.Substring(start + 1, end - start - 1);
        var lines = body.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var kv = line.Trim().Split(':');
            if (kv.Length >= 2)
            {
                var key = kv[0].Trim().Trim('"');
                var valueStr = kv[1].Trim();
                if (int.TryParse(valueStr, out int value))
                    dict[key] = value;
            }
        }
        return dict;
    }

    private void WriteJsonDict(string path, Dictionary<string, int> dict)
    {
        using (var sw = new StreamWriter(path, false))
        {
            sw.WriteLine("{");
            int i = 0;
            foreach (var kv in dict)
            {
                string line = $"  \"{kv.Key}\": {kv.Value}";
                if (i < dict.Count - 1) line += ",";
                sw.WriteLine(line);
                i++;
            }
            sw.WriteLine("}");
        }
    }
}
