using System;
using System.ComponentModel.DataAnnotations;

namespace TTS.Shared.Models.Job
{
    public class JobEditModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [DataType(DataType.MultilineText)]
        [MaxLength(250)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Deadline is required")]
        [DisplayFormat(DataFormatString = "{0:MMM/d/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }
        [Required(ErrorMessage = "Job status is required")]
        public Guid JobStatusId { get; set; }
    }
}