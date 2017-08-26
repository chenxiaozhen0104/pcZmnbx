namespace Repair.Web.Site
{
    public class QueryListModelBase
    {
        public QueryListModelBase()
        {
            PageInfo = new PageInfoByNum
            {
                PageSize = 10
            };
        }

        public PageInfoByNum PageInfo { get; set; }
    }
}