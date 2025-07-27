using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR.ViewControllers.UI
{
	// Generate Id:d8e51a2c-027e-4a4a-806f-8e3dbdcb0813
	public partial class UIMazeMapPanel
	{
		public const string Name = "UIMazeMapPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI CoinText;
		[SerializeField]
		public UnityEngine.UI.Toggle AwardToggle;
		[SerializeField]
		public UnityEngine.UI.Toggle StoreToggle;
		[SerializeField]
		public UnityEngine.UI.Toggle InventoryToggle;
		[SerializeField]
		public UnityEngine.UI.Toggle MazeMapToggle;
		
		private UIMazeMapPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			CoinText = null;
			AwardToggle = null;
			StoreToggle = null;
			InventoryToggle = null;
			MazeMapToggle = null;
			
			mData = null;
		}
		
		public UIMazeMapPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIMazeMapPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIMazeMapPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
