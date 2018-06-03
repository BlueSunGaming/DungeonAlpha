
using System;
using System.Linq;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    [CustomEditor(typeof(RPG_LevelSwitchTrigger))]
    public class RPG_LevelSwitchTriggerEditor : UnityEditor.Editor
    {
        SerializedProperty sceneName;
        SerializedProperty interactType;
        SerializedProperty distance;


        void OnEnable()
        {
            sceneName = serializedObject.FindProperty("SceneName");
            interactType = serializedObject.FindProperty("InteractType");
            distance = serializedObject.FindProperty("Distance");
        }   

        public override void OnInspectorGUI()
        {
            if (Rm_RPGHandler.Instance == null) return;

            serializedObject.Update();
            EditorGUILayout.PropertyField(sceneName, new GUIContent("Scene Name:"));


            EditorGUILayout.PropertyField(interactType, new GUIContent("Condition:"));

            if (interactType.enumValueIndex == (int)InteractType.NearTo)
            {
                EditorGUILayout.PropertyField(distance, new GUIContent("Trigger Distance:"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
