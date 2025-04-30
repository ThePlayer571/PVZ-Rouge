using System.Collections.Generic;
using QAssetBundle;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Managers;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Core;
using TPL.PVZR.Gameplay.Class;
using TPL.PVZR.Gameplay.Class.ZombieAI;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;


namespace TPL.PVZR.Architecture.Systems.InLevel
{
    public interface IZombieAISystem : ISystem
    {
        Matrix<Vertex> matrix { get; }
    }

// 用于僵尸AI的计算单元
    public class ZombieAIUnit : ICanGetModel
    {
        #region 数据存储

        public Matrix<Vertex> mapMatrix;
        public List<Vertex> vertices;
        public List<Vertex> keyVertices;
        public Dictionary<Vertex, List<Edge>> adjacencyList;

        #endregion

        #region 初始化

        public void ReadFromLevelModel()
        {
            var _LevelModel = this.GetModel<ILevelModel>();
            // 初始化
            mapMatrix = new Matrix<Vertex>(_LevelModel.MapConfig.size.x, _LevelModel.MapConfig.size.y);
            vertices = new List<Vertex>();
            keyVertices = new List<Vertex>();
            adjacencyList = new Dictionary<Vertex, List<Edge>>();
            // [STEP 1] 记录所有结点
            var CellGrid = _LevelModel.CellGrid;
            for (int x = 1; x < _LevelModel.MapConfig.size.x - 1; x++)
            {
                // y = 0 一定是基岩，没必要考虑 (其实还有其他无需考虑的值，但只有这个会报错所以。。)
                for (int y = 1; y < _LevelModel.MapConfig.size.y - 1; y++)
                {
                    // ↓ 一些稍微复杂的逻辑
                    if (CellGrid[x, y].IsEmpty && CellGrid[x, y - 1].IsStableTile)
                    {
                        if (CellGrid[x, y + 1].IsEmpty)
                        {
                            var newVertex = new Vertex(x, y, AllowPassHeight.TwoAndMore);
                            mapMatrix[x, y] = newVertex;
                            vertices.Add(newVertex);
                        }
                        else
                        {
                            var newVertex = new Vertex(x, y, AllowPassHeight.One);
                            mapMatrix[x, y] = newVertex;
                            vertices.Add(newVertex);
                        }
                    }
                }
            }

            $"找到结点数量{vertices.Count}".LogInfo();

            // [STEP 2] 遍历结点，构建边 | 设置关键结点
            foreach (var vertex in vertices)
            {
                adjacencyList[vertex] = new List<Edge>();
                var adjacentEdges = adjacencyList[vertex];
                // [TYPE 1] WalkJump
                foreach (var adjacentVertex in mapMatrix.GetNeighbors(vertex.x, vertex.y))
                {
                    if (adjacentVertex is not null)
                    {
                        adjacentEdges.Add(new Edge(vertex, adjacentVertex, Edge.EdgeType.WalkJump));
                    }
                }

                // [TYPE 2] Fall
                foreach (int x in new int[2] { vertex.x - 1, vertex.x + 1 })
                // 检测左右两格
                {
                    // [STEP 0] 排除不可能Fall的情况
                    // Case 0 不用掉落，WalkJump就行了
                    if (mapMatrix[x, vertex.y] is not null || mapMatrix[x, vertex.y + 1] is not null ||
                        mapMatrix[x, vertex.y - 1] is not null)
                    {
                        continue;
                    }
                    // Case 0 根本没法往下走
                    if (CellGrid[x, vertex.y].IsStableTile)
                    {
                        // 障碍并非是Vertex，不得不借用CellGrid（其实也不是啥问题）
                        continue;
                    }
                    // [STEP 1] 判断通路高度
                    var allowPassHeight = AllowPassHeight.NotSet;
                    // Case 1 一格高的通路
                    // Case 2 两格高的通路
                    if (CellGrid[x, vertex.y].IsEmpty && CellGrid[x, vertex.y + 1].IsStableTile)
                    {
                        allowPassHeight = AllowPassHeight.One;
                    }
                    else
                    {
                        allowPassHeight = AllowPassHeight.TwoAndMore;
                    }

                    // [STEP 2] 向下遍历
                    for (int y = vertex.y - 2; y > 0; y--)
                        // 可以直接从WalkJump无法考虑到的高度开始（虽然似乎有点冒险）
                    {
                        if (mapMatrix[x,y] is null) continue;
                        var toVertex = mapMatrix[x, y];
                        // 设置边
                        adjacentEdges.Add(new Edge(vertex, toVertex, Edge.EdgeType.Fall, allowPassHeight));
                        // 设置key
                        vertex.isKey = true;
                        toVertex.isKey = true;
                        keyVertices.Add(vertex);
                        keyVertices.Add(toVertex);
                        break;
                    }
                }
            }

            $"找到关键结点数量{keyVertices.Count}".LogInfo();
        }

        #endregion

        #region 调试

        public void DisplayTheMap()
        {
            var tilemapGroup = ReferenceModel.Get.TilemapGroup;
            var OneHeight = ResLoader.Allocate().LoadSync<Tile>(Oneheight_asset.OneHeight);
            var TwoHeight = ResLoader.Allocate().LoadSync<Tile>(Twoheight_asset.TwoHeight);
            var Key = ResLoader.Allocate().LoadSync<Tile>(Key_asset.Key);
            foreach (var each in vertices)
            {
                if (each.isKey)
                {
                    tilemapGroup.Test.SetTile(new Vector3Int(each.x, each.y, 0), Key);
                }
                else
                {
                    if (each.allowPassHeight is AllowPassHeight.One)
                    {
                        tilemapGroup.Test.SetTile(new Vector3Int(each.x, each.y, 0), OneHeight);
                    }
                    else
                    {
                        tilemapGroup.Test.SetTile(new Vector3Int(each.x, each.y, 0), TwoHeight);
                    }
                }
            }
        }

        #endregion

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }

    public class ZombieAISystem : AbstractSystem, IZombieAISystem
    {
        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    var _ = new ZombieAIUnit();
                    _.ReadFromLevelModel();
                    _.DisplayTheMap();
                }
            });
        }

        public Matrix<Vertex> matrix { get; private set; }
    }
}