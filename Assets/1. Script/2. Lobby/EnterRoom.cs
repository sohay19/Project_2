using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterRoom : MonoBehaviourPunCallbacks
{
    public Canvas canvas;
    public GameObject roomMakeMenu;
    public GameObject tabPasswordInput;
    public GameObject tabClosePopUp;
    public GameObject tabProcessPopUp;

    public InputField inputRoomName;
    public Dropdown dropGameType;
    public GameObject[] dropImage;
    public Toggle toggleRoomSecret;
    public InputField inputRoomPassword;

    GraphicRaycaster ray;
    PointerEventData eventData;

    string roomNum;
    bool roomSecret;
    bool roomPlay;
    string roomPassword;

    byte maxPlayer = 4;


    void Start()
    {
        ray = canvas.GetComponent<GraphicRaycaster>();
        eventData = new PointerEventData(null);

        roomNum = "";
        roomSecret = false;
        roomPassword = "";
    }
    void Update()
    {

        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                eventData.position = Input.GetTouch(0).position;
                //�������� ����

                List<RaycastResult> result = new List<RaycastResult>();
                ray.Raycast(eventData, result);
                //���̸� �̿��Ͽ� eventData�� ����

                if (result.Count > 0)
                {
                    if (result[0].gameObject.layer == LayerMask.NameToLayer("Room"))
                    {
                        PrefabRoom tmpRoom = result[0].gameObject.GetComponent<PrefabRoom>();

                        roomNum = tmpRoom.txtRoomNum.text.Substring(3);
                        roomSecret = tmpRoom.imgRoomSecret.gameObject.activeInHierarchy;
                        roomPlay = (tmpRoom.imgRoomPlaying.color == Color.red) ? true : false;

                        if (roomSecret)
                        {
                            roomPassword = tmpRoom.imgRoomSecret.transform.GetComponentInChildren<Text>().text;
                        }

                        CheckRoom();
                    }
                }
            }
        }

        /*

        if (Input.GetMouseButtonDown(0))
        {
            eventData.position = Input.mousePosition;
            //�������� ����

            List<RaycastResult> result = new List<RaycastResult>();
            ray.Raycast(eventData, result);
            //���̸� �̿��Ͽ� eventData�� ����

            if (result.Count > 0)
            {
                if (result[0].gameObject.layer == LayerMask.NameToLayer("Room"))
                {
                    PrefabRoom tmpRoom = result[0].gameObject.GetComponent<PrefabRoom>();

                    roomNum = tmpRoom.txtRoomNum.text.Substring(3);
                    roomSecret = tmpRoom.imgRoomSecret.gameObject.activeInHierarchy;
                    roomPlay = (tmpRoom.imgRoomPlaying.color == Color.red) ? true : false;

                    if (roomSecret)
                    {
                        roomPassword = tmpRoom.imgRoomSecret.transform.GetComponentInChildren<Text>().text;
                    }

                    CheckRoom();
                }
            }
        }
        */

    }


    #region ����� �Լ�
    void CheckRoom()
    {
        if (int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Coin.ToString()].ToString()) < 100)
        {
            PopUp.Instance.ShowTabClose(tabClosePopUp, "������ ���� ������ �����մϴ�");
        }
        else if (roomPlay)
        {
            PopUp.Instance.ShowTabClose(tabClosePopUp, "�̹� ������ ���� ���Դϴ�");
        }
        else if (roomSecret)
        {
            tabPasswordInput.SetActive(true);
        }
        else
        {
            PhotonNetwork.JoinRoom(roomNum);
        }
    }
    #endregion


    #region ���� �ݹ� �Լ�
    public override void OnJoinedRoom()
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash = PhotonNetwork.CurrentRoom.CustomProperties;

        int index = int.Parse(hash[enumType.roomKeysList[(int)enumType.roomKey.gameType]].ToString());
        enumType.gameTypeScene name;
        switch (index)
        {
            case 1:
                name = (enumType.gameTypeScene)index;
                SceneManager.LoadScene(name.ToString());
                break;
            case 2:
                name = (enumType.gameTypeScene)index;
                SceneManager.LoadScene(name.ToString());
                break;
            case 3:
                name = (enumType.gameTypeScene)index;
                SceneManager.LoadScene(name.ToString());
                break;
            case 4:
                name = (enumType.gameTypeScene)index;
                SceneManager.LoadScene(name.ToString());
                break;
        }
    }
    #endregion


    #region ��ư �̺�Ʈ
    public void OnClickRoomMakeOk()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {
            if (int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Coin.ToString()].ToString()) < 100)
            {
                PopUp.Instance.ShowTabClose(tabClosePopUp, "������ �����մϴ�");
            }
            else if (inputRoomName.text == "" || dropGameType.value == 0)
            {
                PopUp.Instance.ShowTabClose(tabClosePopUp, "���õ��� �ʾҰų�\n �ۼ����� ����\n �׸��� �����մϴ�");
            }
            else if ((toggleRoomSecret.isOn && inputRoomPassword.text == "") || (!toggleRoomSecret.isOn && inputRoomPassword.text != ""))
            {
                PopUp.Instance.ShowTabClose(tabClosePopUp, "��й� ������\n �߸� �Ǿ����ϴ�.\n �ٽ� Ȯ�����ֽñ� �ٶ��ϴ�");
            }
            else
            {
                int roomNum = Lobby.Instance.RoomCounter;

                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

                hash.Add(enumType.roomKeysList[(int)enumType.roomKey.roomNum], roomNum);
                hash.Add(enumType.roomKeysList[(int)enumType.roomKey.roomName], inputRoomName.text);
                hash.Add(enumType.roomKeysList[(int)enumType.roomKey.gameType], dropGameType.value);
                hash.Add(enumType.roomKeysList[(int)enumType.roomKey.roomPassword], inputRoomPassword.text);
                hash.Add(enumType.roomKeysList[(int)enumType.roomKey.isSecret], toggleRoomSecret.isOn);
                hash.Add(enumType.roomKeysList[(int)enumType.roomKey.isPlaying], false);
                //Ŀ����������Ƽ

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayer;
                roomOptions.IsVisible = true;
                roomOptions.IsOpen = true;
                roomOptions.CustomRoomPropertiesForLobby = enumType.roomKeysList.ToArray();
                //Ű�� �����־����!
                roomOptions.CustomRoomProperties = hash;

                PhotonNetwork.CreateRoom(roomNum.ToString(), roomOptions);
            }
        }
    }
    public void OnClickCheckPasswordOK()
    {
        InputField input = tabPasswordInput.transform.GetChild(2).GetComponent<InputField>();
        if (input.text == roomPassword)
        {
            PhotonNetwork.JoinRoom(roomNum);
        }
        else
        {
            PopUp.Instance.ShowTabProcess(tabProcessPopUp, "�߸��� ��й�ȣ �Դϴ�", 1.0f);
        }
    }
    #endregion

    #region OnChanged
    public void OnChangeDropDown(int num)
    {
        switch (num)
        {
            case 0:
                for(int i = 0; i < dropImage.Length; i++)
                {
                    if (i == 0)
                    {
                        dropImage[i].SetActive(true);
                    }
                    else
                    {
                        dropImage[i].SetActive(false);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < dropImage.Length; i++)
                {
                    if (i == 1)
                    {
                        dropImage[i].SetActive(true);
                    }
                    else
                    {
                        dropImage[i].SetActive(false);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < dropImage.Length; i++)
                {
                    if (i == 2)
                    {
                        dropImage[i].SetActive(true);
                    }
                    else
                    {
                        dropImage[i].SetActive(false);
                    }
                }
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }
    #endregion
}
