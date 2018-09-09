﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Payload.MonoScript
{
    public class Console : MonoBehaviour
    {
        private bool Active = true;
        private bool WriteToLog = false;

        private KeyCode Switch = KeyCode.Home;

        private Rect ConsoleRect = new Rect(Screen.width * 0.34f, Screen.height * 0.6f, Screen.width * 0.64f, Screen.height * 0.38f);
        
        private List<string> ConsoleString = new List<string>();
        private Vector2 ScrollPosition = new Vector2();


        private void Awake()
        {
            Application.logMessageReceived +=
                (string condition, string stackTrace, LogType type) =>
                {
                    ScrollPosition = new Vector2(0, System.Single.MaxValue - 1);
                    ConsoleString.Add("<size=15>" + TagString(condition, type) + " </size>" + "\n" + stackTrace + "\n");

                    if (ConsoleString.Count > 20)
                    {
                        ConsoleString.RemoveAt(0);
                    }

                    if (WriteToLog)
                        File.AppendAllText(@".\UnityConsoleInjected.log", "[" + System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff") + "] " + type.ToString() + ": " + condition + "\r\n" + (stackTrace != string.Empty ? stackTrace + "\r\n" : string.Empty));
                };
        }
        private void OnGUI()
        {
            if (!Active) return;
            GUILayout.Window(WindowID.CONSOLE, ConsoleRect, (id) =>
            {
                WriteToLog = GUILayout.Toggle(WriteToLog, "WriteToLog");

                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
                GUILayout.Label(GetConsoleString(), GUIStyles.DEFAULT_LABEL);
                GUILayout.EndScrollView();


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Clr", GUILayout.Height(20))) Clear();
                if (GUILayout.Button("Close", GUILayout.Height(20))) Active = false;
                GUILayout.EndHorizontal();

            }, "Unity Console", GUIStyles.DEFAULT_WINDOW);
        }
        private void Update()
        {
            if (Input.GetKeyDown(Switch)) Active = !Active;
        }

        private string GetConsoleString()
        {
            string str = string.Empty;
            foreach (string s in ConsoleString) { str += s; }
            return str;
        }
        private string TagString(string str, LogType type)
        {
            string ColorTag = string.Empty;
            switch (type)
            {
                case LogType.Log: ColorTag = "<color=white>Log: "; break;
                case LogType.Warning: ColorTag = "<color=yellow>Warning: "; break;
                case LogType.Error: ColorTag = "<color=red>Error: "; break;
                case LogType.Exception: ColorTag = "<color=red>Exception: "; break;
                case LogType.Assert: ColorTag = "<color=magenta>Assert: "; break;
            }
            string ColorTagEnd = " </color>";

            return ColorTag + str + ColorTagEnd;
        }
        private void Clear()
        {
            ConsoleString.Clear();
            ConsoleString = new List<string>();
        }
    }
}
