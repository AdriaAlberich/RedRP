/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */

using System;
using System.Collections.Generic;
using System.Drawing;

using RAGE;

namespace redrp
{

    /// <summary>
    /// Object editor system
    /// </summary>
    public class ObjectEditor : Events.Script
    {
        /// <summary>
        /// Editor data
        /// </summary>
        private const double DefaultVelocity = 0.1;
        private const double MinVelocity = 0.01;
        private const double MaxVelocity = 1.0;
        private const double Components = 6;
        private const double MaxOffset = 10.0;

        public static bool visible = false;
        public static string title = "";
        public static int mode = 0;
        public static double velocity = 0.0;
        public static int component = 0;
        public static Vector3 offset;
        public static Vector3 rotation;
        public static Vector3 position;

        private enum Mode
        {
            AttachedObject,
            Furniture,
            Barrier
        }

        private enum Component
        {
            XOffset,
            YOffset,
            ZOffset,
            XRotation,
            YRotation,
            ZRotation
        }

        private Dictionary<int, string> modeName = new Dictionary<int, string>
        {
            {(int)Mode.AttachedObject, "Objeto enganchado"},
            {(int)Mode.Furniture, "Mueble"},
            {(int)Mode.Barrier, "Barricada"}
        };

        private Dictionary<int, string> componentName = new Dictionary<int, string>
        {
            {(int)Component.XOffset, "Desplazamiento X"},
            {(int)Component.YOffset, "Desplazamiento Y"},
            {(int)Component.ZOffset, "Desplazamiento Z"},
            {(int)Component.XRotation, "Rotación X"},
            {(int)Component.YRotation, "Rotación Y"},
            {(int)Component.ZRotation, "Rotación Z"}
        };

        /// <summary>
        /// Editor initialization
        /// </summary>
        public ObjectEditor()
        {
            Events.Add("objectEditorInitialization", EditorInitialization);

            Events.Tick += Render;
        }

        /// <summary>
        /// Object editor initialization
        /// </summary>
        /// <param name="args"></param>
        private void EditorInitialization(object[] args)
        {
            visible = (bool)args[0];
            title = args[1].ToString();
            mode = (int)args[2];
            velocity = (double)args[3];
            component = (int)args[4];
            offset = (Vector3)args[5];
            rotation = (Vector3)args[6];
            position = (Vector3)args[7];
        }

        /// <summary>
        /// Executes code every tick (game frame)
        /// </summary>
        /// <param name="nametags"></param>
        private void Render(List<Events.TickNametagData> nametags)
        {
            if (visible)
            {
                RAGE.NUI.UIResRectangle.Draw(0, 600, 450, 250, Color.FromArgb(150, Color.Black));
                RAGE.NUI.UIResText.Draw(title, 10, 620, RAGE.Game.Font.ChaletLondon, 0.4f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Modo: ", 10, 660, RAGE.Game.Font.ChaletLondon, 0.3f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw(modeName[mode], 70, 660, RAGE.Game.Font.ChaletLondon, 0.3f, Color.LightYellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Componente: ", 10, 680, RAGE.Game.Font.ChaletLondon, 0.3f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw(componentName[component], 70, 680, RAGE.Game.Font.ChaletLondon, 0.3f, Color.LightYellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Velocidad: ", 10, 700, RAGE.Game.Font.ChaletLondon, 0.3f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw(velocity.ToString(), 70, 700, RAGE.Game.Font.ChaletLondon, 0.3f, Color.LightYellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Posición: ", 10, 720, RAGE.Game.Font.ChaletLondon, 0.3f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw(position.X.ToString("0.##") + " , " + position.Y.ToString("0.##") + " , " + position.Z.ToString("0.##"), 70, 720, RAGE.Game.Font.ChaletLondon, 0.3f, Color.LightYellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Rotación: ", 10, 740, RAGE.Game.Font.ChaletLondon, 0.3f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw(rotation.X.ToString("0.##") + " , " + rotation.Y.ToString("0.##") + " , " + rotation.Z.ToString("0.##"), 70, 740, RAGE.Game.Font.ChaletLondon, 0.3f, Color.LightYellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Cambiar componente: flecha arriba | Cambiar velocidad: flecha abajo", 10, 780, RAGE.Game.Font.ChaletLondon, 0.2f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                RAGE.NUI.UIResText.Draw("Editar: flechas izquierda y derecha | Guardar: Enter | Cancelar: Retroceso", 10, 780, RAGE.Game.Font.ChaletLondon, 0.2f, Color.Yellow, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
            }
        }

        /// <summary>
        /// Switch between components
        /// </summary>
        public static void SwitchEditingComponent()
        {
            component++;
            if(component == Components)
            {
                component = 0;
            }
        }

        /// <summary>
        /// Changes the movement velocity of the objects
        /// </summary>
        public static void ChangeEditingVelocity()
        {
            switch(velocity)
            {
                case DefaultVelocity: velocity = MaxVelocity; break;
                case MaxVelocity: velocity = MinVelocity; break;
                case MinVelocity: velocity = DefaultVelocity; break;
            }
        }

        /// <summary>
        /// Increse the selected component by the selected velocity
        /// </summary>
        public static void IncreaseComponent()
        {
            switch (component)
            {
                case (int)Component.XOffset:
                    {
                        position.X += (float)velocity;
                        offset.X += (float)velocity;
                        break;
                    }
                case (int)Component.YOffset:
                    {
                        position.Y += (float)velocity;
                        offset.Y += (float)velocity;
                        break;
                    }
                case (int)Component.ZOffset:
                    {
                        position.Z += (float)velocity;
                        offset.Z += (float)velocity;
                        break;
                    }
                case (int)Component.XRotation:
                    {
                        rotation.X += (float)velocity;
                        break;
                    }
                case (int)Component.YRotation:
                    {
                        rotation.Y += (float)velocity;
                        break;
                    }
                case (int)Component.ZRotation:
                    {
                        rotation.Z += (float)velocity;
                        break;
                    }
            }

            Events.CallRemote("onObjectEditorUpdate", offset, rotation, position);
        }

        /// <summary>
        /// Decrease the selected component by the selected velocity
        /// </summary>
        public static void DecreaseComponent()
        {
            switch (component)
            {
                case (int)Component.XOffset:
                    {
                        position.X -= (float)velocity;
                        offset.X -= (float)velocity;
                        break;
                    }
                case (int)Component.YOffset:
                    {
                        position.Y -= (float)velocity;
                        offset.Y -= (float)velocity;
                        break;
                    }
                case (int)Component.ZOffset:
                    {
                        position.Z -= (float)velocity;
                        offset.Z -= (float)velocity;
                        break;
                    }
                case (int)Component.XRotation:
                    {
                        rotation.X -= (float)velocity;
                        break;
                    }
                case (int)Component.YRotation:
                    {
                        rotation.Y -= (float)velocity;
                        break;
                    }
                case (int)Component.ZRotation:
                    {
                        rotation.Z -= (float)velocity;
                        break;
                    }
            }

            Events.CallRemote("onObjectEditorUpdate", offset, rotation, position);
        }

    }
}
