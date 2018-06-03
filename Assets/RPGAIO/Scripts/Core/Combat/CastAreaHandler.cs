using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

public class CastAreaHandler : MonoBehaviour
{
    private Projector Projector;
    void Awake()
    {
        Projector = GetComponent<Projector>();
        if(Projector != null)
        {
            var material = Projector.material;
            material.SetTexture("_ShadowTex", Rm_RPGHandler.Instance.Combat.CastAreaTexture.Image);
            material.SetColor("_Color", Rm_RPGHandler.Instance.Combat.CastAreaColor);
        }
    }

    void Update()
    {
        if (Projector != null)
        {
            Projector.ignoreLayers = ~CastAreaLayers.LayerMask;   
        }
    }
}
