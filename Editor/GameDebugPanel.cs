using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEditor;
using UnityEngine;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Helpers.New.ClassCreator;
using TPL.PVZR.Models;
using TPL.PVZR.Services;

public class GameDebugPanel : EditorWindow, ICanGetModel, ICanGetService
{
    private ZombieId selectedZombieId = ZombieId.NormalZombie;
    private Vector2 spawnPosition = new Vector2(22, 9);
    private ZombieSpawnPosId selectedSpawnPosId = ZombieSpawnPosId.Pos_1;
    private IList<string> paras = new List<string>();
    private string parasInput = ""; // 新增字段用于输入参数

    //
    private PlantDef plantDef = new PlantDef(PlantId.PeaShooter, PlantVariant.V0);


    [MenuItem("PVZRouge/Game Debug Panel")]
    public static void ShowWindow()
    {
        GetWindow<GameDebugPanel>("Zombie Summon Panel");
    }

    private void OnGUI()
    {
        #region 僵尸

        EditorGUILayout.LabelField("僵尸生成", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        selectedZombieId = (ZombieId)EditorGUILayout.EnumPopup(selectedZombieId);
        EditorGUILayout.Space();
        selectedSpawnPosId = (ZombieSpawnPosId)EditorGUILayout.EnumPopup(selectedSpawnPosId);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        spawnPosition = EditorGUILayout.Vector2Field("", spawnPosition);
        EditorGUILayout.Space();

        // 参数输入区域
        parasInput = EditorGUILayout.TextField("参数(;分隔)", parasInput);
        paras = string.IsNullOrEmpty(parasInput) ? new List<string>() : parasInput.Split(';').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p)).ToList();
        EditorGUILayout.Space();


        if (GUILayout.Button("生成僵尸"))
        {
            SpawnZombie();
        }

        if (GUILayout.Button("杀死所有僵尸"))
        {
            var zombieService = this.GetService<IZombieService>();
            var list = zombieService.ActiveZombies.ToList();
            foreach (var zombie in list)
            {
                zombie.Kill();
            }
        }

        #endregion

        #region Level

        EditorGUILayout.LabelField("关卡", EditorStyles.boldLabel);
        if (GUILayout.Button("获得500阳光"))
        {
            this.GetModel<ILevelModel>().SunPoint.Value += 500;
        }

        if (GUILayout.Button("重置冷却并获得25阳光"))
        {
            foreach (var seed in this.GetModel<ILevelModel>().ChosenSeeds)
            {
                seed.ColdTimeTimer.SetRemaining(0);
                this.GetModel<ILevelModel>().SunPoint.Value += 25;
            }
        }

        #endregion

        #region Game

        //
        EditorGUILayout.LabelField("背包", EditorStyles.boldLabel);
        if (GUILayout.Button("获得10000金币"))
        {
            this.GetModel<IGameModel>().GameData.InventoryData.Coins.Value += 10000;
        }

        //
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("获得卡牌"))
        {
            var _ = ItemCreator.CreateCardData(plantDef);
            this.GetModel<IGameModel>().GameData.InventoryData.AddCard(_);
        }

        GUILayout.FlexibleSpace();
        plantDef.Id = (PlantId)EditorGUILayout.EnumPopup("", plantDef.Id);

        GUILayout.FlexibleSpace();
        plantDef.Variant = (PlantVariant)EditorGUILayout.EnumPopup("", plantDef.Variant);
        EditorGUILayout.EndHorizontal();

        #endregion
    }


    public void SpawnZombie()
    {
        if (!EditorApplication.isPlaying)
        {
            "请在游戏运行时使用该面板".LogWarning();
            return;
        }

        if (selectedSpawnPosId == ZombieSpawnPosId.NotSet)
        {
            this.GetService<IZombieService>().SpawnZombie(selectedZombieId, spawnPosition, paras);
        }
        else
        {
            var levelData = this.GetModel<ILevelModel>().LevelData as LevelData;
            var spawnPos = levelData.GetZombieSpawnPos(selectedSpawnPosId);
            this.GetService<IZombieService>().SpawnZombie(selectedZombieId, spawnPos, paras);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return PVZRouge.Interface;
    }
}