using System;
using UnityEngine;

public class EvolutionSim : MonoBehaviour
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

    Agent opponent;
    SimpleNN sourceNeuralNet;

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
        SimpleNN winner = trainingSim.Run();

        //Save neuralnet winner and evolution log to file
        JSONHandler.Write(winner.ToJson(), outputFolderPath + "/Evo NN " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
        JSONHandler.Write(trainingSim.Log, outputFolderPath + "/Evo Log " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt");
    }
}
