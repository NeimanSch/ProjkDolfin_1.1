using Otter;
using OtterTutorial;
using OtterTutorial.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtterTutorial.Util
{
    public class Menu
    {
        public Image container;
        public float X;
        public float Y;
        public int width;
        public int height;
        public int padding;
        public Color bgColor;
        public Text[] items;
        public float a = 1f;

        // x pos, y pos, padding, color, text elements in menu
        public Menu(float x, float y, int pad, Color c, params Text[] g)
        {
            items = g;
            bgColor = c;
            container = Image.CreateRectangle(g[0].Width + (4 * pad), 4 * g[0].Height + (4 * pad), c);
            container.X = x;
            container.Y = y;
            Scenes.GameScene.Instance.AddGraphic(container);
            Scenes.GameScene.Instance.AddGraphics(g);


        }

        public void Update()
        {
            if (items.Length == 2)
            {
                container.X = Global.player.X + (GameScene.HALF_SCRENE_X - container.Width);
                container.Y = Global.player.Y - (GameScene.HALF_SCRENE_Y) + 2f;
                items[0].X = container.X + container.HalfWidth;
                items[1].X = container.X + container.HalfWidth;
                
                items[0].Y = container.Y + 10;
                items[1].Y = items[0].Bottom + items[1].HalfHeight + 10;
                
                items[1].String = "Score: " + Global.player.score.ToString();
                items[0].String = "Health: " + Global.player.health.ToString();
            }
            else if (items.Length == 4)
            {
                container.X = Global.player.X + (GameScene.HALF_SCRENE_X - container.Width);
                container.Y = Global.player.Y - (GameScene.HALF_SCRENE_Y) + 2f;
                
                items[0].X = container.X + 50;
                items[1].X = container.X + 65;
                items[2].X = container.X + 60;
                items[3].X = container.X + 65;

                items[0].Y = container.Y + 10;
                items[1].Y = items[0].Bottom + items[1].HalfHeight + 10;
                items[2].Y = items[1].Bottom + items[2].HalfHeight + 10;
                items[3].Y = items[2].Bottom + items[3].HalfHeight + 10;

                items[1].String = "Kills: " + Global.player.score.ToString();
                items[0].String = "Health: " + Global.player.health.ToString();
                items[2].String = "Damage: " + Global.player.equippedWeapon.baseDamage.ToString();
                items[3].String = "Bullets: " + Global.player.equippedWeapon.bulletCount.ToString();

            }
            else if (items.Length == 1)
            {
                container.X = Global.player.X - container.HalfWidth;
                container.Y = Global.player.Y - (2 * container.Height);
                items[0].X = Global.player.X;
                items[0].Y = Global.player.Y - (float)(1.6 * container.Height);
            }
        }

        public void remove()
        {
            Scenes.GameScene.Instance.RemoveGraphic(container);
        }
    }
}
