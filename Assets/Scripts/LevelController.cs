using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{
    public Tower Tower;

    public static LevelController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Tower = StartGame();
    }

    private Tower StartGame()
    {
        Tower tower = TowerCreator.BuildTower(GameManager.Instance.GetGameSettings(), "Tower");
        return tower;
    }

    public void DeactivateBlocks()
    {
        int activeBlocks = GameManager.Instance.GetGameSettings().ActiveBlocks;
        float waitTime = 0;
        for (var i = 0; i < Tower.Circles.Count - activeBlocks - 1; i++)
        {
            StartCoroutine(WaitBeforeExecute(Tower.Circles[i].DeactivateBlocks, waitTime));
            waitTime += 0.05f;
        }
        waitTime += 0.05f;
        for (var i = Tower.Circles.Count - activeBlocks - 1; i < Tower.Circles.Count; i++)
        {
            StartCoroutine(WaitBeforeExecute(Tower.Circles[i].ActivateBlocks, waitTime));
            waitTime += 0.05f;
        }
    }

    public void CheckTowerHeightState(Tower tower)
    {
        if (tower == null) return;
        int activeBlocks = GameManager.Instance.GetGameSettings().ActiveBlocks;
        int currentHighest = tower.HighestCircle.CircleIndex;
        for (var i = 0; i < tower.Circles.Count; i++)
        {
            if (i < currentHighest - activeBlocks)
            {
                tower.Circles[i].DeactivateBlocks();
                continue;
            }
            else
            {
                tower.Circles[i].ActivateBlocks();
            }
        }
        CameraController.Instance.CheckCameraHeight(tower);
    }

    public void GameProgressCheck(Tower tower)
    {
        if (tower == null) return;
        float percent = (1 - ((float)tower.GetCurrentNumberOfBlocks() / (float)tower.TotalBlocks)) * 100;
        percent /= Const.PERCENTAGE_TO_WIN;
        percent *= 100;
        if(percent >= 100)
        {
            percent = 100;
            WinGame();
        }
        UIManager.Instance.UpdatePercentText(percent);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void WinGame()
    {
        UIManager.Instance.OpenLoseMenu();
    }

    private IEnumerator WaitBeforeExecute(UnityAction job, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        job.Invoke();
    }


}
