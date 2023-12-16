using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CellPhoneX_2100009273.Models;


namespace CellPhoneX_2100009273.Controllers
{
    public class ShoppingCartController : Controller
    {
        private DBContext dBContext = new DBContext();
        private string strCart = "Cart";

        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult OrderNow(int? Id)
        {
            if (Id == null) //Kiểm tra ID có tồn tại hay không
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Session[strCart] == null)  // Kiểm tra xem Session đã lưu trữ giỏ hàng chưa.
            {
                List<Cart> ListCart = new List<Cart> // Khởi tạo danh sách Cart
                {
                  new Cart(dBContext.Products.Find(Id), 1) // thêm một cart mới vào session với số lượng là 1.
                };
                Session[strCart] = ListCart; 
            }
            else
            {
                List<Cart> ListCart = (List<Cart>)Session[strCart];
                int check = IsExistingCheck(Id); //Kiểm tra sản phẩm đã tồn tại trong giỏ hàng hay chưa
                if (check == -1)
                    ListCart.Add(new Cart(dBContext.Products.Find(Id), 1));
                else
                    ListCart[check].Quantity++;  // nếu đã tồn tại thì tăng số lượng thêm 1
                Session[strCart] = ListCart;
            }
            return RedirectToAction("Index");
        }


        private int IsExistingCheck(int? Id)
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                if (ListCart[i].Product.ProId == Id)
                    return i;
            }
            return -1;
        }


        public ActionResult RemoveItem(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int check = IsExistingCheck(Id);
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            ListCart.RemoveAt(check);
            if (ListCart.Count == 0)
            {
                Session[strCart] = null;
            }
            else
            {
                Session[strCart] = ListCart;
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UpdateCart(FormCollection field)
        {
            string[] quantities = field.GetValues("quantity");
            List<Cart> ListCart = (List<Cart>)Session[strCart];
            for (int i = 0; i < ListCart.Count; i++)
            {
                ListCart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            Session[strCart] = ListCart;
            return RedirectToAction("Index");
        }



        public ActionResult ClearCart()
        {
            Session[strCart] = null;
            return RedirectToAction("Index");
        }


        public ActionResult CheckOut()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ProcessOrder(FormCollection field)
        {
            List<Cart> ListCart = (List<Cart>)Session[strCart];

            //1.Save the order into Order table
            var order = new CellPhoneX_2100009273.Models.Order()
            {
                //Tạo một đối tượng đơn hàng mới và gán các thông tin khách hàng 
                CustomerName = field["cusName"],
                CustomerPhone = field["cusPhone"],
                CustomerEmail = field["cusEmail"],
                CustomerAddress = field["cusAddress"],
                OrderDate = DateTime.Now,
                PaymentType = "Cash",
                Status = "Processing"
            };
            dBContext.Orders.Add(order);
            dBContext.SaveChanges();


            //2. SAVE THE ORDER DETAIL INTO ORDER DATAIL TABLE
            foreach (Cart cart in ListCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderId = order.OrderId,
                    ProductId = cart.Product.ProId,
                    Quantity = Convert.ToInt32(cart.Quantity),
                    Price = Convert.ToDouble(cart.Product.ProPrice)
                };
                dBContext.OrderDetails.Add(orderDetail);
                dBContext.SaveChanges();
            }


            //3. Remove shopping cart session
            Session.Remove(strCart);
            return View("OrderSuccess");
        }
    }
}