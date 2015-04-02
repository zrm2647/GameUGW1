using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace UGWProject
{
    class DeadlyBlock:Block
    {
        //attributes
        

        //constructor
        public DeadlyBlock(Rectangle blockRectangle, Texture2D deadlyBTexture):base(blockRectangle, deadlyBTexture)
        {

        }

        //kill method
        /// <summary>
        /// The method for switching the player between worlds depending on what world they are
        /// currently in.
        /// </summary>
        /// <param name="player1"> the player object passed in</param> 
        public void Kill(Player player1)
        {
            //stub method. 
            if (player1.RealWorld == false)
            {
                player1.RealWorld = true;
            }
            else if (player1.RealWorld == true)
            {
                player1.RealWorld = false;
            }
        }

    }
}
