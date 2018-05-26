using System;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor;
using UnityEditor;
using UnityEngine;

public class GamePadTest : EditorWindow
{
    
    [MenuItem("Window/Gamepad")]
    static void Init()
    {
        var window = EditorWindow.GetWindow(typeof(GamePadTest));
        window.minSize = new Vector2(600, 600);
    }
    void OnGUI()
    {
        try
        {
            OnGUIX();
        }
        catch (Exception e )
        {
    
        }
    }

    void OnGUIX()
    {
        GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;

        GUILayout.Label(String.Join(",", Input.GetJoystickNames().Select(s => s   + ",").ToArray()));
        if(GUILayout.Button("ASDASDAS"))
        {
            UnityGamepadHandler.AxisDefined("Fire1");
        }


        var e = Event.current;
        if(e.isKey)
        {
            Debug.Log("Key:" + e.keyCode.ToString());
            Debug.Log("Key2:" + e.button.ToString());
        }
    }


    void Update()
    {
        if(Input.GetKey(KeyCode.JoystickButton0))
        {
            Debug.Log("2312");
        }
        if(Input.GetKey(KeyCode.Joystick1Button0))
        {
            Debug.Log("JJJ");
        }
        if (Input.anyKeyDown)
        {
            Debug.Log("??????????????????????");
        }
        if (Input.anyKey)
        {
            Debug.Log("!!!!!!!!!!!");
        }
    }
}

