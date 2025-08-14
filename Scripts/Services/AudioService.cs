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

        // 关卡
        void PlayLevelBGM(LevelMusicId levelMusicId);
        void PauseLevelMusic();
        void ResumeLevelMusic();
        void SetIntensity(float intensity);
        void StopLevelBGM();
    }

    public class AudioService : AbstractService, IAudioService
    {
        protected override void OnInit()
        {
        }

        #region 通用

        public void PlaySFX(string sfxName)
        {
            FMODUnity.RuntimeManager.PlayOneShot(sfxName);
        }

        public void PlayMusic(string musicName)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region 关卡

        private FMOD.Studio.EventInstance _levelMusic;
        private FMOD.Studio.PARAMETER_ID _intensityId;
        // private FMOD.Studio.EventInstance _levelHeartbeat;

        public void PlayLevelBGM(LevelMusicId levelMusicId)
        {
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

        public void PauseLevelMusic()
        {
            _levelMusic.setPaused(true);
        }

        public void ResumeLevelMusic()
        {
            _levelMusic.setPaused(false);
        }

        public void SetIntensity(float intensity)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByID(_intensityId, intensity);
        }

        #endregion


        public void StopLevelBGM()
        {
            _levelMusic.stop(STOP_MODE.ALLOWFADEOUT);
            _levelMusic.release();
            throw new System.NotImplementedException();
        }
    }
}