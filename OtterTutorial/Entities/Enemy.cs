using Otter;
using OtterTutorial;
using OtterTutorial.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtterTutorial.Entities
{
    public class Enemy : Entity
    {
        public const int WIDTH = 32;
        public const int HEIGHT = 40;
        public float moveSpeed = 4.0f;
        public int enemyFireCounter;

        // Default speed an enemy will move in
        public const float DEFAULT_SPEED = 3.4f;

        // Default health points our enemy starts with
        public const int DEFAULT_HEALTH = 4;

        public int health = 1;
        public float speed = 0.8f;

        public Spritemap<string> sprite;
        public Spritemap<string> blood;

        public const float MOVE_DISTANCE = 300;

        // left = true, right = false
        public bool direction = true;
        public bool flipDir = false;
        public bool shoot = false;
        // Used to keep track of the enemy distance moved
        public float distMoved = 0f;

        public Sound hurt = new Sound(Assets.SND_ENEMY_HURT);

        public float xDiff = 0;
        public float yDiff = 0;
        public double pDist = 0;
        public int type;
        public string state;
        public float distance = 0;
        public double circRotationSpeed = 0.2;
        public float bulletSpeed = 1.0f;
        public double circAngle = 0.0;

        public Enemy(float x, float y)
            : base(x, y)
        {
            health = DEFAULT_HEALTH;
            speed = DEFAULT_SPEED;
            type = 1;
            state = "idle";

            //blood = new Spritemap<string>(Assets.BLOOD, 32, 32);
            //blood.Alpha = .75f;
            // Set up the Spritemap in the same manner we did for the player
            sprite = new Spritemap<string>(Assets.ENEMY_SPRITE, 32, 40);
            sprite.Add("standLeft", new int[] { 0, 1, 2 }, new float[] { 10f, 10f, 10f });
            sprite.Add("standRight", new int[] { 0, 1, 2 }, new float[] { 10f, 10f, 10f });
            sprite.Add("standDown", new int[] { 3, 4, 5 }, new float[] { 10f, 10f, 10f });
            sprite.Add("standUp", new int[] { 6, 7, 8 }, new float[] { 10f, 10f, 10f });
            sprite.Add("walkLeft", new int[] { 0, 1, 2 }, new float[] { 10f, 10f, 10f });
            sprite.Add("walkRight", new int[] { 0, 1, 2 }, new float[] { 10f, 10f, 10f });
            sprite.Add("walkDown", new int[] { 3, 4, 5 }, new float[] { 10f, 10f, 10f });
            sprite.Add("walkUp", new int[] { 6, 7, 8 }, new float[] { 10f, 10f, 10f });
            sprite.Add("dead", new int[] { 9, 10, 11 }, new float[] { 15f, 15f, 15f });

            sprite.Play("standLeft");

            Graphic = sprite;

            // Set our Enemy hitbox to be 32 x 40. This goes in our Enemy class
            SetHitbox(32, 40, (int)Global.Type.ENEMY);

            enemyFireCounter = Global.rand.Next(10, 70);

            Random startDeg = new Random();
            circAngle = startDeg.Next(0, 360);
        }

        public Enemy(float x, float y, int t)
            : this(x, y)
        {
            type = t;
        }

        public Enemy(float x, float y, int t, string s)
            : this(x, y, t)
        {
            state = s;
        }

        public override void Update()
        {
            if (Global.paused)
            {
                return;
            }

            if (type == 99)
            {
                if (sprite.CurrentFrame == 11)
                {
                    sprite.Stop();
                }
                
                this.RemoveColliders();
                return;
            }
            else
            {

                base.Update();

            }


            // Access the Enemy's Collider to check collision
            var collb = Collider.Collide(X, Y, (int)Global.Type.BULLET);
            //jb - 99 = dead
            if (collb != null )
            {
                Bullet b = (Bullet)collb.Entity;
                if (b.shooter == "player")
                {
                    b.Destroy();

                    DamageText dt = new DamageText(X, Y, Convert.ToString(Global.player.equippedWeapon.baseDamage));
                    Global.TUTORIAL.Scene.Add(dt);

                    hurt.Play();

                    //health--; // Decrement the health by 1 for each Bullet that hits

                    health -= Global.player.equippedWeapon.baseDamage;

                    if (health <= 0 && this.type != 99)
                    {
                        // Add a new Explosion and remove self from the Scene if out of health
                        Global.TUTORIAL.Scene.Add(new Explosion(X, Y));
                        //moving the item so it's not on top of the enemy sprite.
                        Global.TUTORIAL.Scene.Add(new Item(X-16, Y-16));
                        Global.player.score++;

                        
                        //RemoveSelf();
                        sprite.Play("dead");
                        this.type = 99;
                        //Global.TUTORIAL.Scene.Add(new Entity(X, Y-20, blood, null));
                    }
                }
            }
            // If going left, flip the spritesheet
            sprite.FlippedX = direction;
            Move();
            
           
            if ((enemyFireCounter % 100 == 0) && shoot)
            {
                Global.TUTORIAL.Scene.Add(new Bullet(X, Y, "enemy"));
            }
            enemyFireCounter += 1;
        }

        public void Move()
        {
            xDiff = Global.player.X - X;
            yDiff = Global.player.Y - Y;
            pDist = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
            float newPOS = 0;    
            GameScene checkScene = (GameScene)Scene;
            switch (type)
            {
                case 0:
                    if (pDist > 200)
                    {
                        follow(newPOS, checkScene);
                        shoot = false;
                    }
                    else
                    {
                        shoot = true;
                    }
                    break;
                case 1:
                    distance++;
                        if (distance >= 100)
                        {
                            if (flipDir == false)
                            {
                                flipDir = true;
                            }
                            else
                            {
                                flipDir = false;
                            }
                            distance = 0;
                        }
                    switch (state)
                    {
                        
                        case "idle":
                            sineWave(newPOS, checkScene);
                            if (pDist <= 300)
                            {
                                state = "aware";
                                shoot = true;
                            }
                            break;
                        case "aware":
                            follow(newPOS, checkScene);
                            if (pDist >= 300)
                            {
                                state = "idle";
                                shoot = false;
                            }
                            break;
                    }
                    break;
                case 2:
                    //shoot = true;
                    distance++;
                    sineWave(newPOS, checkScene);
                    break;

                case 99:
                     //dead
                    shoot = false;
                    break;
            }
        }

        public void follow(float n, GameScene s)
        {
            if (pDist > 20)
            {
                if (xDiff < 20)
                {
                    n = X - speed;
                    if (!CheckGridCollisions(s, n, true))
                    {
                        var colle = Collider.Collide(n, (Y), (int)Global.Type.ENEMY, (int)Global.Type.PLAYER);
                         if (colle != null)
                         {
                             X = X;
                         }
                         else
                         {
                             X -= speed;
                         }
                    }
                }
                else if (xDiff > 20)
                {
                    n = X + speed;
                    if (!CheckGridCollisions(s, n, true))
                    {
                        var colle = Collider.Collide(n, (Y), (int)Global.Type.ENEMY, (int)Global.Type.PLAYER);
                        if (colle != null)
                        {
                            X = X;
                        }
                        else
                        {
                            X += speed;
                        }
                    }
                }
                if (yDiff < 20)
                {
                    n = Y - speed;
                    if (!CheckGridCollisions(s, n, false))
                    {
                        var colle = Collider.Collide(X, n, (int)Global.Type.ENEMY, (int)Global.Type.PLAYER);
                        if (colle != null)
                        {
                            Y = Y;
                        }
                        else
                        {
                            Y -= speed;
                        }
                    }
                }
                else if (yDiff > 20)
                {
                    n = Y + speed;
                    if (!CheckGridCollisions(s, n, false))
                    {
                        var colle = Collider.Collide(X, n, (int)Global.Type.ENEMY, (int)Global.Type.PLAYER);
                        if (colle != null)
                        {
                            Y = Y;
                        }
                        else
                        {
                            Y += speed;
                        }
                    }
                }
            }
        }
        public void sineWave(float n, GameScene s)
        {
            if (distance >= 100)
            {
                if (flipDir == false)
                {
                    flipDir = true;
                }
                else
                {
                    flipDir = false;
                }
                distance = 0;
            }
            n = (float)(Y + 10 * Math.Cos(circAngle));
            if (!CheckGridCollisions(s, n, false))
            {
                var colle = Collider.Collide(X, n, (int)Global.Type.ENEMY, (int)Global.Type.PLAYER);
                if (colle != null)
                {
                    Y = Y;
                }
                else
                {
                    Y = (float)(Y + 10 * Math.Cos(circAngle));
                }
            }
            if (circAngle + circRotationSpeed >= 360)
            {
                circAngle = 0.0;
            }
            else
            {
                circAngle += circRotationSpeed;
            }
            if (flipDir)
            {
                n = X - speed;
                if (!CheckGridCollisions(s, n, true))
                {
                    X -= speed;
                }
            }
            else
            {
                n = X + speed;
                if (!CheckGridCollisions(s, n, true))
                {
                    X += speed;
                }
            }
        }

        // checks if there is a collision with the solids grid
        //  takes a GameSceen (scene), speed/distance to move (p),
        //  true or false for p direction (xAxis)
        public Boolean CheckGridCollisions(GameScene scene, float p, Boolean xAxis)
        {
            Boolean collision = false;
            if (xAxis)
            {
                if (scene.grid.GetRect(p, Y, p + WIDTH, Y + HEIGHT, false))
                {
                    collision = true;
                }
            }
            else
            {
                if (scene.grid.GetRect(X, p, X + WIDTH, p + HEIGHT, false))
                {
                    collision = true;
                }
            }
            return collision;
        }
    }
}