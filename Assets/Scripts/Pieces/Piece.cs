using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType {Bauer, Turm, Springer, Laeufer, Koenigin, Koenig};

public abstract class Piece : MonoBehaviour
{
    public PieceType type;

    protected Vector2Int[] TurmDirections =
        {new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0)};

    protected Vector2Int[] LaeuferDirections =
        {new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 1)};

    public abstract List<Vector2Int> MoveLocations(Vector2Int gridPoint);
}
