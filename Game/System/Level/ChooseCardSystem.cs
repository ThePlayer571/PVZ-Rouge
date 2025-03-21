using QFramework;
using UnityEngine;
using System;
using System.Collections.Generic;


namespace TPL.PVZR
{
    public interface IChooseCardSystem : ISystem, IInLevelSystem
    {
        public bool canAddCard { get; }
        public List<Card> chosenCards { get; }

        // 操作
        public GameObject CreateCard(CardDataSO cardDataSO);
        public GameObject CreateSeed(CardDataSO cardDataSO);
        public void AddCard(Card card);
        public void RemoveCard(Card card);


    }

    public class ChooseCardSystem : AbstractSystem, IChooseCardSystem
    {
        ILevelModel _LevelModel;
        private List<Card> _chosenCards;

        protected override void OnInit()
        {
            _LevelModel = this.GetModel<ILevelModel>();
        }

        // 数据
        public List<Card> chosenCards { get;private set; }
        // 操作

        public bool canAddCard => chosenCards.Count < _LevelModel.maxCardCount;

        public void AddCard(Card card)
        {
            if (!canAddCard) return;
            chosenCards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            foreach (var eachCard in chosenCards)
            {
                if (ReferenceEquals(eachCard, card))
                {
                    chosenCards.Remove(eachCard);
                    break;
                }
            }
        }
        

        public GameObject CreateCard(CardDataSO cardDataSO)
        {
            var _ResLoader = ResLoader.Allocate();
            var go = _ResLoader.LoadSync<GameObject>("Card").Instantiate();
            go.GetComponent<Card>().Init(cardDataSO);
            return go;
        }

        public GameObject CreateSeed(CardDataSO cardDataSO)
        {
            var _ResLoader = ResLoader.Allocate();
            var go = _ResLoader.LoadSync<GameObject>("Seed").Instantiate();
            go.GetComponent<Seed>().Init(cardDataSO);
            return go;
        }

        // 闭环
        public void OnChoosingCard()
        {
            chosenCards = new List<Card>();
        }

        public void OnGameplay()
        {
            _LevelModel.chosenCards = chosenCards;
        }

        public void OnExiting()
        {
            chosenCards = null;
        }
    }
}