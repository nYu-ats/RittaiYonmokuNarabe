using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;


interface ISet{
    void SetInfo(string setValue);
}

interface IUserNameExists{
    bool UserNameExists(string readKey);
}

interface IReadWinLose{
    int[] ReadWinLose(string userName);
}

interface IMatching{
    int Matching(string userName);
}


public class ConnectFirebase: ISet, IUserNameExists, IReadWinLose
{
    public bool UserNameExists(string readKey){
        return true;
    }

    public void SetInfo(string setValue){
        Debug.Log("set");;
    }

    public int[] ReadWinLose(string userName){
        return new int[2]{100, 100};
    }
    public bool connectingFlag = true;

    public async UniTask<int> Matching(){
        await UniTask.Delay(5000);
        int gameRoomNumber = 100;
        connectingFlag = false;
        return gameRoomNumber;
    }
}

interface IDelete{

}


interface ISubscribe{
    
}

