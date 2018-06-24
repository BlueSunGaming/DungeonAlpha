using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DungeonArchitect.SpatialConstraints;

using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors.SpatialConstraints
{

    public class SpatialConstraintsEditorWindow : EditorWindow
    {
        [MenuItem("Window/Spatial Constraints Editor")]
        static void ShowWindow()
        {
            GetWindowWithRect<SpatialConstraintsEditorWindow>(new Rect(0, 0, 256, 256));
        }
        
        [SerializeField]
        SpatialConstraintsGraphEditor graphEditor;
        public SpatialConstraintsGraphEditor GraphEditor
        {
            get { return graphEditor; }
        }
        
        void OnEnable()
        {
            wantsMouseMove = true;
            UpdateTitle();

            Init(null, SpatialConstraintsEditorAssignmentState.NotAssigned);
            graphEditor.OnEnable();
        }

        void OnDisable()
        {
            if (graphEditor != null)
            {
                graphEditor.OnDisable();
            }
        }

        void OnDestroy()
        {
            if (graphEditor != null)
            {
                graphEditor.OnDisable();
                graphEditor.OnDestroy();
                graphEditor = null;
            }
        }

        public void Init(SpatialConstraintAsset assetBeingEdited, SpatialConstraintsEditorAssignmentState assignmentState)
        {
            graphEditor = CreateInstance<SpatialConstraintsGraphEditor>();
            var graph = (assetBeingEdited != null) ? assetBeingEdited.Graph : null;
            graphEditor.Init(graph, position);
            graphEditor.AssignmentState = assignmentState;
            graphEditor.AssetBeingEdited = assetBeingEdited;

            Repaint();
        }

        void Update()
        {
            if (graphEditor != null)
            {
                graphEditor.Update();
            }
        }

        void OnGUI()
        {
            if (graphEditor != null)
            {
                var originalColor = GUI.backgroundColor;
                graphEditor.Draw(position);
                GUI.backgroundColor = originalColor;

                DrawToolbar();

                HandleInput(Event.current);
            }
        }

        void HandleInput(Event e)
        {
            graphEditor.HandleInput(e);
            
            switch (e.type)
            {
                case EventType.MouseMove:
                case EventType.MouseDrag:
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.KeyDown:
                case EventType.Layout:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    Repaint();
                    break;
            }
        }

        void UpdateTitle()
        {
            titleContent = new GUIContent("Spatial Constraints");
        }

        void DrawToolbar()
        {
            var graphValid = (graphEditor != null && graphEditor.Graph != null);

            if (graphValid)
            {
                var spatialGraphEditor = graphEditor as SpatialConstraintsGraphEditor;

                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

                GUILayout.FlexibleSpace();
                spatialGraphEditor.RealtimeUpdate = GUILayout.Toggle(spatialGraphEditor.RealtimeUpdate, "Realtime Update", EditorStyles.toolbarButton);

                EditorGUILayout.EndHorizontal();
            }

        }

    }
}