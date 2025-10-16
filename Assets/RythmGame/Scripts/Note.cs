using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class Note : MonoBehaviour, IPointerDownHandler
{
    public bool canBePressed = false;

    //public KeyCode keyToPress;
    public GameObject goodHitEffect, perfectHitEffect, missHitEffect;

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
                    InstantiateParticle(goodHitEffect);
                    GameManager.Instance.GoodHit();
                }


                /* To remove later
                else if (Math.Abs(transform.position.y) > 0.05f)
                {
                    Debug.Log("Good Hit");
                    Instantiate(goodHitEffect, transform.position, transform.rotation);
                    GameManager.Instance.GoodHit();
                }
                */
                else
                {
                    Debug.Log("Perfect Hit");
                    InstantiateParticle(perfectHitEffect);
                    GameManager.Instance.PerfectHit();
                }

            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.CountNotes();

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
            InstantiateParticle(missHitEffect);
            GameManager.Instance.NoteMissed();
            canBePressed = false;
            Debug.Log("Missed");

        }
    }

//Manages particle instantiation and destruction
    private void InstantiateParticle(GameObject particle)
    {
        GameObject p = Instantiate(particle, transform.position, transform.rotation);
        Destroy(p, 1f);
    }
}