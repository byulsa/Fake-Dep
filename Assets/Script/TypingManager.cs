using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Easy,
    Hard,
    Export
}
public class TypingManager : MonoBehaviour
{
    public GameMode currentMode;
    public TMP_InputField inputField;
    public AudioSource audioSource;
    public AudioClip[] clips;
    public TextMeshProUGUI placeholderText;
    public Image fruitDisplay;
    public TextMeshProUGUI scoreText;
    public GameObject[] heartIcons;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI explanationText;

    public TextMeshProUGUI timerText;
    private float limitTime = 10f;
    private float currentTime;
    private bool isTimerRunning = false;

    private string targetAnswer;
    public CropSlot[] previewSlots;

    private CropData currentCrop;
    private int score;
    private int hearts = 3;

    private int currentIdxInList = 0;
    private int completedListsInDay = 0;

    public Color normalColor = Color.white;
    public Color errorColor = Color.red;

    public GameObject ScoreEffect;
    public Animator MovingBelt;
    public Animator PlayerAnimator;

    void Start()
    {
        completedListsInDay = 0;
        hearts = 3;
        explanationText.text = "";
        inputField.onSubmit.AddListener(OnSubmitInput);
        inputField.onValueChanged.AddListener(CheckTypingError);

        StartNewList();
        UpdateUI();
        ResetHearts();

        if (DataLoader.Instance != null)
        {
            currentMode = DataLoader.Instance.GetGameMode();
            UpdatePlaceholder();
        }
    }

    void Update()
    {
        FocusInputField();
        if (isTimerRunning)
        {
            HandleTimer();
        }
    }

    void StartNewList()
    {
        currentIdxInList = 0;
        currentTime = limitTime;
        isTimerRunning = true;

        CropManager.Instance.GenerateNewRound();
        InitSlotUI();
        SetNextCrop();
    }

    void HandleTimer()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = 0;
            isTimerRunning = false;
            OnTimeOver();
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.color = (currentTime <= 10f) ? Color.red : Color.white;
    }

    void OnTimeOver()
    {
        hearts--;
        UpdateHeartUI();
        explanationText.text = "시간 초과! 다음 박스로 넘어갑니다.";
        PlayerAnimator.SetTrigger("Sad");

        if (hearts <= 0)
        {
            GameOver();
            return;
        }

        Invoke("ClearExplanation", 1.5f);
        completedListsInDay++;
        CheckDayProgress();
        Invoke("StartNewList", 1.0f);
    }

    // 수정된 입력 판정 로직
    void OnSubmitInput(string input)
    {
        // 인덱스 범위 초과 방지
        if (currentIdxInList >= previewSlots.Length || string.IsNullOrEmpty(input)) return;

        int PlusScore = 100;
        float TimeExText = 1.5f;

        // 한글 이름 혹은 영어 이름 중 하나라도 일치하면 정답
        bool isCorrect = (input == currentCrop.nameKR || input == currentCrop.nameEN);

        previewSlots[currentIdxInList].ShowResult(isCorrect);

        if (isCorrect)
        {
            PlayerAnimator.SetTrigger("Happy");
            audioSource.PlayOneShot(clips[0]);
            score += PlusScore;
            explanationText.text = currentCrop.information;
            ScoreEffect.GetComponent<TextMeshProUGUI>().text = PlusScore.ToString();
            ScoreEffect.GetComponent<Animator>().Play("Show");
            TimeExText = 3f;
        }
        else
        {
            PlayerAnimator.SetTrigger("Sad");
            explanationText.text = explanation();
            audioSource.PlayOneShot(clips[1]);
            hearts--;
            UpdateHeartUI();

            if (hearts <= 0)
            {
                GameOver();
                return;
            }
        }
        Invoke("ClearExplanation", TimeExText);

        currentIdxInList++;
        MovingBelt.Play("Moving");
        UpdateUI();

        if (currentIdxInList >= 5)
        {
            isTimerRunning = false;
            completedListsInDay++;
            CheckDayProgress();
            inputField.text = ""; // 인덱스 오류 방지를 위해 텍스트 비움
            Invoke("StartNewList", 0.5f);
        }
        else
        {
            SetNextCrop();
        }
    }

    void CheckDayProgress()
    {
        // 25개(5개 리스트) 완료 시 다음 날로 변경
        if (completedListsInDay >= 5)
        {
            CropManager.Instance.currentDay++;
            completedListsInDay = 0;
            limitTime = 10;
            ResetHearts();
        }
    }

    void InitSlotUI()
    {
        var crops = CropManager.Instance.GetCurrentQueue();
        for (int i = 0; i < previewSlots.Length; i++)
        {
            if (i < crops.Count)
            {
                previewSlots[i].gameObject.SetActive(true);
                previewSlots[i].SetSlot(crops[i].image);
            }
            else previewSlots[i].gameObject.SetActive(false);
        }
    }

    void SetNextCrop()
    {
        currentCrop = CropManager.Instance.GetCurrentQueue()[currentIdxInList];
        if (fruitDisplay != null) fruitDisplay.sprite = currentCrop.image;
        inputField.text = "";
        inputField.textComponent.color = normalColor;

        // 정답 힌트 설정 (Easy/Export 모드용)
        bool isEnglish;
        targetAnswer = currentCrop.GetRandomName(out isEnglish);

        UpdatePlaceholder();
        UpdateIndicator();
        FocusInputField();
    }

    void UpdateHeartUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
            heartIcons[i].SetActive(i < hearts);
    }

    void ResetHearts()
    {
        hearts = 3;
        UpdateHeartUI();
    }

    void UpdateIndicator()
    {
        for (int i = 0; i < previewSlots.Length; i++)
        {
            if (previewSlots[i].gameObject.activeSelf)
            {
                float alpha = (i == currentIdxInList) ? 1.0f : 0.4f;
                previewSlots[i].fruitImage.color = new Color(1, 1, 1, alpha);
            }
        }
    }

    void CheckTypingError(string input)
    {
        // Hard 모드에서는 실시간 에러 체크를 비활성화하거나 보이지 않게 처리
        if (currentMode == GameMode.Hard || string.IsNullOrEmpty(input))
        {
            inputField.textComponent.color = normalColor;
            return;
        }

        string targetPart = targetAnswer.Length >= input.Length ? targetAnswer.Substring(0, input.Length) : targetAnswer;
        inputField.textComponent.color = (input != targetPart) ? errorColor : normalColor;
    }

    void UpdateUI()
    {
        scoreText.text = $"Score: {score}";
        dayText.text = $"Day {CropManager.Instance.currentDay}";
    }

    void UpdatePlaceholder()
    {
        if (currentMode == GameMode.Easy) placeholderText.text = targetAnswer;
        else if (currentMode == GameMode.Export) placeholderText.text = IsKorean(targetAnswer) ? "한글로 입력..." : "Enter in English...";
        else placeholderText.text = "???"; // Hard 모드에서는 힌트를 숨김
    }

    void GameOver()
    {
        isTimerRunning = false;
        if (DataLoader.Instance != null) DataLoader.Instance.ScoreData(score);
        SceneManager.LoadScene(2);
    }

    void FocusInputField()
    {
        inputField.ActivateInputField();
        inputField.Select();
    }

    bool IsKorean(string text) => System.Text.RegularExpressions.Regex.IsMatch(text, @"[가-힣]");

    string explanation()
    {
        int random = Random.Range(0, 3);
        return random switch { 0 => "그럴 수 있죠!", 1 => "열심히 해봐요!.", 2 => "일부로는 아니죠?", _ => "" };
    }

    void ClearExplanation() => explanationText.text = "";
}