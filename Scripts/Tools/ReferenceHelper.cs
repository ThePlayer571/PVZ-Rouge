using System;
using QFramework;
using TPL.PVZR.UI;
using UnityEngine;

namespace TPL.PVZR.Core
{
    public static class ReferenceHelper
    {
        
        public static UIChooseSeedPanel ChooseSeedPanel
        {
            get
            {
                if (_ChooseSeedPanel) return _ChooseSeedPanel;

                _ChooseSeedPanel = GameObject.FindObjectOfType<UIChooseSeedPanel>();
                if (!_ChooseSeedPanel) throw new Exception("没有找到UIChooseSeedPanel，请确保它已被正确加载");

                return _ChooseSeedPanel;
            }
        }

        private static UIChooseSeedPanel _ChooseSeedPanel;
        //
        
        public static UILevelGameplayPanel LevelGameplayPanel
        {
            get
            {
                if (_LevelGameplayPanel) return _LevelGameplayPanel;

                _LevelGameplayPanel = GameObject.FindObjectOfType<UILevelGameplayPanel>();
                if (!_LevelGameplayPanel) throw new Exception("没有找到UILevelGameplayPanel，请确保它已被正确加载");

                return _LevelGameplayPanel;
            }
        }

        private static UILevelGameplayPanel _LevelGameplayPanel;
    }
}