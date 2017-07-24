using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System.Linq;

public class Battle : MonoBehaviour
{

    private SocketIOComponent socket;
    private GameObject myrole = null;
    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("Role");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("open", TestOpen);
        socket.On("error", TestError);
        socket.On("close", TestClose);
        socket.On("join", Join);
        socket.On("move", Move);
    }

    public void TestOpen(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
        socket.Emit("serverjoin");
    }

    public void Join(SocketIOEvent e)
    {
        string SocketID = e.data["socketid"].ToString();
        GameObject[] objlt = GameObject.FindGameObjectsWithTag("Role");
        GameObject obj = objlt.Where(a => a.name.Contains(SocketID)).FirstOrDefault();

        if (obj == null)
        {
            GameObject o = new GameObject();
            o = Instantiate(GameObject.Find("Role"));
            if (myrole == null)
            {
                myrole = o;
            }
            o.SetActive(true);
            o.name = SocketID;
            o.transform.position = new Vector3(Random.Range(8, -8), Random.Range(3, -3), 0f);
            //o.transform.parent = Camera.main.transform;
            socket.Emit("serverjoin");
        }
    }
    public void Move(SocketIOEvent e)
    {
        string SocketID = e.data["socketid"].ToString();
        GameObject[] objlt = GameObject.FindGameObjectsWithTag("Role");
        GameObject obj = objlt.Where(a => a.name.Contains(SocketID)).FirstOrDefault();
        float x = float.Parse(e.data["x"].ToString());
        float y = float.Parse(e.data["y"].ToString());
        if (obj != null && myrole != null && myrole.name != obj.name)
        {
            if (x != 0)
            {
                obj.transform.position = new Vector3(x * Time.deltaTime, obj.transform.position.y, 0);
            }
            else
            {
                obj.transform.position = new Vector3(obj.transform.position.x, y * Time.deltaTime, 0);
            }

        }
    }

    public void ServerMove(float x, float y)
    {
        JSONObject json = new JSONObject();
        json.AddField("x", x);
        json.AddField("y", y);
        socket.Emit("servermove", json);
    }
    void FixedUpdate()
    {
        //w键前进  
        if (Input.GetKey(KeyCode.W))
        {
            myrole.transform.Translate(new Vector3(0, 0.5f * Time.deltaTime, 0));
            ServerMove(0, myrole.transform.position.y);
        }
        //s键后退  
        if (Input.GetKey(KeyCode.S))
        {
            myrole.transform.Translate(new Vector3(0, -0.5f * Time.deltaTime, 0));
            ServerMove(0, myrole.transform.position.y);
        }
        //a键后退  
        if (Input.GetKey(KeyCode.A))
        {
            myrole.transform.Translate(new Vector3(-0.5f * Time.deltaTime, 0, 0));
            ServerMove(myrole.transform.position.x, 0);
        }
        //d键后退  
        if (Input.GetKey(KeyCode.D))
        {
            myrole.transform.Translate(new Vector3(0.5f * Time.deltaTime, 0, 0));
            ServerMove(myrole.transform.position.x, 0);
        }
    }
    // Update is called once per frame
    void Update()
    {
       
    }

    public void TestError(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
    }

    public void TestClose(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
    }
}
