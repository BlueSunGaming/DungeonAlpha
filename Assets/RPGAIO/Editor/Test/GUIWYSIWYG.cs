using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GUIWYSIWYG : EditorWindow
{

    private List<Rm_GUIElement> GUIElements = new List<Rm_GUIElement>();
    private Rm_GUIElement selectedGUIelement = null;

    private List<GUIType> addableElements = new List<GUIType>()
                                                {
                                                    GUIType.Textfield,
                                                    GUIType.EditorTextfield,
                                                    GUIType.BeginVertical,
                                                    GUIType.BeginHorizontal
                                                };
    [MenuItem("Window/GUI WYSIWYG")]
    static void Init()
    {
        var window = EditorWindow.GetWindow(typeof(GUIWYSIWYG));
        window.minSize = new Vector2(600, 600);
    }
    
    void OnGUI()
    {
        try
        {

            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            foreach (var element in GUIElements)
            {
                if (element.Type == GUIType.Textfield)
                {
                    element.StringValue = GUILayout.TextField(element.StringValue, element.LayoutOptions);
                }

                if (element.Type == GUIType.EditorTextfield)
                {
                    element.StringValue = EditorGUILayout.TextField(element.EditorGUILabel, element.StringValue);
                }

                if (element.Type == GUIType.BeginHorizontal)
                {
                    GUILayout.BeginHorizontal();
                }
                if (element.Type == GUIType.EndHorizontal)
                {
                    GUILayout.EndHorizontal();
                }
                if (element.Type == GUIType.BeginVertical)
                {
                    GUILayout.BeginVertical();
                }
                if (element.Type == GUIType.EndVertical)
                {
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.MaxWidth(180));
            var rect = RPGMakerGUI.ListArea(GUIElements, ref selectedGUIelement, Rm_ListAreaType.Vitals, true, true);
            var evt = Event.current;
            if (evt.type == EventType.MouseDown)
            {
                var mousePos = evt.mousePosition;
                if (rect.Contains(mousePos))
                {
                    var menu = new GenericMenu();
                    foreach (var typeOf in addableElements)
                    {
                        menu.AddItem(new GUIContent(typeOf.ToString()), false, AddGUIElement(), typeOf);
                    }
                    menu.ShowAsContext();
                    evt.Use();
                }
            }
            if (selectedGUIelement != null)
            {
                RPGMakerGUI.SubTitle("GUI Element Settings");
                if (selectedGUIelement.Type == GUIType.Textfield)
                {
                    selectedGUIelement.MaxWidth = RPGMakerGUI.IntField("Max Width", selectedGUIelement.MaxWidth);
                } if (selectedGUIelement.Type == GUIType.EditorTextfield)
                {
                    selectedGUIelement.EditorGUILabel = RPGMakerGUI.TextField("Prefix Label:", selectedGUIelement.EditorGUILabel);
                }

                if (GUILayout.Button("Save(tesT)"))
                {
                    var b = GeneralMethods.CopyObject(selectedGUIelement);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();



            GUILayout.EndHorizontal();
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    private GenericMenu.MenuFunction2 AddGUIElement()
    {
        return data =>
                   {
                       var type = (GUIType)data;
                       if(type == GUIType.BeginHorizontal)
                       {
                           GUIElements.Add(new Rm_GUIElement(GUIType.BeginHorizontal));
                           GUIElements.Add(new Rm_GUIElement(GUIType.EndHorizontal));
                       }
                       else if(type == GUIType.BeginVertical)
                       {
                           GUIElements.Add(new Rm_GUIElement(GUIType.BeginVertical));
                           GUIElements.Add(new Rm_GUIElement(GUIType.EndVertical));
                       }
                       else
                       {
                           GUIElements.Add(new Rm_GUIElement(type));
                       }
                   };
    }

    void DoSomething()
    {
        Debug.Log("Wow singa");
    }
}

public class Rm_GUIElement
{
    public GUIType Type;
    public string EditorGUILabel;
    public string StringValue;
    public int MaxWidth = 200;

    [JsonIgnore]
    public GUILayoutOption[] LayoutOptions
    {
        get{return new GUILayoutOption[]
                       {
                           GUILayout.MaxWidth(MaxWidth)
                       };}
    }
    public Rm_GUIElement(GUIType type)
    {
        Type = type;
        EditorGUILabel = "Label:";
        StringValue = "[StringValue]";
    }

    public override string ToString()
    {
        return Type.ToString();
    }
}

public enum GUIType
{
    Textfield,
    EditorTextfield,
    BeginVertical,
    EndVertical,
    BeginHorizontal,
    EndHorizontal
}