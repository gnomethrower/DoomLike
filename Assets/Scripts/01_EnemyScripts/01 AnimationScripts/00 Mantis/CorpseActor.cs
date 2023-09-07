using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    public class CorpseActor : MonoBehaviour
    {
        public enum State { CORPSE };

        public ActorBillboard actorBillboard;

        public ActorAnimation corpseAnim;


        public float frameDuration = .13f;

        private Transform myTransform;
        public ActorAnimation currentAnimation = null;
        public State currentActorAnimState = State.CORPSE;

        void Awake()
        {
            myTransform = GetComponent<Transform>();
        }

        void Start()
        {
            SetCurrentAnimationState(State.CORPSE);
        }

        private void OnEnable()
        {
            SetCurrentAnimationState(State.CORPSE);
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
                default:
                    //Debug.Log(newState + " called in Default! Current State is: " + currentActorAnimState);
                    currentAnimation = corpseAnim;
                    break;
            }

            if (actorBillboard != null)
            {
                actorBillboard.PlayAnimation(currentAnimation);
            }
        }

    }
}
