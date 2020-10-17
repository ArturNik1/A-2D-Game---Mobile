using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementLogic : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] Sprite fullStar;
    [SerializeField] Image[] stars;
    [SerializeField] Text title;
    [SerializeField] Text description;
    [SerializeField] Text prize;
    [SerializeField] Slider progressBar;
    [SerializeField] Image fillProgressBar;
    [SerializeField] Image blurProgressBar;
    [SerializeField] Sprite[] progressSprites;
    [SerializeField] GameObject collectButton;
    [SerializeField] GameObject finishedOverlay;
#pragma warning restore CS0649

    public int _current;
    public int[] _target;
    public int _star;
    public string _title;
    public int[] _prize;
    public bool _finished;

    public AchievementInfo info;

    public void FillAchievement(int current, int[] target, int star, string title, int[] prize, bool finished) {
        this._current = current;
        this._target = target;
        this._star = star;
        this._title = title;
        this._prize = prize;
        this._finished = finished;

        ChangeValues();
    }

    public void ChangeValues() { 
        // Fill gold stars...
        for (int i = 0; i < _star; i++) {
            stars[i].sprite = fullStar;
        }

        // Change title...
        title.text = _title;

        // Change description...
        description.text = _current.ToString("n0") + "/" + _target[_star].ToString("n0");

        // Change prize amount for current star level...
        prize.text = _prize[_star].ToString("n0");

        // Progress bar...
        progressBar.value = (float) _current / (float)_target[_star];
        if (_star <= 1) {
            fillProgressBar.sprite = progressSprites[0];
            blurProgressBar.sprite = progressSprites[1];
        }
        else if (_star <= 3) {
            fillProgressBar.sprite = progressSprites[2];
            blurProgressBar.sprite = progressSprites[3];
        }
        else {
            fillProgressBar.sprite = progressSprites[4];
            blurProgressBar.sprite = progressSprites[5];
        }

        // Should collect button be activated...?
        if (_current == _target[_star] && !_finished)
            collectButton.SetActive(true);

        // Should finished overlay be activated...?
        finishedOverlay.SetActive(_finished);
    }

    public float GetProgressBarValue() {
        if (_finished) return 0;
        return progressBar.value;
    }

    public bool IsCollectButtonActive() {
        return collectButton.activeSelf;
    }

    public void ClickCollect() {
        if (_star == 5) _finished = true;
        else if (_star < 5) {
            _star++;
            info.IncreaseStar();
        }
        ChangeValues();
        AchievementHandler.SaveAchievements();
    }

}

