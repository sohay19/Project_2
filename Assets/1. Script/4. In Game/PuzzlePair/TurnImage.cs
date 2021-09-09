using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnImage : MonoBehaviour
{
    public static TurnImage instance;

    public GameObject content;
    public Canvas canvas;

    GraphicRaycaster ray;
    PointerEventData eventData;
    List<Flag> compareFlag;

    bool isturn;


    public static TurnImage Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        ray = canvas.GetComponent<GraphicRaycaster>();
        eventData = new PointerEventData(null);

        compareFlag = new List<Flag>();

        isturn = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isturn)
        {
            eventData.position = Input.mousePosition;
            //���콺 �������� ����

            List<RaycastResult> result = new List<RaycastResult>();
            ray.Raycast(eventData, result);
            //���̸� �̿��Ͽ� eventData�� ����


            if (result.Count > 0)
            {
                if (result[0].gameObject.layer == LayerMask.NameToLayer("Puzzle"))
                {
                    Image tmpImage = result[0].gameObject.transform.GetComponent<Image>();

                    Turn(tmpImage);

                    compareFlag.Add(new Flag(tmpImage.sprite.name, tmpImage));
                    //���� ����߰�
                }
            }
        }

        /*
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                eventData.position = Input.GetTouch(0).position;
                //��ġ �������� ����

                List<RaycastResult> result = new List<RaycastResult>();
                ray.Raycast(eventData, result);
                //���̸� �̿��Ͽ� eventData�� ����

                Debug.Log("result ī��Ʈ�� ���� ��������");

                if (result.Count > 0)
                {
                    Debug.Log("���� ������Ʈ�̸� = " + result[0].gameObject.name);


                    if (result[0].gameObject.layer == LayerMask.NameToLayer("Puzzle"))
                    {
                        Turn(result[0].gameObject.transform.GetComponent<Image>());
                    }
                }
            }
        }
        */
    }


    public void SetIsTurn(bool tmpBool)
    {
        isturn = tmpBool;
    }
    void Turn(Image curImage)
    {
        isturn = true;

        if (curImage.color == Color.black)
        //�޸��϶�
        {
            curImage.transform.GetComponent<Animation>().Play("TurnFront");
        }
        else if (curImage.color == Color.white)
        //�ո��϶�
        {
            curImage.transform.GetComponent<Animation>().Play("TurnBack");
        }
    }
    public void ComparePuzzle()
    {
        if (compareFlag.Count == 2)
        {
            if (compareFlag[0].GetFlagSprite() == compareFlag[1].GetFlagSprite())
            //�������
            {

            }
            else
            //�ٸ����
            {
                Turn(compareFlag[0].GetFlagImage());
                Turn(compareFlag[1].GetFlagImage());
                //�ٽ� ������
            }

            compareFlag.Clear();
            //��ߺ񱳸���Ʈ ����
        }
    }
}
