using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamera : MonoBehaviour
{
    public int heightChange;
    private Vector3 init;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(init == Vector3.zero) init = collision.gameObject.transform.GetChild(4).transform.localPosition;

        if(collision.gameObject.transform.position.x > gameObject.transform.position.x)
        {
            collision.gameObject.transform.GetChild(4).transform.localPosition = new Vector3(init.x,
                                                                                             init.y + heightChange,
                                                                                             init.z);
        } else
        {
            collision.gameObject.transform.GetChild(4).transform.localPosition = init;
        }


    }


}
