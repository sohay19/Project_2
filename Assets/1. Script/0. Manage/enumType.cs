using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class enumType
{
    public enum playerKey
    {
        ID = 0,
        PlayfabID = 1,
        NickName = 2,
        Level = 3,
        Coin = 4,
        Ready = 5,
        Login = 6
    }
    public static List<string> playerKeysList = Enum.GetNames(typeof(enumType.playerKey)).ToList();
    //�÷��̾�
    public enum roomKey
    {
        roomNum = 0,
        roomName = 1,
        gameType = 2,
        roomPassword = 3,
        isSecret = 4,
        isPlaying = 5,
    }
    public static List<string> roomKeysList = Enum.GetNames(typeof(enumType.roomKey)).ToList();
    //��
    public enum gameType
    {
        �������� = 1,
        BINGO = 2,
        �׸����߱� = 3,
        Ÿ�ڰ��� = 4,
    }
    public static List<string> gameTypeKeysList = Enum.GetNames(typeof(enumType.level)).ToList();
    public enum gameTypeScene
    {
        WordQuiz = 1,
        Bingo = 2,
        Typing = 3,
        PuzzlePair = 4,
    }
    public static List<string> gameTypeSceneKeysList = Enum.GetNames(typeof(enumType.level)).ToList();
    //����Ÿ��
    public enum level
    {
        Level1 = 0,
        Level2 = 1,
        Level3 = 2,
        Level4 = 3,
        Level5 = 4,
        Level6 = 5,
        Level7 = 6,
        Level8 = 7,
        Level9 = 8,
        Level10 = 9,
    }
    public static List<string> levelKeysList = Enum.GetNames(typeof(enumType.level)).ToList();
    //�̹��� �ε�� ����
    public enum levelName
    {
        ���ð�� = 0,
        �ݺؾ� = 1,
        �ٹ��̰�� = 2,
        �ۻ縮 = 3,
        ����� = 4,
        �����ؾ� = 5,
        ��������� = 6,
        aka�ϸ� = 7,
        ���񹰰�� = 8,
        �����ǽ� = 9,
    }
    public static List<string> levelNameKeysList = Enum.GetNames(typeof(enumType.levelName)).ToList();
    //������
    public enum levelCoin
    {
        Level1 = 0,
        Level2 = 1000,
        Level3 = 1500,
        Level4 = 2000,
        Level5 = 2500,
        Level6 = 3000,
        Level7 = 3500,
        Level8 = 4000,
        Level9 = 4500,
        Level10 = 5000,
    }
    public static Array levelCoinArr = Enum.GetValues(typeof(enumType.levelCoin));
    //������ ����
}
