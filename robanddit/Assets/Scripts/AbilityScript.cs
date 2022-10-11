using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{

    Ray MouseRay = new Ray();
    Vector3 MousePos = new Vector3();
    Vector3 RelativeMousePos = new Vector3();
    bool HitHauntable;
    RaycastHit HitData; 
    public LayerMask world; 
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RelativeMousePos = MousePos - transform.position;
        MouseRay.origin = new Vector3(transform.position.x+3, transform.position.y+3, 0);
        MouseRay.direction = new Vector3(RelativeMousePos.x, RelativeMousePos.y, 0);
        
        if(Physics.Raycast(MouseRay, out HitData, Mathf.Infinity, 3)) {

        Debug.Log("hi");
        }

        if(Physics.Raycast(new Vector3(0,0,0), new Vector3(100,0,0), 1000, world)) {

        Debug.Log("hiiii");
        }
        Debug.DrawRay(MouseRay.origin, MouseRay.direction*10, Color.red, 10, false);
        Debug.DrawRay(new Vector3(0,0,0), new Vector3(1,0,0)*100, Color.blue, 10, false);
//        Debug.Log("Origin : " + MouseRay.origin + " Direction: " + MouseRay.direction );

    }






}
