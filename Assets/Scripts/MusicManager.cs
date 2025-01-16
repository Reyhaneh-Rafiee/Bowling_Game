using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance = null;
    private AudioSource audioSource;

    public AudioClip scene1Music;
    public AudioClip scene2Music;
    public AudioClip scene3Music;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySceneMusic(string sceneName)
    {
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "GameScene":
                clipToPlay = scene1Music;
                break;
            case "GameScene1":
                clipToPlay = scene2Music;
                break;
            case "GameScene2":
                clipToPlay = scene3Music;
                break;
        }

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }
}
