using System;
using System.Collections.Generic;
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
            DialogueManager.instance.SetDialogue(new List<string>(text.Split("\n")));
        }
    }
}
