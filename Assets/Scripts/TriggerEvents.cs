using System;
using Dialogues;
using Missions;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerEvents : MonoBehaviour
{
    public string enterTag;
    public bool triggerOnce;
    public UnityEvent onEnter;
    public UnityEvent onExit;
    private bool _enterTriggered = false;
    private bool _exitTriggered = false;

    private void Start()
    {
        if (!GetComponent<Collider2D>().isTrigger)
        {
            Debug.LogWarning($"Collider on {name} isn't set to trigger. Events will never be fired");
        }
    }

    public void UpdateMissionProgress(Mission mission)
    {
        if(mission.Complete) return;
        if (!mission.Unlocked)
        {
            _enterTriggered = false;
            _exitTriggered = false;
            return;
        }
        mission.UpdateProgress();
        if (mission.dialogueOnComplete != null)
        {
            DialogueManager.Singleton.BeginDialogue(mission.dialogueOnComplete);
        }
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_enterTriggered && triggerOnce) return;
        if (other.CompareTag(enterTag))
        {
            onEnter?.Invoke();
            _enterTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(_exitTriggered && triggerOnce) return;
        if (other.CompareTag(enterTag))
        {
            onExit?.Invoke();
            _exitTriggered = true;
        }
    }
}