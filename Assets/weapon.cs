using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class weapon : MonoBehaviour
{
    private GameObject owner;
    private Unit ownerinfo;

    public int range=3, plevel=1, charge=20, cast=10, delay=30, cooldown=20;
    private List<Weaponset> wpset;
    public int state = 0;
    public int statetime=0,cooltime=0;
    public float destx=0, desty=0;

    // Start is called before the first frame update
    void Start()
    {
        owner = gameObject;
        if(owner != null)
        {
            ownerinfo = owner.GetComponent<Unit>();
        }
        if(wpset == null)
        {
            wpset = new List<Weaponset>();
        }
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        proc();
    }


    public void createresult(float dx , float dy)
    {
        

        float x = owner.transform.position.x, y = owner.transform.position.y;
        foreach (Weaponset wp in wpset)
        {
            GameObject result = wp.create(x, y, dx, dy);
        }
   
        
    }


    public bool addset(Weaponset dest)
    {
        if(dest == null)
        {
            return false;
        }

        if(wpset == null )
        {
            wpset = new List<Weaponset>();
        }      
        wpset.Add(dest);

        return true;
    }


    private void proc()
    {
        switch(state)
        {
            case 0: //normal
                {
                    if(cooltime > 0)
                    {
                        cooltime--;
                    }
                }
                break;

            case 1: //charge
                {
                    if(statetime <= 0)
                    {
                        state = 2;
                        statetime = cast;
                        ownerinfo.mainstate = "weaponusing";
                        ownerinfo.mainstatetime = cast;
                        break;
                    }
                    statetime--;
                }
                break;
            

            case 2: //cast
                {
                    if(statetime <= 0)
                    {
                        createresult(destx, desty);
                        cooltime = cooldown;
                        state = 0;
                        ownerinfo.mainstate = "weapondelay";
                        ownerinfo.mainstatetime = delay;
                        break;
                    }

                    statetime--;
                }
                break;

        }
    }



    public bool able()
    {
        if(state != 0 || cooltime > 0)
        {
            return false;
        }


        return true;
    }


    public void start(float dx, float dy)
    {
        if(!able())
        {
            return;
        }


        state = 1;
        statetime = charge;
        destx = dx;
        desty = dy;
        ownerinfo.mainstate = "charging";
        ownerinfo.mainstatetime = charge;
    }


    public void cancle() // 액션 중지(스턴이나 캔슬)
    {
        state = 0;
        statetime = 0;
    }

}
