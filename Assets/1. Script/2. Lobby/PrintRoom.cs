using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PrintRoom : MonoBehaviour
{
    public static PrintRoom instance;

    public GameObject prefabRoom;
    public GameObject content;
    public Text emptyRoom;
    public Text pageRoom;

    List<GameObject> printRoom;

    Text roomNum;
    Text roomName;
    Text gameType;
    Image roomSecret;
    Image roomPlaying;
    Text playerCount;

    int curPage;
    int totalPage;


    #region
    public int CurrentPage
    {
        get
        {
            return curPage;
        }
    }
    public static PrintRoom Instance
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
        printRoom = new List<GameObject>();
        curPage = 1;
        totalPage = 1;
    }


    #region Room ���� ����� �Լ�
    public void RoomPrint(int page)
    {
        foreach (GameObject roomObject in printRoom)
        {
            Destroy(roomObject);
        }
        //��ü ����

        if(Lobby.Instance.RoomList.Count <= 5)
        {
            totalPage = 1;
        }
        else
        {
            if (Lobby.Instance.RoomList.Count % 5 > 0)
            {
                totalPage = (Lobby.Instance.RoomList.Count / 5) + 1;
            }
            else
            {
                totalPage = (Lobby.Instance.RoomList.Count / 5);
            }
        }
        //��ü ������ ���

        if (page > totalPage)
        {
            page = totalPage;
            curPage = totalPage;
        }
        //�������� �پ��������

        if (Lobby.Instance.RoomList.Count != 0)
        //�븮��Ʈ�� 0�� �ƴҶ�
        {
            emptyRoom.gameObject.SetActive(false);

            if(curPage < totalPage)
            {
                page = 1;
            }

            for (int i = (5 * page) - 5; i < Lobby.Instance.RoomList.Count; i++)
            {
                if (i == 5 * page)
                {
                    break;
                }

                GameObject tmpObject = Instantiate(prefabRoom);
                tmpObject.transform.SetParent(content.transform);

                printRoom.Add(tmpObject);

                roomNum = tmpObject.GetComponent<PrefabRoom>().txtRoomNum;
                roomName = tmpObject.GetComponent<PrefabRoom>().txtoomName;
                gameType = tmpObject.GetComponent<PrefabRoom>().txtGmeType;
                roomSecret = tmpObject.GetComponent<PrefabRoom>().imgRoomSecret;
                roomPlaying = tmpObject.GetComponent<PrefabRoom>().imgRoomPlaying;
                playerCount = tmpObject.GetComponent<PrefabRoom>().txtPlayerCount;
                //ȭ�鿡 �� ����

                List<string> data = new List<string>();
                foreach (string key in enumType.roomKeysList)
                {
                    data.Add(Lobby.Instance.RoomList[i].CustomProperties[key].ToString());
                }

                roomNum.text = "No." + data[0];
                roomName.text = data[1];
                //���ȣ �� ����
                int index = int.Parse(data[2]);
                enumType.gameType name;
                switch (index)
                {
                    case 1:
                        name = (enumType.gameType)index;
                        gameType.text = name.ToString();
                        break;
                    case 2:
                        name = (enumType.gameType)index;
                        gameType.text = name.ToString();
                        break;
                    case 3:
                        name = (enumType.gameType)index;
                        gameType.text = name.ToString();
                        break;
                    case 4:
                        name = (enumType.gameType)index;
                        gameType.text = name.ToString();
                        break;
                }
                //����Ÿ��
                if (data[4].ToString() == true.ToString())
                {
                    roomSecret.gameObject.SetActive(true);

                    roomSecret.transform.GetComponentInChildren<Text>().text = data[3];
                }
                //��й�����
                if (data[5].ToString() == true.ToString())
                {
                    roomPlaying.color = Color.red;
                }
                //�������� ������
                playerCount.text = Lobby.Instance.RoomList[i].PlayerCount + " / " + Lobby.Instance.RoomList[i].MaxPlayers;
                //�÷��̾� ī��Ʈ
                pageRoom.text = curPage + " / " + totalPage;
                //������ǥ��
            }
        }
        else
        //���� 0��
        {
            emptyRoom.gameObject.SetActive(true);
        }
        //�� ���
    }
    #endregion


    #region ��ư �̺�Ʈ �Լ�
    public void OnClickNextRoomList()
    {
        if (curPage + 1 <= totalPage)
        //�� ���ִ� ��쿡�� �ѱ� �� ����
        {
            foreach (GameObject roomObject in printRoom)
            {
                GameObject.Destroy(roomObject);
            }

            curPage++;

            RoomPrint(curPage);
        }
    }
    public void OnClickPreRoomList()
    {
        if (curPage - 1 > 0)
        //ù�������� �ƴϿ��� �ڷ� �� �� ����
        {
            foreach (GameObject roomObject in printRoom)
            {
                GameObject.Destroy(roomObject);
            }

            curPage--;

            RoomPrint(curPage);
        }
    }
    #endregion
}
