using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mytrashlib
{
    
}


public class msgitem
{
    public string name, type, objtype;
    public object o;



    public msgitem(char[] t, object o)
    {
        type = new string(t);
        this.o = o;
    }


    public msgitem(char[] n, char[] t, char[] ot, object obj)
    {
        name = new string(n);
        type = new string(t);
        objtype = new string(ot);
        o = obj;
    }


    public bool checktype(string dest)
    {
        if (dest != null && type != null)
        {
            return type.CompareTo(dest) == 0;
        }

        return false;
    }

    public bool set(char[] t, object o)
    {
        if (t != null)
        {
            return false;
        }

        type = new string(t);
        this.o = o;

        return true;
    }

}



public class msg
{
    public string type = null;
    public List<msgitem> m;


    public msg()
    {

    }

    public msg(char[] s)
    {
        type = new string(s);
    }


    public bool add(string s, object o)
    {
        if(s == null || o == null)
        {
            return false;
        }



        m.Add(new msgitem(s.ToCharArray(), o));
        return true;
    }

    public bool exist(string dest)
    {
        if(dest  == null)
        {
            return false;
        }

        foreach(msgitem i in m)
        {
            if(i.type == dest)
            {
                return true;
            }
        }

        return false;
    }

    public int count(string dest)
    {
        if(dest == null)
        {
            return 0;
        }

        int r = 0;
        foreach(msgitem i in m)
        {
            if(i.type == dest)
            {
                r++;
            }
        }

        return r;
    }

    public object get(string dest)
    {
        if (dest == null)
        {
            return null;
        }

        foreach(msgitem i in m)
        {
            if(i.type == dest)
            {
                return i;
            }
        }

        return null;
    }

    public object get(string dest, string desttype)
    {
        if (dest == null || desttype == null)
        {
            return false;
        }

        foreach (msgitem i in m)
        {
            if(i.type == dest && i.o.GetType().ToString() == desttype)
            {
                return i;
            }
        }

        return null;
    }


    public object[] getall(string dest)
    {
        if(dest == null)
        {
            return null;
        }

        List<object> r = new List<object>();
        foreach(msgitem i in m)
        {
            if(i.type == dest)
            {
                r.Add(i.o);
            }
        }

        return r.ToArray();
    }

    public msgitem getmsgitem(string dest)
    {
        if(dest == null)
        {
            return null;
        }

        
        foreach(msgitem i in m)
        {
            if(i.type == dest)
            {
                return i;
            }
        }


        return null;
    }


    public msgitem[] getmsgitems(string dest)
    {
        if(dest == null)
        {
            return null;
        }

        List<msgitem> r = new List<msgitem>();
        foreach(msgitem i in m)
        {
            if(i.type == dest)
            {
                r.Add(i);
            }
        }

        return r.ToArray();
    }

    public msgitem getmsgitem(int destindex)
    {
        if(destindex < 0 || destindex >= m.Count)
        {
            return null;
        }

        return m[destindex];
    }

    public msgitem[] getmsgitem()
    {
        return m.ToArray();
    }


    public int count(params string[] dest)
    {
        if(dest == null)
        {
            return 0;
        }
        else if(dest.Length < 1)
        {
            return 0;
        }

        int r = 0;
        foreach(msgitem i in m)
        {
            foreach(string s in dest)
            {
                if(i.type == s)
                {
                    r++;
                    break;
                }
            }
        }

        return r;
    }


    public bool exist(params string[] dest)
    {
        if(dest == null)
        {
            return false;
        }
        else if(dest.Length < 1)
        {
            return false;
        }

        bool b;
        foreach(msgitem i in m)
        {
            b = false;
            foreach(string s in dest)
            {
                if(i.type == s)
                {
                    b = true;
                    break;
                }
            }
            if(!b)
            {
                return false;
            }
        }


        return true;
    }


    public int remove(params string[] dest)
    {
        if(dest == null)
        {
            return 0;
        }
        else if(dest.Length < 1)
        {
            return 0;
        }

        int r = 0;
        List<msgitem> removelist = new List<msgitem>();
        foreach(string s in dest)
        {
            foreach(msgitem i in m)
            {
                if(i.type == s)
                {
                    removelist.Add(i);
                    break;
                }
            }
        }

        foreach(msgitem i in removelist)
        {
            m.Remove(i);
            r++;
        }


        return r;
    }

};















