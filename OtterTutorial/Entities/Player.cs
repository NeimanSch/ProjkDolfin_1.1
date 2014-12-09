using Otter;

using OtterTutorial.Effects;
using OtterTutorial.Scenes;

using System;

namespace OtterTutorial.Entities
{
    public class Player : Entity
    {
        public const int WIDTH = 32;
        public const int HEIGHT = 40;
        public const float DIAGONAL_SPEED = 1.4f;
        public const int STARTING_HEALTH = 10;

        public float moveSpeed = 10f;
        public float maxSpeed = 10f;

        // Our entity's graphic will be a Spritemap
        public Spritemap<string> sprite;

        public int direction = 0;
        public bool stopMovement = false;

        public bool dead = false;

        //Weapon for testing
        public Weapon equippedWeapon;

        public int score;
        public int health;

        public Player(float x = 0, float y = 0)
        {
            // When creating a new player, the desired X,Y coordinates are passed in. If excluded, we start at 0,0
            X = x;
            Y = y;
            health = STARTING_HEALTH;
            // Create a new spritemap, with the player.png image as our source, 32 pixels wide, and 40 pixels tall
            sprite = new Spritemap<string>(Assets.PLAYER2, 32, 40);

            sprite.Add("standLeft", new int[] { 4 }, new float[] { 5f });
            sprite.Add("standRight", new int[] { 8 }, new float[] { 5f });
            sprite.Add("standDown", new int[] { 0 }, new float[] { 5f });
            sprite.Add("standUp", new int[] { 12 }, new float[] { 5f  });
            sprite.Add("walkLeft", new int[] { 4, 5, 6, 7 }, new float[] { 5f, 5f, 5f, 5f });
            sprite.Add("walkRight", new int[] { 8, 9, 10, 11 }, new float[] { 5f, 5f, 5f, 5f });
            sprite.Add("walkDown", new int[] { 0, 1, 2, 3 }, new float[] { 5f, 5f, 5f, 5f });
            sprite.Add("walkUp", new int[] { 12, 13, 14, 15 }, new float[] { 5f, 5f, 5f, 5f });

            // Tell the spritemap which animation to play when the scene starts
            

            // Lastly, we must set our Entity's graphic, otherwise it will not display
            Graphic = sprite;

            equippedWeapon = new Weapon();

            sprite.Play("standDown");

            SetHitbox(WIDTH, HEIGHT, (int)Global.Type.PLAYER);
        }

        public override void Update()
        {
            base.Update();

            if (Global.paused)
            {
                return;
            }

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
                sprite.FlippedX = false;
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
            // CHECK FOR WEAPON SHOOTAN
            equippedWeapon.fire();
            // Add particles if the player is moving in any direction
            if (verticalMovement || horizontalMovement)
            {
                if (verticalMovement && horizontalMovement)
                {
                    var colle = Collider.Collide((X + xSpeed / DIAGONAL_SPEED + 10 ), (Y + ySpeed / DIAGONAL_SPEED + 10), (int)Global.Type.ENEMY);
                    if (colle != null)
                    {
                        X = X;
                        Y = Y;
                    }
                    else
                    {
                        X += xSpeed;
                        Y += ySpeed;
                    }
                }
                else
                {
                    var colle = Collider.Collide((X + xSpeed + 10), (Y + ySpeed + 10), (int)Global.Type.ENEMY);
                    if (colle != null)
                    {
                        X = X;
                        Y = Y;
                    }
                    else
                    {
                        X += xSpeed;
                        Y += ySpeed;
                    }
                }
            }
            var collb = Collider.Collide(X, Y, (int)Global.Type.BOSS_BULLET);
            if (collb != null)
            {
                BossBullet b = (BossBullet)collb.Entity;
                b.RemoveSelf();

                dead = true;
                Global.TUTORIAL.Scene.Add(new Explosion(X, Y, true));
                Die();
            }
            
            var colli = Collider.Collide(X, Y, (int)Global.Type.ITEM);
            if (colli != null)
            {
                Item i = (Item)colli.Entity;
                if (i.attributes.ContainsKey("movementSpeed"))
                {
                    if (moveSpeed * (int)i.attributes["movementSpeed"] <= maxSpeed)
                    {
                        moveSpeed = moveSpeed * (int)i.attributes["movementSpeed"];
                    }
                    else
                    {
                        moveSpeed = maxSpeed;
                    }
                }
                i.RemoveSelf();
            }
        }

        public void TakeDamage(int d)
        {
            //health -= d;
            if (health <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            RemoveSelf();
        }
    }
}