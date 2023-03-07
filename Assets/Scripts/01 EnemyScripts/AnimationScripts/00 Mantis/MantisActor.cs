using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class MantisActor : MonoBehaviour
    {
        public enum State { NONE, IDLE, AGGRO, WALKING, SPRINTING, HURT, JUMPANTICIPATION, INAIR, ATTACK, DEATH };

        public ActorBillboard actorBillboard;
        public MantisEnemyAI mantisEnemyAI;
        public NavMeshAgent navMeshAgent;

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
            navMeshAgent = GetComponentInParent<NavMeshAgent>();
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


        void LateUpdate()
        {

            ActorListener();

            if (actorBillboard != null)
            {
                actorBillboard.SetActorForwardVector(myTransform.forward);
            }

            if (switchToWalking && currentAnimation != walkAnim)
            {
                switchToWalking = false;
                SetCurrentState(State.WALKING);
                //Debug.Log("Switching to Walking is " + switchToWalking);
            }

            if (switchToIdle && currentAnimation != idleAnim)
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
                if (navMeshAgent.speed > 0 && mantisEnemyAI.animationStateInteger == 1) //mantisEnemyAI.animationStateInteger == 1 && currentAnimation != walkAnim && !switchToWalking)
            {
                    switchToWalking = true;
                    //Debug.Log("Switching to Walking is " + switchToWalking);
                }

                if (mantisEnemyAI.animationStateInteger == 0 && currentAnimation != idleAnim && !switchToIdle)
                {
                    switchToIdle = true;
                }
        }

        void SetCurrentState(State newState)
        {
            currentState = newState;
            switch (currentState)
            {

                case State.WALKING:
                    Debug.Log("1: Setting currentAnim to Walking Animation at: " + Time.realtimeSinceStartup);
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

