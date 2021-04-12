namespace ZarinpalIAB
{
    public class Purchase
    {
        public string Authority { get; private set; }
        public string ProductID { get; private set; }


        public Purchase(string authority, string productID)
        {
            Authority = authority;
            ProductID = productID;
        }

        public override string ToString()
        {
            return string.Format("Authority:{0},ProductID:{1}", Authority, ProductID);
        }
    }
}
