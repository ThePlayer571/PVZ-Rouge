using System;
using DG.Tweening;
using QFramework;
using TPL.PVZR.Core;
using TPL.PVZR.Models;
using TPL.PVZR.UI;
using TPL.PVZR.ViewControllers.Managers;
using TPL.PVZR.ViewControllers.Others;
using UnityEngine;
using UnityEngine.UI;

namespace TPL.PVZR.Commands
{
    public class ChooseSeedCommand : AbstractCommand
    {
        public ChooseSeedCommand(SeedOptionController seed)
        {
            this.seed = seed;
        }

        private SeedOptionController seed;


        protected override void OnExecute()
        {
            // 异常处理
            var PhaseModel = this.GetModel<IPhaseModel>();
            if (PhaseModel.GamePhase != GamePhase.ChooseSeeds)
                throw new System.Exception($"在不正确的阶段执行ChooseSeedCommand：{PhaseModel.GamePhase}");
            if (seed.IsSelected == true) throw new Exception("尝试选择一个已经被选择的种子");

            // 数据操作
            ReferenceHelper.ChooseSeedPanel.chosenSeedOptions.Add(seed);
            
            // 动画
            GameObject movingView = null;
            ActionKit.Sequence()
                .Callback(() =>
                {
                    // View
                    movingView = GameObject.Instantiate(
                        seed.View.gameObject,
                        seed.transform.position,
                        Quaternion.identity,
                        seed.transform
                        
                    );
                    movingView.transform.SetParent(ReferenceHelper.ChooseSeedPanel.transform);
                    // Seed
                    seed.View.gameObject.SetActive(false);
                    seed.IsSelected = true;
                    seed.transform.SetParent(ReferenceHelper.ChooseSeedPanel.ChosenSeeds);
                })
                .DelayFrame(1)
                .Callback(() =>
                {
                    movingView.transform.DOMove(seed.transform.position, 0.2f).OnComplete(() =>
                    {
                        // View
                        GameObject.Destroy(movingView);
                        // Seed
                        seed.View.gameObject.SetActive(true);
                    });
                })
                .Start(GameManager.Instance);
        }
    }
}