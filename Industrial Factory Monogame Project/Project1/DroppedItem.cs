﻿using Microsoft.Xna.Framework;
using System.Drawing.Drawing2D;

namespace Industrial_Factory
{
    internal class DroppedItem
    {
        public int id, speed;
        public Rectangle Item;
        Point ObjOffset = new Point(20);

        public DroppedItem(int id, int x, int y)
        {
            Item = new Rectangle(x, y, 20,20);
            speed = 2;
            this.id = id;
        }

        private void Move(int dir)//Data.character.
        {
            Rectangle HB = Item;
            switch (dir)
            {
                case 0: // -- Down Direction
                    HB = new Rectangle(Item.X, Item.Y + 3, Item.Width, Item.Height);
                    ObjOffset = new Point(20, 0);
                    break;
                case 1: // -- Left Direction
                    HB = new Rectangle(Item.X - 3, Item.Y, Item.Width, Item.Height);
                    ObjOffset = new Point(40, 20);
                    break;
                case 2: // -- Up Direction
                    HB = new Rectangle(Item.X, Item.Y - 3, Item.Width, Item.Height);
                    ObjOffset = new Point(20, 40);
                    break;
                case 3: // -- Right Direction
                    HB = new Rectangle(Item.X + 3, Item.Y, Item.Width, Item.Height);
                    ObjOffset = new Point(0, 20);
                    break;
            }
            foreach (var item in Data.droppedItems) 
                if (Item != item.Item && HB.Intersects(item.Item))
                {
                    return;
                }
            if (dir == 2) Item.Y -= speed;
            if (dir == 1) Item.X -= speed;
            if (dir == 0) Item.Y += speed;
            if (dir == 3) Item.X += speed;
        }

        public void OnConveyorCheck() //Grid, (int)Data.ObjInf[x, y][2]
        {
            int scale = (int)(Data.SizeObj * Data.ObjScale); //scale for Drawing obj
             
            Point Grid = new Point((Item.X + ObjOffset.X)/ scale, (Item.Y + ObjOffset.Y) / scale);
            if (Data.ObjMap[Grid.X, Grid.Y] != null && Data.ObjMap[Grid.X, Grid.Y].Tag == "conveer")
            {
                Move((int)Data.ObjInf[Grid.X, Grid.Y][2]);
            }




        }

    }
}
