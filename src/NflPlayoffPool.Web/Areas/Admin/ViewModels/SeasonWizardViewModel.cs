// <copyright file="SeasonWizardViewModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using NflPlayoffPool.Data.Models;
    using NflPlayoffPool.Web.Areas.Admin.Models;

    public class SeasonWizardViewModel
    {
        public int CurrentStep { get; set; } = 1;
        public int TotalSteps { get; } = 4;
        public string? SeasonId { get; set; }
        
        // Step 1: Basic Info
        [Required(ErrorMessage = "Year is required")]
        [Range(2020, 2050, ErrorMessage = "Year must be between 2020 and 2050")]
        public int Year { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Cutoff date is required")]
        [Display(Name = "Bracket Submission Cutoff")]
        public DateTime CutoffDateTime { get; set; }
        
        // Step 2: Scoring Rules
        [Required]
        [Range(1, 20, ErrorMessage = "Wildcard points must be between 1 and 20")]
        [Display(Name = "Wildcard Round Points")]
        public int WildcardPoints { get; set; } = 1;
        
        [Required]
        [Range(1, 20, ErrorMessage = "Divisional points must be between 1 and 20")]
        [Display(Name = "Divisional Round Points")]
        public int DivisionalPoints { get; set; } = 2;
        
        [Required]
        [Range(1, 20, ErrorMessage = "Conference points must be between 1 and 20")]
        [Display(Name = "Conference Championship Points")]
        public int ConferencePoints { get; set; } = 3;
        
        [Required]
        [Range(1, 20, ErrorMessage = "Super Bowl points must be between 1 and 20")]
        [Display(Name = "Super Bowl Points")]
        public int SuperBowlPoints { get; set; } = 5;
        
        // Step 3: Teams Setup
        public TeamSetupMethod TeamSetupMethod { get; set; } = TeamSetupMethod.UseTemplate;
        public List<TeamWizardModel> Teams { get; set; } = new();
        
        // Navigation helpers
        public bool CanGoNext => CurrentStep < TotalSteps;
        public bool CanGoPrevious => CurrentStep > 1;
        public bool IsLastStep => CurrentStep == TotalSteps;
        
        public string GetStepTitle()
        {
            return CurrentStep switch
            {
                1 => "Basic Information",
                2 => "Scoring Rules",
                3 => "Teams Setup",
                4 => "Review & Create",
                _ => "Unknown Step"
            };
        }
        
        public string GetStepDescription()
        {
            return CurrentStep switch
            {
                1 => "Set up the basic details for your NFL playoff pool season",
                2 => "Configure how many points each playoff round is worth",
                3 => "Add the 32 NFL teams and their playoff seedings",
                4 => "Review your settings and create the season",
                _ => ""
            };
        }
        
        public SeasonModel ToSeasonModel()
        {
            return new SeasonModel
            {
                Id = SeasonId,
                Year = Year.ToString(),
                Description = Description,
                CutoffDateTime = CutoffDateTime,
                WildcardPoints = WildcardPoints,
                DivisionalPoints = DivisionalPoints,
                ConferencePoints = ConferencePoints,
                SuperBowlPoints = SuperBowlPoints,
                Status = SeasonStatus.NotStarted,
                CurrentRound = PlayoffRound.Wildcard,
                IsCurrent = false // Will be set manually after creation
            };
        }
    }
    
    public enum TeamSetupMethod
    {
        UseTemplate,
        ImportFile,
        ManualEntry
    }
    
    public class TeamWizardModel
    {
        public string? Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(10)]
        public string Abbreviation { get; set; } = string.Empty;
        
        [Required]
        public Conference Conference { get; set; }
        
        [Required]
        [Range(1, 7, ErrorMessage = "Seed must be between 1 and 7")]
        public int Seed { get; set; }
        
        [StringLength(50)]
        public string? City { get; set; }
    }
}