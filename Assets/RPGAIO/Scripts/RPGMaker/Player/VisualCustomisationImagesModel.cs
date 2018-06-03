using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker
{
    public class VisualCustomisationImagesModel : VisualCustomiser
    {
        public Text TitleText;
        public GameObject ImageContainer;
        public GameObject ImageSelectPrefab;


        public void Init(VisualCustomisation customisation)
        {
            VisualCustomisation = customisation;
            TitleText.text = VisualCustomisation.Identifier;

            cached_targets = new List<GameObject>();

            if (VisualCustomisation.CustomisationType == VisualCustomisationType.GameObject)
            {
                //Spawn Objects
                ImageContainer.transform.DestroyChildren();
                for (int index = 0; index < VisualCustomisation.TargetedGameObjectNames.Count; index++)
                {
                    var option = VisualCustomisation.TargetedGameObjectNames[index];
                    var go = Instantiate(ImageSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(ImageContainer.transform, false);
                    var textSelectModel = go.GetComponent<VisualImageSelectModel>();
                    textSelectModel.Init(this, option, VisualCustomisation.ImageOptions[index]);
                }

                //Cache values
                VisualCustomisation.TargetedGameObjectNames.ForEach(t => cached_targets.Add(FindTargetObject(t)));

                //Set Initial Value and initial colors
                var startingValue = VisualCustomisation.TargetedGameObjectNames[VisualCustomisation.StartingValueInt];
                SetTextOption(startingValue);
            }
            else if (VisualCustomisation.CustomisationType == VisualCustomisationType.MaterialChange)
            {
                //Spawn Objects
                ImageContainer.transform.DestroyChildren();
                for (int index = 0; index < VisualCustomisation.MaterialPaths.Count; index++)
                {
                    var option = VisualCustomisation.MaterialPaths[index];
                    var go = Instantiate(ImageSelectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(ImageContainer.transform, false);
                    var textSelectModel = go.GetComponent<VisualImageSelectModel>();
                    textSelectModel.Init(this, option, VisualCustomisation.ImageOptions[index]);
                }

                //Cache values
                cached_target = FindTargetObject(VisualCustomisation.TargetedGameObjectName);
                cached_target_renderer = cached_target.GetComponent<Renderer>();


                //Set Initial Value and initial colors
                var startingValue = VisualCustomisation.MaterialPaths[VisualCustomisation.StartingValueInt];
                SetTextOption(startingValue);
            }
        }


    }
}