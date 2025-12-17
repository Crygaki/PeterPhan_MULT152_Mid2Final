[System.Serializable]
public class GameData
{
    public int dataVersion = 1; // increment when you change the schema

    public DifficultyMode selectedDifficulty;

    // High scores
    public int highScoreEasy;
    public int highScoreHard;
    public int highScoreExtreme;

    // Score multipliers (earned by player)
    public int multiplierEasy;
    public int multiplierHard;
    public int multiplierExtreme;
}
