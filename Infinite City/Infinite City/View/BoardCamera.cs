using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InfiniteCity.View
{
    internal class Camera
    {
        /// <summary>
        ///   Initialize the camera. Camera resolution will be the same as the
        ///   GraphicsDeviceManager's resolution
        /// </summary>
        /// <param name = "graphics">The game's GraphicsDeviceManager</param>
        public Camera(GraphicsDeviceManager graphics)
        {
            ResolutionHeight = graphics.PreferredBackBufferHeight;
            ResolutionWidth = graphics.PreferredBackBufferWidth;

            Zoom = 1;
            Position = Vector2.Zero;
        }

        public Vector2 Position { get; set; }

        public int ResolutionHeight { get; set; }
        public int ResolutionWidth { get; set; }
        public float Zoom { get; set; }

        /// <summary>
        ///   Given a point in the screen coordinates, it returns the coordinates
        ///   that it maps to in gamespace, given the camera's position.
        ///   position
        /// </summary>
        /// <param name = "screenCoordinates">A point in screen coordinates that need to be converted to game coordinates</param>
        /// <returns></returns>
        public Vector2 GetGamespaceCoordinates(Vector2 screenCoordinates)
        {
            float gamespaceX = (screenCoordinates.X-ResolutionWidth*0.5f+(ResolutionWidth*0.5f+Position.X)*(float)Math.Pow(Zoom, 3))/
                               (float)Math.Pow(Zoom, 3);

            float gamespaceY = ((screenCoordinates.Y-ResolutionHeight*0.5f+(ResolutionHeight*0.5f+Position.Y)*(float)Math.Pow(Zoom, 3)))/
                               (float)Math.Pow(Zoom, 3);

            var gamespaceCoordinates = new Vector2(gamespaceX, gamespaceY);

            return gamespaceCoordinates;
        }

        public Matrix GetTransformation()
        {
            //Sanitize results before
            if (Zoom<.75)
                Zoom = .75F;
            else if (Zoom>3)
                Zoom = 3f;

            Matrix transform = Matrix.CreateTranslation(new Vector3(ResolutionWidth*0.5f-Position.X, ResolutionHeight*0.5f-Position.Y, 0))*
                               Matrix.CreateScale(new Vector3((Zoom*Zoom*Zoom), (Zoom*Zoom*Zoom), 0))*Matrix.CreateRotationZ(0)*
                               Matrix.CreateTranslation(new Vector3(ResolutionWidth*0.5f, ResolutionHeight*0.5f, 0));

            return transform;
        }

        /// <summary>
        ///   Points the camera to a certain point in the game's coordinate system
        /// </summary>
        /// <param name = "gameCoordinates">A point in gamespace coordinates to point the camera to</param>
        public void PointTo(Vector2 gameCoordinates)
        {
            float newCameraX = gameCoordinates.X-(ResolutionWidth/2f);
            float newCameraY = gameCoordinates.Y-(ResolutionHeight/2f);
            Position = new Vector2(newCameraX, newCameraY);
            //Position = new Vector2(gameCoordinates.X, gameCoordinates.Y);
        }

        public void ZoomIn()
        {
            Zoom += 0.01F;
        }

        public void ZoomOut()
        {
            Zoom -= 0.01F;
        }
    }
}