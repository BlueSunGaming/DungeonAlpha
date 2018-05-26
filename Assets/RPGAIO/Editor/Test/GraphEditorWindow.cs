//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;
//
//public class GraphEditorWindow : EditorWindow
//{
//    Rect windowRect = new Rect(100 + 100, 100, 100, 100);
//    Rect windowRect2 = new Rect(100, 100, 100, 100);
//
//    private int squareVal = 2;
//    private int productVal = 3;
//    private int numberVal = 1;
//    private float finalValue = 0;
//
//    [MenuItem("Window/Graphs and Charts Window")]
//    static void Init()
//    {
//        EditorWindow.GetWindow(typeof(GraphEditorWindow));
//    }
//
//
//    private int maxLevel = 60;
//    private int maxExp = 1000000;
//    private AnimationCurve expCurve = new AnimationCurve(new Keyframe(0,0), new Keyframe(1,1));
//    private Texture2D onTop;
//    private bool Loaded = false;
//
//    public int ExpToNextLevel(int currentLevel)
//    {
//        return (int)Mathf.Pow((Mathf.Pow((currentLevel / 2 + 2), 2) + Mathf.Pow(currentLevel, 1.0821F)), 2);
//    }
//
//    private bool isValid = true;
//    private int levelToAdd = 0;
//    private int expAtLevel = 0;
//    private List<levelNode> levelNodes = new List<levelNode>();
//    private float angleToDraw = 45;
//
//    void OnGUI()
//    {
//
//        if(!Loaded)
//        {
//            onTop = Resources.Load("RPGMakerAssets/onTop") as Texture2D;
//            Loaded = true;
//        }
//
//        expCurve = EditorGUI.CurveField(new Rect(0, 0, 200, 100), expCurve);
//        GUILayout.BeginArea(new Rect(0, 255, 200, 600));
//        GUILayout.BeginVertical();
//        GUILayout.Label("ax^2 + bx + c");
//        squareVal = EditorGUILayout.IntField("a", squareVal);
//        productVal = EditorGUILayout.IntField("b", productVal);
//        numberVal = EditorGUILayout.IntField("c", numberVal);
//        GUILayout.EndVertical();
////        if (GUILayout.Button("Generate"))
////        {
////            expCurve = new AnimationCurve();
////            for (float i = 0; i <= 100; i++)
////            {
////                var time = i/100.0f;
////                var value = ((squareVal*(time*time)) + (productVal*time) + numberVal) ;
////                expCurve.AddKey(time, value);
////                finalValue = value;
////            }
////        }
//        GUILayout.EndArea();
//        GUILayout.BeginArea(new Rect(0, 255, 200, 600));
//        if(GUILayout.Button("Validate"))
//        {
//            for (int i = 0; i < maxLevel; i++)
//            {
//                if (ExpForLevelFromCurve(i) < 0)
//                {
//                    Debug.Log("You cannot have negative exp to level values.");
//                    isValid = false;
//                }
//                else if(ExpForLevelFromCurve(i) > maxExp)
//                {
//                    Debug.Log("Should not have values above max exp");
//                    isValid = false;
//                }
//
//                if(isValid)
//                {
//                    Debug.Log("Valid exp curve!");
//                }
//            }
//        }
//        levelToAdd = EditorGUILayout.IntField("Level to add for:", levelToAdd);
//        expAtLevel = EditorGUILayout.IntField("Exp to reach next level:", expAtLevel);
//        if (GUILayout.Button("Add New Level Exp") && levelToAdd != 0 && expAtLevel != 0)
//        {
//            levelNodes.Add(new levelNode(levelToAdd, expAtLevel));
//            levelToAdd = expAtLevel = 0;
//        }
//
//        foreach(var levelNode in levelNodes)
//        {
//            var timePoint = (float)levelNode.Level / maxLevel;
//            var value = (float) levelNode.ExpAtLevel/maxExp;
//            expCurve.AddKey(timePoint, value);
//            GUILayout.Box("Level : " + levelNode.Level + "\t Exp: " + levelNode.ExpAtLevel);
//        }
//
//
//        
//
////        //var offset = -225;
////        //angle - (angle + 45)
////        double angleRadians90 = (Math.PI / 180.0) * (90 - (90 + ((180-90)/2)));
////        Vector2 sasd = new Vector2(lineEnd.x + Mathf.Cos((float)angleRadians90) * 180, lineEnd.y + Mathf.Sin((float)angleRadians90) * 180);
////
////        double angleRadians90r = (Math.PI / 180.0) * (90 - 90 - (90 + ((180 - 90) / 2)));
////        Vector2 sasdR = new Vector2(lineEnd.x + Mathf.Cos((float)angleRadians90r) * 180, lineEnd.y + Mathf.Sin((float)angleRadians90r) * 180);
////
////
////
////
////        Drawing.DrawLine(lineEnd, sasd, Color.red, 5, false);
////        Drawing.DrawLine(lineEnd, sasdR, Color.yellow, 5, false);
//        
////        Drawing.DrawLine(lineEnd, sasd45, Color.blue, 5, false);
////        Drawing.DrawLine(lineEnd, sasd45r, Color.cyan, 5, false);
//        angleToDraw = GUILayout.HorizontalSlider(angleToDraw, 45, 180);
//
//        DrawAngle(new Vector2(110, 505), (int)angleToDraw, Color.green);
////        DrawAngle(new Vector2(110, 505), 45, Color.blue);
////        DrawAngle(new Vector2(110, 500), 30, Color.red);
////        
//
//        GUILayout.EndArea();
//
//        Vector2 lineEnd = new Vector2(20, 505);
//        Vector2 newLine = new Vector2(200, 505);
//        Drawing.DrawLine(lineEnd, newLine, Color.white, 1, false);
//
//        DrawAngle(new Vector2(110, 505), (int)angleToDraw, new Color(255, 165, 0));
//
//        isValid = true;
//        if (expCurve.keys[0].value  != 0)
//        {
//            Debug.Log("Key 0 must be at 0,0");
//            expCurve.MoveKey(0, new Keyframe(0, 0));
//            isValid = false;
//        }
//
//        if (expCurve.keys[expCurve.keys.Length - 1].value != 1)
//        {
//            Debug.Log("Last key must be at 1,1");
//            expCurve.MoveKey(expCurve.keys.Length - 1, new Keyframe(1, 1));
//            isValid = false;
//        }
//
//        EditorGUI.LabelField(new Rect(0, 105, 200, 20), "Valid: " + isValid);
//
//
//        for (int i = 1; i <= maxLevel; i++)
//        {
//            if(i < 33)
//                GUI.Box(new Rect(250, (i*25), 250, 20), i + ":" + ExpForLevelFromCurve(i));
//            else if(i < 66)
//                GUI.Box(new Rect(505, ((i - 33) * 25), 250, 20), i + ":" + ExpForLevelFromCurve(i));
//            else
//                GUI.Box(new Rect(760, ((i - 66) * 25), 250, 20), i + ":" + ExpForLevelFromCurve(i));
//        }
//    }
//    public int ExpForLevelFromCurve(int currentLevel)
//    {
//        var pointToEval = (float)currentLevel / maxLevel;
//        var value = expCurve.Evaluate(pointToEval) / finalValue;
//        return (int)(value * maxExp);
//    }
//
//    public void DrawAngle(Vector2 position, int angle, Color color)
//    {
//        var targetHeight = 60;
//        Vector2 lineEnd = position;
//        double angleRadians45 = (Math.PI / 180.0) * (angle - (angle + ((180 - angle) / 2)));
//        Vector2 sasd45 = new Vector2(lineEnd.x + Mathf.Cos((float)angleRadians45) * 60, lineEnd.y + Mathf.Sin((float)angleRadians45) * 60);
//
//        double angleRadians45r = (Math.PI / 180.0) * (angle - angle - (angle + ((180 - angle) / 2)));
//
//        Vector2 sasd45r = new Vector2(lineEnd.x + Mathf.Cos((float)angleRadians45r) * 60, lineEnd.y + Mathf.Sin((float)angleRadians45r) * 60);
//        for (var i = sasd45r.x; i <= sasd45.x; i += 1.0f)
//        {
//            Drawing.DrawLine(lineEnd, new Vector2(i, sasd45.y), color, 1, false);
//
//        }
//
//
//
//        GUI.DrawTexture(new Rect(sasd45r.x, sasd45.y - (sasd45.y - position.y + targetHeight) + 1.8f, sasd45.x - sasd45r.x, sasd45.y - position.y + targetHeight), onTop);
//    }
//
//
//    void WindowFunction(int windowID)
//    {
//        GUI.DragWindow();
//    }
//}
//
//public class levelNode
//{
//    public int Level;
//    public int ExpAtLevel;
//
//    public levelNode(int levelToAdd, int expAtLevel)
//    {
//        Level = levelToAdd;
//        ExpAtLevel = expAtLevel;
//    }
//}