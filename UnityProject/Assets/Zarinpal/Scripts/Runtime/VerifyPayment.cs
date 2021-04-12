namespace ZarinpalIAB
{
    public class VerifyPayment
    {
        public string RefID { get; private set; }
        public string ProductID { get; private set; }


        public VerifyPayment(string refid, string productID)
        {
            RefID = refid;
            ProductID = productID;
        }

        public override string ToString()
        {
            return string.Format("RefID:{0},ProductID:{1}", RefID, ProductID);
        }
    }
}
