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
            Global.TUTORIAL.SetWindow(Global.GAME_WIDTH, Global.GAME_HEIGHT, false, false);

            Global.PlayerSession = Global.TUTORIAL.AddSession("Player");

            Global.PlayerSession.Controller.AddButton("start");
            Global.PlayerSession.Controller.AddButton("select");
            Global.PlayerSession.Controller.AddButton("up");
            Global.PlayerSession.Controller.AddButton("down");
            Global.PlayerSession.Controller.AddButton("left");
            Global.PlayerSession.Controller.AddButton("right");
            Global.PlayerSession.Controller.AddButton("a");
            Global.PlayerSession.Controller.AddButton("b");
            Global.PlayerSession.Controller.AddButton("x");
            Global.PlayerSession.Controller.AddButton("y");
            Global.PlayerSession.Controller.AddButton("r1");
            Global.PlayerSession.Controller.AddButton("l1");

            Global.PlayerSession.Controller.Button("start").AddKey(Key.Return);
            Global.PlayerSession.Controller.Button("up").AddKey(Key.W);
            Global.PlayerSession.Controller.Button("left").AddKey(Key.A);
            Global.PlayerSession.Controller.Button("down").AddKey(Key.S);
            Global.PlayerSession.Controller.Button("right").AddKey(Key.D);
            Global.PlayerSession.Controller.Button("x").AddKey(Key.Left);
            Global.PlayerSession.Controller.Button("a").AddKey(Key.Down);
            Global.PlayerSession.Controller.Button("b").AddKey(Key.Right);
            Global.PlayerSession.Controller.Button("y").AddKey(Key.Up);
            Global.PlayerSession.Controller.Button("r1").AddKey(Key.Space);
            Global.PlayerSession.Controller.Button("l1").AddKey(Key.Z);
            Global.PlayerSession.Controller.Button("select").AddKey(Key.P);

            Global.TUTORIAL.FirstScene = new TitleScene();
            Global.TUTORIAL.Start();
        }
    }
}
