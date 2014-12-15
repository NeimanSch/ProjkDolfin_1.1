using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtterTutorial.Entities
{
    public class Weapon
    {
        public int baseDamage;
        public int fireDamage;
        public int iceDamage;
        public int fireRate;
        public int fireCooldown;
        public int bulletCount;
        public int tier;
        public int level;
        public float bulletSpeed;
        public float bulletRange;
        public Bullet bullet;
        public List<BulletData> clip;

        public Weapon()
        {
            baseDamage = 0;
            fireDamage = 0;
            iceDamage = 0;
            fireRate = 100;
            clip = new List<BulletData>();

            //GenerateWeapon();
        }


        public void GenerateWeapon()
        {
            Random r = new Random();
            int evenAngle = 0;
            int evenBulletCount = 0;
            int oddAngle = 0;
            int oddBulletCount = 0;
            int angle = 0;
            fireRate = Global.player.equippedWeapon.fireRate; // r.Next(5, 50);
            baseDamage = Global.player.equippedWeapon.baseDamage;//r.Next(5, 50);
            bulletCount = Global.player.equippedWeapon.bulletCount; //r.Next(1, 10);
            //Console.WriteLine(bulletCount);
            Console.WriteLine(bulletCount);
            //All the below code is for figuring out how to angle the projectiles.
            if (bulletCount % 2 == 0)
            {
                //The player's "field of fire" is 180 degrees 
                if(bulletCount == 2)
                {
                    evenAngle = 90 / 2;
                    oddAngle = 0; //doesn't matter since it won't exists
                }
                else
                {
                    evenAngle = 90 / (bulletCount / 2);
                    oddAngle = (90 / ((bulletCount - 1) / 2)) * -1;
                }

            }
            else
            {
                if(bulletCount == 1)
                {
                    evenAngle = 0;
                    oddAngle = 0;
                }
                else if(bulletCount == 3)
                {
                    evenAngle = 90 / 2;
                    oddAngle = 90 / 2 * -1;
                }
                else
                {
                    evenAngle = 90 / (bulletCount - 1);
                    oddAngle = evenAngle * -1;
                }
            }

            for(int bullet = 1; bullet <= bulletCount; bullet++)
            {
                if(bullet == 1)
                {
                    angle = 0;
                    
                }
                else if(bullet % 2 == 0)
                {
                    evenBulletCount++;
                    angle = evenAngle * evenBulletCount;
                }
                else
                {
                    oddBulletCount++;
                    angle = oddAngle * oddBulletCount;
                }

                clip.Add(new BulletData(angle));
                
            }
        }

        //Shoot that shit!
        public void fire()
        {
            //Console.WriteLine(bulletCount);

            if (fireCooldown <= 0)
            {
                int direction = -1;

                if (Global.PlayerSession.Controller.R1.Pressed)
                {
                   direction =  Global.player.direction;
                }
                else if (Global.PlayerSession.Controller.B.Down)
                {
                    direction = Global.DIR_RIGHT;
                    Global.player.sprite.Play("walkRight");
                }
                else if (Global.PlayerSession.Controller.X.Down)
                {
                    direction = Global.DIR_LEFT;
                }
                else if (Global.PlayerSession.Controller.Y.Down)
                {
                    direction = Global.DIR_UP;
                }
                else if (Global.PlayerSession.Controller.A.Down)
                {
                    direction = Global.DIR_DOWN;
                }

                if (direction != -1)
                {
                    foreach(BulletData bullet in clip)
                    {
                        Global.TUTORIAL.Scene.Add(new Bullet(bullet, Global.player.X, Global.player.Y, direction, "player"));
                    }
                    
                    fireCooldown = fireRate;
                }
            }

            if (fireCooldown <= 0)
            {
                fireCooldown = 0;
            }
            else
            {
                fireCooldown = fireCooldown - 1;
            }
        }

        public void WriteWeaponStats()
        {
        
        }
    }
}
