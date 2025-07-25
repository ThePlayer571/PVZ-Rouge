using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using TPL.PVZR.Classes.DataClasses.Level;
using TPL.PVZR.Classes.ConfigLists;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Classes.MazeMap;

namespace TPL.PVZR.Editor
{
    public class LevelDefinitionAutoConfigEditor : EditorWindow
    {
        private string selectedFolderPath = "";
        private LevelDefinitionList selectedLevelDefinitionList;
        private List<LevelDefinition> foundLevelDefinitions = new List<LevelDefinition>();
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private bool showDefaultSettings = false;

        // 默认值设置
        private string defaultFolderPath = "Assets/Resources/Data/LevelDefinitions";
        private string defaultLevelDefinitionListPath = "Assets/Resources/Data/ConfigDefintion/LevelDefinitionList.asset";

        // EditorPrefs键名
        private const string PREFS_DEFAULT_FOLDER = "LevelDefEditor_DefaultFolder";
        private const string PREFS_DEFAULT_LIST = "LevelDefEditor_DefaultList";

        [MenuItem("PVZRouge/Level Definition Auto Config")]
        public static void ShowWindow()
        {
            GetWindow<LevelDefinitionAutoConfigEditor>("Level Definition Auto Config");
        }

        private void OnEnable()
        {
            // 加载保存的默认值
            defaultFolderPath = EditorPrefs.GetString(PREFS_DEFAULT_FOLDER, "Assets/Resources/Data/LevelDefinitions");
            defaultLevelDefinitionListPath = EditorPrefs.GetString(PREFS_DEFAULT_LIST, "Assets/Resources/Data/ConfigDefintion/LevelDefinitionList.asset");
            
            // 如果当前选择为空，则使用默认值
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                selectedFolderPath = defaultFolderPath;
            }
            
            if (selectedLevelDefinitionList == null && !string.IsNullOrEmpty(defaultLevelDefinitionListPath))
            {
                selectedLevelDefinitionList = AssetDatabase.LoadAssetAtPath<LevelDefinitionList>(defaultLevelDefinitionListPath);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Definition Auto Configuration Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 默认设置区域
            showDefaultSettings = EditorGUILayout.Foldout(showDefaultSettings, "Default Settings");
            if (showDefaultSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.LabelField("Default Folder Path:", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                defaultFolderPath = EditorGUILayout.TextField(defaultFolderPath);
                if (GUILayout.Button("Browse", GUILayout.Width(60)))
                {
                    string path = EditorUtility.OpenFolderPanel("Select Default Folder", "Assets", "");
                    if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                    {
                        defaultFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Default Level Definition List:", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                defaultLevelDefinitionListPath = EditorGUILayout.TextField(defaultLevelDefinitionListPath);
                if (GUILayout.Button("Browse", GUILayout.Width(60)))
                {
                    string path = EditorUtility.OpenFilePanel("Select Default Level Definition List", "Assets", "asset");
                    if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                    {
                        defaultLevelDefinitionListPath = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Save Defaults"))
                {
                    SaveDefaults();
                }
                if (GUILayout.Button("Load Defaults"))
                {
                    LoadDefaults();
                }
                if (GUILayout.Button("Apply Defaults"))
                {
                    ApplyDefaults();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            // 选择文件夹部分
            EditorGUILayout.LabelField("Step 1: Select Folder to Scan", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Selected Folder:", selectedFolderPath);
            if (GUILayout.Button("Browse Folder", GUILayout.Width(120)))
            {
                SelectFolder();
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(selectedFolderPath))
            {
                if (GUILayout.Button("Scan for Level Definitions"))
                {
                    ScanForLevelDefinitions();
                }
            }

            EditorGUILayout.Space();

            // 选择LevelDefinitionList文件部分
            EditorGUILayout.LabelField("Step 2: Select Level Definition List", EditorStyles.boldLabel);
            selectedLevelDefinitionList = (LevelDefinitionList)EditorGUILayout.ObjectField(
                "Level Definition List:", 
                selectedLevelDefinitionList, 
                typeof(LevelDefinitionList), 
                false
            );

            EditorGUILayout.Space();

            // 显示扫描结果
            if (foundLevelDefinitions.Count > 0)
            {
                EditorGUILayout.LabelField($"Found {foundLevelDefinitions.Count} Level Definitions:", EditorStyles.boldLabel);
                
                showPreview = EditorGUILayout.Foldout(showPreview, "Preview Found Definitions");
                if (showPreview)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                    foreach (var levelDef in foundLevelDefinitions)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(levelDef, typeof(LevelDefinition), false);
                        EditorGUILayout.LabelField($"Level: {levelDef.LevelDef}", GUILayout.Width(100));
                        EditorGUILayout.LabelField($"Size: {levelDef.MapSize}", GUILayout.Width(80));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.Space();

                // 配置按钮
                if (selectedLevelDefinitionList != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add All to List", GUILayout.Height(30)))
                    {
                        AddLevelDefinitionsToList(false);
                    }
                    if (GUILayout.Button("Replace List Content", GUILayout.Height(30)))
                    {
                        AddLevelDefinitionsToList(true);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("Add All: Adds found definitions to existing list (avoiding duplicates)\nReplace: Clears list and adds only found definitions", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select a Level Definition List to configure.", MessageType.Warning);
                }
            }
        }

        private void SelectFolder()
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder to Scan for Level Definitions", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对于Assets的路径
                if (path.StartsWith(Application.dataPath))
                {
                    selectedFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Please select a folder within the Assets directory.", "OK");
                }
            }
        }

        private void ScanForLevelDefinitions()
        {
            foundLevelDefinitions.Clear();

            if (string.IsNullOrEmpty(selectedFolderPath))
                return;

            // 获取所有LevelDefinition类型的GUID
            string[] guids = AssetDatabase.FindAssets("t:LevelDefinition", new[] { selectedFolderPath });
            
            // 用于检测重复LevelDef的字典
            Dictionary<LevelDef, List<string>> levelDefFileMap = new Dictionary<LevelDef, List<string>>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                LevelDefinition levelDef = AssetDatabase.LoadAssetAtPath<LevelDefinition>(assetPath);
                
                if (levelDef != null)
                {
                    foundLevelDefinitions.Add(levelDef);
                    
                    // 记录LevelDef对应的文件路径
                    if (!levelDefFileMap.ContainsKey(levelDef.LevelDef))
                    {
                        levelDefFileMap[levelDef.LevelDef] = new List<string>();
                    }
                    levelDefFileMap[levelDef.LevelDef].Add(assetPath);
                }
            }

            // 检查重复的LevelDef
            CheckForDuplicateLevelDefs(levelDefFileMap);

            // 按LevelDef排序 - 现在可以直接使用CompareTo方法
            foundLevelDefinitions.Sort((a, b) => a.LevelDef.CompareTo(b.LevelDef));

            Debug.Log($"Found {foundLevelDefinitions.Count} Level Definitions in {selectedFolderPath}");
        }

        private void CheckForDuplicateLevelDefs(Dictionary<LevelDef, List<string>> levelDefFileMap)
        {
            List<string> duplicateWarnings = new List<string>();
            List<string> invalidWarnings = new List<string>();

            foreach (var kvp in levelDefFileMap)
            {
                LevelDef levelDef = kvp.Key;
                List<string> filePaths = kvp.Value;

                // 检查LevelDef是否有无效值（任何字段为NotSet或默认值）
                bool isInvalid = levelDef.Id == LevelId.NotSet || 
                                levelDef.Difficulty == GameDifficulty.NotSet || 
                                levelDef.StageDifficulty == StageDifficulty.NotSet;

                if (isInvalid)
                {
                    foreach (string filePath in filePaths)
                    {
                        string invalidReason = "";
                        if (levelDef.Id == LevelId.NotSet)
                            invalidReason += "Id=NotSet ";
                        if (levelDef.Difficulty == GameDifficulty.NotSet)
                            invalidReason += "Difficulty=NotSet ";
                        if (levelDef.StageDifficulty == StageDifficulty.NotSet)
                            invalidReason += "StageDifficulty=NotSet ";

                        invalidWarnings.Add($"• {System.IO.Path.GetFileName(filePath)} - LevelDef has invalid values: {invalidReason.Trim()}");
                    }
                }
                // 检查是否有重复的LevelDef
                if (filePaths.Count > 1)
                {
                    string fileList = string.Join("\n  ", filePaths.ConvertAll(path => $"• {System.IO.Path.GetFileName(path)}"));
                    duplicateWarnings.Add($"LevelDef '{levelDef}' is used by multiple files:\n  {fileList}");
                }
            }

            // 显示警告对话框
            if (duplicateWarnings.Count > 0 || invalidWarnings.Count > 0)
            {
                string warningMessage = "";
                
                if (duplicateWarnings.Count > 0)
                {
                    warningMessage += "⚠️ DUPLICATE LEVEL DEFs DETECTED:\n\n";
                    warningMessage += string.Join("\n\n", duplicateWarnings);
                }

                if (invalidWarnings.Count > 0)
                {
                    if (warningMessage.Length > 0) warningMessage += "\n\n";
                    warningMessage += "⚠️ LEVEL DEFs WITH INVALID VALUES:\n\n";
                    warningMessage += string.Join("\n", invalidWarnings);
                }

                warningMessage += "\n\nPlease fix these issues to ensure proper configuration.";

                EditorUtility.DisplayDialog("Level Def Validation Warnings", warningMessage, "OK");
                
                // 同时在Console中输出详细信息
                Debug.LogWarning("Level Definition validation completed with warnings:");
                if (duplicateWarnings.Count > 0)
                {
                    Debug.LogWarning($"Found {duplicateWarnings.Count} duplicate LevelDef(s)");
                    foreach (string warning in duplicateWarnings)
                    {
                        Debug.LogWarning(warning);
                    }
                }
                if (invalidWarnings.Count > 0)
                {
                    Debug.LogWarning($"Found {invalidWarnings.Count} LevelDefinition(s) with invalid values");
                    foreach (string warning in invalidWarnings)
                    {
                        Debug.LogWarning(warning);
                    }
                }
            }
            else
            {
                Debug.Log("✅ All Level Definitions have unique and valid LevelDefs!");
            }
        }

        private void AddLevelDefinitionsToList(bool replaceContent)
        {
            if (selectedLevelDefinitionList == null || foundLevelDefinitions.Count == 0)
                return;

            // 记录操作以便撤销
            Undo.RecordObject(selectedLevelDefinitionList, replaceContent ? "Replace Level Definition List" : "Add to Level Definition List");

            if (replaceContent)
            {
                selectedLevelDefinitionList.levelDefinitionList.Clear();
                selectedLevelDefinitionList.levelDefinitionList.AddRange(foundLevelDefinitions);
                Debug.Log($"Replaced Level Definition List with {foundLevelDefinitions.Count} definitions.");
            }
            else
            {
                int addedCount = 0;
                foreach (var levelDef in foundLevelDefinitions)
                {
                    // 检查是否已存在（避免重复）
                    if (!selectedLevelDefinitionList.levelDefinitionList.Contains(levelDef))
                    {
                        selectedLevelDefinitionList.levelDefinitionList.Add(levelDef);
                        addedCount++;
                    }
                }
                Debug.Log($"Added {addedCount} new Level Definitions to the list. Skipped {foundLevelDefinitions.Count - addedCount} duplicates.");
            }

            // 标记为已修改
            EditorUtility.SetDirty(selectedLevelDefinitionList);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", 
                replaceContent ? 
                $"Successfully replaced list with {foundLevelDefinitions.Count} Level Definitions." :
                $"Successfully added Level Definitions to the list.", 
                "OK");
        }

        private void SaveDefaults()
        {
            EditorPrefs.SetString(PREFS_DEFAULT_FOLDER, defaultFolderPath);
            EditorPrefs.SetString(PREFS_DEFAULT_LIST, defaultLevelDefinitionListPath);
            EditorUtility.DisplayDialog("Defaults Saved", "Default folder and Level Definition List paths have been saved.", "OK");
        }

        private void LoadDefaults()
        {
            defaultFolderPath = EditorPrefs.GetString(PREFS_DEFAULT_FOLDER, "Assets/Resources/Data/LevelDefinitions");
            defaultLevelDefinitionListPath = EditorPrefs.GetString(PREFS_DEFAULT_LIST, "Assets/Resources/Data/ConfigDefintion/LevelDefinitionList.asset");
            EditorUtility.DisplayDialog("Defaults Loaded", "Default folder and Level Definition List paths have been loaded.", "OK");
        }

        private void ApplyDefaults()
        {
            selectedFolderPath = defaultFolderPath;

            if (!string.IsNullOrEmpty(defaultLevelDefinitionListPath))
            {
                selectedLevelDefinitionList = AssetDatabase.LoadAssetAtPath<LevelDefinitionList>(defaultLevelDefinitionListPath);
            }

            EditorUtility.DisplayDialog("Defaults Applied", "Default settings have been applied to the editor.", "OK");
        }
    }
}
