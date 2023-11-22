namespace WebBanDoAn.Extensions
{
    public class Pagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalCount { get; set; }
        public int TotalPage
        {
            get
            {
                if (PageNumber == 0) return 0;
                var total = TotalCount / PageSize;
                if (TotalCount % PageSize == 0) return total;
                else
                {
                    total++;
                    return total;
                }
            }
        }
    }
}
