using TMPro;
using UnityEngine;

public class SceneLod : MonoBehaviour
{
    public TextMeshProUGUI Score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(DataLoader.Instance != null && Score != null)
        {
            Score.text = "총 점수 : " + DataLoader.Instance.Score;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
