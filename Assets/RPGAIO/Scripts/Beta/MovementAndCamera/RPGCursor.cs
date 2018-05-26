using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

public class RPGCursor : MonoBehaviour
{
    public enum CursorState
    {
        Crosshair, Cursor,
        Interactable, Item, Enemy, Npc, Harvest,
        Hide, Draggable
    }

    public CursorState cursorState = CursorState.Cursor;
    public CursorMode cursorMode = CursorMode.Auto;

    public static RPGCursor Instance;

    //Return textures from RPGHandler
    private Texture2D crosshairCursor
    {
        get { return null; }
    }
    private Texture2D defaultCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.Default; }
    }
    private Texture2D itemCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.Item; }
    }
    private Texture2D enemyCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.Enemy; }
    }
    private Texture2D npcCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.NPC; }
    }
    private Texture2D harvestCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.Harvest; }
    }
    private Texture2D interactableCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.Interact; }
    }
    private Texture2D draggableCursor
    {
        get { return Rm_RPGHandler.Instance.GameInfo.Cursors.Draggable; }
    }

    private Camera ourCamera
    {
        get { return GetObject.RPGCamera.GetComponent<Camera>(); }
    }

    private Texture2D cursorToUse;

    public bool PauseCursorLockAndVisibility;

    void OnEnable()
    {
        Instance = this;
    }

    void OnGUI()
    {
        if (!GameMaster.ShowUI) return;
        if (!PauseCursorLockAndVisibility)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        cursorState = CursorState.Cursor;

        if(GameMaster.CutsceneActive)
        {
            Cursor.visible = false;
        }

        int x = Screen.width / 2;
        int y = Screen.height / 2;

        Ray ray = RPG_Camera.instance.cameraMode == CameraMode.FirstPerson 
            ? ourCamera.ScreenPointToRay(new Vector3(x, y))
            : ourCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 1000, new Color(1f, 0.922f, 0.016f, 1f));


        RaycastHit hit;
        var oldCursorMode = cursorState;
        var oldCursorToUse = cursorToUse;
        if(Physics.Raycast(ray,out hit,1000))
        {

            switch(hit.transform.tag)
            {
                case "Interactable":
                    cursorState = CursorState.Interactable;
                    cursorToUse = interactableCursor;
                    break;
                case "Enemy":
                    cursorState = CursorState.Enemy;
                    cursorToUse = enemyCursor;
                    break;
                case "NPC":
                    cursorState = CursorState.Npc;
                    cursorToUse = npcCursor;
                    break;
                case "LootItem":
                    cursorState = CursorState.Item;
                    cursorToUse = itemCursor;
                    break;
                case "Harvestable":
                    cursorState = CursorState.Harvest;
                    cursorToUse = harvestCursor;
                    break;
                case "Draggable":
                    cursorState = CursorState.Draggable;
                    cursorToUse = draggableCursor;
                    break;
                default:
                    cursorState = CursorState.Cursor;
                    cursorToUse = defaultCursor;
                    break;

            }
        }

        if (oldCursorToUse != cursorToUse)
        {
            if (cursorState == CursorState.Cursor)
            {
                Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            }
            else if (cursorState == CursorState.Crosshair)
            {
                Cursor.SetCursor(crosshairCursor, Vector2.zero, cursorMode);
            }
            else if (cursorState == CursorState.Hide)
            {
                Cursor.visible = false;
            }
            else
            {
                if (cursorToUse != null)
                    Cursor.SetCursor(cursorToUse, Vector2.zero, cursorMode);
                else
                    Cursor.SetCursor(defaultCursor, Vector2.zero, cursorMode);
            }
        }
        
//        Debug.Log("cursorState:" + cursorState);

        cursorState = oldCursorMode;
    }
}