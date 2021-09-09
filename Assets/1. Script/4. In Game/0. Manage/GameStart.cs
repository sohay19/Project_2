using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.UI;

public class GameStart : MonoBehaviourPunCallbacks
{
    public static GameStart instance;

    public GameObject btnReady;
    public GameObject btnNotReady;
    public GameObject tabClosePopUp;
    public GameObject tabProcessPopUp;

    List<Player> playerTrunList;


    public static GameStart Instance
    {
        get
        {
            return instance;
        }
    }
    public List<Player> PlayerTrunList
    {
        get
        {
            return playerTrunList;
        }
    }


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        btnReady.SetActive(true);
        btnNotReady.SetActive(false);

        playerTrunList = new List<Player>();

        if (PhotonNetwork.IsMasterClient)
        {
            Text txtReady = btnReady.GetComponentInChildren<Text>();
            txtReady.text = "Start";
        }
    }


    #region �����ݹ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Text txtReady = btnReady.GetComponentInChildren<Text>();
            txtReady.text = "Start";
        }
        //������ Ŭ���̾�Ʈ�� ��ư �ؽ�Ʈ ����
    }
    #endregion


    #region ����� �Լ�
    public void SetGame()
    {
        CurPlayersInfo.Instance.UpdatePlayerUI();
        //Ready���� Go�� ����
        btnReady.SetActive(false);
        btnNotReady.SetActive(false);
        //ReadyŰ ���ֱ�

        int curCoin = int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Coin.ToString()].ToString());
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Coin], (curCoin - 100).ToString());
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //���� ������ ���� ����

        GameEnd.Instance.PlayerScoreHash.Clear();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameEnd.Instance.PlayerScoreHash.Add(player.NickName, 0);
            playerTrunList.Add(player);
        }
        GameEnd.Instance.TotalCoin = 100 * PhotonNetwork.PlayerList.Length;
        //�÷��̾� �� ���ھ� + ��ü���� ����

        if (PhotonNetwork.IsMasterClient)
        {
            switch (int.Parse(PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKey.gameType.ToString()].ToString()))
            {
                case 1:
                    WordQuizRun.Instance.PrefabQuiz.ShowQuiz(WordQuizRun.Instance.PrefabQuiz.CurQuizNum);
                    break;
                case 2:
                    BingoRun.Instance.StartBingo();
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
    }
    #endregion


    #region ��ư �̺�Ʈ
    public void OnClickReady()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.PlayerList.Length > 1)
            {
                int readyPlayerCount = 0; ;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (player.CustomProperties[enumType.playerKeysList[(int)enumType.playerKey.Ready]].ToString() == true.ToString())
                    {
                        readyPlayerCount++;
                    }
                }
                if (readyPlayerCount == PhotonNetwork.PlayerList.Length)
                {
                    GameManager.Instance.UpdatRoomProperties(true);
                }
                else
                {
                    PopUp.Instance.ShowTabClose(tabClosePopUp, "�غ����� ����\n����ڰ� �����մϴ�");
                }
            }
            else
            //�÷��� �ο� ����
            {
                PopUp.Instance.ShowTabClose(tabClosePopUp, "�÷��� �ο���\n�����մϴ�");
            }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Ready.ToString()].ToString() == false.ToString())
            {
                btnReady.SetActive(false);
                btnNotReady.SetActive(true);

                GameManager.Instance.UpdatPlayerProperties(PhotonNetwork.LocalPlayer, true);
            }
            else
            {
                btnNotReady.SetActive(false);
                btnReady.SetActive(true);

                GameManager.Instance.UpdatPlayerProperties(PhotonNetwork.LocalPlayer, false);
            }
        }
    }
    #endregion
}
