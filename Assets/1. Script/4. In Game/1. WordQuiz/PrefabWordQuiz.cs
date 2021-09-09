using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class PrefabWordQuiz : MonoBehaviour, IPunObservable
{
    public static PrefabWordQuiz instance;

    public Text txtQuiz;
    public Text txtTime;

    Coroutine showNextQuizCoroutine;

    bool isRight;
    int curQuizNum;
    //���� ���� ��ȣ
    int sameQuizNum;
    //���� ���� ī��Ʈ
    int curTime = 30;
    //Ÿ�̸�


    #region ������Ƽ
    public int CurQuizNum
    {
        get
        {
            return curQuizNum;
        }
        set
        {
            curQuizNum = value;
        }
    }
    public bool IsRight
    {
        get
        {
            return isRight;
        }
        set
        {
            isRight = value;
        }
    }
    public static PrefabWordQuiz Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        curQuizNum = 1;
        sameQuizNum = 0;
        isRight = false;

        GameObject tmpObject = GameObject.Find("Canvas").transform.Find("Panel_Game").gameObject;
        transform.SetParent(tmpObject.transform.GetChild(0));
        //Panel_Game�� �ڽ��� Tab_WordQuiz�ΰ���

        RectTransform rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, 0);
        //������ ����
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(txtQuiz.text);
            stream.SendNext(txtTime.text);
        }
        else
        {
            txtQuiz.text = (string)stream.ReceiveNext();
            txtTime.text = (string)stream.ReceiveNext();
        }
    }


    #region ���� �Լ�
    public void ShowQuiz(int num)
    {
        WordQuizRun.instance.PrefabMap.CheckCell(false);

        curQuizNum = num;

        if (GameManager.Instance.ViewList[0].IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.SetQuizNum), RpcTarget.Others, curQuizNum);
        }

        showNextQuizCoroutine = StartCoroutine(ShowNextQuiz());
    }
    IEnumerator ShowNextQuiz()
    {
        Text txtNum = WordQuizRun.Instance.PrefabMap.TxtQuizNum;
        txtNum.text = curQuizNum + "��° �����Դϴ�";

        WordQuizRun.Instance.PrefabMap.TabQuizNumInfo.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        WordQuizRun.Instance.PrefabMap.TabQuizNumInfo.SetActive(false);
        //���� �����ֱ�

        txtQuiz = WordQuizRun.Instance.TabQuiz.transform.GetChild(0).GetComponent<Text>();

        int index = WordQuizRun.Instance.PrefabMap.WordQuizList.FindIndex(x => x.numQuiz == curQuizNum);

        txtQuiz.text = "Quiz. " + curQuizNum + "\n";
        txtQuiz.text += WordQuizRun.Instance.PrefabMap.WordQuizList[index].quizString;

        WordQuizRun.instance.PrefabMap.CheckCell(true);
        //������ ��ġ ���

        curTime = 30;
        txtTime = WordQuizRun.Instance.TabQuiz.transform.GetChild(1).GetComponentInChildren<Text>();

        if (GameManager.Instance.ViewList[0].IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.PlayTimerSound), RpcTarget.All);
        }
        while (curTime >= 0)
        {
            txtTime.text = curTime.ToString();

            if (isRight)
            {
                break;
            }
            if(curTime == 5)
            {
                if (GameManager.Instance.ViewList[0].IsMine)
                {
                    Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.PlayTimerSpeedSound), RpcTarget.All);
                }
            }

            yield return new WaitForSecondsRealtime(1.0f);

            curTime--;
        }

        curTime = 0;
        txtTime.text = curTime.ToString();

        if (GameManager.Instance.ViewList[0].IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.StopTimerSound), RpcTarget.All);
        }

        WordQuizRun.instance.PrefabMap.CheckCell(false);

        FindNextQuiz();

        if (PhotonNetwork.IsMasterClient && curQuizNum != 0)
        {
            WordQuizRun.Instance.PrefabQuiz.ShowQuiz(curQuizNum);
        }

        yield return null;
    }
    void FindNextQuiz()
    {
        int preQuizNum = 0;
        int nextQuizNum = 0;
        for (int i = 1; i <= WordQuizRun.Instance.PrefabMap.WordQuizList.Count; i++)
        {
            if (WordQuizRun.Instance.PrefabMap.CorrectQuizHash[i].ToString() == false.ToString() && curQuizNum < i)
            {
                if (nextQuizNum == 0 || nextQuizNum > i)
                {
                    nextQuizNum = i;
                }
            }
            else if (WordQuizRun.Instance.PrefabMap.CorrectQuizHash[i].ToString() == false.ToString() && curQuizNum > i)
            {
                if (preQuizNum == 0 || preQuizNum > i)
                {
                    preQuizNum = i;
                }
            }
        }
        //��Ǭ���� ã��

        isRight = false;
        //���� ���� ����
        if (nextQuizNum != 0)
        {
            curQuizNum = nextQuizNum;
        }
        else if (preQuizNum != 0)
        {
            curQuizNum = preQuizNum;
        }
        else
        //���̻� ���� ����
        {
            if (WordQuizRun.Instance.PrefabMap.CorrectQuizHash[curQuizNum].ToString() == false.ToString())
            //���� ������ �������� ��� �ѹ� ��
            {
                if (sameQuizNum != curQuizNum)
                {
                    sameQuizNum = curQuizNum;
                }
                else
                //�̹� �ٽ� �� �������ٸ� ���� ����
                {
                    if (GameManager.Instance.ViewList[0].IsMine)
                    {
                        Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.SetEndGame), RpcTarget.All);
                    }
                }

            }
            else
            //���� ������ ���� ������ ���� ����
            {
                if (GameManager.Instance.ViewList[0].IsMine)
                {
                    Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.SetEndGame), RpcTarget.All);
                }
            }
        }
    }
    #endregion


    #region ����� �Լ�
    public void StopCoroutineFunc()
    {
        if (GameManager.Instance.ViewList[0].IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.StopTimerSound), RpcTarget.All);
        }

        WordQuizRun.instance.PrefabMap.CheckCell(false);

        txtQuiz.text = "";
        txtTime.text = "0";

        if (showNextQuizCoroutine != null)
        {
            StopCoroutine(showNextQuizCoroutine);
        }
    }
    #endregion
}
