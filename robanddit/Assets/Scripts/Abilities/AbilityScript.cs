using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// TODO List:
//  - Somehow stop haunt progression from phasing through objects
//  -

public class AbilityScript : NetworkBehaviour 
{

    #region VARIABLES
    private Vector2 mousePos;     //Absolute mouse position
    private Vector2 dirToMouse;   //Mouse position relative to player's transform

    //Player related variables
    private Rigidbody2D     rb;
    private Collider2D      col;
    public Animator        anim;
    private GameObject      arrow;
    public SpriteRenderer  sprite;

    // Alpha testing
    public Color       originalColour;
    public Color       hauntedColour;
   
    // Haunt related variables
    [SerializeField] private Vector2 hauntCheckSize     = new Vector2(1.5f,1.5f);
    [SerializeField] private float   hauntDistanceLimit = 100;
    [SerializeField] private float   hauntInSpeed       = 0.15f;
    [SerializeField] private float   hauntOutSpeed      = 0.15f;
    [SerializeField] private float   hauntDuration      = 1.75f;
    private Collider2D hauntCollider;
    private GameObject hauntedObject;
    private float      distToHaunt;
    private float      hauntDurationCount;

    public bool inRange;
    public bool currentlyHaunting;
    public bool inHauntObj;

    private BasicMovementScript moveScript;
    public NetworkObjectReference ob_ref;

    #endregion
   
    // Start is called before the first frame update
    void Start()
    {
        hauntDurationCount = 0;

        rb         = GetComponent<Rigidbody2D>();
        col        = GetComponent<Collider2D>();
        moveScript = GetComponent<BasicMovementScript>();

        currentlyHaunting = false;
        inHauntObj        = false;

        arrow = transform.GetChild(1).gameObject;
    }

    void Update() {
        if(!IsOwner || !GameState.controlEnabled) return;

        anim.SetBool("currentlyHaunting", currentlyHaunting);
        anim.SetBool("inHauntObj", inHauntObj);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inRange  = Vector2.Distance(transform.position, mousePos) <= hauntDistanceLimit;

        if (!currentlyHaunting && !inHauntObj && Input.GetButtonDown("Fire1") && inRange) {
             checkHauntIn();
        }
        else if(!currentlyHaunting && inHauntObj && Input.GetButtonDown("Fire1") && inRange) {
            checkHauntOut();
        }

    }


    //----------------------------------------------------------
    // checkHauntIn : Checks if a collider under the mouse is on the Hauntable layer + has a Hauntable tag
    // Starts the coroutine PerformHauntIn to begin haunt
    //----------------------------------------------------------
    private void checkHauntIn() {
        hauntCollider = Physics2D.OverlapBox(mousePos, hauntCheckSize, 0, 1<<7 /*Hauntable*/);

        if (hauntCollider && hauntCollider.CompareTag("Hauntable")) {
            hauntedObject = hauntCollider.gameObject;
            originalColour = hauntedObject.GetComponent<SpriteRenderer>().color;

            StartCoroutine(PerformHauntIn(hauntedObject));
        }
    }

    //----------------------------------------------------------
    // PerformHauntIn : 
    //----------------------------------------------------------
    private IEnumerator PerformHauntIn(GameObject HauntObject) {

        inHauntObj         = false;
        currentlyHaunting  = true;
        moveScript.enabled = false;
        col.enabled        = false;
        rb.simulated       = false;



        Vector2 dirToHaunt  = calcDirToHaunt();
        //Debug.DrawRay(transform.position, dirToHaunt);
        //Debug.DrawRay(transform.position, dirToMouse);
        //Debug.Log(hauntCollider.gameObject.name);
        float   distToHaunt = calcDistToHaunt();

        while (distToHaunt >= 3) {

            dirToHaunt  = calcDirToHaunt();
            distToHaunt = calcDistToHaunt();

            float angle = Mathf.Atan2(dirToHaunt.y, dirToHaunt.x) * Mathf.Rad2Deg;
            transform.GetChild(2).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.position = new Vector3(transform.position.x + dirToHaunt.x * hauntInSpeed, transform.position.y + dirToHaunt.y * hauntInSpeed, 0);
            yield return null;
        }

        sprite.enabled = false;

        transform.position = HauntObject.transform.position;
        transform.GetChild(2).transform.rotation = Quaternion.identity;

        arrow.SetActive(true);

        inHauntObj = true;
        currentlyHaunting = false;

        if(hauntedObject.GetComponent<HauntMovementScript>()) {

            ob_ref = new NetworkObjectReference(hauntedObject);
            changeHauntOwnershipServerRpc(OwnerClientId, ob_ref, "In");

            hauntedObject.GetComponent<HauntMovementScript>().enabled = true;
            hauntedObject.GetComponent<HauntMovementScript>().knockBackForce(dirToHaunt, true);
        }


        yield return null;

    }
    [ServerRpc]
     void changeHauntOwnershipServerRpc(ulong OwnerClientId, NetworkObjectReference ob_ref, string entry)
    {
        NetworkObject net_ob;
        ob_ref.TryGet(out net_ob);

        switch(entry)
        {
            case "In" :
                gameObject.transform.parent.SetParent(net_ob.transform);
                net_ob.ChangeOwnership(OwnerClientId);
                changeHauntParametersClientRpc(ob_ref, entry);
                break;
            case "Out":
                gameObject.transform.parent.SetParent(null);
                net_ob.RemoveOwnership();
                changeHauntParametersClientRpc(ob_ref, entry);
                break;
        }

    }

    [ClientRpc]
    void changeHauntParametersClientRpc(NetworkObjectReference ob_ref, string entry)
    {
        NetworkObject net_ob;
        ob_ref.TryGet(out net_ob);

        switch(entry)
        {
            case "In" :
                sprite.enabled = false;
                net_ob.tag = "Haunted";
                break;
            case "Out":
                sprite.enabled = true;
                net_ob.tag = "Hauntable";
                break;
        }
    }

    //----------------------------------------------------------
    // checkHauntOut : 
    //----------------------------------------------------------
    private void checkHauntOut() {

//        hauntedObject.GetComponent<SpriteRenderer>().color = Color.white; // To be replaced

        hauntCollider = Physics2D.OverlapBox(mousePos, hauntCheckSize, 0, 1<<7 /*Hauntable*/);

        if(hauntCollider && hauntCollider.CompareTag("Haunted")) return ;

        changeHauntOwnershipServerRpc(OwnerClientId, ob_ref, "Out");

        if(hauntCollider && hauntCollider.CompareTag("Hauntable") ) {

            if(hauntedObject.GetComponent<HauntMovementScript>()) {
                hauntedObject.GetComponent<HauntMovementScript>().enabled = false;
                hauntedObject.GetComponent<HauntMovementScript>().knockBackForce(dirToMouse, false);
            }
                hauntedObject = hauntCollider.gameObject;
    
                StartCoroutine(PerformHauntIn(hauntedObject));
         
        } else {
            StartCoroutine(PerformHauntOut());
        }
        
    }

    //----------------------------------------------------------
    // PerformHauntOut : 
    //----------------------------------------------------------
    private IEnumerator PerformHauntOut() {
        inHauntObj        = false;
        currentlyHaunting = true;

        arrow.SetActive(false);      

        Vector2 dirToMouse = calcDirToMouse();

        if(hauntedObject.GetComponent<HauntMovementScript>()) {
            hauntedObject.GetComponent<HauntMovementScript>().enabled = false;
            hauntedObject.GetComponent<HauntMovementScript>().knockBackForce(dirToMouse, false);
        }

        hauntDurationCount = hauntDuration;

        while (currentlyHaunting) {
            if(hauntDurationCount > 0) {
                // Since Time.deltaTime is frame rate dependent, rather not use it for haunt out position determination
                hauntDurationCount -= 0.01f;

                float angle = Mathf.Atan2(dirToMouse.y, dirToMouse.x) * Mathf.Rad2Deg;
                transform.GetChild(2).transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.position = new Vector3(transform.position.x + dirToMouse.x * hauntOutSpeed, transform.position.y + dirToMouse.y * hauntOutSpeed, 0);
            }
            else {

                currentlyHaunting = false;
                col.enabled = true;

                moveScript.enabled = true;
                transform.GetChild(2).transform.rotation = Quaternion.identity;

                rb.simulated = true;
                rb.velocity  = new Vector2(transform.position.x + dirToMouse.x, transform.position.y + dirToMouse.y);

                calcDirToMouse();

                if (dirToMouse.x >= 0) {
                    moveScript.isFacingRight = true;
                    transform.GetChild(2).transform.rotation = Quaternion.identity;
                }
                else {
                    moveScript.isFacingRight = false;
                    transform.GetChild(2).transform.rotation = Quaternion.AngleAxis(180, Vector3.up);;
                }
            }
            
            yield return null;
        }
            
        moveScript.isJumpCut = false;
        moveScript.isJumping = false;

        yield return null;
    }


    //----------------------------------------------------------
    // calculation methods 
    //----------------------------------------------------------

    //----------------------------------------------------------
    // calcDirToHaunt()
    //----------------------------------------------------------
    private Vector2 calcDirToHaunt() {
        Vector2 x = (Vector2)hauntedObject.transform.position - (Vector2)transform.position;
        return x.normalized;
    }

    //----------------------------------------------------------
    // calcDirToMouse()
    //----------------------------------------------------------
    private Vector2 calcDirToMouse() {
        Vector2 x = mousePos - (Vector2)transform.position;
        return x.normalized;
    }

    //----------------------------------------------------------
    // calcDistToHaunt()
    //----------------------------------------------------------
    private float calcDistToHaunt() {
        return ((Vector2)hauntedObject.transform.position - (Vector2)transform.position).magnitude;
    }


    //----------------------------------------------------------
    // Miscellaneous junk 
    //----------------------------------------------------------
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mousePos, hauntCheckSize);
    }


}
