using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR.ViewControllers.UI
{
	// Generate Id:8ff38093-ec69-4b54-a681-5c3abdef33d7
	public partial class UIGameStartPanel
	{
		public const string Name = "UIGameStartPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button ContinueBtn;
		[SerializeField]
		public UnityEngine.UI.Button StartBtn;
		[SerializeField]
		public TMPro.TMP_InputField SeedInputField;
		
		private UIGameStartPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			ContinueBtn = null;
			StartBtn = null;
			SeedInputField = null;
			
			mData = null;
		}
		
		public UIGameStartPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameStartPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameStartPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
