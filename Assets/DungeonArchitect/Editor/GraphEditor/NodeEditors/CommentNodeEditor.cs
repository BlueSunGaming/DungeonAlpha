
//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Graphs;

namespace DungeonArchitect.Editors
{
    /// <summary>
    /// Custom property editors for MarkerNode
    /// </summary>
    [CustomEditor(typeof(CommentNode))]
    public class CommentNodeEditor : Editor
    {
        SerializedObject sobject;
        SerializedProperty message;
        SerializedProperty background;

        public void OnEnable()
        {
            sobject = new SerializedObject(target);
            message = sobject.FindProperty("message");
            background = sobject.FindProperty("background");
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();
            
            GUILayout.Label("Comment Node", EditorStyles.boldLabel);
            message.stringValue = EditorGUILayout.TextArea(message.stringValue, GUILayout.MinHeight(60));
            EditorGUILayout.PropertyField(background);
            sobject.ApplyModifiedProperties();
        }
        
    }

    class CommentNodeRenderUtils
    {
        public static readonly Vector2 padding = new Vector2(20, 20);
        public static Font GetRenderFont()
        {
            return UnityEditor.EditorStyles.miniFont;
        }
    }
    
    /// <summary>
    /// Renders a marker node
    /// </summary>
    public class CommentNodeRenderer : GraphNodeRenderer
    {
        Color textColor = Color.white;
        public CommentNodeRenderer(Color textColor)
        {
            this.textColor = textColor;
        }

        public override void Draw(GraphRendererContext rendererContext, GraphNode node, GraphCamera camera)
        {
            var commentNode = node as CommentNode;

            DrawMessage(rendererContext, commentNode, camera);
            
            // Draw the pins
            base.Draw(rendererContext, node, camera);
        }

        void DrawMessage(GraphRendererContext rendererContext, CommentNode node, GraphCamera camera)
        {
            var style = new GUIStyle(GUI.skin.GetStyle("Label"));
            style.alignment = TextAnchor.UpperLeft;

            var miniFontBaseSize = 20;

            style.normal.textColor = node.Selected ? GraphEditorConstants.TEXT_COLOR_SELECTED : GraphEditorConstants.TEXT_COLOR;
            if (camera.ZoomLevel >= 2)
            {

                float scaledFontSize = style.fontSize;
                if (scaledFontSize == 0)
                {
                    scaledFontSize = miniFontBaseSize;
                }
                scaledFontSize = Mathf.Max(1.0f, scaledFontSize / camera.ZoomLevel);
                
                style.fontSize = Mathf.RoundToInt(scaledFontSize);
                style.font = CommentNodeRenderUtils.GetRenderFont();
            }

            GUI.backgroundColor = node.background;

            // Update the node bounds
            var padding = new Vector2(10, 10);
            var textSize = style.CalcSize(new GUIContent(node.message));
            var nodeSize = textSize + padding * 2;

            Rect boxBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position);
                var sizeScreen = nodeSize / camera.ZoomLevel;
                boxBounds = new Rect(positionScreen, sizeScreen);
            }

            Rect textBounds;
            {
                var positionScreen = camera.WorldToScreen(node.Position + padding);
                var sizeScreen = textSize / camera.ZoomLevel;
                textBounds = new Rect(positionScreen, sizeScreen);
            }

            GUI.Box(boxBounds, "");
            style.normal.textColor = textColor;
            GUI.Label(textBounds, node.message, style);

            {
                var nodeBounds = node.Bounds;
                nodeBounds.size = nodeSize;
                node.Bounds = nodeBounds;
            }
        }
    }
}
