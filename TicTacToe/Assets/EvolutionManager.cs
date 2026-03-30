using System;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    TrainingSimulator trainingSim;
    [SerializeField] int[] networkShape;
    [SerializeField] string outputFolderPath;
    [SerializeField] string sourceNeuralNetFileName;
    //[SerializeField] string opponentNNFileName;

    [HeaderAttribute("Evolution Parameters:")]
    [SerializeField] int n_trainees;
    [SerializeField] int n_generations;
    [SerializeField] int n_survivors;
    [SerializeField] int n_gamesPerGeneration;
    [SerializeField] float mutationAmount;
    [SerializeField] float mutationChance;

    [HeaderAttribute("Testing parameters:")]
    [SerializeField] bool runTestInsteadOfEvolution;
    [SerializeField] int numOfTestGames;
    [SerializeField] string testNeuralNetFileName;


    Agent opponent;
    SimpleNN sourceNeuralNet;
    SimpleNN testNeuralNet;

    private void Awake()
    {
        TRAINING_SIMULATION_DESC desc = new TRAINING_SIMULATION_DESC(
            n_trainees,
            n_generations,
            n_survivors,
            n_gamesPerGeneration,
            mutationAmount,
            mutationChance
            );

        if (!string.IsNullOrEmpty(sourceNeuralNetFileName))
            sourceNeuralNet = new SimpleNN(JSONHandler.Read("/NN Evolution" + sourceNeuralNetFileName));
        
        //if (opponentNNFileName != null)
        //    opponent = new NNAgent(new SimpleNN(JSONHandler.Read("/NN Evolution" + opponentNNFileName)));

        opponent = new RandomAgent();
        trainingSim = new TrainingSimulator(networkShape, opponent, desc, sourceNeuralNet);
        
        if (runTestInsteadOfEvolution) //run test INSTEAD of evolution
        {
            if (!string.IsNullOrEmpty(testNeuralNetFileName))
                testNeuralNet = new SimpleNN(JSONHandler.Read("/NN Evolution" + testNeuralNetFileName));

            string testResult = trainingSim.Test(testNeuralNet, numOfTestGames, testNeuralNetFileName);
            JSONHandler.Write(testResult, outputFolderPath + "/NN TEST " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");
        }
        else //run evolution sim
        {
            SimpleNN winner = trainingSim.Run();

            //Save neuralnet winner and evolution log to file
            JSONHandler.Write(winner.ToJson(), outputFolderPath + "/Evo NN " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
            JSONHandler.Write(trainingSim.Log, outputFolderPath + "/Evo Log " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");
        }
    }
}
