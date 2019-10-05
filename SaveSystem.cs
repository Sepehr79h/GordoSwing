/* Authors: Edwin Zhang, Sepehr Hosseini
 * Last Updated: 2019-08-07
 * 
 * Description: 
 * Creates system file and saves player data
 * 
 */
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private const float startX = 18.53f;
    private const float startY = -4.89f;
    private const float startZ = -0.039248f;

    public static void SavePlayer(UserControls player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.sv";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player.transform.position.x, player.transform.position.y, player.transform.position.z);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void ResetPlayer()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.sv";

        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(startX, startY,startZ);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.sv";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
