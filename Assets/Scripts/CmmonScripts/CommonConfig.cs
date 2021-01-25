using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonConfig
{
    public class GameSceneName
    {
        //ゲームシーンの名前
        private const string homeScene = "Home";
        public static string HomeScene{get {return homeScene;}}

        private const string gameScene = "RittaiYonmoku";
        public static string GameScene{get {return gameScene;}}
    }

    public class PlayerPrefsKey
    {
        //PlayerPrefsのキー
        private const string userNameKey = "UserName";
        public static string UserNameKey{get {return userNameKey;}}
        
        private const string volumeKey = "Volume";
        public static string VolumeKey{get {return volumeKey;}}
    }

    public class AudioConfig
    {
        //オーディオ周りの設定
        private const float maxVolume = 2.0f;
        public static float MaxVolume{get {return maxVolume;}}
    }

    public class Tags
    {
        public class InHome
        {
            //ホーム画面で使っているタグ
        }

        public class InRittaiYonmoku
        {
            //ゲーム画面で使っているタグ
            private const string gameContoroller = "GameController";
            public static string GameController{get {return gameContoroller;}}
        }
    }

    public class URL
    {
        private const string thisAppId = "mitei"; //決まり次第更新
        public static string ThisAppId{get {return thisAppId;}}

        private const string googleAppStoreURL = "https://play.google.com/store/apps/details?id=";
        public static string GoogleAppStoreURL{get {return googleAppStoreURL;}}
    }

    public class GameRule
    {
        //基礎となるゲームルール
        private const string firstAttack = "White"; //白が先行
        public static string FirstAttack{get {return firstAttack;}}

        private const string secondAttack = "Black"; //黒が後攻
        public static string SecondAttack{get {return secondAttack;}}

        private const int totalGoNumber = 64; //碁の総数は64個
        public static int TotalGoNumber{get {return totalGoNumber;}}
    }

    public class BoardStatus
    {
        //ボードのステータスに関する設定
        private const int canNotPut = -1;
        public static int CanNotPut{get {return canNotPut;}}

        private const int vacant = 0;
        public static int Vacant{get {return vacant;}}

        private const int goWhite = 1;
        public static int GoWhite{get {return goWhite;}}

        private const int goBlack = 2;
        public static int GoBlack{get {return goBlack;}}
    }
}