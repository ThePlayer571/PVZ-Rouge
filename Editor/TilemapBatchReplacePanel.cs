using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBatchReplacePanel : EditorWindow
{
    private string selectedFolderPath = "Assets/";
    private TileBase sourceTile;
    private TileBase targetTile;
    private List<GameObject> affectedPrefabs = new List<GameObject>();
    private Vector2 scrollPosition = Vector2.zero;
    private int currentStep = 1;

    [MenuItem("PVZRouge/Tilemap Batch Replace")]
    public static void ShowWindow()
    {
        GetWindow<TilemapBatchReplacePanel>("Tilemap批量替换工具");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Tilemap Tile 批量替换工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 显示当前步骤
        DisplayCurrentStep();
        EditorGUILayout.Space();

        // 步骤1：选择文件夹和Tile
        DisplayStep1();
        
        // 步骤2：显示受影响的文件
        if (currentStep >= 2)
        {
            DisplayStep2();
        }
        
        // 步骤3：执行替换
        if (currentStep >= 3)
        {
            DisplayStep3();
        }
    }

    private void DisplayCurrentStep()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前步骤:", GUILayout.Width(80));
        
        string stepText = "";
        switch (currentStep)
        {
            case 1:
                stepText = "1. 选择文件夹和Tile";
                break;
            case 2:
                stepText = "2. 查看受影响的文件";
                break;
            case 3:
                stepText = "3. 执行替换操作";
                break;
        }
        
        EditorGUILayout.LabelField(stepText, EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
    }

    private void DisplayStep1()
    {
        EditorGUILayout.LabelField("步骤1: 选择文件夹和Tile", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标文件夹:", GUILayout.Width(80));
        EditorGUILayout.TextField(selectedFolderPath);
        
        if (GUILayout.Button("选择文件夹", GUILayout.Width(100)))
        {
            string path = EditorUtility.OpenFolderPanel("选择包含Prefab的文件夹", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                // 转换为相对于项目的路径
                if (path.StartsWith(Application.dataPath))
                {
                    selectedFolderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "请选择项目Assets文件夹内的目录", "确定");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("要替换的Tile (源Tile):");
        sourceTile = EditorGUILayout.ObjectField(sourceTile, typeof(TileBase), false) as TileBase;
        
        EditorGUILayout.LabelField("替换为的Tile (目标Tile):");
        targetTile = EditorGUILayout.ObjectField(targetTile, typeof(TileBase), false) as TileBase;

        EditorGUILayout.Space();

        GUI.enabled = sourceTile != null && targetTile != null && !string.IsNullOrEmpty(selectedFolderPath);
        if (GUILayout.Button("扫描文件夹"))
        {
            ScanFolder();
        }
        GUI.enabled = true;
    }

    private void DisplayStep2()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("步骤2: 受影响的文件", EditorStyles.boldLabel);
        
        if (affectedPrefabs.Count == 0)
        {
            EditorGUILayout.HelpBox("未找到包含指定Tile的Prefab文件", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"找到 {affectedPrefabs.Count} 个包含目标Tile的Prefab:");
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        
        foreach (var prefab in affectedPrefabs)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
            if (GUILayout.Button("定位", GUILayout.Width(50)))
            {
                Selection.activeObject = prefab;
                EditorGUIUtility.PingObject(prefab);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("确认继续替换"))
        {
            currentStep = 3;
        }
    }

    private void DisplayStep3()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("步骤3: 执行替换", EditorStyles.boldLabel);
        
        EditorGUILayout.HelpBox("警告: 此操作将永久修改Prefab文件，建议先备份项目！", MessageType.Warning);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("执行替换", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("确认替换", 
                $"确定要将 {affectedPrefabs.Count} 个Prefab中的Tile进行替换吗？\n\n" +
                $"源Tile: {(sourceTile ? sourceTile.name : "None")}\n" +
                $"目标Tile: {(targetTile ? targetTile.name : "None")}", 
                "确认", "取消"))
            {
                PerformReplacement();
            }
        }
        
        if (GUILayout.Button("重新开始", GUILayout.Height(30)))
        {
            ResetTool();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ScanFolder()
    {
        affectedPrefabs.Clear();
        
        if (!Directory.Exists(selectedFolderPath))
        {
            EditorUtility.DisplayDialog("错误", "指定的文件夹不存在", "确定");
            return;
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { selectedFolderPath });
        
        int totalPrefabs = prefabGuids.Length;
        int processedPrefabs = 0;

        try
        {
            foreach (string guid in prefabGuids)
            {
                processedPrefabs++;
                EditorUtility.DisplayProgressBar("扫描Prefab", 
                    $"正在扫描 {processedPrefabs}/{totalPrefabs}", 
                    (float)processedPrefabs / totalPrefabs);

                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null && ContainsTile(prefab, sourceTile))
                {
                    affectedPrefabs.Add(prefab);
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        currentStep = 2;
        
        Debug.Log($"扫描完成！找到 {affectedPrefabs.Count} 个包含目标Tile的Prefab");
    }

    private bool ContainsTile(GameObject prefab, TileBase tile)
    {
        Tilemap[] tilemaps = prefab.GetComponentsInChildren<Tilemap>(true);
        
        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
            
            foreach (TileBase tileInMap in allTiles)
            {
                if (tileInMap == tile)
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private void PerformReplacement()
    {
        int totalPrefabs = affectedPrefabs.Count;
        int processedPrefabs = 0;
        int totalReplacements = 0;

        try
        {
            foreach (GameObject prefab in affectedPrefabs)
            {
                processedPrefabs++;
                EditorUtility.DisplayProgressBar("替换Tile", 
                    $"正在处理 {processedPrefabs}/{totalPrefabs}: {prefab.name}", 
                    (float)processedPrefabs / totalPrefabs);

                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

                int replacements = ReplaceTilesInGameObject(prefabInstance, sourceTile, targetTile);
                totalReplacements += replacements;

                if (replacements > 0)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                    Debug.Log($"在 {prefab.name} 中替换了 {replacements} 个Tile");
                }

                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("替换完成", 
            $"成功处理了 {totalPrefabs} 个Prefab\n总共替换了 {totalReplacements} 个Tile", "确定");
        
        Debug.Log($"批量替换完成！总共替换了 {totalReplacements} 个Tile");
    }

    private int ReplaceTilesInGameObject(GameObject obj, TileBase sourceToReplace, TileBase replacementTile)
    {
        int replacementCount = 0;
        Tilemap[] tilemaps = obj.GetComponentsInChildren<Tilemap>(true);

        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    TileBase currentTile = tilemap.GetTile(position);
                    
                    if (currentTile == sourceToReplace)
                    {
                        tilemap.SetTile(position, replacementTile);
                        replacementCount++;
                    }
                }
            }
        }

        return replacementCount;
    }

    private void ResetTool()
    {
        currentStep = 1;
        affectedPrefabs.Clear();
        sourceTile = null;
        targetTile = null;
        selectedFolderPath = "Assets/";
    }
}
