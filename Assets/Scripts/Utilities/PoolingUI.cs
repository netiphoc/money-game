using System.Collections.Generic;
using UI;
using UnityEngine;

public class PoolingUI<TType> where TType : BaseUI
{
    private readonly TType _prefab;
    private readonly List<TType> _pools;
    private int _index;
    
    public PoolingUI(TType prefab)
    {
        _prefab = prefab;
        _pools = new List<TType>();
    }

    public TType Request(Transform container)
    {
        _index++;
        
        if (_index >= _pools.Count)
        {
            // new
            TType newItem = Object.Instantiate(_prefab, container);
            _pools.Add(newItem);
            newItem.SetVisible(true);
            return newItem;
        }

        _pools[_index].SetVisible(true);
        return _pools[_index];
    }

    public TType RequestRecycle(Transform container)
    {
        foreach (var item in _pools)
        {
            if(item.Visible) continue;
            item.SetVisible(true);
            return item;
        }

        return Request(container);
    }
    
    public void ClearPool()
    {
        _index = 0;
        
        foreach (var item in _pools)
        {
            item.SetVisible(false);
        }   
    }
}