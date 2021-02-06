using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirebaseChildKey
{
    public class GetKey
    {
        private const string userKey = "user";
        public static string UserKey{get{return userKey;}}

        private const string recordKey = "record";
        public static string RecordKey{get{return recordKey;}}
        private const string matchingRoomKey = "matching";
        public static string MatchingRoomKey{get{return matchingRoomKey;}}
        private const string gamePlayer = "player";
        public static string GamePlayer{get {return gamePlayer;}}
    }
}