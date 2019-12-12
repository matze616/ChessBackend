using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelector : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;
    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;
    
    // Erzeugt ein Highlight-Tile und deaktiviert es direkt anschließend
    void Start()
    {
        this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, new Vector3Int(0, 0, 0), Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    //Erzeugt wie beim TileSelector ein Highlight-Tile an der Stelle des Mauszeigers
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = new Vector2Int((int)Math.Round(point.x, 0), (int)Math.Round(point.z, 0));
            
            tileHighlight.SetActive(true);
            tileHighlight.transform.position = new Vector3(gridPoint.x, -0.4f, gridPoint.y);
            //Überprüft nach dem klicken der linken Maustaste welche Aktionen auf dem ausgewählten Feld ausgeführt werden müssen
            if (Input.GetMouseButtonDown(0))
            {
                if (!moveLocations.Contains(gridPoint))
                {
                    this.enabled = false;
                    TileSelector selector = GetComponent<TileSelector>();
                    tileHighlight.SetActive(false);
                    GameManager.instance.DeselectPiece(movingPiece);
                    movingPiece = null;
                    selector.EnterState();
                    foreach (GameObject highlight in locationHighlights)
                    {
                        Destroy(highlight);
                    }
                    return;
                }

                if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                else
                {
                    GameManager.instance.CapturePieceAt(gridPoint);
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                ExitState();
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }
    
    private void CancelMove()
    {
        Debug.Log("Cancel Move");
        this.enabled = false;

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }

        GameManager.instance.DeselectPiece(movingPiece);
        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();
    }
    
    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();

        if (moveLocations.Count == 0)
        {
            CancelMove();
        }

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (GameManager.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, new Vector3(loc.x, -0.4f, loc.y), Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, new Vector3(loc.x, -0.4f, loc.y), Quaternion.identity, gameObject.transform);
            }
            locationHighlights.Add(highlight);
        }
    }

    public void ExitState()
    {
        this.enabled = false;
        TileSelector selector = GetComponent<TileSelector>();
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        GameManager.instance.NextPlayer();
        selector.EnterState();
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    }
}
