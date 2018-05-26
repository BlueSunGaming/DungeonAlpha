using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

public class RPG_MinimapCamera : MonoBehaviour
{
    private Transform target;
    private bool Initialised;
    private Camera cameraRef;


    private bool RotateWithPlayer
    {
        get { return Rm_RPGHandler.Instance.GameInfo.MinimapOptions.RotateMinimapWithPlayer; }
    }
    public bool UseOrthographicMode;

    private int zoomDistance = 20;
    public int defaultZoomDistance = 20;
    public int zoomChangeAmount = 10;
    public int maxZoomDistance = 100;
    public int minZoomDistance = 20;

    public int maxIconScale = 20;
    public int minIconScale = 5;

    public float IconScale
    {
        get
        {
            var maxDiff = maxZoomDistance - minZoomDistance;
            var diffInPercent = (zoomDistance - 20) / (float)maxDiff;
            var scale = minIconScale + diffInPercent * (maxIconScale - minIconScale);
            return scale;
        }
    }

    public void Init()
    {
        cameraRef = GetComponent<Camera>();
        target = GetObject.PlayerMonoGameObject.transform.Find("cameraPivot");
        Initialised = true;
        zoomDistance = defaultZoomDistance;
    }


    void Update()
    {
        if (!Initialised) return;

        if(!UseOrthographicMode)
        {
            transform.position = target.transform.position + new Vector3(0, zoomDistance, 0);    
        }
        else
        {
            transform.position = target.transform.position + new Vector3(0,1000,0);
            cameraRef.orthographicSize = zoomDistance;
        }

        cameraRef.orthographic = UseOrthographicMode;

        transform.eulerAngles = RotateWithPlayer ? new Vector3(90, target.eulerAngles.y, 0) : new Vector3(90,0,0);

    }

    public void ZoomIn()
    {
        if(zoomDistance - zoomChangeAmount >= minZoomDistance)
        {
            zoomDistance -= zoomChangeAmount;
        }
    }
    public void ZoomOut()
    {
        if (zoomDistance + zoomChangeAmount <= maxZoomDistance)
        {
            zoomDistance += zoomChangeAmount;
        }
    }
}