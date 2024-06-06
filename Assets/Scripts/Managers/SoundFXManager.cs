using Unity.VisualScripting;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void PlaySoundFXClip(AudioClip[] clips, Transform spawnTransform, float volume)
    {
        var index = Random.Range(0, clips.Length);
        var audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = clips[index];
        audioSource.volume = volume;
        audioSource.Play();
        var clipLength = audioSource.clip.length;
        Destroy(audioSource.GameObject(), clipLength);
    }
}

