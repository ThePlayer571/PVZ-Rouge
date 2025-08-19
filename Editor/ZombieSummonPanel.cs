using QFramework;
using TPL.PVZR;
using TPL.PVZR.Classes.DataClasses.Level;
using UnityEditor;
using UnityEngine;
using TPL.PVZR.Classes.InfoClasses;
using TPL.PVZR.Models;
using TPL.PVZR.Services;

public class ZombieSummonPanel : EditorWindow, ICanGetModel, ICanGetService
{
    private ZombieId selectedZombieId = ZombieId.NormalZombie;
    private Vector2 spawnPosition = new Vector2(22, 9);
    private ZombieSpawnPosId selectedSpawnPosId = ZombieSpawnPosId.NotSet;

    [MenuItem("PVZRouge/Zombie Summon Panel")]
    public static void ShowWindow()
    {
        GetWindow<ZombieSummonPanel>("Zombie Summon Panel");
    }

    private void OnGUI()
    {
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