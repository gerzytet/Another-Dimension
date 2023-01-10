
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chair : MonoBehaviour
{
    private bool inRange = false;
    private bool sit = false;
    public string dialogue1 = "What was that noise?";
    public string dialogue2 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
    public GameObject explosionPoint;
    private float oldSpeed;
    private AudioSource introAudioSource;
    void Start() 
    {
        introAudioSource = GetComponent<AudioSource>();
    }
    void explodeEverything()
    {
        var objects = FindObjectsOfType<BoxCollider>();
        List<GameObject> toScatter = new List<GameObject>();
        //toScatter.Add(Player.instance.gameObject);
        foreach (BoxCollider box in objects)
        {
            if (box.isTrigger)
            {
                continue;
            }
            var obj = box.gameObject;
            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                obj.AddComponent<Rigidbody>();
            }
            toScatter.Add(obj);
        }

        foreach (GameObject obj in toScatter)
        {
            obj.GetComponent<Rigidbody>().AddForce(explosionPoint.transform.position - obj.transform.position);
        }
    }

    IEnumerator ExplosionCutscene()
    {
        introAudioSource.Play();
        DialogueManager.instance.SetDialogue(new List<string>() {dialogue1});
        DialogueManager.instance.BlurDialogue();
        yield return new WaitForSeconds(5);
        DialogueManager.instance.SetDialogue(new() {dialogue2});
        explodeEverything();
        Player.instance.fallThreshold = -1000;
        sit = false;
        Player p = Player.instance;
        p.GetComponent<Rigidbody>().useGravity = true;
        DialogueManager.instance.enableSkip = false;
        yield return new WaitForSeconds(3);
        DialogueManager.instance.HideDialogue();
        yield return new WaitForSeconds(4);
        DialogueManager.instance.enableSkip = true;
        while (p.transform.position.y < 1500)
        {
            p.GetComponent<Rigidbody>().AddForce(Vector3.up * 50);
            yield return new WaitForFixedUpdate();
        }
        DontDestroyOnLoad(Player.instance);
        DontDestroyOnLoad(transform.parent.gameObject);
        p.GetComponent<Rigidbody>().velocity = Vector3.zero;
        var op = SceneManager.LoadSceneAsync("Scenes/Level 1 - Moon");
        while (!op.isDone)
        {
            yield return null;
        }

        Vector2 xz(Vector3 xyz)
        {
            return new Vector2(xyz.x, xyz.z);
        }
        while (Vector2.Distance(xz(p.transform.position) ,xz(p.respawnPoint)) > 0.1f)
        {
            float x = Mathf.MoveTowards(transform.position.x, p.respawnPoint.x, 1f);
            float z = Mathf.MoveTowards(transform.position.z, p.respawnPoint.z, 1f);
            p.transform.position = new Vector3(x, p.transform.position.y, z);
            yield return new WaitForFixedUpdate();
        }
        p.speed = oldSpeed;
        Destroy(gameObject);
    }
    void Update()
    {
        Player p = Player.instance;
        if (inRange && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Sit");
            oldSpeed = p.speed;
            p.speed = 0f;
            p.GetComponent<Rigidbody>().useGravity = false;
            p.transform.position = transform.position - new Vector3(0f, 0f, 0.5f);
            sit = true;
            StartCoroutine(ExplosionCutscene());
        }

        if (sit)
        {
            p.transform.position = transform.position - new Vector3(0f, 0f, 0.5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            inRange = false;
        }
    }
}
