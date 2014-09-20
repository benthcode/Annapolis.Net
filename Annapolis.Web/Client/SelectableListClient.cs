namespace Annapolis.Web.Client
{
    public class SelectableItemClient : IdenticalClientModel
    {
        public string Text { get; set; }
    }

    public interface ISelectableListClient : IModelList
    {
        bool MultiSelect { get; set; }
    }

    public class SelectableListClient<T> : ModelListClient<T>, ISelectableListClient where T : SelectableItemClient
    {
        static SelectableListClient()
        {
            RegisterModelTargetClassName(typeof(SelectableListClient<T>), "SelectableList");
        }

        public SelectableListClient()
        {
            NotifyValueChangedEvent = true;
        }

        public bool NotifyValueChangedEvent { get; set; }

        public bool MultiSelect { get; set; }
        public string SelectedValue { get; set; }


        public string Target { get; set; }
        public string Group { get; set; }

    }
}