using System;
using System.Diagnostics;
using System.Linq;
public interface Agent
{
    public int MakeMove(int[] input);
    public void SetGoingFirst(bool goingFirst);
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
    public void SetGoingFirst(bool goingFirst) { }
}

public class NNAgent : Agent
{
    SimpleNN network;
    bool blockIllegalMoves = false;
    bool goingFirst = false;
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
        if (!goingFirst)
        {
            for (int i = 0; i < float_input.Length; i++)
            {
                float_input[i] *= -1;
            }
        }

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

        if (bestMoveIndex < 0 || input[bestMoveIndex] != 0) //no valid moves
            return -1;

        return bestMoveIndex;
    }
    public void SetGoingFirst(bool goingFirst) { this.goingFirst = goingFirst; }

    public NNAgent Clone()
    {
        //return deep copy
        return new NNAgent(network.Clone(), blockIllegalMoves);
    }
}