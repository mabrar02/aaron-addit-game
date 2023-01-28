using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holdRestart : MonoBehaviour
{
    [SerializeField] private float timeToRestart = 3f;
    [SerializeField] private Transform respawnPoint;
    private float restartTimer = 0;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            startTimer();
        }
        else if (Input.GetKey(KeyCode.R)) {
            restartTimer += Time.deltaTime;
            if(restartTimer > timeToRestart) {
                gameObject.transform.position = respawnPoint.position;
                restartTimer = 0;
            }
        }
    }

    private void startTimer() {
        restartTimer = 0;
    }
}
