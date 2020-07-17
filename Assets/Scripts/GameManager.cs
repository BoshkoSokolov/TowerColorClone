using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public UnityAction OnUpdate = null;
    public UnityAction OnStartGame = null;

    private GameObject[] persistantObejcts;
    private GameSettings loadedData = null;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        InitilizeMonoBehaviours();
        GetGameSettings();
    }

    public void StartGame()
    {
        OnStartGame?.Invoke();
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }

    private void InitilizeMonoBehaviours()
    {
        persistantObejcts = Resources.LoadAll("PersistantInstantiator", typeof(GameObject)).Cast<GameObject>().ToArray();
        foreach(var item in persistantObejcts)
        {
            Instantiate(item);
        }
    }

    //Get the current GameSettings
    public GameSettings GetGameSettings()
    {
        if (loadedData != null) return loadedData;
        loadedData = Resources.Load<GameSettings>("GameSettings/mainGameSettings");
        return loadedData;
    }
}
