using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class UIToggleHandler : MonoBehaviour
    {
        public GameObject Essentials_UI;
        public GameObject Essentials_UI_Mobile;
        void Awake()
        {
            if (Essentials_UI == null || Essentials_UI_Mobile == null)
            {
                Debug.LogError("[RPGAIO] Cannot find gameObject [Essentials_UI] or [Essentials_UI_Mobile] please assign these on Essentials > Handlers > UIToggleHandler");
                return;
            }

#if (UNITY_IOS || UNITY_ANDROID)
            Essentials_UI.SetActive(false);
            Essentials_UI_Mobile.SetActive(true);
#else
            Essentials_UI.SetActive(true);
            Essentials_UI_Mobile.SetActive(false);
#endif
            //Init UI once chosen
            GetObject.UIHandler.Init(); 
        }
    }
}