﻿using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class Snow_con : MonoBehaviour
{
    /////<summary></summary>

    /// <summary>マップデータを受け取る参照先</summary>
    Map_con Map_ob = null;

    /// <summary>降る雪の数</summary>
    const int Max_fallsnows = 100;
    /// <summary>積もる雪の分割数</summary>
    const int Split_falledsnow = 25;
    /// <summary>雲の大きさ</summary>
    const float CloudSize_x = 1.000f;

    ///<summary>生成する雲</summary>
    [SerializeField] Sprite spr_cloud = null;
    ///<summary>生成する降る雪</summary>
    [SerializeField] Sprite spr_snow = null;
    ///<summary>生成する積もる雪</summary>
    [SerializeField] Sprite spr_f_snw = null;

    ///<summary>ループ処理を止める</summary>
    bool snow_task = false;
    /// <summary>生成した積もる雪</summary>
    GameObject[] falled_snow = null;
    /// <summary>生成した積もる雪を入れる親オブジェクト</summary>
    GameObject Master_falled_snow = null;
    /// <summary>生成した降る雪</summary>
    GameObject[] fall_snow = new GameObject[Max_fallsnows];
    ///<summary>各降る雪の落下速度</summary>
    float[] fall_spd = new float[Max_fallsnows];
    ///<summary>各降る雪の状態（0 = 停止 , 1 = 落下 , 2 = 停止待機）</summary>
    int[] fall_mode = new int[Max_fallsnows];
    /// <summary>生成した降る雪を入れる親オブジェクト</summary>
    GameObject Master_fall_snow = null;
    /// <summary>生成した雲</summary>
    GameObject Cloud = null;

    //マップの大きさ
    /// <summary>マップの大きさ　X</summary>
    const int map_size_x = 32;
    /// <summary>マップの大きさ　Y</summary>
    const int map_size_y = 18;
    ///<summary>積もる雪を置く位置</summary>
    int[,] snw_pos = new int[4, Split_falledsnow]{
        {25,25,24,23,22,20,19,19,17,17,16,15,14,13,12,11,10,9,8,6,6,5,4,2,1 },
        {1,2,3,4,5,7,8,9,10,11,12,13,14,15,16,17,17,19,19,20,22,23,24,25,25 },
        {14,15,15,16,16,17,17,17,18,18,19,19,20,21,21,21,22,22,23,23,24,24,24,25,25 },
        {1,1,2,3,3,3,4,4,5,6,7,7,7,8,8,9,9,10,10,11,11,11,12,13,14 }
    };
    ///<summary>積もる雪の数</summary>
    int fed_snw;
    ///<summary>積もる雪の位置</summary>
    Vector3[] fed_snw_pos;
    ///<summary>ブロックに分けたときの積もった雪の最小値</summary>
    float[] falled_min = new float[map_size_x];
    ///<summary>最小値を求める</summary>
    void Falled_min_task(){
        for(int lu = 0; lu < map_size_x; ++lu){
            float min = 0.0f;
            for(int na = 0; na < Split_falledsnow; ++na){
                min = falled_snow[lu * Split_falledsnow + na].transform.localScale.y > min ? falled_snow[lu * Split_falledsnow + na].transform.localScale.y : min;
            }
        }
    }
    public float Read_falled_min(int x){
        return falled_min[x];
    }
    /// <summary>マップから必要なデータをもらう</summary>
    void Set_Map_con(){
        Map_ob = GetComponent<Map_con>();
    }
    /// <summary>積もる雪を準備</summary>
    void Set_falledSnows(){
        fed_snw = map_size_x * Split_falledsnow;

        Vector3[] fed_snw_pos_sub = new Vector3[fed_snw];
        int[] fed_snw_num_sub = new int[fed_snw];
        for(int lu = 0; lu < map_size_x; ++lu){
            fed_snw_pos_sub[lu] = Map_ob.Read_fed_snw_pos(lu);
            fed_snw_num_sub[lu] = Map_ob.Read_fed_snw_num(lu);
        }
        Master_falled_snow = new GameObject("falled_snow");
        fed_snw_pos = new Vector3[fed_snw];
        falled_snow = new GameObject[fed_snw];
        for(int lu = 0; lu < map_size_x; ++lu){
            for(int na = 0; na < Split_falledsnow; ++na){
                falled_snow[lu * Split_falledsnow + na] = new GameObject("snow");
                falled_snow[lu * Split_falledsnow + na].AddComponent<SpriteRenderer>().sprite = spr_f_snw;
                falled_snow[lu * Split_falledsnow + na].GetComponent<SpriteRenderer>().sortingOrder = 1;
                falled_snow[lu * Split_falledsnow + na].transform.parent = Master_falled_snow.transform;
                float sub = 1.0f / Split_falledsnow;
                falled_snow[lu * Split_falledsnow + na].transform.localScale = new Vector3(sub, 0.0f, 0.0f);
                switch (fed_snw_num_sub[lu]){
                    case 0:
                        falled_snow[lu * Split_falledsnow + na].transform.position = new Vector3(fed_snw_pos_sub[lu].x - 0.5f + sub * na + sub / 2, fed_snw_pos_sub[lu].y, 0.0f);
                        break;
                    case 10:
                        falled_snow[lu * Split_falledsnow + na].transform.position = new Vector3(fed_snw_pos_sub[lu].x - 0.5f + sub * na + sub / 2, fed_snw_pos_sub[lu].y, 0.0f);
                        break;
                    case 11:
                        falled_snow[lu * Split_falledsnow + na].transform.position = new Vector3(fed_snw_pos_sub[lu].x - 0.5f + sub * na + sub / 2, fed_snw_pos_sub[lu].y, 0.0f);
                        break;
                    case 12:
                        falled_snow[lu * Split_falledsnow + na].transform.position = new Vector3(fed_snw_pos_sub[lu].x - 0.5f + sub * na + sub / 2, fed_snw_pos_sub[lu].y, 0.0f);
                        break;
                    case 13:
                        falled_snow[lu * Split_falledsnow + na].transform.position = new Vector3(fed_snw_pos_sub[lu].x - 0.5f + sub * na + sub / 2, fed_snw_pos_sub[lu].y, 0.0f);
                        break;
                    default:
                        falled_snow[lu * Split_falledsnow + na].transform.position = new Vector3(fed_snw_pos_sub[lu].x - 0.5f + sub * na + sub / 2, fed_snw_pos_sub[lu].y + sub * snw_pos[fed_snw_num_sub[lu] - 3, na], 0.0f);
                        break;
                }
                fed_snw_pos[lu * Split_falledsnow + na] = falled_snow[lu * Split_falledsnow + na].transform.position;

            }
        }
    }
    /// <summary>雲と降る雪を準備</summary>
    void Set_fallsnows(){
        Cloud = new GameObject("cloud");
        Cloud.AddComponent<SpriteRenderer>().sprite = spr_cloud;
        Cloud.GetComponent<SpriteRenderer>().sortingOrder = 1;
        Cloud.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        Master_fall_snow = new GameObject("fall_snows");
        for(int lu = 0; lu < Max_fallsnows; ++lu){
            fall_snow[lu] = new GameObject("snowflake");
            fall_snow[lu].AddComponent<SpriteRenderer>().sprite = spr_snow;
            fall_snow[lu].transform.position = new Vector3(-1.0f, 1.0f, 0.0f);
            fall_snow[lu].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            fall_snow[lu].transform.parent = Master_fall_snow.transform;
            fall_spd[lu] = Random.Range(0.050f, 0.100f);
            fall_mode[lu] = 0;
        }
    }

    /// <summary>積もる雪と降る雪と雲を消す</summary>
    public void Destroy_allsnow(){
        Destroy(Master_fall_snow.gameObject);
        Destroy(Master_falled_snow.gameObject);
        Destroy(Cloud.gameObject);
        snow_task = false;
    }
    /// <summary>降る雪の処理</summary>
    Vector3 plus_snow = new Vector3(0.0f, 0.015f, 0.0f);
    /// <summary>降る雪の処理</summary>
    void Fallsnows_task(){
        Vector3 c_pos = Cloud.transform.position;
        for(int lu = 0; lu < Max_fallsnows; ++lu){
            if (fall_mode[lu] != 0){
                fall_snow[lu].transform.position -= new Vector3(0.0f, fall_spd[lu], 0.0f);
                if (fall_snow[lu].transform.position.y < -map_size_y){
                    if (fall_mode[lu] == 2){
                        fall_snow[lu].transform.position = new Vector3(-1.0f, 1.0f, 0.0f);
                        fall_mode[lu] = 0;
                    }
                    else{
                        fall_snow[lu].transform.position = new Vector3(c_pos.x + Random.Range(-CloudSize_x, CloudSize_x), c_pos.y, 0.0f);
                    }
                }
                if (fall_snow[lu].transform.position.x > -0.5f && fall_snow[lu].transform.position.x < map_size_x){
                    int suba = Mathf.FloorToInt(fall_snow[lu].transform.position.x + 0.5f);
                    int subb = Mathf.FloorToInt((fall_snow[lu].transform.position.x - (suba - 1) - 0.5f) / (1.0f / Split_falledsnow));
                    int subc = Split_falledsnow * suba + subb;
                    if ((subc >= 0 && subc < Split_falledsnow * map_size_x) && fall_snow[lu].transform.position.y < falled_snow[subc].transform.position.y + falled_snow[subc].transform.localScale.y / 2.0f){
                        if(falled_snow[subc].transform.localScale.y + plus_snow.y<Cloud.transform.position.y - fed_snw_pos[subc].y){
                            falled_snow[subc].transform.localScale += plus_snow;
                        }
                        falled_snow[subc].transform.position = new Vector3(fed_snw_pos[subc].x, fed_snw_pos[subc].y + falled_snow[subc].transform.localScale.y / 2.0f, 0.0f);
                        if (fall_mode[lu] == 2){
                            fall_snow[lu].transform.position = new Vector3(-1.0f, 1.0f, 0.0f);
                            fall_mode[lu] = 0;
                        }
                        else{
                            fall_snow[lu].transform.position = new Vector3(c_pos.x + Random.Range(-CloudSize_x, CloudSize_x), c_pos.y, 0.0f);
                        }
                    }
                }
            }
        }
    }
    ///<summary>雪を降らす</summary>
    void Start_fall_snows(){
        Vector3 c_pos = Cloud.transform.position;
        for(int lu = 0; lu < Max_fallsnows; ++lu){
            if (fall_mode[lu] == 0){
                fall_snow[lu].transform.position = new Vector3(c_pos.x + Random.Range(-CloudSize_x, CloudSize_x), c_pos.y, 0.0f);
                fall_mode[lu] = 1;
            }
        }
    }
    ///<summary>雪を止ます（今降っている分は降り続ける）</summary>
    void  Stop_fall_snows(){
        for(int lu = 0; lu < Max_fallsnows; ++lu){
            fall_mode[lu] = 2;
        }
    }
    ///<summary>雪を止める</summary>
    void Stop_fall_snows_c(){
        for(int lu = 0; lu < Max_fallsnows; ++lu){
            fall_mode[lu] = 0;
        }
    }
    ///<summary>雪をリセット</summary>
    public void Reset_fall_snows(){
        for(int lu = 0; lu < Max_fallsnows; ++lu){
            fall_snow[lu].transform.position = new Vector3(-1.0f, 1.0f, 0.0f);
            fall_mode[lu] = 0;
        }
    }
    ///<summary>カメラ</summary>
    Camera maincam;
    ///<summary>カメラを取得</summary>
    void Set_camera(){
        maincam = Camera.main;
    }
    /// <summary>雲を動かす処理</summary>
    void Move_clouds_task(){
        Vector3 m_position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(maincam.transform.position.z));
        Vector3 w_position = Camera.main.ScreenToWorldPoint(m_position);
        int m_x = Mathf.FloorToInt(w_position.x), m_y = Mathf.FloorToInt(w_position.y + 0.5f);
        Cloud.transform.position = new Vector3(m_x + 0.5f , -3, 0.0f);
        if (Input.GetMouseButtonDown(0)) { Start_fall_snows(); }
        if (Input.GetMouseButtonUp(0)) { Stop_fall_snows(); }
    }
    ///<summary>マップ作成時に実行</summary>
    public void Set_all_snows(){
        Set_falledSnows();
        Set_fallsnows();
        snow_task = true;
    }
    // Start is called before the first frame update
    void Start(){
        Set_Map_con();
        Set_camera();
    }

    // Update is called once per frame
    void Update(){
        if (snow_task){
            Fallsnows_task();
            Falled_min_task();
            Move_clouds_task();
        }
    }
}
