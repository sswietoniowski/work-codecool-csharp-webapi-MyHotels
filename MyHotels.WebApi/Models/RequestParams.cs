namespace MyHotels.WebApi.Models;

public class RequestParams
{
    private const int MAX_PAGE_SIZE = 50;
    private int _pageSize = MAX_PAGE_SIZE;

    public int PageNumber { get; set; } = 1;
    public int PageSize {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value <= MAX_PAGE_SIZE) ? value : MAX_PAGE_SIZE;
        }
    }
}