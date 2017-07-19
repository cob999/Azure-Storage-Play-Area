using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AzureStorageBlobPlayArea.Services;

namespace AzureStorageBlobPlayArea.Controllers
{
    public class ImagesController : Controller
    {
        private ImageStorageService _imageStoreService;

        public ImagesController()
        {
            _imageStoreService = new ImageStorageService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase image)
        {
            if (image != null)
            {
                string imageId = await _imageStoreService.SaveImage(image.InputStream);
                return RedirectToAction("ShowImage", new {id = imageId});
            }

            return View("Index");
        }

        public ActionResult ShowImage(string id)
        {
            Uri imageUri = _imageStoreService.GetResourceUri(id);

            return View(imageUri);
        }
    }
}