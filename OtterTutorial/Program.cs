// Must include Otter in your project, so we add this line
using Otter;

using OtterTutorial;
using OtterTutorial.Scenes;

using System;
using System.Collections.Generic;
using System.Text;

namespace OtterTutorial // This can be anything you choose, I opted for my project's name
{
    public class Program
    {
        static void Main(string[] args)
        {
            Global.TUTORIAL = new Game("OtterTutorial", Global.GAME_WIDTH, Global.GAME_HEIGHT);
            Global.TUTORIAL.SetWindow(Global.GAME_WIDTH, Global.GAME_HEIGHT);

            Global.PlayerSession = Global.TUTORIAL.AddSession("Player");
            Global.PlayerSession.Controller.Start.AddKey(Key.Return);
            Global.PlayerSession.Controller.Up.AddKey(Key.W);
            Global.PlayerSession.Controller.Left.AddKey(Key.A);
            Global.PlayerSession.Controller.Down.AddKey(Key.S);
            Global.PlayerSession.Controller.Right.AddKey(Key.D);
            Global.PlayerSession.Controller.X.AddKey(Key.Left);
            Global.PlayerSession.Controller.A.AddKey(Key.Down);
            Global.PlayerSession.Controller.B.AddKey(Key.Right);
            Global.PlayerSession.Controller.Y.AddKey(Key.Up);
            Global.PlayerSession.Controller.R1.AddKey(Key.Space);
            Global.PlayerSession.Controller.L1.AddKey(Key.Z);

            Global.TUTORIAL.FirstScene = new TitleScene();
            Global.TUTORIAL.Start();
        }
    }
}
