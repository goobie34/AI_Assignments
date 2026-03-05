using System;

public class TRAINING_SIMULATION_DESC
{
    public int n_trainees;
    public int n_generations;
    public int n_survivors;
    public int n_gamesPerGeneration;
    public float mutationAmount;
    public float mutationChance;

    public TRAINING_SIMULATION_DESC(
        int n_trainees = 50,
        int n_generations = 100,
        int n_survivors = 2,
        int n_gamesPerGeneration = 5,
        float mutationAmount = 0.2f,
        float mutationChance = 0.2f)
    {
        this.n_trainees = n_trainees;
        this.n_generations = n_generations;
        this.n_survivors = n_survivors;
        this.n_gamesPerGeneration = n_gamesPerGeneration;
        this.mutationAmount = mutationAmount;
        this.mutationChance = mutationChance;
    }

    public override string ToString()
    {
        string output = "";
        output += "Num of trainees: " + n_trainees + "\n";
        output += "Num of generations: " + n_generations + "\n";
        output += "Num of survivors per gen: " + n_survivors + "\n";
        output += "Num of games per gen: " + n_gamesPerGeneration + "\n";
        output += "Mutation chance: " + mutationChance + "\n";
        output += "Mutation amount: " + mutationAmount + "\n";
        return output;
    }
}
public enum SimulationResult
{
    Win = 30,
    Lose = -40,
    Draw = 10,
    IllegalMove = -100
}
public class TrainingSimulator
{
    SimpleNN[] trainees;
    int[] fitnessArray;
    GameSimulator gameSimulator;
    Agent opponent;
    int[] networkShape;

    int n_trainees, n_generations, n_survivors, n_gamesPerGeneration;
    float mutationAmount, mutationChance;

    string log = "--- Evolution Log: --- \n";
    public string Log { get { return log; } }
    public TrainingSimulator(int[] networkShape, Agent opponent, TRAINING_SIMULATION_DESC desc = null, SimpleNN sourceTrainee = null)
    {
        if (desc == null)
            desc = new TRAINING_SIMULATION_DESC();

        this.networkShape = networkShape;
        this.opponent = opponent;

        //set training parameters from description
        this.n_trainees = desc.n_trainees;
        this.n_generations = desc.n_generations;
        this.n_survivors = desc.n_survivors;
        this.mutationAmount = desc.mutationAmount;
        this.mutationChance = desc.mutationChance;
        this.n_gamesPerGeneration = desc.n_gamesPerGeneration;

        //init arrays
        fitnessArray = new int[n_trainees];
        trainees = new SimpleNN[n_trainees];

        //populate trainees
        for (int i = 0; i < trainees.Length; i++)
            trainees[i] = sourceTrainee == null ? new SimpleNN(networkShape) : sourceTrainee.Clone();

        log += desc.ToString();
    }

    public SimpleNN Run()
    {
        MutateAll(mutationChance, mutationAmount);

        int gen_count = 0;
        while(gen_count < n_generations)
        {
            log += "-------------- Generation: " + gen_count + " -------------- \n";
            CalculateFitness();
            SimpleNN[] topPerformers = GetTopPerformers(n_survivors);
            //LogTopPerformers(topPerformers);
            Repopulate(topPerformers);
            MutateChildren(mutationChance, mutationAmount);
            gen_count++;
        }

        return GetTopPerformers(1)[0];
    }

    private void CalculateFitness()
    {
        for (int i = 0; i < trainees.Length; i++)
        {
            fitnessArray[i] = 0;
            NNAgent traineeAgent = new NNAgent(trainees[i]);

            for (int j = 0; j < n_gamesPerGeneration; j++)
            {
                gameSimulator = new GameSimulator(traineeAgent, opponent, i % 2 == 0);
                SimulationResult result = gameSimulator.Run();

                switch (result)
                {
                    case SimulationResult.Win:
                        {
                            fitnessArray[i] += (int)result;
                            fitnessArray[i] -= gameSimulator.NumOfLegalTraineeMoves; //higher score the faster the win
                            break;
                        }
                    case SimulationResult.Draw:
                        {
                            fitnessArray[i] += (int)result;
                            break;
                        }
                    default:
                        {
                            fitnessArray[i] += (int)result;
                            fitnessArray[i] += gameSimulator.NumOfLegalTraineeMoves; //the more legal moves made before losing or doing an illegal move, the better
                            break;
                        }
                }
            }
        }

        LogTraineeFitness();
    }

    private SimpleNN[] GetTopPerformers(int n)
    {
        SimpleNN[] topPerformers = new SimpleNN[n];

        //sort trainees by fitness, lowest --> highest
        Array.Sort(fitnessArray, trainees);

        //get top n performers from highest --> lowest
        for (int i = 0; i < n; i++)
        {
            topPerformers[i] = trainees[trainees.Length - 1 - i]; //starting from the back of trainees array
            topPerformers[i].Fitness = fitnessArray[fitnessArray.Length - 1 - i];
        }

        return topPerformers;
    }

    private void Repopulate(SimpleNN[] survivors)
    {
        //fills population with clones of survivors
        for(int i = 0; i < trainees.Length; i++)
        {
            trainees[i] = survivors[i % n_survivors].Clone(); 
        }
    }

    private void MutateAll(float chance, float amount)
    {
        for (int i = 0; i < trainees.Length; i++)
        {
            trainees[i].Mutate(chance, amount);
        }
    }
    private void MutateChildren(float chance, float amount)
    {
        //i starts at n_survivors so that survivors pass on to next generation without mutation
        for (int i = n_survivors; i < trainees.Length; i++)
        {
            trainees[i].Mutate(chance, amount);
        }
    }

    private void LogTraineeFitness()
    {
        int bestFitnessScore = int.MinValue;
        int bestFitnessID = -1;
        int totalFitness = 0;
        float averageFitness;

        for(int i = 0;i < trainees.Length;i++)
        {
            totalFitness += fitnessArray[i];

            if (fitnessArray[i] > bestFitnessScore)
            {
                bestFitnessScore=fitnessArray[i];
                bestFitnessID = trainees[i].ID;
            }
        }

        averageFitness = (float)totalFitness / (float)trainees.Length;
        log += "Average fitness: " + averageFitness + "\n";
        log += "Highest fitness: " + bestFitnessScore + " ==> by trainee with ID: " + bestFitnessID + "\n";

        ////Logs every single trainee
        //for(int i = 0;i < trainees.Length;i++)
        //{
        //    log += "- Trainee ID: " + trainees[i].ID + "==> Fitness: " + fitnessArray[i] + "\n";
        //}
    }
    private void LogTopPerformers(SimpleNN[] topPerformers) 
    {
        log += "Survivors: " + n_survivors + "\n";

        for (int i = 0; i < topPerformers.Length;i++)
        {
            log += "- Survivor ID: " + topPerformers[i].ID + " ==> With Fitness: " + topPerformers[i].Fitness + "\n";
        }
    }
}