using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public GameObject tileHighlightPrefab;

    private GameObject tileHighlight;
    
    // Erzeugt ein Highlight-Tile und deaktiviert es direkt anschließend
    void Start()
    {
        Vector3 point = new Vector3(0, -0.4f, 0);
        tileHighlight = Instantiate(tileHighlightPrefab, point, Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    // Überprüft ob die Maus auf einem Feld des Spielbretts ist und erzeugt an der Position des Mauszeigers ein Highlight-Tile
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = new Vector2Int((int)Math.Round(point.x, 0), (int)Math.Round(point.z, 0));
            if (gridPoint.x > 7 || gridPoint.x < 0 || gridPoint.y > 7 || gridPoint.y < 0)
            {
                tileHighlight.SetActive(false);
            }
            else
            {
                tileHighlight.SetActive(true);
                tileHighlight.transform.position = new Vector3(gridPoint.x, -0.4f, gridPoint.y);
                //Wenn die linke Maustaste gedrückt wird wird überprüft ob die Figur zum Spieler gehört der momentan an der Reihe ist 
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject selectedPiece = GameManager.instance.PieceAtGrid(gridPoint);
                    if (GameManager.instance.DoesPieceBelongToCurrentPlayer(selectedPiece))
                    {
                        GameManager.instance.SelectPiece(selectedPiece);
                        ExitState(selectedPiece);
                    }
                }
            }

        }
    }

    public void EnterState()
    {
        enabled = true;
    }

    //Beendet den TileSelector und ruft den MoveSelector zum bewegen der Figur auf
    private void ExitState(GameObject movingPiece)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        MoveSelector move = GetComponent<MoveSelector>();
        move.EnterState(movingPiece);
    }
}
