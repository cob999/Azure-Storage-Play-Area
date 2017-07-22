using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AzureStorageBlobPlayArea.Models;
using AzureStorageBlobPlayArea.Services;
using Microsoft.WindowsAzure.Storage.Blob;

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
            List<CloudBlockBlob> blobs = _imageStoreService.GetImages();

            return View(blobs);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase image)
        {
            if (image != null)
            {
                string imageId = await _imageStoreService.SaveImage(image.InputStream);
                return RedirectToAction("ShowImage", new { id = imageId });
            }

            return View("Index");
        }

        public ActionResult ShowImage(string id)
        {
            // use this for container that has access type set to Blob
            // Uri imageUri = _imageStoreService.GetResourceUri(id);

            var viewModel = new ImageViewerViewModel()
            {
               Uri = _imageStoreService.GetResourceUriWithSas(id),
               ResourceId = id
            };

            return View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteImage(string id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            await _imageStoreService.DeleteImage(id);

            return RedirectToAction("Index");
        }
    }
}