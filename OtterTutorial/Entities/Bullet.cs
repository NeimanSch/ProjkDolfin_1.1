using Otter;

using OtterTutorial;
using OtterTutorial.Effects;
using OtterTutorial.Scenes;

using System;

namespace OtterTutorial.Entities
{
    public class Bullet : Entity
    {

        private const int BULLET_PAT_CIRCLE = 0;

        // Default bullet speed
        public float bulletSpeed = 10.0f;
        public float enemyBulletSpeed = 1.0f;
        

        // Direction the bullet is going to travel in
        public int direction = 0;

        // Distance the bullet has traveled
        public float distanceTraveled = 0f;

        // Max distance a bullet can travel
        public float maxDistance = 350f;
        public int WIDTH = 16;
        public int HEIGHT = 14;

        //This is how far apart bullets are if multiple ones shoot out at the same time
        public float projectileSpread;

        // This is how the bullet will travel once it leaves the gun - i.e. a circular pattern, straight, zig zag, etc
        public int bulletPattern = 0;

        //This is for circular bullet patterns
        public double circRadius = 30;
        public double circCenterX = 0.0;
        public double circCenterY = 0.0;
        public double circRotationSpeed = 0.2;
        public double circAngle = 0.0;

        // The image object that is our bullet's graphic
        public virtual Image image { get; set; }

        public Sound shootSnd = new Sound(Assets.SND_BULLET_SHOOT);

        public float xDiff = 0;
        public float yDiff = 0;
        public float b;
        public double pDist = 0;
        public string shooter;
        public Vector2 shootPt;
        public double slope;

        public BulletData data;

        public Bullet(float x, float y)
        {
            // Set the Bullet's X,Y coordinates, and its direction
            X = x + 20;
            Y = y;

            circCenterX = X;
            circCenterY = Y;

            direction = Global.DIR_RIGHT;
            shooter = "";

            // Set the graphic to our bullet image
            image = new Image(Assets.BULLET);
            Graphic = image;

            data = new BulletData(0);

            // This line goes in our constructor
            shootSnd.Play();

            // Add a BulletTrail particle as soon as the Bullet enters the Scene
            Global.TUTORIAL.Scene.Add(new BulletTrail(X, Y));

            // Add this line to the Bullet.cs class
            // Set the Bullet hitbox to 16x14
            SetHitbox(16, 14, (int)Global.Type.BULLET);
            shootPt = new Vector2(Global.player.X, Global.player.Y);
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
            if (shooter == "enemy")
            {
                //theta = Math.Atan((double)((Global.player.Y - Y) / (Global.player.X - X)));
                slope = (Global.player.Y - Y) / (Global.player.X - X);
                b = Y - (X * ((Global.player.Y - Y) / (Global.player.X - X)));
                xDiff = Global.player.X - X;
                image = new Image(Assets.ENEMY_BULLET);
                Graphic = image;
            }
        }

        public Bullet(float x, float y, int dir, string s)
            : this(x, y, dir)
        {
            shooter = s;
        }

        public Bullet(float x, float y, int dir, string s, int bulletHeight, int bulletWidth, float range, float speed)
            : this(x, y, dir, s)
        {
            HEIGHT = bulletHeight;
            WIDTH = bulletWidth;
            maxDistance = range;
            bulletSpeed = speed;

            if (bulletPattern == BULLET_PAT_CIRCLE)
            {
                InitCircPattern();
            }
        }

        public Bullet(BulletData bulletDat, float x, float y, int dir, string shooter) : this (x, y, dir, shooter)
        {
            data = bulletDat;
         
            this.direction = dir;
            if (direction == Global.DIR_RIGHT)
            {
                this.X = x + Global.player.Hitbox.Width;
            }
            else if (direction == Global.DIR_LEFT)
            {
                this.X = x - Global.player.Hitbox.Width;
            }
            else
            {
                this.X = x;
            }
            if (direction == Global.DIR_UP)
            {
                this.Y = y - Global.player.Hitbox.Width;
            }
            else if (direction == Global.DIR_DOWN)
            {
                this.Y = y + Global.player.Hitbox.Width;
            }
            else
            {
                this.Y = y;
            }

            if(bulletDat.patternType == BulletData.BULLET_PAT_CIRCLE)
            {
                InitCircPattern();
            }
            
        }

        /**
         * This method handles the offset of the bullet when shooting in a circular pattern.
         * This prevents the bullet from spawning out from the side or behind the player since
         * initially the player is the center point for the bullet.
         * */
        private void InitCircPattern()
        {
            circCenterX = X;
            circCenterY = Y;
            if (this.direction == Global.DIR_UP)
            {
                circCenterY = Y - circRadius;
                circAngle = 270;
            }
            else if (this.direction == Global.DIR_DOWN)
            {
                circCenterY = Y + circRadius;
                circAngle = 90;
            }
            else if (this.direction == Global.DIR_RIGHT)
            {
                circCenterX = X + circRadius;
                circAngle = 180;
            }
            else if (this.direction == Global.DIR_LEFT)
            {
                circCenterX = X - circRadius;
                circAngle = 0;
            }
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

        public void EnemyBulletMovement(ref GameScene scene, Vector2 vec)
        {
            if (xDiff > 0)
            {
                X += enemyBulletSpeed;
            }
            else
            {
                X -= enemyBulletSpeed;
            }
            Y = (float)slope * X + b;
            if (scene.grid.GetRect(X, Y, X + WIDTH, Y + HEIGHT, false))
            {
                Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y, 1));
                RemoveSelf();
            }
            if (X == Global.player.X && Y == Global.player.Y)
            {
                Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y, 1));
                RemoveSelf();
            }
            var collp = Collider.Collide(X, Y, (int)Global.Type.PLAYER);
            if (collp != null)
            {
                Player p = (Player)collp.Entity;
                //p.TakeDamage(1);
                this.RemoveSelf();
                Global.TUTORIAL.Scene.Add(new BulletExplosion(X, Y, 1));
            }
        }

        public void StraightBulletMovement(ref GameScene scene)
        {
            float newX;
            float newY;
            switch (direction)
            {
                case Global.DIR_UP:
                    {
                        //Y -= bulletSpeed;
                        double y1 = ((data.speed) * Math.Sin( ((data.trajectoryAngle + 90) * Math.PI)/180));
                        Y = (float)(Y - y1);
                        double x1 = ((data.speed) * Math.Cos( ((data.trajectoryAngle + 90) * Math.PI) / 180));
                        X = (float)(this.X + x1);

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
                        //Y += bulletSpeed;
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle + 90) * Math.PI) / 180));
                        Y = (float)(Y + y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle + 90) * Math.PI) / 180));
                        X = (float)(this.X + x1);
                        
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
                        //X -= bulletSpeed;
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle + 180) * Math.PI) / 180));
                        Y = (float)(Y + y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle + 180) * Math.PI) / 180));
                        X = (float)(this.X + x1);

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
                        //X += bulletSpeed;
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle) * Math.PI) / 180));
                        Y = (float)(Y + y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle) * Math.PI) / 180));
                        X = (float)(this.X + x1);

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

        /**
         * This function handles updating the bullet position for circular
         * bullet patterns.
         * x1 = x + radius * Math.Cos(angle * (Math.PI / 180));
         * y1 = y + radius * Math.Sin(angle * (Math.PI / 180));
         **/
        private void CircularMovement(ref GameScene scene)
        {
            float newX;
            float newY;

            switch (direction)
            {
                case Global.DIR_UP:
                    {
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle + 90) * Math.PI) / 180));
                        circCenterY = (float)(circCenterY - y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle + 90) * Math.PI) / 180));
                        circCenterX = (float)(this.circCenterX + x1);

                        Y = (float)(circCenterY + circRadius * Math.Cos(circAngle));
                        X = (float)(circCenterX + circRadius * Math.Sin(circAngle));

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
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle + 90) * Math.PI) / 180));
                        circCenterY = (float)(circCenterY + y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle + 90) * Math.PI) / 180));
                        circCenterX = (float)(this.circCenterX + x1);

                        
                        Y = (float)(circCenterY + circRadius * Math.Cos(circAngle));
                        X = (float)(circCenterX + circRadius * Math.Sin(circAngle));

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
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle + 180) * Math.PI) / 180));
                        circCenterY = (float)(circCenterY + y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle + 180) * Math.PI) / 180));
                        circCenterX = (float)(this.circCenterX + x1);

                        Y = (float)(circCenterY + circRadius * Math.Cos(circAngle));
                        X = (float)(circCenterX + circRadius * Math.Sin(circAngle));

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
                        double y1 = ((data.speed) * Math.Sin(((data.trajectoryAngle) * Math.PI) / 180));
                        circCenterY = (float)(circCenterY + y1);
                        double x1 = ((data.speed) * Math.Cos(((data.trajectoryAngle) * Math.PI) / 180));
                        circCenterX = (float)(this.circCenterX + x1);

                        Y = (float)(circCenterY + circRadius * Math.Cos(circAngle));
                        X = (float)(circCenterX + circRadius * Math.Sin(circAngle));

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

            //Reset the angle after it's done a complete 360
            if (circAngle + circRotationSpeed >= 360)
            {
                circAngle = 0.0;
            }
            else
            {
                circAngle += circRotationSpeed;
            }
        }


        public override void Update()
        {
            base.Update();

            GameScene checkScene = (GameScene)Scene;

            if (shooter == "enemy")
            {
                if (true)
                {
                    EnemyBulletMovement(ref checkScene, shootPt);
                }
                else
                {
                    EnemyBulletMovement(ref checkScene, new Vector2(Global.player.X, Global.player.Y));
                }
            }
            else
            {

                if (data.patternType == BulletData.BULLET_PAT_CIRCLE)
                {
                    CircularMovement(ref checkScene);
                }
                else if (data.patternType == BulletData.BULLET_PAT_STRAIGHT)
                {
                    StraightBulletMovement(ref checkScene);
                }
                else
                {
                    //Move this to the constructor and rather than calling a particular movement function,
                    //change the bullet pattern type

                    //int shotPicker = Global.rand.Next(0, 50);

                    //if(shotPicker <= 25)
                    //{
                    //    CircularMovement(ref checkScene);
                    //}
                    //else
                    //{
                    //    StraightBulletMovement(ref checkScene);
                    //}
                }


                //PlayerBulletMovement(ref checkScene);
            }

            if (distanceTraveled % 60 == 0)
            {
                // Global.TUTORIAL.Scene.Add(new BulletTrail(X, Y));
            }

            // If we have traveled the max distance or more, then
            // the bullet will remove itself from the current Scene
            distanceTraveled += bulletSpeed;

            if (distanceTraveled >= data.range && shooter == "player")
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