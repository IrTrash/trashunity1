using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blocksystem : MonoBehaviour
{
    List<Wall> wlist = new List<Wall>();
    system sys;


    // Start is called before the first frame update
    void Start()
    {
        sys = GetComponent<system>();
        if(sys == null)
        {
            GameObject sysobj = GameObject.Find("system");
            if(sysobj != null)
            {
                sys = sysobj.GetComponent<system>();
            }
        }
    }

    private void FixedUpdate()
    {
        proc();
    }

    private void proc()
    {
        if(sys == null)
        {
            return;
        }

        foreach(Unit u in sys.unitlist)
        {
            float ux = u.transform.position.x, uy = u.transform.position.y;
            if(isblocked(ux,uy))
            {
                u.block();
            }
        }
    }


    public bool isblocked(float x, float y)
    {
        foreach(Wall w in wlist)
        {
            if (Mathf.Abs(w.x - x) < 1 && Mathf.Abs(w.y - y) < 1)
            {
                return true;
            }
        }

        return false;
    }



    public bool setwall(Wall dest)
    {
        if(dest == null)
        {
            return false;
        }

        if(wlist.Exists(w=> w == dest))
        {
            return false;
        }


        wlist.Add(dest);
        return true;
    }


    public bool removewall(Wall dest) => wlist.Remove(dest);


    public (string,int,float) findwayone(int sx, int sy,int dx,int dy,float speed)
    {

        //고민중..
        return ("none", 0, 1);
    }
}
