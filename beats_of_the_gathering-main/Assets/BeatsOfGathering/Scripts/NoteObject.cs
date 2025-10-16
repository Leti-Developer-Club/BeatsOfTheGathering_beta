using System;
using UnityEngine;
using UnityEngine.UI;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public int lane;
    public GameObject goodHitEffect;
    public GameObject perfectHitEffect;
    public GameObject missHitEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Activator"))
        {
            canBePressed = true;
            GameManager.Instance.AddActiveNote(lane, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Activator"))
        {
            if (canBePressed)
            {
                Instantiate(missHitEffect, transform.position, transform.rotation);
                GameManager.Instance.NoteMissed();
                gameObject.SetActive(false);
            }
            canBePressed = false;
            GameManager.Instance.RemoveActiveNote(lane, this);
        }
    }
}