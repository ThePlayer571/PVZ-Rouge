using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Models;
using TPL.PVZR.Services;
using TPL.PVZR.Tools;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others;
using TPL.PVZR.ViewControllers.Others.UI;
using TPL.PVZR.ViewControllers.UI;
using UnityEngine;

namespace TPL.PVZR.CommandEvents.Level_ChooseSeeds
{
    public class UnchooseSeedCommand : AbstractCommand
    {
        public UnchooseSeedCommand(SeedOptionController seed)
        {
            this.seed = seed;
        }

        private SeedOptionController seed;


        protected override void OnExecute()
        {
            var PhaseModel = this.GetModel<IPhaseModel>();
            var ChooseSeedPanel =  UIKit.GetPanel<UIChooseSeedPanel>();
            
            // 异常处理
            if (PhaseModel.GamePhase != GamePhase.ChooseSeeds)
                throw new System.Exception($"在不正确的阶段执行ChooseSeedCommand：{PhaseModel.GamePhase}");
            if (seed.IsSelected == false) throw new Exception("尝试取消选择一个未被选择的种子");


            // 数据操作
            ChooseSeedPanel.chosenSeedOptions.Remove(seed);

            // 动画
            GameObject movingView = null;
            ActionKit.Sequence()
                .Callback(() =>
                {
                    // View
                    movingView = GameObject.Instantiate(
                        seed.cardView.gameObject,
                        seed.transform.position,
                        Quaternion.identity,
                        seed.transform
                    );
                    movingView.transform.SetParent(ChooseSeedPanel.transform);
                    // Seed
                    seed.cardView.gameObject.SetActive(false);
                    seed.IsSelected = false;
                    seed.transform.SetParent(ChooseSeedPanel.InventorySeeds);
                })
                .DelayFrame(1)
                .Callback(() =>
                {
                    movingView.transform.DOMove(seed.transform.position, 0.2f).OnComplete(() =>
                    {
                        // View
                        GameObject.Destroy(movingView);
                        // Seed
                        seed.cardView.gameObject.SetActive(true);
                    });
                })
                .Start(GameManager.Instance);
        }
    }
}