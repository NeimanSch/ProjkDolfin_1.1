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
        public int bulletRange;
        public int bulletSpeed;
        public int bulletCount;
        public int bulletHeight;
        public int bulletWidth;
        public int tier;
        public int level;
        public Bullet bullet;

        public Weapon()
        {
            baseDamage = 0;
            fireDamage = 0;
            iceDamage = 0;
            fireRate = 100;

            GenerateWeapon();
        }


        public void GenerateWeapon()
        {
            Random r = new Random();

            fireRate = r.Next(5, 50);
            baseDamage = r.Next(5, 50);
            bulletRange = r.Next(100, 500);//
            bulletSpeed = 2;// r.Next(5, 8);
            bulletWidth = r.Next(10, 20); //need to look into what's up with this
            bulletHeight = r.Next(10, 20);//this doesn't seem to do much currently
            bulletCount = r.Next(1, 10);
        }

        //Shoot that shit!
        public void fire()
        {
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
                    Global.TUTORIAL.Scene.Add(new Bullet(Global.player.X, Global.player.Y, direction, "player", bulletHeight, bulletWidth, (float)bulletRange, (float)bulletSpeed));
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
