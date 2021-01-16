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
}