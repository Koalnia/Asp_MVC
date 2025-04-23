using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Asp_MVC.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Asp_MVC.Dto
{
    public class AdvertisementCreationDto
    {
        public long Id;

        [Required(ErrorMessage = "Tytuł ogłoszenia nie może być pusty")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Tytuł ogłoszenia musi zawierać od 8 do 50 znaków")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis ogłoszenia nie może być pusty")]
        [StringLength(100, MinimumLength = 9, ErrorMessage = "Opis ogłoszenia musi zawierać od 9 do 100 znaków")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cena ogłoszenia nie może być pusta")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Cena ogłoszenia musi zawierać od 1 do 50 znaków")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Czas trwania ogłoszenia nie może być pusty")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Czas trwania ogłoszenia musi zawierać od 4 do 50 znaków")]
        public string Duration { get; set; }

        public AdvertisementCreationDto() { }

        public AdvertisementCreationDto(Advertisement advertisement)
        {
            Title = advertisement.Title;
            Description = advertisement.Description;
            Price = advertisement.Price;
            Duration = advertisement.Duration;
        }
        
    }

}
