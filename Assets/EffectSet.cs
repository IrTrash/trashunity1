using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectSet : MonoBehaviour
{
    public bool active = false;
    public Effect dest;
    public string type = "none";
    public string str;
    public int[] i;
    public float[] f;

    // Start is called before the first frame update
    void Update()
    {
        if(!active)
        {
            return;
        }
        proc();
        Destroy(this);    

    }


    private void proc()
    {
        if(dest == null)
        {
            return;
        }

        int time = 0    ;
        int[] ibuf = null;
        if (i != null)
        {
            if(i.Length < 1)
            {
                time = 1;
            }
            else
            {
                time = i[0]; //i[0] 이 시간
            }
            
            if (i.Length > 1)
            {
                ibuf = new int[i.Length - 1];
                for (int n = 0; n < i.Length - 1; n++)
                {
                    ibuf[n] = i[n + 1];
                }
            }

           
        }

        switch (type)
        {
            case "add":
                {
                    if(str == null)
                    {
                        break;
                    }                                            
                    dest.add(str, time, ibuf, f);
                }
                break;

            case "set":
                {

                }
                break;
        }
    }
}