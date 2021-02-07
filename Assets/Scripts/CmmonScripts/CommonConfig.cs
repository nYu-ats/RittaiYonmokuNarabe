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

    public class MatchingPattern
    {
        private const int createRoom = 1;
        public static int CreateRoom{get {return createRoom;}}
        private const int enterRoom = 2;
        public static int EnterRoom{get {return enterRoom;}}
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

            private const string whiteGo = "WhiteGo";
            public static string WhiteGo{get {return whiteGo;}}
            private const string blackGo = "BlackGo";
            public static string BlackGo{get {return blackGo;}}

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
        public static int FirstAttack{get {return 1;}}

        private const string secondAttack = "Black"; //黒が後攻
        public static int SecondAttack{get {return 2;}}

        private const int totalGoNumber = 64; //碁の総数は64個
        public static int TotalGoNumber{get {return totalGoNumber;}}
        private const int soloPlayMode = 1;
        public static int SoloPlayMode{get {return soloPlayMode;}}
        private const int multiPlayMode = 2;
        public static int MultiPlayMode{get {return multiPlayMode;}}
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

    public class NPCLevel
    {
        private const int easyLevel = 1;
        public static int EasyLevel{get {return easyLevel;}}

        private const int normalLevel = 2;
        public static int NormalLevel{get {return normalLevel;}}

        private const int hardLevel = 3;
        public static int HardLevel{get {return hardLevel;}}

    }

    public class GamePhaseJudge
    {
        private static (int x, int z, int limit)[] earlyPhasePoint
        = new (int x, int z, int limit)[8]{(x:0, z:0, limit:0), (x:0, z:3, limit:0), (x:3, z:0, limit:0), (x:3, z:3, limit:0),
                                           (x:1, z:1, limit:2), (x:1, z:2, limit:2), (x:2, z:1, limit:2), (x:2, z:2, limit:2)};
        public static (int x, int z, int limit)[] EarlyPhasePoint
        {get {return earlyPhasePoint;}}

        private const int middlePhaseGoLimit = 32;
        public static int MiddlePhaseGoLimit{get {return middlePhaseGoLimit;}}
    }

    public class LinePattern
    {
        private const int pattern0 = 0; //単体の碁の状態
        public static int Pattern0{get{return pattern0;}}
        private const int pattern1 = 1; //条件1 : XとY座標が変化しないライン
        public static int Pattern1{get{return pattern1;}}

        private const int pattern2 = 2; //条件2 : ZとY座標が変化しないライン
        public static int Pattern2{get{return pattern2;}}

        private const int pattern3 = 3; //条件3 : XとZ座標が同じライン
        public static int Pattern3{get{return pattern3;}}

        private const int pattern4 = 4; //条件4 : X座標が変化せずYとZ座標が異なる(1ずつ上昇もしくは下降する)
        public static int Pattern4{get{return pattern4;}}
        private const int pattern5 = 5; //条件5 : Z座標が変化せずXとY座標が異なる(1ずつ上昇もしくは下降する)
        public static int Pattern5{get{return pattern5;}}
        private const int pattern6 = 6; //条件6 : Y座標が変化せずXとZ座標が異なる(1ずつ上昇もしくは下降する)
        public static int Pattern6{get{return pattern6;}}
        private const int pattern7 = 7; //条件7 : X,Y,Z座標いずれも異なる(1ずつ上昇もしくは下降する)
        public static int Pattern7{get{return pattern7;}}

    }
}