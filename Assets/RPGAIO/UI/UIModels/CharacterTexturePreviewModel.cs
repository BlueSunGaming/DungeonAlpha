using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterTexturePreviewModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseOnCharacter;
    private Transform cam;
    public float speed;
    private float mouseVal;
    private bool movingRight;
    private bool moving;

    void Update()
    {
        cam = GameObject.Find("CharacterUICamera").transform;
        if(mouseOnCharacter)
        {
            if(Input.GetMouseButtonDown(0))
            {
                moving = true;
            }
            Debug.Log(mouseVal);
        }

        if(moving)
        {
            mouseVal = Input.GetAxis("Mouse X");
            if (mouseVal > 0)
            {
                movingRight = true;
            }
            else if (mouseVal < 0)
            {
                movingRight = false;
            }
            var speedMov = movingRight ? speed : speed * -1;

            cam.transform.RotateAround(GetObject.PlayerMono.transform.position, Vector3.up, speedMov * Time.deltaTime);
        }

        if(Input.GetMouseButtonUp(0))
        {
            moving = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOnCharacter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOnCharacter = false;
    }
}