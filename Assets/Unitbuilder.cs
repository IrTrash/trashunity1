using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class Unitbuilder : MonoBehaviour
{
    public Unitbuildinfo current;
    public int currenttime, endtime;
    public int destx, desty;

    public List<Unitbuildinfo> list;
    private Queue<int> que;
    

    private void Start()
    {
        list = new List<Unitbuildinfo>();
        updatelist();
        que = new Queue<int>();
    }


    public void updatelist()
    {
        if(list == null)
        {
            return;
        }


        list.Clear();
        foreach(Unitbuildinfo bi in gameObject.GetComponents<Unitbuildinfo>())
        {
            list.Add(bi);
        }
    }


    private void FixedUpdate()
    {
        proc();
    }

    void proc()
    {
        if(current != null)
        {
            if(currenttime >= endtime)
            {
                GameObject buf = current.createresult(transform.position.x, transform.position.y);
                if(buf != null)
                {
                    
                    Unit bufinfo = buf.GetComponent<Unit>(), myinfo = gameObject.GetComponent<Unit>();
                    bufinfo.team = myinfo.team;
                    //추가 처리

                    //렐리 포인트
                    //bufinfo.reserveaction("movedestend", 1, null, new float[] { destx, desty }, null);

                }

                current = null;
                currenttime = 0;
                 
                //que pop
                if(que.Count > 0)
                {
                    int index = que.Dequeue();
                    if(index < list.Count)
                    {
                        current = list[index];
                        endtime = current.time;
                    }
                }
            }
            else
            {
                currenttime++;
            }
        }
    }

    public bool able()
    {
        return current == null;
    }


    public bool getstarted(int index)
    {
        if(!able() || index  >= list.Count)
        {
            return false;
        }


        
        if(current == null)
        {
            current = list[index];
            if(current != null)
            {
                currenttime = 0;
                endtime = current.time;
            }
            else
            {
                return false;
            }           
        }
        else
        {
            que.Enqueue(index);
        }


        

        return true;
    }
}
