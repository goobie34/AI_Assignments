using System.Data;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public enum Player{
    Human = 1,
    AI = -1
}

public enum GameState
{
    PreGame,
    PlayerA,
    PlayerB,
    PostGame
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    [SerializeField] GridManager grid;
    [SerializeField] AIManager aiManager;
    [SerializeField] Player startingPlayer = Player.Human;

    int[] board = new int[9];
    Player currentPlayer = Player.Human;

    public int[] Board { get { return board; } }
    public Player CurrentPlayer { get { return currentPlayer; } }

    private void Awake()
    {
       if (Instance == null)
            Instance = this;
       else
            Destroy(this);
    }

    private void Start()
    {
        StartGame(startingPlayer);
    }

    public void StartGame(Player firstPlayer)
    {
        ClearBoard();
        currentPlayer = firstPlayer;
        if (currentPlayer == Player.AI)
            AIMove();
    }

    public void HumanMove(int move)
    {
        if (currentPlayer == Player.Human)
        {
            bool moveWasLegal = MakeMove(move, Player.Human);
            if (moveWasLegal)
            {
                currentPlayer = Player.AI;
                AIMove();
            }
        }
    }
    public void AIMove()
    {
        if (currentPlayer == Player.AI)
        {
            bool moveWasLegal = MakeMove(aiManager.GetMove(board));
            if (moveWasLegal)
            {
                currentPlayer = Player.Human;
            }
            else
            {
                Debug.Log("WARNING: AI BROKE THE RULES");
                Debug.Log("RESULT: " + Player.Human.ToString() + " WINS");
                StartGame(startingPlayer);
            }
        }
    }

    public bool MakeMove(int move)
    {
        return MakeMove(move, currentPlayer);
    }

    private bool MakeMove(int move, Player player)
    {
        if (player != currentPlayer)
            return false;

        if (!IsMoveLegal(move))
            return false;

        board[move] = (int)player;
        grid.MakeMove(move, player);

        CheckGameOver();

        return true;
    }

    private bool IsMoveLegal(int move)
    {
        if (move >= board.Length || move < 0)
            return false;

        return board[move] == 0;
    }
    private bool CheckForWin()
    {
        if (CheckForWin(Player.Human) || CheckForWin(Player.AI))
            return true;

        return false;
    }
    private bool CheckForWin(Player player)
    {
        //check horizontally
        if (CheckForMatch(0, 1, 2, player) || CheckForMatch(3, 4, 5, player) || CheckForMatch(6, 7, 8, player))
            return true;

        //check vertically
        if (CheckForMatch(0, 3, 6, player) || CheckForMatch(1, 4, 7, player) || CheckForMatch(2, 5, 8, player))
            return true;

        //check diagonally
        if (CheckForMatch(0, 4, 8, player) || CheckForMatch(2, 4, 6, player))
            return true;

        return false;
    }

    //returns true if the three tiles belong to the given player
    private bool CheckForMatch(int x, int y, int z, Player player)
    {
        if (x < board.Length && y < board.Length && z < board.Length) //validate inputs
            if (board[x] == (int)player && board[y] == (int)player && board[z] == (int)player) //check for win
                    return true;

        return false;
    }
    private bool IsBoardFull()
    {
        foreach(int slot in board)
        {
            if (slot == 0)
                return false;
        }

        return true;
    }

    private void ClearBoard()
    {
        board = new int[9];
        grid.ClearBoard();
    }

    private bool CheckGameOver()
    {
        if (CheckForWin())
        {
            Debug.Log("RESULT: " + currentPlayer.ToString() + " WINS!");
            StartGame(startingPlayer);
            return true;
        }

        if (IsBoardFull()) {
            Debug.Log("RESULT: DRAW");
            StartGame(startingPlayer);
            return true;
        }

        return false;
    }
}
