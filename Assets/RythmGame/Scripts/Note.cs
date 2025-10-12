using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Note : MonoBehaviour, IPointerDownHandler
{
    public bool canBePressed = false;

    //public KeyCode keyToPress;
    public GameObject normalHitEffect, goodHitEffect, perfectHitEffect, missHitEffect;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {



            Debug.Log("Left Click");
            //Destroy(gameObject);

            if (canBePressed)
            {
                gameObject.SetActive(false);

                if (Math.Abs(transform.position.y) > 0.25f)
                {
                    Debug.Log("Normal Hit");
                    //Instantiate(normalHitEffect, transform.position, transform.rotation);
                    //GameManager.Instance.NormalHit();
                }
                else if (Math.Abs(transform.position.y) > 0.05f)
                {
                    Debug.Log("Good Hit");
                    //Instantiate(goodHitEffect, transform.position, transform.rotation);
                    //GameManager.Instance.GoodHit();
                }
                else
                {
                    Debug.Log("Perfect Hit");
                    //Instantiate(perfectHitEffect, transform.position, transform.rotation);
                    //GameManager.Instance.PerfectHit();
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        canBePressed = true;
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Activator")
        {
            //Instantiate(missHitEffect, transform.position, transform.rotation);
            //GameManager.Instance.NoteMissed();
            canBePressed = false;
            Debug.Log("Missed");

        }
    }
}