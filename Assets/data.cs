using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;



[System.Serializable]
public class data
{

    public string type,name;

    public class msg
    {
        public string type;
        public int[] i;
        public float[] f;
        public char[][] str;
    }


    public msg[] msglist = null;
    

    public data(Unit unit)
    {
        if(unit == null)
        {
            type = "failed";
            return;
        }

        
    }


    public bool copy(data dest)
    {
        if(dest == null)
        {
            return false;
        }



        type = dest.type;
        name = dest.name;

        if(dest.msglist != null) //일단은 잘모르겠으니 얕은 복사로
        { 
            msglist = dest.msglist;
        }

        return true;
    }



    public bool load(string src)
    {
        if(src == null)
        {
            return false;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Stream input = File.Open(src, FileMode.Open);
        data buf = (data)bf.Deserialize(input);
        input.Close();


        return copy(buf);
    }


    public bool save(string src)
    {
        if(src == null)
        {
            return false;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Stream input = File.Create(src);
        bf.Serialize(input, this);
        input.Close();


        return true;
    }
}
