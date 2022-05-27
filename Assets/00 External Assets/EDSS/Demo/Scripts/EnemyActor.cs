using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class EnemyActor : MonoBehaviour
    {
        public enum State { NONE, IDLE, WALKING, SHOOT, PAIN, DIE };

        public ActorBillboard actorBillboard;
        public Carl_State_Script carlStateScript;
        public Mortality_Script mortalityScript;

        public ActorAnimation idleAnim;
        public ActorAnimation walkAnim;
        public ActorAnimation shootAnim;
        public ActorAnimation painAnim;
        public ActorAnimation dieAnim;

        bool switchToIdle = false;
        bool switchToWalking = false;
        bool switchToPain = false;
        //bool switchToShooting = false;
        //bool switchToPain = false;
        //bool switchToDie = false;

        bool currentlyWalking = false;
        bool currentlyIdle = false;

        private Transform myTransform;
        private ActorAnimation currentAnimation = null;
        private State currentState = State.NONE;

        void Awake()
        {
            myTransform = GetComponent<Transform>();
        }

        void Start()
        {
            SetCurrentState(State.IDLE);
        }

        private void OnEnable()
        {
            SetCurrentState(State.IDLE);
        }

        private void OnValidate()
        {
            if (actorBillboard != null && actorBillboard.CurrentAnimation == null)
                SetCurrentState(currentState);
        }


        void Update()
        {
            ActorListener();

            if (actorBillboard != null)
            {
                actorBillboard.SetActorForwardVector(myTransform.forward);
            }

            if (switchToWalking)
            {
                switchToWalking = false;
                SetCurrentState(State.WALKING);
            }

            if (switchToIdle)
            {
                switchToIdle = false;
                SetCurrentState(State.IDLE);
            }

            if (switchToPain)
            {
                switchToPain = false;
                SetCurrentState(State.PAIN);
            }
        }

        void ActorListener()
        {
            //Debug.Log(currentlyWalking);
            //WalkingListener
            if (carlStateScript.isWalking && !currentlyWalking && carlStateScript.state != 1)
            {
                currentlyWalking = true;
                currentlyIdle = false;

                switchToWalking = true;
            }

            if (carlStateScript.state == 1 && !currentlyIdle)
            {
                currentlyWalking = false;
                currentlyIdle = true;

                switchToIdle = true;
            }

            if (carlStateScript.state == 5)
            {
                switchToPain = true;
            }
        }

        void SetCurrentState(State newState)
        {
            currentState = newState;
            switch (currentState)
            {

                case State.WALKING:
                    currentAnimation = walkAnim;
                    break;

                case State.SHOOT:
                    currentAnimation = shootAnim;
                    break;

                case State.PAIN:
                    currentAnimation = painAnim;
                    break;

                case State.DIE:
                    currentAnimation = dieAnim;
                    break;

                default:
                    currentAnimation = idleAnim;
                    break;
            }

            if (actorBillboard != null)
            {
                actorBillboard.PlayAnimation(currentAnimation);
            }
        }

    }
}

