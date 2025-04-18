using System;
using System.Linq;
using TPL.PVZR.Gameplay.ViewControllers.InLevel;
using TPL.PVZR.Gameplay.ViewControllers.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TPL.PVZR.Architecture.Managers
{
    public class ReferenceModel
    {
        # region 公有方法

        public Seed GetSeed(int seedIndex)
        {
            foreach (var seed in seeds)
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
                if (!_WorldSpaceCanvas)
                {
                    _WorldSpaceCanvas = GameObject.Find("WorldSpaceCanvas")?.transform;
                    if (!_WorldSpaceCanvas) throw new MissingReferenceException("实例未找到");
                }

                return _WorldSpaceCanvas;
            }
        }

        #endregion

        # region Level

        public Dave Dave
        {
            get
            {
                if (!_Dave)
                {
                    _Dave = Object.FindAnyObjectByType<Dave>();
                    if (!_Dave) throw new MissingReferenceException("实例未找到");
                }

                return _Dave;
            }
        }

        public Grid Grid
        {
            get
            {
                if (!_Grid)
                {
                    _Grid = Object.FindAnyObjectByType<Grid>();
                    if (!_Grid) throw new MissingReferenceException("实例未找到");
                }

                return _Grid;
            }
        }

        public Tilemap GroundTilemap
        {
            get
            {
                if (!_GroundTilemap)
                {
                    _GroundTilemap = GameObject.Find("Ground")?.GetComponent<Tilemap>();
                    if (!_GroundTilemap) throw new MissingReferenceException("实例未找到");
                }

                return _GroundTilemap;
            }
        }

        public Tilemap BoundTilemap
        {
            get
            {
                if (!_BoundTilemap)
                {
                    _BoundTilemap = GameObject.Find("Bound")?.GetComponent<Tilemap>();
                    if (!_BoundTilemap) throw new MissingReferenceException("实例未找到");
                }

                return _BoundTilemap;
            }
        }

        public Tilemap DirtTilemap
        {
            get
            {
                if (!_DirtTilemap)
                {
                    _DirtTilemap = GameObject.Find("DirtNotice")?.GetComponent<Tilemap>();
                    if (!_DirtTilemap) throw new MissingReferenceException("实例未找到");
                }

                return _DirtTilemap;
            }
        }

        public GameObject shovel
        {
            get
            {
                if (!_shovel)
                {
                    _shovel = GameObject.Find("Shovel");
                    if (!_shovel) throw new MissingReferenceException("实例未找到");
                }

                return _shovel;
            }
        }

        public GameObject FollowingSprite
        {
            get
            {
                if (!_FollowingSprite)
                {
                    _FollowingSprite = GameObject.Find("FollowingSprite");
                    if (!_FollowingSprite) throw new MissingReferenceException("实例未找到");
                }

                return _FollowingSprite;
            }
        }

        public GameObject SelectFramebox
        {
            get
            {
                if (!_SelectFramebox)
                {
                    _SelectFramebox = GameObject.Find("SelectFramebox");
                    if (!_SelectFramebox) throw new MissingReferenceException("实例未找到");
                }

                return _SelectFramebox;
            }
        }

        // HandSystem里面要用到，似乎有些多余，但不想改以前的代码了
        public Image ShovelImage
        {
            get
            {
                if (!_ShovelImage)
                {
                    _ShovelImage = GameObject.Find("ShovelImage")?.GetComponent<Image>();
                    if (!_ShovelImage) throw new MissingReferenceException("实例未找到");
                }

                return _ShovelImage;
            }
        }

        public Seed[] seeds
        {
            get
            {
                if (_seeds == null || !_seeds.Any() || _seeds.Any(seed => !seed))
                {
                    _seeds = Object.FindObjectsByType<Seed>(FindObjectsSortMode.None);
                    if (_seeds == null || !_seeds.Any() || _seeds.Any(seed => !seed))
                        throw new MissingReferenceException("实例未找到");
                }
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
        private GameObject _FollowingSprite;
        private GameObject _SelectFramebox;

        #endregion

        # region Singleton

        public static ReferenceModel Get { get; private set; }

        static ReferenceModel()
        {
            Get = new ReferenceModel();
        }

        # endregion
    }
}