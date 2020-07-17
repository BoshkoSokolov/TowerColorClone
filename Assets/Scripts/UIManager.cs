using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TMP_Text PercentText;
    public TMP_Text BallsLeftText;
    public Image LosePanel;
    public Animator RestartButtonAnimator;
    public Slider ProgressBarSlider;

    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdatePercentText(float percent)
    {
        PercentText.text = percent.ToString("F0") + "%";
        ProgressBarSlider.value = percent / 100;
    }



    public void UpdateBallsLeftUI(int ballsLeft)
    {
        BallsLeftText.text = ballsLeft.ToString();
    }

    public void StartGame()
    {
        BallsLeftText.gameObject.SetActive(true);
        GameManager.Instance.StartGame();
    }

    public void RestartGame()
    {
        LevelController.Instance.RestartLevel();
    }

    public void OpenLoseMenu()
    {
        LosePanel.gameObject.SetActive(true);
        RestartButtonAnimator.SetTrigger("Open");
    }

    public void OpenProgressBar()
    {
        ProgressBarSlider.GetComponent<Animator>().enabled = true;
    }

}
