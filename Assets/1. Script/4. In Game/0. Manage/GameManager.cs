using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public GameObject tabTurnMaster;

    List<PhotonView> viewList = new List<PhotonView>();
    Player nextPlayer;


    public List<PhotonView> ViewList
    {
        get
        {
            return viewList;
        }
        set
        {
            viewList = value;
        }
    }
    public static GameManager Instance
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
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.IsMessageQueueRunning = true;

        if (PhotonNetwork.IsMasterClient)
        {
            UpdatPlayerProperties(PhotonNetwork.LocalPlayer, true);
        }
        else
        {
            UpdatPlayerProperties(PhotonNetwork.LocalPlayer, false);
        }
    }


    #region ���� �ݹ�
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string masterNick = PhotonNetwork.MasterClient.NickName;
            GameManager.Instance.UpdatPlayerProperties(PhotonNetwork.LocalPlayer, true);
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.TurnMaster), RpcTarget.All, masterNick);
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.UpdatPlayerProperties(otherPlayer, false);

            if (PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKey.isPlaying.ToString()].ToString() == true.ToString())
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                {
                    Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.SetEndGame), RpcTarget.All);
                }
            }
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged[enumType.roomKey.isPlaying.ToString()] != null)
        {
            if (propertiesThatChanged[enumType.roomKey.isPlaying.ToString()].ToString() == true.ToString())
            //�÷��̷� ����
            {
                if (PhotonNetwork.IsMasterClient && Save.CurPhotonView.IsMine)
                {
                    Save.CurPhotonView.RPC(nameof(PrefabPlayer.Instance.SetGameMode), RpcTarget.All);
                }
            }
            else
            //�÷��� ����
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GameEnd.Instance.SetEndMode();
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
        }
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(Save.CurPlayerPrefab);
    }
    #endregion


    #region ����� �Լ�
    public void UpdatRoomProperties(bool isStart)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(enumType.roomKeysList[(int)enumType.roomKey.isPlaying], isStart.ToString());

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        //�� ���� ����
    }
    public void UpdatPlayerProperties(Player player, bool isReady)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Ready], isReady.ToString());

        player.SetCustomProperties(hash);
        //�÷��̾� ���� ����
    }
    public void TurnMasterClient(string nickName)
    {
        Text txtMsg = tabTurnMaster.transform.GetComponentInChildren<Text>();
        txtMsg.color = Color.white;

        txtMsg.text = "\"" + nickName + "\"������" + "\n";
        txtMsg.text += "������ ��ü�Ǿ����ϴ�";

        StartCoroutine(ShowMessage());
    }
    IEnumerator ShowMessage()
    {
        tabTurnMaster.SetActive(true);

        yield return new WaitForSecondsRealtime(3.0f);

        tabTurnMaster.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            TurnAfterMaster();
        }
    }
    void TurnAfterMaster()
    {
        if(!GameEnd.Instance.IsEnd)
        {
            switch (int.Parse(PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKey.gameType.ToString()].ToString()))
            {
                case 1:
                    WordQuizRun.Instance.PrefabQuiz.StopCoroutineFunc();
                    WordQuizRun.Instance.PrefabQuiz.ShowQuiz(WordQuizRun.Instance.PrefabQuiz.CurQuizNum);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
    }
    #endregion


    #region ��ư �̺�Ʈ �Լ�
    public void OnClickExitRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            foreach (Player curPlayer in PhotonNetwork.PlayerList)
            {
                if (curPlayer.NickName == PhotonNetwork.LocalPlayer.NickName)
                {
                    nextPlayer = curPlayer.GetNext();
                }
            }
            PhotonNetwork.SetMasterClient(nextPlayer);
            //������ �ѱ��

            if (PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKey.isPlaying.ToString()].ToString() == true.ToString())
            {
                switch (int.Parse(PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKey.gameType.ToString()].ToString()))
                {
                    case 1:
                        foreach (PhotonView curView in viewList)
                        {
                            curView.TransferOwnership(nextPlayer);
                        }
                        //�����ǰ� ������ �����ǳѱ��
                        WordQuizRun.Instance.PrefabQuiz.StopCoroutineFunc();
                        break;
                    case 2:
                        viewList[1].TransferOwnership(nextPlayer);
                        //�����г� ����� �ѱ��
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
            }
            //���� �� ���� ����
        }

        PhotonNetwork.LeaveRoom();
    }
    #endregion
}
