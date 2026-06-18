using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance;
    public List<CropData> allCropList; // 전체 작물 데이터

    // 현재 리스트(5개)를 저장할 변수
    private List<CropData> currentRoundCrops = new List<CropData>();
    private List<CropData> lastRoundCrops = new List<CropData>(); // 중복 방지용

    public int currentDay = 1;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 첫 리스트 생성
        GenerateNewRound();
    }

    // 질문하신 부분: 5개 과일 세트를 새로 생성하는 핵심 로직
    public void GenerateNewRound()
    {
        // 1. 현재 일차(currentDay)보다 난이도가 낮거나 같은 과일만 필터링
        var availableCrops = allCropList
            .Where(c => c.difficulty <= currentDay)
            .Except(lastRoundCrops) // 직전 리스트에 나왔던 과일 제외
            .ToList();

        // 2. 만약 뽑을 과일이 부족하면 해당 난이도 전체에서 다시 필터링
        if (availableCrops.Count < 5)
        {
            availableCrops = allCropList.Where(c => c.difficulty <= currentDay).ToList();
        }

        // 3. 무작위 셔플 후 딱 5개만 추출
        currentRoundCrops = availableCrops.OrderBy(x => Random.value).Take(5).ToList();
        
        // 4. 이번에 뽑힌 과일을 기록하여 다음 번 중복 방지[cite: 1]
        lastRoundCrops = new List<CropData>(currentRoundCrops);
    }

    // TypingManager가 하단 슬롯을 그릴 때 사용하는 함수[cite: 1]
    public List<CropData> GetCurrentQueue()
    {
        return currentRoundCrops;
    }

    // TypingManager에서 인덱스로 접근할 수 있게 리스트 반환[cite: 1]
    public List<CropData> GetCurrentDayList() 
    {
        return currentRoundCrops;
    }
}