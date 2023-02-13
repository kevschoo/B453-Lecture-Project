using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControls : MonoBehaviour
{
    [field: SerializeField] public Team EntityTeam { get; set; }
    [field: SerializeField] public Base MainBase { get; set; }
    [field: SerializeField] public List<GameObject> Flags { get; set; } //List of Flags
    [field: SerializeField] public GameObject SelectedObj { get; set; }
    [field: SerializeField] public GameObject NearestFlag { get; set; } // Mostly useless var
     enum ObjectType {Base, Billion, Flag, NullObject};

    [field: SerializeField] public LineRenderer LineRender { get; set; }


    [field: SerializeField] public bool UseAnyBase { get; set; } = true; // Testing and Godmode vars
    [field: SerializeField] public bool UseAnyFlag { get; set; } = true; // Testing and Godmode vars
    [field: SerializeField] public bool FindFlagFromAnywhere { get; set; } = true; // Testing and Godmode vars
    Camera mainCam;
    Ray2D ray;
    [SerializeField] ObjectType currentObjectType;
    [SerializeField] Vector2 MousePos;
    [SerializeField] bool RClickDown = false;
    [SerializeField] bool LClickDown = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        LineRender.startColor = Color.red; LineRender.endColor = Color.white;
        LineRender.startWidth = 0.25f; LineRender.endWidth = 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        MousePos = new Vector2(this.mainCam.ScreenToWorldPoint(Input.mousePosition).x, this.mainCam.ScreenToWorldPoint(Input.mousePosition).y);

        if(SelectedObj != null && currentObjectType == ObjectType.Flag && RClickDown)
        {   
            LineRender.gameObject.SetActive(true);
            LineRender.SetPosition(0, new Vector3 (MousePos.x,MousePos.y, 1f));
            LineRender.SetPosition(1, new Vector3 (SelectedObj.transform.position.x, SelectedObj.transform.position.y, 1f));
        }
        else
        { 
            LineRender.gameObject.SetActive(false);
        }
        
        //Middle Click Selects Team from base 
        //Middle Click Disables Flag
        //Middle Click Deletes Disabled Flags
        //Clears Selected object and flag placing logic 
        if (Input.GetButtonDown("MiddleClick"))
        {
            Vector2 raycastPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            new Ray(raycastPos, Vector2.zero);
            RaycastHit2D[] hits = Physics2D.RaycastAll(raycastPos, Vector2.zero);

            if (hits != null)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if(hit.collider.gameObject.GetComponentInParent<Billion>())
                    {
                        Debug.Log("Clicked Billion");
                        //SelectedObj = hit.collider.gameObject.GetComponentInParent<Billion>().gameObject;
                        currentObjectType = ObjectType.Billion;
                    }
                    else if(hit.collider.gameObject.GetComponentInParent<Base>())
                    {                        
                        Debug.Log("Clicked Base");
                        SelectedObj = hit.collider.gameObject.GetComponentInParent<Base>().gameObject;
                        Base BaseScript = SelectedObj.GetComponentInParent<Base>();
                        
                        if(BaseScript.EntityTeam.TeamID == this.EntityTeam.TeamID || this.UseAnyBase == true)
                        {
                            this.EntityTeam = BaseScript.EntityTeam;
                        }
                        currentObjectType = ObjectType.Base;
                    }
                    else if(hit.collider.gameObject.GetComponentInParent<Flag>())
                    {
                        Debug.Log("Clicked Flag");
                        SelectedObj = hit.collider.gameObject.GetComponentInParent<Flag>().gameObject;
                        //currentObjectType = ObjectType.Flag;
                        Flag FlagScript = SelectedObj.GetComponentInParent<Flag>();
                        
                        if(FlagScript.EntityTeam.TeamID == this.EntityTeam.TeamID || this.UseAnyFlag == true )
                        {
                        if(FlagScript.IsActive == true) {FlagScript.IsActive = false;}
                        else {Destroy(SelectedObj);}
                        }
                        break;
                    }
                }
            }
            if(hits.Length == 0)
            {
                Debug.Log("Hits 0" );
                SelectedObj = null;
                currentObjectType = ObjectType.NullObject;
            }
        }

        //Left Click Places Flag > Places Flag again if avaliable after clicking on a base
        //Checks for nearby flags > then moves nearest flag to cursor
        //renables flags
        if (Input.GetButtonDown("LeftClick"))
        {
            Vector2 raycastPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Left clicker");
            
            if (currentObjectType == ObjectType.Base && SelectedObj != null)
            {
                Base BaseScript = SelectedObj.GetComponentInParent<Base>();
                if(BaseScript.EntityTeam.TeamID == this.EntityTeam.TeamID || this.UseAnyBase == true)
                {
                    BaseScript.CreateFlag(this.mainCam.ScreenToWorldPoint(Input.mousePosition));
                    if(BaseScript.Flags.Count >= BaseScript.FlagLimit)
                    {
                        Debug.Log("All Flags Placed" );
                        SelectedObj = null;
                        currentObjectType = ObjectType.NullObject;
                    }
                }
                else
                {
                    SelectedObj = null;
                    currentObjectType = ObjectType.NullObject;
                }
            }
            
            if (SelectedObj == null || currentObjectType == ObjectType.Flag)
            {
                GameObject[] Flags = GameObject.FindGameObjectsWithTag("BillionFlag");
                float DistanceToFlag = Mathf.Infinity;
                foreach(GameObject curFlag in Flags)
                {
                    if(curFlag.GetComponent<Flag>().EntityTeam.TeamID == this.EntityTeam.TeamID || this.UseAnyFlag == true)
                    {
                    float distance = ((Vector2)curFlag.transform.position - raycastPos).sqrMagnitude;
                        if(distance < DistanceToFlag)
                        {
                            DistanceToFlag = distance;
                            NearestFlag = curFlag;
                        }
                    }
                }
                if(NearestFlag != null)
                {
                    NearestFlag.gameObject.transform.position = MousePos; 
                    NearestFlag.GetComponent<Flag>().IsActive = true;
                }

                SelectedObj = null;
                currentObjectType = ObjectType.NullObject;
            }
        }

        //Right Click
        //Hold Right click on a flag to preview next location and drop upon exiting hold
        if (Input.GetButtonDown("RightClick")) // This one is for one frame clicks to renable a flag
        {
            //Feels redundant but I want this code in to re enable flags without moving them
            //This first if checks to see if the spot we clicked was a flag amd we will be dragging that flag
            Vector2 raycastPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            new Ray(raycastPos, Vector2.zero);
            RaycastHit2D[] hits = Physics2D.RaycastAll(raycastPos, Vector2.zero);
            if (hits != null)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if(hit.collider.gameObject.GetComponentInParent<Flag>())
                    {
                        Debug.Log("Clicked Flag");
                        SelectedObj = hit.collider.gameObject.GetComponentInParent<Flag>().gameObject;
                        Flag FlagScript = SelectedObj.GetComponentInParent<Flag>();
                        if(FlagScript.EntityTeam == this.EntityTeam || this.UseAnyFlag == true)
                        {
                        currentObjectType = ObjectType.Flag;
                        FlagScript.IsActive = true;
                        NearestFlag = hit.collider.gameObject; 
                        }
                        else
                        {
                            SelectedObj = null;
                        }
                        break;
                    }
                }
            }
            //This if searches nearby for any flag and we will set it to be the one being dragged
            
            if (SelectedObj == null && FindFlagFromAnywhere)
            {
                GameObject[] Flags = GameObject.FindGameObjectsWithTag("BillionFlag");
                float DistanceToFlag = Mathf.Infinity;
                foreach(GameObject curFlag in Flags)
                {
                    if(curFlag.GetComponent<Flag>().EntityTeam.TeamID == this.EntityTeam.TeamID || this.UseAnyFlag == true)
                    {
                    float distance = ((Vector2)curFlag.transform.position - raycastPos).sqrMagnitude;
                        if(distance < DistanceToFlag)
                        {
                            DistanceToFlag = distance;
                            NearestFlag = curFlag;
                        }
                    }
                }
                if(NearestFlag != null)
                {
                    SelectedObj = NearestFlag;
                    Flag FlagScript = SelectedObj.GetComponentInParent<Flag>();
                    if(FlagScript.EntityTeam.TeamID == this.EntityTeam.TeamID || this.UseAnyFlag == true)
                    {
                        currentObjectType = ObjectType.Flag;
                        SelectedObj = NearestFlag;
                        FlagScript.IsActive = true;
                    }
                }
                else
                {
                SelectedObj = null;
                currentObjectType = ObjectType.NullObject;
                }
            }
            
        }

        //Sets bool and potentially other things
        if(Input.GetButton("RightClick"))
        {
            RClickDown = true;
        }
        //Sets bool and enables a flag and moves it
        if(Input.GetButtonUp("RightClick"))
        {
            RClickDown = false;
            if(SelectedObj != null && currentObjectType == ObjectType.Flag)
            {
                SelectedObj.gameObject.transform.position = MousePos; 
                SelectedObj.GetComponent<Flag>().IsActive = true;
                SelectedObj = null;
                currentObjectType = ObjectType.NullObject;
            }
        }





    }
}
