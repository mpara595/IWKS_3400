using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;   //   for Texture2D
using Microsoft.Xna.Framework;  //  for Vector2


namespace IWKS_3400_Lab4
{
    class Block
    {
        public Texture2D texture { get; set; } //  block texture, read-only property
        public Vector2 position { get; set; }  //  block position on screen
        public Vector2 size { get; set; }      //  block size in pixels
        public Vector2 velocity { get; set; }  //  block velocity
        private Vector2 screenSize { get; set; } //  screen size
        public char reverseKey { get; set; }     //the character that determines which key call the reverse function
        public bool isDown { get; set; }  //a bool value to check if the block is down or not

        public Block(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
            char reverseKey;
            isDown = false; //blocks always start off screen
            
        } 
        public void move(Ball ball)
        {
                //go down until position + size.Y hits the bottom of the screen
                do
            {
                position += new Vector2(0f, -12f);
            } while(this.position.Y + this.size.Y < this.screenSize.Y);

        }
        public void reverse()
        {
            do
            {  //go up until your position + size reaches the top of the screen
                position += new Vector2(0f, 12f);
            } while (this.position.Y + this.size.Y != this.screenSize.Y);
        }
        public void grantKey(char x)
        {
            reverseKey = x;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
