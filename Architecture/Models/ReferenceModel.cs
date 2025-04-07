using System;
using System.Linq;
using QFramework;
using TPL.PVZR.Gameplay.ViewControllers;
using TPL.PVZR.Gameplay.ViewControllers.InLevel;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TPL.PVZR.Architecture.Models
{
    public class ReferenceModel
    {   
        # region 公有方法

        public Seed GetSeed(int seedIndex)
        {
            foreach (var seed in _seeds)
            {
                if (seed.seedIndex == seedIndex) return seed;
            }
            throw new Exception($"未找到对应seedIndex的seed：{seedIndex}");
        }
        # endregion
        
        # region 属性
        # region MazeMap
        public Transform WorldSpaceCanvas
        {
            get
            {
                _WorldSpaceCanvas ??= GameObject.Find("WorldSpaceCanvas")?.transform;
                if (_WorldSpaceCanvas == null) throw new Exception("实例未找到");

                return _WorldSpaceCanvas;
            }
        }
        
        #endregion
        
        # region Level
        
        public Dave Dave
        {
            get
            {
                _Dave ??= Object.FindAnyObjectByType<Dave>();
                if (_Dave == null) throw new Exception("实例未找到");

                return _Dave;
            }
        }
        
        public Grid Grid
        {
            get
            {
                _Grid ??= Object.FindAnyObjectByType<Grid>();
                if (_Grid == null) throw new Exception("实例未找到");

                return _Grid;
            }
        }
        
        public Tilemap GroundTilemap
        {
            get
            {
                _GroundTilemap ??= GameObject.Find("GroundTilemap")?.GetComponent<Tilemap>();
                if (_GroundTilemap == null) throw new Exception("实例未找到");

                return _GroundTilemap;
            }
        }
        
        public Tilemap BoundTilemap
        {
            get
            {
                _BoundTilemap ??= GameObject.Find("BoundTilemap")?.GetComponent<Tilemap>();
                if (_BoundTilemap == null) throw new Exception("实例未找到");

                return _BoundTilemap;
            }
        }
        
        public Tilemap DirtTilemap
        {
            get
            {
                _DirtTilemap ??= GameObject.Find("DirtNotice")?.GetComponent<Tilemap>();
                if (_DirtTilemap == null) throw new Exception("实例未找到");

                return _DirtTilemap;
            }
        }
        
        public GameObject shovel
        {
            get
            {
                _shovel ??= GameObject.Find("Shovel")?.gameObject;
                if (_shovel == null) throw new Exception("实例未找到");

                return _shovel;
            }
        }
        // HandSystem里面要用到，似乎有些多余，但不想改以前的代码了
        public Image ShovelImage
        {
            get
            {
                _ShovelImage ??= GameObject.Find("ShovelImage")?.GetComponent<Image>();
                if (_ShovelImage == null) throw new Exception("实例未找到");

                return _ShovelImage;
            }
        }
        
        public Seed[] seeds
        {
            get
            {
                if (!_seeds.Any() || _seeds.Any(seed => seed == null))
                {
                    _seeds = Object.FindObjectsByType<Seed>(FindObjectsSortMode.None);
                }
                if (!_seeds.Any()) throw new Exception("实例未找到");

                return _seeds;
            }
        }
        
        #endregion
        #endregion

        # region 私有
        private Transform _WorldSpaceCanvas;
        private Dave _Dave;
        private Grid _Grid;
        private Tilemap _GroundTilemap;
        private Tilemap _BoundTilemap;
        private Tilemap _DirtTilemap;
        private GameObject _shovel;
        private Image _ShovelImage;
        private Slider _LevelStageBar;
        private Seed[] _seeds;
        #endregion
        
        # region Singleton
        public static ReferenceModel Get { get;private set; }
        static ReferenceModel()
        {
            Get = new ReferenceModel();
        }
        # endregion
    }
}