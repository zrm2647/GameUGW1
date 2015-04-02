﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
#endregion

namespace UGWProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        StreamReader reader;
        string[] textures;
        Texture2D floor;
        Texture2D sides;
        Texture2D top;
        Rectangle floorrect;
        Rectangle siderectL;
        Rectangle siderectR;
        Rectangle toprect;
        GeneralBlock ceiling;
        GeneralBlock sideL;
        GeneralBlock sideR;
        GeneralBlock ground;
        Enemy enemy1;
        Enemy enemy2;
        Enemy enemy1ghost;
        Enemy enemy2ghost;
        Memories memory;
        int level;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //attributes we need to add:
        //the textures for different enemies/objects/backgrounds will change
        //the paul and the memories will always stay no matter the change in level
        Texture2D paulPhysical;
        Texture2D paulGhost;
        Texture2D memorytexture;
        //the background will change
        Texture2D enemyPhysical1;
        Texture2D enemyPhysical2;
        Texture2D enemyGhost1;
        Texture2D enemyGhost2;
        Texture2D phaseBlockTexture;
        Texture2D deadlyObjs;
        Texture2D backGround;
        Texture2D moveBlockTexture;
        Texture2D basicBlock;
        //this clss will be changed as we get more things done
        //these are the objects
        Player paulPlayer;
        //most of the classes will have methods and attributes that you can call
        //like the player.HasJumped will be called and set to false when the player 
        //is interacting with the ground
        Rectangle paulRect;

        //setting up input
        KeyboardState kboardstate;//getting the keyboard state;
        KeyboardState prevKeyPressed; //takes the previous key that was pressed
        private bool hasJumped; //will set it so that the player can not constantly jump
        private Vector2 velocity;//the velcotiy of the player jumping/falling
        protected Vector2 playerPos; //the position in relation to the rectangle so it can jump;
        //enumerator
        enum PhysicalState { PaulFaceRight, PaulFaceLeft, PaulWalkRight, PaulWalkLeft, PaulJumpRight, PaulJumpLeft, PaulPushLeft, PaulPushRight };
        PhysicalState paulPCurrent = PhysicalState.PaulFaceRight;//default
        //sprite in the ghost state will only be one state, so there does not need to be an enum for it.

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
            level = 1;

            reader = new StreamReader("Textures.txt");
         

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            //each line represents each level
            //[0] floor, [1] side borders, [2] top border,  [3]  enemy1ghost, [4]  enemy 2ghost,[5] enemy1phys, [6] enemy2phys
            //[7] float1, [8] float2, [9] moving block, [10] transblock ghost [11] transblock physical
            if (level > 1)
            {
                string[] lines = reader.ReadToEnd().Split(new char[] { '\n' });
                textures = lines[level - 1].Split(',');

                for (int i = 0; i < textures.Length; i++)
                {
                    textures[i] = textures[i].Replace("\r", "");
                }
            }
            else
            {
                textures = reader.ReadLine().Split(',');
            }


            reader.Close();
            playerPos = new Vector2(300, 300);
            toprect = new Rectangle(41, 0, 942, 40);
            siderectL = new Rectangle(0, 0, 40, 942);
            siderectR = new Rectangle(983, 0, 40, 942);
            floorrect = new Rectangle(41, 730, 942, 40);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here


            paulPhysical = Content.Load<Texture2D>("paulstand1.png");
            floor = Content.Load<Texture2D>(textures[0]);
            sides = Content.Load<Texture2D>(textures[1]);
            top = Content.Load<Texture2D>(textures[2]);
            enemyGhost1 = Content.Load<Texture2D>(textures[3]);
            enemyGhost2 = Content.Load<Texture2D>(textures[4]);
            enemyPhysical1 = Content.Load<Texture2D>(textures[5]);
            enemyPhysical2 = Content.Load<Texture2D>(textures[6]);
            phaseBlockTexture = Content.Load<Texture2D>(textures[10]);
            moveBlockTexture = Content.Load<Texture2D>(textures[9]);
            paulRect = new Rectangle(300, 300, paulPhysical.Width, paulPhysical.Height);
            paulPlayer = new Player(paulRect, paulPhysical, playerPos, false);
            ceiling = new GeneralBlock(toprect, top);
            sideL = new GeneralBlock(siderectL, sides);
            sideR = new GeneralBlock(siderectR, sides);
            ground = new GeneralBlock(floorrect, floor);
        }

        /// <summary>
        /// Player.Move method has been added here to make it easier with the enum/state machine
        /// </summary>
        protected void ProcessInput()
        {
            //Add the player.Move here!
            //going to need a collision detection so that if the player is colliding with the ground then
            // (player object).HasJumped = false so that it detects that player has landed on the ground and
            //can jump again
            //the key controls change depending on if paul is dead or alive
            //control handling

            //THIS STATE MACHINE IS WEIRD! NEED TO FIX IT!

            kboardstate = Keyboard.GetState();
            if (paulPlayer.IsDead == false)
            {

                playerPos += velocity;
                paulPlayer.ObjRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, paulPlayer.ObjRect.Width, paulPlayer.ObjRect.Height);
                //this.ObjRect = new Rectangle( ) 
                if (kboardstate.IsKeyDown(Keys.Escape))
                {
                    //pause menu
                }
                if (kboardstate.IsKeyDown(Keys.A) && prevKeyPressed.IsKeyDown(Keys.A))//IF going left and last state was facing left
                {

                    playerPos.X -= paulPlayer.MoveSpeed;
                    paulPCurrent = PhysicalState.PaulWalkLeft;
                }
                if (kboardstate.IsKeyUp(Keys.A) && prevKeyPressed.IsKeyDown(Keys.D))
                {
                    paulPCurrent = PhysicalState.PaulFaceLeft;
                }
                if (kboardstate.IsKeyDown(Keys.F) && prevKeyPressed.IsKeyDown(Keys.D))
                {
                    //pushing/pulling the block from the right side.
                    playerPos.X += paulPlayer.SpeedWithBlock;
                    paulPCurrent = PhysicalState.PaulPushRight;
                }
                if (kboardstate.IsKeyDown(Keys.D) && prevKeyPressed.IsKeyDown(Keys.D))
                {
                    playerPos.X += paulPlayer.MoveSpeed;
                    paulPCurrent = PhysicalState.PaulWalkRight;
                }
                if (kboardstate.IsKeyDown(Keys.F) && prevKeyPressed.IsKeyDown(Keys.A))
                {
                    //pushing/pulling from the left side of the block
                    playerPos.X -= paulPlayer.SpeedWithBlock;
                    paulPCurrent = PhysicalState.PaulPushLeft;
                }
                //Gravity and jumping v
                if (kboardstate.IsKeyDown(Keys.Space) && hasJumped == false)
                {
                    //jumping  
                    playerPos.Y += 7f;
                    velocity.Y += -5f;
                    hasJumped = true;
                    playerPos += velocity;
                    if (kboardstate.IsKeyDown(Keys.A))
                    {
                        paulPCurrent = PhysicalState.PaulJumpLeft;
                    }
                    if (kboardstate.IsKeyDown(Keys.D))
                    {
                        paulPCurrent = PhysicalState.PaulJumpRight;
                    }
                }
                if (hasJumped == true)
                {
                    //gravity
                    float i = 1;
                    velocity.Y += 0.192f * i;
                    playerPos += velocity;
                }
                if (hasJumped == false)
                {
                    velocity.Y = 0f;
                    //need to make hasjumped = false in the collision method
                    paulPlayer.ObjRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, paulPlayer.ObjRect.Width, paulPlayer.ObjRect.Height);
                    playerPos = new Vector2(paulPlayer.ObjRect.X, paulPlayer.ObjRect.Y);
                    if (paulPCurrent == PhysicalState.PaulJumpRight)
                    {
                        paulPCurrent = PhysicalState.PaulFaceRight;
                    }
                    if (paulPCurrent == PhysicalState.PaulJumpLeft)
                    {
                        paulPCurrent = PhysicalState.PaulPushLeft;
                    }

                }
                paulPlayer.ObjRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, paulPlayer.ObjRect.Width, paulPlayer.ObjRect.Height);

                prevKeyPressed = kboardstate;
            }
            else if (paulPlayer.IsDead == true)
            {
                paulPlayer.ObjRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, paulPlayer.ObjRect.Width, paulPlayer.ObjRect.Height);
                if (kboardstate.IsKeyDown(Keys.Escape))
                {
                    //pause menu
                }
                if (kboardstate.IsKeyDown(Keys.A))
                {

                    playerPos.X -= paulPlayer.MoveSpeed;
                }
                if (kboardstate.IsKeyDown(Keys.D))
                {
                    playerPos.X += paulPlayer.MoveSpeed;
                }
                if (kboardstate.IsKeyDown(Keys.W))
                {
                    playerPos.Y -= paulPlayer.MoveSpeed;
                }
                if (kboardstate.IsKeyDown(Keys.S))
                {
                    playerPos.Y += paulPlayer.MoveSpeed;
                }
                paulPlayer.ObjRect = new Rectangle((int)playerPos.X, (int)playerPos.Y, paulPlayer.ObjRect.Width, paulPlayer.ObjRect.Height);

                prevKeyPressed = kboardstate;
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //Exit();

            // TODO: Add your update logic here
            ProcessInput();
            //the enemy classes .Move() method will go in here. 
            //there will also need to be a collision that changes the direction of the enemy hits an object
            //or is about to fall  off the edge.
            //the .memsAllCollected will be in here. It will constantly be checking to see if the player has collected all the memories

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(ground.GameTexture, ground.ObjRect, Color.White);
            spriteBatch.Draw(sideL.GameTexture, sideL.ObjRect, Color.White);
            spriteBatch.Draw(sideR.GameTexture, sideR.ObjRect, Color.White);
            spriteBatch.Draw(ceiling.GameTexture, ceiling.ObjRect, Color.White);
            spriteBatch.Draw(paulPlayer.GameTexture, paulPlayer.ObjRect, Color.White);


            if(paulPlayer.ObjRect.Bottom > ground.ObjRect.Y)
            {
                Rectangle temp = paulPlayer.ObjRect;
                temp.Y = ground.ObjRect.Top + paulPlayer.ObjRect.Height;
                paulPlayer.ObjRect = temp;
                hasJumped = false;
            }



            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}