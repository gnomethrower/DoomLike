using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class SimplestMantisActor : MonoBehaviour
    {
        public enum State { NONE, IDLE, WALKING, CHASING, MELEE, PAIN, DIE };

        public ActorBillboard actorBillboard;

        public float frameDuration = .13f;

        public ActorAnimation idleAnim;
        public ActorAnimation walkAnim;
        public ActorAnimation chaseAnim;
        public ActorAnimation meleeAnim;
        public ActorAnimation painAnim;
        public ActorAnimation dieAnim;

        private Transform myTransform;
        public ActorAnimation currentAnimation = null;
        public State currentActorAnimState = State.NONE;

        void Awake()
        {
            myTransform = this.GetComponent<Transform>();
        }

        void Start()
        {
            //SetCurrentAnimationState(State.IDLE);
        }

        private void OnEnable()
        {
            //SetCurrentAnimationState(State.IDLE);
        }

        private void OnValidate()
        {
            if (actorBillboard != null && actorBillboard.CurrentAnimation == null)
                SetCurrentAnimationState(currentActorAnimState);
        }

        void Update()
        {

            if (actorBillboard != null)
            {
                actorBillboard.SetActorForwardVector(myTransform.forward);
            }
        }

        public void SetCurrentAnimationState(State newState)
        {
            currentActorAnimState = newState;
            switch (currentActorAnimState)
            {

                case State.WALKING:
                    //Debug.Log(newState + " called in Walking! Current State is:" + currentActorAnimState);
                    currentAnimation = walkAnim;
                    break;

                case State.CHASING:
                    //Debug.Log(newState + " called in Chasing! Current State is:" + currentActorAnimState);
                    currentAnimation = chaseAnim;
                    break;

                case State.MELEE:
                    //Debug.Log(newState + " called in Melee! Current State is:" + currentActorAnimState);
                    currentAnimation = meleeAnim;
                    break;

                case State.PAIN:
                    //Debug.Log(newState + " called in Pain! Current State is:" + currentActorAnimState);
                    currentAnimation = painAnim;
                    break;

                case State.DIE:
                    //Debug.Log(newState + " called in OnPlayerDeath! Current State is:" + currentActorAnimState);
                    currentAnimation = dieAnim;
                    break;

                default:
                    //Debug.Log(newState + " called in Default! Current State is: " + currentActorAnimState);
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
