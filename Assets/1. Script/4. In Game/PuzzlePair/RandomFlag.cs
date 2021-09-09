using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RandomFlag : MonoBehaviour
{
    public List<GameObject> objectFlag;

    List<int> numFlag;
    Hashtable flag;

    int index;


    void Start()
    {
        numFlag = new List<int>();
        flag = new Hashtable();

        ChoiceFlag();
        //6���� ��� ���� �� 12�� �ø���

        ShuffleList();
        //��� ����

        MadeFlagInHash();
        //Sprite �Ҵ� �� �ؽ����̺� �ֱ�
    }
    void Update()
    {

    }


    void ChoiceFlag()
    {
        index = Random.Range(1, 15);
        numFlag.Add(index);
        //ó�� �� �־��ֱ�
        
        while (numFlag.Count < 6)
        {
            index = Random.Range(1, 15);

            if (numFlag.FindIndex(x => x == index) == -1)
            {
                numFlag.Add(index);
            }
        }
        //6������ ����

        numFlag.AddRange(numFlag.ToArray());
        //����Ʈ����
    }
    void ShuffleList()
    {
        int random1, random2;
        int tmp;

        for (int i = 0; i < numFlag.Count; ++i)
        {
            random1 = Random.Range(0, numFlag.Count);
            random2 = Random.Range(0, numFlag.Count);

            tmp = numFlag[random1];
            numFlag[random1] = numFlag[random2];
            numFlag[random2] = tmp;
        }
    }
    void MadeFlagInHash()
    {
        for(int i = 0; i < objectFlag.Count; i++)
        {
            Image curImage = objectFlag[i].GetComponent<Image>();
            curImage.sprite = Resources.Load<Sprite>("ui_flags " + numFlag[i]);
            curImage.color = Color.black;

            Flag tmpFlag = new Flag("ui_flags " + numFlag[i], curImage);

            flag.Add(objectFlag[i].name, tmpFlag);
            //�ؽ����̺���
            //key = ��ü�̸�
            //���� = ��߸�, �ش� Image
        }
    }
}
