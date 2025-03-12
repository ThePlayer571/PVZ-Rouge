using QFramework;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace TPL.PVZR
{
    public interface IInventorySystem
    {
        
    }
    public class InventorySystem: AbstractSystem
    {
        protected override void OnInit()
        {
            cards = new List<CardData>();
        }

        public List<CardData> cards;
    }
}