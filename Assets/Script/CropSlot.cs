using UnityEngine;
using UnityEngine.UI;

public class CropSlot : MonoBehaviour
{
    public Image fruitImage;
    public GameObject checkMark;
    public GameObject errorMark;

    public void SetSlot(Sprite sprite)
    {
        fruitImage.sprite = sprite;
        checkMark.SetActive(false);
        errorMark.SetActive(false);
        fruitImage.color = Color.white;
    }

    public void ShowResult(bool isCorrect)
    {
        if (isCorrect) checkMark.SetActive(true);
        else errorMark.SetActive(true);
        
        // 결과 표시 후 이미지를 약간 어둡게 하여 '처리 완료' 느낌 전달
        fruitImage.color = new Color(1, 1, 1, 0.4f);
    }
}