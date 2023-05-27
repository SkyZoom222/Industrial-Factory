using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industrial_Factory
{
    internal class Player
    {
        int W;
        int H;
        public float speed = 8;
        public float halfspeed;
        public Rectangle pos;
        public Rectangle playerUI;
        public Texture2D texture;
        public float CposX, CposY;
        public float VPx = 0, VPy = 0;
        public float Px = 0, Py = 0;

        public Player(Texture2D texture, float CposX, float CposY) 
        {
            H = 55;
            W = 45;
            this.texture = texture;
            this.CposX = CposX - W;
            this.CposY = CposY - H;
            pos = new Rectangle(1000, 1000, 45, 55);
            halfspeed = speed * 0.7f;
            
        }

        public void Updatepos(float x,float y)
        {
            H = 55;
            W = 45;
            pos = new Rectangle((int)x - W/2, (int)y - H/2, W, H);
        }


        public void Move(int dir, int newspeed = -1)
        {
            int speed;
            if (newspeed == -1) speed = (int)this.speed;
            else speed = newspeed;
            if (dir == 0 && pos.Y + pos.Height / 2 > Game1.ScreenH / 2 + speed && pos.Y + pos.Height / 2 < Data.Hmap * Data.SizeTile * Data.TileScale - Game1.ScreenH / 2) pos.Y -= speed;
            else if (dir == 0 && pos.Y - speed > 0)
            {
                pos.Y -= speed;
                Py -= speed;
            }


            if (dir == 1 && pos.X + pos.Width / 2 > Game1.ScreenW / 2 + speed && pos.X + pos.Width / 2 < Data.Wmap * Data.SizeTile * Data.TileScale - Game1.ScreenW / 2) pos.X -= speed;
            else if (dir == 1 && pos.X - speed > 0)
            {
                pos.X -= speed;
                Px -= speed;
            }

            if (dir == 2 && pos.Y + pos.Height / 2 < Data.Hmap * Data.SizeTile * Data.TileScale - speed - Game1.ScreenH / 2 && pos.Y + pos.Height / 2 > Game1.ScreenH / 2) pos.Y += speed;
            else if (dir == 2 && pos.Y + speed + pos.Height < Data.Hmap * Data.SizeTile * Data.TileScale)
            {
                pos.Y += speed;
                Py += speed;
            }
            if (dir == 3 && pos.X + pos.Width / 2 < Data.Wmap * Data.SizeTile * Data.TileScale - speed - Game1.ScreenW / 2 && pos.X + pos.Width / 2 > Game1.ScreenW / 2) pos.X += speed;
            else if (dir == 3 && pos.X + speed + pos.Width < Data.Wmap * Data.SizeTile * Data.TileScale)
            {
                pos.X += speed;
                Px += speed;
            }
            VPx = -pos.X + Game1.ScreenW / 2 - pos.Width / 2 + Px;
            VPy = -pos.Y + Game1.ScreenH / 2 - pos.Height / 2 + Py;
        }


    }
}
