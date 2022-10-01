using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHero : MonoBehaviour
{
   private Rigidbody2D rb; 
   public float moveSpeed = 5;

   // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
    }

    void movePlayer(){
       if(Input.GetKey("a")){       
          transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
       }
       if (Input.GetKey("s")) {
          transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
       }
       if (Input.GetKey("d")) {
          transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
       } 
       if (Input.GetKey("w")) {
          transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
       }    
    }


}
