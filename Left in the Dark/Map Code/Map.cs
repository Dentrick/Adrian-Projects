using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using LitD.Chars;
using LitD.Items;

namespace LitD
{
    /// <summary>
    /// Written by Team Siren
    /// Code for the in-game levels
    /// </summary>
    public class Map
    {
        //rng
        private Random rng;

        //image & color array
        private Texture2D image;
        private Color[,] colors;
        private Vector2 startingPoint;
        private Point endPoint;

        //full list of sprites
        private Texture2D floor;
        private Texture2D end;
        private Texture2D wall;
        private Texture2D door;
        private Texture2D enemy;
        private Texture2D landmark;
        private Texture2D battery;
        private Texture2D bait;
        private Texture2D cola;

        //full map of map objects
        MapObject[,] map;

        //room assignments
        private List<List<MapObject>> rooms;

        private const int tileSideSize = 300;

        //image property
        public Texture2D Image
        {
            get { return image; }
        }

        //room storage
        public List<List<MapObject>> Rooms
        {
            get { return rooms; }
        }

        //map representation storage
        public MapObject[,] MapRep
        {
            get { return map; }
        }

        //player start coordinates
        private Point playerStart;
        public Point PlayerStart
        {
            get { return playerStart; }
        }

        /// <summary>
        /// readies the bitmap for loading
        /// </summary>
        /// <param name="map">The bitmap</param>
        //really huge method but the alternative is just to scan the directory which wasn't really needed
        public Map(Random rng, Texture2D map, Texture2D floor, Texture2D end, Texture2D wall, Texture2D door, Texture2D enemy, Texture2D landmark,
            Texture2D battery, Texture2D bait, Texture2D cola)
        {
            this.rng = rng;
            image = map;
            
            //populate sprites
            this.floor = floor;
            this.end = end;
            this.wall = wall;
            this.door = door;
            this.enemy = enemy;
            this.landmark = landmark;
            this.battery = battery;
            this.bait = bait;
            this.cola = cola;
        }

        public int Width
        {
            get { return image.Width; }
        }

        public int Height
        {
            get { return image.Height; }
        }

        /// <summary>
        /// Loads a level to the screen
        /// </summary>
        /// <param name="gameObjects">collection for map objects to be added to</param>
        /// /// <param name="graphics">GraphicsDevice object</param>
        //TODO: make it work with spritepacks (folders) instead of specific textures (files)
        public void LevelLoad(Game1 game, GraphicsDevice graphics)
        {
            //reset
            rooms = new List<List<MapObject>>();
            this.map = new MapObject[image.Width, image.Height];


            //get an array of color data from the image
            colors = new Color[image.Width, image.Height];
            Color[] data = new Color[colors.Length];
            image.GetData<Color>(data);

            //use data to populate colors 2d array
            //as well as finding the starting point
            int k = 0;
            bool startFound = false;
            bool endFound = false;
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++, k++)
                {
                    colors[j, i] = data[k];
                    if (colors[j, i].Equals(new Color(0, 255, 0)) && !startFound)
                    {
                        startingPoint = new Vector2(
                            -(graphics.Viewport.Width / 2) + (j * tileSideSize) + (2 * (tileSideSize / 3)),
                            -(graphics.Viewport.Height / 2) + (i * tileSideSize) + (2 * (tileSideSize / 3)));
                        startFound = true;
                    }
                    if (colors[j, i].Equals(new Color(0, 150, 0)) && !endFound)
                    {
                        endPoint = new Point(j, i);
                        endFound = true;
                    }
                }
            }

            //actually populate the map with images (also map representation)
            for (int i = 0; i < image.Height; i++, k++)
            {
                for (int j = 0; j < image.Width; j++, k++)
                {
                    //if starting
                    if (colors[j, i].Equals(new Color(0, 255, 0)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), floor);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;

                        playerStart = new Point(j, i);
                    }
                    //if end
                    if (colors[j, i].Equals(new Color(0, 150, 0)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), end, true);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //if floor
                    else if (colors[j, i].Equals(new Color(255, 255, 255)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), floor);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //if wall
                    else if (colors[j, i].Equals(new Color(0, 0, 255)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), wall, true, false);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //if closed door
                    else if (colors[j, i].Equals(new Color(255, 0, 0)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), door, true, true);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //if open door
                    else if (colors[j, i].Equals(new Color(150, 0, 0)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), door, false, true);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //if enemy
                    else if (colors[j, i].Equals(new Color(255, 255, 0)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), floor);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;

                        game.Enemies.Add(new Enemy(
                            -((int)startingPoint.X - ((j * tileSideSize) + 150)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 100)), 
                            enemy, game.GameObjects, new Point(j, i), game.Enemies, game));
                    }
                    //if secret wall
                    else if (colors[j, i].Equals(new Color(0, 0, 150)))
                    {
                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), wall, true, true);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //if landmark
                    else if (colors[j, i].Equals(new Color(0, 255, 255)))
                    {
                        IItem newItem = RNGItem();

                        MapObject mObj = new MapObject(
                            -((int)startingPoint.X - ((j * tileSideSize) + 50)),
                            -((int)startingPoint.Y - ((i * tileSideSize) + 50)), landmark, 
                            (int)Math.Round((endPoint - new Point(j, i)).ToVector2().Length()), newItem);
                        game.GameObjects.Add(mObj);
                        map[j, i] = mObj;
                    }
                    //else do nothing (black/blank)
                }
            }

            //room assignment
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    //start assigning a room from a floor that is not assigned (and not null)
                    if (map[j, i] != null && map[j, i].IsFloor && !map[j, i].FloorAssgn)
                    {
                        List<MapObject> room = new List<MapObject>();
                        rooms.Add(room);
                        RoomAssgnHelper(j, i, room);
                        RoomWallHelper( room);              
                    }
                }
            }

            //fix landmarks
            foreach (MapObject m in map)
            {
                if (m != null && m.IsLandmark)
                {
                    m.LandmarkFix();
                }
            }
        }

        /// <summary>
        /// Helps out with room assignment recursion
        /// </summary>
        /// <param name="j">x</param>
        /// <param name="i">y</param>
        /// <param name="map">representation of map</param>
        /// <param name="room">room to assign to</param>
        private void RoomAssgnHelper(int j, int i, List<MapObject> room)
        {
            room.Add(map[j, i]);
            map[j, i].FloorAssgn = true;

            if (map[j - 1, i] != null && !map[j - 1, i].IsDoor && !map[j - 1, i].IsSolid && !map[j - 1, i].FloorAssgn)
            {
                RoomAssgnHelper(j - 1, i, room);
            }
            if (map[j + 1, i] != null && !map[j + 1, i].IsDoor && !map[j + 1, i].IsSolid && !map[j + 1, i].FloorAssgn)
            {
                RoomAssgnHelper(j + 1, i, room);
            }
            if (map[j, i - 1] != null && !map[j, i - 1].IsDoor && !map[j, i - 1].IsSolid && !map[j, i - 1].FloorAssgn)
            {
                RoomAssgnHelper(j, i - 1, room);
            }
            if (map[j, i + 1] != null && !map[j, i + 1].IsDoor && !map[j, i + 1].IsSolid && !map[j, i + 1].FloorAssgn)
            {
                RoomAssgnHelper(j, i + 1, room);
            }
        }

        /// <summary>
        /// Bleeds room assignment outwards through one layer of wall
        /// </summary>
        /// <param name="map">representation of map</param>
        /// <param name="room">room to assign to</param>
        private void RoomWallHelper(List<MapObject> room)
        {
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    if (map[j, i] != null && (map[j, i].IsDoor || map[j, i].IsSolid))
                    {
                        //first four are x and y axis checks
                        if (map[j - 1, i] != null && !map[j - 1, i].IsDoor && !map[j - 1, i].IsSolid && room.Contains(map[j - 1, i]))
                        {
                            room.Add(map[j, i]);
                        }
                        if (map[j + 1, i] != null && !map[j + 1, i].IsDoor && !map[j + 1, i].IsSolid && room.Contains(map[j + 1, i]))
                        {
                            room.Add(map[j, i]);
                        }
                        if (map[j, i - 1] != null && !map[j, i - 1].IsDoor && !map[j, i - 1].IsSolid && room.Contains(map[j, i - 1]))
                        {
                            room.Add(map[j, i]);
                        }
                        if (map[j, i + 1] != null && !map[j, i + 1].IsDoor && !map[j, i + 1].IsSolid && room.Contains(map[j, i + 1]))
                        {
                            room.Add(map[j, i]);
                        }

                        //extra four are diagonals
                        if (map[j - 1, i - 1] != null && !map[j - 1, i - 1].IsDoor && !map[j - 1, i - 1].IsSolid && room.Contains(map[j - 1, i - 1]))
                        {
                            room.Add(map[j, i]);
                        }
                        if (map[j + 1, i + 1] != null && !map[j + 1, i + 1].IsDoor && !map[j + 1, i + 1].IsSolid && room.Contains(map[j + 1, i + 1]))
                        {
                            room.Add(map[j, i]);
                        }
                        if (map[j + 1, i - 1] != null && !map[j + 1, i - 1].IsDoor && !map[j + 1, i - 1].IsSolid && room.Contains(map[j + 1, i - 1]))
                        {
                            room.Add(map[j, i]);
                        }
                        if (map[j - 1, i + 1] != null && !map[j - 1, i + 1].IsDoor && !map[j - 1, i + 1].IsSolid && room.Contains(map[j - 1, i + 1]))
                        {
                            room.Add(map[j, i]);
                        }
                    }
                }
            }
        }

        private IItem RNGItem()
        {
            //10% battery, 40% cola, 50% bait
            int randNum = rng.Next(0, 100);
            if (randNum <= 10)
            {
                return new BatteryItem(battery);
            }
            else if (randNum <= 50)
            {
                return new ColaItem(cola);
            }
            else
            {
                return new BaitItem(bait);
            }

        }
    }
}
