using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMove : MonoBehaviour {

    GameObject Stick,Player,Eyes,Ground;
    ScoreUI scoreUI;
    
    //通过数组实现动画效果，动画由6张图片组成
    public Texture[] PlayerAnimation = new Texture[6];
    //控制棍子伸长的速度等级
    public int Velocity = 3;
    public int state = 0;
    float angle = 0;
    Vector3 Pos1;
    //棍子旋转标志位
    bool StickRotateSwitch = false;
    //棍子触地标志位
    bool GroundSwitch = false;
    bool PlayDeadMusic=false;
    bool HitRed = false;
    //棍子操作加锁
    int StickLock = 0,PicItem=0;
    Rigidbody PlayerRigidbody;

    Vector3 OriPos;
    Vector3 StickTopPoint;
    public GameObject prefab_Ground;
    GameObject ColliderGround;

    MusicControll MusicManager;
	void Start () {
        Stick = GameObject.Find("StickParent").gameObject;
        Player = GameObject.Find("Player").gameObject;
        Eyes = GameObject.Find("Eyes").gameObject;
        Ground= GameObject.Find("FirstGround").gameObject;
        PlayerRigidbody = Player.GetComponent<Rigidbody>();
        scoreUI = gameObject.GetComponent<ScoreUI>();
        MusicManager = Eyes.GetComponent<MusicControll>();
    }
	void Update () {
        if(state==0)
        {
            //第一次摁下鼠标左键就将最初为零的标志位加1，只有标志位为1时棍子才能旋转           
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                StickLock ++;
            }
            if (Input.GetKey(KeyCode.Mouse0)&&StickLock==1)
            {
                Stick.transform.localScale += new Vector3(0, Velocity * Time.deltaTime, 0);               
            }
            //伸长棍子后，延迟0.4秒再进行旋转
            if (Input.GetKeyUp(KeyCode.Mouse0)&&StickLock==1)
            {
                Invoke("StickWait", 0.4f);
            }

            if(StickRotateSwitch==true)
            {
                Stick.transform.Rotate(0, 0, -4.5f);
                angle += 4.5f;
                if (angle == 90)
                {
                    Invoke("CheckStickHitGround", 0.4f);
                    //如果已经旋转90度，则停止旋转
                    StickRotateSwitch = false;                   
                    if(GroundSwitch==false)
                    {
                        MusicManager.PlayStickFail();
                    }
                }

            }
        }
        if (state == 1)
        {
            PlayAnimation();
            //MoveTowards()是将第一个参数的坐标移动到第二个参数坐标位置，
            Player.transform.position = Vector3.MoveTowards(Player.transform.position, Pos1 + new Vector3(0, 1.167f, 0), 2*Time.deltaTime);
            if (Player.transform.position == Pos1+new Vector3(0, 1.167f, 0))
            {
                MusicManager.PlayAddScore();
                MoveStick();
            }
        }
        if (state == -1)
        {
            //float StickLong = Stick.transform.localScale.y + GroundX/2;
            PlayAnimation();
            //这里的位移代码并不是完成移动后再执行下一行，而是刚开始移动就会执行下一行
            //Player.transform.position = Vector3.MoveTowards(Player.transform.position, OriPos + new Vector3(StickLong, 0, 0), 2 * Time.deltaTime);
            //float DesX = OriPos.x + StickLong;
            Player.transform.position = Vector3.MoveTowards(Player.transform.position,StickTopPoint+new Vector3(0.1f, 0.2f, 0), 2 * Time.deltaTime);
            if (Player.transform.position.x >= StickTopPoint.x)
            {
                PlayerRigidbody.useGravity = true;
                Eyes.transform.parent = null;
            }
           
            if(Player.transform.position.y < -2)
            {
                PlayerDead();
            }
        }
	}
    /// <summary>
    /// 如果棍子旋转后与地面发生了碰撞，则修改碰撞标志位为true
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")&&HitRed ==false)
        {
            if (scoreUI.getScore()>0)
            {
                Destroy(ColliderGround.transform.parent.gameObject, 2);
            }
            if (scoreUI.getScore() == 0)
            {
                Destroy(Ground.gameObject,2);
            }
            Pos1 = other.transform.position;
            ColliderGround = other.gameObject;
            CreateGround();
            GroundSwitch = true;
            transform.GetComponent<ScoreUI>().AddScore();
            MusicManager.PlayStickHitPlatform();
        }
        else if(other.gameObject.CompareTag("Ground") && HitRed == true)
        {
            if (scoreUI.getScore() > 1)
            {
                Destroy(ColliderGround.transform.parent.gameObject, 2);
            }
            if (scoreUI.getScore() == 0)
            {
                Destroy(Ground.gameObject,2);
            }
            Pos1 = other.transform.position;
            ColliderGround = other.gameObject;
            CreateGround();
            GroundSwitch = true;
            transform.GetComponent<ScoreUI>().AddScore();
        }
        if (other.gameObject.CompareTag("Red"))
        {
            MusicManager.PlayPrefect();
            transform.GetComponent<ScoreUI>().AddScore();
            HitRed = true;
        }
       
    }
    void CheckStickHitGround()
    {

        if (GroundSwitch)//如果棍子正常触地，修改状态为1，人物前进
        {
            state = 1;
        }
        else//如果棍子没有触地，人物死亡
        {
            //在棍子完成旋转后，获取当时人物的位置，然后再加上棍子的长度，就是人物跌落的位置
            //OriPos = Player.transform.position;
            //float StickLong = Stick.transform.localScale.y + GroundX / 2;
            //print("OriPos" + OriPos);
            //print("StickLong" + StickLong);
            StickTopPoint = Stick.transform.Find("Stick").Find("TopPoint").transform.position;
            state = -1;
        }
    }
    void StickWait()
    {
        //修改标志位，开始旋转
        StickRotateSwitch = true;
    }
    /// <summary>
    /// 人物移动到下一个地面物体后，需要将棍子也移动到新的地面的右上角
    /// </summary>
    void MoveStick()
    {
        //将棍子的坐标移动到新创建的地面预制体的右上角
        Stick.transform.position = ColliderGround.transform.Find("StickPos").transform.position;
        Stick.transform.rotation = Quaternion.identity;
        Stick.transform.localScale = new Vector3(0.05f, 0.01f, 0.05f);
        //修改状态位为0
        state = 0;
        //将旋转标志位归零，否则棍子会无限旋转
        angle = 0;
        //重置地面碰撞标志位
        GroundSwitch = false;
        //重置棍子锁定位
        StickLock = 0;
        HitRed = false;
    }
    /// <summary>
    /// 实例化新的体面物体
    /// </summary>
    void CreateGround()
    {       
        Ground = Instantiate(prefab_Ground);
        Ground.transform.position = ColliderGround.transform.position + new Vector3(Random.Range(1, 2.5f), 0, 0);
        Ground.transform.Find("Ground").transform.localScale = new Vector3(Random.Range(0.2f, 1), 1, 1);
    }
    /// <summary>
    /// 通过逐帧替换人物图片来实现行走动画
    /// </summary>
    void PlayAnimation()
    {
        Player.transform.GetComponent<Renderer>().material.mainTexture = PlayerAnimation[PicItem];
        PicItem++;
       if(PicItem==5)
       {
            PicItem = 0;
       }
    }

    
    /// <summary>
    /// 游戏人数死亡
    /// </summary>
    void PlayerDead()
    {
        if (PlayDeadMusic == false)
        {
            MusicManager.PlayPlayerFall();
            PlayDeadMusic = true;
        }
        transform.GetComponent<ScoreUI>().CalculateScore();
    }

}
