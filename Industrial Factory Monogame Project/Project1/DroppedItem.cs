using Microsoft.Xna.Framework;


namespace Industrial_Factory
{
    internal class DroppedItem
    {
        public int id, speed;
        public Rectangle Item;

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
                case 0: // -- Up Direction
                    HB = new Rectangle(Item.X, Item.Y + 3, Item.Width, Item.Height);
                    break;
                case 1: // -- Left Direction
                    HB = new Rectangle(Item.X - 3, Item.Y, Item.Width, Item.Height);
                    break;
                case 2: // -- Down Direction
                    HB = new Rectangle(Item.X, Item.Y - 3, Item.Width, Item.Height);
                    break;
                case 3: // -- Right Direction
                    HB = new Rectangle(Item.X + 3, Item.Y, Item.Width, Item.Height);
                    break;
            }
            foreach (var item in Data.droppedItems) if (Item != item.Item && HB.Intersects(item.Item))
                {
                    return;
                }
            if (dir == 2) Item.Y -= speed;
            if (dir == 1) Item.X -= speed;
            if (dir == 0) Item.Y += speed;
            if (dir == 3) Item.X += speed;
        }

        public void OnConveyorCheck() //Obj, (int)Data.ObjInf[x, y][2]
        {
            int scale = (int)(Data.SizeObj * Data.ObjScale); //scale for Drawing obj
             
            Point Obj = new Point((Item.X + 20 )/ scale, (Item.Y + 20) / scale);
            if (Data.ObjMap[Obj.X, Obj.Y] != null && Data.ObjMap[Obj.X, Obj.Y].Tag == "conveer")
            {
                Move((int)Data.ObjInf[Obj.X, Obj.Y][2]);
            }
        }

    }
}
