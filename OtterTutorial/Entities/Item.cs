using Otter;

using OtterTutorial;
using OtterTutorial.Effects;
using OtterTutorial.Scenes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

namespace OtterTutorial.Entities
{
    public class Item : Entity
    {
        public Hashtable attributes = new Hashtable (); // string in hash format { attribute1: value1, attribute2: value2, ...}
        public Spritemap<string> sprite;

        public Item(float x, float y)
        {
            
            X = x;
            Y = y;
            
            //calculate which item should drop
            //string sprite1 = getItem();

            //Console.WriteLine(sprite1);

            
            //setAttributes();
            string itemSprite = getItem();

            if (itemSprite != "")
            {
                sprite = new Spritemap<string>(itemSprite, 32, 32);
                sprite.Add("float", new int[] { 0,1,2,3 }, new float[] { 20f });
            
                //sprite = new Spritemap<string>(Assets.ITEM, 32, 35);
                //sprite.Add("float", new int[] { 0, 1, 2, 3, 4, 5 }, new float[] { 10f, 10f, 10f, 10f, 10f, 10f });

                // Tell the spritemap which animation to play when the scene starts
                sprite.Play("float");
            
                Graphics.Add(sprite);

            
                SetHitbox(32, 35, (int)Global.Type.ITEM);
            }
        }

        public void setAttributes()
        {
            
            //attributes.Add("movementSpeed", 2);
        }

        public virtual string getItem()
        {
            string path = Assets.ITEMS;
            string spriteValue = "";
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Console.WriteLine(path);

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("item");


            int previous_drop_rate = 0;
            string attribute_name = "";
            int attribute_value = 0;
            bool itemFound = false;
            string pickup_name = "";

            Random random = new Random();
            int randomNumber = random.Next(0, 100);

            Console.WriteLine(randomNumber);

            foreach (XmlNode node in nodes)
            {
                ItemPickUp pickup = new ItemPickUp();

                pickup.name = node.SelectSingleNode("name").InnerText;
                pickup.sprite = node.SelectSingleNode("sprites/sprite").InnerText;
                pickup.attribute_name = node.SelectSingleNode("attributes/name").InnerText;
                pickup.attribute_value = node.SelectSingleNode("attributes/value").InnerText;
                pickup.drop_rate_min = node.SelectSingleNode("droprate_min").InnerText;
                pickup.drop_rate_max = node.SelectSingleNode("droprate_max").InnerText;

                if (randomNumber > Convert.ToInt32(pickup.drop_rate_min) && randomNumber < Convert.ToInt32(pickup.drop_rate_max) && !itemFound)
                {
                    spriteValue = "../../Assets/Graphics/" + pickup.sprite;
                    attribute_name = pickup.attribute_name;
                    attribute_value = Convert.ToInt32(pickup.attribute_value);
                    //previous_drop_rate = Convert.ToInt32(pickup.drop_rate);
                    pickup_name = pickup.name;
                    itemFound = true;
                }                 


            } // for each item

            if (itemFound)
            {
                if (!attributes.ContainsKey(attribute_name))
                {
                    attributes.Add(attribute_name, attribute_value);


                    Text alertText = new Text(pickup_name, 12);
                    alertText.CenterOrigin();
                    alertText.X = this.X + 10; //OtterTutorial.Global.player.X - 10;
                    alertText.Y = this.Y + 30;// OtterTutorial.Global.player.Y; 
                    
                    OtterTutorial.Scenes.GameScene.Instance.AddGraphic(alertText);
                }
            }
            return spriteValue;

        } //getItem

        public void readXML()
        {

        }
    
        class ItemPickUp
        {
            public string name;
            public string sprite;
            public string height;
            public string width;
            public string attribute_name;
            public string attribute_value;
            public string drop_rate_min;
            public string drop_rate_max;
        }
    }
}
