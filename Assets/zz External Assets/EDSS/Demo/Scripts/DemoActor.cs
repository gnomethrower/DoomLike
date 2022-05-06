using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class DemoActor : MonoBehaviour
    {
        public enum State { NONE, IDLE, WALKING, SHOOT, PAIN, DIE};

        public ActorBillboard actorBillboard;

        public ActorAnimation idleAnim;
        public ActorAnimation walkAnim;
        public ActorAnimation shootAnim;
        public ActorAnimation painAnim;
        public ActorAnimation dieAnim;


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
            if (actorBillboard != null)
            {
                actorBillboard.SetActorForwardVector(myTransform.forward);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                State nextState = currentState;
                switch (currentState)
                {
                    case State.NONE:
                        nextState = State.IDLE;
                        break;

                    case State.IDLE:
                        nextState = State.WALKING;
                        break;

                    case State.WALKING:
                        nextState = State.SHOOT;
                        break;

                    case State.SHOOT:
                        nextState = State.PAIN;
                        break;

                    case State.PAIN:
                        nextState = State.DIE;
                        break;

                    case State.DIE:
                        nextState = State.IDLE;
                        break;

                    default:
                        nextState = State.IDLE;
                        break;
                }

                SetCurrentState(nextState);
            }
           
        }

        private void SetCurrentState(State newState)
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
