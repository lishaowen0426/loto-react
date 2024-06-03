namespace Loto.Models
{
    public class BreadcrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; } = false;
    }

    public class BreadcrumbViewModel
    {
        public List<BreadcrumbItem> Items { get; set; } = new List<BreadcrumbItem>();
    }

}
