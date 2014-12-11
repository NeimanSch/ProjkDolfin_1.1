using Otter;
using OtterTutorial;
using OtterTutorial.Entities;
using OtterTutorial.Util;
using MapGen;
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

namespace OtterTutorial.Scenes
{
    public class GameScene : Scene
    {
        public Music gameMusic = new Music(Assets.MUSIC_GAME);

        // Our Tilemap's calculated width and height
        public const int WIDTH = Global.GAME_WIDTH * 8;
        public const int HEIGHT = Global.GAME_HEIGHT * 8;
        public const float HALF_SCRENE_X = Global.GAME_WIDTH / 2;
        public const float HALF_SCRENE_Y = Global.GAME_HEIGHT / 2;
        public Map planetMap = null;
        public Tilemap tilemap = null;
        public GridCollider grid = null;

        public Scene nextScene;

        public Text scoreText;
        public Text healthText;
        public Text pauseText;
        public Image Menu;
        public Menu pauseMenu;
        public Texture texture = new Texture(Assets.TILESET);
        public Menu playerStats;

        // Our new constructor takes in the new J,I coordinates, and a Player object
        public GameScene(int nextJ = 0, int nextI = 0, Player player = null)
            : base()
        {
            // If a Player object isn't passed in, start at the default x,y position of 100,100
            if (player == null)
            {
                Global.player = new Player(100, 100);
            }
            else
            {
                Global.player = player;
            }
            // Create and load our Tilemap and GridCollider
            tilemap = new Tilemap(Assets.TILESET, WIDTH, HEIGHT, Global.GRID_WIDTH, Global.GRID_HEIGHT);
            grid = new GridCollider(WIDTH, HEIGHT, Global.GRID_WIDTH, Global.GRID_HEIGHT);
            

            string mapToLoad = Assets.MAP_WORLD;
            string solidsToLoad = Assets.MAP_SOLID;

            //GENERATE THAT MAP!
            planetMap = new Map();
            planetMap.GenerateMap(100, 100, 10, 5, 20, 5, 15, 0, 4, 16, 3, 3, 1);

            //Set the player's landing/spawning coordinates
            Global.player.X = planetMap.mapPlayerSpawnLocation.Item1;
            Global.player.Y = planetMap.mapPlayerSpawnLocation.Item2;

            LoadWorld(mapToLoad, solidsToLoad);
            if (false)
            {
                Global.gameMusic = new Music(Assets.MUSIC_GAME);
                Global.gameMusic.Play();
                Global.gameMusic.Volume = 0.40f;
            }

            if (Global.boss == null)
            {
                Global.boss = new Boss(900, 600);
            }
            if (Global.camShaker == null)
            {
                Global.camShaker = new CameraShaker();
            }
        }

        // We now add our Entities and Graphics once the Scene has been switched to
        public override void Begin()
        {
            Entity gridEntity = new Entity(0, 0, null, grid);
            Add(gridEntity);
            AddGraphic(tilemap);
            // Ensure that the player is not null
            if (Global.player != null)
            {
                Add(Global.player);
                Global.paused = false;
            }
            Add(Global.camShaker);
            int i = 0;
            foreach (Tuple<float, float> enemyLoc in planetMap.mapEnemySpawnLocations)
            {
                if (i > 2)
                {
                    i = 0;
                }
                if ((Global.player.X >= enemyLoc.Item1) && (enemyLoc.Item1 < Global.player.X + 32))
                {

                    Add(new Enemy(enemyLoc.Item1 + 40, enemyLoc.Item2, i));
                }
                else
                {
                    Add(new Enemy(enemyLoc.Item1, enemyLoc.Item2, i));
                }
                i += 1;
            }
            
            scoreText = new Text("Score: " + Global.player.score.ToString(), Assets.FONT_PANIC, 24);
            //scoreText.OutlineColor = new Otter.Color("7FA8D2");
            //scoreText.OutlineThickness = 3; 
            scoreText.CenterOrigin();
            scoreText.X = Global.player.X;
            scoreText.Y = Global.player.Y;
            healthText = new Text("Health: " + Global.player.health.ToString(), Assets.FONT_PANIC, 24);
            //healthText.OutlineColor = new Otter.Color("d2807f");
            //healthText.OutlineThickness = 3;
            healthText.CenterOrigin();
            healthText.X = Global.player.X;
            healthText.Y = Global.player.Y;
            playerStats = new Menu(Global.player.X, Global.player.Y, 20, Color.Grey, healthText, scoreText);
            playerStats.container.OutlineColor = Color.Gray;
            playerStats.container.OutlineThickness = 2f;
            playerStats.container.Alpha = 0.3f;
            pauseText = new Text("Game Paused", Assets.FONT_PANIC, 24);
            pauseText.OutlineColor = new Otter.Color(Color.Gold);
            pauseText.OutlineThickness = 3;
            pauseText.CenterOrigin();
            pauseText.X = Global.player.X;
            pauseText.Y = Global.player.Y;
        }

        private void LoadWorld(string map, string solids)
        {
            // Get our CSV map in string format and load it via our tilemap
            string newMap = planetMap.MapToString();
            tilemap.LoadCSV(newMap);

            // Get our csv solid map and load it into our GridCollider
            string newSolids = planetMap.CollisionMapToString();
            grid.LoadCSV(newSolids);
        }

        // Add this method to your GameScene.cs class
        private static string CSVToString(string csvMap)
        {
            string ourMap = "";
            using (var reader = new StreamReader(csvMap))
            {
                // Read each line, adding a line-break to the end of each
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    ourMap += line;
                    ourMap += "\n";
                }
            }

            return ourMap;
        }

        public override void Update()
        {
            if (Global.PlayerSession.Controller.Select.Pressed)
            {
                if (!Global.paused)
                {
                    Global.paused = true;
                }
                else
                {
                    Global.paused = false;
                }
                return;
            }
            if (Global.paused)
            {
                //DrawPauseMenu();
                //pauseMenu.Update();
                return;
            }
            else
            {
                
            }
            this.CameraX = Global.player.X - HALF_SCRENE_X;
            this.CameraY = Global.player.Y - HALF_SCRENE_Y;
            playerStats.Update();
        }

        public void DrawPauseMenu()
        {
            pauseMenu = new Menu(Global.player.X, Global.player.Y, 50, Color.Black, pauseText);
        }
    }
}