using System;
using FMOD.Studio;
using FMODUnity;
using QFramework;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace TPL.PVZR.Services
{
    public enum LevelMusicId
    {
        Lawn,
    }

    public interface IAudioService : IService
    {
        // 通用
        void PlaySFX(string sfxName);
        void PlayMusic(string musicName);
        // 
        FMOD.Studio.EventInstance CreateEventInstance(string sfxName);

        // 关卡
        void PlayLevelBGM(LevelMusicId levelMusicId);
        void PauseLevelBGM();
        void ResumeLevelBGM();
        void SetIntensity(float intensity);
        void StopLevelBGM();
    }

    public class AudioService : AbstractService, IAudioService
    {
        protected override void OnInit()
        {
        }

        #region 通用

        private bool _isBgmPlaying = false;

        public void PlaySFX(string sfxName)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sfxName);
        }

        public void PlayMusic(string musicName)
        {
            throw new System.NotImplementedException();
        }

        public EventInstance CreateEventInstance(string sfxName)
        {
            return FMODUnity.RuntimeManager.CreateInstance(sfxName);
        }

        #endregion

        #region 关卡

        private FMOD.Studio.EventInstance _levelMusic;
        private FMOD.Studio.PARAMETER_ID _intensityId;
        // private FMOD.Studio.EventInstance _levelHeartbeat;

        public void PlayLevelBGM(LevelMusicId levelMusicId)
        {
            if (_isBgmPlaying) throw new Exception("已经有音乐在播放，请先关闭先前的音乐");

            var musicStr = levelMusicId switch
            {
                LevelMusicId.Lawn => "event:/Music/Lawn",
                _ => throw new System.ArgumentOutOfRangeException(nameof(levelMusicId), levelMusicId, null)
            };

            _levelMusic = FMODUnity.RuntimeManager.CreateInstance(musicStr);
            FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName("Intensity", out var intensityDesc);
            _intensityId = intensityDesc.id;

            _levelMusic.start();
        }

        public void PauseLevelBGM()
        {
            _levelMusic.setPaused(true);
        }

        public void ResumeLevelBGM()
        {
            _levelMusic.setPaused(false);
        }

        public void SetIntensity(float intensity)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByID(_intensityId, intensity);
        }


        public void StopLevelBGM()
        {
            _levelMusic.stop(STOP_MODE.ALLOWFADEOUT);
            _levelMusic.release();
        }

        #endregion
    }
}