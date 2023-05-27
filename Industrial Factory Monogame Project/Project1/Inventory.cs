using Microsoft.Xna.Framework;



namespace Industrial_Factory
{
    internal class Inventory
    {
        public double[,][] Item;
        public Rectangle[,] Grid;
        public int Object;
        double[] ReplaceObj;

        public double[] Replace
        {
            get
            {
                return ReplaceObj;
            }
            set
            {
                ReplaceObj = value;
            }
        }

        public Inventory(int SelectedBar)
        {
            Object = SelectedBar;
            switch (SelectedBar)
            {
                case -1: 
                    Item = new double[5, 5][];
                    Grid = new Rectangle[5, 5];
                    for (int i = 0; i < 5; i++) // for craft
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Grid[i, j] = new Rectangle(871 + i * 46, 405 + j * 46, 45, 45);
                        }
                    }
                    break;
                case 0:
                    Item = new double[10, 1][];
                    Grid = new Rectangle[10, 1];
                    for (int i = 0; i < 10; i++)//for character hotbar
                    {
                        Grid[i, 0] = new Rectangle(410 + i * 46, 674, 45, 45);
                    }
                    break;
                case 1:
                    Item = new double[10, 3][];
                    Grid = new Rectangle[10, 3];
                    for (int i = 0; i < 10; i++)//for character inventerory
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Grid[i, j] = new Rectangle(410 + i * 46, 497 + j * 46, 45, 45);
                        }
                    }
                    break;
                case 2: //for furnance
                    Item = new double[2, 2][];
                    Grid = new Rectangle[2, 2];
                    Grid[0, 0] = new Rectangle(457, 297, 45, 45);
                    Grid[1, 0] = new Rectangle(777, 297, 45, 45);
                    Grid[0, 1] = new Rectangle(617, 342, 45, 45);
                    Grid[1, 1] = new Rectangle(-20000, -20000, 1, 1);
                    break;
                case 3:
                    Item = new double[10, 3][];
                    Grid = new Rectangle[10, 3];
                    for (int i = 0; i < 10; i++)//for chest
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Grid[i, j] = new Rectangle(410 + i * 46, 297 + j * 46, 45, 45);
                        }
                    }
                    break;
                case 4:
                    Item = new double[10, 3][];
                    Grid = new Rectangle[10, 3];
                    for (int i = 0; i < 10; i++)//for chest
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            Grid[i, j] = new Rectangle(410 + i * 46, 297 + j * 46, 45, 45);
                        }
                    }
                    break;
            }
        }

        public Inventory() { }


        public void GetItemToMouse(int x, int y)
        {
            if (Item[x, y] != null) Item[x, y] = null;
        }

        public void GetHalfToMouse(int x, int y)
        {
            if (Item[x, y] != null)
            {
                int temp = (int)Item[x, y][1];
                Item[x, y][1] = temp / 2;
                if (Item[x, y][1] == 0) Item[x, y] = null;
            }
        }

        public bool GetItemFind(int id, int count = 1, bool onlyCheck = false)
        {
            for (int column = 0; column < Item.GetLength(0); column++)
            {
                for (int line = 0; line < Item.GetLength(1); line++)
                {
                    if (Item[column, line] != null && Item[column, line][0] == id && Item[column, line][1] >= count)
                    {
                        if (!onlyCheck) Item[column, line][1] -= count;
                        if (Item[column, line][1] == 0) Item[column, line] = null;
                        return true;
                    }
                }
            }
            return false;
        }

        public void GetOneItem(int x, int y) //= get one item
        {
            if (Item[x, y] != null)
            {
                Item[x, y][1] -= 1;
                if (Item[x, y][1] == 0) Item[x, y] = null;
            }
        }

        public bool PutItemToInventoryForMouse(int x, int y, int id, int count)
        {
            bool repl = false;
            if (Item[x, y] == null) Item[x, y] = new double[] { id, count };
            else if (Item[x, y][0] == id) Item[x, y][1] += count;
            else
            {
                Replace = Item[x, y];
                Item[x, y] = new double[] { id, count };
                repl = true;
            }

            return repl;
        }
        public bool PutItemToInventory(int x, int y, int id, int count)
        {
            if (Item[x, y] == null)
            {
                Item[x, y] = new double[] { id, count };
                return true;
            }
            else if (Item[x, y][0] == id)
            {
                Item[x, y][1] += count;
                return true;
            }

            return false;
        }

        public bool ExistItem(int x, int y, int id) //allows to put item
        {
            if (Item[x, y] == null) return true;
            else if (Item[x, y][0] == id) return true;
            else return false;

        }

        public bool PutItemToInventory(int id, int count)
        {
            for (int column = 0; column < Item.GetLength(0); column++)
            {
                for (int line = 0; line < Item.GetLength(1); line++)
                {
                    if (Item[column, line] != null && Item[column, line][0] == id)
                    {
                        Item[column, line][1] += count;
                        return true;
                    }
                    if (Item[column, line] == null)
                    {
                        Item[column, line] = new double[] { id, count };
                        return true;
                    }

                }
            }
            return false;

        }

        public bool PutItemToInventory(double[] item)
        {
            for (int column = 0; column < Item.GetLength(0); column++)
            {
                for (int line = 0; line < Item.GetLength(1); line++)
                {
                    if (Item[column, line] != null && Item[column, line][0] == item[0])
                    {
                        Item[column, line][1] += 1;
                        return true;
                    }
                    if (Item[column, line] == null)
                    {
                        Item[column, line] = new double[] { item[0], 1 };
                        return true;
                    }

                }
            }
            return false;

        }


        public double[] GetFirstItem(bool get = false)
        {
            double[] item = null;
            for (int column = 0; column < Item.GetLength(0); column++)
            {
                for (int line = 0; line < Item.GetLength(1); line++)
                {
                    if (Item[column, line] != null)
                    {
                        item = Item[column, line];
                        if (get)
                        {
                            Item[column, line][1] -= 1;
                            if (Item[column,line][1] == 0) Item[column,line] = null;
                        }
                        return item;
                    }
                }
            }
            return item;
        }

    }
}
