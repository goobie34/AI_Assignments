
public class GameSimulator
{
    private enum SimulatedPlayer
    {
        Trainee = 1,
        Opponent = -1,
    }

    int[] board;
    int boardSize = 9;
    Agent trainee, opponent;
    SimulatedPlayer currentPlayer;
    int n_legalTraineeMoves = 0; //how many legal moves the trainee made before game over
    public int NumOfLegalTraineeMoves { get { return n_legalTraineeMoves; } } //used to determine fitness
    public GameSimulator(NNAgent trainee, Agent opponent, bool traineeMovesFirst)
    {
        board = new int[boardSize];

        this.trainee = (Agent)trainee;
        this.opponent = opponent;

        //the goingFirst flag is used to determine how the board is represented to the NNAgent
        //the agent itself always sees itself as 1 while the opponent is -1 on the board
        trainee.SetGoingFirst(traineeMovesFirst);
        opponent.SetGoingFirst(!traineeMovesFirst);

        currentPlayer = traineeMovesFirst ? SimulatedPlayer.Trainee : SimulatedPlayer.Opponent;
    }

    public SimulationResult Run()
    {
        n_legalTraineeMoves = 0;
        int turnCount = 0;
        while(turnCount < boardSize)
        {
            //get move from current simulated player
            int move = (currentPlayer == SimulatedPlayer.Trainee ? trainee.MakeMove(board) : opponent.MakeMove(board));

            //check if trainee tried to make an illegal move
            //here we are assuming that the opponent will never make illegal moves
            if (!IsMoveLegal(move) && currentPlayer == SimulatedPlayer.Trainee)
                return SimulationResult.IllegalMove;
            else if (currentPlayer == SimulatedPlayer.Trainee)
                n_legalTraineeMoves++;

            //make the move
            board[move] = (int)currentPlayer;

            //check if a player just won
            if (CheckForWin(currentPlayer))
                return (currentPlayer == SimulatedPlayer.Trainee ? SimulationResult.Win : SimulationResult.Lose);

            //switch player
            currentPlayer = (currentPlayer == SimulatedPlayer.Trainee ? SimulatedPlayer.Opponent : SimulatedPlayer.Trainee);
            turnCount++;
        }

        //if 9 rounds have been played without a winner, the simulation results in a draw
        return SimulationResult.Draw;
    }

    private bool IsMoveLegal(int move)
    {
        if (move >= board.Length || move < 0)
            return false;

        return board[move] == 0;
    }

    private bool CheckForWin(SimulatedPlayer player)
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
    private bool CheckForMatch(int a, int b, int c, SimulatedPlayer player)
    {
        if (a < board.Length && b < board.Length && c < board.Length) //validate inputs
            if (board[a] == (int)player && board[b] == (int)player && board[c] == (int)player) //check for win
                return true;

        return false;
    }
}