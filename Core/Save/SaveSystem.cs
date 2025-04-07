using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QFramework;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using TPL.PVZR.Architecture.Events.Save;
using TPL.PVZR.Core.Save.Modules;

namespace TPL.PVZR.Core.Save
{
    public class SaveSystem : AbstractSystem
    {

        # region 公有方法
        // 获取数据
        public ISaveModule GetModule(string key)
        {
            foreach (var module in _modules)
            {
                if (module.ModuleKey == key)
                {
                    return module;
                }
            }
            throw new ArgumentException($"不存在对应的键：{key}");
        }
        
        // 保存到指定存档槽
        public void SaveAll(string slot)
        {
            this.SendEvent<OnSaveBegin>();

            var profile = new SaveProfile
            {
                timestamp = DateTime.Now,
                version = Application.version
            };

            foreach (var module in _modules)
            {
                string data = module.Save();
                profile.moduleData[module.ModuleKey] = data;
            }

            string json = JsonConvert.SerializeObject(profile);
            string path = GetSavePath(slot);
            File.WriteAllText(path, json);

            this.SendEvent<OnSaveComplete>();
        }


        // 加载存档
        public void LoadAll(string slot)
        {
            this.SendEvent<OnLoadBegin>();
            string path = GetSavePath(slot);
            if (!File.Exists(path)) return;

            string json = File.ReadAllText(path);
            var profile = JsonConvert.DeserializeObject<SaveProfile>(json);

            if (!ValidateVersion(profile.version))
            {
                Debug.LogError("版本不兼容");
                return;
            }

            foreach (var module in _modules)
            {
                if (profile.moduleData.TryGetValue(module.ModuleKey, out string data))
                {
                    module.Load(data);
                }
            }

            this.SendEvent<OnLoadComplete>();
        }

        # endregion

        # region 私有

        private List<ISaveModule> _modules = new();

        protected override void OnInit()
        {
            RegisterModule(new GameSaveModule());
        }
        // 注册存储模块
        private void RegisterModule(ISaveModule module)
        {
            if (!_modules.Contains(module))
                _modules.Add(module);
        }

        private static string GetSavePath(string slot)
        {
            return Path.Combine(
                Application.persistentDataPath,
                $"save_{slot}.sav"
            );
        }

        private bool ValidateVersion(string saveVersion)
        {
            // 语义化版本校验
            Version current = new Version(Application.version);
            Version saved = new Version(saveVersion);
            return current.Major == saved.Major; // 主版本号一致即可加载
        }

        #endregion
    }
}