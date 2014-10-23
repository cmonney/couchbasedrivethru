namespace CouchbaseDriveThru.Controllers
{
    using System.Web.Mvc;
    using Models;

    public class HomeController : Controller
    {
        public HomeController()
        {
            CustomerRepository = new CustomerRepository();
        }

        public CustomerRepository CustomerRepository { get; set; }

        public ActionResult Index()
        {
            var customers = CustomerRepository.GetAll();
            return View(customers);
        }

        public ActionResult Create()
        {
            var customer = new Customer();
            return View(customer);
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                CustomerRepository.Create(customer);

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        public ActionResult Edit(string id)
        {
            var customer = CustomerRepository.Get(id);
            return View(customer);
        }

        [HttpPost]
        public ActionResult Edit(string id, Customer customer)
        {
            if (ModelState.IsValid)
            {
                CustomerRepository.Update(customer);

                return RedirectToAction("Index");
            }

            return View(customer);
        }

        public ActionResult Details(string id)
        {
            var customer = CustomerRepository.Get(id);
            return View(customer);
        }

        public ActionResult Delete(string id)
        {
            CustomerRepository.Delete(id);
            return RedirectToAction("Deleted");
        }

        public ActionResult Deleted()
        {
            return View();
        }
    }
}