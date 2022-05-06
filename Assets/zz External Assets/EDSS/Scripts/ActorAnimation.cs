
//=============================================================================
//  ActorAnimation
//  by Mariusz Skowroński from Healthbar Games (http://healthbargames.pl)
//
//  This class represents one animation like "stand", "walk", "shoot", "die", ...
//  It contains list of frames and function for getting right sprite for
//  particular frame of animation and direction in which actor is facing.
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EightDirectionalSpriteSystem
{
    [ExecuteInEditMode]
    [CreateAssetMenu(fileName="NewActorAnimation", menuName="EDSS/Actor Animation", order=8100)]
    public class ActorAnimation : ScriptableObject
    {
        public enum AnimMode { Once, Loop, PingPong }

        public enum AnimDirType { OneDirection, EightDirections }

        public string Name
        {
            get { return animName; }
        }

        public AnimDirType AnimType
        {
            get { return animType;  }
        }

        public AnimMode Mode
        {
            get { return animMode;  }
        }

        public List<ActorFrame> FrameList
        {
            get { return frameList;  }
        }

        public float Speed
        {
            get { return speed; }
        }

        public int FrameCount
        {
            get
            {
                if (frameList == null)
                    return 0;
                else
                    return frameList.Count;
            }
        }


        [SerializeField]
        private string animName = "New Actor Animation";

        [SerializeField]
        private AnimDirType animType = AnimDirType.OneDirection;

        [SerializeField]
        private AnimMode animMode = AnimMode.Once;

        [SerializeField]
        private float speed = 1.0f;

        [SerializeField]
        private List<ActorFrame> frameList = new List<ActorFrame>(1);

        public Sprite GetSprite(int frame, int direction)
        {
            if (frameList == null | frameList.Count == 0)
                return null;

            frame = frame % frameList.Count;

            if (AnimType == AnimDirType.OneDirection)
            {
                direction = 0;
            }

            ActorFrame actorFrame = frameList[frame];
            if (actorFrame != null)
            {
                return actorFrame.GetSprite(direction);
            }
            else
            {
                return null;
            }
        }
    }
}
