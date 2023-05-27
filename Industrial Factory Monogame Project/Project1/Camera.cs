using Microsoft.Xna.Framework;

namespace Industrial_Factory
{
    internal class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Player target)
        {
            var position = Matrix.CreateTranslation(
                -target.pos.Location.X - (target.pos.Size.X / 2) + target.Px,
                -target.pos.Location.Y - (target.pos.Size.Y / 2) + target.Py,
                0);

            var offset = Matrix.CreateTranslation(
                Game1.ScreenW / 2,
                Game1.ScreenH / 2,
                0);

            Transform = position * offset;
        }
    }
}
