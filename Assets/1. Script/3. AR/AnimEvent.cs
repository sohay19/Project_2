using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    public GameObject boxCover;

    Animation anim;


    void Start()
    {
        anim = transform.GetComponent<Animation>();
    }


    #region �ִϸ��̼� �̺�Ʈ
    void OpenBox()
    {
        StartCoroutine(Opening());
    }
    void CloseBox()
    {
        anim.Stop();
    }
    IEnumerator Opening()
    {
        yield return new WaitForSeconds(1.0f);

        anim.Play("CloseBox");
    }
    #endregion
}
