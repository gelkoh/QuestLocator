using UnityEngine;

public class SoundFeedbackManager : MonoBehaviour
{
    public static SoundFeedbackManager SoundFeedbackManagerInstance { get; private set; }

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successSound;
    [SerializeField] private AudioClip _failedSound;

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

    public void PlayScanSuccess()
    {
        _audioSource.PlayOneShot(_successSound);
    }

    public void PlayScanFailed()
    {
        _audioSource.PlayOneShot(_failedSound);
    }
}