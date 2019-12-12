using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Koenig : Piece
{
    public override List<Vector2Int> MoveLocations(Vector2Int gridPoint)
    {
        List<Vector2Int> locations = new List<Vector2Int>();
        List<Vector2Int> directions = new List<Vector2Int>(LaeuferDirections);
        directions.AddRange(TurmDirections);

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nextGridPoint = new Vector2Int(gridPoint.x + dir.x, gridPoint.y + dir.y);
            locations.Add(nextGridPoint);
        }

        return locations;
    }
}
