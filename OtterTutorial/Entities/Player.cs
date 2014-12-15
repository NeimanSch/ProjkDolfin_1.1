﻿using Otter;

using OtterTutorial.Effects;
using OtterTutorial.Scenes;

using System;

namespace OtterTutorial.Entities
{
    public class Player : Entity
    {
        public const int WIDTH = 22;
        public const int HEIGHT = 29;
        public const float DIAGONAL_SPEED = 1.4f;

        public float moveSpeed = 4.0f;

        // Our entity's graphic will be a Spritemap
        public Spritemap<string> sprite;

        public int direction = 0;
        public int health = 10;
        public int score = 0;

        public bool dead = false;

        //Weapon for testing
        public Weapon equippedWeapon;

        public Player(float x = 0, float y = 0)
        {
            // When creating a new player, the desired X,Y coordinates are passed in. If excluded, we start at 0,0
            X = x;
            Y = y;
            // Create a new spritemap, with the player.png image as our source, 32 pixels wide, and 40 pixels tall
            sprite = new Spritemap<string>(Assets.PLAYER2, WIDTH, HEIGHT);

            sprite.Add("standLeft", new int[] { 0 }, new float[] { 5f });
            sprite.Add("standRight", new int[] { 0 }, new float[] { 5f });
            sprite.Add("standDown", new int[] { 4 }, new float[] { 5f });
            sprite.Add("standUp", new int[] { 0 }, new float[] { 5f  });
            sprite.Add("walkLeft", new int[] { 0,1,2,3 }, new float[] { 5f, 5f, 5f, 5f});
            sprite.Add("walkRight", new int[] { 0, 1, 2, 3 }, new float[] { 5f, 5f, 5f, 5f });
            sprite.Add("walkDown", new int[] { 4,5,6,7 }, new float[] { 5f, 5f, 5f, 5f });
            sprite.Add("walkUp", new int[] { 0, 1, 2, 3 }, new float[] { 5f, 5f, 5f, 5f });
            sprite.Add("shootLeft", new int[] { 2,3 }, new float[] {5f,5f});

            // Tell the spritemap which animation to play when the scene starts


            // Lastly, we must set our Entity's graphic, otherwise it will not display
            Graphic = sprite;

            //Graphic.CenterOrigin();
            Graphic.OriginY = 16;
            Graphic.OriginX = 8;

            sprite.Play("standDown");
            sprite.Scale = 2f;

            SetHitbox(20, 20, (int)Global.Type.PLAYER);

            equippedWeapon = new Weapon();
            Random r = new Random();

            // Set Defaults for starting weapon
            equippedWeapon.baseDamage = r.Next(1, 2); // 1;
            equippedWeapon.bulletCount = r.Next(1, 2); // 1;
            equippedWeapon.fireRate = r.Next(35, 50); // 40;
            
            sprite.Play("standDown");

           // SetHitbox(WIDTH, HEIGHT, (int)Global.Type.PLAYER);
        }

        public override void Render()
        {
            Hitbox.Render();
            base.Render();
        }

        public override void Update()
        {

            if (Global.paused)
            {
                return;
            }

            base.Update();

            //if (score == 50)
            //{
            //    //jb - testing win game scenario
            //    Global.TUTORIAL.Scene.RemoveAll();
            //    Global.TUTORIAL.SwitchScene(new EndScene());
            //    //
            //}

            // Used to determine which directions we are moving in
            bool horizontalMovement = true;
            bool verticalMovement = true;

            float xSpeed = 0;
            float ySpeed = 0;
            float newX;
            float newY;
            GameScene checkScene = (GameScene)Scene;

            // Check horizontal movement
            if (Global.PlayerSession.Controller.Left.Down)
            {
                newX = X - moveSpeed;
                if (!checkScene.grid.GetRect(newX, Y, newX + WIDTH, Y + HEIGHT, false))
                {
                    xSpeed = -moveSpeed;
                }

                direction = Global.DIR_LEFT;
                sprite.Play("walkLeft");
                sprite.FlippedX = true;
            }
            else if (Global.PlayerSession.Controller.Right.Down)
            {
                newX = X + moveSpeed;
                if (!checkScene.grid.GetRect(newX, Y, newX + WIDTH, Y + HEIGHT, false))
                {
                    xSpeed = moveSpeed;
                }

                direction = Global.DIR_RIGHT;
                sprite.Play("walkRight");
                sprite.FlippedX = false;
            }
            else
            {
                horizontalMovement = false;
            }
            // Check vertical movement
            if (Global.PlayerSession.Controller.Up.Down)
            {
                newY = Y - moveSpeed;
                if (!checkScene.grid.GetRect(X, newY, X + WIDTH, newY + HEIGHT, false))
                {
                    ySpeed = -moveSpeed;
                }

                direction = Global.DIR_UP;
                sprite.Play("walkUp");
                sprite.FlippedX = false;
            }
            else if (Global.PlayerSession.Controller.Down.Down)
            {
                newY = Y + moveSpeed;
                if (!checkScene.grid.GetRect(X, newY, X + WIDTH, newY + HEIGHT, false))
                {
                    ySpeed = moveSpeed;
                }

                direction = Global.DIR_DOWN;
                sprite.Play("walkDown");
                sprite.FlippedX = false;
            }
            else
            {
                verticalMovement = false;
            }

            // If we are not moving play our idle animations
            // Currently our spritesheet lacks true idle
            // animations, but this helps get the idea across
            if (!horizontalMovement && !verticalMovement)
            {
                if (sprite.CurrentAnim.Equals("walkLeft"))
                {
                    sprite.Play("standLeft");
                }
                else if (sprite.CurrentAnim.Equals("walkRight"))
                {
                    sprite.Play("standRight");
                }
                else if (sprite.CurrentAnim.Equals("walkDown"))
                {
                    sprite.Play("standDown");
                }
                else if (sprite.CurrentAnim.Equals("walkUp"))
                {
                    sprite.Play("standUp");
                }
            }
            // CHECK FOR WEAPON SHOOTAN
            equippedWeapon.fire();

            // Add particles if the player is moving in any direction
            if (verticalMovement || horizontalMovement)
            {
                if (verticalMovement && horizontalMovement)
                {
                    X += xSpeed / DIAGONAL_SPEED;
                    Y += ySpeed / DIAGONAL_SPEED;
                }
                else
                {
                    X += xSpeed;
                    Y += ySpeed;
                }
            }
            var collb = Collider.Collide(X, Y, (int)Global.Type.BOSS_BULLET);
            if (collb != null)
            {
                BossBullet b = (BossBullet)collb.Entity;
                b.RemoveSelf();

                dead = true;
                Global.TUTORIAL.Scene.Add(new Explosion(X, Y, true));
                RemoveSelf();
            }

            // Access the Enemy's Collider to check collision
            var colleb = Collider.Collide(X, Y, (int)Global.Type.BULLET);
            if (colleb != null)
            {
                Bullet b = (Bullet)colleb.Entity;
                if (b.shooter == "enemy")
                {
                    b.Destroy();
                    Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y, 1));

                    DamageText dt = new DamageText(X, Y, "1");
                    Global.TUTORIAL.Scene.Add(dt);

                    //hurt.Play();

                    health--; // Decrement the health by 1 for each Bullet that hits
                    if (health <= 0)
                    {
                        // Add a new Explosion and remove self from the Scene if out of health
                        
                        dead = true;
                        Global.TUTORIAL.Scene.Add(new Explosion(X, Y, true));
                        RemoveSelf();
                    }
                }
            }

            var colli = Collider.Collide(X, Y, (int)Global.Type.ITEM);
            if (colli != null)
            {
                Item i = (Item)colli.Entity;
                if (i.attributes.ContainsKey("movementSpeed"))
                {
                    //jb- arbitrary cap on movespeed 
                    if (moveSpeed < 10)
                    {
                        moveSpeed = moveSpeed + (int)i.attributes["movementSpeed"];
                        Console.WriteLine("speed: " + moveSpeed);
                    }
                }
                
                if (i.attributes.ContainsKey("fireRate"))
                {
                    equippedWeapon.fireRate = equippedWeapon.fireRate - (int)i.attributes["fireRate"];
                    Console.WriteLine("fireRate: " + equippedWeapon.fireRate);
                }

                if (i.attributes.ContainsKey("fireRange"))
                {
                    equippedWeapon.bulletRange = equippedWeapon.bulletRange + (int)i.attributes["fireRange"];
                    Console.WriteLine("fireRange: " + equippedWeapon.bulletRange);
                }

                if (i.attributes.ContainsKey("damageUp"))
                {
                    equippedWeapon.baseDamage = equippedWeapon.baseDamage + 1;

                }

                if (i.attributes.ContainsKey("bulletUp"))
                {
                    equippedWeapon.bulletCount = equippedWeapon.bulletCount + 1;
                    equippedWeapon.clip.Clear();
                    equippedWeapon.GenerateWeapon();
                }

                if (i.attributes.ContainsKey("healthUp"))
                {
                    Global.player.health++;
                }

                i.RemoveSelf();
            }
        }
    }
}