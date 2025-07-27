using System;
using QFramework;
using TPL.PVZR.Classes.DataClasses.Game;
using TPL.PVZR.CommandEvents._NotClassified_;
using TPL.PVZR.Helpers.New;
using TPL.PVZR.Tools.Save;

namespace TPL.PVZR.ViewControllers.UI
{
    public class UIGameStartPanelData : UIPanelData
    {
    }

    public partial class UIGameStartPanel : UIPanel, IController
    {
        private ulong? _inputSeed = null;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as UIGameStartPanelData ?? new UIGameStartPanelData();
            // please add init code here

            if (!SaveHelper.Exists(SaveHelper.GAME_DATA_FILE_NAME))
            {
                ContinueBtn.interactable = false;
            }

            ContinueBtn.onClick.AddListener(() =>
            {
                var savedGameData = new GameData(SaveHelper.Load<GameSaveData>(SaveHelper.GAME_DATA_FILE_NAME));
                this.SendCommand<StartGameCommand>(new StartGameCommand(savedGameData, false));
            });

            StartBtn.onClick.AddListener(() =>
            {
                this.SendCommand<StartNewGameCommand>(new StartNewGameCommand(_inputSeed));
            });

            SeedInputField.onValueChanged.AddListener(str =>
            {
                string filtered = "";
                foreach (char c in str)
                {
                    if (Uri.IsHexDigit(c)) filtered += c;
                    if (filtered.Length >= 8) break;
                }

                filtered = filtered.ToUpper();

                if (filtered != str)
                    SeedInputField.text = filtered;
            });

            SeedInputField.onEndEdit.AddListener(str =>
            {
                str.LogInfo();
                if (str.Length == 0) return;
                str = str.PadLeft(8, '0');
                if (SeedHelper.ValidateInput(str))
                {
                    _inputSeed = SeedHelper.ParseFrom(str);
                    SeedInputField.text = str;
                }
                else
                {
                    throw new Exception("怎么会触发呢？已经确保是正确的输入了");
                }
            });
        }

        protected override void OnClose()
        {
            ContinueBtn.onClick.RemoveAllListeners();
            StartBtn.onClick.RemoveAllListeners();
            SeedInputField.onValueChanged.RemoveAllListeners();
            SeedInputField.onEndEdit.RemoveAllListeners();
        }

        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}