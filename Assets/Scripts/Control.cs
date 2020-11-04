using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class Control : MonoBehaviour
{
    public List<GameObject> list;
    // Start is called before the first frame update
    
    public class act
    {

    };



    // Update is called once per frame
    void Update()
    {
        keydown();
        mouse();
    }


    private void keydown()
    {
        string direc = null;
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direc = "left";
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            direc = "right";
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            direc = "up";
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            direc = "down";
        }


        foreach(GameObject obj in list)
        {
         
            if (obj == null)
            {
                continue;
            }


            Unit info = obj.GetComponent<Unit>();
            if (info == null)
            {
                continue;
            }
            else if (info.unittype != "unit" && info.unittype != "Unit")
            {
                continue;
            }

            
            UnitAction action = obj.GetComponent<UnitAction>();
            if (action == null)
            {
                continue;
            }

            if (direc != null)
            {
                int[] i = { action.stringtointdirec(direc) };
                action.add("move4direc", 50, i, null);
            }

        }

        

        if(Input.GetKeyDown(KeyCode.A))
        {
            foreach(GameObject obj in list)
            {
                if(obj == null)
                {
                    continue;
                }

                Unit info = obj.GetComponent<Unit>();
                if (info == null)
                {
                    continue;
                }
                else if (info.unittype != "building")
                {
                    continue;
                }


                UnitAction action = obj.GetComponent<UnitAction>();
                if (action == null)
                {
                    continue;
                }

                int[] i = {0};
                action.add("startunitbuild", 1, i, null);
                Debug.Log("unit1 build action is added");
            }
        }


        
    }

    private void mouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lmousedown(mpos.x, mpos.y);
        }


        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rmousedown(mpos.x, mpos.y);
        }


    }

    private void lmousedown(float x, float y)
    {
        foreach(GameObject obj in list)
        {
            if(obj == null)
            {
                continue;
            }


            Unit info = obj.GetComponent<Unit>();
            if(info == null)
            {
                continue;
            }

            switch(info.unittype)
            {
                case "unit":
                case "Unit":
                    {
                        UnitAction action = obj.GetComponent<UnitAction>();
                        if (action == null)
                        {
                            continue ;
                        }
                        
                        int[] i = { 0 };
                        float[] f = { x, y };
                        action.add("useweapon", 1, i, f);
                    }
                    break;
            }
        }
    }


    private void rmousedown(float x, float y)
    {
        foreach(GameObject obj in list)
        {
            if(obj == null)
            {
                continue;
            }

            Unit info = obj.GetComponent<Unit>();
            if(info == null)
            {
                continue;
            }
            switch (info.unittype)
            {
                case "unit":
                case "Unit":
                    {
                        
                        UnitAction action = obj.GetComponent<UnitAction>();
                        if( action == null)
                        {
                            continue;
                        }

                        action.add("movedest", 1, null,new float[] { x,y,2});
                    }
                    break;
            }
        }
    }

}
