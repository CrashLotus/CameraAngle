using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    const int s_version = 0;

    [Serializable]
    public class DateBlock
    {
        public string date;
        public List<string> files = new List<string>();
    }

    [Serializable]
    public class SaveFile
    {
        public int version = s_version;
        public List<DateBlock> dateBlocks = new List<DateBlock>();
    }

    static SaveData s_theData;
    bool m_isInit = false;
    SaveFile m_saveData;

    public static SaveData Get()
    {
        if (s_theData == null)
        {
            GameObject obj = new GameObject("SaveData");
            s_theData = obj.AddComponent<SaveData>();
            s_theData.Initialize();
        }
        return s_theData;
    }

    public void AddImage(string date, string filePath)
    {
//        Debug.Log("AddImage(\"" + date + "\", \"" + filePath + "\"");
        // look for the date
        bool foundIt = false;
        foreach (DateBlock dateBlock in m_saveData.dateBlocks) 
        { 
            if (dateBlock.date == date)
            {   // found it
                dateBlock.files.Add(filePath);
                foundIt = true;
                break;
            }
        }
        if (false == foundIt)
        {
            DateBlock dateBlock = new DateBlock();
            dateBlock.date = date;
            dateBlock.files.Add(filePath);
            m_saveData.dateBlocks.Add(dateBlock);
        }
        Save();
    }

    static string GetFilename()
    {
        string filename = Application.persistentDataPath + "/angliator.json";
        return filename;
    }

    void Initialize()
    {
        if (m_isInit)
            return;

        m_saveData = new SaveFile();
        string filename = GetFilename();
        if (File.Exists(filename))
        {
            string data = File.ReadAllText(filename);
            m_saveData = JsonUtility.FromJson<SaveFile>(data);
        }

        m_isInit = true;

        PrintData();
    }

    void Save()
    {
        string filename = GetFilename();
        m_saveData.version = s_version;
        string data = JsonUtility.ToJson(m_saveData);
        File.WriteAllText(filename, data);
    }

    void PrintData()
    {
        if (null == m_saveData)
            return;
        foreach (DateBlock dateBlock in m_saveData.dateBlocks)
        {
            Debug.Log("Date: " + dateBlock.date);
            foreach (string file in dateBlock.files)
            {
                Debug.Log(file);
            }
        }
    }
}
