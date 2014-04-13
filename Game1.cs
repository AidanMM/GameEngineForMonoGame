#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MonoGameEngine.Engine_Components;
using MonoGameEngine.Levels;
using MonoGameEngine.Game_Objects;

#endregion

namespace MonoGameEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {

        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Global Variables
        /// <summary>
        /// The overarching object handler for the game. Manages all game objects
        /// </summary>
        public static ObjectHandler OBJECT_HANDLER = new ObjectHandler();

        /// <summary>
        /// This is a safety texture so that all game objects have at least one texture
        /// </summary>
        public static Texture2D DEFAULT_TEXTURE;

        /// <summary>
        /// Conatains all basic textures in the game. All identified using the name of the image
        /// </summary>
        public static Dictionary<string, Texture2D> IMAGE_DICTIONARY;

        /// <summary>
        /// The animation dictionary to access any of the animations that have been added to the game
        /// </summary>
        public static Dictionary<string, List<Texture2D>> ANIMATION_DICTIONARY;

        public static Dictionary<string, List<List<Texture2D>>> ANIMATION_SET_DICTIONARY;

        /// <summary>
        /// This is the displacement of the camera (= displacement of character)
        /// </summary>
        public static Vector2 CAMERA_DISPLACEMENT;

        /// <summary>
        /// The current keyboard state used by the game
        /// </summary>
        public static KeyboardState KBState = new KeyboardState();

        /// <summary>
        /// The keyboard state that existed one frame before the current keyboard state
        /// </summary>
        public static KeyboardState oldKBstate = new KeyboardState();

        /// <summary>
        /// The random object to be used by the entire game. This is to prevent multiple instances of random being created
        /// at the same time
        /// </summary>
        public static Random RANDOM = new Random();

        /// <summary>
        /// A global, counting timer to be used through out the game so that multiple do not need to be instantiated
        /// </summary>
        public static int TIMER;

        /// <summary>
        /// The Default font used by the Game Engine
        /// </summary>
        public SpriteFont DEFUALT_SPRITEFONT;


        /// <summary>
        /// This basic effect is for all primitives
        /// </summary>
        public static BasicEffect basicEffect;

        /// <summary>
        /// Hides, or shows the debug mode command string
        /// </summary>
        public bool debugMode;

        /// <summary>
        /// The debug command prompt for the game
        /// </summary>
        private CommandInput debugPrompt;

        /// <summary>
        /// This bool will toggle whether or not the object handler updates all of its objects
        /// </summary>
        public static bool pauseObjectUpdate;

        public static string TITLE_STRING = "Project Greco";

        /// <summary>
        /// The mouse state for the game to use
        /// </summary>
        public static MouseState mouseState;

        /// <summary>
        /// The previous mouse state of the last frame
        /// </summary>
        public static MouseState prevMouseState;


        #endregion

        #region Some MonoGame Stuff, feel free to look at it!

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "My Game";

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;

            // TODO: Add your initialization logic here
            IMAGE_DICTIONARY = new Dictionary<string, Texture2D>();
            ANIMATION_DICTIONARY = new Dictionary<string, List<Texture2D>>();
            ANIMATION_SET_DICTIONARY = new Dictionary<string, List<List<Texture2D>>>();
            CAMERA_DISPLACEMENT = new Vector2(0, 0);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane

            IsMouseVisible = true;
            debugMode = false;
            pauseObjectUpdate = false;
            debugPrompt = new CommandInput();
            TIMER = 0;

            base.Initialize();
        }
        #endregion

        #region Load your images, content, and animations here! Also where the initial level is set!
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Make sure all content is set to copy always and to be compiled as content


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load in the initial game font
            DEFUALT_SPRITEFONT = Content.Load<SpriteFont>("TempGameFont");

            #region LoadTexturesHere

            //To load textures and add them to the Image Dictionary try:
            //IMAGE_DICTIONARY.Add("NameKey", Content.Load<Texture2D>{"FileName"));
            //The file name should have no file extension
            IMAGE_DICTIONARY.Add("default", Content.Load<Texture2D>("BlueBeat"));
            DEFAULT_TEXTURE = Content.Load<Texture2D>("BlueBeat");

            #endregion



            #region Load Animations Here
            //To load animations into the game and add them to the Animation_Dictionary, you can add each one by hand or try
            //ANIMATION_DICTIONARY.Add("NameKey", A_CreateAnimation("FileName1", "FileName2", "FileName3", "FileName4", "FileName4"));
            //Any ammount of paramaters can be used, leave out file extensions
            ANIMATION_DICTIONARY.Add("default", A_CreateAnimation("BlueBeat"));
            #endregion

            #region Load Animation sets here
            //Call A_CreateAnimationSet and pass in the animations you would like to use
            ANIMATION_SET_DICTIONARY.Add("default" ,A_CreateAnimationSet(ANIMATION_DICTIONARY["default"]));
            #endregion

            OBJECT_HANDLER.ChangeState(new Level1());




        }
        #endregion

        #region Unloading Content function
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region The Core Update function for MonoGame, this is where Object Handler and Debug Mode gets updated
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KBState = Keyboard.GetState();
            mouseState = Mouse.GetState();
            TIMER++;

            if (pauseObjectUpdate == false)
                OBJECT_HANDLER.Update();

            if (debugMode == true)
            {
                debugPrompt.Update();
                this.Window.Title = TITLE_STRING;
            }

            if (KBState.IsKeyDown(Keys.LeftAlt) && KBState.IsKeyDown(Keys.OemTilde) && oldKBstate.IsKeyUp(Keys.OemTilde))
            {
                if (debugMode == true)
                    debugMode = false;
                else
                    debugMode = true;
            }

            oldKBstate = KBState;
            prevMouseState = mouseState;

            base.Update(gameTime);
        }
        #endregion

        #region The Main draw function for the game, this is where the Object Handler Draws all of the functions
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();


            //This line of code is for primitives
            basicEffect.CurrentTechnique.Passes[0].Apply();
            OBJECT_HANDLER.Draw(spriteBatch);

            if (debugMode == true)
            {
                //spriteBatch.DrawString(spFont1, commandString, new Vector2(0, 0), Color.White);
                spriteBatch.DrawString(DEFUALT_SPRITEFONT, debugPrompt.commandString, new Vector2(0, 0), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        #region Texture,animation, and animation set creation functions
        /// <summary>
        /// Creates textures out of the handed in strings, and then takes those textures and stores them as an animation list.  The handed in strings are also added to the static list of textures
        /// </summary>
        /// <param name="numbers">Any number of strings for pngs. If one of them does not exist, it will not be added to the list</param>
        /// <returns>A list with all of the textures added in the paramaters if they existed</returns>
        protected List<Texture2D> A_CreateAnimation(params string[] textures)
        {
            List<Texture2D> returnList = new List<Texture2D>();
            Texture2D temp = DEFAULT_TEXTURE;
            for (int i = 0; i < textures.Length; i++)
            {

                //Try and see if the image has already been loaded in to the project
                try
                {
                    temp = IMAGE_DICTIONARY[textures[i]];
                    returnList.Add(temp);
                }
                //If it hasn't been. Try adding it to the dictionary
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //try to add it to the image dictionary if it does not exist
                    try
                    {

                        temp = Content.Load<Texture2D>(textures[i]);
                        IMAGE_DICTIONARY.Add(textures[i], temp);
                        returnList.Add(temp);
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine(e.Message + "Name does not lead correspond to a texture.");
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Creates an animation set given the animations passed in
        /// </summary>
        /// <param name="animationsArray">animations to be used for the set</param>
        /// <returns>The animation set created from the animations passed in</returns>
        public static List<List<Texture2D>> A_CreateAnimationSet(params List<Texture2D>[] animationsArray)
        {
            List<List<Texture2D>> animationsList = new List<List<Texture2D>>();

            for (int i = 0; i < animationsArray.Length; i++)
            {
                if (animationsArray[i] != null)
                {
                    animationsList.Add(animationsArray[i]);

                }
            }
            return animationsList;
        }




    }
        #endregion
}
