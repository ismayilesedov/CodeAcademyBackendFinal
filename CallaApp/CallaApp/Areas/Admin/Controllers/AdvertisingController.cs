﻿using CallaApp.Areas.Admin.ViewModels.Advertising;
using CallaApp.Data;
using CallaApp.Helpers;
using CallaApp.Models;
using CallaApp.Services;
using CallaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CallaApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdvertisingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IAdvertisingService _advertisingService;
        public AdvertisingController(AppDbContext context, 
                                    IWebHostEnvironment env,
                                    IAdvertisingService advertisingService)
        {
            _context = context;
            _env = env;
            _advertisingService = advertisingService;
        }
        public async Task<IActionResult> Index()
        {
            List<Advertising> advertising = await _advertisingService.GetAllAsync();
            return View(advertising);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();
            Advertising dbAdvertising = await _advertisingService.GetByIdAsync(id);
            if (dbAdvertising is null) return NotFound();

            return View(dbAdvertising);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdvertisingCreateVM advertising)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(advertising);
                }

                if (!advertising.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "File type must be image");
                    return View(advertising);
                }

                if (!advertising.Photo.CheckFileSize(600))
                {
                    ModelState.AddModelError("Photo", "Image size must be max 600kb");
                    return View(advertising);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + advertising.Photo.FileName;
                string path = FileHelper.GetFilePath(_env.WebRootPath, "assets/images", fileName);
                await FileHelper.SaveFileAsync(path, advertising.Photo);

                Advertising newAdvertising = new()
                {
                    Image = fileName,
                    Icon = advertising.Icon,
                    Name = advertising.Name,
                    Description = advertising.Description,
                };

                await _context.Advertisings.AddAsync(newAdvertising);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();
            Advertising dbAdvertising = await _advertisingService.GetByIdAsync(id);
            if (dbAdvertising is null) return NotFound();

            AdvertisingUpdateVM model = new()
            {
                Image = dbAdvertising.Image,
                Icon = dbAdvertising.Icon,
                Name = dbAdvertising.Name,
                Description = dbAdvertising.Description,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, AdvertisingUpdateVM advertising)
        {
            try
            {
                if (id is null) return BadRequest();
                Advertising dbAdvertising = await _advertisingService.GetByIdAsync(id);
                if (dbAdvertising is null) return NotFound();

                AdvertisingUpdateVM model = new()
                {
                    Image = dbAdvertising.Image,
                    Icon = dbAdvertising.Icon,
                    Name = dbAdvertising.Name,
                    Description = dbAdvertising.Description,
                };

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (advertising.Photo != null)
                {

                    if (!advertising.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "File type must be image");
                        return View(advertising);
                    }
                    if (!advertising.Photo.CheckFileSize(600))
                    {
                        ModelState.AddModelError("Photo", "Image size must be max 600kb");
                        return View(advertising);
                    }

                    string oldPath = FileHelper.GetFilePath(_env.WebRootPath, "assets/images", dbAdvertising.Image);
                    FileHelper.DeleteFile(oldPath);

                    string fileName = Guid.NewGuid().ToString() + "_" + advertising.Photo.FileName;
                    string newPath = FileHelper.GetFilePath(_env.WebRootPath, "assets/images", fileName);

                    await FileHelper.SaveFileAsync(newPath, advertising.Photo);

                    dbAdvertising.Image = fileName;
                }
                else
                {
                    AdvertisingUpdateVM newAdvertising = new()
                    {
                        Image = dbAdvertising.Image
                    };
                }

                dbAdvertising.Icon = advertising.Icon;
                dbAdvertising.Name = advertising.Name;
                dbAdvertising.Description = advertising.Description;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null) return BadRequest();
                Advertising dbAdvertising = await _advertisingService.GetByIdAsync(id);
                if (dbAdvertising is null) return NotFound();

                string path = FileHelper.GetFilePath(_env.WebRootPath, "assets/images", dbAdvertising.Image);
                FileHelper.DeleteFile(path);

                _context.Advertisings.Remove(dbAdvertising);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
                return View();
            }
        }


    }
}
