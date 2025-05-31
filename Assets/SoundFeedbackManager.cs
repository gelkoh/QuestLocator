using UnityEngine;

public class SoundFeedbackManager : MonoBehaviour
{
    public static SoundFeedbackManager SoundFeedbackManagerInstance { get; private set; }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successSound;

    void Awake()
    {
        if (SoundFeedbackManagerInstance == null)
        {
            SoundFeedbackManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySuccess()
    {
        _audioSource.PlayOneShot(_successSound);
    }
}
