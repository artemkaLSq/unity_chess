using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSelector : MonoBehaviour
{
    public GameObject moveLocationPrefab;
    public GameObject tileHighlightPrefab;
    public GameObject attackLocationPrefab;

    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    private GameObject tileHighlight;
    private GameObject movingPiece;

    private bool moved;

    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        var gridPoint = GameManager.instance.GridForPiece(piece);
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (GameManager.instance.PieceAtGrid(loc))
            {
                highlight = Instantiate(attackLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            else
            {
                highlight = Instantiate(moveLocationPrefab, Geometry.PointFromGrid(loc), Quaternion.identity, gameObject.transform);
            }
            locationHighlights.Add(highlight);
        }
    }

    private void ExitState(Vector2Int gridPoint)
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        if (moved)
        {
            GameManager.instance.CheckPawn(movingPiece, gridPoint);
            GameManager.instance.Checkcheck(GameManager.instance.currentPlayer);
        }
        movingPiece = null;
        TSelector selector = GetComponent<TSelector>();
        GameManager.instance.NextPlayer();
        selector.EnterState();
        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    
    }

    void Start()
    {
        this.enabled = false;
        tileHighlight = Instantiate(tileHighlightPrefab, Geometry.PointFromGrid(new Vector2Int(0, 0)), Quaternion.identity, gameObject.transform);
        tileHighlight.SetActive(false);
    }

    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);

            if (Input.GetMouseButtonDown(1))
            {
                GameManager.instance.NextPlayer();
                moved = false;
                ExitState(gridPoint);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (!moveLocations.Contains(gridPoint))
                {
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
                moved = true;
                ExitState(gridPoint);
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }

    }

}
