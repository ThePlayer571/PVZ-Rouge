using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.MazeMap;
using TPL.PVZR.Systems;

public class WhenLevelEndedInfoCsvExporter : EditorWindow
{
    private string folderPath = "";
    private string savePath = "";
    private LevelDef selectedLevelDef;
    private bool selectLevelDef = true;
    private bool selectMaxValue = true;
    private bool selectComments = true;
    private bool selectPass = true;
    private bool selectEarnedCoins = true;
    // TODO: 新增字段时，在此处添加toggle变量，并在OnGUI和导出逻辑中处理

    [MenuItem("PVZRouge/导出WhenLevelEndedInfo为CSV")]
    public static void ShowWindow()
    {
        GetWindow<WhenLevelEndedInfoCsvExporter>("WhenLevelEndedInfo导出工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("导出WhenLevelEndedInfo为CSV", EditorStyles.boldLabel);

        if (GUILayout.Button("选择数据文件夹"))
        {
            string path = EditorUtility.OpenFolderPanel("选择数据文件夹", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
                folderPath = path;
        }

        GUILayout.Label("当前文件夹: " + folderPath);

        // LevelDef选择（可自定义UI）
        GUILayout.Label("LevelDef选择（请填写Id, Difficulty, StageDifficulty）");
        selectedLevelDef.Id = (LevelId)EditorGUILayout.EnumPopup("Id", selectedLevelDef.Id);
        selectedLevelDef.Difficulty =
            (GameDifficulty)EditorGUILayout.EnumPopup("Difficulty", selectedLevelDef.Difficulty);
        selectedLevelDef.StageDifficulty =
            (StageDifficulty)EditorGUILayout.EnumPopup("StageDifficulty", selectedLevelDef.StageDifficulty);

        // 字段选择
        GUILayout.Label("选择导出字段:");
        selectLevelDef = EditorGUILayout.Toggle("LevelDef", selectLevelDef);
        selectMaxValue = EditorGUILayout.Toggle("maxValue", selectMaxValue);
        selectComments = EditorGUILayout.Toggle("comments", selectComments);
        selectPass = EditorGUILayout.Toggle("pass", selectPass);
        selectEarnedCoins = EditorGUILayout.Toggle("earnedCoins", selectEarnedCoins);
        // TODO: 新增字段时，在此处添加toggle即可

        if (GUILayout.Button("选择CSV保存位置"))
        {
            string path = EditorUtility.SaveFilePanel("保存CSV文件", Application.dataPath,
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv", "csv");
            if (!string.IsNullOrEmpty(path))
                savePath = path;
        }

        GUILayout.Label("保存路径: " + savePath);

        if (GUILayout.Button("导出CSV"))
        {
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayDialog("错误", "请先选择文件夹和保存路径", "确定");
                return;
            }

            ExportCsv();
        }
    }

    private void ExportCsv()
    {
        var files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        var rows = new List<string>();
        // 表头
        var header = new List<string> { "文件名" };
        if (selectLevelDef) header.Add("LevelDef");
        if (selectMaxValue) header.Add("maxValue");
        if (selectComments) header.Add("comments");
        if (selectPass) header.Add("pass");
        if (selectEarnedCoins) header.Add("earnedCoins");
        // TODO: 新增字段时，在此处添加表头
        rows.Add(string.Join(",", header));

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            WhenLevelEndedInfo info = null;
            try
            {
                info = JsonUtility.FromJson<WhenLevelEndedInfo>(json);
            }
            catch
            {
                Debug.LogWarning($"文件解析失败: {file}");
                continue;
            }

            if (!info.levelDef.Equals(selectedLevelDef)) continue;

            var row = new List<string> { Path.GetFileName(file) };
            if (selectLevelDef) row.Add(info.levelDef.ToString().Replace(",", ";"));
            if (selectMaxValue) row.Add(info.maxValue.ToString());
            if (selectComments) row.Add(EscapeCsv(info.comments));
            if (selectPass) row.Add(info.pass.ToString());
            if (selectEarnedCoins) row.Add(info.earnedCoins.ToString());
            // TODO: 新增字段时，在此处添加数据
            rows.Add(string.Join(",", row));
        }

        File.WriteAllLines(savePath, rows, Encoding.UTF8);
        EditorUtility.DisplayDialog("导出完成", $"已导出{rows.Count - 1}条数据到\n{savePath}", "确定");
    }

    // CSV转义
    private string EscapeCsv(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        if (input.Contains(",") || input.Contains("\"") || input.Contains("\n"))
            return $"\"{input.Replace("\"", "\"\"")}\"";
        return input;
    }
}

// 扩展说明：
// 1. 新增字段时，在顶部toggle变量、OnGUI字段选择、ExportCsv表头和数据收集处都需补充。
// 2. 枚举类型均以ToString()导出。
// 3. 支持递归扫描所有json文件。
// 4. 可根据需要扩展字段和筛选逻辑。