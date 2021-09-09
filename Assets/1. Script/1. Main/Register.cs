using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;

public class Register : MonoBehaviour
{
    public InputField inputId;
    public InputField inputNick;
    public InputField inputPw;
    public InputField inputPwCheck;
    public GameObject tabClosePopUp;
    public GameObject tabProcessPopUp;

    string strId;
    string strNick;
    string strPw;
    string strPwCheck;


    void Start()
    {
        strId = "";
        strNick = "";
        strPw = "";
        strPwCheck = "";
    }


    #region Playfab �Լ�
    void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("���� ����");

        Dictionary<string, string> dirPlayer = new Dictionary<string, string>();
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.ID], result.Username);
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.PlayfabID], result.PlayFabId);
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.NickName], strNick);
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.Level], "Level1");
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.Coin], "1000");
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.Ready], false.ToString());
        dirPlayer.Add(enumType.playerKeysList[(int)enumType.playerKey.Login], false.ToString());

        var request = new UpdateUserDataRequest { Data = dirPlayer };
        PlayFabClientAPI.UpdateUserData(request, PlayerDataUpdateSuccess, PlayerDataUpdateFailure);
    }
    void RegisterFailure(PlayFabError error)
    {
        Debug.LogWarning("���� ����");
        Debug.LogWarning(error.GenerateErrorReport());

        PopUp.instance.ShowTabClose(tabClosePopUp, "������ �߻��߽��ϴ�\n �ٽ� �������ּ���");
    }
    void PlayerDataUpdateSuccess(UpdateUserDataResult result)
    {
        Debug.Log("�÷��̾� ���� ������Ʈ ����");

        inputId.text = "";
        inputNick.text = "";
        inputPw.text = "";
        inputPwCheck.text = "";


        PopUp.Instance.StopPopUp();

        tabProcessPopUp.SetActive(false);
        gameObject.SetActive(false);
    }
    void PlayerDataUpdateFailure(PlayFabError error)
    {
        Debug.LogWarning("�÷��̾� ���� ������Ʈ ����");
        Debug.LogWarning(error.GenerateErrorReport());
    }
    #endregion


    #region ��ư �̺�Ʈ
    public void OnClickRegister()
    {
        strId = inputId.text;
        strNick = inputNick.text;
        strPw = inputPw.text;
        strPwCheck = inputPwCheck.text;


        if (strId.Length < 3 || strId.Length > 6)
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "���̵� �ٽ� Ȯ�����ּ���");
        }
        else if (strNick.Length < 3 || strNick.Length > 6)
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "�г����� �ٽ� Ȯ�����ּ���");
        }
        else if (strPw.Length < 6 || strPw.Length > 10)
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "��й�ȣ�� �ٽ� Ȯ�����ּ���");
        }
        else if (strPw != strPwCheck)
        {
            PopUp.instance.ShowTabClose(tabClosePopUp, "��й�ȣ�� ��ġ���� �ʽ��ϴ�");
        }
        else
        {
            var requestRes = new RegisterPlayFabUserRequest { Username = strId, Password = strPw, DisplayName = strNick, RequireBothUsernameAndEmail = false };
            PlayFabClientAPI.RegisterPlayFabUser(requestRes, RegisterSuccess, RegisterFailure);

            PopUp.instance.ShowTabProcess(tabProcessPopUp, "ó�� �� �Դϴ�", 2.0f);
        }
    }
    #endregion
}
