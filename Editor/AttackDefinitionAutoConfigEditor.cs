using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using TPL.PVZR.Classes.DataClasses_InLevel.Attack;
using TPL.PVZR.Classes.ConfigLists;

namespace TPL.PVZR.Editor
{
    public class AttackDefinitionAutoConfigEditor : EditorWindow
    {
        private string selectedFolderPath = "";
        private AttackDefinitionList selectedAttackDefinitionList;
        private List<AttackDefinition> foundAttackDefinitions = new List<AttackDefinition>();
        private Vector2 scrollPosition;
        private bool showPreview = false;
        private bool showDefaultSettings = false;

        // 默认值设置
        private string defaultFolderPath = "Assets/Resources/Data/AttackDefinitions";
        private string defaultAttackDefinitionListPath = "Assets/Resources/Data/ConfigDefintion/AttackDefinitionList.asset";

        // EditorPrefs键名
        private const string PREFS_DEFAULT_FOLDER = "AttackDefEditor_DefaultFolder";
        private const string PREFS_DEFAULT_LIST = "AttackDefEditor_DefaultList";

        [MenuItem("PVZRouge/Attack Definition Auto Config")]
        public static void ShowWindow()
        {
            GetWindow<AttackDefinitionAutoConfigEditor>("Attack Definition Auto Config");
        }

        private void OnEnable()
        {
            // 加载保存的默认值
            defaultFolderPath = EditorPrefs.GetString(PREFS_DEFAULT_FOLDER, "Assets/Resources/Data/AttackDefinitions");
            defaultAttackDefinitionListPath = EditorPrefs.GetString(PREFS_DEFAULT_LIST, "Assets/Resources/Data/ConfigDefintion/AttackDefinitionList.asset");
            
            // 如果当前选择为空，则使用默认值
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                selectedFolderPath = defaultFolderPath;
            }
            
            if (selectedAttackDefinitionList == null && !string.IsNullOrEmpty(defaultAttackDefinitionListPath))
            {
                selectedAttackDefinitionList = AssetDatabase.LoadAssetAtPath<AttackDefinitionList>(defaultAttackDefinitionListPath);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Attack Definition Auto Configuration Tool", EditorStyles.boldLabel);
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

                EditorGUILayout.LabelField("Default Attack Definition List:", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                defaultAttackDefinitionListPath = EditorGUILayout.TextField(defaultAttackDefinitionListPath);
                if (GUILayout.Button("Browse", GUILayout.Width(60)))
                {
                    string path = EditorUtility.OpenFilePanel("Select Default Attack Definition List", "Assets", "asset");
                    if (!string.IsNullOrEmpty(path) && path.StartsWith(Application.dataPath))
                    {
                        defaultAttackDefinitionListPath = "Assets" + path.Substring(Application.dataPath.Length);
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
                if (GUILayout.Button("Scan for Attack Definitions"))
                {
                    ScanForAttackDefinitions();
                }
            }

            EditorGUILayout.Space();

            // 选择AttackDefinitionList文件部分
            EditorGUILayout.LabelField("Step 2: Select Attack Definition List", EditorStyles.boldLabel);
            selectedAttackDefinitionList = (AttackDefinitionList)EditorGUILayout.ObjectField(
                "Attack Definition List:", 
                selectedAttackDefinitionList, 
                typeof(AttackDefinitionList), 
                false
            );

            EditorGUILayout.Space();

            // 显示扫描结果
            if (foundAttackDefinitions.Count > 0)
            {
                EditorGUILayout.LabelField($"Found {foundAttackDefinitions.Count} Attack Definitions:", EditorStyles.boldLabel);
                
                showPreview = EditorGUILayout.Foldout(showPreview, "Preview Found Definitions");
                if (showPreview)
                {
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                    foreach (var attackDef in foundAttackDefinitions)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(attackDef, typeof(AttackDefinition), false);
                        EditorGUILayout.LabelField($"ID: {attackDef.attackId}", GUILayout.Width(100));
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.Space();

                // 配置按钮
                if (selectedAttackDefinitionList != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add All to List", GUILayout.Height(30)))
                    {
                        AddAttackDefinitionsToList(false);
                    }
                    if (GUILayout.Button("Replace List Content", GUILayout.Height(30)))
                    {
                        AddAttackDefinitionsToList(true);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.HelpBox("Add All: Adds found definitions to existing list (avoiding duplicates)\nReplace: Clears list and adds only found definitions", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please select an Attack Definition List to configure.", MessageType.Warning);
                }
            }
        }

        private void SelectFolder()
        {
            string path = EditorUtility.OpenFolderPanel("Select Folder to Scan for Attack Definitions", "Assets", "");
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

        private void ScanForAttackDefinitions()
        {
            foundAttackDefinitions.Clear();

            if (string.IsNullOrEmpty(selectedFolderPath))
                return;

            // 获取所有AttackDefinition类型的GUID
            string[] guids = AssetDatabase.FindAssets("t:AttackDefinition", new[] { selectedFolderPath });
            
            // 用于检测重复AttackID的字典
            Dictionary<AttackId, List<string>> attackIdFileMap = new Dictionary<AttackId, List<string>>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AttackDefinition attackDef = AssetDatabase.LoadAssetAtPath<AttackDefinition>(assetPath);
                
                if (attackDef != null)
                {
                    foundAttackDefinitions.Add(attackDef);
                    
                    // 记录AttackID对应的文件路径
                    if (!attackIdFileMap.ContainsKey(attackDef.attackId))
                    {
                        attackIdFileMap[attackDef.attackId] = new List<string>();
                    }
                    attackIdFileMap[attackDef.attackId].Add(assetPath);
                }
            }

            // 检查重复的AttackID
            CheckForDuplicateAttackIds(attackIdFileMap);

            // 按攻击ID排序
            foundAttackDefinitions.Sort((a, b) => a.attackId.CompareTo(b.attackId));

            Debug.Log($"Found {foundAttackDefinitions.Count} Attack Definitions in {selectedFolderPath}");
        }

        private void CheckForDuplicateAttackIds(Dictionary<AttackId, List<string>> attackIdFileMap)
        {
            List<string> duplicateWarnings = new List<string>();
            List<string> notSetWarnings = new List<string>();

            foreach (var kvp in attackIdFileMap)
            {
                AttackId attackId = kvp.Key;
                List<string> filePaths = kvp.Value;

                // 检查AttackID是否为NotSet
                if (attackId == AttackId.NotSet)
                {
                    foreach (string filePath in filePaths)
                    {
                        notSetWarnings.Add($"• {System.IO.Path.GetFileName(filePath)} - AttackID is NotSet");
                    }
                }
                // 检查是否有重复的AttackID
                else if (filePaths.Count > 1)
                {
                    string fileList = string.Join("\n  ", filePaths.ConvertAll(path => $"• {System.IO.Path.GetFileName(path)}"));
                    duplicateWarnings.Add($"AttackID '{attackId}' is used by multiple files:\n  {fileList}");
                }
            }

            // 显示警告对话框
            if (duplicateWarnings.Count > 0 || notSetWarnings.Count > 0)
            {
                string warningMessage = "";
                
                if (duplicateWarnings.Count > 0)
                {
                    warningMessage += "⚠️ DUPLICATE ATTACK IDs DETECTED:\n\n";
                    warningMessage += string.Join("\n\n", duplicateWarnings);
                }

                if (notSetWarnings.Count > 0)
                {
                    if (warningMessage.Length > 0) warningMessage += "\n\n";
                    warningMessage += "⚠️ ATTACK IDs NOT SET:\n\n";
                    warningMessage += string.Join("\n", notSetWarnings);
                }

                warningMessage += "\n\nPlease fix these issues to ensure proper configuration.";

                EditorUtility.DisplayDialog("Attack ID Validation Warnings", warningMessage, "OK");
                
                // 同时在Console中输出详细信息
                Debug.LogWarning("Attack Definition validation completed with warnings:");
                if (duplicateWarnings.Count > 0)
                {
                    Debug.LogWarning($"Found {duplicateWarnings.Count} duplicate AttackID(s)");
                    foreach (string warning in duplicateWarnings)
                    {
                        Debug.LogWarning(warning);
                    }
                }
                if (notSetWarnings.Count > 0)
                {
                    Debug.LogWarning($"Found {notSetWarnings.Count} AttackDefinition(s) with NotSet ID");
                    foreach (string warning in notSetWarnings)
                    {
                        Debug.LogWarning(warning);
                    }
                }
            }
            else
            {
                Debug.Log("✅ All Attack Definitions have unique AttackIDs!");
            }
        }

        private void AddAttackDefinitionsToList(bool replaceContent)
        {
            if (selectedAttackDefinitionList == null || foundAttackDefinitions.Count == 0)
                return;

            // 记录操作以便撤销
            Undo.RecordObject(selectedAttackDefinitionList, replaceContent ? "Replace Attack Definition List" : "Add to Attack Definition List");

            if (replaceContent)
            {
                selectedAttackDefinitionList.attackDefinitionList.Clear();
                selectedAttackDefinitionList.attackDefinitionList.AddRange(foundAttackDefinitions);
                Debug.Log($"Replaced Attack Definition List with {foundAttackDefinitions.Count} definitions.");
            }
            else
            {
                int addedCount = 0;
                foreach (var attackDef in foundAttackDefinitions)
                {
                    // 检查是否已存在（避免重复）
                    if (!selectedAttackDefinitionList.attackDefinitionList.Contains(attackDef))
                    {
                        selectedAttackDefinitionList.attackDefinitionList.Add(attackDef);
                        addedCount++;
                    }
                }
                Debug.Log($"Added {addedCount} new Attack Definitions to the list. Skipped {foundAttackDefinitions.Count - addedCount} duplicates.");
            }

            // 标记为已修改
            EditorUtility.SetDirty(selectedAttackDefinitionList);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog("Success", 
                replaceContent ? 
                $"Successfully replaced list with {foundAttackDefinitions.Count} Attack Definitions." :
                $"Successfully added Attack Definitions to the list.", 
                "OK");
        }

        private void SaveDefaults()
        {
            EditorPrefs.SetString(PREFS_DEFAULT_FOLDER, defaultFolderPath);
            EditorPrefs.SetString(PREFS_DEFAULT_LIST, defaultAttackDefinitionListPath);
            EditorUtility.DisplayDialog("Defaults Saved", "Default folder and Attack Definition List paths have been saved.", "OK");
        }

        private void LoadDefaults()
        {
            defaultFolderPath = EditorPrefs.GetString(PREFS_DEFAULT_FOLDER, "Assets/Resources/Data/AttackDefinitions");
            defaultAttackDefinitionListPath = EditorPrefs.GetString(PREFS_DEFAULT_LIST, "Assets/Resources/Data/ConfigDefintion/AttackDefinitionList.asset");
            EditorUtility.DisplayDialog("Defaults Loaded", "Default folder and Attack Definition List paths have been loaded.", "OK");
        }

        private void ApplyDefaults()
        {
            selectedFolderPath = defaultFolderPath;

            if (!string.IsNullOrEmpty(defaultAttackDefinitionListPath))
            {
                selectedAttackDefinitionList = AssetDatabase.LoadAssetAtPath<AttackDefinitionList>(defaultAttackDefinitionListPath);
            }

            EditorUtility.DisplayDialog("Defaults Applied", "Default settings have been applied to the editor.", "OK");
        }
    }
}
