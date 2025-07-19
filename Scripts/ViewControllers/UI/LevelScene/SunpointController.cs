using QFramework;
using TMPro;
using TPL.PVZR.Models;
using UnityEngine;

namespace TPL.PVZR.ViewControllers.Others.UI
{
    public class SunpointController : MonoBehaviour,IController
    {
        [SerializeField] private TextMeshProUGUI SunpointText;
        
        private ILevelModel _LevelModel;

        
        
        private void Awake()
        {
            _LevelModel = this.GetModel<ILevelModel>();
            
            _LevelModel.SunPoint.RegisterWithInitValue(val =>
            {
                SunpointText.text = val.ToString();
            }).UnRegisterWhenGameObjectDestroyed(this);
        }


        public IArchitecture GetArchitecture()
        {
            return PVZRouge.Interface;
        }
    }
}