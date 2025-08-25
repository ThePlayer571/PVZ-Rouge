using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace TPL.PVZR.Tools.Save
{
    public enum SavePathId
    {
        GameData,
        CurrentGameStats,
        PlayerStats_LevelEndedInfo,
        PlayerTipData
    }

    public class SaveManager
    {
        public const string GAME_DATA_FILE_NAME = "Save/GameData";
        public const string CURRENT_GAME_STATS_FILE_NAME = "Save/PlayerStats/CurrentGameStats";
        public const string PLAYER_STATS_LEVEL_ENDED_INFO_FILE_NAME = "Save/PlayerStats/LevelEndedInfo";
        public const string PLAYER_TIP_DATA_FILE_NAME = "Save/PlayerTipData";

        public static string GetFileName(SavePathId pathId)
        {
            return pathId switch
            {
                SavePathId.GameData => GAME_DATA_FILE_NAME,
                SavePathId.CurrentGameStats => CURRENT_GAME_STATS_FILE_NAME,
                SavePathId.PlayerStats_LevelEndedInfo =>
                    $"{SaveManager.PLAYER_STATS_LEVEL_ENDED_INFO_FILE_NAME}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}",
                SavePathId.PlayerTipData => PLAYER_TIP_DATA_FILE_NAME,
                _ => throw new ArgumentOutOfRangeException(nameof(pathId), pathId, null)
            };
        }

        /// <summary>
        /// 加载存档数据（通过 SavePathId），支持异常处理和默认值
        /// </summary>
        /// <typeparam name="TSave">存档数据类型</typeparam>
        /// <param name="pathId">存档路径标识</param>
        /// <param name="defaultValue">加载失败时的默认值</param>
        /// <returns>加载的数据或默认值</returns>
        public TSave Load<TSave>(SavePathId pathId, TSave defaultValue = default(TSave)) where TSave : ISaveData
        {
            return Load<TSave>(GetFileName(pathId), defaultValue);
        }

        /// <summary>
        /// 加载存档数据，支持异常处理和默认值
        /// </summary>
        /// <typeparam name="TSave">存档数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="defaultValue">加载失败时的默认值</param>
        /// <returns>加载的数据或默认值</returns>
        public TSave Load<TSave>(string fileName, TSave defaultValue = default(TSave)) where TSave : ISaveData
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, fileName);

                if (!File.Exists(path))
                {
                    Debug.LogWarning($"存档文件不存在: {path}，返回默认值");
                    return defaultValue;
                }

                string json = File.ReadAllText(path);

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning($"存档文件为空: {path}，返回默认值");
                    return defaultValue;
                }

                var result = JsonConvert.DeserializeObject<TSave>(json);
                return result ?? defaultValue;
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载存档失败 - 文件: {fileName}, 错误: {ex.Message}，返回默认值");
                return defaultValue;
            }
        }

        /// <summary>
        /// 保存数据到文件，带异常处理
        /// </summary>
        /// <typeparam name="TSave">存档数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="saveData">要保存的数据</param>
        /// <param name="TEMP_SaveEnumAsString">控制enum序列化为string</param>
        /// <returns>保存是否成功</returns>
        public bool Save<TSave>(string fileName, TSave saveData, bool TEMP_SaveEnumAsString = false) where TSave : ISaveData
        {
            try
            {
                if (saveData == null)
                {
                    Debug.LogError($"保存数据为空: {fileName}");
                    return false;
                }

                string path = Path.Combine(Application.persistentDataPath, fileName);

                // 确保目录存在
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json;
                if (TEMP_SaveEnumAsString)
                {
                    var settings = new JsonSerializerSettings
                    {
                        Converters = new List<JsonConverter> { new StringEnumConverter() },
                        Formatting = Formatting.Indented
                    };
                    json = JsonConvert.SerializeObject(saveData, settings);
                }
                else
                {
                    json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                }
                File.WriteAllText(path, json);

                Debug.Log($"存档保存成功: {path}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"保存存档失败 - 文件: {fileName}, 错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 保存数据到文件（通过 SavePathId），带异常处理
        /// </summary>
        /// <typeparam name="TSave">存档数据类型</typeparam>
        /// <param name="pathId">存档路径标识</param>
        /// <param name="saveData">要保存的数据</param>
        /// <param name="TEMP_SaveEnumAsString">控制enum序列化为string</param>
        /// <returns>保存是否成功</returns>
        public bool Save<TSave>(SavePathId pathId, TSave saveData, bool TEMP_SaveEnumAsString = false) where TSave : ISaveData
        {
            return Save(GetFileName(pathId), saveData, TEMP_SaveEnumAsString);
        }

        /// <summary>
        /// 删除存档文件（通过 SavePathId）
        /// </summary>
        /// <param name="pathId">要删除的文件路径标识</param>
        /// <returns>删除是否成功</returns>
        public bool Delete(SavePathId pathId)
        {
            return Delete(GetFileName(pathId));
        }

        /// <summary>
        /// 删除存档文件
        /// </summary>
        /// <param name="fileName">要删除的文件名</param>
        /// <returns>删除是否成功</returns>
        public bool Delete(string fileName)
        {
            try
            {
                string path = Path.Combine(Application.persistentDataPath, fileName);

                if (!File.Exists(path))
                {
                    Debug.LogWarning($"存档文件不存在，无需删除: {path}");
                    return true; // 文件不存在也算删除成功
                }

                File.Delete(path);
                Debug.Log($"存档删除成功: {path}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"删除存档失败 - 文件: {fileName}, 错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 判断存档文件是否存在（通过 SavePathId）
        /// </summary>
        /// <param name="pathId">文件路径标识</param>
        /// <returns>文件是否存在</returns>
        public bool Exists(SavePathId pathId)
        {
            return Exists(GetFileName(pathId));
        }

        public bool Exists(string fileName)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            return File.Exists(path);
        }
    }
}