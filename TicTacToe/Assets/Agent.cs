using System;

public interface Agent
{
    public int MakeMove(int[] input);
    public void SetFlipBoard(bool goingFirst);
    public void SetBlockIllegalMoves(bool blockIllegalMoves);
}

public class RandomAgent : Agent
{
    int[] legalMoves;
    Random rnd;
    public RandomAgent(int boardSize = 9)
    {
        legalMoves = new int[boardSize];
        rnd = new Random();
    }
    public int MakeMove(int[] input)
    {
        //reset legalMoves
        for (int i = 0; i < legalMoves.Length; i++)
            legalMoves[i] = 0;

        //count how many legal moves are possible
        //in other words: how many zeroes are there, and where on board are they?
        int n_legalMoves = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == 0)
            {
                legalMoves[n_legalMoves] = i;
                n_legalMoves++;
            }
        }

        if (n_legalMoves == 0) return -1; //if no legal moves exist

        int randomIndex = rnd.Next(0, n_legalMoves); //pick a random legal move
        return legalMoves[randomIndex];
    }
    public void SetFlipBoard(bool goingFirst) { }
    public void SetBlockIllegalMoves(bool blockIllegalMoves) { }
}

public class NNAgent : Agent
{
    SimpleNN network;
    bool blockIllegalMoves = false;
    bool flipBoard = false;
    //string debugLog = string.Empty;
    //public string DebugLog { get { return debugLog; } }
    public SimpleNN Network { get { return network; } }
    public NNAgent(int[] networkShape, bool blockIllegalMoves = false)
    {
        network = new SimpleNN(networkShape);
        this.blockIllegalMoves = blockIllegalMoves;
    }
    public NNAgent(int[] networkShape, float mutationChance, float mutationAmount, bool blockIllegalMoves = false)
    {
        network = new SimpleNN(networkShape);
        this.blockIllegalMoves = blockIllegalMoves;
        network.Mutate(mutationChance, mutationAmount);
    }
    public NNAgent(SimpleNN neuralNetwork, bool blockIllegalMoves = false)
    {
        network = neuralNetwork;
        this.blockIllegalMoves = blockIllegalMoves;
    }

    public int MakeMove(int[] input)
    {
        float[] float_input = new float[input.Length];

        //convert input to floats
        for (int i = 0; i < float_input.Length; i++)
            float_input[i] = (float)input[i];

        //flip input values so that on the board:
        //this agents' moves    = 1
        //their opponents moves = -1

        //debugLog = "BOARD STATE b4 flip:\n";
        //foreach (float boardSlot in float_input)
        //{
        //    debugLog += "[" + boardSlot.ToString() + "] ";
        //}
        //debugLog += "\n";

        if (flipBoard)
        {
            for (int i = 0; i < float_input.Length; i++)
            {
                float_input[i] *= -1;
            }
        }

        //debugLog += "BOARD STATE:\n";
        //foreach (float boardSlot in float_input)
        //{
        //    debugLog += "[" + boardSlot.ToString() + "] ";
        //}
        //debugLog += "\n Flipped board: " + (flipBoard ? "yes" : "no") + "\n";

        float[] outputs = network.Brain(float_input);

        int bestMoveIndex = -1;
        float highestValue = float.MinValue;
        for(int i = 0; i < outputs.Length; i++)
        {
            if (blockIllegalMoves && input[i] != 0) //if this tile is not empty
                continue;                           //don't consider this move

            //find move with highest value in outputs
            if (outputs[i] > highestValue)
            {
                highestValue = outputs[i];
                bestMoveIndex = i;
            }
        }


        //debugLog += "making move: " + bestMoveIndex.ToString() + "\n";
        //UnityEngine.Debug.Log(debugLog);

        if (bestMoveIndex < 0 || input[bestMoveIndex] != 0) //no valid moves
            return -1;



        return bestMoveIndex;
    }
    public void SetFlipBoard(bool goingFirst) { this.flipBoard = goingFirst; }
    public void SetBlockIllegalMoves(bool blockIllegalMoves) { this.blockIllegalMoves = blockIllegalMoves; }

    public NNAgent Clone()
    {
        //return deep copy
        return new NNAgent(network.Clone(), blockIllegalMoves);
    }
}