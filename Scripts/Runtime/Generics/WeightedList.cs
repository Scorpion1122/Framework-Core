using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace This.Core.Generics
{
	public enum WeightType
	{
		Normal,//higher value has more chance
		Inverted,//lower value has more chance
	}
	
	public struct WeightedItem<T>
	{
		private T item;
		public T Item { get { return item; } }

		private int weight;
		public int Weight { get { return weight; } }
		
		public WeightedItem(T item, int weight)
		{
			this.item = item;
			this.weight = weight;
		}
	}

	public class WeightedList<T>
	{
		public int Count { get { return items.Count; } }
		public int TotalWeight { get { return totalWeight; } }
		public WeightType WeightType { get; private set; }

		private List<WeightedItem<T>> items = new List<WeightedItem<T>>();
		private int totalWeight = 0;

		public WeightedList(WeightType weightType = WeightType.Normal)
		{
			WeightType = weightType;
		}
		
		public T this[int index]
		{
			get { return items[index].Item; }
		}

		public T SelectRandom()
		{
			if (WeightType == WeightType.Inverted)
				return SelectRandomInverted();
			return SelectRandomNormal();
		}

		private T SelectRandomNormal()
		{
			int maxWeight = (totalWeight != 0) ? totalWeight + 1 : items.Count;
			int value = Random.Range(0, maxWeight);
			for (int i = 0; i < items.Count; i++)
			{
				if (value <= items[i].Weight)
					return items[i].Item;
				value -= Mathf.Max(1, items[i].Weight);
			}
			return default(T);
		}

		private T SelectRandomInverted()
		{
			int maxWeight = (totalWeight != 0) ? totalWeight + 1 : items.Count;
			int invWeight = (items.Count - 1) * maxWeight;
			int value = Random.Range(0, maxWeight);
			
			for (int i = items.Count - 1; i >= 0; i--)
			{
				if (value <= invWeight)
					return items[i].Item;
				value -= Mathf.Max(1, items[i].Weight);
			}
			return default(T);
		}
		
		/*public T[] SelectMultipleRandom(int amount)
		{
			T[] result = new T[amount];
			
			for (int i = 0; i < amount; i++)
			{
				int maxWeight = (totalWeight != 0) ? totalWeight + 1 : items.Count;
				int value = Random.Range(0, maxWeight);
				for (int j = 0; j < items.Count; j++)
				{
					if (value <= items[j].Weight && !result.Contains(items[j].Item))
					{
						result[i] = items[j].Item;
						break;
					}
					value -= Mathf.Max(1, items[j].Weight);
				}
			}
			return result;
		}*/

		public void Add(T item, int weight)
		{
			if (items.Count == 0)
			{
				totalWeight = totalWeight + weight;
				items.Add(new WeightedItem<T>(item, weight));
			}
			else
				Insert(item, weight);
		}

		private void Insert(T item, int weight)
		{
			for (int i = items.Count; i > 0; i--)
			{
				if (weight <= items[i - 1].Weight)
				{
					Insert(i, item, weight);
					return;
				}
			}
			Insert(0, item, weight);
		}

		private void Insert(int index, T item, int weight)
		{
			totalWeight = totalWeight + weight;
			if (index == items.Count)
				items.Add(new WeightedItem<T>(item, weight));
			else
				items.Insert(index, new WeightedItem<T>(item, weight));
		}

		public bool Set(T item, int weight)
		{
			if (Remove(item))
			{
				Add(item, weight);
				return true;
			}
			return false;
		}

		public int GetWeight(T item)
		{
			for (int i = 0; i < items.Count; i++)
				if (items[i].Item.Equals(item))
					return items[i].Weight;
			return -1;
		}

		public void Clear()
		{
			items.Clear();
			totalWeight = 0;
		}

		public bool Contains(T item)
		{
			for (int i = 0; i < items.Count; i++)
				if (items[i].Item.Equals(item))
					return true;
			return false;
		}

		public bool Remove(T item)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i].Item.Equals(item))
				{
					totalWeight -= items[i].Weight;
					items.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public int IndexOf(T item)
		{
			for (int i = 0; i < items.Count; i++)
				if (items[i].Item.Equals(item))
					return i;
			return -1;
		}
	}
}
