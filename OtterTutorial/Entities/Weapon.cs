using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Otter;
using OtterTutorial.Effects;
using OtterTutorial.Scenes;


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
        public Sound shootSnd = new Sound(Assets.SND_BULLET_SHOOT);

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
        public Boolean fire()
        {
            //Console.WriteLine(bulletCount);



            if (fireCooldown <= 0)
            {
                int direction = -1;

                if (Global.PlayerSession.Controller.Button("r1").Pressed)
                {
                   direction =  Global.player.direction;
                }
                else if (Global.PlayerSession.Controller.Button("b").Down)
                {
                    direction = Global.DIR_RIGHT;
                    Global.player.sprite.Play("walkRight");
                    Global.player.sprite.FlippedX = false;
                }
                else if (Global.PlayerSession.Controller.Button("x").Down)
                {
                    direction = Global.DIR_LEFT;
                    Global.player.sprite.Play("walkLeft");
                    Global.player.sprite.FlippedX = true;
                }
                else if (Global.PlayerSession.Controller.Button("y").Down)
                {
                    direction = Global.DIR_UP;
                    Global.player.sprite.Play("walkUp");
                }
                else if (Global.PlayerSession.Controller.Button("a").Down)
                {
                    direction = Global.DIR_DOWN;
                    Global.player.sprite.Play("walkDown");
                }

                if (direction != -1)
                {
                    foreach(BulletData bullet in clip)
                    {
                        Global.TUTORIAL.Scene.Add(new Bullet(bullet, Global.player.X, Global.player.Y, direction, "player"));
                    }
                    // This line goes in our constructor
                    shootSnd.Volume = .2f;
                    shootSnd.Play();

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

            //Set sprite direction while "shooting"
            if(Global.PlayerSession.Controller.Button("a").Down || Global.PlayerSession.Controller.Button("b").Down || Global.PlayerSession.Controller.Button("y").Down || Global.PlayerSession.Controller.Button("x").Down){
                return true;
            }else{
                return false;
            }

        }

        public void WriteWeaponStats()
        {
        
        }
    }
}
