using UnityEngine;

public class HauntLog : MonoBehaviour {

    #region VARIABLES
    [SerializeField] [Range(1f,10f)] private float growthSpeed = 1f;
    [SerializeField] private float maxHeight = 20f;
    [SerializeField] private float minHeight = 2.5f;

    [HideInInspector] public Vector3 initialPosition;
    [HideInInspector] public Vector3 initialScale;
    [HideInInspector] public Vector3 scaleChange, positionChange;
    private Transform logTransform;
    private Transform playerTransform;
    private bool isAttached = true;
    #endregion

    private void Start() {
        initialPosition = transform.position;
        initialScale = transform.localScale;

        logTransform = GetComponent<Transform>();
        playerTransform = logTransform.GetChild(0);


        float newScale = growthSpeed / 100f;
        float newPos = growthSpeed / 200f;

        scaleChange = new Vector3(0.0f, newScale, 0.0f);
        positionChange = new Vector3(0.0f, newPos, 0.0f);
        Debug.Log(newScale + " " + newPos);
    }

    private void Update() {

        float verticalInput = Input.GetAxis("Vertical");

        if ((logTransform.localScale.y < minHeight && verticalInput < 0) || (logTransform.localScale.y > maxHeight && verticalInput > 0)) {

            return;
        }
        else {

            if (isAttached) {
                playerTransform.parent = null;
                isAttached = false;
            }

            logTransform.localScale += scaleChange * verticalInput;
            logTransform.position += positionChange * verticalInput;

            if (!isAttached) {
                playerTransform.parent = transform;
                isAttached = true;
            }
        }

    }
}
