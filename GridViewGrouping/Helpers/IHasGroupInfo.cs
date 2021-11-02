namespace GridViewGrouping.Helpers
{
    public interface IHasGroupInfo<T>
        where T : GroupInfo
    {

        public new T? GroupInfo { get; set; }

    }

}