using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class BingoRun : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{
    public static BingoRun instance;

    public GameObject bingoChoiceMap;
    public GameObject bingoMap;
    public GameObject bingoPanel;
    public GameObject turnPlay;

    GameObject myBingoChoiceMap;
    GameObject myBingoMap;
    GameObject myBingoPanel;
    
    Hashtable mapHash;

    PrefabBingoChoiceMap prefabChoiceMap;
    PrefabBingoMap prefabMap;
    PrefabBingoPanel prefabPanel;


    #region ������Ƽ
    public static BingoRun Instance
    {
        get
        {
            return instance;
        }
    }
    public PrefabBingoChoiceMap PrefabChoiceMap
    {
        get
        {
            return prefabChoiceMap;
        }
    }
    public PrefabBingoMap PrefabMap
    {
        set
        {
            prefabMap = value;
        }
        get
        {
            return prefabMap;
        }
    }
    public PrefabBingoPanel PrefabPanel
    {
        get
        {
            return prefabPanel;
        }
    }
    public Hashtable MapHash
    {
        get
        {
            return mapHash;
        }
    }
    #endregion


    private void Awake()
    {
        instance = this;


        mapHash = new Hashtable();

        myBingoChoiceMap = Instantiate(bingoChoiceMap);
        prefabChoiceMap = myBingoChoiceMap.GetComponent<PrefabBingoChoiceMap>();
        //���õ� ��ȣ ��
        myBingoMap = PhotonNetwork.Instantiate(nameof(bingoMap), new Vector3(0, 0, 0), Quaternion.identity, 0);
        prefabMap = myBingoMap.GetComponent<PrefabBingoMap>();
        mapHash.Add(PhotonNetwork.LocalPlayer.NickName, myBingoMap.GetPhotonView().ViewID);
        //�� ������
    }
    void Start()
    {
        GameManager.Instance.ViewList.Add(myBingoMap.GetPhotonView());
        //�� �����

        myBingoPanel = null;
        prefabPanel = null;

        if (PhotonNetwork.IsMasterClient)
        {
            myBingoPanel = PhotonNetwork.InstantiateRoomObject(nameof(bingoPanel), new Vector3(0, 0, 0), Quaternion.identity);
            prefabPanel = myBingoPanel.GetComponent<PrefabBingoPanel>();
            //�г� ���� ����
            GameManager.Instance.ViewList.Add(myBingoPanel.GetPhotonView());
            //�г� �����

            turnPlay = PhotonNetwork.InstantiateRoomObject(nameof(turnPlay), new Vector3(0, 0, 0), Quaternion.identity, 0);
            GameManager.Instance.ViewList.Add(turnPlay.GetPhotonView());
            //���� �ѱ�� ���� �����

            if(PhotonNetwork.PlayerList.Length > 1)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.GiveMap), RpcTarget.Others);
                    //���� ��û
                }
            }
        }
    }
    void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest) { }
    void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer) { }
    void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView == GameManager.Instance.ViewList[2] && GameManager.Instance.ViewList[2].IsMine)
        {
            prefabChoiceMap.StartTimer();
        }
        //�����̸� Ÿ�̸� ����
    }


    #region ���� �ݹ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(GameManager.Instance.ViewList[2].IsMine)
        {
            for(int i = 0; i < GameStart.Instance.PlayerTrunList.Count; i++)
            {
                if (otherPlayer == GameStart.Instance.PlayerTrunList[i])
                {
                    int index = i + 1;

                    if (index == GameStart.Instance.PlayerTrunList.Count)
                    {
                        index = 0;
                    }

                    if (GameStart.Instance.PlayerTrunList[index] == PhotonNetwork.LocalPlayer)
                    //�����̸� Ÿ�̸� ����
                    {
                        prefabChoiceMap.StartTimer();
                    }
                    else
                    //���� �Ͽ��� �ѱ�
                    {
                        GameManager.Instance.ViewList[2].TransferOwnership(GameStart.Instance.PlayerTrunList[index]);
                    }
                }
            }
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            if (Save.CurPhotonView.IsMine)
            {
                Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.DeleteMap), RpcTarget.All, otherPlayer.NickName);
            }
        }
        //������ ����� ã�Ƽ� �ؽ� ����
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.GiveMap), RpcTarget.Others);
            //���� ��û
        }
    }
    #endregion


    #region ����� �Լ�
    public void MakeMapList(string name, int viewID)
    {
        if (mapHash.ContainsKey(name) == false)
        {
            mapHash.Add(name, viewID);
        }
        //���� ���� �� �߰�

        if (Save.CurPhotonView.IsMine)
        {
            List<int> tmpList = new List<int>();
            foreach(PhotonView tmpView in GameManager.Instance.ViewList)
            {
                tmpList.Add(tmpView.ViewID);
            }
            tmpList.RemoveAt(0);
            //�� ���� ���� ���� ����

            Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.SetView), RpcTarget.Others, (object)tmpList.ToArray());
            //PhotonViewList ����
        }
        //viewList�� ����� �ֵ��� ��

        string[] strArr = new string[mapHash.Count];
        int[] intArr = new int[mapHash.Count];

        mapHash.Keys.CopyTo(strArr, 0);
        mapHash.Values.CopyTo(intArr, 0);

        Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.SetMap), RpcTarget.Others, strArr, intArr);
        //�߰��� �÷��̾� ����� ����
        SetOtherPanel();
        //�г� �� ����
    }
    public void SetViewList(object viewidArr)
    {
        int[] tmpArray = viewidArr as int[];

        foreach(int tmp in tmpArray)
        {
            GameManager.Instance.ViewList.Add(PhotonView.Find(tmp));
            //����Ʈ�� �߰�
        }

        myBingoPanel = GameManager.Instance.ViewList[1].gameObject;
        prefabPanel = myBingoPanel.GetComponent<PrefabBingoPanel>();
        //�гμ���
    }
    public void SetMapHash(object nameArr, object viewidArr)
    {
        string[] tmpNameArr = nameArr as string[];
        int[] tmpViewArr = viewidArr as int[];

        for (int i = 0; i < tmpNameArr.Length; i++)
        {
            if (mapHash.ContainsKey(tmpNameArr[i]) == false)
            {
                mapHash.Add(tmpNameArr[i], tmpViewArr[i]);
            }
            //���̺� �������� �߰�
        }

        SetOtherPanel();
        //�г� �� ����
    }
    public void DeleteMapHash(string name)
    {
        prefabPanel.PanelMapHash.Remove(name);
        //�г����� �� ����
        mapHash.Remove(name);
        //���� �÷��̾��� �� ����
        prefabPanel.SetButton();
        //�г� ��ư ������Ʈ
    }
    public void SetOtherPanel()
    {
        foreach (DictionaryEntry entry in mapHash)
        {
            if (!PhotonView.Find((int)entry.Value).IsMine)
            //���� �ƴϸ� ��� �г� ��������
            {
                PhotonView.Find((int)entry.Value).transform.SetParent(myBingoPanel.transform.GetChild(0));
                PrefabBingoMap prefabBingoMap = PhotonView.Find((int)entry.Value).GetComponent<PrefabBingoMap>();
                prefabBingoMap.SetScale();
                //��ġ�ű��

                if (!prefabPanel.PanelMapHash.ContainsKey(entry.Key))
                {
                    prefabPanel.PanelMapHash.Add(entry.Key, PhotonView.Find((int)entry.Value).gameObject);
                }
            }
        }

        prefabPanel.SetButton();
        //��ư�� �г��� ǥ��
        prefabPanel.SetClose();
        //�г� �� ���� ����
    }
    public void StartBingo()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.MakeBingoMap), RpcTarget.All);
            //������ ä���
        }
    }
    #endregion


    #region ��ư �Լ�
    public void OnClickBingo()
    {
        if (BingoCheck.Instance.BingoCounter == 5 && Save.CurPhotonView.IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.SetEndGame), RpcTarget.All);
            //���� 5�� �ϼ� ����
        }
    }
    #endregion
}
