using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtterTutorial.Entities
{
    public class BulletData
    {
        public const int BULLET_PAT_CIRCLE = 0;
        public const int BULLET_PAT_STRAIGHT = 1;
        public const int BULLET_PAT_ZIGZAG = 2;
        public const int BULLET_PAT_RANDOM = 3;
        public const int BULLET_PAT_HOMING = 4;

        public double circRadius = 30;
        public double circCenterX = 0.0;
        public double circCenterY = 0.0;
        public double circRotationSpeed = 0.2;
        public double circAngle = 0.0;
        public double trajectoryAngle = 0;

        public float speed = 0f;
        public float range = 800f;
        public float x = 0f;
        public float y = 0f;

        public int damage = 1;
        public int width = 0;
        public int height = 0;
        public int patternType = 0;
        


        public BulletData(double angle)
        {
            trajectoryAngle = angle;
            GenerateBullet();
            //Damage needs to be passed in from the player

        }

        public void GenerateBullet()
        {
            //Random r = new Random();
            range = Global.rand.Next(800, 1000);//
            speed = Global.rand.Next(2, 10);
            width = Global.rand.Next(10, 20); //need to look into what's up with this
            height = Global.rand.Next(10, 20);//this doesn't seem to do much currently
            patternType =  Global.rand.Next(0, 50);
            circRadius = Global.rand.Next(10, 60);
            circRotationSpeed = Global.rand.NextDouble(); //this probbaly needs to be more on the slow side and not generate a num between 0.0 and 1.0 - 1.0 is pretty fast
                
            //For some reason generating a number between 0 and 1 wouldn't work...
            if(patternType <= 25)
            {
                patternType = BULLET_PAT_CIRCLE;
            }
            else if (patternType > 25)
            {
                patternType = BULLET_PAT_STRAIGHT;
            }
           
                
          
        }
    }
}
