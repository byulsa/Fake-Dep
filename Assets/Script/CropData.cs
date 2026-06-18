using UnityEngine;

[CreateAssetMenu(fileName = "NewCropData", menuName = "Game/Crop Data")]
public class CropData : ScriptableObject
{
    public string nameKR; 
    public string nameEN; 
    public Sprite image;    
    public int difficulty = 1; 
    
    [TextArea(3, 5)]
    public string information;

    // 현재 문제로 사용할 언어와 정답을 반환
    public string GetRandomName(out bool isEnglish)
    {
        isEnglish = Random.Range(0, 2) == 0;
        return isEnglish ? nameEN : nameKR;
    }
}