using System.Collections.Generic;
using UnityEngine;

namespace ZestGames
{
    public class AudioHandler : MonoBehaviour
    {
        private static Dictionary<Enums.AudioType, float> audioTimerDictionary;
        private static GameObject oneShotGameObject;
        private static AudioSource oneShotAudioSource;

        // we initialized dictionary for delayed sound.
        // Initalize this in Awake which script you want to use delayed sound.
        public static void Initalize()
        {
            audioTimerDictionary = new Dictionary<Enums.AudioType, float>();
            audioTimerDictionary[Enums.AudioType.Testing_PlayerMove] = 0;
        }

        public static void PlayAudio(Enums.AudioType audioType, Vector3 position)
        {
            if (!SettingsManager.SoundOn) return;

            if (CanPlayAudio(audioType))
            {
                GameObject audioGameObject = new GameObject("Audio");
                audioGameObject.transform.position = position;

                AudioSource audioSource = audioGameObject.AddComponent<AudioSource>();
                audioSource.clip = GetAudioClip(audioType);
                audioSource.Play();

                Object.Destroy(audioGameObject, audioSource.clip.length);// Destroy when clip is finished
            }
        }

        public static void PlayAudio(Enums.AudioType audioType, float volume = 1f, float pitch = 1f)
        {
            if (!SettingsManager.SoundOn) return;

            if (CanPlayAudio(audioType))
            {
                if (oneShotGameObject == null)
                {
                    oneShotGameObject = new GameObject("One Shot Audio");
                    oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                }

                oneShotAudioSource.volume = volume;
                oneShotAudioSource.pitch = pitch;
                oneShotAudioSource.PlayOneShot(GetAudioClip(audioType));
            }
        }

        // Add delayed audios here.
        private static bool CanPlayAudio(Enums.AudioType audioType)
        {
            switch (audioType)
            {
                default:
                    return true;
                case Enums.AudioType.Testing_PlayerMove:
                    if (audioTimerDictionary.ContainsKey(audioType))
                    {
                        float lastTimePlayed = audioTimerDictionary[audioType];
                        float playerMoveTimerMax = .15f;
                        if (lastTimePlayed + playerMoveTimerMax < Time.time)
                        {
                            audioTimerDictionary[audioType] = Time.time;
                            return true;
                        }
                        else return false;
                    }
                    else return true;
                    //break;
            }
        }

        private static AudioClip GetAudioClip(Enums.AudioType audioType)
        {
            List<AudioClip> multipleAudioClips = new List<AudioClip>();
            foreach (Audio audio in AudioData.Instance.Audios)
            {
                if (audio.Type == audioType)
                    multipleAudioClips.Add(audio.Clip);
            }

            if (multipleAudioClips.Count == 1)
                return multipleAudioClips[0];
            else if (multipleAudioClips.Count > 1)
                return multipleAudioClips[Random.Range(0, multipleAudioClips.Count)];
            else
            {
                Debug.LogError("Audio " + audioType + " not found!");
                return null;
            }
        }
    }
}
