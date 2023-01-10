using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> sentences = new();
    public float displayRate = 6f;
    public float progress = 0f;
    public List<GameObject> dialogueObjects;
    public TextMeshProUGUI tmp;
    public static DialogueManager instance;
    public bool enableSkip = true;
    public Image distortionImage;
    public Material broken;
    public Material blurred;
    public Material clear;
    public Image clickPrompt;

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
        if (Input.GetMouseButtonDown(0) && sentences.Count > 0 && enableSkip)
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

        clickPrompt.color = Color.clear;
        if (sentences.Count > 0)
        {
            string curr = sentences.Peek();
            string slice = curr.Substring(0, (int)progress);
            tmp.text = slice;

            if (Mathf.Abs(progress - curr.Length) < Mathf.Epsilon)
            {
                clickPrompt.color = Color.Lerp(Color.clear, Color.white, Mathf.Sin(Time.time * 5) / 4 + 0.5f);
            }
        }
    }
    
    public void SetDialogue(List<string> sentences)
    {
        this.sentences.Clear();
        foreach (string sentence in sentences)
        {
            this.sentences.Enqueue(sentence);
        }
    }

    private void SetMaterial(Material mat)
    {
        distortionImage.material = mat;
    }

    public void BlurDialogue()
    {
        SetMaterial(blurred);
    }
    
    public void HideDialogue()
    {
        SetMaterial(broken);
    }
    
    public void RestoreDialogue()
    {
        SetMaterial(clear);
    }
}