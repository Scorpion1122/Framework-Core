using System;
using UnityEngine;

namespace TKO.Core.Environment
{
    [RequireComponent(typeof(Light))]
    public class Sun : MonoBehaviour
    {
        // Default set to Amsterdam
        [SerializeField] private float longitude = 4.899431f;
        [SerializeField] float latitude = 52.379189f;

        [SerializeField, Range(0, 24)] int hour;
        [SerializeField, Range(0, 60)] int minutes;

        private new Light light;
        private DateTime time;

        [SerializeField]
        float timeSpeed = 1;

        private void Awake()
        {
            light = GetComponent<Light>();
        }

        private void OnEnable()
        {
            InitializeAndUpdatePosition();
        }

        public void SetTime(int hour, int minutes) 
        {
            this.hour = hour;
            this.minutes = minutes;
            InitializeTime();
        }

        private void InitializeTime()
        {
            time = DateTime.Now.Date + new TimeSpan(hour, minutes, 0);
        }

        private void Update()
        {
            time = time.AddSeconds(timeSpeed * Time.deltaTime);
            hour = time.Hour;
            minutes = time.Minute;
            UpdatePosition();
        }

        public void InitializeAndUpdatePosition()
        {
            InitializeTime();
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 angles = new Vector3();
            double alt;
            double azi;
            SunUtility.CalculateSunPosition(time, (double)latitude, (double)longitude, out azi, out alt);
            angles.x = (float)alt * Mathf.Rad2Deg;
            angles.y = (float)azi * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(angles);
            light.intensity = Mathf.InverseLerp(-12, 0, angles.x);
        }

    }
}