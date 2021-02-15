using UnityEngine;
using Firebase.Database;

public class BaseFirebaseFunc : MonoBehaviour
{
    protected DatabaseReference reference;
    void Start(){
        //起動時にreference作らないといけない
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
}
