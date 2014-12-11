﻿using Otter;

using OtterTutorial;
using OtterTutorial.Effects;

using System;

namespace OtterTutorial.Effects
{
    // BulletExplosion will extend ParticleEffect
    public class BulletExplosion : BulletParticle
    {
        // Used to keep track of when to remove explosion from the scene
        public const int DESTROY_FRAME = 3;

        // Sound that is played when to remove explosion from the scene
        private Sound bulletExplodeSnd = new Sound(Assets.SND_BULLET_EXPLODE);

        public BulletExplosion(float x, float y)
            : base(x, y)
        {
            destroyFrame = DESTROY_FRAME;

            // Set up our explosion animation, and play it. Lastly, set our graphic.
            sprite = new Spritemap<string>(Assets.BULLET_EXPLOSION, 32, 40);
            sprite.Add("Emit", new int[] { 0, 1, 2, 3 }, new float[] { 10f, 10f, 10f, 10f });
            sprite.Play("Emit");
            Graphic = sprite;

            bulletExplodeSnd.Play();

            // Shake the camera each time we explode
            Global.camShaker.ShakeCamera();
        }

        public BulletExplosion(float x, float y, int i)
            : this(x, y)
        {
            if (i == 1)
            {
                sprite = new Spritemap<string>(Assets.ENEMY_BULLET_EXPLOSION, 32, 40);
                sprite.Add("Emit", new int[] { 0, 1, 2, 3 }, new float[] { 10f, 10f, 10f, 10f });
                sprite.Play("Emit");
                Graphic = sprite;
            }
        }
    }
}