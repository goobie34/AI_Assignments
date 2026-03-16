using UnityEngine;

public enum PlayerType
{
    Human,
    AI
}

public class TurnManager : MonoBehaviour
{
    [SerializeField] PlayerType playerA;
    [SerializeField] PlayerType playerB;

    public Player currentPlayer;
    Agent opponentAI;

    private void Awake()
    {
        int[] networkShape = { 9, 4, 4, 9 };
        //opponentAI = new RandomAgent();
        //NNAgent nnAgent = new NNAgent(networkShape, true);
        //nnAgent.Network.Mutate(30f, 30);
        //JSONHandler.Write(nnAgent.Network.ToJson());
        //string data = JSONHandler.Read("nn.json");
        //SimpleNN simpleNN = new SimpleNN(data);
        //NNAgent nnAgent = new NNAgent(simpleNN);
        //opponentAI = nnAgent;
    }
    public void NextPlayer()
    {
        currentPlayer = (currentPlayer == Player.Human ? Player.AI : Player.Human);
        if (currentPlayer == Player.Human && playerA == PlayerType.AI || currentPlayer == Player.AI && playerB == PlayerType.AI)
        {
            int move = opponentAI.MakeMove(GameManager.Instance.Board);
            if (move < 0)
            {
                Debug.Log("AI FAILED TO MAKE A MOVE" + move);
                return;
            }
            GameManager.Instance.MakeMove(move);
            Debug.Log("NN JUST MADE A MOVE: " + move);
        }
    }



}


