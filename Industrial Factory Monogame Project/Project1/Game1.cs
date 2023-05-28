using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Reflection;
using SharpDX.Direct3D9;

namespace Industrial_Factory
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        SpriteFont Font;
        int scale;
        MouseState mouse;
        bool mouseLB = true;
        bool mouseRB = true;
        int seed = 2334324;
        int tick, last_tick = -100;
        int id, count;

        Texture2D[][] ObjectTexturesRot;
        Texture2D[] ObjectTextures;

        Camera _camera;

        Rectangle DrawingBorder;

        Dictionary<int, Dictionary<int, int>> CraftPrice = new Dictionary<int, Dictionary<int, int>>
        {
            {0, new Dictionary<int, int>{ {6, 5}, { 8, 5 } } },
            {1, new Dictionary<int, int>{ {6, 5} } },
            {2, new Dictionary<int, int>{ {5, 10} } },
            {3, new Dictionary<int, int>{ {5, 2}, { 7, 3 } } },
            {4, new Dictionary<int, int>{ {5, 4}, { 7, 2 } } },
            {10, new Dictionary<int, int>{ {5, 1}, { 7, 3 } } },
            {11, new Dictionary<int, int>{ {5, 10}, { 7, 5 } } },
        };

        Dictionary<int, int> CraftInfo = new Dictionary<int, int>();

        public static int ScreenW;
        public static int ScreenH;


        //Debug
        bool F3;
        //Debug

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            ScreenW = _graphics.PreferredBackBufferWidth = 1280;
            ScreenH = _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //init Data
            Data.random = new Random();
            Data.Hmap = 300; Data.Wmap = 300;
            Data.SizeTile = 360;
            Data.TileScale = 0.125;
            Data.SizeObj = 90;
            Data.ObjScale = 0.5;
            Data.countObjInTile = (int)((Data.SizeTile * Data.TileScale) / (Data.SizeObj * Data.ObjScale));
            Data.noise = new MapGenerator(Data.Hmap, Data.Wmap, seed);
            Data.mouse2D = new Texture2D[2];
            Data.ObjMap = new Texture2D[Data.Wmap * Data.countObjInTile, Data.Hmap * Data.countObjInTile];
            Data.SelectedBar = 0;
            Data.SelectedRot = 0;
            Data.hotbars = new Texture2D[10];
            Data.RenderedMap = Data.noise.rendermap(Content.Load<Texture2D>("grass0"), Content.Load<Texture2D>("water0"), Content.Load<Texture2D>("sand0"));
            Data.IronOreMap = Data.noise.oreGenerator(Content.Load<Texture2D>("iron_earth"), seed + 1, -0.9);
            Data.IronOreCountMap = Data.noise.GetCount;
            Data.CopperOreMap = Data.noise.oreGenerator(Content.Load<Texture2D>("coper_earth"), seed + 2, -0.9);
            Data.CopperOreCountMap = Data.noise.GetCount;
            Data.CoalOreMap = Data.noise.oreGenerator(Content.Load<Texture2D>("coal_earth"), seed + 3, -0.9);
            Data.CoalOreCountMap = Data.noise.GetCount;
            Data.ObjInf = new double[Data.Wmap * Data.countObjInTile, Data.Hmap * Data.countObjInTile][];
            Data.activeUIs = new List<double[]>(10);
            Data.xUI = 409; Data.yUI = 250;
            Data.UItexture = new Texture2D[10];
            Data.ObjInventory = new Inventory[Data.Wmap * Data.countObjInTile, Data.Hmap * Data.countObjInTile];
            Data.droppedItems = new List<DroppedItem>();
            Data.ProgressBar = new Texture2D[2];
            Data.Progress = 0;
            Data.ProgressBarDraw = false;
            Data.CoolDown_ore = 40;
            Data.Objects = new Object[Data.Wmap * Data.countObjInTile, Data.Hmap * Data.countObjInTile];
            Data.keyE = true;
            //init Data

            _camera = new Camera();
            InitUI();

            base.Initialize();
        }

        private void InitUI()
        {

            Data.activeUIs.Add(new double[] { 409, 673, 0, 1, 461, 46 }); //Hotbar
            Data.activeUIs.Add(new double[] { 409, 450, 1, 0, 461, 185 }); //Inventory
            Data.activeUIs.Add(new double[] { Data.xUI, Data.yUI, 2, 0, 461, 138 }); //Furnance
            Data.activeUIs.Add(new double[] { Data.xUI, Data.yUI, 3, 0, 461, 185 });//Chest
            Data.activeUIs.Add(new double[] { Data.xUI, Data.yUI, 4, 0, 461, 185 });//Ironchest
            Data.activeUIs.Add(new double[] { 871, 356, 1, 0, 231, 277 });

            Data.hotbar = new Inventory(0); //init hotbar items
            Data.hotbar.Item[0, 0] = new double[] { 0, 100 };
            Data.hotbar.Item[1, 0] = new double[] { 1, 100 };
            Data.hotbar.Item[2, 0] = new double[] { 2, 100 };
            Data.hotbar.Item[3, 0] = new double[] { 3, 100 };
            Data.hotbar.Item[4, 0] = new double[] { 4, 100 };
            Data.hotbar.Item[5, 0] = new double[] { 6, 100 };
            Data.hotbar.Item[6, 0] = new double[] { 8, 100 };
            Data.hotbar.Item[7, 0] = new double[] { 9, 100 };
            Data.hotbar.Item[8, 0] = new double[] { 10, 100 }; //electric pole
            Data.hotbar.Item[9, 0] = new double[] { 11, 50 }; //digger

            Data.ProgressBar[0] = Content.Load<Texture2D>("progressBar");
            Data.ProgressBar[1] = Content.Load<Texture2D>("progressBar1");

            Data.invent = new Inventory(1); //init inventory items

            Data.craft = new Inventory(-1);
            Data.craft.Item[0, 0] = new double[] { 0, 0 };
            Data.craft.Item[1, 0] = new double[] { 1, 0 };
            Data.craft.Item[2, 0] = new double[] { 2, 0 };
            Data.craft.Item[3, 0] = new double[] { 3, 0 };
            Data.craft.Item[4, 0] = new double[] { 4, 0 };

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Data.character = new Player(Content.Load<Texture2D>("human10"), _graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            Data.character.Move(0, 0);
            Data.mouse2D[0] = Content.Load<Texture2D>("mouse1");
            Data.mouse2D[1] = Content.Load<Texture2D>("mouse2");
            for (int i = 0; i < 10; i++) Data.hotbars[i] = Content.Load<Texture2D>($"hotbar/hotbar{i}");
            Data.UItexture[0] = Content.Load<Texture2D>("inventoryUI0");
            Data.UItexture[1] = Content.Load<Texture2D>("furnanceUI0");
            Data.UItexture[2] = Content.Load<Texture2D>("chestUI0");
            Data.UItexture[3] = Content.Load<Texture2D>("chestUI0");
            Data.UItexture[4] = Content.Load<Texture2D>("craftUI");
            Data.poleBG = Content.Load<Texture2D>("pole-BG");

            ObjectTexturesRot = new Texture2D[13][];
            for (int i = 0; i < ObjectTexturesRot.Length; i++)
            {
                ObjectTexturesRot[i] = new Texture2D[4];
                for (int rot = 0; rot < 4; rot++)
                {
                    ObjectTexturesRot[i][rot] = LoadTextureObject(i, rot);
                }
            }

            ObjectTextures = new Texture2D[13];
            for(int i = 0; i < ObjectTextures.Length; i++)
            {
                ObjectTextures[i] = LoadTextureObject(i);
            }

            Font = Content.Load<SpriteFont>("font/font");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                Exit();
            // TODO: Add your update logic here
            
            Collide(4);
            
            if (tick - last_tick <= Data.CoolDown_ore) ProgressBar(Data.CoolDown_ore);
            else Data.ProgressBarDraw = false;
            KeyboardIN();
            tick++;
            foreach (DroppedItem item in Data.droppedItems)
            {
                item.OnConveyorCheck();
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _camera.Follow(Data.character);
            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            // TODO: Add your drawing code here

            scale = (int)(Data.SizeTile * Data.TileScale); // scale for Drawing tile
            CorrectBorder();

            #region Draw Titles

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++) //Drawing tiles
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {

                    Rectangle border = new Rectangle(x * scale, y * scale, scale, scale);
                    _spriteBatch.Draw(Data.RenderedMap[x, y], border, Color.White);

                }
            } //Drawing tiles
            #endregion

            _spriteBatch.DrawString(Font, Convert.ToString(DrawingBorder), Vector2.Zero, Color.White);

            #region Draw ores

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++) //Drawing tiles
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    if (Data.RenderedMap[x,y].Tag == "water") continue;
                    Rectangle border = new Rectangle(x * scale, y * scale, scale, scale);
                    if (Data.IronOreMap[x, y] != null)
                    {
                        _spriteBatch.Draw(Data.IronOreMap[x, y], border, Color.White);
                        _spriteBatch.DrawString(Font, Data.IronOreCountMap[x, y].ToString(), border.Location.ToVector2(), Color.White);
                    }
                    else if (Data.CopperOreMap[x, y] != null)
                    {
                        _spriteBatch.Draw(Data.CopperOreMap[x, y], border, Color.White);
                        _spriteBatch.DrawString(Font, Data.CopperOreCountMap[x, y].ToString(), border.Location.ToVector2(), Color.White);
                    }
                    else if (Data.CoalOreMap[x, y] != null)
                    {
                        _spriteBatch.Draw(Data.CoalOreMap[x, y], border, Color.White);
                        _spriteBatch.DrawString(Font, Data.CoalOreCountMap[x, y].ToString(), border.Location.ToVector2(), Color.White);
                    }

                }
            } //Drawing ores
            #endregion

            scale = (int)(Data.SizeObj * Data.ObjScale); //scale for Drawing obj
            CorrectBorder();

            #region Draw Conveyors

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++) 
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    if (Data.ObjInf[x, y] == null) continue;
                    Rectangle poleAround = new Rectangle(x * scale - 2 * scale, y * scale - 2 * scale, scale * 5, scale * 5);
                    if (Data.hotbar.Item[Data.SelectedBar, 0] != null && Data.hotbar.Item[Data.SelectedBar, 0][0] == 10 && Data.ObjInf[x, y][0] == 10) _spriteBatch.Draw(Data.poleBG, poleAround, Color.White);
                    Rectangle border;
                    if (Data.ObjMap[x, y].Tag == "conveer")
                    {
                        border = new Rectangle(x * scale, y * scale - 10, scale, scale + 10);
                        _spriteBatch.Draw(Data.ObjMap[x, y], border, Color.White);
                    }
                }
            }//Draw conveyors
            #endregion

            #region Draw objects behind player without monipulators and conveyors

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++) 
            {
                if (y * scale > Data.character.pos.Y + 4) continue;
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    if (Data.ObjInf[x, y] == null) continue;
                    Rectangle border;
                    if (Data.ObjMap[x, y].Tag == "moni" || Data.ObjMap[x, y].Tag == "conveer" || Data.ObjMap[x,y].Tag == "electric") continue;
                    border = new Rectangle(x * scale, y * scale - 10, scale, scale + 10);
                    _spriteBatch.Draw(Data.ObjMap[x, y], border, Color.White);
                }
            }
            #endregion

            #region Draw Manipulators

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    if (Data.ObjInf[x, y] == null) continue;
                    Rectangle border;
                    if (Data.ObjMap[x, y].Tag == "moni")
                    {
                        border = new Rectangle(x * scale - 18, y * scale - 30, (int)(scale * 1.5 + 23), (int)(scale * 1.5 + 8));
                        _spriteBatch.Draw(Data.ObjMap[x, y], border, Color.White);
                    }
                    if (Data.ObjMap[x, y].Tag == "electric")
                    {
                        border = new Rectangle(x * scale, y * scale - 10, scale, scale + 10);
                        border.Y -= 90; border.Height += 90;
                        _spriteBatch.Draw(Data.ObjMap[x, y], border, Color.White);
                    }

                }
            }
            #endregion

            #region DroppedItems

            foreach (DroppedItem item in Data.droppedItems)
            {
                _spriteBatch.Draw(GetTextureObject(item.id, false), item.Item, Color.White);
            }

            #endregion

            _spriteBatch.Draw(Data.character.texture, Data.character.pos, Color.White); //Draw player

            #region Draw objects in front of player without monipulators and conveyors

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                if (y * scale < Data.character.pos.Y) continue;
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    if (x * scale < -Data.character.VPx - 50 - scale) continue;
                    if (Data.ObjInf[x, y] == null) continue;
                    Rectangle border;
                    if (Data.ObjMap[x, y].Tag == "moni" || Data.ObjMap[x, y].Tag == "conveer" || Data.ObjMap[x,y].Tag == "electric") continue;
                    border = new Rectangle(x * scale, y * scale - 10, scale, scale + 10);
                    _spriteBatch.Draw(Data.ObjMap[x, y], border, Color.White);

                }
            }
            #endregion

            //scale changed to Tiles
            #region Debug

            if (F3) //Debug
            {
                _spriteBatch.DrawString(Font, "P pos --- " + Convert.ToString(Data.character.pos), new Vector2(-Data.character.VPx, -Data.character.VPy), Color.Black);
                _spriteBatch.DrawString(Font, "Map height = " + Convert.ToString(Data.Hmap) + "  Map Width = " + Convert.ToString(Data.Wmap), new Vector2(-Data.character.VPx, -Data.character.VPy + 20), Color.Black);
                _spriteBatch.DrawString(Font, "DIRECTX 11 : STABLE WORKING", new Vector2(-Data.character.VPx, -Data.character.VPy + 40), Color.Black);
                _spriteBatch.DrawString(Font, Convert.ToString(mouse.ScrollWheelValue), new Vector2(-Data.character.VPx, -Data.character.VPy + 60), Color.Black);
                _spriteBatch.DrawString(Font, "ViewPort pos -  " + Convert.ToString(Data.character.VPx) + " --- " + Convert.ToString(Data.character.VPy), new Vector2(-Data.character.VPx, -Data.character.VPy + 80), Color.Black);


                scale = (int)(Data.SizeTile * Data.TileScale);
                CorrectBorder();
                for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
                {
                    for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                    {
                        if (Data.RenderedMap[x, y].Tag != "water") continue;

                        Rectangle border = new Rectangle(x * scale, y * scale, scale, scale);
                        _spriteBatch.Draw(Data.mouse2D[0], border, Color.White);

                    }
                }
            }
            #endregion

            scale = (int)(Data.SizeObj * Data.ObjScale); // scale for Drawing obj
            CorrectBorder();

            #region ObjectUI

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    double[] Obj = Data.ObjInf[x, y];
                    if (Obj == null) continue;
                    if (Obj[1] == 0) continue;
                    if ((int)Obj[0] + 3 > Data.activeUIs.Count - 1) continue;
                    double[] UI = Data.activeUIs[(int)Obj[0] + 2]; //Math.Pow(Math.Pow(Data.character.pos.X - UI[0], 2) + Math.Pow(Data.character.pos.Y - UI[1], 2), 0.5)

                    double dist = Math.Pow(Math.Pow(Data.character.pos.X - Obj[3], 2) + Math.Pow(Data.character.pos.Y - Obj[4], 2), 0.5);
                    if (F3) _spriteBatch.DrawString(Font, "distance to opened obj --- " + Convert.ToString(dist), new Vector2(-Data.character.VPx, -Data.character.VPy + 200), Color.Black);
                    if (dist > 600)
                    {
                        Obj[1] = 0;
                        Data.activeUIs[(int)Obj[0] + 2][3] = 0;
                        continue;
                    }
                    _spriteBatch.Draw(Data.UItexture[(int)Obj[0] + 1], new Rectangle((int)(UI[0] - Data.character.VPx), (int)(UI[1] - Data.character.VPy), (int)UI[4], (int)UI[5]), Color.White);
                }
            }
            #endregion

            #region Player Inventory and items and craft

            if (Data.activeUIs[1][3] == 1)
            {
                _spriteBatch.Draw(Data.UItexture[0], new Rectangle((int)(Data.activeUIs[1][0] - Data.character.VPx), (int)(Data.activeUIs[1][1] - Data.character.VPy), (int)Data.activeUIs[1][4], (int)Data.activeUIs[1][5]), Color.White);
                _spriteBatch.Draw(Data.UItexture[4], new Rectangle((int)(Data.activeUIs[1][0] - Data.character.VPx) + (int)Data.activeUIs[1][4], (int)(Data.activeUIs[1][1] - Data.character.VPy) - Data.SizeObj - 2, 231, 277), Color.White);
                for (int column = 0; column < 10; column++)//for character inventerory
                {
                    for (int line = 0; line < 3; line++)
                    {
                        Rectangle Grid = Data.invent.Grid[column, line];
                        if (Data.invent.Item[column, line] == null) continue;
                        Vector2 Pos = new Vector2(Grid.X - Data.character.VPx + 2, Grid.Y - Data.character.VPy + 2 + 27); ;
                        Rectangle Pos1 = new Rectangle(Grid.X - (int)Data.character.VPx + 2, Grid.Y - (int)Data.character.VPy + 2, Grid.Width - 4, Grid.Height - 4);
                        _spriteBatch.Draw(GetTextureObject((int)Data.invent.Item[column, line][0], false), Pos1, Color.White);
                        _spriteBatch.DrawString(Font, Convert.ToString(Data.invent.Item[column, line][1]), Pos, Color.White);
                    }
                }
                for (int column = 0; column < 5; column++)//for character craft
                {
                    for (int line = 0; line < 5; line++)
                    {
                        Rectangle Grid = Data.craft.Grid[column, line];
                        if (Data.craft.Item[column, line] == null) continue;
                        Vector2 Pos = new Vector2(Grid.X - Data.character.VPx + 2, Grid.Y - Data.character.VPy + 2 + 27); ;
                        Rectangle Pos1 = new Rectangle(Grid.X - (int)Data.character.VPx + 2, Grid.Y - (int)Data.character.VPy + 2, Grid.Width - 4, Grid.Height - 4);
                        _spriteBatch.Draw(GetTextureObject((int)Data.craft.Item[column, line][0], false), Pos1, Color.White);
                    }
                }

                int InfoCounter = 0;
                foreach(int key in CraftInfo.Keys)
                {
                    Rectangle itemPos = new Rectangle(new Point((int)(mouse.X + 25 * InfoCounter - Data.character.VPx - 30), (int)(mouse.Y - Data.character.VPy - 30)), new Point(25));
                    Vector2 CountPos = itemPos.Location.ToVector2();
                    _spriteBatch.Draw(GetTextureObject(key, false), itemPos, Color.White);
                    _spriteBatch.DrawString(Font, Convert.ToString(CraftInfo[key]), CountPos, Color.White);
                    InfoCounter++;
                }
                CraftInfo = new Dictionary<int, int>();

            }
            //Draw inventory
            #endregion

            _spriteBatch.Draw(Data.hotbars[Data.SelectedBar], new Rectangle(409 - (int)Data.character.VPx, 673 - (int)Data.character.VPy, 461, 47), Color.White); //hotbar

            #region Object Inventory items


            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    Inventory items;
                    if (Data.ObjInf[x, y] == null) continue;
                    if (Data.ObjInf[x, y][1] == 0) continue;
                    items = Data.ObjInventory[x, y];
                    if (items == null) continue;
                    for (int column = 0; column < items.Item.GetLength(0); column++)
                    {
                        for (int line = 0; line < items.Item.GetLength(1); line++)
                        {
                            Rectangle Grid = items.Grid[column, line];
                            if (items.Item[column, line] == null) continue;

                            Vector2 Pos = new Vector2(Grid.X - Data.character.VPx + 2, Grid.Y - Data.character.VPy + 2 + 27); ;
                            Rectangle Pos1 = new Rectangle(Grid.X - (int)Data.character.VPx + 2, Grid.Y - (int)Data.character.VPy + 2, Grid.Width - 4, Grid.Height - 4);
                            _spriteBatch.Draw(GetTextureObject((int)items.Item[column, line][0], false), Pos1, Color.White);

                            _spriteBatch.DrawString(Font, Convert.ToString(items.Item[column, line][1]), Pos, Color.White);

                        }
                    }
                }
            }
            #endregion

            #region MouseIN
            mouse = Mouse.GetState();

            if ((MouseIN().X == -1234 || Data.MouseHaveObj) && (id != -1 || count != -1)) //mouseLB on ui
            {
                double x = -Data.character.VPx + mouse.X;
                double y = -Data.character.VPy + mouse.Y;
                Rectangle ObjInMouse = new Rectangle((int)x - 2, (int)y - 2, 41, 41);
                _spriteBatch.Draw(GetTextureObject(id, false), ObjInMouse, Color.White);
                _spriteBatch.DrawString(Font, Convert.ToString(count), new Vector2(ObjInMouse.X, ObjInMouse.Y + 27), Color.Black);
                _spriteBatch.Draw(Data.mouse2D[0], MouseIN(), Color.White);
            }
            else //Mouse move
            {
                double x = -Data.character.VPx + mouse.X;
                double y = -Data.character.VPy + mouse.Y;
                Rectangle poleAround = new Rectangle((int)(x - x % scale) - 2 * scale, (int)(y - y % scale) - 2 * scale, 5 * scale, 5 * scale);
                _spriteBatch.Draw(Data.mouse2D[0], MouseIN(), Color.White);
                if (Data.hotbar.Item[Data.SelectedBar, 0] != null && Data.hotbar.Item[Data.SelectedBar, 0][0] == 10) _spriteBatch.Draw(Data.poleBG,poleAround, Color.White );
            }
            #endregion

            #region For Character HotBar

            for (int i = 0; i < 10; i++) //for character hotbar
            {
                Rectangle Grid = Data.hotbar.Grid[i, 0];
                if (Data.hotbar.Item[i, 0] == null) continue;
                Vector2 Pos = new Vector2(Grid.X - Data.character.VPx + 2, Grid.Y - Data.character.VPy + 2 + 27); ;
                Rectangle Pos1 = new Rectangle(Grid.X - (int)Data.character.VPx + 2, Grid.Y - (int)Data.character.VPy + 2, Grid.Width - 4, Grid.Height - 4);
                _spriteBatch.Draw(GetTextureObject((int)Data.hotbar.Item[i, 0][0], false), Pos1, Color.White);
                _spriteBatch.DrawString(Font, Convert.ToString(Data.hotbar.Item[i, 0][1]), Pos, Color.White);
            }
            #endregion

            #region Player ProgressBar

            if (Data.ProgressBarDraw)
            {
                Rectangle temp = new Rectangle(409 - (int)Data.character.VPx, 660 - (int)Data.character.VPy, 461, 13 );
                _spriteBatch.Draw(Data.ProgressBar[0], temp, Color.White);
                temp = new Rectangle(409 - (int)Data.character.VPx, 660 - (int)Data.character.VPy, (int)(4.6 * Data.Progress), 13);
                _spriteBatch.Draw(Data.ProgressBar[1], temp, Color.White);
            }
            #endregion

            #region Objects update ProgressBar

            foreach (var obj in Data.Objects)
            {
                if (obj == null) continue;
                obj.Update(tick);
                if (obj.ProgressBarDraw && Data.ObjInf[obj.x, obj.y][1] == 1)
                {
                    Rectangle temp = new Rectangle(Data.xUI - (int)Data.character.VPx, Data.yUI - (int)Data.character.VPy - 13, 461, 13);
                    _spriteBatch.Draw(Data.ProgressBar[0], temp, Color.White);
                    temp = new Rectangle(Data.xUI - (int)Data.character.VPx, Data.yUI - (int)Data.character.VPy - 13, (int)(4.6 * obj.Progress), 13);
                    _spriteBatch.Draw(Data.ProgressBar[1], temp, Color.White);
                }
            }
            #endregion

            _spriteBatch.End();
            base.Draw(gameTime);
        }


        private bool MouseUI(Point mouse)
        {
            Point mous = new Point((int)(mouse.X + Data.character.VPx), (int)(mouse.Y + Data.character.VPy));
            foreach (double[] UI in Data.activeUIs)
            {
                if (UI == null) continue;
                Rectangle UIObj = new Rectangle((int)UI[0], (int)UI[1], (int)UI[4], (int)UI[5]);
                if (UIObj.Contains(mous) && UI[3] == 1)
                    return true;
            }
            return false;
        }

        private Rectangle MouseUI(Point mouse, int click)
        {
            scale = (int)(Data.SizeObj * Data.ObjScale); 
            Rectangle pos = new Rectangle();
            Point mous = mouse;
            bool clickL = false, clickR = false;
            switch (click)
            {
                case 0:
                    break;
                case 1:
                    clickL = true;
                    break;
                case 2:
                    clickR = true;
                    break;
            }



            #region Inventory of objects

            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {


                    if (Data.ObjInf[x, y] == null) continue;
                    if (Data.ObjInf[x, y][1] == 0) continue;
                    Inventory inventory = Data.ObjInventory[x, y];
                    if (inventory == null) continue;
                    for (int column = 0; column < inventory.Item.GetLength(0); column++)
                    {
                        for (int line = 0; line < inventory.Item.GetLength(1); line++)
                        {

                            pos = inventory.Grid[column, line];
                            pos.X -= (int)Data.character.VPx;
                            pos.Y -= (int)Data.character.VPy;

                            if (pos.Contains(mous) && !clickL && !clickR)
                            {
                                return pos;
                            }
                            else if (pos.Contains(mous) && clickL && Data.MouseHaveObj)
                            {
                                if (inventory.PutItemToInventoryForMouse(column, line, id, count))
                                {
                                    id = (int)inventory.Replace[0];
                                    count = (int)inventory.Replace[1];
                                }
                                else
                                {
                                    id = -1;
                                    count = -1;
                                    Data.MouseHaveObj = false;
                                }
                                return pos;
                            }
                            else if (pos.Contains(mous) && clickL)
                            {

                                pos.X = -1234;
                                if (inventory.Item[column, line] == null) return pos;
                                id = (int)inventory.Item[column, line][0];
                                count = (int)inventory.Item[column, line][1];
                                bool Done = false;
                                if (Data.Shift && !Done)
                                {

                                    foreach (double[] i in Data.hotbar.Item) if (i != null && i[0] == id) { i[1] += count; Done = true; break; }

                                }
                                if (Data.Shift && !Done)
                                {
                                    foreach (double[] i in Data.invent.Item) if (i != null && i[0] == id) { i[1] += count; Done = true; break; }
                                    if (!Done) { if (Data.invent.PutItemToInventory(id, count)) Done = true; }
                                    if (!Done) { if (Data.hotbar.PutItemToInventory(id, count)) Done = true; }
                                }
                                if (Done) inventory.GetItemToMouse(column, line);
                                if (!Data.Shift)
                                {
                                    Data.MouseHaveObj = true;
                                    inventory.GetItemToMouse(column, line);
                                }
                                return pos;
                            }
                            else if (pos.Contains(mous) && clickR && Data.MouseHaveObj && inventory.ExistItem(column, line, id))
                            {
                                inventory.PutItemToInventoryForMouse(column, line, id, 1);
                                if (count - 1 > 0) count -= 1;
                                else Data.MouseHaveObj = false;

                            }
                            else if (pos.Contains(mous) && clickR && inventory.Item[column, line] != null && !Data.MouseHaveObj)
                            {
                                pos.X = -1234;
                                id = (int)inventory.Item[column, line][0];
                                int temp = (int)inventory.Item[column, line][1];
                                count = temp / 2 + temp % 2;
                                Data.MouseHaveObj = true;
                                inventory.GetHalfToMouse(column, line);
                                return pos;
                            }
                        }

                    }

                }
            }
            #endregion

            #region HotBar

            for (int column = 0; column < Data.hotbar.Item.GetLength(0); column++)
            {
                for (int line = 0; line < Data.hotbar.Item.GetLength(1); line++)
                {

                    pos = Data.hotbar.Grid[column, line];
                    pos.X -= (int)Data.character.VPx;
                    pos.Y -= (int)Data.character.VPy;
                    if (pos.Contains(mous) && !clickL && !clickR)
                    {
                        return pos;
                    }
                    else if (pos.Contains(mous) && clickL && Data.MouseHaveObj)
                    {
                        if (Data.hotbar.PutItemToInventoryForMouse(column, line, id, count))
                        {
                            id = (int)Data.hotbar.Replace[0];
                            count = (int)Data.hotbar.Replace[1];
                        }
                        else
                        {
                            id = -1;
                            count = -1;
                            Data.MouseHaveObj = false;
                        }
                        return pos;
                    }
                    else if (pos.Contains(mous) && clickL)
                    {
                        pos.X = -1234;
                        if (Data.hotbar.Item[column, line] != null)
                        {
                            id = (int)Data.hotbar.Item[column, line][0];
                            count = (int)Data.hotbar.Item[column, line][1];
                            Data.MouseHaveObj = true;

                            Data.hotbar.GetItemToMouse(column, line);

                            return pos;
                        }
                    }
                    else if (pos.Contains(mous) && clickR && Data.MouseHaveObj && Data.hotbar.ExistItem(column, line, id))
                    {
                        Data.hotbar.PutItemToInventoryForMouse(column, line, id, 1);
                        if (count - 1 > 0) count -= 1;
                        else Data.MouseHaveObj = false;
                    }
                    else if (pos.Contains(mous) && clickR && Data.hotbar.Item[column, line] != null && !Data.MouseHaveObj)
                    {
                        pos.X = -1234;
                        id = (int)Data.hotbar.Item[column, line][0];
                        int temp = (int)Data.hotbar.Item[column, line][1];
                        count = temp / 2 + temp % 2;
                        Data.MouseHaveObj = true;
                        Data.hotbar.GetHalfToMouse(column, line);
                        return pos;
                    }
                }
            }
            #endregion

            #region Inventory player

            for (int column = 0; column < Data.invent.Item.GetLength(0); column++)
            {
                for (int line = 0; line < Data.invent.Item.GetLength(1); line++)
                {

                    pos = Data.invent.Grid[column, line];
                    pos.X -= (int)Data.character.VPx;
                    pos.Y -= (int)Data.character.VPy;
                    if (pos.Contains(mous) && !clickL && !clickR)
                    {
                        return pos;
                    }
                    else if (pos.Contains(mous) && clickL && Data.MouseHaveObj)
                    {
                        if (Data.invent.PutItemToInventoryForMouse(column, line, id, count))
                        {
                            id = (int)Data.invent.Replace[0];
                            count = (int)Data.invent.Replace[1];
                        }
                        else
                        {
                            id = -1;
                            count = -1;
                            Data.MouseHaveObj = false;
                        }
                        return pos;

                    }
                    else if (pos.Contains(mous) && clickL)
                    {
                        pos.X = -1234;
                        if (Data.invent.Item[column, line] != null)
                        {
                            id = (int)Data.invent.Item[column, line][0];
                            count = (int)Data.invent.Item[column, line][1];
                            Data.MouseHaveObj = true;
                            Data.invent.GetItemToMouse(column, line);
                        }
                        return pos;

                    }
                    else if (pos.Contains(mous) && clickR && Data.MouseHaveObj && Data.invent.ExistItem(column, line, id))
                    {
                        Data.invent.PutItemToInventoryForMouse(column, line, id, 1);
                        if (count - 1 > 0) count -= 1;
                        else Data.MouseHaveObj = false;
                    }
                    else if (pos.Contains(mous) && clickR && Data.invent.Item[column, line] != null && !Data.MouseHaveObj)
                    {
                        pos.X = -1234;
                        id = (int)Data.invent.Item[column, line][0];
                        int temp = (int)Data.invent.Item[column, line][1];
                        count = temp / 2 + temp % 2;
                        Data.MouseHaveObj = true;
                        Data.invent.GetHalfToMouse(column, line);
                        return pos;
                    }
                }
            }
            #endregion

            #region Craft Interface

            
            for (int column = 0; column < Data.craft.Item.GetLength(0); column++)
            {
                for (int line = 0; line < Data.craft.Item.GetLength(1); line++)
                {
                    pos = Data.craft.Grid[column, line];
                    pos.X -= (int)Data.character.VPx;
                    pos.Y -= (int)Data.character.VPy;
                    if (pos.Contains(mous) && !clickL && !clickR)
                    {
                        if (Data.craft.Item[column, line] != null) CraftInfo = CraftPrice[(int)Data.craft.Item[column, line][0]];
                        return pos;
                    }

                    if (pos.Contains(mous) && clickL)
                    {
                        if (Data.craft.Item[column, line] != null) Craft((int)Data.craft.Item[column, line][0]);
                        return pos;
                    }

                    if (pos.Contains(mous) && clickR)
                    {
                        if (Data.craft.Item[column, line] != null) Craft((int)Data.craft.Item[column, line][0], 5);
                        return pos;
                    }

                }
            }

            #endregion


            return new Rectangle(-1000, -1000, 1, 1);
        }

        private bool Craft(int id, int count = 1)
        {
            List<bool> ResourcesExist = new List<bool>();
            bool craft = true;
            CraftInfo = CraftPrice[id];
            foreach (int key in CraftInfo.Keys)
            {
                if (Data.invent.GetItemFind(key, CraftInfo[key] * count, true)) ResourcesExist.Add(true);
                else if (Data.hotbar.GetItemFind(key, CraftInfo[key] * count, true)) ResourcesExist.Add(true);
                else ResourcesExist.Add(false);
            }
            foreach (bool b in ResourcesExist) if (!b) craft = false;
            if (craft)
            {
                foreach (int key in CraftInfo.Keys)
                {
                    if (Data.invent.GetItemFind(key, CraftInfo[key] * count)) ResourcesExist.Add(true);
                    else if (Data.hotbar.GetItemFind(key, CraftInfo[key] * count)) ResourcesExist.Add(true);
                }
                if (!Data.invent.PutItemToInventory(id, count)) 
                    if(!Data.hotbar.PutItemToInventory(id, count))
                    {
                        foreach (int key in CraftInfo.Keys)
                        {
                            Data.invent.PutItemToInventory(key, CraftInfo[key] * count);
                            Data.hotbar.PutItemToInventory(key, CraftInfo[key] * count);
                        }
                    }
            }


            return true;
        }

        private void CorrectBorder()
        {
            DrawingBorder = new Rectangle(
                (int)(-Data.character.VPx + Data.character.VPx % scale) / scale - 10,
                (int)(-Data.character.VPy + Data.character.VPx % scale) / scale - 10,
                (int)(-Data.character.VPx + Data.character.VPx % scale) / scale + 40,
                (int)(-Data.character.VPy + Data.character.VPx % scale) / scale + 40);
            if (DrawingBorder.X < 0) DrawingBorder.X = 0;
            if (DrawingBorder.Y < 0) DrawingBorder.Y = 0;
            if (DrawingBorder.Width > Data.Wmap) DrawingBorder.Width = Data.Wmap;
            if (DrawingBorder.Height > Data.Hmap) DrawingBorder.Height = Data.Hmap;
        }

        public void DropItem(Point mous)
        {
            scale = (int)(Data.SizeTile * Data.TileScale); // scale for Drawing tile
            if (Data.RenderedMap[(int)(mous.X - mous.X % scale) / scale, (int)(mous.Y - mous.Y % scale) / scale].Tag == "water") return;

            scale = (int)(Data.SizeObj * Data.ObjScale);
            Point itempoint = new Point(mous.X - scale / 2, mous.Y - scale / 2);
            Rectangle itemrect = new Rectangle(itempoint, new Point(20));
            foreach (var drop in Data.droppedItems) if (itemrect.Intersects(drop.Item))
                {
                    return;
                }
            Data.droppedItems.Add(new DroppedItem(id, mous.X - scale / 2, mous.Y - scale / 2));
            count -= 1;
            if (count <= 0) Data.MouseHaveObj = false;

        }

        public void GetItem()
        {
            for (int i = 0; i < Data.droppedItems.Count; i++)
            {
                DroppedItem item = Data.droppedItems[i];
                if (item.Item.Intersects(Data.character.pos))
                {
                    if (!Data.invent.PutItemToInventory(item.id, 1))
                        if (!Data.hotbar.PutItemToInventory(item.id, 1)) return;
                        else Data.droppedItems.Remove(item);
                    else Data.droppedItems.Remove(item);
                }
            }
        }

        public Rectangle MouseIN() //Mouse move Input
        {
            
            scale = (int)(Data.SizeObj * Data.ObjScale);
            mouse = Mouse.GetState();
            Data.SelectedBar = Math.Abs((mouse.ScrollWheelValue / 120) % 10);
            double x = -Data.character.VPx + mouse.X;
            double y = -Data.character.VPy + mouse.Y;
            Point mous = new Point((int)x, (int)y);
            Rectangle RObj = new Rectangle((int)(x - x % scale), (int)(y - y % scale),scale, scale);
            if (mouse.LeftButton == ButtonState.Pressed && mouseLB)
            {
                mouseLB = false;
                if (MouseUI(mous))
                {

                    return MouseUI(mous, 1);
                }
                else if (!Data.MouseHaveObj)
                    MouseLeft(RObj, mous);
                else
                    DropItem(mous);


            }
            if (mouse.RightButton == ButtonState.Pressed && mouseRB)
            {
                mouseRB = false;
                if (MouseUI(mous))
                    return MouseUI(mous, 2);
                else 
                    MouseRight(RObj, mous);

            }
            if (mouse.LeftButton == ButtonState.Released) mouseLB = true;

            if (mouse.RightButton == ButtonState.Released) mouseRB = true;
            if (MouseUI(mous))
                return MouseUI(mous, 0);
            return RObj;
            
        }

        private void ProgressBar(int MaxValue = 100)
        {
            int value = tick - last_tick;
            Data.ProgressBarDraw = true;
            Data.Progress = (int)((double)value / MaxValue * 100);
            mouseRB = true;
        }

        public void MouseRight(Rectangle RObj, Point mous)
        {
            scale = (int)(Data.SizeObj * Data.ObjScale);
            int x = RObj.X / scale;
            int y = RObj.Y / scale;
            if (Data.ObjMap.GetLength(0) < x || Data.ObjMap.GetLength(1) < y) return;
            else if (Data.ObjInf[x, y] != null)
            {
                if (Data.ObjInventory[x, y] != null)
                {
                    List<bool> Done = new List<bool>();
                    int last = 0;

                    for (int column = 0; column < Data.ObjInventory[x, y].Item.GetLength(0); column++)
                    {
                        for (int line = 0; line < Data.ObjInventory[x, y].Item.GetLength(1); line++)
                        {
                            double[] i = Data.ObjInventory[x, y].Item[column, line];
                            if (i != null)
                            {
                                Done.Add(false);

                                foreach (double[] j in Data.hotbar.Item) if (j != null && j[0] == i[0]) { j[1] += i[1]; Done[last] = true; break; }

                                if (!Done[last])
                                {
                                    foreach (double[] j in Data.invent.Item) if (j != null && j[0] == i[0]) { j[1] += i[1]; Done[last] = true; break; }
                                    if (!Done[last]) if (Data.invent.PutItemToInventory((int)i[0], (int)i[1])) Done[last] = true;
                                    if (!Done[last]) if (Data.hotbar.PutItemToInventory((int)i[0], (int)i[1])) Done[last] = true;
                                }
                                if (Done[last]) Data.ObjInventory[x, y].Item[column, line] = null;
                                last++;

                            }

                        }
                    }
                    foreach (bool done in Done) if (!done) return;
                   
                }
                bool Exist = false;
                foreach (double[] j in Data.hotbar.Item) if (j != null && j[0] == Data.ObjInf[x, y][0]) { j[1] += 1; Exist = true; break; }
                foreach (double[] j in Data.invent.Item) if (j != null && j[0] == Data.ObjInf[x, y][0] && !Exist) { j[1] += 1; Exist = true; break; }
                if (!Exist) if (Data.invent.PutItemToInventory((int)Data.ObjInf[x, y][0], 1)) Exist = true;
                if (!Exist) if (Data.hotbar.PutItemToInventory((int)Data.ObjInf[x, y][0], 1)) Exist = true;
                if (!Exist) return;
                Data.ObjInf[x, y] = null;
                Data.ObjMap[x, y] = null;
                Data.ObjInventory[x, y] = null;
                Data.Objects[x, y] = null;
                for (int i = 2; i < Data.activeUIs.Count; i++)
                {
                    if (Data.activeUIs[i] != null) Data.activeUIs[i][3] = 0;
                }
            }
            if (tick - last_tick > Data.CoolDown_ore)
            {
                
                if (Data.IronOreCountMap[x, y] > 0)
                {
                    bool Exist = false;
                    if (!Exist) if (Data.invent.PutItemToInventory(6, 1)) Exist = true;
                    if (!Exist) if (Data.hotbar.PutItemToInventory(6, 1)) Exist = true;
                    if (!Exist) return;
                    last_tick = tick;
                    Data.IronOreCountMap[x, y]--;
                    if (Data.IronOreCountMap[x, y] == 0) Data.IronOreMap[x, y] = null;
                }

                if (Data.CopperOreCountMap[x, y] > 0)
                {
                    bool Exist = false;
                    if (!Exist) if (Data.invent.PutItemToInventory(8, 1)) Exist = true;
                    if (!Exist) if (Data.hotbar.PutItemToInventory(8, 1)) Exist = true;
                    if (!Exist) return;
                    last_tick = tick;
                    Data.CopperOreCountMap[x, y]--;
                    if (Data.CopperOreCountMap[x, y] == 0) Data.CopperOreMap[x, y] = null;
                }

                if (Data.CoalOreCountMap[x, y] > 0)
                {
                    bool Exist = false;
                    if (!Exist) if (Data.invent.PutItemToInventory(9, 1)) Exist = true;
                    if (!Exist) if (Data.hotbar.PutItemToInventory(9, 1)) Exist = true;
                    if (!Exist) return;
                    last_tick = tick;
                    Data.CoalOreCountMap[x, y]--;
                    if (Data.CoalOreCountMap[x, y] == 0) Data.CoalOreMap[x, y] = null;
                }
            }
            
            
        }

        public void MouseLeft(Rectangle RObj, Point mous) //(x, y of left up border, W and H of Obj)
        {
            int scale = (int)(Data.SizeObj * Data.ObjScale);
            if (Data.ObjMap.GetLength(0) < (int)RObj.X / scale || Data.ObjMap.GetLength(1) < (int)RObj.Y / scale) return;
            else if (Data.ObjInf[(int)RObj.X / scale, (int)RObj.Y / scale] == null && Collide(RObj) && CollideWater(RObj)) 
            {
                if (Data.hotbar.Item[Data.SelectedBar, 0] == null) return;
                if (Data.hotbar.Item[Data.SelectedBar, 0][1] == 0) return;
                if (Data.hotbar.Item[Data.SelectedBar, 0][0] < 5 || Data.hotbar.Item[Data.SelectedBar, 0][0] == 10 || Data.hotbar.Item[Data.SelectedBar, 0][0] == 11) ;    //добавить возможность установки объекта с новым id
                else return;
                Data.ObjInf[(int)RObj.X / scale, (int)RObj.Y / scale] = new double[] { Data.hotbar.Item[Data.SelectedBar, 0][0], 0, Data.SelectedRot, RObj.X, RObj.Y };
                
                if (Data.hotbar.Item[Data.SelectedBar, 0][0] + 1 < 4)  //проверка id
                {
                    Data.ObjInventory[(int)RObj.X / scale, (int)RObj.Y / scale] = new Inventory((int)Data.hotbar.Item[Data.SelectedBar, 0][0] + 2);
                }
                

                switch ((int)Data.hotbar.Item[Data.SelectedBar, 0][0])
                {
                    case 0:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(0);
                        Data.Objects[RObj.X / scale, RObj.Y / scale] = new Object(0, Data.ObjInventory[RObj.X / scale, RObj.Y / scale], RObj.X / scale, RObj.Y / scale);
                        break;
                    case 1:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(1);
                        break;
                    case 2:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(2);
                        break;
                    case 3:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(3);
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale].Tag = "conveer";
                        break;
                    case 4:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(4);
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale].Tag = "moni";
                        Data.Objects[RObj.X / scale, RObj.Y / scale] = new Object(4, Data.ObjInventory[RObj.X / scale, RObj.Y / scale], RObj.X / scale, RObj.Y / scale);
                        break;
                    case 10:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(10);
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale].Tag = "electric";
                        Data.Objects[RObj.X / scale, RObj.Y / scale] = new Object(10, null, RObj.X / scale, RObj.Y / scale);
                        break;
                    case 11:
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale] = GetTextureObject(11);
                        Data.ObjMap[(int)RObj.X / scale, (int)RObj.Y / scale].Tag = "digger";
                        Data.Objects[RObj.X / scale, RObj.Y / scale] = new Object(11, null, RObj.X / scale, RObj.Y / scale);
                        break;
                }
                Data.hotbar.GetOneItem(Data.SelectedBar, 0);



            }
            else if (RObj.Contains(mous) && Collide(4) && Data.ObjInf[(int)RObj.X / scale, (int)RObj.Y / scale] != null)
            {
                foreach (double[] z in Data.ObjInf)
                {
                    if (z != null)
                    {
                        z[1] = 0;
                    }
                }
                for (int i = 2; i < Data.activeUIs.Count; i++)
                {
                    if (Data.activeUIs[i] != null) Data.activeUIs[i][3] = 0;
                }
                Data.ObjInf[(int)RObj.X / scale, (int)RObj.Y / scale][1] = 1;
                if ((int)Data.ObjInf[(int)RObj.X / scale, (int)RObj.Y / scale][0] + 3 <= Data.activeUIs.Count)
                    Data.activeUIs[(int)Data.ObjInf[(int)RObj.X / scale, (int)RObj.Y / scale][0] + 2][3] = 1;
            }
        }

        public Texture2D GetTextureObject(int i, bool rot = true)
        {
            if (rot)
            {
                return ObjectTexturesRot[i][Data.SelectedRot];
            }
            else
            {
                return ObjectTextures[i];
            }
        }

        public Texture2D LoadTextureObject(int i, int rot = -1)
        {
            if (rot != -1)
                switch (i) //10 pole, 11 digger
                {
                    case 0:
                        return Content.Load<Texture2D>($"Furnance{rot + 1}");
                    case 1:
                        return Content.Load<Texture2D>($"Chest{rot}");
                    case 2:
                        return Content.Load<Texture2D>($"ChestIron{rot}");
                    case 3:
                        return Content.Load<Texture2D>($"conveer{rot}");
                    case 4:
                        return Content.Load<Texture2D>($"monipulator{rot}");
                    case 5:
                        return Content.Load<Texture2D>($"Furnance{rot + 1}_activated");
                    case 10:
                        return Content.Load<Texture2D>($"ElectricPole");
                    case 11:
                        return Content.Load<Texture2D>($"Digger{rot}");
                    case 12:
                        return Content.Load<Texture2D>($"Generator");
                    default:
                        return Content.Load<Texture2D>($"FurnanceOFf0"); // electric pole ;

                }
            else
                switch (i)
                {
                    case 0:
                        return Content.Load<Texture2D>($"Furnance1");
                    case 1:
                        return Content.Load<Texture2D>($"Chest0");
                    case 2:
                        return Content.Load<Texture2D>($"ChestIron0");
                    case 3:
                        return Content.Load<Texture2D>($"conveer0");
                    case 4:
                        return Content.Load<Texture2D>($"monipulator");
                    case 5:
                        return Content.Load<Texture2D>($"iron_ingot");
                    case 6:
                        return Content.Load<Texture2D>($"iron_inventory");
                    case 7:
                        return Content.Load<Texture2D>($"coper_ingot");
                    case 8:
                        return Content.Load<Texture2D>($"coper_inventory");
                    case 9:
                        return Content.Load<Texture2D>($"coal_inventory");
                    case 10:
                        return Content.Load<Texture2D>($"ElectricPole");
                    case 11:
                        return Content.Load<Texture2D>($"Digger0");
                    case 12:
                        return Content.Load<Texture2D>($"Generator");
                    default:
                        return Content.Load<Texture2D>($"FurnanceOFf0");//electric pole 10; 11 digger

                }
        }

        public void KeyboardIN()
        {
            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.A) ||
               kstate.IsKeyDown(Keys.W) && kstate.IsKeyDown(Keys.D) ||
               kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.A) ||
               kstate.IsKeyDown(Keys.S) && kstate.IsKeyDown(Keys.D)) Data.character.speed = Data.character.halfspeed;


            if (kstate.IsKeyDown(Keys.W) && Collide(0)) Data.character.Move(0);
            if (kstate.IsKeyDown(Keys.A) && Collide(1)) Data.character.Move(1);
            if (kstate.IsKeyDown(Keys.S) && Collide(2)) Data.character.Move(2);
            if (kstate.IsKeyDown(Keys.D) && Collide(3)) Data.character.Move(3);

            if (kstate.IsKeyDown(Keys.F)) GetItem();

            if (kstate.IsKeyDown(Keys.E) && Data.keyE)
            {
                if (Data.activeUIs[1][3] == 1)
                {
                    Data.activeUIs[1][3] = 0;
                    Data.activeUIs[5][3] = 0;
                }
                else
                {
                    Data.activeUIs[1][3] = 1;
                    Data.activeUIs[5][3] = 1;
                }
                Data.keyE = false;
            }
            
            if (kstate.IsKeyUp(Keys.E)) Data.keyE = true;


            if (kstate.IsKeyDown(Keys.D1)) Data.SelectedBar = 0;
            if (kstate.IsKeyDown(Keys.D2)) Data.SelectedBar = 1;
            if (kstate.IsKeyDown(Keys.D3)) Data.SelectedBar = 2;
            if (kstate.IsKeyDown(Keys.D4)) Data.SelectedBar = 3;
            if (kstate.IsKeyDown(Keys.D5)) Data.SelectedBar = 4;
            if (kstate.IsKeyDown(Keys.D6)) Data.SelectedBar = 5;
            if (kstate.IsKeyDown(Keys.D7)) Data.SelectedBar = 6;
            if (kstate.IsKeyDown(Keys.D8)) Data.SelectedBar = 7;
            if (kstate.IsKeyDown(Keys.D9)) Data.SelectedBar = 8;
            if (kstate.IsKeyDown(Keys.D0)) Data.SelectedBar = 9;

            if (kstate.IsKeyDown(Keys.LeftShift)) Data.Shift = true;
            if (kstate.IsKeyUp(Keys.LeftShift)) Data.Shift = false;

            if (kstate.IsKeyDown(Keys.Down)) Data.SelectedRot = 0;
            if (kstate.IsKeyDown(Keys.Left)) Data.SelectedRot = 1;
            if (kstate.IsKeyDown(Keys.Up)) Data.SelectedRot = 2;
            if (kstate.IsKeyDown(Keys.Right)) Data.SelectedRot = 3;
            if (kstate.IsKeyDown(Keys.F3)) F3 = true;
            if (kstate.IsKeyDown(Keys.F2)) F3 = false;

            if (kstate.IsKeyDown(Keys.Escape))
            {
                foreach (double[] x in Data.ObjInf)
                {
                    if (x != null) x[1] = 0;
                }
                for (int i = 1; i < Data.activeUIs.Count; i++)
                {
                    if (Data.activeUIs[i] != null) Data.activeUIs[i][3] = 0;
                }
            }
        }

        #region Collide
        private bool CollideWater(Rectangle RObj)
        {
            scale = (int)(Data.SizeTile * Data.TileScale);
            CorrectBorder();
            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    Rectangle Tile = new Rectangle((int)(x * scale), (int)(y * scale), (int)(scale), (int)(scale));
                    if (RObj.Intersects(Tile) && Data.RenderedMap[x, y].Tag == "water")
                    {
                        return false;
                    }

                }
            }
            return true;
        }

        public void OnConveyor(int dir)
        {
            switch (dir)
            {
                case 0:
                    if (Collide(2, false)) Data.character.Move(2, 2);
                    break;
                case 1:
                    if (Collide(1, false)) Data.character.Move(1, 2);
                    break;
                case 2:
                    if (Collide(0, false)) Data.character.Move(0, 2);
                    break;
                case 3:
                    if (Collide(3, false)) Data.character.Move(3, 2);
                    break;
            }
        }

        private bool Collide(int i, bool conv = true) //Check collision with i direction
        {
            scale = (int)(Data.SizeTile * Data.TileScale);
            Rectangle HBplayer = Data.character.pos;
            switch (i)
            {
                case 0: // -- Up Direction
                    HBplayer = new Rectangle(Data.character.pos.X, Data.character.pos.Y - 7, Data.character.pos.Width, Data.character.pos.Height);
                    break;
                case 1: // -- Left Direction
                    HBplayer = new Rectangle(Data.character.pos.X - 7, Data.character.pos.Y, Data.character.pos.Width, Data.character.pos.Height);
                    break;
                case 2: // -- Down Direction
                    HBplayer = new Rectangle(Data.character.pos.X, Data.character.pos.Y + 7, Data.character.pos.Width, Data.character.pos.Height);
                    break;
                case 3: // -- Right Direction
                    HBplayer = new Rectangle(Data.character.pos.X + 7, Data.character.pos.Y, Data.character.pos.Width, Data.character.pos.Height);
                    break;
                case 4: // -- Without Direction
                    HBplayer = new Rectangle(Data.character.pos.X, Data.character.pos.Y, Data.character.pos.Width, Data.character.pos.Height);
                    break;
            }
            HBplayer.X += 10;
            HBplayer.Width -= 20;
            HBplayer.Y += 40;
            HBplayer.Height -= 40;


            //Check collision with water tile
            CorrectBorder();
            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    Rectangle Tile = new Rectangle((int)(x * scale), (int)(y * scale), (int)(scale), (int)(scale));
                    if (HBplayer.Intersects(Tile) && Data.RenderedMap[x,y].Tag == "water")
                    {
                        return false;
                    }
                    
                }
            }

            scale = (int)(Data.SizeObj * Data.ObjScale);
            CorrectBorder();
            for (int y = DrawingBorder.Y; y < DrawingBorder.Height; y++)
            {
                for (int x = DrawingBorder.X; x < DrawingBorder.Width; x++)
                {
                    Rectangle Obj = new Rectangle((int)(x * scale), (int)(y * scale), (int)(scale), (int)(scale));
                    if (Data.ObjMap[x, y] == null) continue;
                    if (Data.ObjMap[x, y].Tag == "moni") continue;                    
                    if (HBplayer.Intersects(Obj))
                    {
                        if (Data.ObjMap[x, y].Tag == "conveer")
                        {
                            if(conv) OnConveyor((int)Data.ObjInf[x, y][2]);
                            return true;
                        }
                        return false;
                    }


                }
            }
            return true;
        }

        

        private bool Collide(Rectangle HB, bool inversion = false) //Check collision with Rect HB
        {
            Rectangle HBplayer = Data.character.pos;
            HBplayer.X += 10;
            HBplayer.Width -= 20;
            HBplayer.Y += 40;
            HBplayer.Height -= 40;
            if (HBplayer.Intersects(HB) && inversion) return true;
            else if (inversion) return false;
            if (HBplayer.Intersects(HB)) return false;
            else return true;
        }
        #endregion
    }
}