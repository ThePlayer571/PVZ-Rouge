using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using TPL.PVZR.Classes.InfoClasses;

// by claude
namespace TPL.PVZR.Editor
{
    /// <summary>
    /// Recipe合成表配置编辑器
    /// 
    /// 工作流程：
    /// 1. 编辑CSV文件（按照下面的格式说明）
    /// 2. 点击"检查CSV数据"按钮进行验证和自动修正
    /// 3. 点击"转换CSV到JSON"按钮生成最终的JSON配置文件
    /// 
    /// CSV文件格式说明：
    /// - Output: 合成产品的PlantId
    /// - weight: 在商店中出现的概率权重
    /// - Input (5列): 输入材料，可以是PlantId或"(min,max)"格式的金币范围
    /// - InputValue: 投入产物的总价值（会自动计算）
    /// - OutputValue: 生成物的价值（会自动计算）
    /// 
    /// 机制：商店中随机出现合成表，玩家可以通过消耗材料和金币来合成新的植物
    /// </summary>
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
        private string csvFilePath = "Assets/Resource/Data/ConfigDefintion/RecipeConfigTable.csv";
        private string jsonFilePath = "Assets/Resource/Data/ConfigDefintion/JsonConfigs/RecipeConfigs.json";
        private string plantValuePath = "Assets/Resource/Data/ConfigDefintion/JsonConfigs/PlantValueList.json";

        private Dictionary<string, int> plantValueDict = new Dictionary<string, int>();
        private Vector2 scrollPosition;
        private string lastValidationResult = "";

        [MenuItem("PVZRouge/Recipe Config Editor")]
        public static void ShowWindow()
        {
            GetWindow<RecipeConfigEditor>("Recipe Config Editor");
        }

        private void OnEnable()
        {
            LoadPlantValues();
        }

        private void LoadPlantValues()
        {
            try
            {
                if (File.Exists(plantValuePath))
                {
                    string json = File.ReadAllText(plantValuePath);
                    plantValueDict = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
                }
                else
                {
                    Debug.LogWarning($"PlantValueList.json not found at: {plantValuePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load plant values: {ex.Message}");
            }
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Recipe Config Editor", EditorStyles.boldLabel);

            // 显示工作流程说明
            EditorGUILayout.Space();
            ShowDocumentation();

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

            // 主要操作按钮
            EditorGUILayout.LabelField("工作流程", EditorStyles.boldLabel);

            if (GUILayout.Button("1. 检查CSV数据 (Check & Auto-Fix)", GUILayout.Height(30)))
            {
                CheckAndFixCSVData();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("2. 转换CSV到JSON (Convert to JSON)", GUILayout.Height(30)))
            {
                ConvertCSVToJSON();
            }

            EditorGUILayout.Space();

            // 额外功能
            EditorGUILayout.LabelField("额外功能", EditorStyles.boldLabel);

            if (GUILayout.Button("显示未使用的PlantId"))
            {
                ShowUnusedPlantIds();
            }

            if (GUILayout.Button("重新加载植物价值表"))
            {
                LoadPlantValues();
            }

            // 显示验证结果
            if (!string.IsNullOrEmpty(lastValidationResult))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("最近检查结果:", EditorStyles.boldLabel);
                EditorGUILayout.TextArea(lastValidationResult, GUILayout.Height(100));
            }

            EditorGUILayout.EndScrollView();
        }

        private void ShowDocumentation()
        {
            EditorGUILayout.HelpBox(
                "CSV文件格式说明：\n" +
                "• Output: 合成产品的PlantId（如PeaShooter、Sunflower等）\n" +
                "• weight: 在商店中出现的概率权重（数字越大出现概率越高）\n" +
                "• Input (5列): 输入材料，可以是：\n" +
                "  - PlantId（如PeaShooter、Sunflower等）\n" +
                "  - 金币范围：\"(min-max)\"格式�����如\"(10-20)\"表示消耗10-20金币）\n" +
                "• InputValue: 投入产物的总价值（自动计算，可手动修正）\n" +
                "• OutputValue: 生成物的价值（自动计算，可手动修正）\n\n" +
                "工作流程：\n" +
                "1. 编辑CSV文件\n" +
                "2. 点击\"检查CSV数据\"进行验证和自动修正\n" +
                "3. 点击\"转换CSV到JSON\"生成最终配置文件",
                MessageType.Info);
        }

        private void CheckAndFixCSVData()
        {
            try
            {
                if (!File.Exists(csvFilePath))
                {
                    EditorUtility.DisplayDialog("Error", $"CSV file not found: {csvFilePath}", "OK");
                    return;
                }

                LoadPlantValues();

                string[] lines = File.ReadAllLines(csvFilePath);
                List<string> errors = new List<string>();
                List<string> fixes = new List<string>();
                List<string> newLines = new List<string>();

                // 保留标题行
                if (lines.Length > 0)
                {
                    newLines.Add(lines[0]);
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                    {
                        newLines.Add(line);
                        continue;
                    }

                    string[] parts = ParseCSVRow(line);
                    if (parts.Length < 8)
                    {
                        errors.Add($"Line {i + 1}: 列数不足，需要至少8列");
                        newLines.Add(line);
                        continue;
                    }

                    bool lineModified = false;
                    bool hasInvalidPlantIds = false; // 标记是否存在无效的PlantId

                    // 检查输出卡片
                    string outputCard = parts[0].Trim();
                    if (string.IsNullOrEmpty(outputCard))
                    {
                        newLines.Add(line);
                        continue;
                    }

                    int outputValidation = ValidatePlantIdWithCorrection(outputCard, out string correctedOutputCard);
                    if (outputValidation == 0)
                    {
                        errors.Add($"Line {i + 1}: 无效的输出植物ID '{outputCard}'");
                        hasInvalidPlantIds = true;
                    }
                    else if (outputValidation == 2)
                    {
                        parts[0] = correctedOutputCard;
                        fixes.Add($"Line {i + 1}: 修正输出植物ID大小写: '{outputCard}' -> '{correctedOutputCard}'");
                        lineModified = true;
                        outputCard = correctedOutputCard; // 更新变量���于后续计算
                    }

                    // 检查输入卡片和计算价值
                    List<string> inputCards = new List<string>();
                    int coinMin = 0, coinMax = 0;
                    bool hasCoinRange = false;

                    for (int j = 2; j <= 6; j++)
                    {
                        if (j < parts.Length)
                        {
                            string inputCard = parts[j].Trim();
                            if (!string.IsNullOrEmpty(inputCard))
                            {
                                if (inputCard.StartsWith("(") && inputCard.EndsWith(")"))
                                {
                                    // 金币范围 - 使用连字符分隔
                                    string coinRangeStr = inputCard.Substring(1, inputCard.Length - 2);
                                    string[] coinRange = coinRangeStr.Split('-');
                                    if (coinRange.Length == 2)
                                    {
                                        if (int.TryParse(coinRange[0].Trim(), out coinMin) &&
                                            int.TryParse(coinRange[1].Trim(), out coinMax))
                                        {
                                            hasCoinRange = true;
                                        }
                                        else
                                        {
                                            errors.Add($"Line {i + 1}: 无效的金币范围格式 '{inputCard}' - 数字解析失败");
                                            hasInvalidPlantIds = true;
                                        }
                                    }
                                    else
                                    {
                                        errors.Add($"Line {i + 1}: 无效的金币范围格式 '{inputCard}' - 应该使用格式(min-max)");
                                        hasInvalidPlantIds = true;
                                    }
                                }
                                else
                                {
                                    int inputValidation =
                                        ValidatePlantIdWithCorrection(inputCard, out string correctedInputCard);
                                    if (inputValidation == 0)
                                    {
                                        errors.Add($"Line {i + 1}: 无效的输入植物ID '{inputCard}'");
                                        hasInvalidPlantIds = true;
                                    }
                                    else if (inputValidation == 1)
                                    {
                                        inputCards.Add(correctedInputCard);
                                    }
                                    else if (inputValidation == 2)
                                    {
                                        fixes.Add(
                                            $"Line {i + 1}: 修正输入植物ID大小写: '{inputCard}' -> '{correctedInputCard}'");
                                        parts[j] = correctedInputCard;
                                        lineModified = true;
                                        inputCards.Add(correctedInputCard);
                                    }
                                }
                            }
                        }
                    }

                    // 只有在所有PlantId都有效时才修正价值
                    if (!hasInvalidPlantIds)
                    {
                        // 计算正确的InputValue和OutputValue
                        int correctInputValue =
                            CalculateInputValue(inputCards, hasCoinRange ? (coinMin + coinMax) / 2 : 0);
                        int correctOutputValue = GetPlantValue(outputCard);

                        // 检查和修正InputValue
                        if (parts.Length > 7)
                        {
                            string inputValueStr = parts[7].Trim();
                            if (string.IsNullOrEmpty(inputValueStr))
                            {
                                parts[7] = correctInputValue.ToString();
                                fixes.Add($"Line {i + 1}: 自动填入InputValue = {correctInputValue}");
                                lineModified = true;
                            }
                            else if (int.TryParse(inputValueStr, out int currentInputValue))
                            {
                                if (currentInputValue != correctInputValue)
                                {
                                    parts[7] = correctInputValue.ToString();
                                    fixes.Add(
                                        $"Line {i + 1}: 修正InputValue: {currentInputValue} -> {correctInputValue}");
                                    lineModified = true;
                                }
                            }
                            else
                            {
                                parts[7] = correctInputValue.ToString();
                                fixes.Add($"Line {i + 1}: 修正无效的InputValue '{inputValueStr}' -> {correctInputValue}");
                                lineModified = true;
                            }
                        }

                        // 检查和修正OutputValue
                        if (parts.Length > 8)
                        {
                            string outputValueStr = parts[8].Trim();
                            if (string.IsNullOrEmpty(outputValueStr))
                            {
                                parts[8] = correctOutputValue.ToString();
                                fixes.Add($"Line {i + 1}: 自动填入OutputValue = {correctOutputValue}");
                                lineModified = true;
                            }
                            else if (int.TryParse(outputValueStr, out int currentOutputValue))
                            {
                                if (currentOutputValue != correctOutputValue)
                                {
                                    parts[8] = correctOutputValue.ToString();
                                    fixes.Add(
                                        $"Line {i + 1}: 修正OutputValue: {currentOutputValue} -> {correctOutputValue}");
                                    lineModified = true;
                                }
                            }
                            else
                            {
                                parts[8] = correctOutputValue.ToString();
                                fixes.Add($"Line {i + 1}: 修正无效的OutputValue '{outputValueStr}' -> {correctOutputValue}");
                                lineModified = true;
                            }
                        }
                        else
                        {
                            // 扩展数组以包含OutputValue
                            Array.Resize(ref parts, 9);
                            parts[8] = correctOutputValue.ToString();
                            fixes.Add($"Line {i + 1}: 添加OutputValue = {correctOutputValue}");
                            lineModified = true;
                        }
                    }
                    else
                    {
                        // 如果存在无效的PlantId，说明跳过了价值修正
                        errors.Add($"Line {i + 1}: 由于存在无效的PlantId，跳过价值修正");
                    }

                    if (lineModified)
                    {
                        newLines.Add(string.Join(",", parts));
                    }
                    else
                    {
                        newLines.Add(line);
                    }
                }

                // 如果有修正，保存文件
                if (fixes.Count > 0)
                {
                    File.WriteAllLines(csvFilePath, newLines);
                    AssetDatabase.Refresh();
                }

                // 准备结果报告
                string result = "";
                if (errors.Count > 0)
                {
                    result += "发现的问题:\n" + string.Join("\n", errors) + "\n\n";
                }

                if (fixes.Count > 0)
                {
                    result += "自动修正:\n" + string.Join("\n", fixes) + "\n\n";
                }

                if (errors.Count == 0 && fixes.Count == 0)
                {
                    result = "CSV数据检查完成，没有发现问题！";
                }
                else
                {
                    result += $"检查完成！发现 {errors.Count} 个问题，进行了 {fixes.Count} 个自动修正。";
                }

                lastValidationResult = result;

                if (errors.Count > 0)
                {
                    EditorUtility.DisplayDialog("检查完成", result, "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("检查成功", result, "OK");
                }
            }
            catch (Exception ex)
            {
                string errorMsg = $"检查CSV数据时发生错误:\n{ex.Message}";
                lastValidationResult = errorMsg;
                EditorUtility.DisplayDialog("Error", errorMsg, "OK");
            }
        }

        private int CalculateInputValue(List<string> inputCards, int coinValue)
        {
            int total = coinValue;
            foreach (string card in inputCards)
            {
                total += GetPlantValue(card);
            }

            return total;
        }

        private int GetPlantValue(string plantId)
        {
            if (plantValueDict.ContainsKey(plantId))
            {
                return plantValueDict[plantId];
            }

            Debug.LogWarning($"Plant value not found for: {plantId}");
            return 0;
        }

        private void ShowUnusedPlantIds()
        {
            try
            {
                if (!File.Exists(csvFilePath))
                {
                    EditorUtility.DisplayDialog("Error", $"CSV file not found: {csvFilePath}", "OK");
                    return;
                }

                // 获取所有PlantId
                var allPlantIds = Enum.GetValues(typeof(PlantId)).Cast<PlantId>()
                    .Where(id => id != PlantId.NotSet)
                    .Select(id => id.ToString())
                    .ToHashSet();

                // 获取CSV中使用的PlantId
                var usedPlantIds = new HashSet<string>();
                string[] lines = File.ReadAllLines(csvFilePath);

                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line)) continue;

                    string[] parts = ParseCSVRow(line);
                    if (parts.Length < 2) continue;

                    // 输出植物
                    string outputCard = parts[0].Trim();
                    if (IsValidPlantId(outputCard))
                    {
                        usedPlantIds.Add(outputCard);
                    }

                    // 输入植物
                    for (int j = 2; j <= 6; j++)
                    {
                        if (j < parts.Length)
                        {
                            string inputCard = parts[j].Trim();
                            if (!string.IsNullOrEmpty(inputCard) &&
                                !inputCard.StartsWith("(") &&
                                IsValidPlantId(inputCard))
                            {
                                usedPlantIds.Add(inputCard);
                            }
                        }
                    }
                }

                var unusedPlantIds = allPlantIds.Except(usedPlantIds).ToList();
                unusedPlantIds.Sort();

                string message = unusedPlantIds.Count > 0
                    ? $"未在CSV中使用的PlantId ({unusedPlantIds.Count}个):\n\n{string.Join("\n", unusedPlantIds)}"
                    : "所有PlantId都已在CSV中使用！";

                lastValidationResult = message;
                EditorUtility.DisplayDialog("未使用的PlantId", message, "OK");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error", $"检查未使用PlantId时发生错误:\n{ex.Message}", "OK");
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
                            string[] coinRange = inputCard.Substring(1, inputCard.Length - 2).Split('-');
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
            bool inParentheses = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"' && !inParentheses)
                {
                    inQuotes = !inQuotes;
                }
                else if (c == '(' && !inQuotes)
                {
                    inParentheses = true;
                    currentField += c;
                }
                else if (c == ')' && !inQuotes && inParentheses)
                {
                    inParentheses = false;
                    currentField += c;
                }
                else if (c == ',' && !inQuotes && !inParentheses)
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

        /// <summary>
        /// 检查PlantId是否有效，如果只是大小写错误则返回正确的名称
        /// </summary>
        /// <param name="cardName">输入的植物名称</param>
        /// <param name="correctedName">修正后的名称</param>
        /// <returns>0=无效, 1=有效且大小写正确, 2=有效但大小写错误</returns>
        private int ValidatePlantIdWithCorrection(string cardName, out string correctedName)
        {
            correctedName = cardName;

            // 首先检查是否严格匹配（大小写敏感）
            if (Enum.TryParse<PlantId>(cardName, false, out PlantId exactMatch))
            {
                return 1; // 完全正确
            }

            // 然后检查是否忽略大小写匹配
            if (Enum.TryParse<PlantId>(cardName, true, out PlantId caseInsensitiveMatch))
            {
                // 找到正确的大小写格式
                correctedName = caseInsensitiveMatch.ToString();
                return 2; // 大小写错误但可以修正
            }

            return 0; // 完全无效
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