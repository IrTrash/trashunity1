using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;

public class unitpattern : MonoBehaviour
{
    public int type = 1, delay = 0;
    public const int normalunit = 1, building = 2;
    



    Unit uinfo;
    UnitAction action;
    private bool valid;
    public int state = 1;
    public float sightrange=3,reactrange=3,tracerange=5; //시야 거리, 반응 거리, 추적 거리

    List<UnitAction.act> candidates = new List<UnitAction.act>();
    UnitAction.act decision = null;

    public GameObject enemytarget, allytarget;
    class information
    {
        public string type = "none", name = null;
        public Dictionary<string, object> msg = new Dictionary<string,object>();


        public information(string type, string name, KeyValuePair<string,object>[] msg)
        {
            if(type != null)
            {
                this.type = type;
            }
            if(name != null)
            {
                this.name = name;
            }

            if(msg != null)
            {
                foreach (KeyValuePair<string, object> m in msg)
                {
                    this.msg.Add(m.Key, m.Value);
                }
            }
            
        }

        public bool addmsg(string msgtype, object msgvalue)
        {
            if(msgtype == null)
            {
                return false;
            }


            msg.Add(msgtype, msgvalue);
            return true;
        }

        
        public object find(string dest)
        {
            if(dest == null)
            {
                return null;
            }


            return msg[dest];
        }

        public object find(string dest,string objtype)
        {
            if(dest == null || objtype == null)
            {
                return null;
            }

            object r = msg[dest];

            return r.GetType().ToString() == objtype ? r : null;
        }
 

       public bool compare(information dest)
        {
            if (dest == null)
            {
                return false;
            }
            else if (dest.type == null)
            {
                return false;
            }



            return dest.name == name;
        }


        public int addmsg(msg dest)
        {
            if(dest == null)
            {
                return 0;
            }
            else if(dest.count() < 1)
            {
                return 0;
            }

            int r = 0;
            foreach(msgitem i in dest.getmsgitem())
            {
                addmsg(i.type, i.o);
                r++;
            }
            return r;
        }

    };

    List<information> incident = new List<information>();


    class priorityvar
    {
        public string name;
        public object msg;
        public int p;

        public priorityvar(string s, object o, int i) 
        {
            name = s;
            msg = o;
            p = i;
        }
    }

    class priority
    {
        List<priorityvar> msg = new List<priorityvar>();


        public priority()
        {

        }


        public bool add(string name, object msg, int p)
        {
            if(name == null || msg == null)
            {
                return false;
            }

            
            this.msg.Add(new priorityvar(name,msg,p));
            return true;
        }
        
        public priorityvar getrandom_high()
        {
            if(msg.Count < 1)
            {
                return null;
            }

            int h = msg[0].p;
            foreach(priorityvar pv in msg)
            {
                if( h < pv.p)
                {
                    h = pv.p;
                }
            }

            List<priorityvar> buf = new List<priorityvar>();
            foreach(priorityvar pv in msg)
            {
                if(pv.p == h)
                {
                    buf.Add(pv);
                }
            }

            return buf[UnityEngine.Random.Range(0, buf.Count)];
        }

        public priorityvar getrandom_priority()
        {
            if(msg.Count < 1 )
            {
                return null;
            }

            int r = 0;
            foreach(priorityvar pv in msg)
            {
                if(pv == null)
                {
                    continue;
                }

                r += pv.p;
            }

            r = UnityEngine.Random.Range(1, r + 1);
            foreach(priorityvar pv in msg)
            {
                r -= pv.p;
                if(r <= 0)
                {
                    return pv;
                }
            }

            //??
            return null;
        }
    }



    private void Start()
    {
        valid = loadresource();
    }


    private bool loadresource()
    {
        bool r = true;
        uinfo = gameObject.GetComponent<Unit>();
        action = gameObject.GetComponent<UnitAction>();

        r = uinfo != null && action != null;


        if(uinfo != null)
        {
            sightrange = uinfo.sightrange;
        }
        return r;
    }


    private void FixedUpdate()
    {
        if(!valid)
        {
            valid = loadresource();
            return;
        }
        else if ( delay > 0)
        {
            delay--;
                return;
        }


        switch(state)
        {
            case 1: //자기 개인 용무
                if(!personalact()) 
                {
                    state = 2;
                }
                break;

            case 2: //할게 없으면 주변상황 등에서 정보를 얻어 할 거 찾기
                if(makedecision())
                {
                    //decision을 저장할까?
                    state = 1;
                }
                else
                {

                }
                break;


            default:
                state = 1;
                break;
        }




        
    }


    private bool personalact()
    {
        if(!uinfo.canaction) //행동불가면 행동을 할 수 없으니까 굳이 진행할 필요없음
        {
            return true;
        }

        if(decision != null)
        {
            //타겟에 관한 행동을 하는 중인데 타겟이 없어진다면 모든 행동을 취소해야 할 필요가 있는거같음
            //타겟이 없을때 하는 행동은 아직 구상이 안됬으므로 아군타겟 적타겟 둘다 없으면 그냥 취소하는걸로
            if(enemytarget == null && allytarget == null)
            {
                action.add(new UnitAction.act("stop",0,null,null));
                decision = null;
                return false;
            }


            //있을 경우 decision을 action에 추가 후 true 리턴
            if (action.actnum <= 0 && uinfo.canaction)
            {
                action.add(decision);
                decision = null;
            }            
            return true;
        }

        //하고있는걸 멈춰야 하는지 판단, 그렇지않으면 그냥 기다림
        bool istimetostop = false;
        //멈춰야 하는지 판단할 정보는 incident(직접 여기서 수집한 것이 아닌 다른 시점에서 자신의 게임오브젝트가 받은 것 중에 의미있는것(데미지 입음 등)을 저장하는 곳임)
        foreach(information i in incident)
        { 
            //현재 상태와 i를 바탕으로 istimetostop을 결정(true가 되면 바로 break)
            switch(i.type)
            {
                case "phit": //투사체 피격
                    {
                        //투사체를 보낸 유닛의 정보 수집
                        GameObject attacker = (GameObject)i.find("owner", "GameObject");

                        //타겟이 없는데 추적거리 안에 있으면 추적 밑 공격

                    }
                    break;
            }

            
            if(istimetostop)
            {
                break;
            }
        }

        if(istimetostop)
        {
            action.add("stop", 0, null, null);
            //멈추고 infomation에 저장 후 false를 리턴하여 infomation을 바탕으로 한 makedecision 유도



            return false;
        }

        //아무것도 하는것도 할 것도 없으면 할 것을 찾기 위해 false 리턴
        return false;
    }


    private bool makedecision()
    {
        candidates.AddRange(makedecisions(gatherinfo()));

        if(candidates.Count <= 0)
        {
            return false;
        }

        switch(type)
        {
            case normalunit:
                {
                    //우선도
                    priority p = new priority();
                    foreach(UnitAction.act c in candidates)
                    {
                        switch(c.type)
                        {
                            case "useweapon":
                                {
                                    p.add("unitact", c, 3);
                                }
                                break;

                            case "movedest":
                                {
                                    p.add("unitact", c, 2);
                                }
                                break;


                            default:
                                {
                                    p.add("unitact", c, 1);
                                }
                                break;
                        }
                    }

                    decision = (UnitAction.act)p.getrandom_priority().msg;
                    Debug.Log("Decision set");
                }
                break;

            case building:
                {

                }
                break;

           
        }

        candidates.Clear();

        return true;
    }



    private UnitAction.act[] makedecisions(information[] info)
    {
        List<UnitAction.act> r = new List<UnitAction.act>();
        
        switch(type)
        {
            case normalunit :
                {
                    
                    normalunit_decision(info, r);
                }
                break;

            case building:
                {

                }                
                break;


            default:
                break;
        }



        return r.ToArray() ;
    }



    private information[] gatherinfo()
    {
        List<information> r = new List<information>();
        
        switch(type)
        {
            case normalunit:
                {


                    //주변 유닛 찾기
                    Vector2 pos = new Vector2(transform.position.x, transform.position.y);
                    Collider2D[] obj = Physics2D.OverlapCircleAll(pos, sightrange);
                    foreach(Collider2D c in obj)
                    {
                        var o = c.gameObject;
                        if(o.GetInstanceID() == gameObject.GetInstanceID()) //자신은 제외
                        {
                            continue;
                        }

                        if(o.tag == "Unit")
                        {
                            //올바른 유닛인지에 대한 검사는 나중에 구현

                            information newinfo = new information("Unitinfo", o.name, null);
                            r.Add(newinfo);
                            newinfo.addmsg("gameobject", o);
                            Debug.Log("Unit information has been added to unitpattern");
                        }
                    }


                    //공격을 받았거나 등의 피사건이 있는지 확인
                    //보통은 피격되거나 했을 시점에서 unitpattern 컴포넌트를 받아서 incident에 저장하는 식으로 하는게 좋을듯
                    foreach(information i in incident)
                    {
                        if(i == null)
                        {
                            continue;
                        }

                        switch (i.type)
                        {

                        }
                        
                    }
                }
                break;
        }


        return r.ToArray();
    }


    
    private bool normalunit_decision(information[] info, List<UnitAction.act> dest) //우선은 이렇게 하드코딩을 하고 나중에 뭔가 작성할 수 있는 스크립트 시스템을 짜는걸로..
    {
        if(info == null || dest == null)
        {
            return false;
        }

        Debug.Log("normalunit_decision started");

        //정보 처리 -> 처리된 정보를 바탕으로 action을 생성하여 dest에 추가
        List<GameObject> enemylist = new List<GameObject>();

        foreach (information i in info)
        {

            switch (i.type)
            {
                case "Unitinfo":
                    {
                        object o = i.find("gameobject");
                        if (o == null)
                        {
                            continue;
                        }
                        else if (o.GetType().Name != "GameObject")
                        {
                            continue;
                        }

                        GameObject gobj = (GameObject)o;
                        Unit ui = gobj.GetComponent<Unit>();
                        if (ui == null)
                        {
                            continue;
                        }

                        if (ui.team != uinfo.team) //팀이 다르면
                        {
                            enemylist.Add(gobj);
                            Debug.Log("enemylist added");
                        }


                    }
                    break;

                case "attackinfo": //적의 총알 등의 공격
                    {

                    }
                    break;

                case "damageinfo": //피해를 입었을 때의 정보
                    {

                    }
                    break;
            }
        }

        //normalunit의 행동 리스트 : 적이 있을경우 공격 및 추적, 공격받았을 경우 공격자 추적, 범위 밖이라면 공격자 쪽으로 이동, 


        //먼저 공격할 적을 추림
        //이미 공격중인 타겟이 건재하면 앵간해선 안 바꾸도록
        //근데 적 타겟이 건물이거나 할떄 적 유닛이 있으면 그쪽으로 바꾸는것도 좋을듯
        float x = gameObject.transform.position.x, y = gameObject.transform.position.y;
        float ex = 0, ey = 0;
        if (enemytarget != null)
        {
            ex = enemytarget.transform.position.x;
            ey = enemytarget.transform.position.y;

            float d = Mathf.Sqrt((ex - x) * (ex - x) + (ey - y) * (ey - y));
            if (d > tracerange)
            {
                enemytarget = null;
            }

            
            //다른 타겟리스트와 현재 타겟을 비교하여 더 우선순위를 확인하여 변경 등의 조치

        }

        if (enemytarget == null)
        {
            float mindistance = tracerange, d;
            foreach (GameObject e in enemylist) //일단 제일 가까운 애를 고르는걸로
            {
                ex = e.transform.position.x;
                ey = e.transform.position.y;
                d = Mathf.Sqrt((ex - x) * (ex - x) + (ey - y) * (ey - y));
                if (mindistance >= d)
                {
                    mindistance = d;
                    enemytarget = e;
                }
            }
        }

        UnitAction.act actbuf;
        //적 추적 및 공격 액션 생성          
        if (enemytarget != null)
        {
            //usewapon 액션 생성
            List<weapon> wplist = new List<weapon>(gameObject.GetComponents<weapon>());
            bool nowp = wplist.Count < 1; //weapon이 없음
            bool outofrange = true; //weapon은 있는데 모두 사정거리 외, 이 경우에는 적에게 접근해야함
            bool nowpavailable = true; //모든 weapon이 쿨다운 등의 이유로 사용불가. 이 경우는 도망가거나 무빙을 치거나 해서 시간을 벌어야겠지

            float range = 0;
            weapon outrangedwp;
            float distance = Mathf.Sqrt((ex - x) * (ex - x) + (ey - y) * (ey - y));
            foreach (weapon wp in wplist)
            {
                if (wp == null)
                {
                    continue;
                }

                if (!wp.able())
                {
                    nowpavailable = nowpavailable && true;
                    continue;
                }
                else
                {
                    nowpavailable = false;
                }

                if (wp.range < distance)
                {
                    outrangedwp = wp;
                    if (range <= 0)
                    {
                        range = wp.range;
                    }
                    outofrange = outofrange && true;
                    continue;
                }
                else
                {
                    outofrange = false;

                }


                actbuf = new UnitAction.act("useweapon", 1, new int[] { getwpindex(wp) }, new float[] { ex, ey });
                dest.Add(actbuf);
            }


            if (nowpavailable || nowp)
            {
                //무-빙
                dest.Add(new UnitAction.act("moverandom", 1, null, new float[] { 1 }));
                
            }
            else if (outofrange) //그냥 else로해도되나
            {
                //적을 따라감
                float allowrange = 0.5f; //무조건 사정거리 끝에 걸리게 이동하는게 정답은 아닐 수 있으므로 더 가까이 가도록 할려하는데 일단 고민중임
                float movedistance = distance - range + allowrange;


                dest.Add(new UnitAction.act("movedest", 1, null, new float[] { ex, ey, movedistance }));
            }


            if(dest.Count < 1 )
            {
                Debug.Log("enemytarget is exist but no action");
            }
        }



            //이 메소드는 일단 뭘 할지에 대한 리스트를 작성하는것이므로 결정 자체는 다른 메소드에서..     

        return true;
    }






    private int getwpindex(weapon dest)
    {
        if(dest == null)
        {
            return -1;
        }



        int r = 0;
        foreach(weapon wp in gameObject.GetComponents<weapon>())
        {
            if(wp.GetInstanceID() == dest.GetInstanceID())
            {
                return r;
            }
            r++;
        }


        return -2;
    }
  


    private weapon getwpbyindex(int dest)
    {
        if(dest < 0)
        {
            return null;
        }

        int i = 0;
        foreach(weapon wp in gameObject.GetComponents<weapon>())
        {
            if(i++ == dest)
            {
                return wp;
            }
        }

        return null;
    }


    public weapon getwp(weapon dest) => getwpbyindex(getwpindex(dest));//현제 게임오브젝트에 있어야 받아옴




    public bool addincident(msg dest)
    {
        if(dest == null)
        {
            return false;
        }

        information i = new information(dest.type, (string)dest.get("name", "string"), null);
        dest.remove("name");


        i.addmsg(dest);
        incident.Add(i);
        return true;
    }


}

