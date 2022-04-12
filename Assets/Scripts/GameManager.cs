
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Board board;
    public HUDMan hud;
    public GameObject pawnchange;
    public GameObject endgame;
    public Text endgametxt;
    public Text p1name;
    public Text p2name;
    public Text whitecheck;
    public Text blackcheck;
    public Vector2Int pawnpos;

    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    public GameObject[,] pieces;

    public Player white;
    public Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    private Vector3 _whiteCameraPos = new Vector3(0.02f, 7.5f, -6.5f);
    private Vector3 _blackCameraPos = new Vector3(0.02f, 7.5f, 6.5f);
    private Vector3 _whiteCameraRot = new Vector3(50, 0, 0);
    private Vector3 _blackCameraRot = new Vector3(50, 180, 0);
    private Vector3 _currentPos;
    private Vector3 _currentRot;



    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pieces = new GameObject[8, 8];

        white = new Player("white", true);
        black = new Player("black", false);

        currentPlayer = white;
        otherPlayer = black;
        InitialSetup();
    }

    private void InitialSetup()
    {
        AddPiece(whiteRook, white, 0, 0);
        AddPiece(whiteKnight, white, 1, 0);
        AddPiece(whiteBishop, white, 2, 0);
        AddPiece(whiteQueen, white, 3, 0);
        AddPiece(whiteKing, white, 4, 0);
        AddPiece(whiteBishop, white, 5, 0);
        AddPiece(whiteKnight, white, 6, 0);
        AddPiece(whiteRook, white, 7, 0);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(whitePawn, white, i, 1);
        }

        AddPiece(blackRook, black, 0, 7);
        AddPiece(blackKnight, black, 1, 7);
        AddPiece(blackBishop, black, 2, 7);
        AddPiece(blackQueen, black, 3, 7);
        AddPiece(blackKing, black, 4, 7);
        AddPiece(blackBishop, black, 5, 7);
        AddPiece(blackKnight, black, 6, 7);
        AddPiece(blackRook, black, 7, 7);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(blackPawn, black, i, 6);
        }
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    public void SelectPieceAtGrid(Vector2Int gridPoint)
    {
        GameObject selectedPiece = pieces[gridPoint.x, gridPoint.y];
        if (selectedPiece)
        {
            board.SelectPiece(selectedPiece);
        }
    }

    public void SelectPiece(GameObject piece)
    {
        board.SelectPiece(piece);
    }

    public void DeselectPiece(GameObject piece)
    {
        board.DeselectPiece(piece);
    }

    public GameObject PieceAtGrid(Vector2Int gridPoint)
    {
        if (gridPoint.x > 7 || gridPoint.y > 7 || gridPoint.x < 0 || gridPoint.y < 0)
        {
            return null;
        }
        return pieces[gridPoint.x, gridPoint.y];
    }

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

    public bool FriendlyPieceAt(Vector2Int gridPoint)
    {
        GameObject piece = PieceAtGrid(gridPoint);

        if (piece == null) {
            return false;
        }

        if (otherPlayer.pieces.Contains(piece))
        {
            return false;
        }

        return true;
    }

    public bool DoesPieceBelongToCurrentPlayer(GameObject piece)
    {
        return currentPlayer.pieces.Contains(piece);
    }

    public void Move(GameObject piece, Vector2Int gridPoint)
    {
        Vector2Int startGridPoint = GridForPiece(piece);
        pieces[startGridPoint.x, startGridPoint.y] = null;
        pieces[gridPoint.x, gridPoint.y] = piece;
        board.MovePiece(piece, gridPoint);

        if ((piece.GetComponent<Piece>().type == PieceType.King) && (Math.Abs(startGridPoint.x - gridPoint.x) > 1))
        {
            if (gridPoint.x - startGridPoint.x > 0)
            {
                Move(PieceAtGrid(new Vector2Int(7, startGridPoint.y)), new Vector2Int(5, startGridPoint.y));
            }
            else
            {
                Move(PieceAtGrid(new Vector2Int(0, startGridPoint.y)), new Vector2Int(3, startGridPoint.y));
            }
        }
    }

    public List<Vector2Int> MovesForPiece(GameObject pieceObject)
    {
        Piece piece = pieceObject.GetComponent<Piece>();
        Vector2Int gridPoint = GridForPiece(pieceObject);
        var locations = piece.MoveLocations(gridPoint);

        if ((pieceObject.GetComponent<Piece>().type == PieceType.King) && currentPlayer.pieces.Contains(pieceObject))
        {
            pieces[gridPoint.x, gridPoint.y] = null;
            foreach (GameObject otherpiece in otherPlayer.pieces)
            {
                if (otherpiece.GetComponent<Piece>().type != PieceType.Pawn)
                    locations.RemoveAll(tile => MovesForPiece(otherpiece).Contains(tile));
                else
                {
                    if (currentPlayer == white)
                    {
                        locations.Remove(new Vector2Int(GridForPiece(otherpiece).x + 1, GridForPiece(otherpiece).y - 1));
                        locations.Remove(new Vector2Int(GridForPiece(otherpiece).x - 1, GridForPiece(otherpiece).y - 1));
                    }
                    else
                    {
                        locations.Remove(new Vector2Int(GridForPiece(otherpiece).x + 1, GridForPiece(otherpiece).y + 1));
                        locations.Remove(new Vector2Int(GridForPiece(otherpiece).x - 1, GridForPiece(otherpiece).y + 1));
                    }

                }
            }
            pieces[gridPoint.x, gridPoint.y] = pieceObject;

            int kingY = gridPoint.y;

            if (gridPoint.x==4 && gridPoint.y == kingY)
            {
                if (PieceAtGrid(new Vector2Int(7, kingY))!=null)
                    if ((PieceAtGrid(new Vector2Int(7,kingY)).GetComponent<Piece>().type == PieceType.Rook) && (PieceAtGrid(new Vector2Int(5, kingY))==null) && (PieceAtGrid(new Vector2Int(6, kingY)) == null))
                    {
                    locations.Add(new Vector2Int(6, kingY));
                    }
                if (PieceAtGrid(new Vector2Int(0, kingY)) != null)
                    if ((PieceAtGrid(new Vector2Int(0, 0)).GetComponent<Piece>().type == PieceType.Rook) && (PieceAtGrid(new Vector2Int(1, kingY)) == null) && (PieceAtGrid(new Vector2Int(2, kingY)) == null) && (PieceAtGrid(new Vector2Int(3, kingY)) == null))
                    {
                    locations.Add(new Vector2Int(2, kingY));
                    }
            }
            
        }

        locations.RemoveAll(tile => tile.x < 0 || tile.x > 7
            || tile.y < 0 || tile.y > 7);

        locations.RemoveAll(tile => FriendlyPieceAt(tile));

        return locations;
    }

    public void NextPlayer()
    {
        Player tempPlayer = currentPlayer;
        currentPlayer = otherPlayer;
        otherPlayer = tempPlayer;
        hud.SwapTurn();
        this.enabled = true;

        if (currentPlayer == white)
        {
            _currentPos = _whiteCameraPos;
            _currentRot = _whiteCameraRot;
        }
        else
        {
            _currentPos = _blackCameraPos;
            _currentRot = _blackCameraRot;
        }

        Camera.main.transform.position = _currentPos;
        Camera.main.transform.rotation = Quaternion.Euler(_currentRot);
    }

    public void CapturePieceAt(Vector2Int gridPoint)
    {
        GameObject pieceToCapture = PieceAtGrid(gridPoint);
        if (pieceToCapture.GetComponent<Piece>().type == PieceType.King)
        {
            endgame.gameObject.SetActive(true);
            if (currentPlayer == white)
                endgametxt.text = p1name.text + " wins!";
            else
                endgametxt.text = p2name.text + " wins!";
            Destroy(board.GetComponent<TSelector>());
            Destroy(board.GetComponent<MSelector>());
        }
        if (pieceToCapture.GetComponent<Piece>().type != PieceType.Pawn)
            currentPlayer.capturedPieces.Add(pieceToCapture);
        pieces[gridPoint.x, gridPoint.y] = null;
        pieceToCapture.SetActive(false);
    }
    public void CheckPawn(GameObject piece, Vector2Int point)
    {
        if (piece.GetComponent<Piece>().type == PieceType.Pawn)
        {
            if ((point.y == 7)||(point.y == 0))
            {
                pawnpos = point;      
                piece.SetActive(false);
                pawnchange.SetActive(true);
                GameManager.instance.board.gameObject.SetActive(false);             
            }
        }
    }

    public void Checkcheck(Player pl) 
    {
        foreach (GameObject otherpiece in pl.pieces)
        {
            if (otherpiece.GetComponent<Piece>().type != PieceType.Pawn)
                foreach (Vector2Int move in MovesForPiece(otherpiece))
                {
                    var piece = PieceAtGrid(move);
                    if (piece != null)
                        if ((piece.GetComponent<Piece>().type == PieceType.King) && (!pl.pieces.Contains(piece)))
                            if (currentPlayer == black) blackcheck.gameObject.SetActive(true);
                            else whitecheck.gameObject.SetActive(true);
                }

            else
            {
                if (pl == black)
                {
                    var piece1 = PieceAtGrid(new Vector2Int(GridForPiece(otherpiece).x + 1, GridForPiece(otherpiece).y - 1));
                    var piece2 = PieceAtGrid(new Vector2Int(GridForPiece(otherpiece).x - 1, GridForPiece(otherpiece).y - 1));
                    if (piece1!=null)
                        if ((piece1.GetComponent<Piece>().type == PieceType.King) && (!pl.pieces.Contains(piece1)))
                            if (currentPlayer == black) blackcheck.gameObject.SetActive(true);
                            else whitecheck.gameObject.SetActive(true);
                    if (piece2 != null)
                        if ((piece2.GetComponent<Piece>().type == PieceType.King) && (!pl.pieces.Contains(piece2)))
                            if (currentPlayer == black) blackcheck.gameObject.SetActive(true);
                            else whitecheck.gameObject.SetActive(true);

                }
                else
                {
                    var piece1 = PieceAtGrid(new Vector2Int(GridForPiece(otherpiece).x + 1, GridForPiece(otherpiece).y + 1));
                    var piece2 = PieceAtGrid(new Vector2Int(GridForPiece(otherpiece).x - 1, GridForPiece(otherpiece).y + 1));
                    if (piece1 != null)
                        if ((piece1.GetComponent<Piece>().type == PieceType.King) && (!pl.pieces.Contains(piece1)))
                            if (currentPlayer == black) blackcheck.gameObject.SetActive(true);
                            else whitecheck.gameObject.SetActive(true);
                    if (piece2 != null)
                        if ((piece2.GetComponent<Piece>().type == PieceType.King) && (!pl.pieces.Contains(piece2)))
                            if (currentPlayer == black) blackcheck.gameObject.SetActive(true);
                            else whitecheck.gameObject.SetActive(true);
                }

            }
        }

        if (currentPlayer == white) blackcheck.gameObject.SetActive(false);
        else whitecheck.gameObject.SetActive(false);
    }

    private bool Check_check(Player pl, )
}
