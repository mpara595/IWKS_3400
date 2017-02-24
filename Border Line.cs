using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;   //   for Texture2D
using Microsoft.Xna.Framework;  //  for Vector2

namespace IWKS_3400_Lab4
{
    class Border_Line
    {
        public Texture2D texture { get; set; } //  border texture, read-only property
        public Vector2 position { get; set; }  //  border position on screen
        public Vector2 size { get; set; }      //  border size in pixels
        public Vector2 velocity { get; set; }  //  border velocity
        private Vector2 screenSize { get; set; } //  screen size

        public Border_Line(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
