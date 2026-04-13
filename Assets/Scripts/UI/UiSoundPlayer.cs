using UnityEngine;

namespace CaravanRoguelite.UI
{
    public class UiSoundPlayer : MonoBehaviour
    {
        private AudioSource _source;

        private AudioClip _click;
        private AudioClip _ok;
        private AudioClip _warn;
        private AudioClip _hit;
        private AudioClip _win;
        private AudioClip _lose;
        private AudioClip _travel;
        private AudioClip _event;
        private AudioClip _city;
        private AudioClip _combat;

        private void Awake()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _source.volume = 0.35f;

            _click = BuildTone(680f, 0.06f, Wave.Square, 0.6f);
            _ok = BuildTone(740f, 0.09f, Wave.Sine, 0.8f);
            _warn = BuildTone(230f, 0.12f, Wave.Saw, 0.85f);
            _hit = BuildTone(320f, 0.08f, Wave.Square, 0.9f);
            _win = BuildSweep(420f, 920f, 0.22f, 0.95f);
            _lose = BuildSweep(380f, 150f, 0.24f, 0.95f);
            _travel = BuildSweep(520f, 620f, 0.11f, 0.7f);
            _event = BuildSweep(560f, 340f, 0.14f, 0.75f);
            _city = BuildTone(510f, 0.1f, Wave.Sine, 0.45f);
            _combat = BuildTone(180f, 0.13f, Wave.Saw, 0.92f);
        }

        public void PlayClick() => Play(_click, 0.55f);
        public void PlayOk() => Play(_ok, 0.75f);
        public void PlayWarn() => Play(_warn, 0.8f);
        public void PlayHit() => Play(_hit, 0.8f);
        public void PlayWin() => Play(_win, 0.95f);
        public void PlayLose() => Play(_lose, 0.95f);
        public void PlayTravel() => Play(_travel, 0.8f);
        public void PlayEvent() => Play(_event, 0.7f);
        public void PlayCity() => Play(_city, 0.85f);
        public void PlayCombat() => Play(_combat, 0.8f);

        private void Play(AudioClip clip, float volume)
        {
            if (clip != null)
            {
                _source.PlayOneShot(clip, volume);
            }
        }

        private static AudioClip BuildTone(float frequency, float duration, Wave wave, float decay)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)sampleRate;
                float osc = wave switch
                {
                    Wave.Square => Mathf.Sign(Mathf.Sin(2f * Mathf.PI * frequency * t)),
                    Wave.Saw => 2f * (frequency * t - Mathf.Floor(0.5f + frequency * t)),
                    _ => Mathf.Sin(2f * Mathf.PI * frequency * t)
                };

                float envelope = Mathf.Pow(1f - (i / (float)samples), Mathf.Lerp(1.2f, 3f, decay));
                data[i] = osc * envelope * 0.25f;
            }

            var clip = AudioClip.Create($"tone_{frequency}", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip BuildSweep(float fromFrequency, float toFrequency, float duration, float decay)
        {
            const int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            float phase = 0f;

            for (int i = 0; i < samples; i++)
            {
                float progress = i / (float)(samples - 1);
                float freq = Mathf.Lerp(fromFrequency, toFrequency, progress);
                phase += 2f * Mathf.PI * freq / sampleRate;
                float envelope = Mathf.Pow(1f - progress, Mathf.Lerp(1.1f, 2.6f, decay));
                data[i] = Mathf.Sin(phase) * envelope * 0.25f;
            }

            var clip = AudioClip.Create($"sweep_{fromFrequency}_{toFrequency}", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private enum Wave
        {
            Sine,
            Square,
            Saw
        }
    }
}
