﻿using Logios.Models;
using Logios.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logios.Controllers
{
    public class AdministratorController : Controller
    {
        AdministratorServices adminServices = new AdministratorServices();

        // GET: Administrator
        public ActionResult ControlPanel()
        {
            return View();
        }

        public ActionResult AddExercise()
        {
            var model = new CreateExerciseViewModel();
            model.ComboTopics = adminServices.GetAllTopics();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddExercise(CreateExerciseViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = adminServices.CreateNewExercise(model, User.Identity.GetUserId());
                    return PartialView("_CreateExerciseResult", result);
                }

                throw new Exception();
            }            
            catch
            {                
                return View(model);
            }            
        }

        public ActionResult EditExercise(int id)
        {                        
            return View(adminServices.GetExerciseDataCreated(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditExercise(int id, EditExerciseViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var result = adminServices.EditCurrentExercise(id, model, User.Identity.GetUserId());
                    return PartialView("_EditExerciseResult", result);                    
                }

                throw new Exception();
            }
            catch
            {
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult DeleteExercise(int? id, int? test)
        {
            return PartialView("_DeleteForm", id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteExercise(int? id)
        {
            TempData["result"] = adminServices.DeleteExerciseFromDB(id);

            return RedirectToAction("ControlPanel", "Administrator");
            //return View("ControlPanel");
        }
    }
}