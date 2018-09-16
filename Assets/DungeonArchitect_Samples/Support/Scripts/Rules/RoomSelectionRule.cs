//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;

public class RoomSelectionRule : SelectorRule {
	public override bool CanSelect(PropSocket socket, Matrix4x4 propTransform, DungeonModel model, System.Random random)
	{
	    bool returnVal = true;

		if (model is GridDungeonModel)
		{
			var gridModel = model as GridDungeonModel;
			var cell = gridModel.GetCell(socket.cellId);
		    if (cell != null)
		    {
		        returnVal = cell.CellType != CellType.Room;
		    }
		}

	    return returnVal;
	}
}
