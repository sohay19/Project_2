using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchVertical : MonoBehaviour
{
    private void Awake()
    {
        Application.runInBackground = true;
        //��׶��� ����
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //��ũ����������
        Screen.orientation = ScreenOrientation.Portrait;
        //���θ��

        StartCoroutine(LoadCamera());
    }


    IEnumerator LoadCamera()
    {
        yield return new WaitForSeconds(1.0f);

        Screen.SetResolution(1080, 1920, false);
    }
}
