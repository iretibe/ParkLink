namespace ParkLink.Users.Dtos.Users
{
    public sealed class UserQueryParameters
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 20;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize =
                value <= 0
                    ? 20
                    : Math.Min(value, MaxPageSize);
        }

        public string? Search { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
