[System.Serializable]
public class ModeInfo
{
    public string modeName;
    public int[] costs; // 단계별 가격: {25, 35, 45, 70}
    public int currentLevel = 0;
    public int maxLevel;
}