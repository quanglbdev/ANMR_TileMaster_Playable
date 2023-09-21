using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Random = UnityEngine.Random;

public class StarGroup : MonoBehaviour
{
    public List<Image> listStars = new List<Image>();
    public Sprite spriteStar_Normal;
    public Sprite spriteStar_Dark;

    public Image imgProgress;

    private int _currStar;

    private const float COMBO_TIMER = 5f;

    private int _currentCombo;

    public int CurrentCombo
    {
        get => _currentCombo;
        set
        {
            _currentCombo = value;
            coefficientCombo = _coefficientComboMap[_currentCombo];
        }
    }

    private readonly Dictionary<int, float> _coefficientComboMap =
        new() { { 1, 1f }, { 2, 1.1f }, { 3, 1.3f }, { 4, 1.6f }, { 5, 2f } };

    public float coefficientCombo;
    private float _timer;
    private bool _isCounting;

    [Header("Combo")] [SerializeField] private TextMeshProUGUI txtCombo;
    public Image comboProgress;

    public void StartGame(int limitScore, Config.LEVEL_DIFFICULTY difficulty)
    {
        ResetCombo();
        _isCounting = true;
        RestartCombo();
        this.limitScore = limitScore;
        this.LevelDifficulty = difficulty;

        _currStar = 0;
        foreach (var star in listStars)
        {
            star.sprite = spriteStar_Dark;
        }
    }

    private void Update()
    {
        if (Config.gameState != Config.GAME_STATE.PLAYING) return;
        if (Config.CheckTutorial_Match3()) return;
        if (Config.CheckTutorial_Undo()) return;
        if (Config.CheckTutorial_Suggest()) return;
        if (Config.CheckTutorial_Shuffle()) return;

        if (!_isCounting || CurrentCombo <= 1) return;
        _timer += Time.deltaTime;
        UpdateCombo();
        if (!(_timer >= COMBO_TIMER)) return;
        _timer = 0f;
        DecreaseCombo();
    }

    public void IncreaseCombo()
    {
        if (CurrentCombo < 5)
            CurrentCombo += 1;
        _timer = 0;
        txtCombo.text = $"Combo: x{coefficientCombo}";
        _isCounting = true;
    }

    private void DecreaseCombo()
    {
        CurrentCombo = 1;
        txtCombo.text = $"Combo: x{coefficientCombo}";
    }

    private void ResetCombo()
    {
        CurrentCombo = 1;
        txtCombo.text = $"Combo: x{coefficientCombo}";
        _isCounting = false;
        _timer = COMBO_TIMER;
    }

    private ConfigLevelGame configLevelGame;

    public void InitStarGroup(ConfigLevelGame _configLevelGame)
    {
        configLevelGame = _configLevelGame;
        // for (var i = 0; i < listStars.Count; i++)
        // {
        //     listStars[i].rectTransform.anchoredPosition =
        //         new Vector2(-286.5f + configLevelGame.listScrore_Stars[i] / 100f * 573f, 0f);
        // }
    }

    public void Revive_InitStarGroup()
    {
        ResetCombo();
    }

    private float countDownTimeA;

    private float _score;

    public float Score
    {
        get => _score;
        set
        {
            _score = value;
            UpdateScore();
        }
    }

    private Config.LEVEL_DIFFICULTY LevelDifficulty
    {
        get => _levelDifficulty;
        set
        {
            _levelDifficulty = value;
            UpdateStarScore();
        }
    }

    private void UpdateStarScore()
    {
    }

    public int limitScore;
    private Config.LEVEL_DIFFICULTY _levelDifficulty;

    private void UpdateScore()
    {
        float args;

        if (_currStar == 0)
        {
            args = limitScore * configLevelGame.listScrore_Stars[0] / 100;
            imgProgress.fillAmount = (Score / args) * .2f;
        }
        else if (_currStar == 1)
        {
            args = limitScore * configLevelGame.listScrore_Stars[1] / 100;
            imgProgress.fillAmount = (Score / args) * .5f;
        }
        else if (_currStar >= 2)
        {
            args = limitScore * configLevelGame.listScrore_Stars[2] / 100;
            imgProgress.fillAmount = (Score / args) * .8f;
        }

        UpdateStar();
    }

    private void UpdateCombo()
    {
        comboProgress.fillAmount = (COMBO_TIMER - _timer) / COMBO_TIMER;
    }

    private void RestartCombo()
    {
        comboProgress.fillAmount = 0f;
    }


    private void UpdateStar()
    {
        if (_currStar >= 3) return;
        if (_currStar >= configLevelGame.listScrore_Stars.Count - 1)
            _currStar = configLevelGame.listScrore_Stars.Count - 1;
        var milestoneScoreByStar = limitScore / 100f * configLevelGame.listScrore_Stars[_currStar];
        if (Mathf.CeilToInt(Score) >= milestoneScoreByStar)
        {
            ChangeStar();
        }
    }

    private void ChangeStar()
    {
        if (_currStar >= 3) return;
        var sequenceChangeStar = DOTween.Sequence();
        sequenceChangeStar.Insert(0f, listStars[_currStar].transform.DOScale(0f, 0.1f).SetEase(Ease.OutQuad));
        sequenceChangeStar.Insert(0f,
            listStars[_currStar].transform.DORotate(new Vector3(0f, 0f, Random.Range(90f, 180f)), 0.1f)
                .SetEase(Ease.OutQuad));
        sequenceChangeStar.InsertCallback(0.1f, () =>
        {
            if (_currStar > 2) _currStar = 2;
            listStars[_currStar].sprite = spriteStar_Normal;
            listStars[_currStar].SetNativeSize();
        });
        sequenceChangeStar.Insert(0.15f,
            listStars[_currStar].transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.OutBounce));
        sequenceChangeStar.Insert(0.15f, listStars[_currStar].transform.DOScale(1f, 0.2f).SetEase(Ease.OutBounce));
        sequenceChangeStar.OnComplete(() => { _currStar += 1; });
    }

    public int GetCurrStar()
    {
        return _currStar > 3 ? 3 : _currStar;
    }
}