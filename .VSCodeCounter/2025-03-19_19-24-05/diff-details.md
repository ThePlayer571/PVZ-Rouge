# Diff Details

Date : 2025-03-19 19:24:05

Directory d:\\FileFolder\\Life\\Unity\\Projects\\PVZR\\PVZ-R\\Assets\\Scripts

Total : 42 files,  431 codes, 19 comments, 69 blanks, all 519 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Game/Classes/Data/ArmorData.cs](/Game/Classes/Data/ArmorData.cs) | C# | -11 | 0 | -1 | -12 |
| [Game/Classes/Data/AttackData.cs](/Game/Classes/Data/AttackData.cs) | C# | -11 | 0 | -1 | -12 |
| [Game/Classes/Data/CardDataSO.cs](/Game/Classes/Data/CardDataSO.cs) | C# | -9 | 0 | -1 | -10 |
| [Game/Classes/Data/LootData.cs](/Game/Classes/Data/LootData.cs) | C# | 51 | 2 | 8 | 61 |
| [Game/Classes/Data/SeedDataSO.cs](/Game/Classes/Data/SeedDataSO.cs) | C# | -15 | -2 | -1 | -18 |
| [Game/Classes/Data/ZombieSpawnData.cs](/Game/Classes/Data/ZombieSpawnData.cs) | C# | 46 | 0 | 5 | 51 |
| [Game/Classes/Enums.cs](/Game/Classes/Enums.cs) | C# | -26 | 0 | -4 | -30 |
| [Game/Classes/Framework/Commands.cs](/Game/Classes/Framework/Commands.cs) | C# | 6 | 0 | 2 | 8 |
| [Game/Classes/Framework/Enums.cs](/Game/Classes/Framework/Enums.cs) | C# | 26 | 0 | 4 | 30 |
| [Game/Classes/Framework/Events.cs](/Game/Classes/Framework/Events.cs) | C# | 3 | 0 | 2 | 5 |
| [Game/Classes/Level/Classes/Configs.cs](/Game/Classes/Level/Classes/Configs.cs) | C# | 53 | 2 | 8 | 63 |
| [Game/Classes/Level/Level.cs](/Game/Classes/Level/Level.cs) | C# | -33 | -5 | 1 | -37 |
| [Game/Classes/Level/LevelDaveHouse.cs](/Game/Classes/Level/LevelDaveHouse.cs) | C# | 13 | -1 | 2 | 14 |
| [Game/Classes/ScriptableObject/ArmorData.cs](/Game/Classes/ScriptableObject/ArmorData.cs) | C# | 11 | 0 | 1 | 12 |
| [Game/Classes/ScriptableObject/AttackData.cs](/Game/Classes/ScriptableObject/AttackData.cs) | C# | 11 | 0 | 1 | 12 |
| [Game/Classes/ScriptableObject/CardDataSO.cs](/Game/Classes/ScriptableObject/CardDataSO.cs) | C# | 35 | 0 | 4 | 39 |
| [Game/Classes/ScriptableObject/SeedDataSO.cs](/Game/Classes/ScriptableObject/SeedDataSO.cs) | C# | 19 | 2 | 2 | 23 |
| [Game/Classes/ZombieSpawnData.cs](/Game/Classes/ZombieSpawnData.cs) | C# | -36 | 0 | -4 | -40 |
| [Game/Manager/GameManager.cs](/Game/Manager/GameManager.cs) | C# | 4 | 0 | 1 | 5 |
| [Game/Manager/GameTestManager.cs](/Game/Manager/GameTestManager.cs) | C# | -4 | 0 | 0 | -4 |
| [Game/Models/LevelModel.cs](/Game/Models/LevelModel.cs) | C# | 12 | 5 | 2 | 19 |
| [Game/PVZRouge.cs](/Game/PVZRouge.cs) | C# | 0 | 3 | 0 | 3 |
| [Game/System/EntityCreateSystem.cs](/Game/System/EntityCreateSystem.cs) | C# | -129 | -20 | -32 | -181 |
| [Game/System/Game/LootCreateSystem.cs](/Game/System/Game/LootCreateSystem.cs) | C# | 23 | 1 | 3 | 27 |
| [Game/System/InputSystem.cs](/Game/System/InputSystem.cs) | C# | -3 | 0 | 1 | -2 |
| [Game/System/Level/EntitySystem.cs](/Game/System/Level/EntitySystem.cs) | C# | 100 | 15 | 26 | 141 |
| [Game/System/Level/EntitySystem\_Create.cs](/Game/System/Level/EntitySystem_Create.cs) | C# | 60 | 5 | 8 | 73 |
| [Game/System/Level/LevelSystem.cs](/Game/System/Level/LevelSystem.cs) | C# | -67 | -14 | -1 | -82 |
| [Game/System/Level/LevelSystem\_SetUpState.cs](/Game/System/Level/LevelSystem_SetUpState.cs) | C# | 99 | 16 | 3 | 118 |
| [Game/System/Level/WaveSystem.cs](/Game/System/Level/WaveSystem.cs) | C# | 9 | 1 | 2 | 12 |
| [Game/System/Level/ZombieSpawnSystem.cs](/Game/System/Level/ZombieSpawnSystem.cs) | C# | 7 | 3 | 0 | 10 |
| [Game/ViewControllers/EndLevelLootChest.Designer.cs](/Game/ViewControllers/EndLevelLootChest.Designer.cs) | C# | 8 | 1 | 3 | 12 |
| [Game/ViewControllers/EndLevelLootChest.cs](/Game/ViewControllers/EndLevelLootChest.cs) | C# | 28 | 0 | 5 | 33 |
| [Game/ViewControllers/Entities/Plants/Base/Plant.cs](/Game/ViewControllers/Entities/Plants/Base/Plant.cs) | C# | 2 | 0 | 0 | 2 |
| [Game/ViewControllers/Entities/Plants/CherryBoom.cs](/Game/ViewControllers/Entities/Plants/CherryBoom.cs) | C# | 3 | 0 | -1 | 2 |
| [Game/ViewControllers/Entities/Zombies/Zombie.cs](/Game/ViewControllers/Entities/Zombies/Zombie.cs) | C# | 1 | 0 | 1 | 2 |
| [Game/ViewControllers/InteractableIndicator.Designer.cs](/Game/ViewControllers/InteractableIndicator.Designer.cs) | C# | 8 | 1 | 3 | 12 |
| [Game/ViewControllers/InteractableIndicator.cs](/Game/ViewControllers/InteractableIndicator.cs) | C# | 86 | 1 | 18 | 105 |
| [UI/UIGamePanel.Designer.cs](/UI/UIGamePanel.Designer.cs) | C# | -44 | -1 | -8 | -53 |
| [UI/UIGamePanel.cs](/UI/UIGamePanel.cs) | C# | -57 | -3 | -41 | -101 |
| [UI/UILevelPanel.Designer.cs](/UI/UILevelPanel.Designer.cs) | C# | 62 | 1 | 8 | 71 |
| [UI/UILevelPanel.cs](/UI/UILevelPanel.cs) | C# | 90 | 6 | 40 | 136 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details