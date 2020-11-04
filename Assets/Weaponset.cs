using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weaponset : MonoBehaviour
{
    //Weapon data

    public weapon dest;
    public string type = "none";
    public GameObject o;
    public float speed= 1,movedirec=0, movedistance=0, adddirec=0; //위치변경할 방향,거리 , 방향변위

    private void Start()
    {

        if(dest == null)
        {
            dest = GetComponent<weapon>();
        }

        if(dest != null)
        {
            dest.addset(this);
        }
    }

    public GameObject create(float x, float y,float dx, float dy)
    {

        switch(type)
        {
            case "Bullet":
            case "bullet":
                if ( o == null)
                {
                    break;
                }

                GameObject r = Instantiate(o, new Vector3(x,y,0), Quaternion.identity);
                
                projectile prj = r.GetComponent<projectile>();
                if(prj != null)
                {
                    prj.owner = gameObject;
                    prj.speed = speed;
                    prj.setdest(dx, dy);
                }

                EffectSet[] effectSets = r.GetComponents<EffectSet>();
                Effect e = r.GetComponent<Effect>();
                foreach(EffectSet es in effectSets)
                {
                    es.dest = e;
                    es.active = true;
                }

                return r;

            default:
                return null;
        }


        return null;
    }
}
