using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class Lobby : MonoBehaviourPunCallbacks
{
    public static Lobby instance;

    public GameObject myInfoMenu;

    List<RoomInfo> roomAllList;
    //���� �渮��Ʈ
    int roomCounter;
    //���ȣ


    #region ������Ƽ
    public List<RoomInfo> RoomList
    {
        get
        {
            return roomAllList;
        }
    }
    public int RoomCounter
    {
        get
        {
            return roomCounter;
        }
    }
    public static Lobby Instance
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
        roomAllList = new List<RoomInfo>();
        roomCounter = 1;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby(new TypedLobby("MainLobby", LobbyType.Default));
        }
    }


    #region Photon CallBack
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(new TypedLobby("MainLobby", LobbyType.Default));
    }
    public override void OnJoinedLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            Save.Instance.ChangeLevel();
            UpdateProfile();
        }
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomList.Count != 0)
        //����O
        {
            UpdataRoom(roomList);
        }
        //�� ���� �� ����

        if (roomAllList.Count == 0)
        {
            PrintRoom.Instance.RoomPrint(1);
        }
        else
        {
            PrintRoom.Instance.RoomPrint(PrintRoom.Instance.CurrentPage);
        }
    }
    #endregion


    #region ����� �Լ�
    void UpdataRoom(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            int index = roomAllList.FindIndex(x => x.Name == room.Name);

            if (room.PlayerCount != 0)
            //����� �ִ� ��
            {
                if (index == -1)
                //����Ʈ�� �� ����(�߰�)
                {
                    roomAllList.Add(room);
                }
                else
                //����Ʈ�� ������(������Ʈ)
                {
                    roomAllList[index] = room;
                }
            }
            else
            //����ִ¹�
            {
                if (index != -1)
                //����Ʈ�� ������ ����
                {
                    roomAllList.RemoveAt(index);
                    //���� �� ����Ʈ���� ����
                }
            }
        }
        //�� ����

        roomAllList = roomAllList.OrderBy(x => x.CustomProperties[enumType.roomKeysList[0]]).ToList();
        //�� ����

        roomCounter = 1;
        for (int i = 0; i < roomAllList.Count; i++)
        {
            if (roomCounter == int.Parse(roomAllList[i].Name))
            {
                roomCounter++;
            }
        }
        //���ȣ ����
    }
    public void UpdateProfile()
    {
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

        myInfoMenu.transform.GetChild(2).GetComponent<Text>().text = hash[enumType.playerKeysList[(int)enumType.playerKey.NickName]].ToString();
        //�г���
        myInfoMenu.transform.GetChild(1).GetComponent<Image>().material = Resources.Load<Material>(hash[enumType.playerKeysList[(int)enumType.playerKey.Level]].ToString());
        //�÷��̾� �̹���
        int index = enumType.levelKeysList.FindIndex(x => x == hash[enumType.playerKeysList[(int)enumType.playerKey.Level]].ToString());
        myInfoMenu.transform.GetChild(4).GetComponent<Text>().text = enumType.levelNameKeysList[index].ToString();
        //����
        myInfoMenu.transform.GetChild(6).GetComponent<Text>().text = hash[enumType.playerKeysList[(int)enumType.playerKey.Coin]].ToString();
        //����
    }
    #endregion
}
