# Diff Details

Date : 2025-07-24 23:53:59

Directory d:\\FileFolder\\Life\\Unity\\Projects\\PVZR\\PVZ_Rouge\\Assets\\Scripts

Total : 85 files,  959 codes, 71 comments, 198 blanks, all 1228 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Classes/ConfigLists/PlantConfigList.cs](/Classes/ConfigLists/PlantConfigList.cs) | C# | 1 | 0 | 0 | 1 |
| [Classes/DataClasses/GlobalEntityData.cs](/Classes/DataClasses/GlobalEntityData.cs) | C# | 7 | 0 | 0 | 7 |
| [Classes/DataClasses/Level/LevelData.cs](/Classes/DataClasses/Level/LevelData.cs) | C# | -3 | 0 | 1 | -2 |
| [Classes/DataClasses\_InLevel/Attack/AttackData.cs](/Classes/DataClasses_InLevel/Attack/AttackData.cs) | C# | 10 | 1 | 2 | 13 |
| [Classes/DataClasses\_InLevel/Attack/AttackDefinition.cs](/Classes/DataClasses_InLevel/Attack/AttackDefinition.cs) | C# | 1 | 0 | 0 | 1 |
| [Classes/DataClasses\_InLevel/Attack/AttackId.cs](/Classes/DataClasses_InLevel/Attack/AttackId.cs) | C# | 13 | 0 | 1 | 14 |
| [Classes/DataClasses\_InLevel/Cell.cs](/Classes/DataClasses_InLevel/Cell.cs) | C# | -89 | -6 | -20 | -115 |
| [Classes/DataClasses\_InLevel/Cell/AllowedPlantingLocation.cs](/Classes/DataClasses_InLevel/Cell/AllowedPlantingLocation.cs) | C# | 58 | 7 | 11 | 76 |
| [Classes/DataClasses\_InLevel/Cell/Cell.cs](/Classes/DataClasses_InLevel/Cell/Cell.cs) | C# | 43 | 4 | 9 | 56 |
| [Classes/DataClasses\_InLevel/Cell/CellPlantData.cs](/Classes/DataClasses_InLevel/Cell/CellPlantData.cs) | C# | 109 | 0 | 15 | 124 |
| [Classes/DataClasses\_InLevel/Cell/PlacementSlot.cs](/Classes/DataClasses_InLevel/Cell/PlacementSlot.cs) | C# | 10 | 3 | 0 | 13 |
| [Classes/DataClasses\_InLevel/Cell/TileState.cs](/Classes/DataClasses_InLevel/Cell/TileState.cs) | C# | 13 | 0 | 0 | 13 |
| [Classes/DataClasses\_InLevel/Effect/EffectGroup.cs](/Classes/DataClasses_InLevel/Effect/EffectGroup.cs) | C# | 16 | 0 | 3 | 19 |
| [Classes/DataClasses\_InLevel/Effect/EffectId.cs](/Classes/DataClasses_InLevel/Effect/EffectId.cs) | C# | 1 | 0 | 0 | 1 |
| [Classes/InfoClasses/PlantDef.cs](/Classes/InfoClasses/PlantDef.cs) | C# | 4 | 0 | 1 | 5 |
| [Classes/InfoClasses/PlantId.cs](/Classes/InfoClasses/PlantId.cs) | C# | 7 | 0 | 0 | 7 |
| [Classes/InfoClasses/ProjectileId.cs](/Classes/InfoClasses/ProjectileId.cs) | C# | 7 | 0 | 0 | 7 |
| [Classes/LootPool/LootPoolInfo.cs](/Classes/LootPool/LootPoolInfo.cs) | C# | 37 | 0 | 8 | 45 |
| [Classes/MazeMap/Controllers/DaveLawnController.cs](/Classes/MazeMap/Controllers/DaveLawnController.cs) | C# | 3 | 0 | 2 | 5 |
| [Classes/MazeMap/MazeMapData.cs](/Classes/MazeMap/MazeMapData.cs) | C# | 5 | 1 | 4 | 10 |
| [Classes/MazeMap/MazeMapDefinition.cs](/Classes/MazeMap/MazeMapDefinition.cs) | C# | 3 | 1 | 1 | 5 |
| [Classes/ZombieAI/PathFinding/ZombieAIUnit.cs](/Classes/ZombieAI/PathFinding/ZombieAIUnit.cs) | C# | 4 | 0 | 1 | 5 |
| [CommandEvents/Level\_Hand/UseShovelCommand.cs](/CommandEvents/Level_Hand/UseShovelCommand.cs) | C# | 0 | -1 | 0 | -1 |
| [CommandEvents/Level\_Zombie/OnZombieDeathCommand.cs](/CommandEvents/Level_Zombie/OnZombieDeathCommand.cs) | C# | 1 | 0 | 1 | 2 |
| [Docs/待办事项.md](/Docs/%E5%BE%85%E5%8A%9E%E4%BA%8B%E9%A1%B9.md) | Markdown | -3 | 0 | 0 | -3 |
| [Docs/抽卡理论.md](/Docs/%E6%8A%BD%E5%8D%A1%E7%90%86%E8%AE%BA.md) | Markdown | 23 | 0 | 18 | 41 |
| [Docs/目前相对于原版做的创新.md](/Docs/%E7%9B%AE%E5%89%8D%E7%9B%B8%E5%AF%B9%E4%BA%8E%E5%8E%9F%E7%89%88%E5%81%9A%E7%9A%84%E5%88%9B%E6%96%B0.md) | Markdown | 1 | 0 | 0 | 1 |
| [Helpers/ClassCreator/LootCreator.cs](/Helpers/ClassCreator/LootCreator.cs) | C# | 42 | 28 | 7 | 77 |
| [Helpers/ClassCreator/TradeCreator.cs](/Helpers/ClassCreator/TradeCreator.cs) | C# | 24 | 0 | 7 | 31 |
| [Helpers/ConfigReader/AttackConfigReader.cs](/Helpers/ConfigReader/AttackConfigReader.cs) | C# | 39 | 0 | 10 | 49 |
| [Helpers/ConfigReader/EconomyConfigReader.cs](/Helpers/ConfigReader/EconomyConfigReader.cs) | C# | 129 | 10 | 30 | 169 |
| [Helpers/ConfigReader/GameConfigReader.cs](/Helpers/ConfigReader/GameConfigReader.cs) | C# | 75 | 3 | 16 | 94 |
| [Helpers/ConfigReader/LootPoolConfigReader.cs](/Helpers/ConfigReader/LootPoolConfigReader.cs) | C# | 92 | 8 | 15 | 115 |
| [Helpers/ConfigReader/PlantBookConfigReader.cs](/Helpers/ConfigReader/PlantBookConfigReader.cs) | C# | 41 | 0 | 10 | 51 |
| [Helpers/ConfigReader/PlantConfigReader.cs](/Helpers/ConfigReader/PlantConfigReader.cs) | C# | 111 | 1 | 22 | 134 |
| [Helpers/ConfigReader/RecipeConfigReader.cs](/Helpers/ConfigReader/RecipeConfigReader.cs) | C# | 88 | 10 | 15 | 113 |
| [Helpers/ConfigReader/ZombieArmorConfigReader.cs](/Helpers/ConfigReader/ZombieArmorConfigReader.cs) | C# | 40 | 1 | 10 | 51 |
| [Helpers/DataReader/AttackConfigReader.cs](/Helpers/DataReader/AttackConfigReader.cs) | C# | -38 | 0 | -9 | -47 |
| [Helpers/DataReader/GameConfigReader.cs](/Helpers/DataReader/GameConfigReader.cs) | C# | -74 | -3 | -15 | -92 |
| [Helpers/DataReader/PlantBookConfigReader.cs](/Helpers/DataReader/PlantBookConfigReader.cs) | C# | -40 | 0 | -9 | -49 |
| [Helpers/DataReader/PlantConfigReader.cs](/Helpers/DataReader/PlantConfigReader.cs) | C# | -79 | -1 | -16 | -96 |
| [Helpers/DataReader/TradeConfigReader.cs](/Helpers/DataReader/TradeConfigReader.cs) | C# | -179 | -9 | -36 | -224 |
| [Helpers/DataReader/ZombieArmorConfigReader.cs](/Helpers/DataReader/ZombieArmorConfigReader.cs) | C# | -39 | -1 | -9 | -49 |
| [Helpers/GameObjectFactory/EntityFactory.cs](/Helpers/GameObjectFactory/EntityFactory.cs) | C# | 4 | 1 | 0 | 5 |
| [Helpers/GameObjectFactory/ItemViewFactory.cs](/Helpers/GameObjectFactory/ItemViewFactory.cs) | C# | 1 | 0 | 2 | 3 |
| [Helpers/GameObjectFactory/ShitFactory.cs](/Helpers/GameObjectFactory/ShitFactory.cs) | C# | 1 | 0 | 2 | 3 |
| [Helpers/Methods/LevelMatrixHelper.cs](/Helpers/Methods/LevelMatrixHelper.cs) | C# | 1 | 0 | 1 | 2 |
| [Models/LevelGridModel.cs](/Models/LevelGridModel.cs) | C# | -16 | 1 | 1 | -14 |
| [Systems/Level\_Data/LevelSystem.cs](/Systems/Level_Data/LevelSystem.cs) | C# | 0 | 1 | 0 | 1 |
| [Systems/MainGameSystem.cs](/Systems/MainGameSystem.cs) | C# | 1 | 0 | 1 | 2 |
| [Systems/MazeMap/CoinStoreSystem.cs](/Systems/MazeMap/CoinStoreSystem.cs) | C# | 16 | 0 | 2 | 18 |
| [Tools/CollisionDetector.cs](/Tools/CollisionDetector.cs) | C# | 4 | 2 | 5 | 11 |
| [Tools/Extensions.cs](/Tools/Extensions.cs) | C# | 16 | 8 | 1 | 25 |
| [ViewControllers/Entities/Plants/Base/Plant.cs](/ViewControllers/Entities/Plants/Base/Plant.cs) | C# | 10 | 0 | 1 | 11 |
| [ViewControllers/Entities/Plants/Dave/BonkChoy.cs](/ViewControllers/Entities/Plants/Dave/BonkChoy.cs) | C# | 41 | 0 | 7 | 48 |
| [ViewControllers/Entities/Plants/Dave/Tallnut.cs](/ViewControllers/Entities/Plants/Dave/Tallnut.cs) | C# | 15 | 0 | 2 | 17 |
| [ViewControllers/Entities/Plants/General/Flowerpot.cs](/ViewControllers/Entities/Plants/General/Flowerpot.cs) | C# | 3 | 0 | 0 | 3 |
| [ViewControllers/Entities/Plants/General/Jalapeno.cs](/ViewControllers/Entities/Plants/General/Jalapeno.cs) | C# | 38 | 0 | 8 | 46 |
| [ViewControllers/Entities/Plants/General/Pumpkin.cs](/ViewControllers/Entities/Plants/General/Pumpkin.cs) | C# | 23 | 0 | 3 | 26 |
| [ViewControllers/Entities/Plants/General/Squash.cs](/ViewControllers/Entities/Plants/General/Squash.cs) | C# | 64 | 0 | 11 | 75 |
| [ViewControllers/Entities/Plants/PeaFamily/Threepeater.cs](/ViewControllers/Entities/Plants/PeaFamily/Threepeater.cs) | C# | 1 | 0 | 0 | 1 |
| [ViewControllers/Entities/Plants/PeaFamily/Torchwood.cs](/ViewControllers/Entities/Plants/PeaFamily/Torchwood.cs) | C# | 31 | 0 | 6 | 37 |
| [ViewControllers/Entities/Plants/PultFamily/CabbagePult.cs](/ViewControllers/Entities/Plants/PultFamily/CabbagePult.cs) | C# | 41 | 0 | 8 | 49 |
| [ViewControllers/Entities/Projectiles/Base/ICanBeIgnited.cs](/ViewControllers/Entities/Projectiles/Base/ICanBeIgnited.cs) | C# | 12 | 0 | 1 | 13 |
| [ViewControllers/Entities/Projectiles/Base/IPeaLikeInit.cs](/ViewControllers/Entities/Projectiles/Base/IPeaLikeInit.cs) | C# | 9 | 0 | 1 | 10 |
| [ViewControllers/Entities/Projectiles/Base/IProjectile.cs](/ViewControllers/Entities/Projectiles/Base/IProjectile.cs) | C# | 7 | 0 | 1 | 8 |
| [ViewControllers/Entities/Projectiles/Base/Projectile.cs](/ViewControllers/Entities/Projectiles/Base/Projectile.cs) | C# | 14 | 0 | 4 | 18 |
| [ViewControllers/Entities/Projectiles/Cabbage.cs](/ViewControllers/Entities/Projectiles/Cabbage.cs) | C# | 31 | 0 | 6 | 37 |
| [ViewControllers/Entities/Projectiles/FirePea.cs](/ViewControllers/Entities/Projectiles/FirePea.cs) | C# | 40 | 0 | 6 | 46 |
| [ViewControllers/Entities/Projectiles/FrozenPea.cs](/ViewControllers/Entities/Projectiles/FrozenPea.cs) | C# | 2 | 0 | 1 | 3 |
| [ViewControllers/Entities/Projectiles/IPeaLikeInit.cs](/ViewControllers/Entities/Projectiles/IPeaLikeInit.cs) | C# | -9 | 0 | -1 | -10 |
| [ViewControllers/Entities/Projectiles/IProjectile.cs](/ViewControllers/Entities/Projectiles/IProjectile.cs) | C# | -7 | 0 | -1 | -8 |
| [ViewControllers/Entities/Projectiles/MungBean.cs](/ViewControllers/Entities/Projectiles/MungBean.cs) | C# | 2 | 0 | 1 | 3 |
| [ViewControllers/Entities/Projectiles/Pea.cs](/ViewControllers/Entities/Projectiles/Pea.cs) | C# | 24 | 0 | 3 | 27 |
| [ViewControllers/Entities/Projectiles/Projectile.cs](/ViewControllers/Entities/Projectiles/Projectile.cs) | C# | -14 | 0 | -2 | -16 |
| [ViewControllers/Entities/Projectiles/SnipePea.cs](/ViewControllers/Entities/Projectiles/SnipePea.cs) | C# | 2 | 0 | 1 | 3 |
| [ViewControllers/Entities/Projectiles/Spike.cs](/ViewControllers/Entities/Projectiles/Spike.cs) | C# | 2 | 0 | 1 | 3 |
| [ViewControllers/Entities/Projectiles/Star.cs](/ViewControllers/Entities/Projectiles/Star.cs) | C# | 4 | 0 | 1 | 5 |
| [ViewControllers/Entities/Zombies/Base/Zombie.cs](/ViewControllers/Entities/Zombies/Base/Zombie.cs) | C# | 10 | 1 | 6 | 17 |
| [ViewControllers/Entities/Zombies/Instances/NewspaperZombie.cs](/ViewControllers/Entities/Zombies/Instances/NewspaperZombie.cs) | C# | 1 | 0 | 0 | 1 |
| [ViewControllers/Entities/Zombies/States/DeadState.cs](/ViewControllers/Entities/Zombies/States/DeadState.cs) | C# | 11 | 0 | 1 | 12 |
| [ViewControllers/Entities/Zombies/States/ZombieState.cs](/ViewControllers/Entities/Zombies/States/ZombieState.cs) | C# | 1 | 0 | 0 | 1 |
| [ViewControllers/Managers/TestDataManager.cs](/ViewControllers/Managers/TestDataManager.cs) | C# | 0 | 0 | -1 | -1 |
| [ViewControllers/Others/LevelScene/FollowingImage.cs](/ViewControllers/Others/LevelScene/FollowingImage.cs) | C# | 6 | 0 | 3 | 9 |
| [ViewControllers/UI/LevelScene/LevelStateBarPanelController.cs](/ViewControllers/UI/LevelScene/LevelStateBarPanelController.cs) | C# | 2 | 0 | -1 | 1 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details