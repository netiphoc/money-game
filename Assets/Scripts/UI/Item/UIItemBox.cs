using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Item
{
    [RequireComponent(typeof(ItemBox))]
    public class UIItemBox : BaseUI
    {
        [SerializeField] private Image[] iconItems;
        [SerializeField] private TMP_Text itemAmount;

        protected override void OnEnable()
        {
            base.OnEnable();
            ItemBox itemBox = GetComponent<ItemBox>();
            itemBox.OnItemChanged += OnItemChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ItemBox itemBox = GetComponent<ItemBox>();
            itemBox.OnItemChanged -= OnItemChanged;
        }

        protected override void Start()
        {
            base.Start();
            OnItemChanged(GetComponent<ItemBox>());
        }

        private void OnItemChanged(ItemBox itemBox)
        {
            foreach (var icon in iconItems)
            {
                icon.sprite = itemBox.itemData.icon;
            }
            
            itemAmount.SetText($"{itemBox.currentQuantity} / {itemBox.maxCapacity}");
        }
    }
}