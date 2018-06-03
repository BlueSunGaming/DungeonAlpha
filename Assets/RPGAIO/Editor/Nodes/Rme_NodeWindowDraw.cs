using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public class Rme_NodeWindowDraw
    {
        public static Color nodeLinkColor = new Color(173f/255,216f/255,230f/255,0.6f);
        public static Color nodePropLinkColor = new Color(51f/255,153f/255,230f/255,0.6f);
        public static void DrawLinesNew(List<Node> nodes, Vector2 nodeScrollPos, Rect position,int nodeLinkQuality, List<object[]> allLinks = null)
        {
            allLinks = allLinks ?? new List<object[]>();

            if (allLinks.Count == 0) return;

            for (int index = 0; index < allLinks.Count; index++)
            {
                var link = allLinks[index];
                var isPropLink = link.Length == 4;
                var from = ((Node)link[0]).Rect;
                var to = ((Node)link[1]).Rect;

                var toIsProp =((Node) link[1]).NodeType == NodeType.Property;
                var toIsAdvProp = ((Node)link[1]).NodeType == NodeType.PropertyAndSimple;

                var linkIndex = (int)link[2];

                Vector2 vectorFrom;
                Vector2 vectorTo;
                if(isPropLink)
                {
                    vectorFrom = new Vector2(from.x + from.width - 15,from.y + 15) - nodeScrollPos;

                    var yVal = toIsProp ? 25 : 55;
                    yVal = toIsAdvProp ? 55 : yVal;

                    vectorTo = new Vector2(to.x + 12, to.y + yVal + (25 * (linkIndex + 1))) - nodeScrollPos;
                }
                else
                {
                    vectorFrom = new Vector2(from.x + from.width - 15, from.y + 56 + (25 * (linkIndex))) - nodeScrollPos;
                    vectorTo = new Vector2(to.x + 12, to.y + 55) - nodeScrollPos;
                }


                vectorFrom.x = Mathf.Clamp(vectorFrom.x, 0, position.width);
                vectorFrom.y = Mathf.Clamp(vectorFrom.y, 0, position.height);

                vectorTo.x = Mathf.Clamp(vectorTo.x, 0, position.width);
                vectorTo.y = Mathf.Clamp(vectorTo.y, 0, position.height);

                if (!(vectorFrom.x == 0 && vectorTo.x == 0) &&
                    !(vectorFrom.x == NodeWindow.ScrollDimensions && vectorTo.x == NodeWindow.ScrollDimensions) &&
                    !(vectorFrom.y == 0 && vectorTo.y == 0) &&
                    !(vectorFrom.y == NodeWindow.ScrollDimensions && vectorTo.y == NodeWindow.ScrollDimensions))
                {

                    var dist = Vector2.Distance(vectorFrom, vectorTo);
                    var multiplier = 1 + (dist / 100);
                    var linkQuality =  (int)(nodeLinkQuality*multiplier);
                    RPGAIODrawing.BezierLineX(vectorFrom, vectorFrom + Vector2.right * 50.0f, vectorTo, vectorTo - Vector2.right * 50.0f,
                        isPropLink ? nodePropLinkColor : nodeLinkColor,
                        1.2f, true, linkQuality, position.width, position.height - 25);
                }

            }
        }

        public static void DrawLineFromTo(Node from, int index, Vector2 to, Vector2 nodeScrollPos, Rect position, int nodeLinkQuality)
        {
            var isPropLink = from.NodeType == NodeType.Property;
            Vector2 vectorFrom;
            if (isPropLink)
            {
                vectorFrom = new Vector2(from.Rect.x + from.Rect.width - 13, from.Rect.y + 15) - nodeScrollPos;
            }
            else
            {
                vectorFrom = new Vector2(from.Rect.x + from.Rect.width - 13,from.Rect.y + 56 + (25 * index)) - nodeScrollPos;
            }


            var vectorTo = new Vector2(to.x , to.y);

            vectorFrom.x = Mathf.Clamp(vectorFrom.x, 0, position.width);
            vectorFrom.y = Mathf.Clamp(vectorFrom.y, 0, position.height);

            vectorTo.x = Mathf.Clamp(vectorTo.x, 0, position.width);
            vectorTo.y = Mathf.Clamp(vectorTo.y, 0, position.height);

            if (!(vectorFrom.x == 0 && vectorTo.x == 0) &&
                !(vectorFrom.x == NodeWindow.ScrollDimensions && vectorTo.x == NodeWindow.ScrollDimensions) &&
                !(vectorFrom.y == 0 && vectorTo.y == 0) &&
                !(vectorFrom.y == NodeWindow.ScrollDimensions && vectorTo.y == NodeWindow.ScrollDimensions))
            {

                var dist = Vector2.Distance(vectorFrom, vectorTo);
                var multiplier = 1 + (dist / 100);
                nodeLinkQuality = (int)(nodeLinkQuality * multiplier);
                RPGAIODrawing.BezierLineX(vectorFrom, vectorFrom + Vector2.right * 50.0f, vectorTo, vectorTo - Vector2.right * 50.0f, nodeLinkColor, 1.2f, true, nodeLinkQuality, position.width, position.height - 25);
            }
        }

        public static void DrawNodeGrid(Rect position, float snapInt, Color color, float alpha = 0.3f, float width = 1)
        {
            color.a = alpha;
            //vertical lines
            for (float x = snapInt; x < position.width; x += snapInt)
            {
                RPGAIODrawing.DrawLine(new Vector2(x, position.y), new Vector2(x, position.height - 15), color, width, false);
            }

            //horizontal lines
            for (float y = position.y; y <= position.height; y += snapInt)  
            {
                RPGAIODrawing.DrawLine(new Vector2(0, y), new Vector2((position.width * 2) - 30, y), color, width, false);
            }
        }
    }
}