// Generate Id:2ac8bfc7-b54e-4246-a3c9-b2baae9dc94a

using QFramework;
using UnityEngine;

namespace TPL.PVZR
{
	public partial class Card
	{
		public TMPro.TextMeshProUGUI SunText;
		
		public UnityEngine.UI.Image Plant;

		public static CardDataSO GetCardData(PlantIdentifier plantIdentifier)
		{
			var _ResLoader = ResLoader.Allocate();
			switch (plantIdentifier)
			{
				case PlantIdentifier.PeaShooter: return _ResLoader.LoadSync<CardDataSO>("CardData_PeaShooter");
				case PlantIdentifier.Sunflower: return _ResLoader.LoadSync<CardDataSO>("CardData_Sunflower");
				case PlantIdentifier.Wallnut: return _ResLoader.LoadSync<CardDataSO>("CardData_Wallnut");
				case PlantIdentifier.Flowerpot: return _ResLoader.LoadSync<CardDataSO>("CardData_Flowerpot");
				case PlantIdentifier.SnowPea: return _ResLoader.LoadSync<CardDataSO>("CardData_SnowPea");
				case PlantIdentifier.CherryBoom: return _ResLoader.LoadSync<CardDataSO>("CardData_CherryBoom");
				case PlantIdentifier.PotatoMine: return _ResLoader.LoadSync<CardDataSO>("CardData_PotatoMine");
				default: return _ResLoader.LoadSync<CardDataSO>("CardData_PeaShooter");
			}
		}
	}
}
