using System;
using TMPro;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [TextArea(15,20)]
    public string text;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if (player != null && !triggered)
        {
            triggered = true;
            foreach (string line in text.Split('\n'))
            {
                DialogueManager.instance.sentences.Enqueue(line);
            }
        }
    }
}
