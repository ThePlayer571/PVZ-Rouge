using System.Collections.Generic;
using System.Linq;
using QFramework;
using TPL.PVZR.Architecture.Events.GamePhase;
using TPL.PVZR.Architecture.Models;
using TPL.PVZR.Architecture.Systems.PhaseSystems;
using TPL.PVZR.Gameplay.ViewControllers.UI;

namespace TPL.PVZR.Architecture.Systems.InLevel
{
    /// <summary>
    /// 选择卡牌时用到的系统
    /// </summary>
    public interface IChooseCardSystem : ISystem
    {
        public bool canAddCard { get; }
        public List<UICard> chosenCards { get; }

        // 操作
        public void AddCard(UICard uiCard);
        public void RemoveCard(UICard uiCard);


    }

    public class ChooseCardSystem : AbstractSystem, IChooseCardSystem
    {
        #region IChooseCardSystem

        
        public bool canAddCard => chosenCards.Count < _LevelModel.maxCardCount;
        public List<UICard> chosenCards { get;private set; }
        // 操作

        public void AddCard(UICard uiCard)
        {
            if (!canAddCard) return;
            chosenCards.Add(uiCard);
        }

        public void RemoveCard(UICard uiCard)
        {
            foreach (var eachCard in chosenCards)
            {
                if (ReferenceEquals(eachCard, uiCard))
                {
                    chosenCards.Remove(eachCard);
                    break;
                }
            }
        }

        #endregion
        
        private ILevelModel _LevelModel;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            // 
            RegisterEvents();
        }
        
        private void RegisterEvents()
        {
            this.RegisterEvent<OnEnterPhaseEvent>(e =>
            {
                if (e.changeToPhase is GamePhaseSystem.GamePhase.LevelInitialization)
                {
                    chosenCards = new List<UICard>();
                }
            });
            this.RegisterEvent<OnLeavePhaseEvent>(e =>
            {
                if (e.leaveFromPhase is GamePhaseSystem.GamePhase.ChooseCards)
                {
                    _LevelModel.SetChosenCards(chosenCards.Select(UICard => UICard.card).ToList());
                    chosenCards = null;
                }
            });
        }
    }
}