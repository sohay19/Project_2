using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabWordMap : MonoBehaviour, IPunObservable
{
    public static PrefabWordMap instance;

    public WordQuiz1 wordQuiz;
    public Map1 map;
    //QuickSheet����
    public GameObject tabQuizNumInfo;

    Text txtQuizNum;
    GameObject content;
    List<WordQuiz> wordQuizList;
    //WordQuiz Ŭ����
    Hashtable mapTableHash;
    //key = ����
    //value = �ش� ĭ�� ��ġ
    Hashtable correctQuizHash;
    //key = ������ȣ
    //value = ���߾���������


    #region ���� �Լ�
    public List<WordQuiz> WordQuizList
    {
        get
        {
            return wordQuizList;
        }
    }
    public Hashtable CorrectQuizHash
    {
        get
        {
            return correctQuizHash;
        }
    }
    public GameObject TabQuizNumInfo
    {
        get
        {
            return tabQuizNumInfo;
        }
    }
    public Text TxtQuizNum
    {
        get
        {
            return txtQuizNum;
        }
    }
    public static PrefabWordMap Instance
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
        content = transform.Find("Content").gameObject;
    }
    void Start()
    {
        wordQuizList = new List<WordQuiz>();
        mapTableHash = new Hashtable();
        correctQuizHash = new Hashtable();
        txtQuizNum = tabQuizNumInfo.GetComponentInChildren<Text>();
        //���� �ʱ�ȭ

        GameObject tmpObject = GameObject.Find("Canvas").transform.Find("Panel_Game").gameObject;
        transform.SetParent(tmpObject.transform.GetChild(0));
        //Panel_Game�� �ڽ��� Tab_WordQuiz�ΰ���

        RectTransform rect = GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition.x, rect.anchoredPosition.y, 0);
        //������ ����

        SetWordQuiz();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(txtQuizNum.text);
            stream.SendNext(tabQuizNumInfo.activeInHierarchy.ToString());
            for (int i = 0; i < content.transform.childCount; i++)
            {
                Image image = content.transform.GetChild(i).GetComponent<Image>();
                stream.SendNext(image.color.r);
                stream.SendNext(image.color.g);
                stream.SendNext(image.color.b);
                stream.SendNext(image.color.a);
                Text txt = content.transform.GetChild(i).GetComponentInChildren<Text>();
                stream.SendNext(txt.text);
                stream.SendNext(txt.color.a);
            }
        }
        else
        {
            txtQuizNum.text = (string)stream.ReceiveNext();
            tabQuizNumInfo.SetActive((string)stream.ReceiveNext() == true.ToString() ? true : false);
            for (int i = 0; i < content.transform.childCount; i++)
            {
                Image image = content.transform.GetChild(i).GetComponent<Image>();
                image.color = new Color((float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext(), (float)stream.ReceiveNext());
                Text txt = content.transform.GetChild(i).GetComponentInChildren<Text>();
                txt.text = (string)stream.ReceiveNext();
                txt.color = new Color(50/255f, 50/255f, 50/255f, (float)stream.ReceiveNext());
            }
        }
    }


    #region ���� �Լ�
    public void SetWordQuiz()
    {
        foreach (Map1Data data in map.dataArray)
        {
            mapTableHash.Add(data.Answer, new List<int>(new int[] { data.Childcount1, data.Childcount1, data.Childcount2, data.Childcount3, data.Childcount4, data.Childcount5, data.Childcount6 }));

            List<int> list = mapTableHash[data.Answer] as List<int>;
            list.RemoveAt(0);
            //�Ǿտ� �ѹ��� ���� �� ����
        }

        foreach (WordQuiz1Data data in wordQuiz.dataArray)
        {
            List<int> list = mapTableHash[data.Answer] as List<int>;

            wordQuizList.Add(new WordQuiz(data.Numquiz, data.Quiz, data.Answer, list));
        }
        //������ ��������

        foreach (WordQuiz quiz in wordQuizList)
        {
            correctQuizHash.Add(quiz.numQuiz, false);
        }
        //���� Ȯ�� ��� �ۼ�

        foreach (WordQuiz quiz in wordQuizList)
        {
            for (int i = 0; i < quiz.answerStirng.Length; i++)
            {
                Text txtAnswer = content.transform.GetChild(quiz.mapChildList[i]).GetComponentInChildren<Text>();
                txtAnswer.text = quiz.answerStirng[i].ToString();
                txtAnswer.color = new Color32(50, 50, 50, 0);
                //�۾� �����ϰ�
            }
        }
        //���� �ۼ�
    }
    #endregion


    #region ���� ���� �Լ�
    public void TurnCell()
    {
        StartCoroutine(CellLoad());
    }
    IEnumerator CellLoad()
    {
        int firstIndex = wordQuizList.FindIndex(x => x.numQuiz == WordQuizRun.Instance.PrefabQuiz.CurQuizNum);
        List<int> childList = mapTableHash[wordQuizList[firstIndex].answerStirng] as List<int>;

        foreach (int secondIndex in childList)
        {
            if(secondIndex < 100)
            {
                Text txtAnswer = content.transform.GetChild(secondIndex).GetComponentInChildren<Text>();

                byte count = 0;
                while (count < 255)
                {
                    txtAnswer.color = new Color32(50, 50, 50, count);
                    count += 1;
                }
            }
        }

        yield return null;
    }
    #endregion


    #region Ȯ�� �Լ�
    public void CheckCell(bool isCheck)
    {
        int index = wordQuizList.FindIndex(x => x.numQuiz == WordQuizRun.Instance.PrefabQuiz.CurQuizNum);
        List<int> list = mapTableHash[wordQuizList[index].answerStirng] as List<int>;

        if (isCheck)
        {
            foreach (int chileNum in list)
            {
                if (chileNum < 100)
                {
                    content.transform.GetChild(chileNum).GetComponent<Image>().color = Color.yellow;
                }
            }
        }
        else
        {
            foreach (int chileNum in list)
            {
                if (chileNum < 100)
                {
                    content.transform.GetChild(chileNum).GetComponent<Image>().color = new Color32(255, 255, 255, 220);
                }
            }
        }
        
    }
    public bool CompareAnswer(string answer)
    {
        int index = wordQuizList.FindIndex(x => x.numQuiz == WordQuizRun.Instance.PrefabQuiz.CurQuizNum);
        string curAnswer = wordQuizList[index].answerStirng;

        if (curAnswer == answer)
        {
            correctQuizHash[WordQuizRun.Instance.PrefabQuiz.CurQuizNum] = true;
        }
        //���Ṯ���� ����

        return curAnswer == answer ? true : false;
    }
    #endregion
}
