using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCamera : MonoBehaviour
{
    public int heightChange;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.transform.position.x > gameObject.transform.position.x)
        {
            collision.gameObject.transform.GetChild(4).transform.localPosition = new Vector3(collision.gameObject.transform.GetChild(4).transform.localPosition.x,
                                                                                             collision.gameObject.transform.GetChild(4).transform.localPosition.y + heightChange,
                                                                                             collision.gameObject.transform.GetChild(4).transform.localPosition.z);

        } else
        {
            collision.gameObject.transform.GetChild(4).transform.localPosition = new Vector3(collision.gameObject.transform.GetChild(4).transform.localPosition.x,
                                                                                             collision.gameObject.transform.GetChild(4).transform.localPosition.y - heightChange,
                                                                                             collision.gameObject.transform.GetChild(4).transform.localPosition.z);
        }


    }


}
