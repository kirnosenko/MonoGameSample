using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Model model;
		Texture texture;
		Effect modelEffect;
		double angle = 0;
		Vector3 lightPoint = Vector3.Zero;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
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
			model = Content.Load<Model>("Statue");
			texture = Content.Load<Texture2D>("Bronze");
			modelEffect = Content.Load<Effect>("Model");
			modelEffect.Parameters["world"].SetValue(Matrix.CreateFromYawPitchRoll(-1,0,0));
			modelEffect.Parameters["projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(
				MathHelper.ToRadians(90),
				graphics.GraphicsDevice.Viewport.AspectRatio,
				0.01f,
				100.0f
			));
			modelEffect.Parameters["view"].SetValue(Matrix.CreateLookAt(
				new Vector3(15, 20, 15),
				new Vector3(0, 25, 0),
				Vector3.Up
			));
			modelEffect.Parameters["colorMap"].SetValue(texture);

			graphics.GraphicsDevice.SamplerStates[0] = new SamplerState()
			{
				AddressU = TextureAddressMode.Clamp,
				AddressV = TextureAddressMode.Clamp,
				Filter = TextureFilter.Anisotropic,
				MaxAnisotropy = 8
			};
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
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
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			angle += (Math.PI / 3) * gameTime.ElapsedGameTime.TotalSeconds;
			lightPoint = new Vector3(
				(float)Math.Cos(angle) * 50,
				50,
				(float)Math.Sin(angle) * 50
			);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			modelEffect.Parameters["lightPoint"].SetValue(lightPoint);
			modelEffect.CurrentTechnique = modelEffect.Techniques["Model"];
			modelEffect.CurrentTechnique.Passes[0].Apply();
			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					graphics.GraphicsDevice.Indices = meshPart.IndexBuffer;
					graphics.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer);

					graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.NumVertices, meshPart.StartIndex, meshPart.PrimitiveCount);
				}
			}
			
			base.Draw(gameTime);
		}
	}
}
