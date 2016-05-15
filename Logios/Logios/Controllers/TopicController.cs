﻿using Logios.Models;
using Logios.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logios.Controllers
{
    public class TopicController : Controller
    {
        private readonly ExerciseServices ExerciseService;
        private readonly TopicsService TopicService;

        public TopicController()
        {
            this.ExerciseService = new ExerciseServices();
            this.TopicService = new TopicsService();
        }

        // GET: Topic
        public ActionResult Index(int topicId)
        {
            var exercises = this.ExerciseService.GetExercisesByTopic(topicId);
            var topic = this.TopicService.GetById(topicId);
            var resultsViewModel = new ExerciseResultViewModel()
            {
                Exercises = exercises,
                TopicImageUrl = string.Concat(@"/Content/images/thumbnails/", topic.Description, ".png")
            };

            ViewBag.TopicDescription = topic.Description;

            return View(resultsViewModel);
        }
    }
}