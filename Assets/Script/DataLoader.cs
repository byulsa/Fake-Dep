using UnityEngine;
using UnityEngine.SceneManagement;

public class DataLoader : MonoBehaviour
{
    public static DataLoader Instance;
    public GameMode SelectMode;
    public int Score = 0;
    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 이 오브젝트가 유지되도록 설정
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetGameMode(GameMode mode)
    {
        SelectMode = mode;
        SceneManager.LoadScene(1); 
    }
    public GameMode GetGameMode()
    {
        return SelectMode;
    }
    public void ScoreData(int score)
    {
        Score = score;
    }
}
