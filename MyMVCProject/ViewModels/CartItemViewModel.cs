namespace MyMVCProject.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string UserName { get; set; }
        public string Address { get; set; }
    }
}
