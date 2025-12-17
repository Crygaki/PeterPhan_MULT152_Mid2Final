[System.Serializable]
public class GameData
{
    public int dataVersion = 1; // increment when schema changes

    public DifficultyMode selectedDifficulty;

    // High scores
    public int highScoreEasy;
    public int highScoreHard;
    public int highScoreExtreme;

    // Best multipliers (all-time record, not session)
    public int bestMultiplierEasy;
    public int bestMultiplierHard;
    public int bestMultiplierExtreme;
}