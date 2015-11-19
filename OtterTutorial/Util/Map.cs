using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OtterTutorial;

namespace MapGen
{
    /* *
     * TODO: 2
     * 1. Fix issue where small maps with tunnels can cause stand alone rooms.
     * 2. Make constants for tile integers. Right now they are scattered about and annoying as hell to update. 
     * 3. Spawn points need some major work. Right now shit spawns all over the place. It really likes buildings for some reason...
     * */
    public class Map
    {
        /************************************/
        /******** PROPERTIES        *********/
        /************************************/
        private static int mapXMax;                //Map's Max Width
        private static int mapYMax;                //Map's Max Height
        private static int mapNumOfSqrRooms;       //Number of Square Rooms to Generate
        private static int mapNumOfCirRooms;       //Number of Circle Rooms to Generate
        private static int mapSqrRoomMinWidth;
        private static int mapSqrRoomMaxWidth;
        private static int mapSqrRoomMinHeight;
        private static int mapSqrRoomMaxHeight;
        private static int mapCirRoomMinRadius;
        private static int mapCirRoomMaxRadius;
        private static int mapMaxTunnelWidth;
        private static int mapMinTunnelWidth;
        private static int mapMinBuildings;
        private static int mapMaxBuildings;
        private static int mapType;

        public Tuple<float, float> mapPlayerSpawnLocation;
        public List<Tuple<float, float>> mapEnemySpawnLocations;

        private static List<Room> roomList;
        private static List<Room> buildingList;
        private static int[,] mapData;              //Numeric representation of map
        private static int[,] mapCollisionData;
        private static string[,] mapDisplayData;    //This is character display use for debugging
        private Dictionary<string, int> tileMapDict;

        /******** TILE MAP CONSTANTS ********/
        public const int TILE_MAIN_GROUND = 16;
        public const int TILE_MAIN_WALL = 15;

        //The numbers below translate as so - 0(north)0(east)0(south)0(west) basically the numbers move clockwise starting with north. 
        //Example: 0100 would mean that the wall has a connection on the right side only
        public const int TILE_WALL_0000 = 0;
        public const int TILE_WALL_1000 = 1;
        public const int TILE_WALL_0100 = 2;
        public const int TILE_WALL_1100 = 3; //Bottom Left Corner
        public const int TILE_WALL_0010 = 4;
        public const int TILE_WALL_1010 = 5;
        public const int TILE_WALL_0110 = 6;
        public const int TILE_WALL_1110 = 7;
        public const int TILE_WALL_0001 = 8;
        public const int TILE_WALL_1001 = 9;
        public const int TILE_WALL_0101 = 10;
        public const int TILE_WALL_1101 = 11;
        public const int TILE_WALL_0011 = 12;
        public const int TILE_WALL_1011 = 13;
        public const int TILE_WALL_0111 = 14;
        public const int TILE_WALL_1111 = 15;

        /************************************/
        /******** CONSTRUCTOR       *********/
        /************************************/
        public Map()
        {
            mapType = 0;
            mapXMax = 100;
            mapYMax = 100;
            mapNumOfSqrRooms = 40;
            mapNumOfCirRooms = 30;
            mapCirRoomMaxRadius = 6;
            mapCirRoomMinRadius = 4;
            mapSqrRoomMinWidth = 5;
            mapSqrRoomMaxWidth = 15;
            mapSqrRoomMinHeight = 5;
            mapSqrRoomMaxHeight = 15;
            mapMinTunnelWidth = 1;
            mapMaxTunnelWidth = 6;
            mapData = new int[mapYMax, mapXMax];
            mapCollisionData = new int[mapYMax, mapXMax];
            mapDisplayData = new string[mapYMax, mapXMax];
            roomList = new List<Room>();
            buildingList = new List<Room>();
            mapPlayerSpawnLocation = new Tuple<float, float>(0, 0);
            mapEnemySpawnLocations = new List<Tuple<float, float>>();
            InitTileMapDictionary();

        }


        /************************************/
        /******** PUBLIC FUNCTIONS  *********/
        /************************************/

        //Uses the default settings - pretty useless function at the moment
        public void GenerateMap()
        {
            InitMapData();
            BuildMap();
            RefineTileMap();
            SetPlayerSpawn();
            SetEnemySpawns();
        }

        //Provides a way to override the map settings
        public void GenerateMap(int mapWidth, int mapHeight, int numOfSqrRooms, int sqrRoomMinHeight, int sqrRoomMaxHeight, int sqrRoomMinWidth, 
                                       int sqrRoomMaxWidth, int numOfCirRooms, int cirRoomMinRadius, int cirRoomMaxRadius, int tunnelMinWidth, int tunnelMaxWidth, int type)
        {
            mapXMax = mapWidth;
            mapYMax = mapHeight;
            mapNumOfSqrRooms = numOfSqrRooms;
            mapNumOfCirRooms = numOfCirRooms;
            mapCirRoomMinRadius = cirRoomMinRadius;
            mapCirRoomMaxRadius = cirRoomMaxRadius;
            mapSqrRoomMinWidth = sqrRoomMinWidth;
            mapSqrRoomMaxWidth = sqrRoomMaxWidth;
            mapSqrRoomMinHeight = sqrRoomMinHeight;
            mapSqrRoomMaxHeight = sqrRoomMaxHeight;
            mapType = type;
            mapMinTunnelWidth = tunnelMinWidth;
            mapMaxTunnelWidth = tunnelMaxWidth;
            mapData = new int[mapYMax, mapXMax];
            mapCollisionData = new int[mapYMax, mapXMax];
            mapDisplayData = new string[mapYMax, mapXMax];

            InitMapData();
            BuildMap();
            RefineTileMap();
            SetPlayerSpawn();
            SetEnemySpawns();
        }

        //Function for debugging purposes only
        public void DumpMapDataToFile(string filePath)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(filePath);

            for (int i = 0; i < mapDisplayData.GetLength(0); i++)
            {
                for (int j = 0; j < mapDisplayData.GetLength(1); j++)
                {
                    file.Write(mapCollisionData[i, j]);
                }

                file.Write("\r\n");
            }

            file.Close();
        }


        
        public string MapToString()
        {
            string map = "";

            for (int y = 0; y < mapDisplayData.GetLength(0); y++)
            {
                for (int x = 0; x < mapDisplayData.GetLength(1); x++)
                {
                    map += mapData[y, x] + ",";
                }

                map += "\n";
            }

            return map;
        }

        public string CollisionMapToString()
        {
            string mapCollisions = "";

            for (int y = 0; y < mapCollisionData.GetLength(0); y++)
            {
                for (int x = 0; x < mapCollisionData.GetLength(1); x++)
                {
                    mapCollisions += mapCollisionData[y, x] + ",";
                }

                mapCollisions += "\n";
            }

            return mapCollisions;
        }

        /************************************/
        /******** PRIVATE FUNCTIONS *********/
        /************************************/

        /**
         * This function loops through the map data array and fills the map with
         * "walls". The map generation algorithms "carve" out the map.
         **/
        private static void InitMapData()
        {
            for (int y = 0; y < mapData.GetLength(0); y++)
            {
                for (int x = 0; x < mapData.GetLength(1); x++)
                {
                    mapData[y, x] = TILE_MAIN_WALL;
                    mapCollisionData[y, x] = 1;
                    mapDisplayData[y, x] = "#";
                }
            }
        }


        private void InitTileMapDictionary()
        {
            tileMapDict = new Dictionary<string, int>();
            tileMapDict.Add("TILE_WALL_0000", 0);
            tileMapDict.Add("TILE_WALL_1000", 1);
            tileMapDict.Add("TILE_WALL_0100", 2);
            tileMapDict.Add("TILE_WALL_1100", 3);
            tileMapDict.Add("TILE_WALL_0010", 4);
            tileMapDict.Add("TILE_WALL_1010", 5);
            tileMapDict.Add("TILE_WALL_0110", 6);
            tileMapDict.Add("TILE_WALL_1110", 7);
            tileMapDict.Add("TILE_WALL_0001", 8);
            tileMapDict.Add("TILE_WALL_1001", 9);
            tileMapDict.Add("TILE_WALL_0101", 10);
            tileMapDict.Add("TILE_WALL_1101", 11);
            tileMapDict.Add("TILE_WALL_0011", 12);
            tileMapDict.Add("TILE_WALL_1011", 13);
            tileMapDict.Add("TILE_WALL_0111", 14);
            tileMapDict.Add("TILE_WALL_1111", TILE_MAIN_WALL);
        
        }


        private static void BuildMap()
        {
            //This will be where we place the logic to  determine what rooms to build based with what type of
            //map objects.
            if(mapType == 0)
            {
                BuildCircleRooms();
                BuildSquareRooms();
            }
            else if(mapType == 1)
            {
                BuildDungeonMap();
                BuildSquareRooms();
                BuildCircleRooms();
                
                //BuildBuildings();
            }
            else
            {
                BuildDungeonMap();
                BuildCircleRooms();
                BuildBuildings();
               
            }

            PopulateMapData();
              
        }

        private static void BuildSquareRooms()
        {
            Random r = new Random();

            for (int i = 0; i < mapNumOfSqrRooms; i++)
            {
                Room newRoom = new Room();
                newRoom.displayCharacter = ".";
                newRoom.dataType = TILE_MAIN_GROUND;
                newRoom.width = r.Next(mapSqrRoomMinWidth, mapSqrRoomMaxWidth);
                newRoom.height = r.Next(mapSqrRoomMinHeight, mapSqrRoomMaxHeight);

                if (roomList.Count == 0)
                {
                    ////If this is the first room - put it in the center of the map
                    newRoom.xLoc = (mapData.GetLength(0) / 2) - (newRoom.width / 2);
                    newRoom.yLoc = (mapData.GetLength(1) / 2) - (newRoom.height / 2);
                }
                else
                {
                    //Pick a random room from the list and base the location of the next room on it
                    int roomNum = 0;

                    roomNum = r.Next(0, roomList.Count - 1);
                    newRoom.xLoc = r.Next(roomList[roomNum].xLoc - (newRoom.width - 1), roomList[roomNum].XMaxLocation() - 1);
                    newRoom.yLoc = r.Next(roomList[roomNum].yLoc - (newRoom.height - 1), roomList[roomNum].YMaxLocation() - 1);
                }

                //Check to make sure we didn't go out of bounds
                if (newRoom.xLoc < 0)
                {
                    newRoom.xLoc = 1; //Set it to the edge + 1 so we still have a barrier
                }

                if (newRoom.xLoc >= mapData.GetLength(1))
                {
                    newRoom.xLoc = mapData.GetLength(1) - 2; //Set it to the edge minus 2 to account for the edge
                }

                //Do The same for y axis
                if (newRoom.yLoc < 0)
                {
                    newRoom.yLoc = 1;
                }

                if (newRoom.yLoc >= mapData.GetLength(0))
                {
                    newRoom.yLoc = mapData.GetLength(0) - 2; //Set it to the edge minus 2 to account for the edge
                }

                roomList.Add(newRoom);

            }
        }

        private static void BuildCircleRooms()
        {
            Random r = new Random();

            for (int i = 0; i < mapNumOfCirRooms; i++)
            {
                bool bCircleComplete = false;
                int xCenter = 0; //mapData.GetLength(1) / 2;
                int yCenter = 0; //mapData.GetLength(0) / 2;
                int xStopPoint = 0;
                int roomCounter = 0;
                int tempWidth = 0;
                int tempHeight = 0;
                int tempXLoc = 0;
                int tempYLoc = 0;
                int radius = r.Next(mapCirRoomMinRadius, mapCirRoomMaxRadius);

                if(roomList.Count == 0)
                {
                    xCenter = mapData.GetLength(1)/2;
                    yCenter = mapData.GetLength(0)/2;
                }
                else
                {
                    int roomNum = r.Next(0, roomList.Count - 1);
                    xCenter = r.Next(roomList[roomNum].xLoc - radius, roomList[roomNum].XMaxLocation() - 1);
                    yCenter = r.Next(roomList[roomNum].yLoc - radius, roomList[roomNum].YMaxLocation() - 1);
                }

                xStopPoint = xCenter - radius;

                while (!bCircleComplete)
                {
                    Room newRoom = new Room();
                    newRoom.displayCharacter = ".";
                    newRoom.dataType = TILE_MAIN_GROUND;

                    if (roomCounter == 0)
                    {
                        newRoom.width = radius;
                        newRoom.height = radius * 2;
                        newRoom.xLoc = xCenter - (radius / 2);
                        newRoom.yLoc = yCenter - radius;
                    }
                    else
                    {
                        newRoom.width = tempWidth + 2;
                        newRoom.height = tempHeight - 2;
                        newRoom.xLoc = tempXLoc - 1;
                        newRoom.yLoc = tempYLoc + 1;
                    }

                    tempWidth = newRoom.width;
                    tempHeight = newRoom.height;
                    tempXLoc = newRoom.xLoc;
                    tempYLoc = newRoom.yLoc;
                    roomCounter++;
                    roomList.Add(newRoom);

                    if (tempXLoc <= xStopPoint)
                    {
                        bCircleComplete = true;
                    }
                }
            }

        }


        private static void BuildIslands()
        {
            Random r = new Random();

            for(int k = 0; k < 10; k++)
            {

                int lastX = 0;
                int lastY = 0;
                Room roomToPopulate = roomList[r.Next(0, roomList.Count - 1)];

                lastX = r.Next(roomToPopulate.xLoc, roomToPopulate.XMaxLocation());
                lastY = r.Next(roomToPopulate.yLoc, roomToPopulate.YMaxLocation());

                for(int i = 0; i < 10; i++)
                {
                    int dir = r.Next(0, 3);

                    Room newObj = new Room();

                    if(dir == 0) //West
                    {
                        newObj.xLoc = lastX - 1;
                        newObj.yLoc = lastY;
                    }
                    else if(dir == 1)
                    {
                        newObj.xLoc = lastX + 1;
                        newObj.yLoc = lastY;
                    }
                    else if(dir == 2)
                    {
                        newObj.yLoc = lastY - 1;
                        newObj.xLoc = lastX;
                    }
                    else
                    {
                        newObj.yLoc = lastY + 1;
                        newObj.xLoc = lastX;
                    }

                    lastY = newObj.yLoc;
                    lastX = newObj.xLoc;
                    newObj.height = 1;
                    newObj.width = 1;
                    newObj.displayCharacter = "#";
                    newObj.dataType = 1;

                    roomList.Add(newObj);
                }

            }
        }


        private static void BuildDungeonMap()
        {

            Random r = new Random();
            int tunnelWidth = 0;

            for (int i = 0; i < mapNumOfSqrRooms; i++)
            {
                Room newRoom = new Room();
                newRoom.displayCharacter = ".";
                newRoom.dataType = TILE_MAIN_GROUND;
                newRoom.width = r.Next(mapSqrRoomMinWidth, mapSqrRoomMaxWidth);
                newRoom.height = r.Next(mapSqrRoomMinHeight, mapSqrRoomMaxHeight);


                ////If this is the first room - put it in the center of the map
                newRoom.xLoc = r.Next(0, mapXMax - (newRoom.width + 1));
                newRoom.yLoc = r.Next(0, mapYMax - (newRoom.height + 1));

                roomList.Add(newRoom);

            }

            for (int i = 0; i < roomList.Count - 1; i++ )
            {
                if (roomList.Count <= 1)
                {
                    break;
                }

                //Pick a random wall to connect
                int dir = r.Next(0, 3);
                int room1X = 0;
                int room1Y = 0;
                int room2X = 0;
                int room2Y = 0;

                if (dir == 0) //West
                {
                    room1X = roomList[i].xLoc;
                    room1Y = r.Next(roomList[i].yLoc, roomList[i].YMaxLocation());
                }
                else if (dir == 1) //east
                {
                    room1X = roomList[i].XMaxLocation();
                    room1Y = r.Next(roomList[i].yLoc, roomList[i].YMaxLocation());
                }
                else if (dir == 2) //north
                {
                    room1X = r.Next(roomList[i].xLoc, roomList[i].XMaxLocation());
                    room1Y = roomList[i].yLoc;
                }
                else //south
                {
                    room1X = r.Next(roomList[i].xLoc, roomList[i].XMaxLocation());
                    room1Y = roomList[i].YMaxLocation();
                }


                //Get random wall from other room
                if (dir == 0) //West
                {
                    room2X = roomList[i + 1].xLoc;
                    room2Y = r.Next(roomList[i + 1].yLoc, roomList[i + 1].YMaxLocation());
                }
                else if (dir == 1) //east
                {
                    room2X = roomList[i + 1].XMaxLocation();
                    room2Y = r.Next(roomList[i + 1].yLoc, roomList[i + 1].YMaxLocation());
                }
                else if (dir == 2) //north
                {
                    room2X = r.Next(roomList[i + 1].xLoc, roomList[i + 1].XMaxLocation());
                    room2Y = roomList[i + 1].yLoc;
                }
                else //south
                {
                    room2X = r.Next(roomList[i + 1].xLoc, roomList[i + 1].XMaxLocation());
                    room2Y = roomList[i + 1].YMaxLocation();
                }

                //THIS NEEDS TO BE REFACTORED BIG TIME!!!!! There's lots of repeated code in here that can be written better...
                //Just wanted to brute force it to make sure it would work. - this could be done the same wall rooms are being done with drawing squares s
                if (room1X > room2X)
                {
                    while (room1X > room2X)
                    {
                        if (room1X >= mapData.GetLength(1) || room1Y >= mapData.GetLength(0) || room1X <= 0 || room1Y <= 0)
                        {
                            break;
                        }

                        tunnelWidth  = r.Next(mapMinTunnelWidth, mapMaxTunnelWidth);
                        
                        for (int tunnelY = 0; tunnelY < tunnelWidth; tunnelY++ )
                        {
                            int tunnelYPlace = room1Y - tunnelY;

                            if(tunnelYPlace >= mapData.GetLength(0) || tunnelYPlace <= 0)
                            {
                                break;
                            }

                            mapData[tunnelYPlace, room1X] = TILE_MAIN_GROUND;
                            mapCollisionData[tunnelYPlace, room1X] = 0; 
                            mapDisplayData[tunnelYPlace, room1X] = "."; 
                        }

                        for (int tunnelY = 0; tunnelY < tunnelWidth; tunnelY++)
                        {
                            int tunnelYPlace = room1Y + tunnelY;

                            if (tunnelYPlace >= mapData.GetLength(0) || tunnelYPlace <= 0)
                            {
                                break;
                            }

                            mapData[tunnelYPlace, room1X] = TILE_MAIN_GROUND;
                            mapCollisionData[tunnelYPlace, room1X] = 0;
                            mapDisplayData[tunnelYPlace, room1X] = ".";
                        }

                        
                        mapDisplayData[room1Y, room1X] = ".";
                        room1X--;
                    }
                }
                if (room1X < room2X)
                {
                    while (room1X < room2X)
                    {
                        if (room1X >= mapData.GetLength(1) || room1Y >= mapData.GetLength(0) || room1X <= 0 || room1Y <= 0)
                        {
                            break;
                        }

                        tunnelWidth = r.Next(mapMinTunnelWidth, mapMaxTunnelWidth);

                        for (int tunnelY = 0; tunnelY < tunnelWidth; tunnelY++)
                        {
                            int tunnelYPlace = room1Y - tunnelY;

                            if (tunnelYPlace >= mapData.GetLength(0) || tunnelYPlace <= 0)
                            {
                                break;
                            }

                            mapData[tunnelYPlace, room1X] = TILE_MAIN_GROUND;
                            mapCollisionData[tunnelYPlace, room1X] = 0;
                            mapDisplayData[tunnelYPlace, room1X] = ".";
                        }

                        for (int tunnelY = 0; tunnelY < tunnelWidth; tunnelY++)
                        {
                            int tunnelYPlace = room1Y + tunnelY;

                            if (tunnelYPlace >= mapData.GetLength(0) || tunnelYPlace <= 0)
                            {
                                break;
                            }

                            mapData[tunnelYPlace, room1X] = TILE_MAIN_GROUND;
                            mapCollisionData[tunnelYPlace, room1X] = 0;
                            mapDisplayData[tunnelYPlace, room1X] = ".";
                        }

                        mapData[room1Y, room1X] = TILE_MAIN_GROUND;
                        mapCollisionData[room1Y, room1X] = 0;
                        mapDisplayData[room1Y, room1X] = ".";
                        room1X++;
                    }
                }
                if (room1Y > room2Y)
                {
                    while (room1Y > room2Y)
                    {
                        if (room1X >= mapData.GetLength(1) || room1Y >= mapData.GetLength(0) || room1X <= 0 || room1Y <= 0)
                        {
                            break;
                        }

                        tunnelWidth = r.Next(mapMinTunnelWidth, mapMaxTunnelWidth);

                        for (int tunnelX = 0; tunnelX < tunnelWidth; tunnelX++)
                        {
                            int tunnelXPlace = room1X - tunnelX;

                            if (tunnelXPlace >= mapData.GetLength(0) || tunnelXPlace <= 0)
                            {
                                break;
                            }

                            mapData[room1Y, tunnelXPlace] = TILE_MAIN_GROUND;
                            mapCollisionData[room1Y, tunnelXPlace] = 0;
                            mapDisplayData[room1Y, tunnelXPlace] = ".";
                        }

                        for (int tunnelX = 0; tunnelX < tunnelWidth; tunnelX++)
                        {
                            int tunnelXPlace = room1X + tunnelX;

                            if (tunnelXPlace >= mapData.GetLength(0) || tunnelXPlace <= 0)
                            {
                                break;
                            }

                            mapData[room1Y, tunnelXPlace] = TILE_MAIN_GROUND;
                            mapCollisionData[room1Y, tunnelXPlace] = 0;
                            mapDisplayData[room1Y, tunnelXPlace] = ".";
                        }

                        mapData[room1Y, room1X] = TILE_MAIN_GROUND;
                        mapCollisionData[room1Y, room1X] = 0;
                        mapDisplayData[room1Y, room1X] = ".";
                        room1Y--;
                    }
                }
                if (room1Y < room2Y)
                {
                    while (room1Y < room2Y)
                    {
                        if (room1X >= mapData.GetLength(1) || room1Y >= mapData.GetLength(0) || room1X <= 0 || room1Y <= 0)
                        {
                            break;
                        }

                        tunnelWidth = r.Next(mapMinTunnelWidth, mapMaxTunnelWidth);

                        for (int tunnelX = 0; tunnelX < tunnelWidth; tunnelX++)
                        {
                            int tunnelXPlace = room1X - tunnelX;

                            if (tunnelXPlace >= mapData.GetLength(0) || tunnelXPlace <= 0)
                            {
                                break;
                            }

                            mapData[room1Y, tunnelXPlace] = TILE_MAIN_GROUND;
                            mapCollisionData[room1Y, tunnelXPlace] = 0;
                            mapDisplayData[room1Y, tunnelXPlace] = ".";
                        }

                        for (int tunnelX = 0; tunnelX < tunnelWidth; tunnelX++)
                        {
                            int tunnelXPlace = room1X + tunnelX;

                            if (tunnelXPlace >= mapData.GetLength(0) || tunnelXPlace <= 0)
                            {
                                break;
                            }

                            mapData[room1Y, tunnelXPlace] = TILE_MAIN_GROUND;
                            mapCollisionData[room1Y, tunnelXPlace] = 0;
                            mapDisplayData[room1Y, tunnelXPlace] = ".";
                        }

                        mapData[room1Y, room1X] = TILE_MAIN_GROUND;
                        mapCollisionData[room1Y, room1X] = 0;
                        mapDisplayData[room1Y, room1X] = ".";
                        room1Y++;
                    }
                }
            }
        }



        private static void BuildBuildings()
        {
            Random r = new Random();
            int numOfBuildings = 5;

            for(int i = 0; i < numOfBuildings; i++)
            {
                int roomNum = r.Next(0, roomList.Count);
                Room landscape = roomList[roomNum];
                Room newRoom = new Room();

                //A building must be at least 3x3 or else it would just be walls
                if (landscape.width < 3 || landscape.height < 3)
                {
                    break;
                }

                newRoom.width = r.Next(3, landscape.width);
                newRoom.height = r.Next(3, landscape.height);
                newRoom.xLoc = r.Next(landscape.xLoc, landscape.XMaxLocation() - (newRoom.width + 1));
                newRoom.yLoc = r.Next(landscape.yLoc, landscape.YMaxLocation() - (newRoom.height + 1));
                newRoom.floorDisplayChar = "+";
                newRoom.wallDisplayChar = "*";

                buildingList.Add(newRoom);
            }
        }


        private static void PopulateMapData()
        {

            //Draw out landscape rooms
            foreach(Room r in roomList)
            {
                for (int x = r.xLoc; x <= r.XMaxLocation(); x++)
                {
                    for (int y = r.yLoc; y < r.YMaxLocation(); y++)
                    {
                        //Do some additional edge checks
                        if (x >= mapData.GetLength(1) - 1 || y >= mapData.GetLength(0) - 1 || x <= 0 || y <= 0)
                        {
                            break;
                        }

                        mapData[y, x] = r.dataType;
                        mapCollisionData[y, x] = 0;
                        mapDisplayData[y, x] = r.displayCharacter;
                    }
                }
            }

            //Draw out the buildings 
            foreach(Room r in buildingList)
            {
                for (int x = r.xLoc; x <= r.XMaxLocation(); x++)
                {
                    for (int y = r.yLoc; y <= r.YMaxLocation(); y++)
                    {
                        if (x >= mapData.GetLength(1) - 1 || y >= mapData.GetLength(0) - 1 || x <= 0 || y <= 0)
                        {
                            break;
                        }

                        if (x == r.xLoc || x == r.XMaxLocation() || y == r.yLoc || y == r.YMaxLocation())
                        {
                            mapDisplayData[y, x] = r.wallDisplayChar;
                            mapCollisionData[y, x] = 1;
                            mapData[y, x] = 3;

                        }
                        else
                        {
                            mapDisplayData[y, x] = r.floorDisplayChar;
                            mapCollisionData[y, x] = 0;
                            mapData[y, x] = r.floorDataType;
                        }
                    }
                }
            }
        }


        /**
         * This function loops the map data and builds out the wall tile maps.
         **/
        private void RefineTileMap()
        {
            for(int y = 0; y < mapCollisionData.GetLength(0); y++)
            {
                for(int x = 0; x < mapCollisionData.GetLength(1); x++)
                {
                    string north = "0";
                    string east = "0";
                    string south = "0";
                    string west = "0";
                    string tileKey = "TILE_WALL_";

                    if(mapCollisionData[y,x] == 1)
                    {
                        //Check north
                        if(y - 1 >= 0)
                        {
                            if(mapCollisionData[y - 1, x] == 1)
                            {
                                north = "1";
                            }
                        }
                        else
                        {
                            north = "1";
                        }

                        //Check east
                        if (x + 1 <= mapCollisionData.GetLength(1) - 1)
                        {
                            if (mapCollisionData[y, x + 1] == 1)
                            {
                                east = "1";
                            }
                        }
                        else
                        {
                            east = "1";
                        }

                        //Check south
                        if (y + 1 <= mapCollisionData.GetLength(0) - 1)
                        {
                            if (mapCollisionData[y + 1, x] == 1)
                            {
                                south = "1";
                            }
                        }
                        else
                        {
                            south = "1";
                        }


                        //Check west
                        if (x - 1 >= 0)
                        {
                            if (mapCollisionData[y, x - 1] == 1)
                            {
                                west = "1";
                            }
                        }
                        else
                        {
                            west = "1";
                        }

                        tileKey = tileKey + north + east + south + west;
                        mapData[y, x] = tileMapDict[tileKey];
                    }
                }
            }
        }

        private void SetPlayerSpawn()
        {
            Random r = new Random();
            int roomNum = r.Next(0, roomList.Count - 1);
            mapPlayerSpawnLocation = new Tuple<float, float>(r.Next(roomList[roomNum].xLoc, roomList[roomNum].XMaxLocation()) * Global.GRID_WIDTH, r.Next(roomList[roomNum].yLoc, roomList[roomNum].YMaxLocation() ) * Global.GRID_HEIGHT);
        }

        private void SetEnemySpawns()
        {
            Random r = new Random();
            int numOfEnemies = 100;
            int enemyCounter = 0;

            while(enemyCounter < numOfEnemies)
            {
                int roomNum = r.Next(0, roomList.Count - 1);
                Tuple<float, float> enemyLoc = new Tuple<float, float>(r.Next(roomList[roomNum].xLoc, roomList[roomNum].XMaxLocation()) * Global.GRID_WIDTH, r.Next(roomList[roomNum].yLoc, roomList[roomNum].YMaxLocation()) * Global.GRID_HEIGHT);

                //Add logic here to prevent enemies from being spawned right on top of the player

                if(enemyLoc.Item1 > 1 * Global.GRID_WIDTH && enemyLoc.Item1 < mapData.GetLength(1) * Global.GRID_WIDTH 
                    && enemyLoc.Item2 > 1 * Global.GAME_HEIGHT && enemyLoc.Item2 < mapData.GetLength(0) * Global.GRID_HEIGHT)
                {
                    mapEnemySpawnLocations.Add(enemyLoc);
                    enemyCounter++;
                }
                
            }

        }
    }
}
