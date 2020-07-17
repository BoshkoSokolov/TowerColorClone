using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private Color[] blockColors;
    private Color previousColor = Color.white;

    private void Awake()
    {
        blockColors = GameManager.Instance.GetGameSettings().BlockColors;
    }

    public Color GetRandomColor()
    {
        if (blockColors.Length == 0) return Color.red;
        Color randomColor = blockColors[Random.Range(0, blockColors.Length)];
        if (randomColor != previousColor && Random.Range(0, 100) < GameManager.Instance.GetGameSettings().BlockColorConsistency)
            randomColor = previousColor;
        previousColor = randomColor;
        return randomColor;
    }

    #region SINGLETON PATTERN
    public static ColorManager _instance;
    public static ColorManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ColorManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("ColorManager");
                    _instance = container.AddComponent<ColorManager>();
                }
            }

            return _instance;
        }
    }
    #endregion
}
