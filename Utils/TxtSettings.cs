using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class TxtSettings : MonoBehaviour {

    Dictionary<string, string> configs;

    public bool LoadSetting(string _fileName)
    {
        try
        {
            configs = new Dictionary<string, string>();
            string line;
            StreamReader theReader = new StreamReader(_fileName);

            using (theReader)
            {
                do
                {
                    line = theReader.ReadLine();

                    if (line != null)
                    {
                        string[] entries = line.Split(' ');

                        if (entries.Length > 0)
                        {
                            configs[entries[0]] = entries[1];
                        }
                    }
                }
                while (line != null);

                theReader.Close();

                return true;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error loading " + _fileName + ":\n" + e);
            return false;
        }
    }

    public string GetSetting(string _item)
    {
        return configs[_item];
    }

}
