using UnityEngine;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Builders.Grid
{
    /// <summary>
    /// Helper functions to draw debug information of the dungeon layout in the scene view 
    /// </summary>
    public class GridDebugDrawUtils
    {

        public static void DrawCell(Cell cell, Color color, Vector3 gridScale, bool mode2D)
        {
            DebugDrawUtils.DrawBounds(cell.Bounds, color, gridScale, mode2D);
        }

        public static void DrawCellId(Cell cell, Vector3 gridScale, bool mode2D)
        {
            var center = Vector3.Scale(cell.Bounds.CenterF(), gridScale); // + new Vector3(0, .2f, 0);
            var screenCoord = Camera.main.WorldToScreenPoint(center);
            if (screenCoord.z > 0)
            {
                GUI.Label(new Rect(screenCoord.x, Screen.height - screenCoord.y, 100, 50), "" + cell.Id);
            }
        }
        
        public static void DrawAdjacentCells(Cell cell, GridDungeonModel model, Color color, bool mode2D)
        {
            if (model == null) return;
            var gridConfig = model.Config as GridDungeonConfig;
            if (gridConfig == null) return;

            foreach (var adjacentId in cell.AdjacentCells)
            {
                var adjacentCell = model.GetCell(adjacentId);
                if (adjacentCell == null) return;
                var centerA = Vector3.Scale(cell.Bounds.CenterF(), gridConfig.GridCellSize);
                var centerB = Vector3.Scale(adjacentCell.Bounds.CenterF(), gridConfig.GridCellSize);
                DebugDrawUtils.DrawLine(centerA, centerB, color, 0, false, mode2D);
            }

            foreach (var adjacentId in cell.FixedRoomConnections)
            {
                var adjacentCell = model.GetCell(adjacentId);
                if (adjacentCell == null) return;
                var centerA = Vector3.Scale(cell.Bounds.CenterF(), gridConfig.GridCellSize) + new Vector3(0, 0.2f, 0);
                var centerB = Vector3.Scale(adjacentCell.Bounds.CenterF(), gridConfig.GridCellSize) + new Vector3(0, 0.2f, 0);
                DebugDrawUtils.DrawLine(centerA, centerB, Color.red, 0, false, mode2D);
            }

        }
        
    }
}
