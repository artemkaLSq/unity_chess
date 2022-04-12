using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDMan : MonoBehaviour
{
    public static HUDMan instance;
    public Text yourturn;
    public Text p1name;
    public Text p2name;
    public Dropdown pawndrop;
    public InputField inputp1name;
    public InputField inputp2name;
    private Vector3 _whiteyourturn = new Vector3(230f, 900f, 0f);
    private Vector3 _blackyourturn = new Vector3(1700f, 900f, 0f);



    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SwapTurn()
    {
        if (GameManager.instance.currentPlayer == GameManager.instance.white)
            yourturn.transform.position = _whiteyourturn;
        else
            yourturn.transform.position = _blackyourturn;
    }

    public void SetNames()
    {
        if (inputp1name.text == "")         
            p1name.text = "Player 1";
        else
            p1name.text = inputp1name.text;
        if (inputp2name.text == "")
            p2name.text = "Player 2";
        else
            p2name.text = inputp2name.text;

        GameManager.instance.board.gameObject.SetActive(true);
    }

    public void PawnButton()
    {
        int x = GameManager.instance.pawnpos.x;
        int y = GameManager.instance.pawnpos.y;


        if (GameManager.instance.currentPlayer == GameManager.instance.white)
            switch (pawndrop.value)
            {
            case 0: 
                GameManager.instance.AddPiece(GameManager.instance.blackKnight, GameManager.instance.black, x, y);
                break;
            case 1:
                    GameManager.instance.AddPiece(GameManager.instance.blackBishop, GameManager.instance.black, x, y);
                    break;
            case 2:
                    GameManager.instance.AddPiece(GameManager.instance.blackQueen, GameManager.instance.black, x, y);
                    break;
            case 3:
                    GameManager.instance.AddPiece(GameManager.instance.blackRook, GameManager.instance.black, x, y);
                    break;
                default: break;
            }
        else
            switch (pawndrop.value)
            {
                case 0:
                    GameManager.instance.AddPiece(GameManager.instance.whiteKnight, GameManager.instance.white, x, y);
                    break;
                case 1:
                    GameManager.instance.AddPiece(GameManager.instance.whiteBishop, GameManager.instance.white, x, y);
                    break;
                case 2:
                    GameManager.instance.AddPiece(GameManager.instance.whiteQueen, GameManager.instance.white, x, y);
                    break;
                case 3:
                    GameManager.instance.AddPiece(GameManager.instance.whiteRook, GameManager.instance.white, x, y);
                    break;
                default: break;
            }
        GameManager.instance.pawnchange.SetActive(false);
        GameManager.instance.board.gameObject.SetActive(true);
    }

    public void Escbutton()
    {
        SceneManager.LoadScene("Menu");
    }
}
