/* Authors: Edwin Zhang, Sepehr Hosseini
 * Last Updated: 2019-08-07
 * 
 * Description: 
 * Plays and stops background music based on biome
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public Rigidbody2D rb;
    public AudioSource jungleMusic;
    public AudioSource volcanoMusic;

    private int songPlaying = 0;
    private string biome;

    private const int jungleBiomeBorder = 84;

    //checks which biome gordo is in
    void checkBiome()
    {
        if (rb.transform.position.y < jungleBiomeBorder)
        {
            biome = "JUNGLE";
        }
        else
        {
            biome = "VOLCANO";
        }
    }

    //stops all the music from playing and looping
    void stopAllMusic()
    {
        jungleMusic.loop = false;
        jungleMusic.Stop();
        volcanoMusic.loop = false;
        volcanoMusic.Stop();
    }

    //plays music according to biome
    void musicChanger()
    {
        if (biome == "JUNGLE" && songPlaying != 1)
        {
            stopAllMusic();
            jungleMusic.Play();
            jungleMusic.loop = true;
            songPlaying = 1;
        }
        else if (biome == "VOLCANO" && songPlaying != 2)
        {
            stopAllMusic();
            volcanoMusic.Play();
            volcanoMusic.loop = true;
            songPlaying = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkBiome();
        musicChanger();
    }
}
