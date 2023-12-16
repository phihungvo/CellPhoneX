using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CellPhoneX_2100009273.Models;
using PagedList;
using PagedList.Mvc;

namespace CellPhoneX_2100009273.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private DBContext db = new DBContext();

        // GET: Admin/Products
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(await products.ToListAsync());
        }

        //public ActionResult Index(int? page)
        //{
        //    int pageSize = 10; // Số lượng sản phẩm trên mỗi trang
        //    int pageNumber = (page ?? 1);
        //    var products = db.Products.OrderBy(p => p.Category).Include(p=>p.CatId).ToPagedList(pageNumber, pageSize);
        //    return View(products);
        //}



        // GET: Admin/Products/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CatId = new SelectList(db.Categories, "Id", "CatName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProId,ProName,ProImage,ProPrice,CatId,isNew,bestSelling,stock_quantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CatId = new SelectList(db.Categories, "Id", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatId = new SelectList(db.Categories, "Id", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProId,ProName,ProImage,ProPrice,CatId,isNew,bestSelling,stock_quantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CatId = new SelectList(db.Categories, "Id", "CatName", product.CatId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
