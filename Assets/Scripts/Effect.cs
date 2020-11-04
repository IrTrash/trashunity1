using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class Effect : MonoBehaviour
{
    private const int msg_varmax = 16;
   public class msg
    {
        public msg()
        {
            type = "none";
            time = 1;
            i = new int[msg_varmax];
            f = new float[msg_varmax];
        }

        public msg(msg dest)
        {
            copy(dest);
        }

        public msg(string s,int time, int[] i, float[] f)
        {
            set(s, time, i, f);
        }
        
        public string type;
        public int time;
        public int[] i;
        public float[] f;

        public void set(string s,int time, int[] i, float[] f)
        {
            if(s != null)
            {
                type = new string(s.ToCharArray());
                this.time = time;
                if(i != null)
                {
                    this.i = (int[])i.Clone();
                }
                if(f != null)
                {
                    this.f = (float[])f.Clone();
                }
            }
        }

        public bool copy(msg dest)
        {
            if(dest == null)
            {
                return false;
            }

            if(dest.type != null)
            {
                type = new string(dest.type.ToCharArray());
            }
            else
            {
                type = "none";
            }
            
            if(dest.i != null)
            {
                i = (int[])dest.i.Clone();
            }
            if(dest.f != null)
            {
                f = (float[])dest.f.Clone();
            }
            time = dest.time;

            return true;
        }

        public void reset()
        {
            type = "none";
            time = 0;
        }
    };

    private List<msg> msglist;
    public int msgnum = 0;
    private void Start()
    {
        msglist = new List<msg>();
    }





    public bool add(string name,int time, int[] i, float[] f)
    {
        if(name == null || time <= 0)
        {
            return false;
        }


        msglist.Add(new msg(name, time, i, f));
        msgnum++;
        return true;
    }

    public void load(msg[] dest)
    {
        foreach(msg m in dest)
        {
            msglist.Add(new msg(m));
        }
    }



    public msg[] getmsg()
    {
      if(msglist == null)
        {
            return null;
        }
      return msglist.ToArray();
    }
    


    public void proc()
    {
        foreach(msg m in msglist)
        {
            if(--m.time > 0)
            {
                
            }
            else
            { 
                msglist.Remove(m);
                msgnum--;
                break;
            }
        }
       
    }
}
