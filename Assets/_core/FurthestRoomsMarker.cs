using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;

public class FurthestRoomsMarker : DungeonMarkerEmitter
{

    public string startMarker = "StartRoom";
    public string endMarker = "EndRoom";

    public override void EmitMarkers(DungeonBuilder builder)
    {
        var gridModel = builder.Model as GridDungeonModel;
        if (gridModel == null)
        {
            // Can only be used with a grid dungeon
            return;
        }
        var furthestCells = GridDungeonModelUtils.FindFurthestRooms(gridModel);
        var cellSize = gridModel.Config.GridCellSize;
        EmitMarkerOnCell(furthestCells[0], cellSize, builder, startMarker);
        Debug.Log(furthestCells[0] + " represents the start marker");
        EmitMarkerOnCell(furthestCells[1], cellSize, builder, endMarker);
        

        Debug.Log(furthestCells[1] + " represents the end marker");
    }

    void EmitMarkerOnCell(Cell cell, Vector3 cellSize, DungeonBuilder builder, string markerName)
    {
        var bounds = cell.Bounds;
        var cx = (bounds.Location.x + bounds.Size.x / 2.0f) * cellSize.x;
        var cy = bounds.Location.y * cellSize.y;
        var cz = (bounds.Location.z + bounds.Size.z / 2.0f) * cellSize.z;
        var position = new Vector3(cx, cy, cz);
        var transform = Matrix4x4.TRS(position, Quaternion.identity, Vector3.one);
        builder.EmitMarker(markerName, transform, cell.Bounds.Location, cell.Id);
    }

}