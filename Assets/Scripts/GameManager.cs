using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Board board;
    public GameObject BauerWeiss;
    public GameObject TurmWeiss;
    public GameObject SpringerWeiss;
    public GameObject LaeuferWeiss;
    public GameObject KoeniginWeiss;
    public GameObject KoenigWeiss;
    
    public GameObject BauerSchwarz;
    public GameObject TurmSchwarz;
    public GameObject SpringerSchwarz;
    public GameObject LaeuferSchwarz;
    public GameObject KoeniginSchwarz;
    public GameObject KoenigSchwarz;

    private GameObject[,] pieces;
    private List<GameObject> movedBauern;

    private Player black;
    private Player white;
    public Player currentPlayer;
    public Player otherPlayer;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pieces = new GameObject[8,8];
        movedBauern = new List<GameObject>();
        
        black = new Player("black",true);
        white = new Player("red",false);
        
        currentPlayer = black;
        otherPlayer = white;
        
        InitialSetup();
    }

    //Figuren werden mit der AddPiece-Funktion aufs Spielfeld gestellt
    private void InitialSetup()
    {
        AddPiece(TurmSchwarz, black, 0, 0);
        AddPiece(TurmSchwarz, black, 7, 0);
        AddPiece(SpringerSchwarz, black, 1, 0);
        AddPiece(SpringerSchwarz, black, 6, 0);
        AddPiece(LaeuferSchwarz, black, 2, 0);
        AddPiece(LaeuferSchwarz, black, 5, 0);
        AddPiece(KoeniginSchwarz, black, 3, 0);
        AddPiece(KoenigSchwarz, black, 4, 0);
        
        AddPiece(TurmWeiss, white, 0, 7);
        AddPiece(TurmWeiss, white, 7, 7);
        AddPiece(SpringerWeiss, white, 1, 7);
        AddPiece(SpringerWeiss, white, 6, 7);
        AddPiece(LaeuferWeiss, white, 2,7);
        AddPiece(LaeuferWeiss, white, 5,7);
        AddPiece(KoeniginWeiss, white, 3, 7);
        AddPiece(KoenigWeiss, white, 4,7);
        
        for (int i = 0; i < 8; i++)
        {
            AddPiece(BauerSchwarz, black, i, 1);
            AddPiece(BauerWeiss, white, i, 6);
        }
        
    }

    //Stellt die Figuren auf die übergebene Position und fügt sie dem Figuren-Array des jeweiligen Spielers hinzu
    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    //Bewegt eine Figur auf dem Spiefeld und änder die Position im Figuren-Array des GameManager
    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Piece pieceComponent = piece.GetComponent<Piece>();
        if (pieceComponent.type == PieceType.Bauer && !HasBauerMoved(piece))
        {
            movedBauern.Add(piece);
        }

        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);
    }

    //Gibt der Board-Klasse die Anweisung eine Figur auszuwählen
    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    //Gibt der Board-Klasse die Anweisung eine Auswahl aufzuheben
    public void DeselectPiece(GameObject piece)
    {
        board.DeselectPiece(piece);
    }

    //Entfernt die Spielfigur an der angegebenen Position aus dem Spiel und fügt sie dem Array der geschlagenen Figuren hinzu
    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        //Gewinn Benachrichtigung wenn König geschlagen wird
        currentPlayer.capturedPieces.Add(pieceToCapture);
        Debug.Log(currentPlayer.capturedPieces.Count);
        pieces[gridPoint.x, gridPoint.y] = null;
        Destroy(pieceToCapture);
    }

    //gibt das GameObject zurück das auf dem übergebenen Punkt steht
    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.x < 0 || gridPoint.y > 7 || gridPoint.y < 0)
        {
            return null;
        }

        return pieces[gridPoint.x, gridPoint.y];
    }

    //Gibt die Züge zurück die eine Figur durchführen kann
    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        List<Vector2Int> locations = piece.MoveLocations(gridPoint);
        
        //Locations außerhalb des Boards ausfiltern
        locations.RemoveAll(gp => gp.x < 0 || gp.x > 7 || gp.y < 0 || gp.y > 7);

        locations.RemoveAll(gp => FriendlyPieceAt(gp));

        return locations;
    }
    
    //Gibt die Position einer Spielfigur zurück
    public Vector2Int GridForPiece(GameObject piece)
    {
        for (int i = 0; i < 8; i++) 
        {
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] == piece)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    //Gibt an ob auf dem Feld mit den übergebenen Koordinaten eine eigene Figur steht
    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null)
        {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }

    //Gibt zurück ob die übergebene Figur im eigenen Figuren-Array vorhanden ist
    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        return currentPlayer.pieces.Contains(piece);
    }

    public bool HasBauerMoved(GameObject bauer)
    {
        return movedBauern.Contains(bauer);
    }

    //Wechselt den Spieler der an der Reihe ist
    public void NextPlayer()
    {
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
    }
}
