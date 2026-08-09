using UnityEngine;

//single access point to every config value = ConfigManager.instance.config.GetBoatCost(2)
public class ConfigManager : MonoBehaviour
{
    public static ConfigManager instance;
    public GameConfig config; //drag a GameConfig asset here, or it builds a default one

    void Awake()
    {
        //standard singleton so any script can reach the config
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        //use the asset in Resources/Configs if one exists, otherwise fall back to defaults
        if (config == null)
        {
            GameConfig loaded = Resources.Load<GameConfig>("Configs/GameConfig");
            config = loaded != null ? loaded : ScriptableObject.CreateInstance<GameConfig>();
        }
    }
}
