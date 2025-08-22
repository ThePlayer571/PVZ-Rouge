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
            this.GetService<IZombieService>().SpawnZombie(selectedZombieId, spawnPosition);
        }
        else
        {
            var levelData = this.GetModel<ILevelModel>().LevelData as LevelData;
            var spawnPos = levelData.GetZombieSpawnPos(selectedSpawnPosId);
            this.GetService<IZombieService>().SpawnZombie(selectedZombieId, spawnPos);
        }
    }

    public IArchitecture GetArchitecture()
    {
        return PVZRouge.Interface;
    }
}