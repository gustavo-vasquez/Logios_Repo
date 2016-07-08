﻿using Logios.DTOs;
using Logios.Entities;
using Logios.Models;
using Logios.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logios.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ExerciseServices ExerciseService = new ExerciseServices();
        private TopicsService TopicService = new TopicsService();

        public ActionResult Index()
        {            
            return View();
        }

        [HttpPost]
        public PartialViewResult Search(string topicDescription, bool showResolvedExercises = false)
        {
            var resultsViewModel = new ExerciseResultViewModel();

            if (!string.IsNullOrEmpty(topicDescription))
            {
                var topic = TopicService.GetByDescription(topicDescription);
                var exercises = ExerciseService.GetExercisesByTopic(topicDescription);
                var userId = User.Identity.GetUserId();

                resultsViewModel.Exercises = ExerciseService.GetExerciseDTOsCards(userId, topic.TopicId, showResolvedExercises);                
                resultsViewModel.TopicImageUrl = string.Concat(@"/Content/images/thumbnails/", topic.Description.Replace(' ', '_'), ".png");
            }

            return PartialView("_ExerciseSearchResult", resultsViewModel);
        }

        public JsonResult GetTopics()
        {
            var topics = this.TopicService.GetAll();
            return Json(topics, JsonRequestBehavior.AllowGet);
        }
    }
}