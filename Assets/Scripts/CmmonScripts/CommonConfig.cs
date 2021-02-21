//複数のクラスからアクセスされるような共通の設定値をここにまとめる

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
        //PlayerPrefsの参照キー
        private const string userNameKey = "UserName";
        public static string UserNameKey{get {return userNameKey;}}
        
        private const string volumeKey = "Volume";
        public static string VolumeKey{get {return volumeKey;}}
        private const string bgmVolumeKey = "BGM";
        public static string BgmVolumeKey{get {return bgmVolumeKey;}}
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
        private const float maxBgmVolume = 2.0f;
        
        //playSEのclipのインデックス
        public static float MaxBgmVolume{get {return maxBgmVolume;}}
        private const int buttonPushIndex = 0;
        public static int ButtonPushIndex{get {return buttonPushIndex;}}
        private const int returnIndex = 1;
        public static int ReturnIndex{get {return returnIndex;}}
        private const int volumeButton = 2;
        public static int VolumeButton{get {return volumeButton;}}
        private const int goPutButtonIndex = 2;
        public static int GoPutButtonIndex{get {return goPutButtonIndex;}}
        private const int goPositionMoveButtonIndex = 3;
        public static int GoPositionMoveButtonIndex{get {return goPositionMoveButtonIndex;}}
        private const int gameEndIndex = 4;
        public static int GameEndIndex{get {return gameEndIndex;}}
    }

    public class Tags
    {
        public class InRittaiYonmoku
        {
            //ゲーム画面で使っているタグ
            private const string gameContoroller = "GameController";
            public static string GameController{get {return gameContoroller;}}
            private const string reachChecker = "ReachChecker";
            public static string ReachChecker{get {return reachChecker;}}
        }
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
        private const int giveUpSignal = 9;
        public static int GiveUpSignal{get {return giveUpSignal;}}
        private const int drawSignal = -1;
        public static int DrawSignal{get {return drawSignal;}}
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
        //NPCのレベル
        //現状イージーしかない
        private const int easyLevel = 1;
        public static int EasyLevel{get {return easyLevel;}}

        private const int normalLevel = 2;
        public static int NormalLevel{get {return normalLevel;}}

        private const int hardLevel = 3;
        public static int HardLevel{get {return hardLevel;}}

    }

    public class GamePhaseJudge
    {
        //5ラインのリーチにからむことができる重要なポジションの配列
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
        //碁単体の状態
        private const int pattern0 = 0;
        public static int Pattern0{get{return pattern0;}}

        //条件1 : XとY座標が変化しないライン
        private const int pattern1 = 1;
        public static int Pattern1{get{return pattern1;}}

        //条件2 : ZとY座標が変化しないライン
        private const int pattern2 = 2;
        public static int Pattern2{get{return pattern2;}}

        //条件3 : XとZ座標が同じライン
        private const int pattern3 = 3;
        public static int Pattern3{get{return pattern3;}}

        //条件4 : X座標が変化せずYとZ座標が異なる(1ずつ上昇もしくは下降する)
        private const int pattern4 = 4;
        public static int Pattern4{get{return pattern4;}}

        //条件5 : Z座標が変化せずXとY座標が異なる(1ずつ上昇もしくは下降する)
        private const int pattern5 = 5;
        public static int Pattern5{get{return pattern5;}}

        //条件6 : Y座標が変化せずXとZ座標が異なる(1ずつ上昇もしくは下降する)
        private const int pattern6 = 6;
        public static int Pattern6{get{return pattern6;}}

         //条件7 : X,Y,Z座標いずれも異なる(1ずつ上昇もしくは下降する)
        private const int pattern7 = 7;
        public static int Pattern7{get{return pattern7;}}

    }
}