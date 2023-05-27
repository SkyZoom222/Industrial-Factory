using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Industrial_Factory
{
    static class Data
    {
        //fields
        public static Random random; //random

        public static Player character; //player

        public static MapGenerator noise; //map with noises

        public static Texture2D poleBG;

        public static Texture2D[,] RenderedMap; //rendered tile map
        public static Texture2D[,] ObjMap; //object map
        public static Texture2D[,] IronOreMap;
        public static int[,] IronOreCountMap;
        public static Texture2D[,] CopperOreMap;
        public static int[,] CopperOreCountMap;
        public static Texture2D[,] CoalOreMap;
        public static int[,] CoalOreCountMap;
        public static double[,][] ObjInf; //info about object

        public static int Hmap, Wmap; // Width and Heigth of map

        public static int SizeTile;
        public static double TileScale; // Size of tiles and scale for tiles

        public static int SizeObj; // Size of Obj and scale
        public static double ObjScale;

        public static int countObjInTile; //Count Objects in tile

        public static Texture2D[] mouse2D; //Texture of selected cell for mouse

        public static int SelectedRot;
        public static int SelectedBar;

        public static Texture2D[] hotbars;
        public static Inventory hotbar;
        public static Inventory invent;


        public static List<double[]> activeUIs;
        public static Texture2D[] UItexture;
        public static int xUI, yUI;
        public static Inventory[,] ObjInventory;

        public static Texture2D[] ProgressBar;

        public static bool MouseHaveObj;
        public static bool Shift;

        public static List<DroppedItem> droppedItems;

        public static bool ProgressBarDraw;
        public static int Progress;

        public static int CoolDown_ore;

        public static Object[,] Objects;

        public static Inventory craft;

        public static bool keyE;


        //fields
    }
}
