using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Controll : MonoBehaviour
{
    public static Controll instance;

    public Camera camera;
    public ARSessionOrigin sessionOrigin;
    public GameObject tabProcessPopUp;
    public GameObject prefabArrow;

    GameObject targetObject;
    GameObject arrowObject;

    Ray ray;
    RaycastHit hit;


    #region ������Ƽ
    public static Controll Instance
    {
        get
        {
            return instance;
        }
    }
    public GameObject TargetObject
    {
        get
        {
            return targetObject;
        }
        set
        {
            targetObject = value;
        }
    }
    public GameObject ArrowObject
    {
        get
        {
            return arrowObject;
        }
        set
        {
            arrowObject = value;
        }
    }
    #endregion


    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (Input.touchCount > 0)
        //Input.touchCount = ��ġ�� �հ��� ��
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            //��ġ�Ѽ���
            {
                if (!EventSystem.current.IsPointerOverGameObject(0))
                //���� �κ��� UI�� �ƴ�
                {
                    ray = camera.ScreenPointToRay(Input.GetTouch(0).position);

                    if (ARGameManager.Instance.IsFindStart)
                    {
                        if (Physics.Raycast(ray, out hit, LayerMask.NameToLayer("Find")))
                        {
                            ParticleSystem particle = hit.transform.GetComponentInChildren<ParticleSystem>();
                            particle.Play();
                            ARGameManager.Instance.ResultFind();
                        }
                        else
                        {
                            ARGameManager.Instance.CheckFind();
                        }
                    }

                    if (Physics.Raycast(ray, out hit) && Map.Instance.IsMade)
                    //ray�� ���� ��ü�� ����
                    {
                        if (targetObject == null)
                        {
                            targetObject = hit.transform.gameObject;
                            //Ÿ��

                            arrowObject = Instantiate(prefabArrow, Vector3.zero, prefabArrow.transform.rotation);
                            arrowObject.transform.SetParent(targetObject.transform);
                            arrowObject.transform.localScale = prefabArrow.transform.localScale;
                            arrowObject.transform.localPosition = prefabArrow.transform.localPosition;
                            //���� �� ��ġ ����
                            sessionOrigin.MakeContentAppearAt(targetObject.transform, hit.transform.position, hit.transform.rotation);
                            //Ÿ�� ����

                            if (ARGameManager.Instance.IsSwapEnd)
                            {
                                ARGameManager.Instance.ResultSwap();
                            }
                            //Swap����� ��Ȯ��

                        }
                        else if (targetObject != hit.transform.gameObject)
                        {
                            targetObject = hit.transform.gameObject;
                            //Ÿ�� ����
                            arrowObject.transform.SetParent(targetObject.transform);
                            //��ġ ����
                            sessionOrigin.MakeContentAppearAt(targetObject.transform, hit.transform.position, hit.transform.rotation);
                            //Ÿ�� ����
                        }
                        else
                        {
                            Destroy(arrowObject);
                            targetObject = null;
                            //����
                        }
                    }
                }
            }
        }
    }


    #region OnChanged
    public void OnChangedSize(float scale)
    {
        if (targetObject != null)
        {
            float value = 20 * scale;

            if (value < 0.005f)
            {
                value = 0.1f;
            }
            sessionOrigin.transform.localScale = new Vector3(value, value, value);
        }
        else
        {
            PopUp.instance.ShowTabProcess(tabProcessPopUp, "���õ��� �ʾҽ��ϴ�", 2.0f);
        }
    }
    #endregion
}
