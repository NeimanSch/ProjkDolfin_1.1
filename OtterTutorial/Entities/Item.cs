using Otter;

using OtterTutorial;
using OtterTutorial.Effects;
using OtterTutorial.Scenes;

using System;
using System.Collections;


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
            setAttributes();
            
            sprite = new Spritemap<string>(Assets.ITEM2, 42, 35);
            sprite.Add("float", new int[] { 0, 1, 2, 3, 4, 3, 2, 1 }, new float[] { 10f, 10f, 10f, 10f, 10f, 10f, 10f, 10f });
            
            //sprite = new Spritemap<string>(Assets.ITEM, 32, 35);
            //sprite.Add("float", new int[] { 0, 1, 2, 3, 4, 5 }, new float[] { 10f, 10f, 10f, 10f, 10f, 10f });

            // Tell the spritemap which animation to play when the scene starts
            sprite.Play("float");

            // Lastly, we must set our Entity's graphic, otherwise it will not display
            Graphic = sprite;
            //SetHitbox(32, 35, (int)Global.Type.ITEM);
            SetHitbox(42, 35, (int)Global.Type.ITEM);
        }

        public void setAttributes()
        {
            attributes.Add("movementSpeed", 2);
        }


    }
}
