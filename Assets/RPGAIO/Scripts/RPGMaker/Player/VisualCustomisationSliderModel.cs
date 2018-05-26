using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker
{
    public class VisualCustomisationSliderModel : VisualCustomiser
    {
        public Text TitleText;
        public Slider Slider;

        public void Init(VisualCustomisation customisation)
        {
            VisualCustomisation = customisation;
            TitleText.text = VisualCustomisation.Identifier;

            if(VisualCustomisation.CustomisationType == VisualCustomisationType.BlendShape)
            {
                //Cache values
                cached_target = FindTargetObject(VisualCustomisation.TargetedGameObjectName);
                cached_target_skinnedMeshRender = cached_target.GetComponent<SkinnedMeshRenderer>();
                cached_int_ref = cached_target_skinnedMeshRender.sharedMesh.GetBlendShapeIndex(VisualCustomisation.StringRef);
                
                //Set Initial Value
                var startingValue = VisualCustomisation.StartingValueFloat;

                //Setup slider 
                Slider.minValue = VisualCustomisation.MinFloatValue;
                Slider.maxValue = VisualCustomisation.MaxFloatValue;
                Slider.value = startingValue;

            }
            else if(VisualCustomisation.CustomisationType == VisualCustomisationType.Scale)
            {
                //Cache values
                cached_target = FindTargetObject(VisualCustomisation.TargetedGameObjectName);

                //Set Initial Value
                var startingValue = VisualCustomisation.StartingValueFloat;

                //Setup slider 
                Slider.minValue = VisualCustomisation.MinFloatValue;
                Slider.maxValue = VisualCustomisation.MaxFloatValue;
                Slider.value = startingValue;

            }

            //For all
            Slider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
            OnSliderValueChange();
        }

        private void OnSliderValueChange()
        {
            SetFloatOption(Slider.value);
        }
    }
}