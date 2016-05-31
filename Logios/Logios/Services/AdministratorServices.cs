﻿using Logios.DTOs;
using Logios.Entities;
using Logios.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Logios.Services
{
    public class AdministratorServices
    {
        private ApplicationDbContext context = new ApplicationDbContext();

        private void Refresh()
        {
            this.context.Dispose();
            this.context = new ApplicationDbContext();
        }

        public IEnumerable<SelectListItem> GetAllTopics()
        {
            var topics = context.Topics.Where(t => t.IsDeleted == false).ToList();

            return new SelectList(topics, "TopicId", "Description");
        }

        public IEnumerable<ExercisesPanelViewModel> GetAllExercises()
        {            
            var exerciseData = from e in context.Exercises
                               join t in context.Topics on e.Topic.TopicId equals t.TopicId
                               join u in context.Users on e.User.Id equals u.Id
                               select new ExercisesPanelViewModel { ExerciseId = e.ExerciseId, Description = e.Description, IsDeleted = e.IsDeleted, TopicName = t.Description, UserName = u.UserName };

            return exerciseData;
        }

        public IEnumerable<ExercisesPanelViewModel> SearchExercisesByTopic(string topicDescription)
        {            
            var exerciseData = this.GetAllExercises();

            if(topicDescription != "")
            {
                var exercisesFiltered = exerciseData.Where(ed => ed.TopicName.ToLower() == topicDescription.ToLower()).ToList();
                return exercisesFiltered;
            }

            return exerciseData;
        }

        public Boolean? CreateNewExercise(CreateExerciseViewModel model, string userId)
        {
            try
            {
                Exercise exercise = new Exercise();
                exercise.ExerciseId = context.Exercises.Count() + 1;
                exercise.Problem = model.Exercise.Problem;
                exercise.Development = model.Exercise.Development;
                exercise.Solution = model.Exercise.Solution;
                exercise.Description = model.Exercise.Description;                
                exercise.Topic = context.Topics.FirstOrDefault(t => t.TopicId == model.Topic.TopicId);
                exercise.User = context.Users.FirstOrDefault(u => u.Id == userId);
                context.Exercises.Add(exercise);
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public EditExerciseViewModel GetExerciseDataCreated(int exerciseId)
        {
            var exerciseCreated = context.Exercises.FirstOrDefault(e => e.ExerciseId == exerciseId);
            var model = new EditExerciseViewModel();
            var exercise = new ExerciseDTO();
            var topic = new Topic();

            exercise.Problem = exerciseCreated.Problem;
            exercise.Development = exerciseCreated.Development;
            exercise.Solution = exerciseCreated.Solution;
            exercise.Description = exerciseCreated.Description;            
            topic.TopicId = exerciseCreated.Topic.TopicId;            

            model.Exercise = exercise;
            model.Topic = topic;
            model.ComboTopics = this.GetAllTopics();

            return model;
        }

        public Boolean? EditCurrentExercise(int id, EditExerciseViewModel model, string userId)
        {
            try
            {                
                Exercise exerciseToEdit = context.Exercises.FirstOrDefault(e => e.ExerciseId == id);
                exerciseToEdit.Problem = model.Exercise.Problem;
                exerciseToEdit.Development = model.Exercise.Development;
                exerciseToEdit.Solution = model.Exercise.Solution;
                exerciseToEdit.Description = model.Exercise.Description;
                exerciseToEdit.Topic = context.Topics.FirstOrDefault(t => t.TopicId == model.Topic.TopicId);
                exerciseToEdit.User = context.Users.FirstOrDefault(u => u.Id == userId);
                context.SaveChanges();                

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Boolean? DeleteExerciseFromDB(int? id)
        {
            try
            {
                var exerciseToDelete = context.Exercises.FirstOrDefault(e => e.ExerciseId == id);
                exerciseToDelete.IsDeleted = true;
                //context.Exercises.Remove(exerciseToDelete);
                context.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<ApplicationUser> SortSelectListBy(string filter, ApplicationUserManager UserManager)
        {
            List<ApplicationUser> ordenedList;

            if(filter == "asc")
            {
               ordenedList = UserManager.Users.OrderBy(u => u.UserName).ToList();
            }            
            else
            {
                ordenedList = UserManager.Users.OrderByDescending(u => u.UserName).ToList();
            }

            return ordenedList;
        }

        public List<Topic> GetTopicsCreated()
        {
            return context.Topics.Where(t => t.IsDeleted == false).ToList();
        }

        public void CreateNewTopic(string description)
        {
            try
            {
                Topic topic = new Topic();
                topic.TopicId = context.Topics.Count() + 1;
                topic.Description = description;

                context.Topics.Add(topic);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public TopicDTO GetTopicById(int id)
        {
            TopicDTO topicToEdit = new TopicDTO();
            Topic topicDB = context.Topics.FirstOrDefault(t => t.TopicId == id);
            topicToEdit.TopicId = topicDB.TopicId;
            topicToEdit.Description = topicDB.Description;

            return topicToEdit;
        }

        public bool CheckTopicExist(string description)
        {
            if(context.Topics.FirstOrDefault(t => t.Description == description) == null)
            {
                return false;
            }

            return true;
        }

        public void EditThisTopic(TopicDTO model)
        {            
            Topic topicToEdit = context.Topics.FirstOrDefault(t => t.TopicId == model.TopicId);
            topicToEdit.Description = model.Description;
            context.SaveChanges();            
        }

        public void DeleteTopic(int? id)
        {
            Topic topicToDelete = context.Topics.FirstOrDefault(t => t.TopicId == id);
            topicToDelete.IsDeleted = true;
            context.SaveChanges();
        }
    }
}