﻿using Logios.DTOs;
using Logios.Entities;
using Logios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Logios.Services
{
    public class TopicAreaService
    {
        private ExerciseServices ExerciseServices;

        public TopicAreaService()
        {
            this.ExerciseServices = new ExerciseServices();
        }

        public TopicAreaViewModel GetAll()
        {
            var result = new TopicAreaViewModel();

            using (var context = new ApplicationDbContext())
            {
                var topicAreas = context.TopicAreas.OrderBy(x => x.Description).ToList();
                var topicAreaTopics = context.TopicAreaTopics.ToList();
                var topics = context.Topics.ToList();

                foreach (var topicArea in topicAreas)
                {
                    var topicsForThisArea = topicAreaTopics
                                               .Where(tat => tat.TopicAreaId == topicArea.TopicAreaId)
                                               .Select(tat => tat.Topic)
                                               .OrderBy(t => t.Description);

                    var topicAreaName = topicArea.Description;

                    result.TopicsByArea.Add(topicAreaName, topicsForThisArea);
                }
            }

            return result;
        }

        public ExercisesByAreaViewModel GetExercisesByTopicArea(string userId, string topicAreaDescription)
        {
            var result = new ExercisesByAreaViewModel();

            using (var context = new ApplicationDbContext())
            {
                var topicArea = context.TopicAreas.FirstOrDefault(ta => ta.Description == topicAreaDescription);

                var topicsForThisArea = context.TopicAreaTopics
                                                   .Where(tat => tat.TopicAreaId == topicArea.TopicAreaId)
                                                   .Select(tat => tat.Topic)
                                                   .ToList();

                result.Topics = topicsForThisArea;

                var exerciseResultViewModels = new List<ExerciseResultViewModel>();

                foreach (var topic in result.Topics)
                {
                    var newViewModel = new ExerciseResultViewModel();
                    newViewModel.Exercises = this.ExerciseServices.GetExerciseDTOsCards(userId, topic.TopicId,true);

                    newViewModel.TopicImageUrl = string.Concat("/Content/images/thumbnails/", topic.Description.Replace(' ', '_'), ".png");

                    exerciseResultViewModels.Add(newViewModel);
                }

                result.ExerciseResultViewModels = exerciseResultViewModels;

                return result;
            }
        }

        public List<TopicArea> GetTopicAreas()
        {
            using (var context = new ApplicationDbContext())
            {
                return context.TopicAreas.Where(x => x.IsDeleted == false).ToList();
            }
        }

        public bool AreaExists(string description)
        {
            using (var context = new ApplicationDbContext())
            {
                if(context.TopicAreas.FirstOrDefault(x => x.Description.ToLower() == description.ToLower()) == null)
                {
                    return false;
                }

                return true;
            }            
        }

        public bool AreaExists(string description, int topicAreaId)
        {
            using (var context = new ApplicationDbContext())
            {
                if (context.TopicAreas.FirstOrDefault(x => x.Description.ToLower() == description.ToLower() && x.TopicAreaId != topicAreaId) == null)
                {
                    return false;
                }

                return true;
            }
        }

        public void CreateNewArea(TopicAreaDTO model)
        {
            using (var context = new ApplicationDbContext())
            {
                TopicArea topicArea = new TopicArea();
                topicArea.TopicAreaId = context.TopicAreas.Count() + 1;
                topicArea.Description = model.Description;
                context.TopicAreas.Add(topicArea);
                context.SaveChanges();
            }
        }

        public TopicAreaDTO GetTopicAreaById(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                TopicAreaDTO topicAreaDto = new TopicAreaDTO();
                var topicAreaDB = context.TopicAreas.FirstOrDefault(x => x.TopicAreaId == id);
                topicAreaDto.TopicAreaId = topicAreaDB.TopicAreaId;
                topicAreaDto.Description = topicAreaDB.Description;

                return topicAreaDto;
            }
        }
        
        public void EditThisArea(TopicAreaDTO model)
        {
            using (var context = new ApplicationDbContext())
            {
                TopicArea topicAreaToEdit = context.TopicAreas.FirstOrDefault(x => x.TopicAreaId == model.TopicAreaId);
                topicAreaToEdit.Description = model.Description;

                context.SaveChanges();
            }                
        }

        public bool CanDeleteArea(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                try
                {
                    context.TopicAreaTopics.First(x => x.TopicAreaId == id);
                }
                catch
                {
                    return true;
                }

                var topicsNotDeleted = context.TopicAreaTopics.Join(context.Topics, a => a.TopicId, t => t.TopicId,
                                    (a,t) => new { TopicAreaTopic = a, Topic = t }).Where(c => c.TopicAreaTopic.TopicAreaId == id && !c.Topic.IsDeleted);

                if (topicsNotDeleted.Count() > 0)
                    return false;
                else
                    return true;
            }
        }

        public void DeleteArea(int id)
        {
            using (var context = new ApplicationDbContext())
            {
                var areaToDelete = context.TopicAreas.FirstOrDefault(x => x.TopicAreaId == id);
                areaToDelete.IsDeleted = true;

                context.SaveChanges();
            }
        }
    }
}