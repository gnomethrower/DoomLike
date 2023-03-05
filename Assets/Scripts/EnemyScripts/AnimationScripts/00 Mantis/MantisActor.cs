using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class MantisActor : MonoBehaviour
    {
        public enum State { NONE, IDLE, AGGRO, WALKING, SPRINTING, HURT, JUMPANTICIPATION, INAIR, ATTACK, DEATH };

        public ActorBillboard actorBillboard;
        public MantisEnemyScript mantisEnemyScript;

        public ActorAnimation idleAnim;
        public ActorAnimation aggroAnim;
        public ActorAnimation walkAnim;
        public ActorAnimation sprintAnim;
        public ActorAnimation hurtAnim;
        public ActorAnimation jumpAnticipationAnim;
        public ActorAnimation InAirAnim;
        public ActorAnimation attackAnim;
        public ActorAnimation deathAnim;

        bool switchToIdle = false;
        bool switchToWalking = false;
        bool switchToHurt = false;
        
        /*
        bool switchToSprinting = false;
        bool switchToJumpAnticipation = false;
        bool switchToJumpInAir = false;
        bool switchToAttack = false;
        bool switchToDeath = false;
        bool currentlyIdle = false;
        bool currentlyWalking = false;
        bool currentlySprinting = false;
        bool currentlyInAir = false;
        */

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

            if (switchToHurt)
            {
                switchToHurt = false;
                SetCurrentState(State.HURT);
            }
        }

        void ActorListener()
        {
            /*
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
                switchToHurt = true;
            }
            */
        }

        void SetCurrentState(State newState)
        {
            currentState = newState;
            switch (currentState)
            {

                case State.WALKING:
                    currentAnimation = walkAnim;
                    break;

                case State.ATTACK:
                    currentAnimation = attackAnim;
                    break;

                case State.HURT:
                    currentAnimation = hurtAnim;
                    break;

                case State.DEATH:
                    currentAnimation = deathAnim;
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

