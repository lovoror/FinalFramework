using UnityEngine;
using System.Collections;
using FirClient.Utility;
using System;
using System.IO;
using LuaInterface;

namespace FirClient.Manager
{
    public class SoundManager : BaseManager
    {
        private AudioSource audio = null;
        private Hashtable sounds = new Hashtable();

        [NoToLua]
        public override void Initialize()
        {
            audio = ManagementCenter.managerObject.GetComponent<AudioSource>();
        }

        [NoToLua]
        public override void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// ����һ������
        /// </summary>
        void Add(string key, AudioClip value)
        {
            if (sounds[key] != null || value == null)
            {
                return;
            }
            sounds.Add(key, value);
        }

        /// <summary>
        /// ��ȡһ������
        /// </summary>
        AudioClip Get(string key)
        {
            if (sounds[key] == null)
            {
                return null;
            }
            return sounds[key] as AudioClip;
        }

        /// <summary>
        /// ����һ����Ƶ
        /// </summary>
        void LoadAudioClip(string path, Action<AudioClip> action)
        {
            AudioClip ac = Get(path);
            if (ac == null)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                resMgr.LoadAssetAsync<AudioClip>(path, new[] { name }, delegate (UnityEngine.Object[] objs)
                {
                    if (objs.Length == 0 || objs[0] == null)
                    {
                        return;
                    }
                    var clip = objs[0] as AudioClip;
                    if (clip != null)
                    {
                        Add(path, clip);
                        action(clip);
                    }
                });
            }
            else
            {
                action(ac);
            }
        }

        /// <summary>
        /// �Ƿ񲥷ű������֣�Ĭ����1������
        /// </summary>
        /// <returns></returns>
        public bool CanPlayBackSound()
        {
            string key = AppConst.AppPrefix + "BackSound";
            int i = PlayerPrefs.GetInt(key, 1);
            return i == 1;
        }

        /// <summary>
        /// ���ű�������
        /// </summary>
        /// <param name="canPlay"></param>
        public void PlayBacksound(string name, bool canPlay)
        {
            if (audio.clip != null)
            {
                if (name.IndexOf(audio.clip.name) > -1)
                {
                    if (!canPlay)
                    {
                        audio.Stop();
                        audio.clip = null;
                        Util.ClearMemory();
                    }
                    return;
                }
            }
            if (canPlay)
            {
                audio.loop = true;
                LoadAudioClip(name, delegate(AudioClip clip){
                    audio.clip = clip;
                    audio.Play();
                });
            }
            else
            {
                audio.Stop();
                audio.clip = null;
                Util.ClearMemory();
            }
        }

        /// <summary>
        /// �Ƿ񲥷���Ч,Ĭ����1������
        /// </summary>
        /// <returns></returns>
        public bool CanPlaySoundEffect()
        {
            string key = AppConst.AppPrefix + "SoundEffect";
            int i = PlayerPrefs.GetInt(key, 1);
            return i == 1;
        }

        /// <summary>
        /// ������Ƶ����
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="position"></param>
        void PlayInternal(AudioClip clip, Vector3 position)
        {
            if (!CanPlaySoundEffect())
            {
                return;
            }
            AudioSource.PlayClipAtPoint(clip, position);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="path"></param>
        public void Play(string path)
        {
            LoadAudioClip(path, delegate (AudioClip clip)
            {
                if (clip != null)
                {
                    this.PlayInternal(clip, Vector3.zero);
                }
            });
        }

        [NoToLua]
        public override void OnDispose()
        {
        }
    }
}