using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneStaticHelper : MonoBehaviour
{
    public static IEnumerator WaitBeforeExecute(UnityAction job, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        job.Invoke();
    }

    public static GameObject SpawnObject(Object obj, string name)
    {
        GameObject newGO = new GameObject(name);
        newGO.AddComponent(typeof(object));
        return newGO;
    }
}
