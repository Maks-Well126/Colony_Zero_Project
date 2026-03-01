using UnityEngine;


public class Save : MonoBehaviour
{
    [ContextMenu("Reset All Saves")]
    public void ResetAllSavesFromInspector()
    {
        ResetAllStates();        
    }
    private const string LEVEL1_BUILT_KEY = "ShipUpgrade_Level1Built";
    private const string LEVEL2_BUILT_KEY = "ShipUpgrade_Level2Built";

    public static void SaveLevel1State(bool isBuilt)
    {
        PlayerPrefs.SetInt(LEVEL1_BUILT_KEY, isBuilt ? 1 : 0);
        PlayerPrefs.Save();        
    }

    public static void SaveLevel2State(bool isBuilt)
    {
        PlayerPrefs.SetInt(LEVEL2_BUILT_KEY, isBuilt ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool LoadLevel1State()
    {
        if (PlayerPrefs.HasKey(LEVEL1_BUILT_KEY))
        {
            int value = PlayerPrefs.GetInt(LEVEL1_BUILT_KEY);
            return value == 1;
        }
        return false; 
    }

    public static bool LoadLevel2State()
    {
        if (PlayerPrefs.HasKey(LEVEL2_BUILT_KEY))
        {
            int value = PlayerPrefs.GetInt(LEVEL2_BUILT_KEY);
            return value == 1;
        }
        return false; 
    }

    public static void LoadAllStates(out bool level1Built, out bool level2Built)
    {
        level1Built = LoadLevel1State();
        level2Built = LoadLevel2State();

    }

    public static void ResetAllStates()
    {
        PlayerPrefs.DeleteKey(LEVEL1_BUILT_KEY);
        PlayerPrefs.DeleteKey(LEVEL2_BUILT_KEY);
        PlayerPrefs.Save();
    }

    public static bool HasLevel1Save()
    {
        return PlayerPrefs.HasKey(LEVEL1_BUILT_KEY);
    }

    public static bool HasLevel2Save()
    {
        return PlayerPrefs.HasKey(LEVEL2_BUILT_KEY);
    }

}
