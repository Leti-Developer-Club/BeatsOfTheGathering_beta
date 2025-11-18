using UnityEngine;

public class npc_controller : MonoBehaviour
{

    private int isCelebratingHash = Animator.StringToHash("IsCelebrating");

    public enum NPCState
    {
        Idle,
        Celebrating,
        Mad,

    }

    private Animator animator;
    public NPCState currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentState = NPCState.Idle;
    }

    // Update is called once per frame
    void Update()
    {

        if(GameManager.Instance.celebrationReached)
        {
            currentState = NPCState.Celebrating;
        }
        else
        {
            currentState = NPCState.Idle;
        }
        switch (currentState)
        {
            case NPCState.Idle:
                animator.SetBool(isCelebratingHash, false);
                //animator.Play("Idle");
                break;
            case NPCState.Celebrating:
                animator.SetBool(isCelebratingHash, true);
                //animator.Play("Celebrate");
                break;
            case NPCState.Mad:
                animator.Play("Mad");
                break;
        }

    }
}
