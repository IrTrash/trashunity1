using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    
    public int x,y;
    public bool able = false, custom = false;
    public blocksystem bsys;


    // Start is called before the first frame update
    void Start()
    {
        if (bsys == null)
        {
            bsys = GameObject.Find("system").GetComponent<blocksystem>();
        }
        if (bsys == null)
        {
            Debug.LogError("wall is created but there is no block system so this wall component is destroyed");
            Destroy(this);
        }


        if(!custom) //따로 위치지정된게아니면
        {
            x = (int)transform.position.x;
            y = (int)transform.position.y;
        }               
        if(gameObject != null) //실체가 없을떄도 약간 임시적으로 벽을 만들수도있는 그런.. 
        {
            able = true;
        }


        bsys.setwall(this);
    }

    



    public void xyupdate(int destx, int desty)
    {
        x = destx;
        y = desty;
    }
}
