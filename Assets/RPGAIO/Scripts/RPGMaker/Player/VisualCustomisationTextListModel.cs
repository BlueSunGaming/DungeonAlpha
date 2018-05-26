using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker
{
    public class VisualCustomisationTextListModel : VisualCustomiser
    {
        public Text TitleText;
        public Text SelectedOption;
        
        private int _curOption;
        private int _maxOption;
        //private SkinnedMeshRenderer targetRenderer;
        //private List<Material> targetMaterials;


        public void Init(VisualCustomisation customisation)
        {
            VisualCustomisation = customisation;
            TitleText.text = VisualCustomisation.Identifier;

            cached_targets = new List<GameObject>();

            if (VisualCustomisation.CustomisationType == VisualCustomisationType.GameObject)
            {
                //Cache values
                VisualCustomisation.TargetedGameObjectNames.ForEach(t => cached_targets.Add(FindTargetObject(t)));
                _maxOption = VisualCustomisation.TargetedGameObjectNames.Count - 1;
            }
            else if(VisualCustomisation.CustomisationType == VisualCustomisationType.MaterialChange)
            {
                //Cache values
                cached_target = FindTargetObject(VisualCustomisation.TargetedGameObjectName);
                cached_target_renderer = cached_target.GetComponent<Renderer>();
                _maxOption = VisualCustomisation.MaterialPaths.Count - 1;
            }

            //Set Initial Value and initial colors
            _curOption = VisualCustomisation.StartingValueInt;
            SetOption();
        }

        public void NextOption()
        {
            _curOption++;
            if (_curOption > _maxOption)
            {
                _curOption = 0;
            }

            SetOption();
        }

        private void SetOption()
        {
            SelectedOption.text = VisualCustomisation.LabelOptions[_curOption];
            SetTextOption(_curOption);
        }

        public void PrevOption()
        {
            _curOption--;
            if (_curOption < 0)
            {
                _curOption = _maxOption;
            }

            SetOption();
        }

    }
}