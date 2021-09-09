using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchHorizon : MonoBehaviour
{
    private void Awake()
    {
        Application.runInBackground = true;
        //��׶��� ����
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //��ũ����������
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        //���θ��

        StartCoroutine(LoadCamera());
    }


    IEnumerator LoadCamera()
    {
        yield return new WaitForSeconds(1.0f);

        Screen.SetResolution(1920, 1080, false);
    }
}
