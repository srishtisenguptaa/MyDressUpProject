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
        public List<UserAddressViewModel> Addresses { get; set; }
        //public string UserName { get; set; }
        //public string Address { get; set; }
    }

    //public class CartPageViewModel
    //{
    //    public List<CartItemViewModel> CartItems { get; set; }
    //    public List<UserAddressViewModel> Addresses { get; set; }
    //}

    public class UserAddressViewModel
    {
        public int AddressId { get; set; }
        public string UserName { get; set; }
        public string FullAddress { get; set; }
    }

}
