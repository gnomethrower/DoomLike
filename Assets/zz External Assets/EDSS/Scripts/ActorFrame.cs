
//=============================================================================
//  ActorFrame
//  by Mariusz Skowroński from Healthbar Games (http://healthbargames.pl)
//
//  This class describes single frame of EDSS animation.
//  It contains eight sprites (0 .. 7) - one for each of eight world directions.
//  For animations that has only one-direction sprites we use only first sprite
//  from this array.
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EightDirectionalSpriteSystem
{
    [System.Serializable]
    public class ActorFrame
    {
        [SerializeField]
        private Sprite[] sprites;

        public ActorFrame()
        {
            sprites = new Sprite[8];
        }

        public ActorFrame(ActorFrame other)
        {
            sprites = new Sprite[8];
            for (int i = 0; i < 8; i++)
                sprites[i] = other.sprites[i];
        }

        public ActorFrame(Sprite[] sprites)
        {
            this.sprites = new Sprite[8];
            for (int i = 0; i < 8; i++)
                this.sprites[i] = sprites[i];
        }

        public ActorFrame(Sprite sprite)
        {
            this.sprites = new Sprite[8];
            for (int i = 1; i < 8; i++)
                this.sprites[i] = null;

            sprites[0] = sprite;
        }

        public void SetSprite(int direction, Sprite sprite)
        {
            if (sprites == null)
                return;

            sprites[direction % 8] = sprite;
        }

        public Sprite GetSprite(int direction)
        {
            if (sprites == null)
                return null;

            return sprites[direction % 8];
        }
    }
}
