using System;
using System.Collections.Generic;
using UI;
using Utilities;

namespace Systems
{
    public struct InteractionPromptData
    {
        public KeyIcon Icon;
        public string Prompt;
        public Func<string> RealTimePrompt;
    }
    
    public abstract class BaseInteractable : BaseMonoBehaviour, IInteractable
    {
        protected static readonly InteractionPromptData InteractionPrompt_MOVE = new InteractionPromptData
        {
            Icon = KeyIcon.MOUSE_LEFT_CLICK,
            Prompt = "(Hold) MOVE"
        };
        
        protected static readonly InteractionPromptData InteractionPrompt_PLACE = new InteractionPromptData
        {
            Icon = KeyIcon.MOUSE_LEFT_CLICK,
            Prompt = "PLACE"
        };
        
        protected static readonly InteractionPromptData InteractionPrompt_PICKUP = new InteractionPromptData
        {
            Icon = KeyIcon.MOUSE_LEFT_CLICK,
            Prompt = "PICKUP"
        };
        
        protected static readonly InteractionPromptData InteractionPrompt_THROW = new InteractionPromptData
        {
            Icon = KeyIcon.G,
            Prompt = "THROW"
        };
        
        private readonly Dictionary<int, InteractionPromptData[]> _interactionPromptMap = new ();
        
        protected override void Awake()
        {
            base.Awake();
            
            InitInteractionPrompts();
        }
        protected virtual void InitInteractionPrompts()
        {
            AddInteractionPrompt(new []
            {
                InteractionPrompt_PICKUP,
            });
        }
        
        protected void AddInteractionPrompt(InteractionPromptData[] data)
        {
            _interactionPromptMap.Add(_interactionPromptMap.Count, data);
        }

        protected InteractionPromptData[] GetInteractionPromptByIndex(int index)
        {
            return _interactionPromptMap[index];
        }

        public virtual InteractionPromptData[] GetInteractionPrompts()
        {
            return GetInteractionPromptByIndex(0);
        }
        
        public virtual BaseUI GetUI()
        {
            return null;
        }

        public virtual void OnInteract(PlayerInteraction player)
        {
        }

        public virtual void OnAltInteract(PlayerInteraction player)
        {
        }
    }
}