using UnityEngine;
using Firebase.Database;

public class BaseFirebaseFunc : MonoBehaviour
{
    protected DatabaseReference reference;
    string realTimeDatabaseUrl = "https://rittaiyonmoku-b3d9f-default-rtdb.firebaseio.com/";
    void Start(){
        //起動時にreference作らないといけない
        //reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference = FirebaseDatabase.GetInstance(realTimeDatabaseUrl).RootReference;
    }
}
