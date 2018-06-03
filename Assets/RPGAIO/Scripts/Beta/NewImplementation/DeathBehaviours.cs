using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using UnityEngine;

namespace Assets.Scripts.Beta.NewImplementation
{
    public class DeathBehaviours
    {
        public static List<Material> GetAllMaterials(Transform transform)
        {
            var allRenderers = transform.GetComponentsInChildren<Renderer>();
            var allMats = new List<Material>();
            var matsToLerp = new List<Material>();

            foreach(var renderer in allRenderers)
            {
                allMats.AddRange(renderer.materials);
            }

            foreach(var mat in allMats)
            {
                if(Rm_RPGHandler.Instance.Combat.ShadersToLerp.Any(s => s.ShaderName == mat.shader.name))
                {
                    matsToLerp.Add(mat);
                }
            }

            return matsToLerp;
        }
    }
}