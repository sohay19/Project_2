using Photon.Realtime;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class Save : MonoBehaviourPunCallbacks
{
    public static Save instance;
    public static GameObject CurPlayerPrefab;
    public static PhotonView CurPhotonView;

    bool isQuit;
    

    public static Save Instance
    {
        get
        {
            return instance;
        }
    }
    public bool IsQuit
    {
        set
        {
            isQuit = value;
        }
    }


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        CurPlayerPrefab = null;
        CurPhotonView = null;

        isQuit = false;

        DontDestroyOnLoad(this);
    }


    #region ���� �ݹ�
    public override void OnLeftRoom()
    {
        UpadatePlayerInfoToPlayfab();
        SceneManager.LoadScene("2.Lobby");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Audio.TurnOffBGM();
        Destroy(this.gameObject);
        //DontDestroyOnLoad ��ü ��� ����

        if (PhotonNetwork.LocalPlayer.NickName != "")
        {
            SavePlayerInfoToPlayfab();
            SceneManager.LoadScene("1.Main");
        }
    }
    #endregion


    #region �� �ݹ�
    private void OnApplicationQuit()
    {
        if (PhotonNetwork.LocalPlayer.NickName != "")
        {
            if (!isQuit)
            {
                SavePlayerInfoToPlayfab();

                isQuit = true;
            }
        }
    }
    #endregion


    #region ����� �Լ�
    public void UpadatePlayerInfoToPlayfab()
    {
        ChangeLevel();

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Ready], false.ToString());
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //Ready����

        Dictionary<string, string> dirPlayer = new Dictionary<string, string>();

        foreach (string key in enumType.playerKeysList)
        {
            dirPlayer.Add(key, PhotonNetwork.LocalPlayer.CustomProperties[key].ToString());
        }

        var request = new UpdateUserDataRequest { Data = dirPlayer };
        PlayFabClientAPI.UpdateUserData(request, PlayerDataUpdateSuccess, PlayerDataUpdateFailure);
    }
    public void SavePlayerInfoToPlayfab()
    {
        ChangeLevel();

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Ready], false.ToString());
        hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Login], false.ToString());
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //�α��� �� Ready����

        Dictionary<string, string> dirPlayer = new Dictionary<string, string>();

        foreach (string key in enumType.playerKeysList)
        {
            dirPlayer.Add(key, PhotonNetwork.LocalPlayer.CustomProperties[key].ToString());
        }

        var request = new UpdateUserDataRequest { Data = dirPlayer };
        PlayFabClientAPI.UpdateUserData(request, PlayerDataUpdateSuccess, PlayerDataUpdateFailure);
    }
    public void ChangeLevel()
    {
        if(PhotonNetwork.IsConnected)
        {
            string level = "Level1";
            int curCoin = int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Coin.ToString()].ToString());

            for(int i = 0; i < enumType.levelCoinArr.Length; i++)
            {
                if (curCoin > (int)enumType.levelCoinArr.GetValue(i))
                {
                    level = enumType.levelKeysList[i];
                }
                else
                {
                    break;
                }
            }

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add(enumType.playerKey.Level.ToString(), level);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            //����������Ʈ
        }
    }
    #endregion


    #region PlayFab �Լ�
    void PlayerDataUpdateSuccess(UpdateUserDataResult result)
    {
        Debug.Log("�÷��̾� ���� ������Ʈ ����");

        if(isQuit)
        {
            Debug.Log("��������");

            Application.Quit();
        }
    }
    void PlayerDataUpdateFailure(PlayFabError error)
    {
        Debug.LogWarning("�÷��̾� ���� ������Ʈ ����");
        Debug.LogWarning(error.GenerateErrorReport());

        SavePlayerInfoToPlayfab();
    }
    #endregion
}
