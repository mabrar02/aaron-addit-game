using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    public string msg;
    public static event EventHandler<UIEventArgs> triggerEnter;
    public UIEventArgs args;

    private void Start()
    {
        args = new UIEventArgs();
        args.msg = msg;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            triggerEnter?.Invoke(this, args);
            Destroy(this);
        }
    }

}

public class UIEventArgs : EventArgs
{
    public string msg { get; set; }
}
