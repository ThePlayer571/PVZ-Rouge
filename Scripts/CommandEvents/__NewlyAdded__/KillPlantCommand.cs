// using System;
// using QFramework;
// using TPL.PVZR.Classes.LevelStuff;
// using TPL.PVZR.Models;
// using TPL.PVZR.Systems;
// using TPL.PVZR.ViewControllers.Entities.Plants;
// using UnityEngine;
//
// namespace TPL.PVZR.CommandEvents.__NewlyAdded__
// {
//     public class KillPlantCommand : AbstractCommand
//     {
//         public KillPlantCommand(Plant plant)
//         {
//             this._plant = plant;
//         }
//
//         private Plant _plant;
//
//         protected override void OnExecute()
//         {
//             var _LevelGridModel = this.GetModel<ILevelGridModel>();
//             var _PhaseModel = this.GetModel<IPhaseModel>();
//             var _HandSystem = this.GetSystem<IHandSystem>();
//             
//             // 异常处理
//             if (_PhaseModel.GamePhase != GamePhase.Gameplay)
//                 throw new Exception($"尝试调用SpawnPlantCommand，但GameState: {_PhaseModel.GamePhase}"); // 游戏阶段正确
//             //
//         }
//     }
// }