using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace TPL.PVZR.UI
{
	// Generate Id:a62747f6-f6d5-4215-9058-c339cc916299
	public partial class UIGameStartPanel
	{
		public const string Name = "UIGameStartPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button BtnStart;
		[SerializeField]
		public TMPro.TMP_InputField SeedInputField;
		
		private UIGameStartPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			BtnStart = null;
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
