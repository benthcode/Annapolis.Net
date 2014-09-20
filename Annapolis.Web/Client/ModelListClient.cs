using System.Collections.Generic;
using Newtonsoft.Json;

namespace Annapolis.Web.Client
{
    public interface IModelList : IClientModel
    {
       string TargetModelNameSpace { get; set; }
       string TargetModel { get; set; }
    }

    [JsonObject]
    public class ModelListClient<T> : ClientModel, IModelList, IList<T>
       where T : ClientModel
    {
        static ModelListClient()
        {
            RegisterModelTargetClassName(typeof(ModelListClient<T>), "ModelList");
        }

        public ModelListClient()
        {
            TargetModelNameSpace = ClientModel.GetTargetModelNameSapce(typeof(T));
            TargetModel = ClientModel.GetTargetModelName(typeof(T));
            Models = new List<T>();
        }

        public string TargetModelNameSpace { get; set; }
        public string TargetModel { get; set; }

        public IList<T> Models { get; set; }

        #region IList

        public int IndexOf(T item)
        {
            return Models.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
             Models.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Models.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return Models[index];
            }
            set
            {
                Models[index] = value;
            }
        }

        public void Add(T item)
        {
            Models.Add(item);
        }

        public void Clear()
        {
            Models.Clear();
        }

        public bool Contains(T item)
        {
            return Models.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Models.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Models.Count; }
        }

        public bool IsReadOnly
        {
            get { return Models.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return Models.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Models.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Models.GetEnumerator();
        }

        #endregion
    }
    
}

