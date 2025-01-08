using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using Test.Abstractions;
using Test.Data.Models;
using Test.Data.Repositories;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private IValidator<Contact> _validator;

        public ContactsController(IContactRepository contactRepository, IValidator<Contact> validator)
        {
            _validator = validator;
            _contactRepository = contactRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            return Ok(await _contactRepository.GetAll());
        }

        [HttpGet("clear")]
        public async Task<IActionResult> RemoveContacts()
        {
            await _contactRepository.Clear();
            await _contactRepository.Save();
            return Ok("Contacts cleared");
        }

        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateContact(Guid Id, [FromBody] Contact contact)
        {
            if (contact != null) 
            {
                var validationResult = await _validator.ValidateAsync(contact);

                if (validationResult.IsValid) 
                {
                    await _contactRepository.Update(Id, contact);
                    await _contactRepository.Save();
                    return Ok();
                }
                else
                {
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                        );

                    return ValidationProblem(new ValidationProblemDetails
                    {
                        Title = "Validation failed",
                        Status = StatusCodes.Status400BadRequest,
                        Errors = errors
                    });
                }
            }

            return BadRequest($"No data provided");
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCSV(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await _contactRepository.Clear();
                await _contactRepository.Save();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var csvData = await reader.ReadToEndAsync();
                    var lines = csvData.Split('\n');
                    if(lines.Length < 100000)
                    {
                        foreach (var line in lines)
                        {
                            Console.WriteLine(line);
                            var values = line.Split(',');

                            if (values.Length == 5)
                            {
                                var contact = new Contact
                                {
                                    Name = values[0].Trim('"'),
                                    DateOfBirth = DateTime.ParseExact(values[1].Trim('"'), "dd.MM.yyyy", CultureInfo.InvariantCulture),
                                    Married = bool.Parse(values[2].Trim('"')),
                                    Phone = values[3].Trim('"'),
                                    Salary = decimal.Parse(values[4].Trim('"'))
                                };

                                var validationResult = await _validator.ValidateAsync(contact);

                                if (validationResult.IsValid)
                                {
                                    await _contactRepository.Add(contact);
                                }
                                else
                                {
                                    var errors = validationResult.Errors
                                        .GroupBy(e => e.PropertyName)
                                        .ToDictionary(
                                        g => g.Key,
                                        g => g.Select(e => e.ErrorMessage).ToArray()
                                        );

                                    return ValidationProblem(new ValidationProblemDetails
                                    {
                                        Title = "Validation failed",
                                        Status = StatusCodes.Status400BadRequest,
                                        Errors = errors
                                    });
                                }
                            }
                        }
                        await _contactRepository.Save();
                        return Ok("File uploaded successfully");
                    }
                    else
                    {
                        return BadRequest("File should contain be less than 100000 contacts and it's can't be empty");
                    }
                }
                
            }
            else
            {
                return BadRequest("No file uploaded");
            }
        }

        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteContact(Guid Id)
        {
            await _contactRepository.Delete(Id);
            await _contactRepository.Save();
            return Ok();
        }
    }
}
