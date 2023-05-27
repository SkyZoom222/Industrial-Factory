using Microsoft.Xna.Framework;

namespace Industrial_Factory
{
    internal class Object
    {
        int id;
        int last_tick = - 100;
        int tick;
        public int x,y; //grid x y
        Inventory inventory;
        public bool ProgressBarDraw;
        public int Progress;
        Point obj1, obj2; // for monipulator
        int obj1Type, obj2Type; // for monipulator
        Rectangle GettingRectangle; // for monipulator

        bool electro = false;
        public Object(int id, Inventory inventrory, int x, int y)
        {
            this.id = id;
            this.inventory = inventrory;
            this.x = x;
            this.y = y;
        }

        private void Init() // 0 down 1 left 2 up 3 right
        {
            obj1 = new Point(x, y);
            obj2 = new Point(x, y);
            int scale = (int)(Data.SizeObj * Data.ObjScale);
            switch (Data.ObjInf[x, y][2])
            {
                case 0:
                    obj1.Y--;
                    obj2.Y++;
                    GettingRectangle = new Rectangle(obj1.X * scale, obj1.Y * scale, scale, scale + scale / 5);
                    break;
                case 1:
                    obj1.X++;
                    obj2.X--;
                    GettingRectangle = new Rectangle(obj1.X * scale - scale / 5, obj1.Y * scale, scale + scale / 5, scale);
                    break;
                case 2:
                    obj1.Y++;
                    obj2.Y--;
                    GettingRectangle = new Rectangle(obj1.X * scale, obj1.Y * scale - scale / 5, scale, scale + scale / 5);
                    break;
                case 3:
                    obj1.X--;
                    obj2.X++;
                    GettingRectangle = new Rectangle(obj1.X * scale, obj1.Y * scale, scale + scale / 5, scale);
                    break;
            }
            if (Data.ObjInf[obj1.X, obj1.Y] != null) obj1Type = (int)Data.ObjInf[obj1.X, obj1.Y][0];
            else obj1Type = -1;
            if (Data.ObjInf[obj2.X, obj2.Y] != null) obj2Type = (int)Data.ObjInf[obj2.X, obj2.Y][0];
            else obj2Type = -1;
        }

        public void Update(int tick)
        {
            this.tick = tick;
            ProgressBarDraw = false;
            switch (id)
            {
                case 0:
                    CheckElectricity();
                    if (electro) Furnance();
                    break;

                case 4:
                    CheckElectricity();
                    if (electro) Manipulator();
                    break;
                case 10:
                    ElectricPole();
                    break;

                case 11:
                    CheckElectricity();
                    if (electro) Digger();
                    break;
            }


        }

        private void ProgressBar(int tick, int MaxValue = 120)
        {
            int value = tick - last_tick;
            ProgressBarDraw = true;
            Progress = (int)((double)value / MaxValue * 100);
        }

        private void Furnance()
        {
            if (tick - last_tick > 120)
            {
                if (inventory.Item[0, 0] == null || inventory.Item[0, 1] == null) return;
                if (inventory.Item[0, 1][0] == 9 && inventory.Item[0, 0][0] == 6 && inventory.PutItemToInventory(1, 0, 5, 1))
                {
                    last_tick = tick;
                    inventory.Item[0, 0][1]--;
                    inventory.Item[0, 1][1]--;
                    if (inventory.Item[0, 0][1] == 0) inventory.Item[0, 0] = null;
                    if (inventory.Item[0, 1][1] == 0) inventory.Item[0, 1] = null;
                }
                else if (inventory.Item[0, 1][0] == 9 && inventory.Item[0, 0][0] == 8 && inventory.PutItemToInventory(1, 0, 7, 1))
                {
                    last_tick = tick;
                    inventory.Item[0, 0][1]--;
                    inventory.Item[0, 1][1]--;
                    if (inventory.Item[0, 0][1] == 0) inventory.Item[0, 0] = null;
                    if (inventory.Item[0, 1][1] == 0) inventory.Item[0, 1] = null;
                }
            }
            else ProgressBar(tick);
        }

        private void Manipulator()
        {
            Init();
            if (tick - last_tick > 60 && obj1Type != -1)
            {
                if ((obj1Type == 1 || obj1Type == 2) && obj2Type == 0) //chest to furnance
                {
                    if (Data.ObjInventory[obj1.X, obj1.Y].GetItemFind(6))
                    {
                        if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(0, 0, 6, 1))
                        {
                            last_tick = tick;
                        }
                        else
                        {
                            Data.ObjInventory[obj1.X, obj1.Y].PutItemToInventory(6, 1);
                            return;
                        }
                    }

                    if (Data.ObjInventory[obj1.X, obj1.Y].GetItemFind(8))
                    {
                        if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(0, 0, 8, 1))
                        {
                            last_tick = tick;
                        }
                        else
                        {
                            Data.ObjInventory[obj1.X, obj1.Y].PutItemToInventory(8, 1);
                            return;
                        }
                    }


                    if (Data.ObjInventory[obj1.X, obj1.Y].GetItemFind(9))
                    {
                        if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(0, 1, 9, 1))
                        {
                            last_tick = tick;
                        }
                        else
                        {
                            Data.ObjInventory[obj1.X, obj1.Y].PutItemToInventory(9, 1);
                            return;
                        }
                    }
                }
                else if (obj1Type == 0 && (obj2Type == 1 || obj2Type == 2)) //furnance to chest
                {
                    if (Data.ObjInventory[obj1.X, obj1.Y].Item[1, 0] != null)
                    {
                        if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(Data.ObjInventory[obj1.X, obj1.Y].Item[1, 0]))
                        {
                            last_tick = tick;
                            Data.ObjInventory[obj1.X, obj1.Y].GetOneItem(1, 0);
                        }
                    }
                }
                else if (obj1Type == 0 && obj2Type == 3) //furnance to conveyor
                {
                    if (Data.ObjInventory[obj1.X, obj1.Y].Item[1, 0] != null)
                    {
                        int scaleTile = (int)(Data.SizeTile * Data.TileScale);
                        int scaleObj = (int)(Data.SizeObj * Data.ObjScale);
                        Point p = new Point(obj2.X * scaleObj, obj2.Y * scaleObj);
                        if (Data.RenderedMap[(int)(p.X - p.X % scaleTile) / scaleTile, (int)(p.Y - p.Y % scaleTile) / scaleTile].Tag == "water") return;
                        Point itempoint = new Point(p.X + scaleObj / 2, p.Y + scaleObj / 2);
                        Rectangle itemrect = new Rectangle(itempoint, new Point(20));
                        foreach (var drop in Data.droppedItems) if (itemrect.Intersects(drop.Item))
                            {
                                return;
                            }
                        Data.droppedItems.Add(new DroppedItem((int)Data.ObjInventory[obj1.X, obj1.Y].Item[1, 0][0], p.X + scaleObj / 2, p.Y + scaleObj / 2));
                        Data.ObjInventory[obj1.X, obj1.Y].GetOneItem(1, 0);
                        last_tick = tick;
                    }
                }
                else if (obj1Type == 3 && obj2Type == 0) //conveyor to furnance
                {

                    foreach (var drop in Data.droppedItems)
                    {
                        if (GettingRectangle.Intersects(drop.Item))
                        {

                            if (drop.id == 6)
                            {
                                if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(0, 0, 6, 1))
                                {
                                    last_tick = tick;
                                    Data.droppedItems.Remove(drop);
                                    return;
                                }
                                else return;
                            }

                            if (drop.id == 8)
                            {
                                if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(0, 0, 8, 1))
                                {
                                    last_tick = tick;
                                    Data.droppedItems.Remove(drop);
                                    return;
                                }
                                else return;
                            }


                            if (drop.id == 9)
                            {
                                if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(0, 1, 9, 1))
                                {
                                    last_tick = tick;
                                    Data.droppedItems.Remove(drop);
                                    return;
                                }
                                else return;
                            }

                        }
                    }
                }
                else if ((obj1Type == 1 || obj1Type == 2) && obj2Type == 3) //chest to conveyor
                {
                    double[] item = Data.ObjInventory[obj1.X, obj1.Y].GetFirstItem();
                    if (item != null)
                    {
                        int scaleTile = (int)(Data.SizeTile * Data.TileScale);
                        int scaleObj = (int)(Data.SizeObj * Data.ObjScale);
                        Point p = new Point(obj2.X * scaleObj, obj2.Y * scaleObj);
                        if (Data.RenderedMap[(int)(p.X - p.X % scaleTile) / scaleTile, (int)(p.Y - p.Y % scaleTile) / scaleTile].Tag == "water") return;
                        Point itempoint = new Point(p.X + scaleObj / 2, p.Y + scaleObj / 2);
                        Rectangle itemrect = new Rectangle(itempoint, new Point(20));
                        foreach (var drop in Data.droppedItems) if (itemrect.Intersects(drop.Item))
                            {
                                return;
                            }
                        Data.droppedItems.Add(new DroppedItem((int)item[0], p.X + scaleObj / 2, p.Y + scaleObj / 2));
                        Data.ObjInventory[obj1.X, obj1.Y].GetItemFind((int)item[0]);
                        last_tick = tick;
                    }
                }
                else if (obj1Type == 3 && (obj2Type == 1 || obj2Type == 2)) //conveyor to chest
                {
                    foreach (var drop in Data.droppedItems)
                    {
                        if (GettingRectangle.Intersects(drop.Item))
                        {
                            if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory(drop.id, 1))
                            {
                                last_tick = tick;
                                Data.droppedItems.Remove(drop);
                                return;
                            }
                        }
                    }
                }
                else if ((obj2Type == 1 || obj2Type == 2) && (obj2Type == 1 || obj2Type == 2)) //chest to chest
                {
                    double[] item = Data.ObjInventory[obj1.X, obj1.Y].GetFirstItem();
                    if (item == null) return;
                    if (Data.ObjInventory[obj2.X, obj2.Y].PutItemToInventory((int)item[0], 1))
                    {
                        last_tick = tick;
                        Data.ObjInventory[obj1.X, obj1.Y].GetFirstItem(true);
                        return;
                    }
                }

            }
        }

        private void ElectricPole()
        {
            electro = true;
        }

        public void CheckElectricity()
        {
            electro = false;
            for (int x = this.x - 2; x < this.x + 3; x++)
            {
                for (int y = this.y - 2; y < this.y + 3; y++)
                {
                    if (Data.Objects[x, y] != null && Data.Objects[x, y].id == 10 && Data.Objects[x, y].electro) electro = true;
                }
            }
        }

        public void Digger()
        {
            Init();
            if (tick - last_tick > 60)
            {
                int scaleTile = (int)(Data.SizeTile * Data.TileScale);
                int scaleObj = (int)(Data.SizeObj * Data.ObjScale);
                Point p = new Point(obj2.X * scaleObj, obj2.Y * scaleObj);
#pragma warning disable CS0252 // Возможно, использовано непреднамеренное сравнение ссылок: для левой стороны требуется приведение
                if (Data.RenderedMap[(int)(p.X - p.X % scaleTile) / scaleTile, (int)(p.Y - p.Y % scaleTile) / scaleTile].Tag == "water") return;
#pragma warning restore CS0252 // Возможно, использовано непреднамеренное сравнение ссылок: для левой стороны требуется приведение
                Point itempoint = new Point(p.X + scaleObj / 2, p.Y + scaleObj / 2);
                Rectangle itemrect = new Rectangle(itempoint, new Point(20));
                foreach (var drop in Data.droppedItems) 
                    if (itemrect.Intersects(drop.Item))
                    {
                        return;
                    }
                if (Data.CoalOreCountMap[x,y] > 0)
                {
                    Data.CoalOreCountMap[x, y] -= 1;
                    Data.droppedItems.Add(new DroppedItem(9, p.X + scaleObj / 2, p.Y + scaleObj / 2));
                }
                else if (Data.CopperOreCountMap[x, y] > 0)
                {
                    Data.CopperOreCountMap[x, y] -= 1;
                    Data.droppedItems.Add(new DroppedItem(8, p.X + scaleObj / 2, p.Y + scaleObj / 2));
                }
                else if (Data.IronOreCountMap[x, y] > 0)
                {
                    Data.IronOreCountMap[x, y] -= 1;
                    Data.droppedItems.Add(new DroppedItem(6, p.X + scaleObj / 2, p.Y + scaleObj / 2));
                }
                last_tick = tick;
            }
        }
    }
}
