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
        public float a = 0.5f;

        // x pos, y pos, padding, color, text elements in menu
        public Menu(float x, float y, int pad, Color c, params Text[] g)
        {
            items = g;
            bgColor = c;
            container = Image.CreateRectangle(g[0].Width + (2 * pad), 2 * g[0].Height + (2 * pad), c);
            container.X = x;
            container.Y = y;
            Scenes.GameScene.Instance.AddGraphic(container);
            Scenes.GameScene.Instance.AddGraphics(g);


        }

        public void Update()
        {
            if (Global.paused)
            {
                Scenes.GameScene.Instance.RemoveGraphic(container);
                foreach (Text item in items)
                {
                    Scenes.GameScene.Instance.RemoveGraphic(item);
                }
            }
            if (items.Length == 2)
            {
                container.X = Global.player.X + (GameScene.HALF_SCRENE_X - container.Width);
                container.Y = Global.player.Y - (GameScene.HALF_SCRENE_Y) + 2f;
                items[1].X = container.X + container.HalfWidth;
                items[1].Y = container.Y + 10;
                items[0].X = container.X + container.HalfWidth;
                items[0].Y = items[1].Bottom + items[0].HalfHeight + 10;
                items[1].String = "Score: " + Global.player.score.ToString();
                items[0].String = "Health: " + Global.player.health.ToString();
            }
            else if (items.Length == 1)
            {
                container.X = Global.player.X - container.HalfWidth;
                container.Y = Global.player.Y - (2 * container.Height);
                items[0].X = Global.player.X;
                items[0].Y = Global.player.Y - (float)(1.5 * container.Height);

            }
        }
    }
}
