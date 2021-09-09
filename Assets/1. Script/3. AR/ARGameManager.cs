using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARGameManager : MonoBehaviour
{
    public static ARGameManager instance;

    public GameObject tabFind;
    public GameObject tabSwap;

    GameObject btnSwap;
    Text txtFind;
    Text txtSwap;

    bool isFindStart;
    bool isSwapEnd;


    #region ������Ƽ
    public static ARGameManager Instance
    {
        get
        {
            return instance;
        }
    }
    public bool IsFindStart
    {
        get
        {
            return isFindStart;
        }
        set
        {
            isFindStart = value;
        }
    }
    public bool IsSwapEnd
    {
        get
        {
            return isSwapEnd;
        }
    }
    #endregion


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        txtFind = tabFind.transform.GetChild(1).GetComponent<Text>();
        txtSwap = tabSwap.transform.GetChild(1).GetComponent<Text>();
        btnSwap = tabSwap.transform.GetChild(2).gameObject;

        isFindStart = false;
        isSwapEnd = false;
    }


    #region ����� �Լ�
    public void CheckFind()
    {
        tabFind.SetActive(true);
        txtFind.text = "Ʋ�Ƚ��ϴ�. �ٽ� �������ּ���";

        StartCoroutine(ResetText());
    }
    IEnumerator ResetText()
    {
        yield return new WaitForSeconds(1.0f);

        tabFind.SetActive(false);
    }
    public void ResultFind()
    {
        tabFind.SetActive(true);
        txtFind.text = "�����Դϴ�! 500������ ȹ���Ͽ����ϴ�";

        int curCoin = int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Coin.ToString()].ToString());
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Coin], (curCoin + 500).ToString());

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        //100�����߰�

        StartCoroutine(LoadMain());
    }
    IEnumerator LoadMain()
    {
        yield return new WaitForSeconds(3.0f);

        PhotonNetwork.LoadLevel("3.AR");
    }
    public void CheckSwap()
    {
        tabSwap.SetActive(true);
        txtSwap.text = "������ ����ִ� ���ڸ� �������ּ���";

        Controll.Instance.TargetObject = null;
        Destroy(Controll.Instance.ArrowObject);

        isSwapEnd = true;
    }
    public void ResultSwap()
    {
        Swap swap = Map.Instance.SwapObject.GetComponent<Swap>();

        if (Controll.Instance.TargetObject == swap.BoxList[1].gameObject)
        {
            swap.BoxList[1].GetComponent<Animation>().Play("OpenBox");
            txtSwap.text = "�����Դϴ�! 500������ ȹ���Ͽ����ϴ�";

            int curCoin = int.Parse(PhotonNetwork.LocalPlayer.CustomProperties[enumType.playerKey.Coin.ToString()].ToString());
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash.Add(enumType.playerKeysList[(int)enumType.playerKey.Coin], (curCoin + 500).ToString());

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            //100�����߰�
        }
        else
        {
            Controll.Instance.TargetObject.GetComponent<Animation>().Play("OpenBox");
            txtSwap.text = "Ʋ�Ƚ��ϴ�. �ٽ� �� �� �������ּ���";
        }

        Controll.Instance.TargetObject = null;
        Destroy(Controll.Instance.ArrowObject);

        btnSwap.SetActive(true);
    }
    #endregion
}
