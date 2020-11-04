using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class system : MonoBehaviour
{
    private datasystem dsys;
    private List<instruct> inst;

    public List<Unit> unitlist = new List<Unit>();
    public bool unitflag = false;

    private void Start()
    {
        dsys = new datasystem(gameObject);
        inst = new List<instruct>();
        unitlistupdate();
    }



    private void FixedUpdate()
    {
        proc();    
    }

    public void proc()
    {
        if(unitflag)
        {
            unitlistupdate();
            unitflag = false;
        }

        if(inst == null)
        {
            return;
        }

        foreach(instruct i in inst)
        {
            if(i == null)
            {
                continue;
            }
            
            
            i.proc(); 
        }
    }


    private void unitlistupdate()
    {
        unitlist.Clear();
        GameObject[] list = GameObject.FindGameObjectsWithTag("Unit");
        foreach(GameObject o in list)
        {
            Unit u = o.GetComponent<Unit>();
            if(u == null)
            {
                continue;
            }
            unitlist.Add(u);
        }
    }


    public bool addnewunit(Unit dest)
    {
        if(dest == null)
        {
            return false;
        }
        else if(unitlist.Exists(u=> u == dest))
            {
            return false;
        }


        unitlist.Add(dest);
        return true;
    }


    public bool delunit(Unit dest)
    {
        return unitlist.Remove(dest);
    }

}
