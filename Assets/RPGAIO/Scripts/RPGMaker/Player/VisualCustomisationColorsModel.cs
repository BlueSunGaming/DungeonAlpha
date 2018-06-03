using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker
{
    public class VisualCustomisationColorsModel : VisualCustomiser
    {
        public Text TitleText;
        public GameObject ColorContainer;
        public GameObject ColorSelectPrefab;
        
        public void Init(VisualCustomisation customisation)
        {
            VisualCustomisation = customisation;
            TitleText.text = VisualCustomisation.Identifier;

            if (VisualCustomisation.CustomisationType == VisualCustomisationType.MaterialColor)
            {
                //Spawn Objects
                ColorContainer.transform.DestroyChildren();
                foreach (var colorOption in VisualCustomisation.ColorOptions)
                {
                    var go = Instantiate(ColorSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(ColorContainer.transform, false);
                    var colorSelectModel = go.GetComponent<VisualColorSelectModel>();
                    colorSelectModel.Init(this, colorOption);
                    
                }

                //Cache values
                var targetGo = FindTargetObject(VisualCustomisation.TargetedGameObjectName);
                cached_target_renderer = targetGo.GetComponent<Renderer>();

                //Set Initial Value and initial colors
                var startingValue = VisualCustomisation.ColorOptions[VisualCustomisation.StartingValueInt];
                SetColorOption(startingValue);
            }
        }
    }
}