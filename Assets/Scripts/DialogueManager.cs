using System;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences = new();
    public float displayRate = 6f;
    public float progress = 0f;
    public List<GameObject> dialogueObjects;
    public TextMeshProUGUI tmp;
    public static DialogueManager instance;

    void Start()
    {
        instance = this;
    }
    public void Update()
    {
        if (sentences.Count > 0)
        {
            string curr = sentences.Peek();
            progress += displayRate * Time.deltaTime;
            progress = Mathf.Min(progress, curr.Length);
        }
        if (Input.GetMouseButtonDown(0) && sentences.Count > 0)
        {
            string curr = sentences.Peek();
            if (progress < curr.Length)
            {
                progress = curr.Length;
            }
            else
            {
                sentences.Dequeue();
                progress = 0f;
            }
        }

        foreach (GameObject obj in dialogueObjects)
        {
            obj.SetActive(sentences.Count > 0);
        }

        if (sentences.Count > 0)
        {
            string curr = sentences.Peek();
            string slice = curr.Substring(0, (int)progress);
            tmp.text = slice;
        }
    }
}