﻿using Logios.DTOs;
using Logios.Entities;
using Logios.Models;
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
            var topics = context.Topics.ToList();            

            return new SelectList(topics, "TopicId", "Description");
        }

        public Boolean? CreateNewExercise(CreateExerciseViewModel model)
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
            var topic = new TopicDTO();

            exercise.Problem = exerciseCreated.Problem;
            exercise.Development = exerciseCreated.Development;
            exercise.Solution = exerciseCreated.Solution;
            exercise.Description = exerciseCreated.Description;            
            topic.TopicId = exerciseCreated.Topic.TopicId;
            topic.Description = exerciseCreated.Topic.Description;

            model.Exercise = exercise;
            model.Topic = topic;
            model.ComboTopics = this.GetAllTopics();

            return model;
        }

        public Boolean? EditCurrentExercise(int id, EditExerciseViewModel model)
        {
            try
            {                
                Exercise exerciseToEdit = context.Exercises.FirstOrDefault(e => e.ExerciseId == id);
                exerciseToEdit.Problem = model.Exercise.Problem;
                exerciseToEdit.Development = model.Exercise.Development;
                exerciseToEdit.Solution = model.Exercise.Solution;
                exerciseToEdit.Description = model.Exercise.Description;
                exerciseToEdit.Topic = context.Topics.FirstOrDefault(t => t.TopicId == model.Topic.TopicId);
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
    }
}