using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundGo : MonoBehaviour
{
    [SerializeField] float destroyTime = 3.0f;
    private float time;
    void Start(){
        time = 0.0f;
        this.gameObject.GetComponent<Rigidbody>().AddTorque(5.0f, 0.0f, 0.0f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update(){
        time += Time.deltaTime;
        if(time > destroyTime){
            Destroy(this.gameObject);
        }
    }
}
