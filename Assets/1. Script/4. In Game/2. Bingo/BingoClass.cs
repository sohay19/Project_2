using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BingoClass
{
    string name;
    Image image;
    int check;
    //ī������ ���� bool�� �ƴ� int�� ����
    int num;


    #region ������Ƽ
    public string Name
    {
        get
        {
            return name;
        }
    }
    public Image Image
    {
        get
        {
            return image;
        }
    }
    public int Check
    {
        get
        {
            return check;
        }
    }
    public int Num
    {
        get
        {
            return num;
        }
        set
        {
            num = value;
        }
    }
    #endregion


    public BingoClass()
    {
        name = "";
        image = null;
        check = 0;
        num = 0;
    }
    public BingoClass(string tmpName, Image tmpImg, int tmpNum)
    {
        name = tmpName;
        image = tmpImg;
        check = 0;
        num = tmpNum;
    }


    public void CheckUp()
    {
        check = 1;
    }
}
