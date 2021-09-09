using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BingoCheck : MonoBehaviour
{
    public static BingoCheck instance;
    public Canvas canvas;

    GraphicRaycaster ray;
    PointerEventData eventData;

    bool isClick;
    int bingoCounter;


    #region ������Ƽ
    public static BingoCheck Instance
    {
        get
        {
            return instance;
        }
    }
    public int BingoCounter
    {
        get
        {
            return bingoCounter;
        }
    }
    #endregion


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        ray = canvas.GetComponent<GraphicRaycaster>();
        eventData = new PointerEventData(null);
        bingoCounter = 0;
        isClick = false;
    }
    void Update()
    {

        if (GameManager.Instance.ViewList.Count > 2)
        {
            if(Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    eventData.position = Input.mousePosition;
                //���콺 �������� ����

                List<RaycastResult> result = new List<RaycastResult>();
                ray.Raycast(eventData, result);
                //���̸� �̿��Ͽ� eventData�� ����

                if (result.Count > 0)
                {
                    if (result[0].gameObject.layer == LayerMask.NameToLayer("Bingo"))
                    {
                        int index = BingoRun.Instance.PrefabMap.CellList.FindIndex(x => x.Name == result[0].gameObject.name);
                        //�ش� �� ã��
                        int num = BingoRun.Instance.PrefabMap.CellList[index].Num;
                        //�ش� ��ȣ ã��

                        if (BingoRun.Instance.PrefabMap.CellList[index].Check != 1)
                        //�� �ʿ��� ������ ���� ��ȣ
                        {
                            if (GameManager.Instance.ViewList[2].IsMine && !isClick && !BingoRun.Instance.PrefabChoiceMap.IsChoice)
                            //�����϶�
                            {
                                CheckMap(result[0].gameObject.name, index, num);

                                if (BingoRun.Instance.PrefabChoiceMap.ChoiceAllNumList.FindIndex(x => x == num) == -1)
                                //���õ��� ���� ��ȣ�� ����Ʈ ���̽����߰�
                                {
                                    isClick = true;

                                    CheckChoiceMap(num);
                                    //ChoiceMap üũ
                                }
                            }
                            else
                            //���ϾƴҶ�
                            {
                                if (BingoRun.Instance.PrefabChoiceMap.ChoiceAllNumList.FindIndex(x => x == num) != -1)
                                //�̹� ���õ� ��ȣ���߸� ����
                                {
                                    CheckMap(result[0].gameObject.name, index, num);
                                }
                            }
                        }
                    }
                }
                }
            }
        }

        /*
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKey.isPlaying.ToString()].ToString() == true.ToString())
        {
            if (Input.GetMouseButtonDown(0))
            {
                eventData.position = Input.mousePosition;
                //���콺 �������� ����

                List<RaycastResult> result = new List<RaycastResult>();
                ray.Raycast(eventData, result);
                //���̸� �̿��Ͽ� eventData�� ����

                if (result.Count > 0)
                {
                    if (result[0].gameObject.layer == LayerMask.NameToLayer("Bingo"))
                    {
                        int index = BingoRun.Instance.PrefabMap.CellList.FindIndex(x => x.Name == result[0].gameObject.name);
                        //�ش� �� ã��
                        int num = BingoRun.Instance.PrefabMap.CellList[index].Num;
                        //�ش� ��ȣ ã��

                        if (BingoRun.Instance.PrefabMap.CellList[index].Check != 1)
                        //�� �ʿ��� ������ ���� ��ȣ
                        {
                            if (GameManager.Instance.ViewList[2].IsMine && !isClick && !BingoRun.Instance.PrefabChoiceMap.IsChoice)
                            //�����϶�
                            {
                                CheckMap(result[0].gameObject.name, index, num);

                                if (BingoRun.Instance.PrefabChoiceMap.ChoiceAllNumList.FindIndex(x => x == num) == -1)
                                //���õ��� ���� ��ȣ�� ����Ʈ ���̽����߰�
                                {
                                    isClick = true;

                                    CheckChoiceMap(num);
                                    //ChoiceMap üũ
                                }
                            }
                            else
                            //���ϾƴҶ�
                            {
                                if (BingoRun.Instance.PrefabChoiceMap.ChoiceAllNumList.FindIndex(x => x == num) != -1)
                                //�̹� ���õ� ��ȣ���߸� ����
                                {
                                    CheckMap(result[0].gameObject.name, index, num);
                                }
                            }
                        }
                    }
                }
            }
        }
        */

    }



    void CheckMap(string name, int index, int num)
    {
        BingoRun.Instance.PrefabMap.CellList[index].CheckUp();
        BingoRun.Instance.PrefabMap.CellList[index].Image.color = Color.gray;

        CheckBingo(name, index, num);
        //���� Ȯ��
    }
    void CheckBingo(string name, int index, int num)
    {
        List<List<BingoClass>> bingoLine = BingoRun.Instance.PrefabMap.pointList(name);
        foreach (List<BingoClass> list in bingoLine)
        {
            int count = 0;

            foreach (BingoClass bingo in list)
            {
                count += bingo.Check;
            }

            if (count == 7)
            {
                int score = int.Parse(GameEnd.Instance.PlayerScoreHash[PhotonNetwork.LocalPlayer.NickName].ToString());
                GameEnd.Instance.PlayerScoreHash[PhotonNetwork.LocalPlayer.NickName] = score + 20;
                //���ھ� ����

                if (Save.CurPhotonView.IsMine)
                {
                    Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.OtherScoreUp), RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName);
                }
                //�ٸ� �÷��̾�� ���ھ� ����

                bingoCounter++;

                Audio.PlayCorrectSound();

                BingoRun.Instance.PrefabMap.InfoBingoCount(bingoCounter);
                //����˸�

                foreach (BingoClass completeBingo in list)
                {
                    int bingoIndex = BingoRun.Instance.PrefabMap.CellList.FindIndex(x => x.Name == completeBingo.Name);

                    completeBingo.Image.color = Color.red;

                    if (Save.CurPhotonView.IsMine)
                    {
                        Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.CheckBingoNum), RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, bingoIndex, completeBingo.Num);
                    }
                    //�ٸ� �÷��̾�� ������ ����
                }
                //���� �� ���� ��ĥ
            }
            else
            {
                PassMyMap(index, num);
                //���ʻ��� �ѱ��
            }
            count = 0;
        }
    }
    public void CheckChoiceMap(int num)
    {
        BingoRun.Instance.PrefabChoiceMap.ChoiceAllNumList.Add(num);
        BingoRun.Instance.PrefabChoiceMap.AddChoiceNum(num);
        //���̽� �� ä���

        if (GameManager.Instance.ViewList[2].IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.PassChoiceNum), RpcTarget.Others, num);
            BingoRun.Instance.PrefabChoiceMap.IsChoice = true;

            isClick = false;
        }
    }
    void PassMyMap(int index, int num)
    {
        if (Save.CurPhotonView.IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.CheckNum), RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, index, num);
        }
        //�ٸ� �÷��̾�� ����
    }
    public void CheckOtherMap(string name, int index, int num)
    {
        GameObject otherMap = PhotonView.Find((int)BingoRun.Instance.MapHash[name]).gameObject;
        PrefabBingoMap tmpMap = otherMap.GetComponent<PrefabBingoMap>();

        tmpMap.content.transform.GetChild(index).GetComponentInChildren<Image>().color = Color.gray;
        tmpMap.content.transform.GetChild(index).GetComponentInChildren<Text>().text = num.ToString();
        //��� ������ ǥ��
    }
    public void CheckBingoMap(string name, int index, int num)
    {
        GameObject otherMap = PhotonView.Find((int)BingoRun.Instance.MapHash[name]).gameObject;
        PrefabBingoMap tmpMap = otherMap.GetComponent<PrefabBingoMap>();

        tmpMap.content.transform.GetChild(index).GetComponentInChildren<Image>().color = Color.red;
        tmpMap.content.transform.GetChild(index).GetComponentInChildren<Text>().text = num.ToString();
        //��� ������ ǥ��
    }
    public void ScoreUp(string name)
    {
        int score = int.Parse(GameEnd.Instance.PlayerScoreHash[name].ToString());
        GameEnd.Instance.PlayerScoreHash[name] = score + 20;
        //�ش� �÷��̾��� ���ھ� ����
    }
}
