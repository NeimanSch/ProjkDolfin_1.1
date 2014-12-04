using Otter;

using OtterTutorial;
using OtterTutorial.Effects;
using OtterTutorial.Scenes;

using System;

namespace OtterTutorial.Entities
{
    public class Bullet : Entity
    {
        // Default bullet speed
        public float bulletSpeed = 5.0f;

        // Direction the bullet is going to travel in
        public int direction = 0;

        // Distance the bullet has traveled
        public float distanceTraveled = 0f;

        // Max distance a bullet can travel
        public float maxDistance = 350f;
        public int WIDTH = 16;
        public int HEIGHT = 14;
        // The image object that is our bullet's graphic
        public virtual Image image { get; set; }

        public Sound shootSnd = new Sound(Assets.SND_BULLET_SHOOT);

        public float xDiff = 0;
        public float yDiff = 0;
        public double pDist = 0;
        public string shooter;

        public Spritemap<string> sprite;

        public Bullet(float x, float y)
        {
            // Set the Bullet's X,Y coordinates, and its direction
            X = x;
            Y = y;
            direction = Global.DIR_RIGHT;
            shooter = "";

            // Set the graphic to our bullet image
            image = new Image(Assets.BULLET);
            Graphic = image;


            // This line goes in our constructor
            shootSnd.Play();

            // Add a BulletTrail particle as soon as the Bullet enters the Scene
            Global.TUTORIAL.Scene.Add(new BulletTrail(X, Y));

            // Add this line to the Bullet.cs class
            // Set the Bullet hitbox to 16x14
            SetHitbox(16, 14, (int)Global.Type.BULLET);

        }

        public Bullet(float x, float y, int dir)
            : this(x, y)
        {
            direction = dir;
        }

        public Bullet(float x, float y, string s)
            : this(x, y)
        {
            shooter = s;
        }

        public Bullet(float x, float y, int dir, string s)
            : this(x, y, dir)
        {
            shooter = s;
        }

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
        public void EnemyBulletMovement(GameScene scene)
        {
            xDiff = Global.player.X - X;
            yDiff = Global.player.Y - Y;
            pDist = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
            float newPOS = 0;
            if (xDiff < 0)
            {
                newPOS = X - bulletSpeed;
                if (!CheckGridCollisions(scene, newPOS, true))
                {
                    X -= bulletSpeed;
                }
                else
                {
                    Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                    RemoveSelf();
                }
            }
            else
            {
                newPOS = X + bulletSpeed;
                if (!CheckGridCollisions(scene, newPOS, true))
                {
                    X += bulletSpeed;
                }
                else
                {
                    Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                    RemoveSelf();
                }
            }
            if (yDiff < 0)
            {
                newPOS = Y - bulletSpeed;
                if (!CheckGridCollisions(scene, newPOS, false))
                {
                    Y -= bulletSpeed;
                }
                else
                {
                    Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                    RemoveSelf();
                }
            }
            else
            {
                newPOS = Y + bulletSpeed;
                if (!CheckGridCollisions(scene, newPOS, false))
                {
                    Y += bulletSpeed;
                }
                else
                {
                    Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                    RemoveSelf();
                }
            }
            if (X == Global.player.X && Y == Global.player.Y)
            {
                Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                RemoveSelf();
            }
        }
        public void PlayerBulletMovement(GameScene scene)
        {
            float newX;
            float newY;
            switch (direction)
            {
                case Global.DIR_UP:
                    {
                        Y -= bulletSpeed;
                        //jb - udpated to make the bullets collide with solid map objectss
                        newY = Y + bulletSpeed;
                        if (scene.grid.GetRect(X, newY, X + WIDTH, newY + HEIGHT, false))
                        {
                            Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                            RemoveSelf();
                        }
                        break;
                    }
                case Global.DIR_DOWN:
                    {
                        Y += bulletSpeed;
                        //jb - udpated to make the bullets collide with solid map objectss
                        newY = Y - bulletSpeed;
                        if (scene.grid.GetRect(X, newY, X + WIDTH, newY + HEIGHT, false))
                        {
                            Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                            RemoveSelf();
                        }
                        break;
                    }
                case Global.DIR_LEFT:
                    {
                        X -= bulletSpeed;

                        //jb - udpated to make the bullets collide with solid map objectss
                        newX = X + bulletSpeed;
                        if (scene.grid.GetRect(newX, Y, newX + WIDTH, Y + HEIGHT, false))
                        {
                            Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                            RemoveSelf();
                        }
                        break;
                    }
                case Global.DIR_RIGHT:
                    {
                        X += bulletSpeed;
                        //jb - udpated to make the bullets collide with solid map objectss
                        newX = X - bulletSpeed;
                        if (scene.grid.GetRect(newX, Y, newX + WIDTH, Y + HEIGHT, false))
                        {
                            Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                            RemoveSelf();
                        }

                        break;
                    }
            }
        }

        public override void Update()
        {
            base.Update();

            GameScene checkScene = (GameScene)Scene;

            if (shooter == "enemy")
            {
                EnemyBulletMovement(checkScene);
            }
            else
            {
                PlayerBulletMovement(checkScene);
            }
            if (distanceTraveled % 60 == 0)
            {
                Global.TUTORIAL.Scene.Add(new BulletTrail(X, Y));
            }

            // If we have traveled the max distance or more, then
            // the bullet will remove itself from the current Scene
            distanceTraveled += bulletSpeed;
            if (distanceTraveled >= maxDistance)
            {
                //Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y));
                RemoveSelf();
            }

            // Add a new BulletTrail particle every 60 pixels traveled
            if (distanceTraveled % 60 == 0)
            {
                //Global.TUTORIAL.Scene.Add(new BulletTrail(X, Y));
            }
        }

        // Add this to your Bullet class, below your Update method
        public void Destroy()
        {
            RemoveSelf();
        }
    }
}