using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace TKO.UI.Toolkit
{
    public class TimerText : TextElement
    {
        #region Consts
        
        private static CustomStyleProperty<Color> UtcEndTime = new CustomStyleProperty<Color>("--utc-end-time");
        
        #endregion
        
        #region Variables
        
        private DateTime _utcEndTime;
        
        #endregion
        
        #region Lifecycle

        public TimerText()
            : this(DateTime.UtcNow)
        {
        }

        public TimerText(DateTime utcEndTime)
        {
            _utcEndTime = utcEndTime;
            UpdateTimerText();
            
            schedule
                .Execute(UpdateTimerText)
                .Every(1000);
        }

        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<TimerText, TimerText.UxmlTraits>
        {
        }

        public new class UxmlTraits : TextElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _utcEndTimeAttribute;
                
            public UxmlTraits() : base()
            {
                _utcEndTimeAttribute = new UxmlStringAttributeDescription();
                _utcEndTimeAttribute.name = "utcEndTime";
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                string utcEndTimeString = string.Empty;
                if(_utcEndTimeAttribute.TryGetValueFromBag(bag, cc, ref utcEndTimeString)
                    && DateTime.TryParse(utcEndTimeString, out DateTime utcEndTime))
                {
                    ((TimerText)ve).SetUtcEndTime(utcEndTime);
                }
            }
        }
        
        #endregion
        
        #region Public Methods

        public void SetUtcEndTime(DateTime utcEndTime)
        {
            _utcEndTime = utcEndTime;
        }
        
        #endregion
        
        #region Private Methods

        private void UpdateTimerText()
        {
            TimeSpan timeSpan = _utcEndTime - DateTime.UtcNow;
            timeSpan = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            text = timeSpan.ToString();
        }
        
        #endregion
    }
}
