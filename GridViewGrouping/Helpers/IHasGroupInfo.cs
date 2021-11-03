namespace GridViewGrouping.Helpers
{
    public interface IHasGroupInfo<T>
        where T : GroupInfo
    {

        public T? GroupInfo { get; set; }

    }

}