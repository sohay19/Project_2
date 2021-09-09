using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PrefabBingoMap : MonoBehaviour
{
    public static PrefabBingoMap instance;

    public GameObject content;
    public GameObject popupInfo;
    public Text txtBingoCount;

    List<BingoClass> cellList = new List<BingoClass>();
    List<List<BingoClass>> lineAllList = new List<List<BingoClass>>();
    //���� ������ ��(�� 16��)
    //key = ��ü �̸�
    //value = List<BingoClass>
    Hashtable hashPointLine = new Hashtable();
    //key = ��ü �̸�
    //value = �ش� ��ü�� ���Ե� ���� ������ ���� �ε�����ȣ List
    RectTransform rect;


    #region ������Ƽ
    public List<BingoClass> CellList
    {
        get
        {
            return cellList;
        }
    }
    #endregion


    private void Awake()
    {
        instance = this;

        GameObject tmpObject = GameObject.Find("Canvas").transform.Find("Panel_Game").gameObject;
        transform.SetParent(tmpObject.transform.GetChild(0));
    }
    void Start()
    {
        rect = transform.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y, 0);
    }


    #region
    public void PassMyMap()
    {
        if (Save.CurPhotonView.IsMine)
        {
            Save.CurPhotonView.RPC(nameof(PrefabPlayer.instance.PassMap), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName, gameObject.GetPhotonView().ViewID);
            //�� ������� ����� �ѱ��
        }
    }
    public void SetScale()
    {
        rect = transform.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
    }
    public void MakeBingo()
    {
        SetMap();
        //���ڼ���
        SetBingo();
        //������ ����
        FindPoint();
        //���� ���� �� �� ����ƮȮ��

        if (GameManager.Instance.ViewList[2].IsMine)
        {
            BingoRun.Instance.PrefabChoiceMap.StartTimer();
        }
        //�����̸� Ÿ�̸� ����
    }
    public void InfoBingoCount(int count)
    {
        txtBingoCount.text = count + "��° �����Դϴ�!";

        StartCoroutine(ShowMassage());
    }
    IEnumerator ShowMassage()
    {
        popupInfo.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        popupInfo.SetActive(false);
    }
    #endregion


    #region ����� �Լ�
    public List<List<BingoClass>> pointList(string name)
    {
        List<List<BingoClass>> bingoList = new List<List<BingoClass>>();
        //���õ� ���ڰ� ���Ե� ������ ����
        List<int> tmpList = hashPointLine[name] as List<int>;
        //���õ� ���ڰ� ���Ե� �ε��� ����Ʈ

        foreach (int index in tmpList)
        {
            bingoList.Add(lineAllList[index]);
        }

        return bingoList;
    }
    void FindPoint()
    {
        List<int> tmpList = new List<int>();

        for (int i = 0; i < cellList.Count; i++)
        {
            string nameImg = cellList[i].Name;

            for (int j = 0; j < lineAllList.Count; j++)
            {
                foreach (BingoClass bingo in lineAllList[j])
                {
                    if (bingo.Name == nameImg)
                    {
                        tmpList.Add(j);
                    }
                }
            }

            hashPointLine.Add(nameImg, tmpList.ToList());
            //�� ��ü�� ������ �ľ� �Ϸ�
            tmpList.Clear();
            //�ӽ� ����Ʈ ����
        }
    }
    void SetBingo()
    {
        List<BingoClass> line = new List<BingoClass>();
        //���� �Ǵ� �� ��(BingoClass�� ����)

        for (int i = 0; i < content.transform.childCount; i++)
        {
            line.Add(cellList[i]);

            if ((i + 1) % 7 == 0)
            {
                lineAllList.Add(line.ToList());
                //�� �� �ϼ� �� ����
                line.Clear();
                //����
            }
        }
        //���� ��

        for (int i = 0; i < content.transform.childCount; i += 7)
        {
            line.Add(cellList[i]);

            if (i >= 42 && i <= 48)
            {
                lineAllList.Add(line.ToList());
                //�� �� �ϼ� �� ����
                line.Clear();
                //����

                if (i != 48)
                {
                    i -= 48;
                    //���� i�� ����
                }
            }
        }
        //���� ��

        for (int i = 0; i < content.transform.childCount; i += 8)
        {
            line.Add(cellList[i]);
        }
        lineAllList.Add(line.ToList());
        //�� �� �ϼ� �� ����
        line.Clear();
        //����

        for (int i = 6; i < content.transform.childCount - 6; i += 6)
        {
            line.Add(cellList[i]);
        }
        lineAllList.Add(line.ToList());
        //�� �� �ϼ� �� ����
        line.Clear();
        //����

        //�밢�� 2��
    }
    void SetMap()
    {
        List<int> tmpList = new List<int>();

        int randNum = Random.Range(1, 51);
        tmpList.Add(randNum);
        //ó�� �� �ֱ�

        while (tmpList.Count < 49)
        {
            randNum = Random.Range(1, 51);

            if (tmpList.FindIndex(x => x == randNum) == -1)
            {
                tmpList.Add(randNum);
            }
        }
        //49���� ���� ���� �ֱ�

        int random1, random2;
        int tmp;

        for (int i = 0; i < tmpList.Count; ++i)
        {
            random1 = Random.Range(0, tmpList.Count);
            random2 = Random.Range(0, tmpList.Count);

            tmp = tmpList[random1];
            tmpList[random1] = tmpList[random2];
            tmpList[random2] = tmp;
        }
        //ShuffleList()

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Image tmpImg = content.transform.GetChild(i).GetComponent<Image>();
            Text txt = tmpImg.GetComponentInChildren<Text>();

            txt.text = tmpList[0].ToString();
            tmpList.RemoveAt(0);
            //����ǥ��

            cellList.Add(new BingoClass(tmpImg.name, tmpImg, int.Parse(txt.text)));
            //��ü ĭ�� ���� ���� ����
        }
    }
    #endregion
}
