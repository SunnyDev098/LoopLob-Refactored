using UnityEngine;

public class BallAudioHandler : MonoBehaviour
{

   [SerializeField] private AudioClip ballAttachSFX;
   [SerializeField] private AudioClip ballBounceSFX;
   [SerializeField] private AudioClip ballRotationSFX;
   [SerializeField] private AudioSource audioSource;


    public void playAttach() => audioSource.PlayOneShot(ballAttachSFX);
    public void playBounce() => audioSource.PlayOneShot(ballBounceSFX);
    public void playRotation() => audioSource.PlayOneShot(ballRotationSFX);
  

}
