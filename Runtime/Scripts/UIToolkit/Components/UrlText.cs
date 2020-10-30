using UnityEngine;
using UnityEngine.UIElements;

namespace TKO.UI.Toolkit
{
    public class UrlText : TextElement
    {
        #region Consts

        private const string UrlStyleName = "url";
        private const string UrlVisitedStyleName = "url-visited";
        
        #endregion
        
        #region Variables
        
        private string _url;
        private Clickable _clickable;
        
        #endregion
        
        #region Lifecycle

        public UrlText() 
            : this(string.Empty, string.Empty)
        {
        }
        
        public UrlText(string url) 
            : this(url, url)
        {
        }

        public UrlText(string text, string url)
        {
            _url = url;
            tooltip = url;
            this.text = text;
            
            _clickable = new Clickable(OnClickEvent);
            this.AddManipulator(_clickable);
            
            AddToClassList(UrlStyleName);
        }

        public new class UxmlFactory : UnityEngine.UIElements.UxmlFactory<UrlText, UrlText.UxmlTraits>
        {
        }

        public new class UxmlTraits : TextElement.UxmlTraits
        {
            private UxmlStringAttributeDescription _urlAttribute;
                
            public UxmlTraits() : base()
            {
                _urlAttribute = new UxmlStringAttributeDescription();
                _urlAttribute.name = "url";
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                string url = string.Empty;
                if(_urlAttribute.TryGetValueFromBag(bag, cc, ref url))
                {
                    ((UrlText)ve).SetUrl(url);
                }
            }
        }
        
        #endregion
        
        #region Public Methods

        public void SetUrl(string url)
        {
            _url = url;
            tooltip = url;
        }
        
        #endregion
        
        #region Private Methods
        
        private void OnClickEvent(EventBase eventBase)
        {
            Application.OpenURL(_url);
            RemoveFromClassList(UrlStyleName);
            AddToClassList(UrlVisitedStyleName);
        }
        
        #endregion
    }
}
