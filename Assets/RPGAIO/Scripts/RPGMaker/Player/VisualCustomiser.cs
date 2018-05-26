using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class VisualCustomiser : MonoBehaviour
    {
        public VisualCustomisation VisualCustomisation;

        //cached data
        protected GameObject cached_target;
        protected Renderer cached_target_renderer;
        protected List<GameObject> cached_targets;
        protected SkinnedMeshRenderer cached_target_skinnedMeshRender;
        protected int cached_int_ref;

        protected GameObject FindTargetObject(string gameObjectName)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            if(player == null)
            {
                Debug.LogError("[RPGAIO] Could not find player gameobject.");
                return null;
            }

            var foundChild = player.transform.FindInChildren(gameObjectName);
            return foundChild != null ? foundChild.gameObject : null;
        }

        public void SetColorOption(RPG_Color color)
        {
            var sharedMaterials = cached_target_renderer.sharedMaterials;
            var newMaterials = new List<Material>();
            foreach(var mat in sharedMaterials)
            {
                newMaterials.Add(new Material(mat));
            }

            var applicableMaterials = newMaterials.Where(m => m.name == VisualCustomisation.StringRef).ToList();
            applicableMaterials.ForEach(m => m.SetColor(VisualCustomisation.StringRefTwo, color.ToUnityColor()));

            cached_target_renderer.sharedMaterials = newMaterials.ToArray();

            VisualCustomisation.SavedColorValue = color;
        }

        public void SetTextOption(int curOption)
        {
            var optionInfo = "";
            if (VisualCustomisation.CustomisationType == VisualCustomisationType.GameObject)
            {
                optionInfo = VisualCustomisation.TargetedGameObjectNames[curOption];
                cached_targets.ForEach(m =>
                {
                    if (m != null)
                    {
                        m.SetActive(false);
                    }
                });

                if (!string.IsNullOrEmpty(optionInfo))
                {
                    cached_targets.First(t => t.name == optionInfo).SetActive(true);
                }
            }
            else if (VisualCustomisation.CustomisationType == VisualCustomisationType.MaterialChange)
            {
                optionInfo = VisualCustomisation.MaterialPaths[curOption];
                var material = (Material)Resources.Load(optionInfo);
                cached_target_renderer.sharedMaterial = material;


                Material[] sharedMaterialsCopy = cached_target_renderer.sharedMaterials;
                for (int index = 0; index < sharedMaterialsCopy.Length; index++)
                {
                    sharedMaterialsCopy[index] = material;
                }
                cached_target_renderer.sharedMaterials = sharedMaterialsCopy;
            }

            VisualCustomisation.SavedStringValue = optionInfo;

        }

        public void SetTextOption(string stringOption)
        {
            var optionInfo = "";
            if (VisualCustomisation.CustomisationType == VisualCustomisationType.GameObject)
            {
                optionInfo = stringOption;
                cached_targets.ForEach(m =>
                {
                    if (m != null)
                    {
                        m.SetActive(false);
                    }
                });

                if (!string.IsNullOrEmpty(optionInfo))
                {
                    cached_targets.First(t => t.name == optionInfo).SetActive(true);
                }
            }
            else if (VisualCustomisation.CustomisationType == VisualCustomisationType.MaterialChange)
            {
                optionInfo = stringOption;
                var material = (Material)Resources.Load(optionInfo);
                cached_target_renderer.sharedMaterial = material;


                Material[] sharedMaterialsCopy = cached_target_renderer.sharedMaterials;
                for (int index = 0; index < sharedMaterialsCopy.Length; index++)
                {
                    sharedMaterialsCopy[index] = material;
                }
                cached_target_renderer.sharedMaterials = sharedMaterialsCopy;
            }

            VisualCustomisation.SavedStringValue = optionInfo;

        }

        public void SetFloatOption(float value)
        {
            if (VisualCustomisation.CustomisationType == VisualCustomisationType.BlendShape)
            {
                cached_target_skinnedMeshRender.SetBlendShapeWeight(cached_int_ref, value);
                VisualCustomisation.SavedFloatValue = value;
            }
            else if (VisualCustomisation.CustomisationType == VisualCustomisationType.Scale)
            {

                var localScale = cached_target.transform.localScale;
                var scale = new Vector3(VisualCustomisation.ScaleX ? value : localScale.x,
                                        VisualCustomisation.ScaleY ? value : localScale.y,
                                        VisualCustomisation.ScaleZ ? value : localScale.z);
                cached_target.transform.localScale = scale;
                VisualCustomisation.SavedFloatValue = value;

                if(VisualCustomisation.ChildCustomisations.Count > 0)
                {

                    var minValue = VisualCustomisation.MinFloatValue;
                    var maxValue = VisualCustomisation.MaxFloatValue;

                    var difference = maxValue - minValue;

                    var ratio = (value - minValue) / difference; 

                    foreach(var childCustomisation in VisualCustomisation.ChildCustomisations)
                    {
                        var minChildValue = childCustomisation.MinFloatValue;
                        var maxChildValue = childCustomisation.MaxFloatValue;

                        var childDifference = maxChildValue - minChildValue;

                        var childValue = minChildValue + (ratio * childDifference);


                        var localChildGameObject = FindTargetObject(childCustomisation.TargetedGameObjectName);
                        var localChildScale = localChildGameObject.transform.localScale;
                        var childScale = new Vector3(childCustomisation.ScaleX ? childValue : localChildScale.x,
                                                childCustomisation.ScaleY ? childValue : localChildScale.y,
                                                childCustomisation.ScaleZ ? childValue : localChildScale.z);
                        localChildGameObject.transform.localScale = childScale;
                        childCustomisation.SavedFloatValue = value;
                    }
                }
                
            }
        }
    }
}