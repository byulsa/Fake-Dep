using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject SelectDifficultyPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Quit() => Application.Quit();

    public void StartGame()
    {
        MainMenuPanel.SetActive(false);
        SelectDifficultyPanel.SetActive(true);
    }

    public void SetGameMode(int mode)
    {
        DataLoader.Instance.SetGameMode((GameMode)mode);
    }
}
