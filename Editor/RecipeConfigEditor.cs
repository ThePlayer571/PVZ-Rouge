using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using TPL.PVZR.Classes.InfoClasses;

// by claude
namespace TPL.PVZR.Editor
{
    [Serializable]
    public class RecipeConfig
    {
        public int weight;
        public RecipeOutput output;
        public RecipeInput input;
    }

    [Serializable]
    public class RecipeOutput
    {
        public string card;
    }

    [Serializable]
    public class RecipeInput
    {
        public List<string> cards;
        public int[] coinRange;
    }

    public class RecipeConfigEditor : EditorWindow
    {
        private string csvFilePath = "Assets/Resources/Data/ConfigDefintion/RecipeConfigTable.csv";
        private string jsonFilePath = "Assets/Resources/Data/ConfigDefintion/RecipeConfigs.json";

        [MenuItem("PVZRouge/Recipe Config Editor")]
        public static void ShowWindow()
        {
            GetWindow<RecipeConfigEditor>("Recipe Config Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Recipe Config Editor", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // CSV文件路径
            GUILayout.Label("CSV File Path:", EditorStyles.label);
            csvFilePath = EditorGUILayout.TextField(csvFilePath);

            if (GUILayout.Button("Browse CSV File"))
            {
                string path =
                    EditorUtility.OpenFilePanel("Select CSV File", "Assets/Resources/Data/ConfigDefintion", "csv");
                if (!string.IsNullOrEmpty(path))
                {
                    csvFilePath = FileUtil.GetProjectRelativePath(path);
                }
            }

            EditorGUILayout.Space();

            // JSON文件路径
            GUILayout.Label("JSON File Path:", EditorStyles.label);
            jsonFilePath = EditorGUILayout.TextField(jsonFilePath);

            if (GUILayout.Button("Browse JSON File"))
            {
                string path = EditorUtility.SaveFilePanel("Select JSON File", "Assets/Resources/Data/ConfigDefintion",
                    "RecipeConfigs", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    jsonFilePath = FileUtil.GetProjectRelativePath(path);
                }
            }

            EditorGUILayout.Space();

            // 转换按钮
            if (GUILayout.Button("Convert CSV to JSON", GUILayout.Height(30)))
            {
                ConvertCSVToJSON();
            }

            EditorGUILayout.Space();

            // 验证按钮
            if (GUILayout.Button("Validate CSV Data"))
            {
                ValidateCSVData();
            }
        }

        private void ConvertCSVToJSON()
        {
            try
            {
                if (!File.Exists(csvFilePath))
                {
                    EditorUtility.DisplayDialog("Error", $"CSV file not found: {csvFilePath}", "OK");
                    return;
                }

                List<RecipeConfig> recipes = ParseCSVToRecipes(csvFilePath);

                if (recipes.Count == 0)
                {
                    EditorUtility.DisplayDialog("Warning", "No valid recipes found in CSV file", "OK");
                    return;
                }

                string json = JsonConvert.SerializeObject(recipes, Formatting.Indented);

                // 确保目录存在
                string directory = Path.GetDirectoryName(jsonFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(jsonFilePath, json);

                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Success",
                    $"Successfully converted {recipes.Count} recipes from CSV to JSON!\n" +
                    $"Output file: {jsonFilePath}", "OK");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to convert CSV to JSON:\n{ex.Message}", "OK");
            }
        }

        private List<RecipeConfig> ParseCSVToRecipes(string csvPath)
        {
            List<RecipeConfig> recipes = new List<RecipeConfig>();
            string[] lines = File.ReadAllLines(csvPath);

            // 跳过标题行
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                try
                {
                    RecipeConfig recipe = ParseCSVLine(line);
                    if (recipe != null)
                    {
                        recipes.Add(recipe);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to parse line {i + 1}: {line}\nError: {ex.Message}");
                }
            }

            return recipes;
        }

        private RecipeConfig ParseCSVLine(string line)
        {
            string[] parts = ParseCSVRow(line);
            if (parts.Length < 8) return null;

            string outputCard = parts[0].Trim();
            if (string.IsNullOrEmpty(outputCard)) return null;

            // 验证输出卡片是否有效
            if (!IsValidPlantId(outputCard))
            {
                Debug.LogWarning($"Invalid output card: {outputCard}");
                return null;
            }

            RecipeConfig recipe = new RecipeConfig
            {
                weight = int.Parse(parts[1]),
                output = new RecipeOutput { card = outputCard },
                input = new RecipeInput { cards = new List<string>() }
            };

            // 解析输入卡片 (Input列2-6)
            for (int i = 2; i <= 6; i++)
            {
                if (i < parts.Length)
                {
                    string inputCard = parts[i].Trim();
                    if (!string.IsNullOrEmpty(inputCard))
                    {
                        // 检查是否是金币范围格式 "(min,max)"
                        if (inputCard.StartsWith("(") && inputCard.EndsWith(")"))
                        {
                            string[] coinRange = inputCard.Substring(1, inputCard.Length - 2).Split(',');
                            if (coinRange.Length == 2)
                            {
                                recipe.input.coinRange = new int[]
                                {
                                    int.Parse(coinRange[0].Trim()),
                                    int.Parse(coinRange[1].Trim())
                                };
                            }
                        }
                        else
                        {
                            // 验证输入卡片是否有效
                            if (IsValidPlantId(inputCard))
                            {
                                recipe.input.cards.Add(inputCard);
                            }
                            else
                            {
                                Debug.LogWarning($"Invalid input card: {inputCard}");
                            }
                        }
                    }
                }
            }

            return recipe;
        }

        private string[] ParseCSVRow(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            result.Add(currentField);
            return result.ToArray();
        }

        private bool IsValidPlantId(string cardName)
        {
            // 将卡片名称转换为PlantId枚举检查
            return Enum.TryParse<PlantId>(cardName, true, out _);
        }

        private void ValidateCSVData()
        {
            try
            {
                if (!File.Exists(csvFilePath))
                {
                    EditorUtility.DisplayDialog("Error", $"CSV file not found: {csvFilePath}", "OK");
                    return;
                }

                string[] lines = File.ReadAllLines(csvFilePath);
                List<string> errors = new List<string>();

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] parts = ParseCSVRow(line);
                    if (parts.Length < 8)
                    {
                        errors.Add($"Line {i + 1}: Not enough columns");
                        continue;
                    }

                    // 验证输出卡片
                    string outputCard = parts[0].Trim();
                    if (!IsValidPlantId(outputCard))
                    {
                        errors.Add($"Line {i + 1}: Invalid output card '{outputCard}'");
                    }

                    // 验证输入卡片
                    for (int j = 2; j <= 6; j++)
                    {
                        if (j < parts.Length)
                        {
                            string inputCard = parts[j].Trim();
                            if (!string.IsNullOrEmpty(inputCard))
                            {
                                if (!(inputCard.StartsWith("(") && inputCard.EndsWith(")")) &&
                                    !IsValidPlantId(inputCard))
                                {
                                    errors.Add($"Line {i + 1}: Invalid input card '{inputCard}'");
                                }
                            }
                        }
                    }
                }

                if (errors.Count == 0)
                {
                    EditorUtility.DisplayDialog("Validation Success", "CSV data is valid!", "OK");
                }
                else
                {
                    string errorMessage = "Validation errors found:\n" + string.Join("\n", errors);
                    EditorUtility.DisplayDialog("Validation Errors", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to validate CSV:\n{ex.Message}", "OK");
            }
        }
    }
}