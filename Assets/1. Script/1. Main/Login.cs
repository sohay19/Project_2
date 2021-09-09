using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System;
using System.Linq;

public class Login : MonoBehaviourPunCallbacks
{
    public static Login instance;

    public GameObject tabClosePopUp;
    public GameObject tabProcessPopUp;
    public InputField inputId;
    public InputField inputPw;

    List<string> playerData = new List<string>();
    //�÷��̾� Ŀ���� ������Ƽ

    string strId;
    string strPw;


    public static Login Instance
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
        strId = "";
        strPw = "";

        PhotonNetwork.AutomaticallySyncScene = true;
    }


    #region ����� �Լ�
    public void SetPlayerInfo()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerData[2];

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

            for(int i = 0; i < playerData.Count; i++)
            {
                hash.Add(enumType.playerKeysList[i], playerData[i]);
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

            PhotonNetwork.LoadLevel("2.Lobby");
        }
    }
    #endregion


    #region Playfab �Լ�
    void LoginSuccess(LoginResult result)
    {
        var request = new GetUserDataRequest { PlayFabId = result.PlayFabId, Keys = enumType.playerKeysList };
        PlayFabClientAPI.GetUserData(request, UserDataGetSuccess, UserDataGetFailure);
    }
    void LoginFailure(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "���̵� �Ǵ� �н����尡\n�߸��Ǿ����ϴ�");
        }
        else if (error.Error == PlayFabErrorCode.InvalidParams)
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "���̵�� 3�� �̻�, ��й�ȣ��\n6�� �̻� �Է��ؾ� �մϴ�");
        }
        else
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "�ٽ� �õ� ���ֽñ� �ٶ��ϴ�");
        }
    }
    //�α���
    void UserDataGetSuccess(GetUserDataResult result)
    {
        Debug.LogWarning("���� ������������ ����");

        playerData.Clear();
        for(int i = 0; i < result.Data.Count; i++)
        {
            playerData.Add(result.Data[enumType.playerKeysList[i].ToString()].Value.ToString());
        }

        if (playerData[(int)enumType.playerKey.Login].ToString() == false.ToString())
        //�α��λ��¾ƴ�
        {
            Dictionary<string, string> dirPlayer = new Dictionary<string, string>();
            dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.Login], true.ToString());

            var request = new UpdateUserDataRequest { Data = dirPlayer };
            PlayFabClientAPI.UpdateUserData(request, PlayerDataUpdateSuccess, PlayerDataUpdateFailure);
        }
        else
        //�̹� �α��ε� ������
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "�̹� �α��� �� �����Դϴ�");
        }
    }
    void UserDataGetFailure(PlayFabError error)
    {
        Debug.LogWarning("���� ������������ ����");
    }
    void PlayerDataUpdateSuccess(UpdateUserDataResult result)
    {
        Debug.Log("�÷��̾� ���� ������Ʈ ����");

        PhotonNetwork.ConnectUsingSettings();
        //�α��� ���� ������Ʈ �� Ŀ��Ʈ
    }
    void PlayerDataUpdateFailure(PlayFabError error)
    {
        Debug.LogWarning("�÷��̾� ���� ������Ʈ ����");
        Debug.LogWarning(error.GenerateErrorReport());
    }
    #endregion


    #region Photon CallBack
    public override void OnConnectedToMaster()
    {
        SetPlayerInfo();

        Save.Instance.ChangeLevel();
    }
    #endregion


    #region ��ư �̺�Ʈ �Լ�
    public void OnClickLogin()
    {
        strId = inputId.text;
        strPw = inputPw.text;

        var request = new LoginWithPlayFabRequest { Username = strId, Password = strPw };
        PlayFabClientAPI.LoginWithPlayFab(request, LoginSuccess, LoginFailure);

        PopUp.instance.ShowTabProcess(tabProcessPopUp, "ó�� �� �Դϴ�", 5.0f);
    }
    #endregion
}
