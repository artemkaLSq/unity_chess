
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        var locations = new List<Vector2Int>();

        int forwardDirection = GameManager.instance.currentPlayer.forward;
        Vector2Int forward = new Vector2Int(gridPoint.x, gridPoint.y + forwardDirection);
        Vector2Int forward2 = new Vector2Int();

        if (gridPoint.y == 1 && forwardDirection == 1)
        {
            forward2.x = gridPoint.x;
            forward2.y = 3;
        }

        if (gridPoint.y == 6 && forwardDirection == -1) 
        {
            forward2.x = gridPoint.x;
            forward2.y = 4;
        }

        if ((GameManager.instance.PieceAtGrid(forward2) == false) && (GameManager.instance.PieceAtGrid(forward) == false))
        {
            locations.Add(forward2);
        }

        if (GameManager.instance.PieceAtGrid(forward) == false)
        {
            locations.Add(forward);
        }

        Vector2Int forwardRight = new Vector2Int(gridPoint.x + 1, gridPoint.y + forwardDirection);
        if (GameManager.instance.PieceAtGrid(forwardRight))
        {
            locations.Add(forwardRight);
        }

        Vector2Int forwardLeft = new Vector2Int(gridPoint.x - 1, gridPoint.y + forwardDirection);
        if (GameManager.instance.PieceAtGrid(forwardLeft))
        {
            locations.Add(forwardLeft);
        }

        return locations;
    }
}
