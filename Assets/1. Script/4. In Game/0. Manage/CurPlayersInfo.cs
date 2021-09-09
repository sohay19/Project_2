using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurPlayersInfo : MonoBehaviourPunCallbacks
{
    public static CurPlayersInfo instance;

    public GameObject prefabPlayer;
    public GameObject uiPlayer;
    public GameObject content;

    Image imgReady;
    Text txtTurnNum;
    Image imgProfile;
    Text txtNick;
    Text txtCoin;


    public static CurPlayersInfo Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        instance = this;
        CreatePlayerPrefab();
        //����䰡 �޸� ��ü �÷��̾� ��ü����
    }
    void Start()
    {
        UpdatePlayerUI();
    }


    #region �����ݹ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerUI();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerUI();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        UpdatePlayerUI();
    }
    #endregion


    #region ����� �Լ�
    void CreatePlayerPrefab()
    {
        if (Save.CurPlayerPrefab == null)
        {
            Save.CurPlayerPrefab = PhotonNetwork.Instantiate(nameof(prefabPlayer), new Vector3(0, 0, 0), Quaternion.identity, 0);
            Save.CurPhotonView = Save.CurPlayerPrefab.GetComponent<PhotonView>();
        }
    }
    public void UpdatePlayerUI()
    {
        for(int i = 0; i <content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }

        int turn = 0;
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject tmpObject = Instantiate(uiPlayer);
            tmpObject.transform.SetParent(content.transform);
            //UI����

            imgReady = tmpObject.GetComponent<PlayerCellUI>().imgReady;
            txtTurnNum = tmpObject.GetComponent<PlayerCellUI>().imgTurn.GetComponentInChildren<Text>();
            imgProfile = tmpObject.GetComponent<PlayerCellUI>().imgPlayerProfile;
            txtNick = tmpObject.GetComponent<PlayerCellUI>().txtNinkName;
            txtCoin = tmpObject.GetComponent<PlayerCellUI>().txtCoin;
            //UI ����

            ExitGames.Client.Photon.Hashtable hash = player.CustomProperties;
            //Ŀ����������Ƽ �޾ƿ���
            if (hash[enumType.playerKeysList[(int)enumType.playerKey.Ready]].ToString() == true.ToString())
            {
                imgReady.color = new Color32(255, 0, 0, 200);
                if(player.NickName == PhotonNetwork.MasterClient.NickName) //�����ϰ��
                {
                    Text txtGo = imgReady.GetComponentInChildren<Text>();
                    txtGo.text = "Start";
                }
                //����ǥ��
            }
            else
            {
                imgReady.color = new Color32(25, 5, 84, 200);
            }
            //Ready ����
            turn++;
            txtTurnNum.text = turn.ToString();
            //����
            if (player.NickName == PhotonNetwork.LocalPlayer.NickName) //�ڱ��ڽ��϶�
            {
                txtNick.color = Color.yellow;
            }
            txtNick.text = player.NickName;
            //�г���
            imgProfile.material = Resources.Load<Material>(hash[enumType.playerKeysList[(int)enumType.playerKey.Level]].ToString());
            //�����ʻ���
            txtCoin.text = hash[enumType.playerKeysList[(int)enumType.playerKey.Coin]].ToString().ToString();
            //����

            if (PhotonNetwork.CurrentRoom.CustomProperties[enumType.roomKeysList[(int)enumType.roomKey.isPlaying]].ToString() == true.ToString())
            {
                Text txtGo = imgReady.GetComponentInChildren<Text>();
                txtGo.text = "Go";
            }
            //���� ���� ���
        }
    }
    #endregion
}
