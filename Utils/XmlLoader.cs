using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class XmlLoader : MonoBehaviour 
{
    XmlDocument xmlDoc;

    public bool LoadXml(string _xmlPath)
    {
        try
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(_xmlPath);

            //Debug.Log(xmlDoc.InnerXml);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error loading xml " + _xmlPath + ":\n" + e);
            return false;
        }
    }
    
    public XmlDocument GetXmldoc()
    {
        return xmlDoc;
    }

}
